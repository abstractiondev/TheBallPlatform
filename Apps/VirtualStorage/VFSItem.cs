using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProtoBuf;

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


    public class VFSContext : DbContext
    {
        public readonly string DbPath;
        public static async Task<VFSContext> CreateContext(string sqliteDbPath)
        {
            bool createDatabase = !File.Exists(sqliteDbPath);
            var ctx = new VFSContext(sqliteDbPath);
            await ctx.Database.EnsureCreatedAsync();
            return ctx;
        }

        public VFSContext(string dbPath, DbContextOptions<VFSContext> options = null) : base(options)
        {
            DbPath = dbPath;
        }

        public DbSet<FileNameData> FileNameDataTable { get; set; }
        public DbSet<ContentStorageData> ContentStorageDataTable { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = $"Data Source={DbPath}";
            optionsBuilder.UseSqlite(connStr, options =>
            {
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    [Table("FileNameData")]
    public class FileNameData
    {
        [Key]
        public string FileName { get; set; }

        [ForeignKey("ContentStorageData")]
        public string ContentMD5 { get; set; }

        [NotMapped]
        public string StorageFileName => ContentStorageData.getStorageFileName(ContentMD5);

        public ContentStorageData ContentStorageData { get; set; }
    }

    [Table("ContentStorageData")]
    public class ContentStorageData
    {
        [MaxLength(24)]
        [Key]
        public string ContentMD5 { get; set; }

        [NotMapped]
        public string StorageFileName => getStorageFileName(ContentMD5);

        [MaxLength(24)]
        public long ContentLength { get; set; }

        internal static string getStorageFileName(string contentMD5)
        {
            return contentMD5.Replace("+", "-").Replace("/", "_");
        }

        public ContentStorageData()
        {
            FileNames = new List<FileNameData>();
        }

        [InverseProperty("ContentStorageData")]
        public List<FileNameData> FileNames { get; set; }
    }
}