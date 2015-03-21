 


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


namespace SQLite.TheBall.Payments { 
		
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
				tableCreationCommands.Add(GroupSubscriptionPlan.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccount.GetCreateTableSQL());
				tableCreationCommands.Add(GroupSubscriptionPlanCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccountCollection.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "GroupSubscriptionPlan")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupSubscriptionPlanTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PlanName = serializedObject.PlanName;
		            existingObject.Description = serializedObject.Description;
                    existingObject.GroupIDs.Clear();
					if(serializedObject.GroupIDs != null)
	                    serializedObject.GroupIDs.ForEach(item => existingObject.GroupIDs.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "CustomerAccount")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CustomerAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StripeID = serializedObject.StripeID;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.Description = serializedObject.Description;
                    existingObject.ActivePlans.Clear();
					if(serializedObject.ActivePlans != null)
	                    serializedObject.ActivePlans.ForEach(item => existingObject.ActivePlans.Add(item));
					
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "GroupSubscriptionPlan")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupSubscriptionPlan {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PlanName = serializedObject.PlanName;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.GroupIDs != null)
						serializedObject.GroupIDs.ForEach(item => objectToAdd.GroupIDs.Add(item));
					GroupSubscriptionPlanTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CustomerAccount")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CustomerAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StripeID = serializedObject.StripeID;
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.ActivePlans != null)
						serializedObject.ActivePlans.ForEach(item => objectToAdd.ActivePlans.Add(item));
					CustomerAccountTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "GroupSubscriptionPlan")
		        {
		            var objectToDelete = new GroupSubscriptionPlan {ID = deleteData.ID};
                    GroupSubscriptionPlanTable.Attach(objectToDelete);
                    GroupSubscriptionPlanTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CustomerAccount")
		        {
		            var objectToDelete = new CustomerAccount {ID = deleteData.ID};
                    CustomerAccountTable.Attach(objectToDelete);
                    CustomerAccountTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GroupSubscriptionPlanCollection")
		        {
		            var objectToDelete = new GroupSubscriptionPlanCollection {ID = deleteData.ID};
                    GroupSubscriptionPlanCollectionTable.Attach(objectToDelete);
                    GroupSubscriptionPlanCollectionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CustomerAccountCollection")
		        {
		            var objectToDelete = new CustomerAccountCollection {ID = deleteData.ID};
                    CustomerAccountCollectionTable.Attach(objectToDelete);
                    CustomerAccountCollectionTable.DeleteOnSubmit(objectToDelete);
		            return;
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
			public Table<GroupSubscriptionPlanCollection> GroupSubscriptionPlanCollectionTable {
				get {
					return this.GetTable<GroupSubscriptionPlanCollection>();
				}
			}
			public Table<CustomerAccountCollection> CustomerAccountCollectionTable {
				get {
					return this.GetTable<CustomerAccountCollection>();
				}
			}
        }

    [Table(Name = "GroupSubscriptionPlan")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupSubscriptionPlan: {ID}")]
	public class GroupSubscriptionPlan : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupSubscriptionPlan() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSubscriptionPlan](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PlanName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[GroupIDs] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "GroupIDs")] 
        [ScaffoldColumn(true)]
		public string GroupIDsData { get; set; }

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
		
			if(PlanName == null)
				PlanName = string.Empty;
			if(Description == null)
				Description = string.Empty;
            if (_IsGroupIDsChanged || isInitialInsert)
            {
                var dataToStore = GroupIDs.ToArray();
                GroupIDsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "CustomerAccount")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CustomerAccount: {ID}")]
	public class CustomerAccount : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CustomerAccount() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CustomerAccount](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[StripeID] TEXT NOT NULL, 
[EmailAddress] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ActivePlans] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string StripeID { get; set; }
		// private string _unmodified_StripeID;

		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "ActivePlans")] 
        [ScaffoldColumn(true)]
		public string ActivePlansData { get; set; }

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
		
			if(StripeID == null)
				StripeID = string.Empty;
			if(EmailAddress == null)
				EmailAddress = string.Empty;
			if(Description == null)
				Description = string.Empty;
            if (_IsActivePlansChanged || isInitialInsert)
            {
                var dataToStore = ActivePlans.ToArray();
                ActivePlansData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "GroupSubscriptionPlanCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupSubscriptionPlanCollection: {ID}")]
	public class GroupSubscriptionPlanCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupSubscriptionPlanCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSubscriptionPlanCollection](
[ID] TEXT NOT NULL, 
[CollectionItemID] TEXT NOT NULL, 
[ETag] TEXT NOT NULL,
	PRIMARY KEY (ID) )";
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		}
		//[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "CustomerAccountCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CustomerAccountCollection: {ID}")]
	public class CustomerAccountCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CustomerAccountCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CustomerAccountCollection](
[ID] TEXT NOT NULL, 
[CollectionItemID] TEXT NOT NULL, 
[ETag] TEXT NOT NULL,
	PRIMARY KEY (ID) )";
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		}
		//[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
 } 
