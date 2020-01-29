using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class SQLiteMetaDataContext : DbContext
    {
        public class BlobStorageItemEFConf : IEntityTypeConfiguration<BlobStorageItem>
        {
            public void Configure(EntityTypeBuilder<BlobStorageItem> builder)
            {
                builder.HasKey(e => e.Name);
                builder.Property(e => e.ETag);
                builder.Property(e => e.Length);
                builder.Property(e => e.LastModified);
                builder.Property(e => e.Metadata)
                    .HasConversion(obj =>
                            obj.Count == 0 ? null : JsonConvert.SerializeObject(obj),
                        serObj =>
                            serObj == null
                                ? new Dictionary<string, string>()
                                : JsonConvert.DeserializeObject<Dictionary<string, string>>(serObj))
                    .HasColumnType("TEXT");
                builder.HasIndex(e => e.ContentMD5);
            }
        }

        public readonly string SQLiteDBPath = ":memory:";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={SQLiteDBPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BlobStorageItemEFConf());
        }

        public static async Task<SQLiteMetaDataContext> CreateOrAttachToExistingDB(string pathToDBFile)
        {
            var sqliteConnectionString = $"{pathToDBFile}";
            var dataContext = new SQLiteMetaDataContext(sqliteConnectionString);
            var db = dataContext.Database;
            await db.OpenConnectionAsync();
            await dataContext.Database.MigrateAsync();
            //using (var transaction = db.BeginTransaction())
            {
                //await transaction.CommitAsync();
            }
            return dataContext;
        }

        public SQLiteMetaDataContext()
        {

        }
        public SQLiteMetaDataContext(string sqLiteDBPath) : base()
        {
            SQLiteDBPath = sqLiteDBPath;
        }


        public DbSet<BlobStorageItem> BlobStorageItems { get; set; }

    }
}