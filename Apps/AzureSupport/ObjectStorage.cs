using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureSupport.TheBall.CORE;
using TheBall.CORE;
using TheBall.CORE.Storage;

namespace TheBall
{
    public static class ObjectStorage
    {
        public static async Task<T> RetrieveFromDefaultLocationA<T>(string id, IContainerOwner owner = null)
        {
            string relativeLocation = GetRelativeLocationFromID<T>(id);
            return await RetrieveObjectA<T>(relativeLocation, owner);
        }

        internal static async Task<IInformationObject> RetrieveFromDefaultLocationA(string contentDomain, string contentTypeName, string contentObjectID, IContainerOwner owner)
        {
            string relativeLocation = GetRelativeLocationFromID(contentDomain, contentTypeName, contentObjectID);
            return await RetrieveObjectA<IInformationObject>(relativeLocation, owner);
        }


        public static async Task<string[]> ListOwnerObjectIDs<T>(IContainerOwner containerOwner)
        {
            var typePrefix = getTypePrefix(typeof (T));
            var blobItems = await BlobStorage.GetBlobItemsA(containerOwner, typePrefix + "/", item => item.FileName.Contains(".") == false);
            var ids = blobItems.Select(item => item.FileName).ToArray();
            return ids;
        }

        public static string GetRelativeLocationFromID<T>(string id)
        {
            string namespaceName = typeof (T).Namespace;
            string className = typeof (T).Name;
            return GetRelativeLocationFromID(namespaceName, className, id);
        }

        public static string GetRelativeLocationFromID(string namespaceName, string className, string id)
        {
            return Path.Combine(namespaceName, className, id).Replace("\\", "/");
        }


        public static async Task<T> RetrieveFromSystemOwner<T>(string contentName, string eTag = null, bool requireExisting = false)
        {
            return await RetrieveFromOwnerContentA<T>(SystemSupport.SystemOwner, contentName, eTag, requireExisting);
        }

        private static string getTypePrefix(Type objectType)
        {
            string namespaceName = objectType.Namespace;
            string className = objectType.Name;
            var typePrefix = $"{namespaceName}/{className}";
            return typePrefix;
        }

        public static async Task<T> RetrieveFromOwnerContentA<T>(IContainerOwner containerOwner, string contentName, string eTag = null, bool requireExisting = false)
        {
            var typePrefix = getTypePrefix(typeof (T));
            string locationPath = $"{typePrefix}/{contentName}";
            var result = await RetrieveObjectA<T>(locationPath, containerOwner, eTag);
            if (result == null && requireExisting)
                throw new InvalidDataException($"{typePrefix} missing or changed");
            return result;
        }

        public static async Task<T> RetrieveFromOwnerContentA<T>(string contentName, string eTag = null, bool requireExisting = false)
        {
            return await RetrieveFromOwnerContentA<T>(InformationContext.CurrentOwner, contentName, eTag, requireExisting);
        }


        public static async Task StoreInterfaceObject(object dataObject, string objectName = null,
            bool isInterfaceData = false)
        {
            await StoreInterfaceObject(null, dataObject, objectName, isInterfaceData);
        }

        public static async Task StoreInterfaceObject(IContainerOwner owner, object dataObject, string objectName = null,
            bool isInterfaceData = false)
        {
            if (isInterfaceData && objectName == null)
                throw new ArgumentException("ObjectName must be given if isInterfaceData is true", nameof(objectName));
            var objectType = dataObject.GetType();
            if (objectName == null)
                objectName = objectType.Name;
            if (owner == null)
                owner = InformationContext.CurrentOwner;
            var ownerPrefixedWithExtension = getOwnerPrefixedNameWithExtension(owner, objectName, isInterfaceData, objectType);
            await BlobStorage.StoreBlobJsonContentA(ownerPrefixedWithExtension, dataObject);
        }


        public static async Task<T> GetInterfaceObject<T>(string objectName = null, bool isInterfaceData = false) where T : class
        {
            return await GetInterfaceObject<T>(null, objectName, isInterfaceData);
        }

        public static async Task<T> GetInterfaceObject<T>(IContainerOwner owner, string objectName = null, bool isInterfaceData = false) where T : class
        {
            if(isInterfaceData && objectName == null)
                throw new ArgumentException("ObjectName must be given if isInterfaceData is true", nameof(objectName));
            var objectType = typeof (T);
            if (objectName == null)
                objectName = objectType.Name;
            if (owner == null)
                owner = InformationContext.CurrentOwner;
            var ownerPrefixedWithExtension = getOwnerPrefixedNameWithExtension(owner, objectName, isInterfaceData, objectType);
            var data = await BlobStorage.GetBlobJsonContentA<T>(ownerPrefixedWithExtension);
            return data;
        }

        private static string getOwnerPrefixedNameWithExtension(IContainerOwner owner, string objectName,
            bool isInterfaceData, Type objectType) 
        {
            string namePart = isInterfaceData
                ? "TheBall.Interface/InterfaceData/" + objectName
                : getTypePrefix(objectType).Replace("TheBall.Interface.INT", "TheBall.Interface") + "/" + objectName;
            var ownerPrefixedWithExtension = StorageSupport.GetOwnerContentLocation(owner, namePart) + ".json";
            return ownerPrefixedWithExtension;
        }

        public static async Task<T> RetrieveObjectA<T>(string relativeLocation, IContainerOwner owner = null, string eTag = null)
        {
            var result = (T) await StorageSupport.RetrieveInformationA(relativeLocation, typeof (T), eTag, owner);
            return result;
        }

    }
}