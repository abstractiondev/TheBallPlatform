using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace TheBall.Interface
{
    public class ExecuteInterfaceOperationsByOwnerAndReleaseLockImplementation
    {
        public static async Task ExecuteMethod_ExecuteOperationsAndReleaseLockAsync(string instanceName, string lockedOwnerPrefix, string lockedOwnerID, string[] operationIDs, string[] operationQueueItems, string lockBlobFullPath)
        {
            var executionOwner = new VirtualOwner(lockedOwnerPrefix,
                lockedOwnerID);
            try
            {
                // TODO: Fetch owner specific initializing with dependency injection
                var finalizingActions = getOwnerFinalizingActions();
                InformationContext.InitializeToLogicalContext(executionOwner, instanceName, finalizingActions);
                foreach (var operationID in operationIDs)
                {
                    try
                    {
                        await
                            ExecuteInterfaceOperation.ExecuteAsync(new ExecuteInterfaceOperationParameters
                            {
                                OperationID = operationID
                            });
                    }
                    catch (Exception exception)
                    {
                        // mark operation as error and continue
                    }

                }
                await InformationContext.ExecuteAsOwnerAsync(SystemOwner.CurrentSystem, async () =>
                {
                    await StorageSupport.DeleteBlobsAsync(operationQueueItems);
                    var lockFullName = lockBlobFullPath;
                    await StorageSupport.ReleaseLogicalLockByDeletingBlobAsync(lockFullName);
                });
            }
            finally
            {
                await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
            }
        }


        private static FinalizingDependencyAction[] ownerDependencyActions = null;

        private static FinalizingDependencyAction[] getOwnerFinalizingActions()
        {
            if (ownerDependencyActions != null)
                return ownerDependencyActions;
            FinalizingDependencyAction[] masterCollectioActions = getMasterCollectionRefreshActions();
            FinalizingDependencyAction[] customDependencyActions = getCustomDependencyActions();
            /*
            var result = new[]
            {
                new FinalizingDependencyAction(typeof (AaltoGlobalImpact.OIP.TextContentCollection), new Type[]
                {
                    typeof (AaltoGlobalImpact.OIP.TextContent)
                }, async type =>
                {
                    var masterCollection =
                        AaltoGlobalImpact.OIP.TextContentCollection.GetMasterCollectionInstance(
                            InformationContext.CurrentOwner);
                    masterCollection.RefreshContent();
                    await masterCollection.StoreInformationAsync();
                }),
            };*/
            ownerDependencyActions = masterCollectioActions.Concat(customDependencyActions).ToArray();
            return ownerDependencyActions;
        }

        private static FinalizingDependencyAction[] getCustomDependencyActions()
        {
            return new[]
            {
                new FinalizingDependencyAction(typeof(AaltoGlobalImpact.OIP.NodeSummaryContainer),
                new[]
                {
                    typeof(AaltoGlobalImpact.OIP.RenderedNodeCollection),
                    typeof(AaltoGlobalImpact.OIP.TextContentCollection),
                    typeof(AaltoGlobalImpact.OIP.LinkToContentCollection),
                    typeof(AaltoGlobalImpact.OIP.EmbeddedContentCollection),
                    typeof(AaltoGlobalImpact.OIP.ImageCollection),
                    typeof(AaltoGlobalImpact.OIP.BinaryFileCollection),
                    typeof(AaltoGlobalImpact.OIP.CategoryCollection)
                }, async types =>
                {
                    IInformationObject nodeSummaryContainer =
                        await
                            ObjectStorage.RetrieveFromOwnerContentA<AaltoGlobalImpact.OIP.NodeSummaryContainer>(InformationContext.CurrentOwner, "default");
                    foreach (var type in types)
                    {
                        var getInstanceMethod = type.GetMethod("GetMasterCollectionInstance");
                        if (getInstanceMethod == null)
                            continue;
                        IInformationCollection masterObject =
                            (IInformationCollection)
                                getInstanceMethod.Invoke(null, new object[] {InformationContext.CurrentOwner});
                        nodeSummaryContainer.UpdateCollections(masterObject);
                    }
                    await nodeSummaryContainer.StoreInformationAsync();
                }), 
            };
        }

        private static FinalizingDependencyAction[] getMasterCollectionRefreshActions()
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var iObjType = typeof (IInformationObject);
            var allInformationObjects = allTypes.Where(type => iObjType.IsAssignableFrom(type)).ToArray();
            var noCollectionObjects =
                allInformationObjects.Where(type => type.Name.EndsWith("Collection") == false).ToArray();
            var collectionObjects =
                allInformationObjects.Where(type => type.Name.EndsWith("Collection")).ToArray();
            var matchingTypePairs = collectionObjects.Select(collType =>
            {
                var noCollectionTypeName = collType.Name.Replace("Collection", "");
                var noCollectionType = noCollectionObjects.FirstOrDefault(item => item.Namespace == collType.Namespace &&  item.Name == noCollectionTypeName);
                return new
                {
                    SingleType = noCollectionType,
                    CollectionType = collType
                };
            }).Where(item => item.SingleType != null).ToArray();
            var actions = matchingTypePairs.Select(pair =>
            {
                var collType = pair.CollectionType;
                return new FinalizingDependencyAction(pair.CollectionType, new []{ pair.SingleType }, async types =>
                {
                    var getInstanceMethod = collType.GetMethod("GetMasterCollectionInstance");
                    IInformationCollection masterObject = (IInformationCollection) getInstanceMethod.Invoke(null, new object[] { InformationContext.CurrentOwner});
                    masterObject.RefreshContent();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterObject);
                    //var masterCollection = TextContentCollection.GetMasterCollectionInstance(CurrentOwner); 
                    //masterCollection.RefreshContent(); 
                    //masterCollection.StoreInformation(); 

                });
            }).ToArray();
            return actions;
        }
    }
}