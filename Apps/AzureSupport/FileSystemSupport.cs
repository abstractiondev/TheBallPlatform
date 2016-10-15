using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using TheBall.CORE;

namespace TheBall
{
    public class BlobStorageContent
    {
        private byte[] _binaryContent;
        public string FileName { get; set; }

        public byte[] BinaryContent
        {
            get { return _binaryContent;  }
            set
            {
                _binaryContent = value;
                updateMD5();
            }
        }

        public string MD5Hash { get; private set; }

        private void updateMD5()
        {
            if (_binaryContent == null)
                MD5Hash = null;
            MD5 md5 = MD5.Create();
            byte[] md5result = md5.ComputeHash(_binaryContent);
            MD5Hash = Convert.ToBase64String(md5result);
        }
    }

    public delegate string InformationTypeResolver(BlobStorageContent blobStorageContent);

    public static class FileSystemSupport
    {
        public static async Task<string[]> UploadTemplateContentA(string[] allFiles, IContainerOwner owner, string targetLocation, bool clearOldTarget,
            Action<BlobStorageContent> preprocessor = null, Predicate<BlobStorageContent> contentFilterer = null)
        {
            if (clearOldTarget)
            {
                await StorageSupport.DeleteBlobsFromOwnerTargetA(owner, targetLocation);
            }
            var processedDict = allFiles.Where(file => file.EndsWith(".txt")).Where(File.Exists).ToDictionary(file => Path.GetFullPath(file), file => false);
            List<ErrorItem> errorList = new List<ErrorItem>();
            var fixedContent = allFiles.Where(fileName => fileName.EndsWith(".txt") == false)
                .Select(fileName =>
                        new BlobStorageContent
                        {
                            FileName = fileName,
                            BinaryContent = File.ReadAllBytes(fileName)
                            // BinaryContent = GetBlobContent(fileName, errorList, processedDict)
                        })
                .ToArray();
            if (preprocessor != null)
            {
                foreach (var content in fixedContent)
                {
                    preprocessor(content);
                }
            }
            List<Task> uploadTasks = new List<Task>();
            foreach (var content in fixedContent)
            {
                if (contentFilterer != null && contentFilterer(content) == false)
                {
                    // TODO: Properly implement delete above
                    continue;
                }
                string webtemplatePath = Path.Combine(targetLocation, content.FileName).Replace("\\", "/");
                Console.WriteLine("Uploading: " + webtemplatePath);
                var uploadTask = StorageSupport.UploadOwnerBlobBinaryA(owner, webtemplatePath, content.BinaryContent);
                uploadTasks.Add(uploadTask);
            }
            await Task.WhenAll(uploadTasks);
            return processedDict.Keys.ToArray();
        }
    }
}