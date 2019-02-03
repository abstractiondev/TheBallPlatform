using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureSupport;
using Microsoft.WindowsAzure.Storage.Blob;
using SQLiteSupport;
using TheBall.CORE.InstanceSupport;

namespace TheBall.CORE
{
    public class UpdateOwnerDomainObjectsInSQLiteStorageImplementation
    {
        public static string GetTarget_OwnerRootPath(IContainerOwner owner)
        {
            return StorageSupport.GetOwnerRootAddress(owner);
        }

        public static CloudBlockBlob[] GetTarget_BlobsToSync(IContainerOwner owner, string semanticDomain)
        {
            throw new NotImplementedException();
            //var blobListing = owner.GetOwnerBlobListing(semanticDomain, true);
            //return blobListing.Cast<CloudBlockBlob>().ToArray();
        }

        public static async Task<CloudBlockBlob[]> GetTarget_BlobsToSyncAsync(IContainerOwner owner, string semanticDomain)
        {
            List<CloudBlockBlob> results = new List<CloudBlockBlob>();
            BlobContinuationToken continuationToken = null;
            do
            {
                var blobSegment = await owner.ListBlobsWithPrefixAsync(semanticDomain, true, continuationToken, true);
                var blobsLQ = blobSegment.Results.Cast<CloudBlockBlob>();
                results.AddRange(blobsLQ);
                continuationToken = blobSegment.ContinuationToken;
            } while (continuationToken != null);
            return results.ToArray();
        }

        public static async Task ExecuteMethod_PerformSyncingAsync(Type dataContextType, string databaseAttachOrCreateMethodName, string sqLiteDbLocationFileName, string ownerRootPath, CloudBlockBlob[] blobsToSync)
        {
            if (dataContextType == null)
                return;
            if (databaseAttachOrCreateMethodName == null)
                throw new ArgumentNullException("databaseAttachOrCreateMethodName");
            if (sqLiteDbLocationFileName == null)
                throw new ArgumentNullException("sqLiteDbLocationFileName");
            if (ownerRootPath == null)
                throw new ArgumentNullException("ownerRootPath");
            if (blobsToSync == null)
                throw new ArgumentNullException("blobsToSync");
            ContentStorage.GetContentAsStringAsyncFunc = async
                blobPath =>
                {
                    var blob = StorageSupport.GetOwnerBlobReference(blobPath);
                    var xmlResponse = await blob.DownloadTextAsync(Encoding.UTF8, null, null, null, CancellationToken.None);
                    int index = xmlResponse.IndexOf('<');
                    if (index > 0)
                    {
                        xmlResponse = xmlResponse.Substring(index, xmlResponse.Length - index);
                    }
                    return xmlResponse;
                };
            using (
                IStorageSyncableDataContext dbContext = (IStorageSyncableDataContext)dataContextType.InvokeMember(databaseAttachOrCreateMethodName, BindingFlags.InvokeMethod, null, null, new object[] { sqLiteDbLocationFileName })
                //SQLite.TheBall.Payments.TheBallDataContext.CreateOrAttachToExistingDB(sqLiteDbLocationFileName)
                )
            {

                bool anyChangesApplied = await SQLiteSync.ApplyStorageChangesToSQLiteDBAsync(ownerRootPath, dbContext,
                    rootPath =>
                    {
                        List<InformationObjectMetaData> metaDatas = new List<InformationObjectMetaData>();
                        foreach (CloudBlockBlob blob in blobsToSync)
                        {
                            if (Path.GetExtension(blob.Name) != String.Empty)
                                continue;
                            var nameComponents = blob.Name.Split('/');
                            string objectID = nameComponents[nameComponents.Length - 1];
                            string objectType = nameComponents[nameComponents.Length - 2];
                            string semanticDomain = nameComponents[nameComponents.Length - 3];
                            var metaData = new InformationObjectMetaData
                            {
                                CurrentStoragePath = blob.Name.Substring(ownerRootPath.Length),
                                FileLength = blob.Properties.Length,
                                LastWriteTime = blob.Properties.LastModified.GetValueOrDefault().ToString("s"),
                                MD5 = blob.Properties.ContentMD5,
                                ETag = blob.Properties.ETag,
                                SemanticDomain = semanticDomain,
                                ObjectType = objectType,
                                ObjectID = objectID
                            };
                            metaDatas.Add(metaData);
                        }
                        return metaDatas.ToArray();
                    });
            }

        }

        public static void ExecuteMethod_PerformSyncing(Type dataContextType, string databaseAttachOrCreateMethodName, string sqLiteDbLocationFileName, string ownerRootPath, CloudBlockBlob[] blobsToSync)
        {
            // For now clear the datacontext type
            if (dataContextType == null)
                return;
            if(databaseAttachOrCreateMethodName == null)
                throw new ArgumentNullException("databaseAttachOrCreateMethodName");
            if (sqLiteDbLocationFileName == null)
                throw new ArgumentNullException("sqLiteDbLocationFileName");
            if (ownerRootPath == null)
                throw new ArgumentNullException("ownerRootPath");
            if (blobsToSync == null)
                throw new ArgumentNullException("blobsToSync");
            ContentStorage.GetContentAsStringFunc =
                blobPath =>
                {
                    throw new NotImplementedException();
                    var xmlResponse = "";
                        //Encoding.UTF8.GetString(StorageSupport.GetOwnerBlobReference(blobPath).DownloadByteArray());
                    int index = xmlResponse.IndexOf('<');
                    if (index > 0)
                    {
                        xmlResponse = xmlResponse.Substring(index, xmlResponse.Length - index);
                    }
                    return xmlResponse;
                };
            //dataContextType.InvokeMember()
            using (
                IStorageSyncableDataContext dbContext = (IStorageSyncableDataContext)dataContextType.InvokeMember(databaseAttachOrCreateMethodName, BindingFlags.InvokeMethod, null, null, new object[] { sqLiteDbLocationFileName })
                    //SQLite.TheBall.Payments.TheBallDataContext.CreateOrAttachToExistingDB(sqLiteDbLocationFileName)
                )
            {

                bool anyChangesApplied = SQLiteSync.ApplyStorageChangesToSQLiteDB(ownerRootPath, dbContext,
                    rootPath =>
                    {
                        List<InformationObjectMetaData> metaDatas = new List<InformationObjectMetaData>();
                        foreach (CloudBlockBlob blob in blobsToSync)
                        {
                            if (Path.GetExtension(blob.Name) != String.Empty)
                                continue;
                            var nameComponents = blob.Name.Split('/');
                            string objectID = nameComponents[nameComponents.Length - 1];
                            string objectType = nameComponents[nameComponents.Length - 2];
                            string semanticDomain = nameComponents[nameComponents.Length - 3];
                            var metaData = new InformationObjectMetaData
                            {
                                CurrentStoragePath = blob.Name.Substring(ownerRootPath.Length),
                                FileLength = blob.Properties.Length,
                                LastWriteTime = blob.Properties.LastModified.GetValueOrDefault().ToString("s"),
                                MD5 = blob.Properties.ContentMD5,
                                ETag = blob.Properties.ETag,
                                SemanticDomain = semanticDomain,
                                ObjectType = objectType,
                                ObjectID = objectID
                            };
                            metaDatas.Add(metaData);
                        }
                        return metaDatas.ToArray();
                    });
            }
        }

        public static string GetTarget_SQLiteDBLocationDirectory(IContainerOwner owner)
        {
            string instanceName = InformationContext.Current.InstanceName;
            string dbDirectory = Path.Combine(SecureConfig.Current.CoreShareWithFolderName, instanceName,
                owner.ContainerName, owner.LocationPrefix);
            return dbDirectory;

        }

        public static void ExecuteMethod_CreateDBLocationDirectoryIfMissing(string sqLiteDbLocationDirectory)
        {
            if (!Directory.Exists(sqLiteDbLocationDirectory))
                Directory.CreateDirectory(sqLiteDbLocationDirectory);
        }

        public static string GetTarget_SQLiteDBLocationFileName(string semanticDomain, string sqLiteDbLocationDirectory)
        {
            string fullFileName = Path.Combine(sqLiteDbLocationDirectory, semanticDomain + ".sqlite");
            if (fullFileName.StartsWith(@"\\"))
                fullFileName = @"\\" + fullFileName;
            return fullFileName;
        }

        public static string GetTarget_DataContextFullTypeName(string semanticDomain)
        {
            return String.Format("SQLite.{0}.TheBallDataContext", semanticDomain);
        }

        public static Type GetTarget_DataContextType(string dataContextFullTypeName)
        {
            //Type dataContextType = Type.GetType(dataContextFullTypeName);
            Type dataContextType = ReflectionSupport.GetSQLiteDataContextType(dataContextFullTypeName);
            return dataContextType;
        }

        public static string GetTarget_DatabaseAttachOrCreateMethodName()
        {
            return "CreateOrAttachToExistingDB";
        }

    }
}