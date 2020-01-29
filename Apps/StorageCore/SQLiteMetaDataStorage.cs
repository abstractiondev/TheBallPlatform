using System.Collections.Generic;
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
                builder.Property(e => e.Metadata)
                    .HasConversion(obj =>
                            obj.Count == 0 ? null : JsonConvert.SerializeObject(obj),
                        serObj =>
                            serObj == null
                                ? new Dictionary<string, string>()
                                : JsonConvert.DeserializeObject<Dictionary<string, string>>(serObj));
                builder.HasIndex(e => e.ContentMD5);
            }
        }

        public readonly string SQLiteDBPath;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite($"Filename={SQLiteDBPath}");
        }

        public static SQLiteMetaDataContext CreateOrAttachToExistingDB(string pathToDBFile)
        {
            var sqliteConnectionString = $"{pathToDBFile}";
            var dataContext = new SQLiteMetaDataContext(sqliteConnectionString);
            var db = dataContext.Database;
            //db.OpenConnection();
            using (var transaction = db.BeginTransaction())
            {
                //dataContext.CreateDomainDatabaseTablesIfNotExists();
                transaction.Commit();
            }
            return dataContext;
        }

        public SQLiteMetaDataContext(string sqLiteDBPath) : base()
        {
            SQLiteDBPath = sqLiteDBPath;
        }


        public DbSet<BlobStorageItem> BlobStorageItems { get; set; }

    }
}