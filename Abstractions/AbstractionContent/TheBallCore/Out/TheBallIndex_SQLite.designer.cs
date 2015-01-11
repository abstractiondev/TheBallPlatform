 


using System;
using System.Collections.ObjectModel;
using System.Data;
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
		void PrepareForStoring();
	}


		public class TheBallDataContext : DataContext
		{

            public TheBallDataContext(IDbConnection connection) : base(connection)
		    {

		    }

            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var requiringBeforeSaveProcessing = changeSet.Inserts.Concat(changeSet.Updates).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in requiringBeforeSaveProcessing)
                    itemToProcess.PrepareForStoring();
                base.SubmitChanges(failureMode);
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

        public void PrepareForStoring()
        {
		
            if (_IsObjectLocationsChanged)
            {
                var dataToStore = _ObjectLocations.ToArray();
                ObjectLocationsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "QueryRequest")]
	public class QueryRequest : ITheBallDataContextStorable
	{
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

        public void PrepareForStoring()
        {
		
            if (_IsQueryResultObjectsChanged)
            {
                var dataToStore = _QueryResultObjects.ToArray();
                QueryResultObjectsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "QueryResultItem")]
	public class QueryResultItem : ITheBallDataContextStorable
	{
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
        public void PrepareForStoring()
        {
		
		}
	}
 } 
