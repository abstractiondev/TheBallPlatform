using System.IO;

namespace TheBall.Infra.AzureRoleSupport
{
    public class RoleAppInfo
    {
        public string RoleSpecificManagerArgs;
        public string AppConfigPath;
        public string AppRootFolder;
        public string ComponentName;
        public RoleAppType AppType;

        public string TargetConsolePath => Path.Combine(AppRootFolder, ComponentName + ".exe");
    }
}