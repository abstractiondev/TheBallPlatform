using System.IO;

namespace TheBall.CORE.Storage
{
    public class BlobStorageFolder
    {
        public BlobStorageFolder(string fullPath)
        {
            FullPath = fullPath.Replace(@"\", "/");
            FolderName = Path.GetFileName(fullPath.TrimEnd('/'));
        }

        public string FolderName { get; private set; }
        public string FullPath { get; private set; }
    }
}