 


using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using SQLiteSupport;
using Key=System.ComponentModel.DataAnnotations.KeyAttribute;
//using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
//using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SQLite.TheBall.Infrastructure { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DbContext, IStorageSyncableDataContext
		{
            // Track whether Dispose has been called. 
            private bool disposed = false;
		    void IDisposable.Dispose()
		    {
		        if (disposed)
		            return;
                GC.Collect();
                GC.WaitForPendingFinalizers();
		        disposed = true;
		    }

            public static Func<DbConnection> GetCurrentConnectionFunc { get; set; }

		    public TheBallDataContext() : base(new DbContextOptions<TheBallDataContext>())
		    {
		        
		    }

		    public readonly string SQLiteDBPath;
		    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		    {
		        optionsBuilder.UseSqlite($"Filename={SQLiteDBPath}");
		    }

		    public static TheBallDataContext CreateOrAttachToExistingDB(string pathToDBFile)
		    {
		        var sqliteConnectionString = $"{pathToDBFile}";
                var dataContext = new TheBallDataContext(sqliteConnectionString);
		        var db = dataContext.Database;
                db.OpenConnection();
		        using (var transaction = db.BeginTransaction())
		        {
                    dataContext.CreateDomainDatabaseTablesIfNotExists();
                    transaction.Commit();
		        }
                return dataContext;
		    }

            public TheBallDataContext(string sqLiteDBPath) : base()
            {
                SQLiteDBPath = sqLiteDBPath;
            }

		    public override int SaveChanges(bool acceptAllChangesOnSuccess)
		    {
                //if(connection.State != ConnectionState.Open)
                //    connection.Open();
		        return base.SaveChanges(acceptAllChangesOnSuccess);
		    }

			/*
            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var insertsToProcess = changeSet.Inserts.Where(insert => insert is ITheBallDataContextStorable).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in insertsToProcess)
                    itemToProcess.PrepareForStoring(true);
                var updatesToProcess = changeSet.Updates.Where(update => update is ITheBallDataContextStorable).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in updatesToProcess)
                    itemToProcess.PrepareForStoring(false);
                base.SubmitChanges(failureMode);
            }

		    public async Task SubmitChangesAsync()
		    {
		        await Task.Run(() => SubmitChanges());
		    }
			*/

			public void CreateDomainDatabaseTablesIfNotExists()
			{
				List<string> tableCreationCommands = new List<string>();
                tableCreationCommands.AddRange(InformationObjectMetaData.GetMetaDataTableCreateSQLs());
			    //var connection = this.Connection;
			    var db = this.Database;
			    var connection = db.GetDbConnection();
				foreach (string commandText in tableCreationCommands)
			    {
			        var command = connection.CreateCommand();
			        command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
			        command.ExecuteNonQuery();
			    }
			}

			public DbSet<InformationObjectMetaData> InformationObjectMetaDataTable {
				get {
					return this.Set<InformationObjectMetaData>();
				}
			}


			public void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Infrastructure")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
				}
			}



        }

 } 
