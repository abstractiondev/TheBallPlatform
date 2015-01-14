 


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

namespace SQLite.TheBall.Interface { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}


		public class TheBallDataContext : DataContext
		{

            public TheBallDataContext(SQLiteConnection connection) : base(connection)
		    {
                if(connection.State != ConnectionState.Open)
                    connection.Open();
		    }

            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var insertsToProcess = changeSet.Inserts.Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in insertsToProcess)
                    itemToProcess.PrepareForStoring(true);
                var updatesToProcess = changeSet.Updates.Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in updatesToProcess)
                    itemToProcess.PrepareForStoring(false);
                base.SubmitChanges(failureMode);
            }

			public void CreateDomainDatabaseTablesIfNotExists()
			{
				List<string> tableCreationCommands = new List<string>();
				tableCreationCommands.Add(WizardContainer.GetCreateTableSQL());
				tableCreationCommands.Add(WizardTask.GetCreateTableSQL());
				tableCreationCommands.Add(Connection.GetCreateTableSQL());
				tableCreationCommands.Add(TransferPackage.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryLink.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(StatusSummary.GetCreateTableSQL());
				tableCreationCommands.Add(InformationChangeItem.GetCreateTableSQL());
				tableCreationCommands.Add(OperationExecutionItem.GetCreateTableSQL());
				tableCreationCommands.Add(GenericCollectionableObject.GetCreateTableSQL());
				tableCreationCommands.Add(GenericObject.GetCreateTableSQL());
				tableCreationCommands.Add(GenericValue.GetCreateTableSQL());
			    var connection = this.Connection;
				foreach (string commandText in tableCreationCommands)
			    {
			        var command = connection.CreateCommand();
			        command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
			        command.ExecuteNonQuery();
			    }
			}

			public Table<WizardContainer> WizardContainerTable {
				get {
					return this.GetTable<WizardContainer>();
				}
			}
			public Table<WizardTask> WizardTaskTable {
				get {
					return this.GetTable<WizardTask>();
				}
			}
			public Table<Connection> ConnectionTable {
				get {
					return this.GetTable<Connection>();
				}
			}
			public Table<TransferPackage> TransferPackageTable {
				get {
					return this.GetTable<TransferPackage>();
				}
			}
			public Table<CategoryLink> CategoryLinkTable {
				get {
					return this.GetTable<CategoryLink>();
				}
			}
			public Table<Category> CategoryTable {
				get {
					return this.GetTable<Category>();
				}
			}
			public Table<StatusSummary> StatusSummaryTable {
				get {
					return this.GetTable<StatusSummary>();
				}
			}
			public Table<InformationChangeItem> InformationChangeItemTable {
				get {
					return this.GetTable<InformationChangeItem>();
				}
			}
			public Table<OperationExecutionItem> OperationExecutionItemTable {
				get {
					return this.GetTable<OperationExecutionItem>();
				}
			}
			public Table<GenericCollectionableObject> GenericCollectionableObjectTable {
				get {
					return this.GetTable<GenericCollectionableObject>();
				}
			}
			public Table<GenericObject> GenericObjectTable {
				get {
					return this.GetTable<GenericObject>();
				}
			}
			public Table<GenericValue> GenericValueTable {
				get {
					return this.GetTable<GenericValue>();
				}
			}
        }

    [Table(Name = "WizardContainer")]
	public class WizardContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS WizardContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActiveTasks] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "ActiveTasks")] public string ActiveTasksData;

        private bool _IsActiveTasksRetrieved = false;
        private bool _IsActiveTasksChanged = false;
        private ObservableCollection<WizardTask> _ActiveTasks = null;
        public ObservableCollection<WizardTask> ActiveTasks
        {
            get
            {
                if (!_IsActiveTasksRetrieved)
                {
                    if (ActiveTasksData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<WizardTask[]>(ActiveTasksData);
                        _ActiveTasks = new ObservableCollection<WizardTask>(arrayData);
                    }
                    else
                    {
                        _ActiveTasks = new ObservableCollection<WizardTask>();
						ActiveTasksData = Guid.NewGuid().ToString();
						_IsActiveTasksChanged = true;
                    }
                    _IsActiveTasksRetrieved = true;
                    _ActiveTasks.CollectionChanged += (sender, args) =>
						{
							ActiveTasksData = Guid.NewGuid().ToString();
							_IsActiveTasksChanged = true;
						};
                }
                return _ActiveTasks;
            }
            set 
			{ 
				_ActiveTasks = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsActiveTasksRetrieved = true;
                ActiveTasksData = Guid.NewGuid().ToString();
                _IsActiveTasksChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsActiveTasksChanged || isInitialInsert)
            {
                var dataToStore = ActiveTasks.ToArray();
                ActiveTasksData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "WizardTask")]
	public class WizardTask : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS WizardTask(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TaskName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[InputType] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TaskName { get; set; }
		// private string _unmodified_TaskName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string InputType { get; set; }
		// private string _unmodified_InputType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Connection")]
	public class Connection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Connection(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OutputInformationID] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[DeviceID] TEXT NOT NULL, 
[IsActiveParty] INTEGER NOT NULL, 
[OtherSideConnectionID] TEXT NOT NULL, 
[ThisSideCategories] TEXT NOT NULL, 
[OtherSideCategories] TEXT NOT NULL, 
[CategoryLinks] TEXT NOT NULL, 
[IncomingPackages] TEXT NOT NULL, 
[OutgoingPackages] TEXT NOT NULL, 
[OperationNameToListPackageContents] TEXT NOT NULL, 
[OperationNameToProcessReceived] TEXT NOT NULL, 
[OperationNameToUpdateThisSideCategories] TEXT NOT NULL, 
[ProcessIDToListPackageContents] TEXT NOT NULL, 
[ProcessIDToProcessReceived] TEXT NOT NULL, 
[ProcessIDToUpdateThisSideCategories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OutputInformationID { get; set; }
		// private string _unmodified_OutputInformationID;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string DeviceID { get; set; }
		// private string _unmodified_DeviceID;

		[Column]
		public bool IsActiveParty { get; set; }
		// private bool _unmodified_IsActiveParty;

		[Column]
		public string OtherSideConnectionID { get; set; }
		// private string _unmodified_OtherSideConnectionID;
        [Column(Name = "ThisSideCategories")] public string ThisSideCategoriesData;

        private bool _IsThisSideCategoriesRetrieved = false;
        private bool _IsThisSideCategoriesChanged = false;
        private ObservableCollection<Category> _ThisSideCategories = null;
        public ObservableCollection<Category> ThisSideCategories
        {
            get
            {
                if (!_IsThisSideCategoriesRetrieved)
                {
                    if (ThisSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<Category[]>(ThisSideCategoriesData);
                        _ThisSideCategories = new ObservableCollection<Category>(arrayData);
                    }
                    else
                    {
                        _ThisSideCategories = new ObservableCollection<Category>();
						ThisSideCategoriesData = Guid.NewGuid().ToString();
						_IsThisSideCategoriesChanged = true;
                    }
                    _IsThisSideCategoriesRetrieved = true;
                    _ThisSideCategories.CollectionChanged += (sender, args) =>
						{
							ThisSideCategoriesData = Guid.NewGuid().ToString();
							_IsThisSideCategoriesChanged = true;
						};
                }
                return _ThisSideCategories;
            }
            set 
			{ 
				_ThisSideCategories = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsThisSideCategoriesRetrieved = true;
                ThisSideCategoriesData = Guid.NewGuid().ToString();
                _IsThisSideCategoriesChanged = true;

			}
        }

        [Column(Name = "OtherSideCategories")] public string OtherSideCategoriesData;

        private bool _IsOtherSideCategoriesRetrieved = false;
        private bool _IsOtherSideCategoriesChanged = false;
        private ObservableCollection<Category> _OtherSideCategories = null;
        public ObservableCollection<Category> OtherSideCategories
        {
            get
            {
                if (!_IsOtherSideCategoriesRetrieved)
                {
                    if (OtherSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<Category[]>(OtherSideCategoriesData);
                        _OtherSideCategories = new ObservableCollection<Category>(arrayData);
                    }
                    else
                    {
                        _OtherSideCategories = new ObservableCollection<Category>();
						OtherSideCategoriesData = Guid.NewGuid().ToString();
						_IsOtherSideCategoriesChanged = true;
                    }
                    _IsOtherSideCategoriesRetrieved = true;
                    _OtherSideCategories.CollectionChanged += (sender, args) =>
						{
							OtherSideCategoriesData = Guid.NewGuid().ToString();
							_IsOtherSideCategoriesChanged = true;
						};
                }
                return _OtherSideCategories;
            }
            set 
			{ 
				_OtherSideCategories = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsOtherSideCategoriesRetrieved = true;
                OtherSideCategoriesData = Guid.NewGuid().ToString();
                _IsOtherSideCategoriesChanged = true;

			}
        }

        [Column(Name = "CategoryLinks")] public string CategoryLinksData;

        private bool _IsCategoryLinksRetrieved = false;
        private bool _IsCategoryLinksChanged = false;
        private ObservableCollection<CategoryLink> _CategoryLinks = null;
        public ObservableCollection<CategoryLink> CategoryLinks
        {
            get
            {
                if (!_IsCategoryLinksRetrieved)
                {
                    if (CategoryLinksData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<CategoryLink[]>(CategoryLinksData);
                        _CategoryLinks = new ObservableCollection<CategoryLink>(arrayData);
                    }
                    else
                    {
                        _CategoryLinks = new ObservableCollection<CategoryLink>();
						CategoryLinksData = Guid.NewGuid().ToString();
						_IsCategoryLinksChanged = true;
                    }
                    _IsCategoryLinksRetrieved = true;
                    _CategoryLinks.CollectionChanged += (sender, args) =>
						{
							CategoryLinksData = Guid.NewGuid().ToString();
							_IsCategoryLinksChanged = true;
						};
                }
                return _CategoryLinks;
            }
            set 
			{ 
				_CategoryLinks = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsCategoryLinksRetrieved = true;
                CategoryLinksData = Guid.NewGuid().ToString();
                _IsCategoryLinksChanged = true;

			}
        }

        [Column(Name = "IncomingPackages")] public string IncomingPackagesData;

        private bool _IsIncomingPackagesRetrieved = false;
        private bool _IsIncomingPackagesChanged = false;
        private ObservableCollection<TransferPackage> _IncomingPackages = null;
        public ObservableCollection<TransferPackage> IncomingPackages
        {
            get
            {
                if (!_IsIncomingPackagesRetrieved)
                {
                    if (IncomingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(IncomingPackagesData);
                        _IncomingPackages = new ObservableCollection<TransferPackage>(arrayData);
                    }
                    else
                    {
                        _IncomingPackages = new ObservableCollection<TransferPackage>();
						IncomingPackagesData = Guid.NewGuid().ToString();
						_IsIncomingPackagesChanged = true;
                    }
                    _IsIncomingPackagesRetrieved = true;
                    _IncomingPackages.CollectionChanged += (sender, args) =>
						{
							IncomingPackagesData = Guid.NewGuid().ToString();
							_IsIncomingPackagesChanged = true;
						};
                }
                return _IncomingPackages;
            }
            set 
			{ 
				_IncomingPackages = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsIncomingPackagesRetrieved = true;
                IncomingPackagesData = Guid.NewGuid().ToString();
                _IsIncomingPackagesChanged = true;

			}
        }

        [Column(Name = "OutgoingPackages")] public string OutgoingPackagesData;

        private bool _IsOutgoingPackagesRetrieved = false;
        private bool _IsOutgoingPackagesChanged = false;
        private ObservableCollection<TransferPackage> _OutgoingPackages = null;
        public ObservableCollection<TransferPackage> OutgoingPackages
        {
            get
            {
                if (!_IsOutgoingPackagesRetrieved)
                {
                    if (OutgoingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(OutgoingPackagesData);
                        _OutgoingPackages = new ObservableCollection<TransferPackage>(arrayData);
                    }
                    else
                    {
                        _OutgoingPackages = new ObservableCollection<TransferPackage>();
						OutgoingPackagesData = Guid.NewGuid().ToString();
						_IsOutgoingPackagesChanged = true;
                    }
                    _IsOutgoingPackagesRetrieved = true;
                    _OutgoingPackages.CollectionChanged += (sender, args) =>
						{
							OutgoingPackagesData = Guid.NewGuid().ToString();
							_IsOutgoingPackagesChanged = true;
						};
                }
                return _OutgoingPackages;
            }
            set 
			{ 
				_OutgoingPackages = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsOutgoingPackagesRetrieved = true;
                OutgoingPackagesData = Guid.NewGuid().ToString();
                _IsOutgoingPackagesChanged = true;

			}
        }


		[Column]
		public string OperationNameToListPackageContents { get; set; }
		// private string _unmodified_OperationNameToListPackageContents;

		[Column]
		public string OperationNameToProcessReceived { get; set; }
		// private string _unmodified_OperationNameToProcessReceived;

		[Column]
		public string OperationNameToUpdateThisSideCategories { get; set; }
		// private string _unmodified_OperationNameToUpdateThisSideCategories;

		[Column]
		public string ProcessIDToListPackageContents { get; set; }
		// private string _unmodified_ProcessIDToListPackageContents;

		[Column]
		public string ProcessIDToProcessReceived { get; set; }
		// private string _unmodified_ProcessIDToProcessReceived;

		[Column]
		public string ProcessIDToUpdateThisSideCategories { get; set; }
		// private string _unmodified_ProcessIDToUpdateThisSideCategories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsThisSideCategoriesChanged || isInitialInsert)
            {
                var dataToStore = ThisSideCategories.ToArray();
                ThisSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsOtherSideCategoriesChanged || isInitialInsert)
            {
                var dataToStore = OtherSideCategories.ToArray();
                OtherSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsCategoryLinksChanged || isInitialInsert)
            {
                var dataToStore = CategoryLinks.ToArray();
                CategoryLinksData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsIncomingPackagesChanged || isInitialInsert)
            {
                var dataToStore = IncomingPackages.ToArray();
                IncomingPackagesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsOutgoingPackagesChanged || isInitialInsert)
            {
                var dataToStore = OutgoingPackages.ToArray();
                OutgoingPackagesData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "TransferPackage")]
	public class TransferPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TransferPackage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ConnectionID] TEXT NOT NULL, 
[PackageDirection] TEXT NOT NULL, 
[PackageType] TEXT NOT NULL, 
[IsProcessed] INTEGER NOT NULL, 
[PackageContentBlobs] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ConnectionID { get; set; }
		// private string _unmodified_ConnectionID;

		[Column]
		public string PackageDirection { get; set; }
		// private string _unmodified_PackageDirection;

		[Column]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
		public bool IsProcessed { get; set; }
		// private bool _unmodified_IsProcessed;
        [Column(Name = "PackageContentBlobs")] public string PackageContentBlobsData;

        private bool _IsPackageContentBlobsRetrieved = false;
        private bool _IsPackageContentBlobsChanged = false;
        private ObservableCollection<string> _PackageContentBlobs = null;
        public ObservableCollection<string> PackageContentBlobs
        {
            get
            {
                if (!_IsPackageContentBlobsRetrieved)
                {
                    if (PackageContentBlobsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(PackageContentBlobsData);
                        _PackageContentBlobs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _PackageContentBlobs = new ObservableCollection<string>();
						PackageContentBlobsData = Guid.NewGuid().ToString();
						_IsPackageContentBlobsChanged = true;
                    }
                    _IsPackageContentBlobsRetrieved = true;
                    _PackageContentBlobs.CollectionChanged += (sender, args) =>
						{
							PackageContentBlobsData = Guid.NewGuid().ToString();
							_IsPackageContentBlobsChanged = true;
						};
                }
                return _PackageContentBlobs;
            }
            set 
			{ 
				_PackageContentBlobs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsPackageContentBlobsRetrieved = true;
                PackageContentBlobsData = Guid.NewGuid().ToString();
                _IsPackageContentBlobsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsPackageContentBlobsChanged || isInitialInsert)
            {
                var dataToStore = PackageContentBlobs.ToArray();
                PackageContentBlobsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "CategoryLink")]
	public class CategoryLink : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CategoryLink(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SourceCategoryID] TEXT NOT NULL, 
[TargetCategoryID] TEXT NOT NULL, 
[LinkingType] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SourceCategoryID { get; set; }
		// private string _unmodified_SourceCategoryID;

		[Column]
		public string TargetCategoryID { get; set; }
		// private string _unmodified_TargetCategoryID;

		[Column]
		public string LinkingType { get; set; }
		// private string _unmodified_LinkingType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Category")]
	public class Category : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Category(
[ID] TEXT NOT NULL PRIMARY KEY, 
[NativeCategoryID] TEXT NOT NULL, 
[NativeCategoryDomainName] TEXT NOT NULL, 
[NativeCategoryObjectName] TEXT NOT NULL, 
[NativeCategoryTitle] TEXT NOT NULL, 
[IdentifyingCategoryName] TEXT NOT NULL, 
[ParentCategoryID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string NativeCategoryID { get; set; }
		// private string _unmodified_NativeCategoryID;

		[Column]
		public string NativeCategoryDomainName { get; set; }
		// private string _unmodified_NativeCategoryDomainName;

		[Column]
		public string NativeCategoryObjectName { get; set; }
		// private string _unmodified_NativeCategoryObjectName;

		[Column]
		public string NativeCategoryTitle { get; set; }
		// private string _unmodified_NativeCategoryTitle;

		[Column]
		public string IdentifyingCategoryName { get; set; }
		// private string _unmodified_IdentifyingCategoryName;

		[Column]
		public string ParentCategoryID { get; set; }
		// private string _unmodified_ParentCategoryID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "StatusSummary")]
	public class StatusSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS StatusSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PendingOperations] TEXT NOT NULL, 
[ExecutingOperations] TEXT NOT NULL, 
[RecentCompletedOperations] TEXT NOT NULL, 
[ChangeItemTrackingList] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "PendingOperations")] public string PendingOperationsData;

        private bool _IsPendingOperationsRetrieved = false;
        private bool _IsPendingOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _PendingOperations = null;
        public ObservableCollection<OperationExecutionItem> PendingOperations
        {
            get
            {
                if (!_IsPendingOperationsRetrieved)
                {
                    if (PendingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(PendingOperationsData);
                        _PendingOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _PendingOperations = new ObservableCollection<OperationExecutionItem>();
						PendingOperationsData = Guid.NewGuid().ToString();
						_IsPendingOperationsChanged = true;
                    }
                    _IsPendingOperationsRetrieved = true;
                    _PendingOperations.CollectionChanged += (sender, args) =>
						{
							PendingOperationsData = Guid.NewGuid().ToString();
							_IsPendingOperationsChanged = true;
						};
                }
                return _PendingOperations;
            }
            set 
			{ 
				_PendingOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsPendingOperationsRetrieved = true;
                PendingOperationsData = Guid.NewGuid().ToString();
                _IsPendingOperationsChanged = true;

			}
        }

        [Column(Name = "ExecutingOperations")] public string ExecutingOperationsData;

        private bool _IsExecutingOperationsRetrieved = false;
        private bool _IsExecutingOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _ExecutingOperations = null;
        public ObservableCollection<OperationExecutionItem> ExecutingOperations
        {
            get
            {
                if (!_IsExecutingOperationsRetrieved)
                {
                    if (ExecutingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(ExecutingOperationsData);
                        _ExecutingOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _ExecutingOperations = new ObservableCollection<OperationExecutionItem>();
						ExecutingOperationsData = Guid.NewGuid().ToString();
						_IsExecutingOperationsChanged = true;
                    }
                    _IsExecutingOperationsRetrieved = true;
                    _ExecutingOperations.CollectionChanged += (sender, args) =>
						{
							ExecutingOperationsData = Guid.NewGuid().ToString();
							_IsExecutingOperationsChanged = true;
						};
                }
                return _ExecutingOperations;
            }
            set 
			{ 
				_ExecutingOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsExecutingOperationsRetrieved = true;
                ExecutingOperationsData = Guid.NewGuid().ToString();
                _IsExecutingOperationsChanged = true;

			}
        }

        [Column(Name = "RecentCompletedOperations")] public string RecentCompletedOperationsData;

        private bool _IsRecentCompletedOperationsRetrieved = false;
        private bool _IsRecentCompletedOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _RecentCompletedOperations = null;
        public ObservableCollection<OperationExecutionItem> RecentCompletedOperations
        {
            get
            {
                if (!_IsRecentCompletedOperationsRetrieved)
                {
                    if (RecentCompletedOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(RecentCompletedOperationsData);
                        _RecentCompletedOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _RecentCompletedOperations = new ObservableCollection<OperationExecutionItem>();
						RecentCompletedOperationsData = Guid.NewGuid().ToString();
						_IsRecentCompletedOperationsChanged = true;
                    }
                    _IsRecentCompletedOperationsRetrieved = true;
                    _RecentCompletedOperations.CollectionChanged += (sender, args) =>
						{
							RecentCompletedOperationsData = Guid.NewGuid().ToString();
							_IsRecentCompletedOperationsChanged = true;
						};
                }
                return _RecentCompletedOperations;
            }
            set 
			{ 
				_RecentCompletedOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsRecentCompletedOperationsRetrieved = true;
                RecentCompletedOperationsData = Guid.NewGuid().ToString();
                _IsRecentCompletedOperationsChanged = true;

			}
        }

        [Column(Name = "ChangeItemTrackingList")] public string ChangeItemTrackingListData;

        private bool _IsChangeItemTrackingListRetrieved = false;
        private bool _IsChangeItemTrackingListChanged = false;
        private ObservableCollection<string> _ChangeItemTrackingList = null;
        public ObservableCollection<string> ChangeItemTrackingList
        {
            get
            {
                if (!_IsChangeItemTrackingListRetrieved)
                {
                    if (ChangeItemTrackingListData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ChangeItemTrackingListData);
                        _ChangeItemTrackingList = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ChangeItemTrackingList = new ObservableCollection<string>();
						ChangeItemTrackingListData = Guid.NewGuid().ToString();
						_IsChangeItemTrackingListChanged = true;
                    }
                    _IsChangeItemTrackingListRetrieved = true;
                    _ChangeItemTrackingList.CollectionChanged += (sender, args) =>
						{
							ChangeItemTrackingListData = Guid.NewGuid().ToString();
							_IsChangeItemTrackingListChanged = true;
						};
                }
                return _ChangeItemTrackingList;
            }
            set 
			{ 
				_ChangeItemTrackingList = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsChangeItemTrackingListRetrieved = true;
                ChangeItemTrackingListData = Guid.NewGuid().ToString();
                _IsChangeItemTrackingListChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsPendingOperationsChanged || isInitialInsert)
            {
                var dataToStore = PendingOperations.ToArray();
                PendingOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsExecutingOperationsChanged || isInitialInsert)
            {
                var dataToStore = ExecutingOperations.ToArray();
                ExecutingOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsRecentCompletedOperationsChanged || isInitialInsert)
            {
                var dataToStore = RecentCompletedOperations.ToArray();
                RecentCompletedOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsChangeItemTrackingListChanged || isInitialInsert)
            {
                var dataToStore = ChangeItemTrackingList.ToArray();
                ChangeItemTrackingListData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "InformationChangeItem")]
	public class InformationChangeItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationChangeItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[StartTimeUTC] TEXT NOT NULL, 
[EndTimeUTC] TEXT NOT NULL, 
[ChangedObjectIDList] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		[Column]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
        [Column(Name = "ChangedObjectIDList")] public string ChangedObjectIDListData;

        private bool _IsChangedObjectIDListRetrieved = false;
        private bool _IsChangedObjectIDListChanged = false;
        private ObservableCollection<string> _ChangedObjectIDList = null;
        public ObservableCollection<string> ChangedObjectIDList
        {
            get
            {
                if (!_IsChangedObjectIDListRetrieved)
                {
                    if (ChangedObjectIDListData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ChangedObjectIDListData);
                        _ChangedObjectIDList = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ChangedObjectIDList = new ObservableCollection<string>();
						ChangedObjectIDListData = Guid.NewGuid().ToString();
						_IsChangedObjectIDListChanged = true;
                    }
                    _IsChangedObjectIDListRetrieved = true;
                    _ChangedObjectIDList.CollectionChanged += (sender, args) =>
						{
							ChangedObjectIDListData = Guid.NewGuid().ToString();
							_IsChangedObjectIDListChanged = true;
						};
                }
                return _ChangedObjectIDList;
            }
            set 
			{ 
				_ChangedObjectIDList = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsChangedObjectIDListRetrieved = true;
                ChangedObjectIDListData = Guid.NewGuid().ToString();
                _IsChangedObjectIDListChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsChangedObjectIDListChanged || isInitialInsert)
            {
                var dataToStore = ChangedObjectIDList.ToArray();
                ChangedObjectIDListData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "OperationExecutionItem")]
	public class OperationExecutionItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS OperationExecutionItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OperationName] TEXT NOT NULL, 
[OperationDomain] TEXT NOT NULL, 
[OperationID] TEXT NOT NULL, 
[CallerProvidedInfo] TEXT NOT NULL, 
[CreationTime] TEXT NOT NULL, 
[ExecutionBeginTime] TEXT NOT NULL, 
[ExecutionCompletedTime] TEXT NOT NULL, 
[ExecutionStatus] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OperationName { get; set; }
		// private string _unmodified_OperationName;

		[Column]
		public string OperationDomain { get; set; }
		// private string _unmodified_OperationDomain;

		[Column]
		public string OperationID { get; set; }
		// private string _unmodified_OperationID;

		[Column]
		public string CallerProvidedInfo { get; set; }
		// private string _unmodified_CallerProvidedInfo;

		[Column]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;

		[Column]
		public DateTime ExecutionBeginTime { get; set; }
		// private DateTime _unmodified_ExecutionBeginTime;

		[Column]
		public DateTime ExecutionCompletedTime { get; set; }
		// private DateTime _unmodified_ExecutionCompletedTime;

		[Column]
		public string ExecutionStatus { get; set; }
		// private string _unmodified_ExecutionStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GenericCollectionableObject")]
	public class GenericCollectionableObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericCollectionableObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ValueObject] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public GenericObject ValueObject { get; set; }
		// private GenericObject _unmodified_ValueObject;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GenericObject")]
	public class GenericObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Values] TEXT NOT NULL, 
[IncludeInCollection] INTEGER NOT NULL, 
[OptionalCollectionName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "Values")] public string ValuesData;

        private bool _IsValuesRetrieved = false;
        private bool _IsValuesChanged = false;
        private ObservableCollection<GenericValue> _Values = null;
        public ObservableCollection<GenericValue> Values
        {
            get
            {
                if (!_IsValuesRetrieved)
                {
                    if (ValuesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<GenericValue[]>(ValuesData);
                        _Values = new ObservableCollection<GenericValue>(arrayData);
                    }
                    else
                    {
                        _Values = new ObservableCollection<GenericValue>();
						ValuesData = Guid.NewGuid().ToString();
						_IsValuesChanged = true;
                    }
                    _IsValuesRetrieved = true;
                    _Values.CollectionChanged += (sender, args) =>
						{
							ValuesData = Guid.NewGuid().ToString();
							_IsValuesChanged = true;
						};
                }
                return _Values;
            }
            set 
			{ 
				_Values = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsValuesRetrieved = true;
                ValuesData = Guid.NewGuid().ToString();
                _IsValuesChanged = true;

			}
        }


		[Column]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		[Column]
		public string OptionalCollectionName { get; set; }
		// private string _unmodified_OptionalCollectionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsValuesChanged || isInitialInsert)
            {
                var dataToStore = Values.ToArray();
                ValuesData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "GenericValue")]
	public class GenericValue : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericValue(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ValueName] TEXT NOT NULL, 
[String] TEXT NOT NULL, 
[StringArray] TEXT NOT NULL, 
[Number] REAL NOT NULL, 
[NumberArray] TEXT NOT NULL, 
[Boolean] INTEGER NOT NULL, 
[BooleanArray] TEXT NOT NULL, 
[DateTime] TEXT NOT NULL, 
[DateTimeArray] TEXT NOT NULL, 
[Object] TEXT NOT NULL, 
[ObjectArray] TEXT NOT NULL, 
[IndexingInfo] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		[Column]
		public string String { get; set; }
		// private string _unmodified_String;
        [Column(Name = "StringArray")] public string StringArrayData;

        private bool _IsStringArrayRetrieved = false;
        private bool _IsStringArrayChanged = false;
        private ObservableCollection<string> _StringArray = null;
        public ObservableCollection<string> StringArray
        {
            get
            {
                if (!_IsStringArrayRetrieved)
                {
                    if (StringArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(StringArrayData);
                        _StringArray = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _StringArray = new ObservableCollection<string>();
						StringArrayData = Guid.NewGuid().ToString();
						_IsStringArrayChanged = true;
                    }
                    _IsStringArrayRetrieved = true;
                    _StringArray.CollectionChanged += (sender, args) =>
						{
							StringArrayData = Guid.NewGuid().ToString();
							_IsStringArrayChanged = true;
						};
                }
                return _StringArray;
            }
            set 
			{ 
				_StringArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsStringArrayRetrieved = true;
                StringArrayData = Guid.NewGuid().ToString();
                _IsStringArrayChanged = true;

			}
        }


		[Column]
		public double Number { get; set; }
		// private double _unmodified_Number;
        [Column(Name = "NumberArray")] public string NumberArrayData;

        private bool _IsNumberArrayRetrieved = false;
        private bool _IsNumberArrayChanged = false;
        private ObservableCollection<double> _NumberArray = null;
        public ObservableCollection<double> NumberArray
        {
            get
            {
                if (!_IsNumberArrayRetrieved)
                {
                    if (NumberArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<double[]>(NumberArrayData);
                        _NumberArray = new ObservableCollection<double>(arrayData);
                    }
                    else
                    {
                        _NumberArray = new ObservableCollection<double>();
						NumberArrayData = Guid.NewGuid().ToString();
						_IsNumberArrayChanged = true;
                    }
                    _IsNumberArrayRetrieved = true;
                    _NumberArray.CollectionChanged += (sender, args) =>
						{
							NumberArrayData = Guid.NewGuid().ToString();
							_IsNumberArrayChanged = true;
						};
                }
                return _NumberArray;
            }
            set 
			{ 
				_NumberArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsNumberArrayRetrieved = true;
                NumberArrayData = Guid.NewGuid().ToString();
                _IsNumberArrayChanged = true;

			}
        }


		[Column]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        [Column(Name = "BooleanArray")] public string BooleanArrayData;

        private bool _IsBooleanArrayRetrieved = false;
        private bool _IsBooleanArrayChanged = false;
        private ObservableCollection<bool> _BooleanArray = null;
        public ObservableCollection<bool> BooleanArray
        {
            get
            {
                if (!_IsBooleanArrayRetrieved)
                {
                    if (BooleanArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<bool[]>(BooleanArrayData);
                        _BooleanArray = new ObservableCollection<bool>(arrayData);
                    }
                    else
                    {
                        _BooleanArray = new ObservableCollection<bool>();
						BooleanArrayData = Guid.NewGuid().ToString();
						_IsBooleanArrayChanged = true;
                    }
                    _IsBooleanArrayRetrieved = true;
                    _BooleanArray.CollectionChanged += (sender, args) =>
						{
							BooleanArrayData = Guid.NewGuid().ToString();
							_IsBooleanArrayChanged = true;
						};
                }
                return _BooleanArray;
            }
            set 
			{ 
				_BooleanArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsBooleanArrayRetrieved = true;
                BooleanArrayData = Guid.NewGuid().ToString();
                _IsBooleanArrayChanged = true;

			}
        }


		[Column]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        [Column(Name = "DateTimeArray")] public string DateTimeArrayData;

        private bool _IsDateTimeArrayRetrieved = false;
        private bool _IsDateTimeArrayChanged = false;
        private ObservableCollection<DateTime> _DateTimeArray = null;
        public ObservableCollection<DateTime> DateTimeArray
        {
            get
            {
                if (!_IsDateTimeArrayRetrieved)
                {
                    if (DateTimeArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<DateTime[]>(DateTimeArrayData);
                        _DateTimeArray = new ObservableCollection<DateTime>(arrayData);
                    }
                    else
                    {
                        _DateTimeArray = new ObservableCollection<DateTime>();
						DateTimeArrayData = Guid.NewGuid().ToString();
						_IsDateTimeArrayChanged = true;
                    }
                    _IsDateTimeArrayRetrieved = true;
                    _DateTimeArray.CollectionChanged += (sender, args) =>
						{
							DateTimeArrayData = Guid.NewGuid().ToString();
							_IsDateTimeArrayChanged = true;
						};
                }
                return _DateTimeArray;
            }
            set 
			{ 
				_DateTimeArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsDateTimeArrayRetrieved = true;
                DateTimeArrayData = Guid.NewGuid().ToString();
                _IsDateTimeArrayChanged = true;

			}
        }


		[Column]
		public GenericObject Object { get; set; }
		// private GenericObject _unmodified_Object;
        [Column(Name = "ObjectArray")] public string ObjectArrayData;

        private bool _IsObjectArrayRetrieved = false;
        private bool _IsObjectArrayChanged = false;
        private ObservableCollection<GenericObject> _ObjectArray = null;
        public ObservableCollection<GenericObject> ObjectArray
        {
            get
            {
                if (!_IsObjectArrayRetrieved)
                {
                    if (ObjectArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<GenericObject[]>(ObjectArrayData);
                        _ObjectArray = new ObservableCollection<GenericObject>(arrayData);
                    }
                    else
                    {
                        _ObjectArray = new ObservableCollection<GenericObject>();
						ObjectArrayData = Guid.NewGuid().ToString();
						_IsObjectArrayChanged = true;
                    }
                    _IsObjectArrayRetrieved = true;
                    _ObjectArray.CollectionChanged += (sender, args) =>
						{
							ObjectArrayData = Guid.NewGuid().ToString();
							_IsObjectArrayChanged = true;
						};
                }
                return _ObjectArray;
            }
            set 
			{ 
				_ObjectArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsObjectArrayRetrieved = true;
                ObjectArrayData = Guid.NewGuid().ToString();
                _IsObjectArrayChanged = true;

			}
        }


		[Column]
		public string IndexingInfo { get; set; }
		// private string _unmodified_IndexingInfo;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsStringArrayChanged || isInitialInsert)
            {
                var dataToStore = StringArray.ToArray();
                StringArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsNumberArrayChanged || isInitialInsert)
            {
                var dataToStore = NumberArray.ToArray();
                NumberArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsBooleanArrayChanged || isInitialInsert)
            {
                var dataToStore = BooleanArray.ToArray();
                BooleanArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsDateTimeArrayChanged || isInitialInsert)
            {
                var dataToStore = DateTimeArray.ToArray();
                DateTimeArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsObjectArrayChanged || isInitialInsert)
            {
                var dataToStore = ObjectArray.ToArray();
                ObjectArrayData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
 } 
