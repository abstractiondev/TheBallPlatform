using System;
using System.IO;

namespace TheBall.CORE
{
    public interface IContainerOwner
    {
        string ContainerName { get; }
        string LocationPrefix { get; }
    }

    public class VirtualOwner : IContainerOwner
    {
        private const int GuidLength = 36;
        public static IContainerOwner FigureOwner(string relativeLocation)
        {
            if (relativeLocation.StartsWith("acc/") || relativeLocation.StartsWith("grp/"))
                return new VirtualOwner(relativeLocation.Substring(0, 3),
                    relativeLocation.Substring(4, GuidLength));
            if (relativeLocation.StartsWith("sys/AAA"))
                return new VirtualOwner("sys", "AAA");
            if(relativeLocation.StartsWith("dev/DEV"))
                return new VirtualOwner("dev", "DEV");
            throw new InvalidDataException("Cannot figure owner of: " + relativeLocation);
        }

        public VirtualOwner(string containerName, string locationPrefix, bool requireSafeAccountOrGroup = false)
        {
            if (requireSafeAccountOrGroup)
            {
                if (containerName != "acc" && containerName != "grp")
                    throw new ArgumentException("Invalid owner type: " + containerName, nameof(containerName));
                Guid realGuid;
                if (!Guid.TryParse(locationPrefix, out realGuid) || realGuid.ToString() != locationPrefix)
                    throw new ArgumentException("Invalid owner ID: " + locationPrefix);
            }
            this.ContainerName = containerName;
            this.LocationPrefix = locationPrefix;
        }

        public string ContainerName { get; }

        public string LocationPrefix { get; }
    }

}