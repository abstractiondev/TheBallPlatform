using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheBall.CORE.Storage
{
    public class BlobStorageItem
    {
        public string ContentMD5 { get; private set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public DateTime LastModified { get; private set; }

        public readonly string FileName;
        public readonly string DirectoryName;

        public BlobStorageItem(string name, string contentMd5, long length, DateTime lastModified)
        {
            Name = name;
            FileName = Path.GetFileName(Name);
            DirectoryName = Path.GetDirectoryName(Name).Replace(@"\", "/");
            ContentMD5 = contentMd5;
            Length = length;
            LastModified = lastModified;
        }
        
    }
}
