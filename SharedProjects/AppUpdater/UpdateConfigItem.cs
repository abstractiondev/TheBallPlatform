namespace TheBall.Infra.AppUpdater
{
    public enum UpdatingStatus
    {
        UpToDate = 0,
        UpdatingFetchAndUnpack,
        UpdatingCleanUp
    }

    public class UpdateConfigItem
    {
        public AccessInfo AccessInfo;
        public string Name;
        public string MaturityLevel;
        public string BuildNumber;
        public string Commit;
        public UpdatingStatus UpdatingStatus;

        private string _UpdateDirName;
        public string UpdateDirName
        {
            get
            {
                if (_UpdateDirName == null && BuildNumber != null)
                {
                    _UpdateDirName = $"{BuildNumber}_{MaturityLevel}_{Commit}";
                }
                return _UpdateDirName;
            }
        }

    }
}