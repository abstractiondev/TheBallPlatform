 


using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using SQLiteSupport;
using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
using ScaffoldTable=System.ComponentModel.DataAnnotations.ScaffoldTableAttribute;
using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;
using System.Diagnostics;


namespace SQLite.Caloom.Housing { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DataContext, IStorageSyncableDataContext
		{
            // Track whether Dispose has been called. 
            private bool disposed = false;
		    protected override void Dispose(bool disposing)
		    {
		        if (disposed)
		            return;
                base.Dispose(disposing);
                GC.Collect();
                GC.WaitForPendingFinalizers();
		        disposed = true;
		    }

            public static Func<DbConnection> GetCurrentConnectionFunc { get; set; }

		    public TheBallDataContext() : base(GetCurrentConnectionFunc())
		    {
		        
		    }

		    public static TheBallDataContext CreateOrAttachToExistingDB(string pathToDBFile)
		    {
		        SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0}", pathToDBFile));
                var dataContext = new TheBallDataContext(connection);
		        using (var transaction = connection.BeginTransaction())
		        {
                    dataContext.CreateDomainDatabaseTablesIfNotExists();
                    transaction.Commit();
		        }
                return dataContext;
		    }

            public TheBallDataContext(SQLiteConnection connection) : base(connection)
		    {
                if(connection.State != ConnectionState.Open)
                    connection.Open();
		    }

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

			public void CreateDomainDatabaseTablesIfNotExists()
			{
				List<string> tableCreationCommands = new List<string>();
                tableCreationCommands.AddRange(InformationObjectMetaData.GetMetaDataTableCreateSQLs());
				tableCreationCommands.Add(House.GetCreateTableSQL());
			    var connection = this.Connection;
				foreach (string commandText in tableCreationCommands)
			    {
			        var command = connection.CreateCommand();
			        command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
			        command.ExecuteNonQuery();
			    }
			}

			public Table<InformationObjectMetaData> InformationObjectMetaDataTable {
				get {
					return this.GetTable<InformationObjectMetaData>();
				}
			}

			public void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "Caloom.Housing")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "House")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.Housing.House.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = HouseTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "Caloom.Housing")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "House")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.Housing.House.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new House {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
					HouseTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "Caloom.Housing")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "House")
		        {
		            var objectToDelete = new House {ID = deleteData.ID};
                    HouseTable.Attach(objectToDelete);
                    HouseTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		    }


			public Table<House> HouseTable {
				get {
					return this.GetTable<House>();
				}
			}
        }

    [Table(Name = "House")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("House: {ID}")]
	public class House : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public House() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [House](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
 } 
