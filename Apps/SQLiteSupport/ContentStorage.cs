using System;
using System.IO;

namespace SQLiteSupport
{
    public class ContentStorage
    {
        public static Func<string, string> GetContentAsStringFunc = File.ReadAllText;

        public static string GetContentAsString(string currentFullStoragePath)
        {
            return GetContentAsStringFunc(currentFullStoragePath);
        }
    }
}