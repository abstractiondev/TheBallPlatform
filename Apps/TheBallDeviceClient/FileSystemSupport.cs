using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace TheBall.Support.DeviceClient
{
    public static class FileSystemSupport
    {
        public static ContentItemLocationWithMD5[] GetContentRelativeFromRoot(string rootItem)
        {
            bool rootItemIsDir = Directory.Exists(rootItem);
            FileInfo[] fileInfos = null;
            int relativeNameStartingIX;
            if (rootItemIsDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootItem);
                fileInfos = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                relativeNameStartingIX = rootItem.EndsWith("/") ? rootItem.Length : rootItem.Length + 1;
            }
            else
            {
                fileInfos = new[] {new FileInfo(rootItem)};
                var fileName = Path.GetFileName(rootItem);
                relativeNameStartingIX = rootItem.Length - fileName.Length;
            }
            List<ContentItemLocationWithMD5> contentItems = new List<ContentItemLocationWithMD5>();
            Console.WriteLine("Getting MD5 for {0} files...", fileInfos.Length);
            int totalTODO = fileInfos.Length;
            int currDone = 0;
            int currDots = 0;
            foreach (var fileInfo in fileInfos)
            {
                if (!fileInfo.Exists)
                    continue;
                ContentItemLocationWithMD5 contentItem = new ContentItemLocationWithMD5
                {
                    ContentLocation = fileInfo.FullName.Substring(relativeNameStartingIX).Replace('\\', '/'),
                    ContentMD5 = getMD5(fileInfo)
                };
                contentItems.Add(contentItem);
                currDone++;
                int currProgress = (currDone*10)/totalTODO;
                if (currDots < currProgress)
                {
                    Console.Write(".");
                    currDots++;
                }
            }
            Console.WriteLine(" done.");
            return contentItems.ToArray();
        }

        private static MD5 md5 = MD5.Create();

        private static string getMD5(FileInfo fileInfo)
        {
            var fileData = File.ReadAllBytes(fileInfo.FullName);
            var md5Hash = md5.ComputeHash(fileData);
            return Convert.ToBase64String(md5Hash);
        }

        public static Stream GetLocalTargetAsIs(string targetFullName)
        {
            string targetDirectoryName = Path.GetDirectoryName(targetFullName);
            try
            {
                if (Directory.Exists(targetDirectoryName) == false)
                    Directory.CreateDirectory(targetDirectoryName);
            }
            catch
            {
            }
            Stream targetStream = File.Create(targetFullName);
            return targetStream;
        }

        public static void RemoveLocalTarget(string targetfullname)
        {
            File.Delete(targetfullname);
        }
    }
}
