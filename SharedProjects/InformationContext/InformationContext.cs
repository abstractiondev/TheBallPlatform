using System.Collections.Generic;
using System.Text;

namespace TheBall.CORE
{
    public class InformationContext
    {
        public class NoOwner : IContainerOwner
        {
            public string ContainerName => null;
            public string LocationPrefix => null;
        }
        private static IContainerOwner currentOwner = new NoOwner();
        public static IContainerOwner CurrentOwner => currentOwner;
    }
}
