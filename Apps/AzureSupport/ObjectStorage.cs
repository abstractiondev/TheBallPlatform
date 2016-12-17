using System;
using System.IO;
using System.Threading.Tasks;
using AzureSupport.TheBall.CORE;
using TheBall.CORE;

namespace TheBall
{
    public static class ObjectStorage
    {
        public static T RetrieveFromDefaultLocation<T>(string id,
            IContainerOwner owner = null)
        {
            string relativeLocation = GetRelativeLocationFromID<T>(id);
            return RetrieveObject<T>(relativeLocation, owner);
        }

        public static async Task<T> RetrieveFromDefaultLocationA<T>(string id, IContainerOwner owner = null)
        {
            string relativeLocation = GetRelativeLocationFromID<T>(id);
            return await RetrieveObjectA<T>(relativeLocation, owner);
        }


        public static T RetrieveObject<T>(string relativeLocation, IContainerOwner owner = null)
        {
            var result = (T)StorageSupport.RetrieveInformation(relativeLocation, typeof(T), null, owner);
            return result;
        }

        public static string GetRelativeLocationFromID<T>(string id)
        {
            string namespaceName = typeof (T).Namespace;
            string className = typeof (T).Name;
            return Path.Combine(namespaceName, className, id).Replace("\\", "/");
        }

        public static T RetrieveFromOwnerContent<T>(IContainerOwner containerOwner, string contentName)
        {
            string namespaceName = typeof (T).Namespace;
            string className = typeof (T).Name;
            string locationPath = String.Format("{0}/{1}/{2}", namespaceName, className, contentName);
            var result = RetrieveObject<T>(locationPath, containerOwner);
            return result;
        }


        public static async Task<T> RetrieveFromSystemOwner<T>(string contentName, string eTag = null, bool requireExisting = false)
        {
            return await RetrieveFromOwnerContentA<T>(SystemSupport.SystemOwner, contentName, eTag, requireExisting);
        }

        public static async Task<T> RetrieveFromOwnerContentA<T>(IContainerOwner containerOwner, string contentName, string eTag = null, bool requireExisting = false)
        {
            string namespaceName = typeof(T).Namespace;
            string className = typeof(T).Name;
            string locationPath = $"{namespaceName}/{className}/{contentName}";
            var result = await RetrieveObjectA<T>(locationPath, containerOwner, eTag);
            if (result == null && requireExisting)
                throw new InvalidDataException($"{className} missing or changed");
            return result;
        }

        public static async Task<T> RetrieveFromOwnerContentA<T>(string contentName, string eTag = null, bool requireExisting = false)
        {
            return await RetrieveFromOwnerContentA<T>(InformationContext.CurrentOwner, contentName, eTag, requireExisting);
        }

        public static async Task<T> RetrieveObjectA<T>(string relativeLocation, IContainerOwner owner = null, string eTag = null)
        {
            var result = (T) await StorageSupport.RetrieveInformationA(relativeLocation, typeof (T), eTag, owner);
            return result;
        }


    }
}