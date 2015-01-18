using System.IO;

namespace SQLiteSupport
{
    public class ContentStorage
    {
        public static string GetContentAsString(string currentFullStoragePath)
        {
            return File.ReadAllText(currentFullStoragePath);
        }
    }
}