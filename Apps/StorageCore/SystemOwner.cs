namespace TheBall.Core
{
    public class SystemOwner : IContainerOwner
    {
        public static SystemOwner CurrentSystem { get; private set; }

        static SystemOwner()
        {
            CurrentSystem = new SystemOwner("sys", "AAA");
        }

        private SystemOwner(string containerName, string locationPrefix)
        {
            ContainerName = containerName;
            LocationPrefix = locationPrefix;
        }

        public string ContainerName { get; }
        public string LocationPrefix { get; }
    }
}