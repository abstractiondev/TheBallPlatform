using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;
using TheBall.CORE;
using TheBall.Index;
using Category = AaltoGlobalImpact.OIP.Category;
using CategoryCollection = AaltoGlobalImpact.OIP.CategoryCollection;
using OIPDomain = AaltoGlobalImpact.OIP.DomainInformationSupport;
using CoreDomain = TheBall.CORE.DomainInformationSupport;
using Process = TheBall.CORE.Process;

namespace TheBallTool
{
    public static class DataPatcher
    {
        public static void SetAllInvitedViewerMembersAsFullCollaborators()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach(var acctID in accountIDs)
            {
                TBRAccountRoot accountRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRAccountRoot>(acctID);
                TBAccount account = accountRoot.Account;
                foreach(var grpRole in account.GroupRoleCollection.CollectionContent)
                {
                    if (TBCollaboratorRole.IsRoleStatusValidMember(grpRole.RoleStatus) == false)
                        grpRole.RoleStatus = TBCollaboratorRole.RoleStatusMemberValue;
                    if (grpRole.GroupRole == TBCollaboratorRole.ViewerRoleValue)
                        grpRole.GroupRole = TBCollaboratorRole.CollaboratorRoleValue;
                }
                account.StoreAccountToRoot();
            }

        }

        public static void EnsureAndRefreshMasterCollections()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach (string accountID in accountIDs)
            {
                string acctLocation = "acc/" + accountID + "/";
                VirtualOwner owner = VirtualOwner.FigureOwner(acctLocation);
                //CoreDomain.EnsureMasterCollections(owner);
                //CoreDomain.RefreshMasterCollections(owner);
                OIPDomain.EnsureMasterCollections(owner);
                OIPDomain.RefreshMasterCollections(owner);
            }
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            foreach (string groupID in groupIDs)
            {
                string grpLocation = "grp/" + groupID + "/";
                VirtualOwner owner = VirtualOwner.FigureOwner(grpLocation);
                //CoreDomain.EnsureMasterCollections(owner);
                //CoreDomain.RefreshMasterCollections(owner);
                OIPDomain.EnsureMasterCollections(owner);
                OIPDomain.RefreshMasterCollections(owner);
            }
        }

        public static string[] GetAllOwnerLocations()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            var accountLocs = accountIDs.Select(accID => "acc/" + accID + "/");
            var groupLocs = GetAllGroupLocations();
            return accountLocs.Union(groupLocs).ToArray();
        }

        public static string[] GetAllGroupLocations()
        {
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            var groupLocs = groupIDs.Select(grpID => "grp/" + grpID + "/");
            return groupLocs.ToArray();
        }



        public static string[] GetAllAccountLocations()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            var accountLocs = accountIDs.Select(accID => "acc/" + accID + "/");
            return accountLocs.ToArray();
        }

        public static void ReconnectGroupsMastersAndCollections(string groupID = null, string objectNamePart = null)
        {
            var groupLocs = GetAllGroupLocations();
            if(groupID != null)
                groupLocs = groupLocs.Where(grpLoc => grpLoc.Contains(groupID)).ToArray();
            foreach(var grpLoc in groupLocs)
                ReconnectMastersAndCollections(grpLoc, objectNamePart);
        }

        public static void ReconnectAccountsMastersAndCollections(string objectNamePart = null)
        {
            var acctLocs = GetAllAccountLocations();
            foreach (var acctLoc in acctLocs)
                ReconnectMastersAndCollections(acctLoc, objectNamePart);
        }

        private static IInformationObject[] GetAllInformationObjects(Predicate<string> filterByFullName,  Predicate<IInformationObject> filterIfFalse)
        {
            string[] ownerLocations = GetAllOwnerLocations();
            List<IInformationObject> result = new List<IInformationObject>();
            foreach(string ownerLocation in ownerLocations)
            {
                Debug.WriteLine("Getting objects for owner: " + ownerLocation);
                var ownerObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation, filterByFullName,  filterIfFalse);
                result.AddRange(ownerObjects);
            }
            return result.ToArray();
        }

        private static void ReconnectMastersAndCollections(string ownerLocation, string objectNamePart)
        {
            //string myLocalAccountID = "0c560c69-c3a7-4363-b125-ba1660d21cf4";
            //string acctLoc = "acc/" + myLocalAccountID + "/";

            VirtualOwner me = VirtualOwner.FigureOwner(ownerLocation);

            var informationObjects = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation, blobName => objectNamePart == null || blobName.Contains(objectNamePart), 
                                                                                              nonMaster =>
                                                                                              nonMaster.
                                                                                                  IsIndependentMaster ==
                                                                                              false && (nonMaster is TBEmailValidation == false)).ToArray();
            foreach (var iObj in informationObjects)
            {
                try
                {
                    iObj.ReconnectMastersAndCollections(true);
                } catch(Exception ex)
                {
                    bool ignoreException = false;
                    if (ignoreException == false)
                        throw;
                }
            }
        }

        private static void RefreshAllAccounts()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach (var accountID in accountIDs)
            {
                var accountRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRAccountRoot>(accountID);
                accountRoot.Account.StoreAccountToRoot();
            }

        }

        private static void FixGroupMastersAndCollections(string groupID)
        {
            Debug.WriteLine("Fixing group: " + groupID);
            TBRGroupRoot groupRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRGroupRoot>(groupID);
            IContainerOwner owner = groupRoot.Group;
            owner.InitializeAndConnectMastersAndCollections();
            //OIPDomain.EnsureMasterCollections(groupRoot.Group);
            //OIPDomain.RefreshMasterCollections(groupRoot.Group);
            //groupRoot.Group.ReconnectMastersAndCollectionsForOwner();
        }

        private static void FixAllGroupsMastersAndCollections()
        {
            var groupIDs = TBRGroupRoot.GetAllGroupIDs();
            foreach (var groupID in groupIDs)
            {
                FixGroupMastersAndCollections(groupID);
            }
        }


        private static void AddLegacyGroupWithInitiator(string groupID, string initiatorEmailAddress)
        {
            var groupRoot = TBRGroupRoot.CreateLegacyNewWithGroup(groupID);
            groupRoot.Group.JoinToGroup(initiatorEmailAddress, TBCollaboratorRole.InitiatorRoleValue);
            //groupRoot.Group.JoinToGroup("jeroen@caloom.com", "moderator");
            StorageSupport.StoreInformation(groupRoot);
            OIPDomain.EnsureMasterCollections(groupRoot.Group);
            OIPDomain.RefreshMasterCollections(groupRoot.Group);
            groupRoot.Group.ReconnectMastersAndCollectionsForOwner();
        }

        private static void UpdateAllImageFormatsCustomGroup()
        {
            //var images =
            //    GetAllInformationObjects(name => name.Contains("/Image/") && name.Contains("/9798daca-"), io => io is Image).Cast<Image>().ToArray();
            var ownerLocations = GetAllOwnerLocations();
            var ownerLocation = ownerLocations.Where(loc => loc.Contains("/9798daca-")).SingleOrDefault();
            var images = StorageSupport.CurrActiveContainer.GetInformationObjects(ownerLocation, name => name.Contains("/Image/"), io => io is Image).Cast<Image>().ToArray();
            int currImageIndex = 0;
            foreach (var image in images)
            {
                if (image.ImageData.ID.Contains("a25982") == false)
                    continue;
                image.ImageData.UpdateAdditionalMediaFormats();
                Console.WriteLine("Processed Image: " + ++currImageIndex + " out of " + images.Length);
            }
            InformationContext.ProcessAndClearCurrentIfAvailable();
            //InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
        }



        private static void UpdateAccountAndGroups(string accountEmail)
        {
            string emailID = TBREmailRoot.GetIDFromEmailAddress(accountEmail);
            TBREmailRoot emailRoot = ObjectStorage.RetrieveFromDefaultLocation<TBREmailRoot>(emailID);
            TBRAccountRoot accountRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRAccountRoot>(emailRoot.Account.ID);
            foreach(var groupRole in accountRoot.Account.GroupRoleCollection.CollectionContent)
            {
                TBRGroupRoot groupRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRGroupRoot>(groupRole.GroupID);
                RefreshAccountGroupMemberships.Execute(new RefreshAccountGroupMembershipsParameters
                {
                    AccountID = accountRoot.Account.ID,
                    GroupRoot = groupRoot
                });
                //InformationContext.ProcessAndClearCurrentIfAvailable();
                //InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
            }
        }

        private static void RemoveMemberFromGroup(string groupID, string memberEmail)
        {
            AaltoGlobalImpact.OIP.RemoveMemberFromGroup.Execute(new RemoveMemberFromGroupParameters()
                                                                    {
                                                                        GroupID = groupID,
                                                                        EmailAddress = memberEmail
                                                                    });
            //InformationContext.ProcessAndClearCurrentIfAvailable();
            //InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
        }

        /*
        private static void RenderAllPagesInWorker()
        {
            RenderWebSupport.RefreshAllAccountAndGroupTemplates(true, "AaltoGlobalImpact.OIP.Blog", "AaltoGlobalImpact.OIP.Activity", "AaltoGlobalImpact.OIP.AddressAndLocation",
                "AaltoGlobalImpact.OIP.Image", "AaltoGlobalImpact.OIP.ImageGroup", "AaltoGlobalImpact.OIP.Category");
        }*/

        private static void resendIndexingRequests(string groupID)
        {
            VirtualOwner owner = new VirtualOwner("grp", groupID);
            var indexingRequests = StorageSupport.CurrActiveContainer.GetInformationObjects(owner, "TheBall.Index/IndexingRequest");
            foreach (var ixReq in indexingRequests)
            {
                IndexSupport.PutIndexingRequestToQueue(StorageSupport.CurrActiveContainer.Name, IndexSupport.DefaultIndexName, owner, ixReq.ID);
            }
        }

        private static void PatchAccountsUpToDateWithRoot()
        {
            var accountIDs = TBRAccountRoot.GetAllAccountIDs();
            foreach (var accountID in accountIDs)
            {
                UpdateAccountRootToReferences.Execute(new UpdateAccountRootToReferencesParameters
                                                          {
                                                              AccountID = accountID
                                                          });
            }
        }

        private static void PatchDefaultValues()
        {
            // TODO: Something to 
            // AccountContainer.AccountModule.Introduction
            // Patch & Fix existing activities, blogs, groups with titles 

        }
        
        private static void PatchTextContentBodyContentToRawHtml(string groupID, DateTime patchModifiedBeforeLimit)
        {
            VirtualOwner owner = new VirtualOwner("grp", groupID);
            var textContentBlobs = owner.GetOwnerBlobListing("AaltoGlobalImpact.OIP/TextContent/", true);
            patchModifiedBeforeLimit = patchModifiedBeforeLimit.ToUniversalTime();
            foreach (CloudBlockBlob tcBlob in textContentBlobs)
            {
                if (tcBlob.Properties.LastModified >= patchModifiedBeforeLimit)
                    continue;
                var textContent = ObjectStorage.RetrieveObject<TextContent>(tcBlob.Name, owner);
                if (String.IsNullOrEmpty(textContent.RawHtmlContent))
                {
                    textContent.RawHtmlContent = textContent.Body;
                    textContent.Body = "ACTUAL CONTENT IN RAW FIELD!" + Environment.NewLine + textContent.Body;
                    textContent.StoreInformation();
                }
            }
        }

        private static void PatchCollectionsToNodeSummaries()
        {
            var groupLocations = GetAllGroupLocations();
            foreach (var groupLocation in groupLocations)
            {
                Debug.WriteLine("Patching group: " + groupLocation);
                var nodeSummaryContainers = StorageSupport.CurrActiveContainer.GetInformationObjects(groupLocation,
                                                                                                     name =>
                                                                                                     name.Contains(
                                                                                                         "/NodeSummaryContainer/"));
                foreach (NodeSummaryContainer nodeSummaryContainer in nodeSummaryContainers)
                {
                    if (nodeSummaryContainer.NodeSourceEmbeddedContent != null &&
                        nodeSummaryContainer.NodeSourceLinkToContent != null &&
                        nodeSummaryContainer.NodeSourceImages != null &&
                        nodeSummaryContainer.NodeSourceBinaryFiles != null)
                        continue;
                    Debug.WriteLine("Patched something...");
                    if(nodeSummaryContainer.NodeSourceEmbeddedContent == null)
                        nodeSummaryContainer.NodeSourceEmbeddedContent = new EmbeddedContentCollection();
                    if(nodeSummaryContainer.NodeSourceLinkToContent == null)
                        nodeSummaryContainer.NodeSourceLinkToContent  = new LinkToContentCollection();
                    if(nodeSummaryContainer.NodeSourceImages == null)
                        nodeSummaryContainer.NodeSourceImages = new ImageCollection();
                    if(nodeSummaryContainer.NodeSourceBinaryFiles == null)
                        nodeSummaryContainer.NodeSourceBinaryFiles = new BinaryFileCollection();
                    nodeSummaryContainer.StoreInformation();
                }
            }
        }

        private static void PatchCategoriesAndTextContentCollectionNodeSummarySpecificGroup(string groupID)
        {
            var nodesummaryContainers = GetAllInformationObjects(name => name.Contains("NodeSummaryContainer") && name.Contains(groupID),
                                                                 iObj => iObj is NodeSummaryContainer);
            foreach (NodeSummaryContainer nodeSummaryContainer in nodesummaryContainers)
            {
                bool changed = false;
                if (nodeSummaryContainer.NodeSourceTextContent == null)
                {
                    Debug.WriteLine("Fixing nodesummary: " + nodeSummaryContainer.RelativeLocation);
                    nodeSummaryContainer.NodeSourceTextContent = new TextContentCollection();
                    changed = true;
                }
                if (nodeSummaryContainer.NodeSourceCategories == null)
                {
                    Debug.WriteLine("Fixing nodesummary: " + nodeSummaryContainer.RelativeLocation);
                    nodeSummaryContainer.NodeSourceCategories = new CategoryCollection();
                    changed = true;
                }
                if(changed)
                    nodeSummaryContainer.StoreInformation();
            }
        }

        public static bool DoPatching()
        {
            //return false;
            Debugger.Break();
            bool skip = false;
            if (skip == false)
                throw new NotSupportedException("Skip this with debugger");

            //PatchSubscriptionsToSubmitted();
            UpdateAccountAndGroups(accountEmail:"maijahseppala@gmail.com");

            //testProcessWithAGISiteMigration(true);
            //testProcessWithWeconomySiteMigration(true);
            //resendIndexingRequests("d6347c47-aeee-4ce2-8f1f-601e52ecd7ac");
            //InitCategoryParentIDFromParentCategory();

            //ReconnectAccountsMastersAndCollections();

            //PatchCollectionsToNodeSummaries();
            //PatchEmbeddedAndLinkToContentToGroupNodeSummaries();
            //FixGroupMastersAndCollections("f0a2650b-9c42-4098-95e2-0979be189b8e"); // Proj2
            //FixGroupMastersAndCollections("ecc5fac6-49d3-4c57-b01b-349d83503d93"); // Proj2
            //FixAllGroupsMastersAndCollections();

            //PatchCategoriesAndTextContentCollectionNodeSummarySpecificGroup("9798daca-afc4-4046-a99b-d0d88bb364e0");
            //PatchCategoriesAndTextContentCollectionNodeSummarySpecificGroup("c229a54c-31fe-4c33-957d-e7b52cdbc06a");
            //FixGroupMastersAndCollections("c229a54c-31fe-4c33-957d-e7b52cdbc06a"); // Proj1
            //PatchCategoriesAndTextContentCollectionNodeSummarySpecificGroup("0d687b5f-d032-4f36-a5ea-6ff4fb3c5963");

            //PatchCategoriesAndTextContentCollectionNodeSummarySpecificGroup("b9ba3208-bf89-4fa8-bad5-c2cb524b5fd9");
            //FixGroupMastersAndCollections("b9ba3208-bf89-4fa8-bad5-c2cb524b5fd9"); // Proj1

            //PatchTextContentCollectionNodeSummarySpecificGroup("9798daca-afc4-4046-a99b-d0d88bb364e0");
            //FixGroupMastersAndCollections("9798daca-afc4-4046-a99b-d0d88bb364e0");

            // fff483ed-f45e-419e-8e3a-99f48d2f4fa8
            //PatchTextContentCollectionNodeSummarySpecificGroup("fff483ed-f45e-419e-8e3a-99f48d2f4fa8");
            //FixGroupMastersAndCollections("fff483ed-f45e-419e-8e3a-99f48d2f4fa8");

            //InitBlogProfileAndIconOnce();

            //EnsureAndRefreshMasterCollections();
            //RemoveIncontextEditingFromBlogsAndActivitiesFromCertainGroup();
            //ReconnectGroupsMastersAndCollections("a0ea605a-1a3e-4424-9807-77b5423d615c");
            //ReconnectGroupsMastersAndCollections("NodeSummaryContainer");
            //RenderAllPagesInWorker();

            //SyncWwwPublicFromDefaultGroup();
            //AddLegacyGroupWithInitiator("9798daca-afc4-4046-a99b-d0d88bb364e0", "kalle.launiala@citrus.fi");
            //FixGroupMastersAndCollections("9798daca-afc4-4046-a99b-d0d88bb364e0");
            //FixGroupMastersAndCollections("a0ea605a-1a3e-4424-9807-77b5423d615c");
            //FixGroupMastersAndCollections("705dbb02-ea90-4b4c-b802-085287ca2264");


            //InitBlogAndActivityLocationCollectionsOnce();
            //InitBlogGroupActivityImageGroupCollectionsOnce();

            //ReconnectAccountsMastersAndCollections();
            //ReconnectGroupsMastersAndCollections();
            //EnsureAndRefreshMasterCollections();
            //ConnectMapContainerToCollections();
            //ClearEmptyLocations();


            //RenderAllPagesInWorker();
            //ReportAllSubscriptionCounts();
            //TestWorkerSubscriberChainExecutionPerformance();
            //TestSubscriptionExecution();
            //TestSubscriptionChainPick();

            //ExecuteSubscriptionChain(RenderWebSupport.DefaultGroupID);
            //PatchAccountsUpToDateWithRoot();
            //PatchBlogsAndActivitiesSelectedCollections();

            //UpdateAccountAndGroups(accountEmail: "kalle.launiala@citrus.fi");
            //UpdateAccountAndGroups(accountEmail: "kalle.launiala@gmail.com");
            //RemoveMemberFromGroup(groupID: "9798daca-afc4-4046-a99b-d0d88bb364e0",
            //                      memberEmail: "kalle.launiala@gmail.com");

            return true;
        }

    }
}