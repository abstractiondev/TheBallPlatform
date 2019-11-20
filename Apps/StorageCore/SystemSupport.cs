using System;
using System.Linq;

namespace TheBall.Core
{
    public static class SystemSupport
    {
        public static readonly string[] ReservedDomainNames = new string[] {"TheBall.Core", "TheBall.Payments", "TheBall.Interface"};
        public const string SystemOwnerRoot = "sys/AAA";
        public static readonly IContainerOwner SystemOwner;
        public static readonly IContainerOwner AnonymousOwner;

        static SystemSupport()
        {
            SystemOwner = new VirtualOwner("sys", "AAA");
            AnonymousOwner = new VirtualOwner("anon", "public" );
        }


        public static string[] FilterAwayReservedFolders(string[] directories)
        {
            return directories.Where(dir => ReservedDomainNames.Any(resDom => dir.Contains(resDom) == false)).ToArray();
        }

        public static bool IsValidTemplateName(string templateName)
        {
            var isValid = String.IsNullOrWhiteSpace(templateName) == false &&
                          templateName.All(chr => chr != '\\' && chr != '/' && chr != '.');
            return isValid;
        }
    }
}