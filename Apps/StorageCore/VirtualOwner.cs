using System;
using System.Collections;
using System.IO;
using TheBall;
using TheBall.Core;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class VirtualOwner : IContainerOwner
    {
        private string containerName;
        private string locationPrefix;

        public static IContainerOwner FigureOwner(string relativeLocation)
        {
            if (relativeLocation.StartsWith("acc/") || relativeLocation.StartsWith("grp/"))
                return new VirtualOwner(relativeLocation.Substring(0, 3),
                    relativeLocation.Substring(4, BlobStorage.StorageSupport.GuidLength));
            if (relativeLocation.StartsWith("sys/AAA"))
                return SystemSupport.SystemOwner;
            throw new InvalidDataException("Cannot figure owner of: " + relativeLocation);
        }

        public VirtualOwner(string containerName, string locationPrefix, bool requireSafeAccountOrGroup = false)
        {
            if (requireSafeAccountOrGroup)
            {
                if (containerName != "acc" && containerName != "grp")
                    throw new ArgumentException("Invalid owner type: " + containerName, "containerName");
                Guid realGuid;
                if(!Guid.TryParse(locationPrefix, out realGuid) || realGuid.ToString() != locationPrefix)
                    throw new ArgumentException("Invalid owner ID: " + locationPrefix);
            }
            this.containerName = containerName;
            this.locationPrefix = locationPrefix;
        }

        public string ContainerName
        {
            get { return containerName; }
        }

        public string LocationPrefix
        {
            get { return locationPrefix; }
        }

    }
}