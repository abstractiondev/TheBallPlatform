 


using System;
using System.Collections.ObjectModel;
using System.Data;
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

namespace SQLite.TheBall.Index { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		[Flags]
		public enum SerializationType 
		{
			Undefined = 0,
			XML = 1,
			JSON = 2,
			XML_AND_JSON = XML | JSON
		}

		[Table]
		public class InformationObjectMetaData
		{
			[Column(IsPrimaryKey = true)]
			public string ID { get; set; }

			[Column]
			public string SemanticDomain { get; set; }
			[Column]
			public string ObjectType { get; set; }
			[Column]
			public string ObjectID { get; set; }
			[Column]
			public string MD5 { get; set; }
			[Column]
			public string LastWriteTime { get; set; }
			[Column]
			public long FileLength { get; set; }
			[Column]
			public SerializationType SerializationType { get; set; }

            public ChangeAction CurrentChangeAction { get; set; }
		}


		public class TheBallDataContext : DataContext
		{

		    public static string[] GetMetaDataTableCreateSQLs()
		    {
		        return new string[]
		        {
		            @"
CREATE TABLE IF NOT EXISTS InformationObjectMetaData(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SemanticDomain] TEXT NOT NULL, 
[ObjectType] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL,
[MD5] TEXT NOT NULL,
[LastWriteTime] TEXT NOT NULL,
[FileLength] INTEGER NOT NULL,
[SerializationType] INTEGER NOT NULL,
)",
		            @"
CREATE UNIQUE INDEX ObjectIX ON InformationObjectMetaData (
SemanticDomain, 
ObjectType, 
ObjectID
)"
		        };
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
                tableCreationCommands.AddRange(GetMetaDataTableCreateSQLs());
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
	public class IndexingRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS IndexingRequest(
[ID] TEXT NOT NULL PRIMARY KEY, 
[IndexName] TEXT NOT NULL, 
[ObjectLocations] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;
        [Column(Name = "ObjectLocations")] public string ObjectLocationsData;

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
		
            if (_IsObjectLocationsChanged || isInitialInsert)
            {
                var dataToStore = ObjectLocations.ToArray();
                ObjectLocationsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "QueryRequest")]
	public class QueryRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS QueryRequest(
[ID] TEXT NOT NULL PRIMARY KEY, 
[QueryString] TEXT NOT NULL, 
[DefaultFieldName] TEXT NOT NULL, 
[IndexName] TEXT NOT NULL, 
[IsQueryCompleted] INTEGER NOT NULL, 
[LastRequestTime] TEXT NOT NULL, 
[LastCompletionTime] TEXT NOT NULL, 
[LastCompletionDurationMs] INTEGER NOT NULL, 
[QueryResultObjects] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string QueryString { get; set; }
		// private string _unmodified_QueryString;

		[Column]
		public string DefaultFieldName { get; set; }
		// private string _unmodified_DefaultFieldName;

		[Column]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;

		[Column]
		public bool IsQueryCompleted { get; set; }
		// private bool _unmodified_IsQueryCompleted;

		[Column]
		public DateTime LastRequestTime { get; set; }
		// private DateTime _unmodified_LastRequestTime;

		[Column]
		public DateTime LastCompletionTime { get; set; }
		// private DateTime _unmodified_LastCompletionTime;

		[Column]
		public long LastCompletionDurationMs { get; set; }
		// private long _unmodified_LastCompletionDurationMs;
        [Column(Name = "QueryResultObjects")] public string QueryResultObjectsData;

        private bool _IsQueryResultObjectsRetrieved = false;
        private bool _IsQueryResultObjectsChanged = false;
        private ObservableCollection<QueryResultItem> _QueryResultObjects = null;
        public ObservableCollection<QueryResultItem> QueryResultObjects
        {
            get
            {
                if (!_IsQueryResultObjectsRetrieved)
                {
                    if (QueryResultObjectsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<QueryResultItem[]>(QueryResultObjectsData);
                        _QueryResultObjects = new ObservableCollection<QueryResultItem>(arrayData);
                    }
                    else
                    {
                        _QueryResultObjects = new ObservableCollection<QueryResultItem>();
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
		
            if (_IsQueryResultObjectsChanged || isInitialInsert)
            {
                var dataToStore = QueryResultObjects.ToArray();
                QueryResultObjectsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "QueryResultItem")]
	public class QueryResultItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS QueryResultItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ObjectDomainName] TEXT NOT NULL, 
[ObjectName] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL, 
[Rank] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ObjectDomainName { get; set; }
		// private string _unmodified_ObjectDomainName;

		[Column]
		public string ObjectName { get; set; }
		// private string _unmodified_ObjectName;

		[Column]
		public string ObjectID { get; set; }
		// private string _unmodified_ObjectID;

		[Column]
		public double Rank { get; set; }
		// private double _unmodified_Rank;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
 } 
