using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using TheBall.Core.Storage;

namespace TheBall.Core.StorageCore
{
    public class MetaDataContext : DbContext
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

        public static async Task<MetaDataContext> CreateOrAttachToExistingDB(string pathToDBFile)
        {
            var sqliteConnectionString = pathToDBFile;
            var dataContext = new MetaDataContext(sqliteConnectionString);
            var db = dataContext.Database;
            await db.OpenConnectionAsync();
            await dataContext.Database.MigrateAsync();
            return dataContext;
        }

        public MetaDataContext()
        {

        }
        public MetaDataContext(string sqLiteDBPath, bool isReadOnly = false) : base()
        {
            SQLiteDBPath = sqLiteDBPath;
            IsReadOnly = isReadOnly;
        }
        public bool IsReadOnly { get; }
        private void validateNotReadOnly()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("IsReadOnly context may not save changes");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            validateNotReadOnly();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            validateNotReadOnly();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            validateNotReadOnly();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            validateNotReadOnly();
            return base.SaveChanges();
        }



        public DbSet<BlobStorageItem> BlobStorageItems { get; set; }

    }
}