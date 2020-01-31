using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheBall.Core.Storage
{
    public class BlobStorageItem
    {
        public string ContentMD5 { get; set; }
        public string ETag { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public readonly string FileName;
        public readonly string DirectoryName;

        public BlobStorageItem(string name, string contentMd5, string eTag, long length, DateTimeOffset? lastModified) 
            : this(name, contentMd5, eTag, length, lastModified?.UtcDateTime ?? DateTime.MinValue.ToUniversalTime())
        {

        }

        public BlobStorageItem()
        {

        }

        public BlobStorageItem(string name, string contentMd5, string eTag, long length, DateTime lastModified)
        {
            Name = name;
            FileName = Path.GetFileName(Name);
            DirectoryName = Path.GetDirectoryName(Name.Replace('/', Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar, '/');
            ContentMD5 = contentMd5;
            ETag = eTag;
            Length = length;
            LastModified = lastModified;
            Metadata = new Dictionary<string, string>();
        }

    }
}
