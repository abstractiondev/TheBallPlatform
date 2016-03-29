namespace TheBall.Infra.AzureRoleSupport
{
    public class AppTypeInfo
    {
        public readonly string AppType;
        public readonly string AppExecutablePath;
        public readonly string AppConfigPath;
        public AppManager CurrentManager;

        public AppTypeInfo(string appType, string appExecutablePath, string appConfigPath)
        {
            AppType = appType;
            AppExecutablePath = appExecutablePath;
            AppConfigPath = appConfigPath;
        }
    }
}