 


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

namespace SQLite.TheBall.Payments { 
		
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
[SerializationType] INTEGER NOT NULL
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
				tableCreationCommands.Add(GroupSubscriptionPlan.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccount.GetCreateTableSQL());
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

			public Table<GroupSubscriptionPlan> GroupSubscriptionPlanTable {
				get {
					return this.GetTable<GroupSubscriptionPlan>();
				}
			}
			public Table<CustomerAccount> CustomerAccountTable {
				get {
					return this.GetTable<CustomerAccount>();
				}
			}
        }

    [Table(Name = "GroupSubscriptionPlan")]
	public class GroupSubscriptionPlan : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GroupSubscriptionPlan(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PlanName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[GroupIDs] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "GroupIDs")] public string GroupIDsData;

        private bool _IsGroupIDsRetrieved = false;
        private bool _IsGroupIDsChanged = false;
        private ObservableCollection<string> _GroupIDs = null;
        public ObservableCollection<string> GroupIDs
        {
            get
            {
                if (!_IsGroupIDsRetrieved)
                {
                    if (GroupIDsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(GroupIDsData);
                        _GroupIDs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _GroupIDs = new ObservableCollection<string>();
						GroupIDsData = Guid.NewGuid().ToString();
						_IsGroupIDsChanged = true;
                    }
                    _IsGroupIDsRetrieved = true;
                    _GroupIDs.CollectionChanged += (sender, args) =>
						{
							GroupIDsData = Guid.NewGuid().ToString();
							_IsGroupIDsChanged = true;
						};
                }
                return _GroupIDs;
            }
            set 
			{ 
				_GroupIDs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsGroupIDsRetrieved = true;
                GroupIDsData = Guid.NewGuid().ToString();
                _IsGroupIDsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsGroupIDsChanged || isInitialInsert)
            {
                var dataToStore = GroupIDs.ToArray();
                GroupIDsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "CustomerAccount")]
	public class CustomerAccount : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CustomerAccount(
[ID] TEXT NOT NULL PRIMARY KEY, 
[StripeID] TEXT NOT NULL, 
[EmailAddress] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ActivePlans] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string StripeID { get; set; }
		// private string _unmodified_StripeID;

		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "ActivePlans")] public string ActivePlansData;

        private bool _IsActivePlansRetrieved = false;
        private bool _IsActivePlansChanged = false;
        private ObservableCollection<string> _ActivePlans = null;
        public ObservableCollection<string> ActivePlans
        {
            get
            {
                if (!_IsActivePlansRetrieved)
                {
                    if (ActivePlansData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ActivePlansData);
                        _ActivePlans = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ActivePlans = new ObservableCollection<string>();
						ActivePlansData = Guid.NewGuid().ToString();
						_IsActivePlansChanged = true;
                    }
                    _IsActivePlansRetrieved = true;
                    _ActivePlans.CollectionChanged += (sender, args) =>
						{
							ActivePlansData = Guid.NewGuid().ToString();
							_IsActivePlansChanged = true;
						};
                }
                return _ActivePlans;
            }
            set 
			{ 
				_ActivePlans = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsActivePlansRetrieved = true;
                ActivePlansData = Guid.NewGuid().ToString();
                _IsActivePlansChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsActivePlansChanged || isInitialInsert)
            {
                var dataToStore = ActivePlans.ToArray();
                ActivePlansData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
 } 
