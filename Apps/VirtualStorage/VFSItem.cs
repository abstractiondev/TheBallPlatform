using System.Collections.Generic;
using ProtoBuf;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace TheBall.Support.VirtualStorage
{
    [ProtoContract]
    public class VFSItem
    {
        [ProtoMember(1)]
        public string FileName { get; set; }

        [ProtoMember(2)]
        public string StorageFileName { get; set; }

        [ProtoMember(3)]
        public string ContentMD5 { get; set; }

        [ProtoMember(4)]
        public long ContentLength { get; set; }
        //public DateTime LastModifiedTime;
    }

    [Table("FileNameData")]
    public class FileNameData
    {
        [PrimaryKey]
        public string FileName { get; set; }

        [ForeignKey(typeof(ContentStorageData))]
        public string ContentMD5 { get; set; }

        [Ignore]
        public string StorageFileName => ContentStorageData.getStorageFileName(ContentMD5);

        [ManyToOne]
        public ContentStorageData ContentStorageData { get; set; }
    }

    [Table("ContentStorageData")]
    public class ContentStorageData
    {
        [MaxLength(24)]
        [PrimaryKey]
        public string ContentMD5 { get; set; }

        [Ignore]
        public string StorageFileName => getStorageFileName(ContentMD5);

        [MaxLength(24)]
        public long ContentLength { get; set; }

        internal static string getStorageFileName(string contentMD5)
        {
            return contentMD5.Replace("+", "-").Replace("/", "_");
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<FileNameData>  FileNames { get; set; }
    }
}