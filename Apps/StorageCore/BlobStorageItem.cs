using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheBall.Core.Storage
{
    public class BlobStorageItem
    {
        public string ContentMD5 { get; private set; }
        public string ETag { get; set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public DateTime LastModified { get; private set; }

        public Dictionary<string, string> Metadata { get; private set; }

        public readonly string FileName;
        public readonly string DirectoryName;

        public BlobStorageItem(string name, string contentMd5, string eTag, long length, DateTimeOffset? lastModified) 
            : this(name, contentMd5, eTag, length, lastModified?.UtcDateTime ?? DateTime.MinValue.ToUniversalTime())
        {

        }

        public BlobStorageItem(string name, string contentMd5, string eTag, long length, DateTime lastModified)
        {
            Name = name;
            FileName = Path.GetFileName(Name);
            DirectoryName = Path.GetDirectoryName(Name).Replace(@"\", "/");
            ContentMD5 = contentMd5;
            ETag = eTag;
            Length = length;
            LastModified = lastModified;
            Metadata = new Dictionary<string, string>();
        }

    }
}
