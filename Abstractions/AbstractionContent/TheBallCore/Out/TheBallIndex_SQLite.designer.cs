 


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


namespace SQLite.TheBall.Index { 
		
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
				tableCreationCommands.Add(IndexingRequest.GetCreateTableSQL());
				tableCreationCommands.Add(QueryRequest.GetCreateTableSQL());
				tableCreationCommands.Add(QueryResultItem.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "IndexingRequest")
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
					
		            return;
		        } 
		        if (updateData.ObjectType == "QueryRequest")
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
					
		            return;
		        } 
		        if (updateData.ObjectType == "QueryResultItem")
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
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "IndexingRequest")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Index.IndexingRequest.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new IndexingRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndexName = serializedObject.IndexName;
					if(serializedObject.ObjectLocations != null)
						serializedObject.ObjectLocations.ForEach(item => objectToAdd.ObjectLocations.Add(item));
					IndexingRequestTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "QueryRequest")
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
					QueryRequestTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "QueryResultItem")
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
					QueryResultItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Index")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "IndexingRequest")
		        {
		            var objectToDelete = new IndexingRequest {ID = deleteData.ID};
                    IndexingRequestTable.Attach(objectToDelete);
                    IndexingRequestTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "QueryRequest")
		        {
		            var objectToDelete = new QueryRequest {ID = deleteData.ID};
                    QueryRequestTable.Attach(objectToDelete);
                    QueryRequestTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "QueryResultItem")
		        {
		            var objectToDelete = new QueryResultItem {ID = deleteData.ID};
                    QueryResultItemTable.Attach(objectToDelete);
                    QueryResultItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		    }


			public Table<IndexingRequest> IndexingRequestTable {
				get {
					return this.GetTable<IndexingRequest>();
				}
			}
			public Table<QueryRequest> QueryRequestTable {
				get {
					return this.GetTable<QueryRequest>();
				}
			}
			public Table<QueryResultItem> QueryResultItemTable {
				get {
					return this.GetTable<QueryResultItem>();
				}
			}
        }

    [Table(Name = "IndexingRequest")]
	[ScaffoldTable(true)]
	public class IndexingRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [IndexingRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IndexName] TEXT NOT NULL, 
[ObjectLocations] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public IndexingRequest() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;
        [Column(Name = "ObjectLocations")] 
        [ScaffoldColumn(true)]
		public string ObjectLocationsData { get; set; }

        private bool _IsObjectLocationsRetrieved = false;
        private bool _IsObjectLocationsChanged = false;
        private ObservableCollection<string> _ObjectLocations = null;
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
    [Table(Name = "QueryRequest")]
	[ScaffoldTable(true)]
	public class QueryRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueryRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[QueryString] TEXT NOT NULL, 
[DefaultFieldName] TEXT NOT NULL, 
[IndexName] TEXT NOT NULL, 
[IsQueryCompleted] INTEGER NOT NULL, 
[LastRequestTime] TEXT NOT NULL, 
[LastCompletionTime] TEXT NOT NULL, 
[LastCompletionDurationMs] INTEGER NOT NULL, 
[QueryResultObjectsID] TEXT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public QueryRequest() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string QueryString { get; set; }
		// private string _unmodified_QueryString;

		[Column]
        [ScaffoldColumn(true)]
		public string DefaultFieldName { get; set; }
		// private string _unmodified_DefaultFieldName;

		[Column]
        [ScaffoldColumn(true)]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsQueryCompleted { get; set; }
		// private bool _unmodified_IsQueryCompleted;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime LastRequestTime { get; set; }
		// private DateTime _unmodified_LastRequestTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime LastCompletionTime { get; set; }
		// private DateTime _unmodified_LastCompletionTime;

		[Column]
        [ScaffoldColumn(true)]
		public long LastCompletionDurationMs { get; set; }
		// private long _unmodified_LastCompletionDurationMs;
        [Column(Name = "QueryResultObjects")] 
        [ScaffoldColumn(true)]
		public string QueryResultObjectsData { get; set; }

        private bool _IsQueryResultObjectsRetrieved = false;
        private bool _IsQueryResultObjectsChanged = false;
        private ObservableCollection<SER.TheBall.Index.QueryResultItem> _QueryResultObjects = null;
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
    [Table(Name = "QueryResultItem")]
	[ScaffoldTable(true)]
	public class QueryResultItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueryResultItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ObjectDomainName] TEXT NOT NULL, 
[ObjectName] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL, 
[Rank] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public QueryResultItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ObjectDomainName { get; set; }
		// private string _unmodified_ObjectDomainName;

		[Column]
        [ScaffoldColumn(true)]
		public string ObjectName { get; set; }
		// private string _unmodified_ObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string ObjectID { get; set; }
		// private string _unmodified_ObjectID;

		[Column]
        [ScaffoldColumn(true)]
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
