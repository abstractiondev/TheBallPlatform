using System.IO;
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

        private static T RetrieveObject<T>(string relativeLocation, IContainerOwner owner)
        {
            return (T) StorageSupport.RetrieveInformation(relativeLocation, typeof (T));
        }

        public static string GetRelativeLocationFromID<T>(string id)
        {
            string namespaceName = typeof (T).Namespace;
            string className = typeof (T).Name;
            return Path.Combine(namespaceName, className, id).Replace("\\", "/");
        }
    }
}