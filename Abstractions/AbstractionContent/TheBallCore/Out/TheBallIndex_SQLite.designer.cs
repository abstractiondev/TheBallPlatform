 


using System;
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
		[Column]
		public string ID { get; set; }


		[Column]
		public string IndexName { get; set; }
		// private string _unmodified_IndexName;
        [Column(Name = "ObjectLocations")] public byte[] ObjectLocationsData;

		private bool _IsObjectLocationsUsed = false;
        private List<string> _ObjectLocations = null;
        public List<string> ObjectLocations
        {
            get
            {
                if (_ObjectLocations == null && ObjectLocationsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ObjectLocationsData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _ObjectLocations = new List<string>(objectArray);
					_IsObjectLocationsUsed = true;
                }
                return _ObjectLocations;
            }
            set { _ObjectLocations = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsObjectLocationsUsed)
            {
                if (_ObjectLocations == null)
                    ObjectLocationsData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ObjectLocations.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ObjectLocationsData = memStream.ToArray();
                    }
                }
            }

		}
	}
    [Table(Name = "QueryRequest")]
	public class QueryRequest : ITheBallDataContextStorable
	{
		[Column]
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
        [Column(Name = "QueryResultObjects")] public byte[] QueryResultObjectsData;

		private bool _IsQueryResultObjectsUsed = false;
        private List<QueryResultItem> _QueryResultObjects = null;
        public List<QueryResultItem> QueryResultObjects
        {
            get
            {
                if (_QueryResultObjects == null && QueryResultObjectsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    QueryResultItem[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(QueryResultObjectsData))
                        objectArray = (QueryResultItem[]) bf.Deserialize(memStream);
                    _QueryResultObjects = new List<QueryResultItem>(objectArray);
					_IsQueryResultObjectsUsed = true;
                }
                return _QueryResultObjects;
            }
            set { _QueryResultObjects = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsQueryResultObjectsUsed)
            {
                if (_QueryResultObjects == null)
                    QueryResultObjectsData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _QueryResultObjects.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        QueryResultObjectsData = memStream.ToArray();
                    }
                }
            }

		}
	}
    [Table(Name = "QueryResultItem")]
	public class QueryResultItem : ITheBallDataContextStorable
	{
		[Column]
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
