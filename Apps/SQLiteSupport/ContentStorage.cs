using System;
using System.IO;
using System.Threading.Tasks;

namespace SQLiteSupport
{
    public class ContentStorage
    {
        public static Func<string, string> GetContentAsStringFunc = File.ReadAllText;

        public static Func<string, Task<string>> GetContentAsStringAsyncFunc;

        public static string GetContentAsString(string currentFullStoragePath)
        {
            return GetContentAsStringFunc(currentFullStoragePath);
        }

        public static async Task<string> GetContentAsStringAsync(string currentFullStoragePath)
        {
            return await GetContentAsStringAsyncFunc(currentFullStoragePath);
        }
    }
}