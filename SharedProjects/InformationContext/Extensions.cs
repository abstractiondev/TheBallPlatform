using System;

namespace TheBall.CORE
{
    public static partial class Extensions
    {
        public static bool IsNoOwner(this IContainerOwner owner)
        {
            return owner.ContainerName == null && owner.LocationPrefix == null;
        }
        public static bool IsSystemOwner(this IContainerOwner owner)
        {
            return false;
        }

        public static string GetOwnerContentLocation(this IContainerOwner owner, string path)
        {
            if (!owner.IsNoOwner())
                throw new NotSupportedException("Only NoOwner supported");
            return path;
        }
    }
}