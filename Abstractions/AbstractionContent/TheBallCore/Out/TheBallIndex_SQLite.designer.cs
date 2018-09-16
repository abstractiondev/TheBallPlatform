 


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
using System.ComponentModel.DataAnnotations.Schema;
using Key=System.ComponentModel.DataAnnotations.KeyAttribute;
//using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
//using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SQLite.TheBall.Index { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DbContext, IStorageSyncableDataContext
		{
		    protected override void OnModelCreating(ModelBuilder modelBuilder)
		    {
				IndexingRequest.EntityConfig(modelBuilder);
				QueryRequest.EntityConfig(modelBuilder);
				QueryResultItem.EntityConfig(modelBuilder);

		    }

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
				tableCreationCommands.Add(IndexingRequest.GetCreateTableSQL());
				tableCreationCommands.Add(QueryRequest.GetCreateTableSQL());
				tableCreationCommands.Add(QueryResultItem.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "IndexingRequest":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.IndexingRequest.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = IndexingRequestTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IndexName = serializedObject.IndexName;
                    existingObject.ObjectLocations.Clear();
					if(serializedObject.ObjectLocations != null)
	                    serializedObject.ObjectLocations.ForEach(item => existingObject.ObjectLocations.Add(item));
					
		            break;
		        } 
		        case "QueryRequest":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.QueryRequest.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = QueryRequestTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.QueryString = serializedObject.QueryString;
		            existingObject.DefaultFieldName = serializedObject.DefaultFieldName;
		            existingObject.IndexName = serializedObject.IndexName;
		            existingObject.IsQueryCompleted = serializedObject.IsQueryCompleted;
		            existingObject.LastRequestTime = serializedObject.LastRequestTime;
		            existingObject.LastCompletionTime = serializedObject.LastCompletionTime;
		            existingObject.LastCompletionDurationMs = serializedObject.LastCompletionDurationMs;
                    existingObject.QueryResultObjects.Clear();
					if(serializedObject.QueryResultObjects != null)
	                    serializedObject.QueryResultObjects.ForEach(item => existingObject.QueryResultObjects.Add(item));
					
		            break;
		        } 
		        case "QueryResultItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.QueryResultItem.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = QueryResultItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ObjectDomainName = serializedObject.ObjectDomainName;
		            existingObject.ObjectName = serializedObject.ObjectName;
		            existingObject.ObjectID = serializedObject.ObjectID;
		            existingObject.Rank = serializedObject.Rank;
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "IndexingRequest":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.IndexingRequest.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = IndexingRequestTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IndexName = serializedObject.IndexName;
                    existingObject.ObjectLocations.Clear();
					if(serializedObject.ObjectLocations != null)
	                    serializedObject.ObjectLocations.ForEach(item => existingObject.ObjectLocations.Add(item));
					
		            break;
		        } 
		        case "QueryRequest":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.QueryRequest.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = QueryRequestTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.QueryString = serializedObject.QueryString;
		            existingObject.DefaultFieldName = serializedObject.DefaultFieldName;
		            existingObject.IndexName = serializedObject.IndexName;
		            existingObject.IsQueryCompleted = serializedObject.IsQueryCompleted;
		            existingObject.LastRequestTime = serializedObject.LastRequestTime;
		            existingObject.LastCompletionTime = serializedObject.LastCompletionTime;
		            existingObject.LastCompletionDurationMs = serializedObject.LastCompletionDurationMs;
                    existingObject.QueryResultObjects.Clear();
					if(serializedObject.QueryResultObjects != null)
	                    serializedObject.QueryResultObjects.ForEach(item => existingObject.QueryResultObjects.Add(item));
					
		            break;
		        } 
		        case "QueryResultItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Index.QueryResultItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = QueryResultItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ObjectDomainName = serializedObject.ObjectDomainName;
		            existingObject.ObjectName = serializedObject.ObjectName;
		            existingObject.ObjectID = serializedObject.ObjectID;
		            existingObject.Rank = serializedObject.Rank;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "IndexingRequest":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.IndexingRequest.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new IndexingRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndexName = serializedObject.IndexName;
					if(serializedObject.ObjectLocations != null)
						serializedObject.ObjectLocations.ForEach(item => objectToAdd.ObjectLocations.Add(item));
					IndexingRequestTable.Add(objectToAdd);
                    break;
                }
                case "QueryRequest":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.QueryRequest.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new QueryRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.QueryString = serializedObject.QueryString;
		            objectToAdd.DefaultFieldName = serializedObject.DefaultFieldName;
		            objectToAdd.IndexName = serializedObject.IndexName;
		            objectToAdd.IsQueryCompleted = serializedObject.IsQueryCompleted;
		            objectToAdd.LastRequestTime = serializedObject.LastRequestTime;
		            objectToAdd.LastCompletionTime = serializedObject.LastCompletionTime;
		            objectToAdd.LastCompletionDurationMs = serializedObject.LastCompletionDurationMs;
					if(serializedObject.QueryResultObjects != null)
						serializedObject.QueryResultObjects.ForEach(item => objectToAdd.QueryResultObjects.Add(item));
					QueryRequestTable.Add(objectToAdd);
                    break;
                }
                case "QueryResultItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.QueryResultItem.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new QueryResultItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ObjectDomainName = serializedObject.ObjectDomainName;
		            objectToAdd.ObjectName = serializedObject.ObjectName;
		            objectToAdd.ObjectID = serializedObject.ObjectID;
		            objectToAdd.Rank = serializedObject.Rank;
					QueryResultItemTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "IndexingRequest":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.IndexingRequest.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new IndexingRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndexName = serializedObject.IndexName;
					if(serializedObject.ObjectLocations != null)
						serializedObject.ObjectLocations.ForEach(item => objectToAdd.ObjectLocations.Add(item));
					IndexingRequestTable.Add(objectToAdd);
                    break;
                }
                case "QueryRequest":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.QueryRequest.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new QueryRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.QueryString = serializedObject.QueryString;
		            objectToAdd.DefaultFieldName = serializedObject.DefaultFieldName;
		            objectToAdd.IndexName = serializedObject.IndexName;
		            objectToAdd.IsQueryCompleted = serializedObject.IsQueryCompleted;
		            objectToAdd.LastRequestTime = serializedObject.LastRequestTime;
		            objectToAdd.LastCompletionTime = serializedObject.LastCompletionTime;
		            objectToAdd.LastCompletionDurationMs = serializedObject.LastCompletionDurationMs;
					if(serializedObject.QueryResultObjects != null)
						serializedObject.QueryResultObjects.ForEach(item => objectToAdd.QueryResultObjects.Add(item));
					QueryRequestTable.Add(objectToAdd);
                    break;
                }
                case "QueryResultItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.QueryResultItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new QueryResultItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ObjectDomainName = serializedObject.ObjectDomainName;
		            objectToAdd.ObjectName = serializedObject.ObjectName;
		            objectToAdd.ObjectID = serializedObject.ObjectID;
		            objectToAdd.Rank = serializedObject.Rank;
					QueryResultItemTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "IndexingRequest":
					{
						//var objectToDelete = new IndexingRequest {ID = deleteData.ObjectID};
						//IndexingRequestTable.Attach(objectToDelete);
						var objectToDelete = IndexingRequestTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							IndexingRequestTable.Remove(objectToDelete);
						break;
					}
					case "QueryRequest":
					{
						//var objectToDelete = new QueryRequest {ID = deleteData.ObjectID};
						//QueryRequestTable.Attach(objectToDelete);
						var objectToDelete = QueryRequestTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							QueryRequestTable.Remove(objectToDelete);
						break;
					}
					case "QueryResultItem":
					{
						//var objectToDelete = new QueryResultItem {ID = deleteData.ObjectID};
						//QueryResultItemTable.Attach(objectToDelete);
						var objectToDelete = QueryResultItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							QueryResultItemTable.Remove(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "IndexingRequest":
					{
						//var objectToDelete = new IndexingRequest {ID = deleteData.ObjectID};
						//IndexingRequestTable.Attach(objectToDelete);
						var objectToDelete = IndexingRequestTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							IndexingRequestTable.Remove(objectToDelete);
						break;
					}
					case "QueryRequest":
					{
						//var objectToDelete = new QueryRequest {ID = deleteData.ObjectID};
						//QueryRequestTable.Attach(objectToDelete);
						var objectToDelete = QueryRequestTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							QueryRequestTable.Remove(objectToDelete);
						break;
					}
					case "QueryResultItem":
					{
						//var objectToDelete = new QueryResultItem {ID = deleteData.ObjectID};
						//QueryResultItemTable.Attach(objectToDelete);
						var objectToDelete = QueryResultItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							QueryResultItemTable.Remove(objectToDelete);
						break;
					}
				}
			}



			public DbSet<IndexingRequest> IndexingRequestTable { get; set; }
			public DbSet<QueryRequest> QueryRequestTable { get; set; }
			public DbSet<QueryResultItem> QueryResultItemTable { get; set; }
        }

    [Table("IndexingRequest")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("IndexingRequest: {ID}")]
	public class IndexingRequest : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public IndexingRequest() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [IndexingRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IndexName] TEXT DEFAULT '', 
[ObjectLocations] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;
        //[Column(Name = "ObjectLocations")] 
        //[ScaffoldColumn(true)]
		public string ObjectLocationsData { get; set; }

        private bool _IsObjectLocationsRetrieved = false;
        private bool _IsObjectLocationsChanged = false;
        private ObservableCollection<string> _ObjectLocations = null;
        [NotMapped]
		public ObservableCollection<string> ObjectLocations
        {
            get
            {
                if (!_IsObjectLocationsRetrieved)
                {
                    if (ObjectLocationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ObjectLocationsData);
                        _ObjectLocations = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ObjectLocations = new ObservableCollection<string>();
						ObjectLocationsData = Guid.NewGuid().ToString();
						_IsObjectLocationsChanged = true;
                    }
                    _IsObjectLocationsRetrieved = true;
                    _ObjectLocations.CollectionChanged += (sender, args) =>
						{
							ObjectLocationsData = Guid.NewGuid().ToString();
							_IsObjectLocationsChanged = true;
						};
                }
                return _ObjectLocations;
            }
            set 
			{ 
				_ObjectLocations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsObjectLocationsRetrieved = true;
                ObjectLocationsData = Guid.NewGuid().ToString();
                _IsObjectLocationsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(IndexName == null)
				IndexName = string.Empty;
            if (_IsObjectLocationsChanged || isInitialInsert)
            {
                var dataToStore = ObjectLocations.ToArray();
                ObjectLocationsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table("QueryRequest")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("QueryRequest: {ID}")]
	public class QueryRequest : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public QueryRequest() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueryRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[QueryString] TEXT DEFAULT '', 
[DefaultFieldName] TEXT DEFAULT '', 
[IndexName] TEXT DEFAULT '', 
[IsQueryCompleted] INTEGER NOT NULL, 
[LastRequestTime] TEXT DEFAULT '', 
[LastCompletionTime] TEXT DEFAULT '', 
[LastCompletionDurationMs] INTEGER NOT NULL, 
[QueryResultObjectsID] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string QueryString { get; set; }
		// private string _unmodified_QueryString;

		//[Column]
        //[ScaffoldColumn(true)]
		public string DefaultFieldName { get; set; }
		// private string _unmodified_DefaultFieldName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool IsQueryCompleted { get; set; }
		// private bool _unmodified_IsQueryCompleted;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime LastRequestTime { get; set; }
		// private DateTime _unmodified_LastRequestTime;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime LastCompletionTime { get; set; }
		// private DateTime _unmodified_LastCompletionTime;

		//[Column]
        //[ScaffoldColumn(true)]
		public long LastCompletionDurationMs { get; set; }
		// private long _unmodified_LastCompletionDurationMs;
        //[Column(Name = "QueryResultObjects")] 
        //[ScaffoldColumn(true)]
		public string QueryResultObjectsData { get; set; }

        private bool _IsQueryResultObjectsRetrieved = false;
        private bool _IsQueryResultObjectsChanged = false;
        private ObservableCollection<SER.TheBall.Index.QueryResultItem> _QueryResultObjects = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Index.QueryResultItem> QueryResultObjects
        {
            get
            {
                if (!_IsQueryResultObjectsRetrieved)
                {
                    if (QueryResultObjectsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Index.QueryResultItem[]>(QueryResultObjectsData);
                        _QueryResultObjects = new ObservableCollection<SER.TheBall.Index.QueryResultItem>(arrayData);
                    }
                    else
                    {
                        _QueryResultObjects = new ObservableCollection<SER.TheBall.Index.QueryResultItem>();
						QueryResultObjectsData = Guid.NewGuid().ToString();
						_IsQueryResultObjectsChanged = true;
                    }
                    _IsQueryResultObjectsRetrieved = true;
                    _QueryResultObjects.CollectionChanged += (sender, args) =>
						{
							QueryResultObjectsData = Guid.NewGuid().ToString();
							_IsQueryResultObjectsChanged = true;
						};
                }
                return _QueryResultObjects;
            }
            set 
			{ 
				_QueryResultObjects = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsQueryResultObjectsRetrieved = true;
                QueryResultObjectsData = Guid.NewGuid().ToString();
                _IsQueryResultObjectsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(QueryString == null)
				QueryString = string.Empty;
			if(DefaultFieldName == null)
				DefaultFieldName = string.Empty;
			if(IndexName == null)
				IndexName = string.Empty;
            if (_IsQueryResultObjectsChanged || isInitialInsert)
            {
                var dataToStore = QueryResultObjects.ToArray();
                QueryResultObjectsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table("QueryResultItem")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("QueryResultItem: {ID}")]
	public class QueryResultItem : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public QueryResultItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueryResultItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ObjectDomainName] TEXT DEFAULT '', 
[ObjectName] TEXT DEFAULT '', 
[ObjectID] TEXT DEFAULT '', 
[Rank] REAL NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string ObjectDomainName { get; set; }
		// private string _unmodified_ObjectDomainName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ObjectName { get; set; }
		// private string _unmodified_ObjectName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ObjectID { get; set; }
		// private string _unmodified_ObjectID;

		//[Column]
        //[ScaffoldColumn(true)]
		public double Rank { get; set; }
		// private double _unmodified_Rank;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ObjectDomainName == null)
				ObjectDomainName = string.Empty;
			if(ObjectName == null)
				ObjectName = string.Empty;
			if(ObjectID == null)
				ObjectID = string.Empty;
		}
	}
 } 
