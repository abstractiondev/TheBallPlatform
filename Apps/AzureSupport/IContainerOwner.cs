using System.Drawing;

namespace TheBall.CORE
{
    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }

    public static class IContainerOwnerExt
    {
        public static string GetOwnerPrefix(this IContainerOwner containerOwner)
        {
            return containerOwner.ContainerName + "/" + containerOwner.LocationPrefix;
        }

        public static string GetIDFromLocationPrefix(this IContainerOwner containerOwner)
        {
            return containerOwner.LocationPrefix;
        }

        public static bool IsSameOwner(this IContainerOwner thisOwner, IContainerOwner containerOwner)
        {
            return thisOwner.ContainerName == containerOwner.ContainerName && thisOwner.LocationPrefix == containerOwner.LocationPrefix;
        }

        public static string GetOwnerContentLocation(this IContainerOwner owner, string location)
        {
            return StorageSupport.GetOwnerContentLocation(owner, location);
        }
    }

}