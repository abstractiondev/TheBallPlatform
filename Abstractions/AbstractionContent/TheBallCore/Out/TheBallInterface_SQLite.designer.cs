 


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
using Newtonsoft.Json;

namespace SQLite.TheBall.Interface { 
		
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
		[Column]
		public string ID { get; set; }

        [Column(Name = "ActiveTasks")] public string ActiveTasksData;

		private bool _IsActiveTasksUsed = false;
        private List<WizardTask> _ActiveTasks = null;
        public List<WizardTask> ActiveTasks
        {
            get
            {
                if (_ActiveTasks == null && ActiveTasksData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<WizardTask[]>(ActiveTasksData);
                    _ActiveTasks = new List<WizardTask>(arrayData);
					_IsActiveTasksUsed = true;
                }
                return _ActiveTasks;
            }
            set { _ActiveTasks = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsActiveTasksUsed)
            {
                if (_ActiveTasks == null)
                    ActiveTasksData = null;
                else
                {
                    var dataToStore = _ActiveTasks.ToArray();
                    ActiveTasksData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "WizardTask")]
	public class WizardTask : ITheBallDataContextStorable
	{
		[Column]
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "Connection")]
	public class Connection : ITheBallDataContextStorable
	{
		[Column]
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

		private bool _IsThisSideCategoriesUsed = false;
        private List<Category> _ThisSideCategories = null;
        public List<Category> ThisSideCategories
        {
            get
            {
                if (_ThisSideCategories == null && ThisSideCategoriesData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<Category[]>(ThisSideCategoriesData);
                    _ThisSideCategories = new List<Category>(arrayData);
					_IsThisSideCategoriesUsed = true;
                }
                return _ThisSideCategories;
            }
            set { _ThisSideCategories = value; }
        }

        [Column(Name = "OtherSideCategories")] public string OtherSideCategoriesData;

		private bool _IsOtherSideCategoriesUsed = false;
        private List<Category> _OtherSideCategories = null;
        public List<Category> OtherSideCategories
        {
            get
            {
                if (_OtherSideCategories == null && OtherSideCategoriesData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<Category[]>(OtherSideCategoriesData);
                    _OtherSideCategories = new List<Category>(arrayData);
					_IsOtherSideCategoriesUsed = true;
                }
                return _OtherSideCategories;
            }
            set { _OtherSideCategories = value; }
        }

        [Column(Name = "CategoryLinks")] public string CategoryLinksData;

		private bool _IsCategoryLinksUsed = false;
        private List<CategoryLink> _CategoryLinks = null;
        public List<CategoryLink> CategoryLinks
        {
            get
            {
                if (_CategoryLinks == null && CategoryLinksData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<CategoryLink[]>(CategoryLinksData);
                    _CategoryLinks = new List<CategoryLink>(arrayData);
					_IsCategoryLinksUsed = true;
                }
                return _CategoryLinks;
            }
            set { _CategoryLinks = value; }
        }

        [Column(Name = "IncomingPackages")] public string IncomingPackagesData;

		private bool _IsIncomingPackagesUsed = false;
        private List<TransferPackage> _IncomingPackages = null;
        public List<TransferPackage> IncomingPackages
        {
            get
            {
                if (_IncomingPackages == null && IncomingPackagesData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(IncomingPackagesData);
                    _IncomingPackages = new List<TransferPackage>(arrayData);
					_IsIncomingPackagesUsed = true;
                }
                return _IncomingPackages;
            }
            set { _IncomingPackages = value; }
        }

        [Column(Name = "OutgoingPackages")] public string OutgoingPackagesData;

		private bool _IsOutgoingPackagesUsed = false;
        private List<TransferPackage> _OutgoingPackages = null;
        public List<TransferPackage> OutgoingPackages
        {
            get
            {
                if (_OutgoingPackages == null && OutgoingPackagesData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(OutgoingPackagesData);
                    _OutgoingPackages = new List<TransferPackage>(arrayData);
					_IsOutgoingPackagesUsed = true;
                }
                return _OutgoingPackages;
            }
            set { _OutgoingPackages = value; }
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
        public void PrepareForStoring()
        {
		
            if (_IsThisSideCategoriesUsed)
            {
                if (_ThisSideCategories == null)
                    ThisSideCategoriesData = null;
                else
                {
                    var dataToStore = _ThisSideCategories.ToArray();
                    ThisSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsOtherSideCategoriesUsed)
            {
                if (_OtherSideCategories == null)
                    OtherSideCategoriesData = null;
                else
                {
                    var dataToStore = _OtherSideCategories.ToArray();
                    OtherSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsCategoryLinksUsed)
            {
                if (_CategoryLinks == null)
                    CategoryLinksData = null;
                else
                {
                    var dataToStore = _CategoryLinks.ToArray();
                    CategoryLinksData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsIncomingPackagesUsed)
            {
                if (_IncomingPackages == null)
                    IncomingPackagesData = null;
                else
                {
                    var dataToStore = _IncomingPackages.ToArray();
                    IncomingPackagesData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsOutgoingPackagesUsed)
            {
                if (_OutgoingPackages == null)
                    OutgoingPackagesData = null;
                else
                {
                    var dataToStore = _OutgoingPackages.ToArray();
                    OutgoingPackagesData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "TransferPackage")]
	public class TransferPackage : ITheBallDataContextStorable
	{
		[Column]
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

		private bool _IsPackageContentBlobsUsed = false;
        private List<string> _PackageContentBlobs = null;
        public List<string> PackageContentBlobs
        {
            get
            {
                if (_PackageContentBlobs == null && PackageContentBlobsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<string[]>(PackageContentBlobsData);
                    _PackageContentBlobs = new List<string>(arrayData);
					_IsPackageContentBlobsUsed = true;
                }
                return _PackageContentBlobs;
            }
            set { _PackageContentBlobs = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsPackageContentBlobsUsed)
            {
                if (_PackageContentBlobs == null)
                    PackageContentBlobsData = null;
                else
                {
                    var dataToStore = _PackageContentBlobs.ToArray();
                    PackageContentBlobsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "CategoryLink")]
	public class CategoryLink : ITheBallDataContextStorable
	{
		[Column]
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "Category")]
	public class Category : ITheBallDataContextStorable
	{
		[Column]
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "StatusSummary")]
	public class StatusSummary : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }

        [Column(Name = "PendingOperations")] public string PendingOperationsData;

		private bool _IsPendingOperationsUsed = false;
        private List<OperationExecutionItem> _PendingOperations = null;
        public List<OperationExecutionItem> PendingOperations
        {
            get
            {
                if (_PendingOperations == null && PendingOperationsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(PendingOperationsData);
                    _PendingOperations = new List<OperationExecutionItem>(arrayData);
					_IsPendingOperationsUsed = true;
                }
                return _PendingOperations;
            }
            set { _PendingOperations = value; }
        }

        [Column(Name = "ExecutingOperations")] public string ExecutingOperationsData;

		private bool _IsExecutingOperationsUsed = false;
        private List<OperationExecutionItem> _ExecutingOperations = null;
        public List<OperationExecutionItem> ExecutingOperations
        {
            get
            {
                if (_ExecutingOperations == null && ExecutingOperationsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(ExecutingOperationsData);
                    _ExecutingOperations = new List<OperationExecutionItem>(arrayData);
					_IsExecutingOperationsUsed = true;
                }
                return _ExecutingOperations;
            }
            set { _ExecutingOperations = value; }
        }

        [Column(Name = "RecentCompletedOperations")] public string RecentCompletedOperationsData;

		private bool _IsRecentCompletedOperationsUsed = false;
        private List<OperationExecutionItem> _RecentCompletedOperations = null;
        public List<OperationExecutionItem> RecentCompletedOperations
        {
            get
            {
                if (_RecentCompletedOperations == null && RecentCompletedOperationsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(RecentCompletedOperationsData);
                    _RecentCompletedOperations = new List<OperationExecutionItem>(arrayData);
					_IsRecentCompletedOperationsUsed = true;
                }
                return _RecentCompletedOperations;
            }
            set { _RecentCompletedOperations = value; }
        }

        [Column(Name = "ChangeItemTrackingList")] public string ChangeItemTrackingListData;

		private bool _IsChangeItemTrackingListUsed = false;
        private List<string> _ChangeItemTrackingList = null;
        public List<string> ChangeItemTrackingList
        {
            get
            {
                if (_ChangeItemTrackingList == null && ChangeItemTrackingListData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<string[]>(ChangeItemTrackingListData);
                    _ChangeItemTrackingList = new List<string>(arrayData);
					_IsChangeItemTrackingListUsed = true;
                }
                return _ChangeItemTrackingList;
            }
            set { _ChangeItemTrackingList = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsPendingOperationsUsed)
            {
                if (_PendingOperations == null)
                    PendingOperationsData = null;
                else
                {
                    var dataToStore = _PendingOperations.ToArray();
                    PendingOperationsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsExecutingOperationsUsed)
            {
                if (_ExecutingOperations == null)
                    ExecutingOperationsData = null;
                else
                {
                    var dataToStore = _ExecutingOperations.ToArray();
                    ExecutingOperationsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsRecentCompletedOperationsUsed)
            {
                if (_RecentCompletedOperations == null)
                    RecentCompletedOperationsData = null;
                else
                {
                    var dataToStore = _RecentCompletedOperations.ToArray();
                    RecentCompletedOperationsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsChangeItemTrackingListUsed)
            {
                if (_ChangeItemTrackingList == null)
                    ChangeItemTrackingListData = null;
                else
                {
                    var dataToStore = _ChangeItemTrackingList.ToArray();
                    ChangeItemTrackingListData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "InformationChangeItem")]
	public class InformationChangeItem : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		[Column]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
        [Column(Name = "ChangedObjectIDList")] public string ChangedObjectIDListData;

		private bool _IsChangedObjectIDListUsed = false;
        private List<string> _ChangedObjectIDList = null;
        public List<string> ChangedObjectIDList
        {
            get
            {
                if (_ChangedObjectIDList == null && ChangedObjectIDListData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<string[]>(ChangedObjectIDListData);
                    _ChangedObjectIDList = new List<string>(arrayData);
					_IsChangedObjectIDListUsed = true;
                }
                return _ChangedObjectIDList;
            }
            set { _ChangedObjectIDList = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsChangedObjectIDListUsed)
            {
                if (_ChangedObjectIDList == null)
                    ChangedObjectIDListData = null;
                else
                {
                    var dataToStore = _ChangedObjectIDList.ToArray();
                    ChangedObjectIDListData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "OperationExecutionItem")]
	public class OperationExecutionItem : ITheBallDataContextStorable
	{
		[Column]
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "GenericCollectionableObject")]
	public class GenericCollectionableObject : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public GenericObject ValueObject { get; set; }
		// private GenericObject _unmodified_ValueObject;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "GenericObject")]
	public class GenericObject : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }

        [Column(Name = "Values")] public string ValuesData;

		private bool _IsValuesUsed = false;
        private List<GenericValue> _Values = null;
        public List<GenericValue> Values
        {
            get
            {
                if (_Values == null && ValuesData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<GenericValue[]>(ValuesData);
                    _Values = new List<GenericValue>(arrayData);
					_IsValuesUsed = true;
                }
                return _Values;
            }
            set { _Values = value; }
        }


		[Column]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		[Column]
		public string OptionalCollectionName { get; set; }
		// private string _unmodified_OptionalCollectionName;
        public void PrepareForStoring()
        {
		
            if (_IsValuesUsed)
            {
                if (_Values == null)
                    ValuesData = null;
                else
                {
                    var dataToStore = _Values.ToArray();
                    ValuesData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "GenericValue")]
	public class GenericValue : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		[Column]
		public string String { get; set; }
		// private string _unmodified_String;
        [Column(Name = "StringArray")] public string StringArrayData;

		private bool _IsStringArrayUsed = false;
        private List<string> _StringArray = null;
        public List<string> StringArray
        {
            get
            {
                if (_StringArray == null && StringArrayData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<string[]>(StringArrayData);
                    _StringArray = new List<string>(arrayData);
					_IsStringArrayUsed = true;
                }
                return _StringArray;
            }
            set { _StringArray = value; }
        }


		[Column]
		public double Number { get; set; }
		// private double _unmodified_Number;
        [Column(Name = "NumberArray")] public string NumberArrayData;

		private bool _IsNumberArrayUsed = false;
        private List<double> _NumberArray = null;
        public List<double> NumberArray
        {
            get
            {
                if (_NumberArray == null && NumberArrayData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<double[]>(NumberArrayData);
                    _NumberArray = new List<double>(arrayData);
					_IsNumberArrayUsed = true;
                }
                return _NumberArray;
            }
            set { _NumberArray = value; }
        }


		[Column]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        [Column(Name = "BooleanArray")] public string BooleanArrayData;

		private bool _IsBooleanArrayUsed = false;
        private List<bool> _BooleanArray = null;
        public List<bool> BooleanArray
        {
            get
            {
                if (_BooleanArray == null && BooleanArrayData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<bool[]>(BooleanArrayData);
                    _BooleanArray = new List<bool>(arrayData);
					_IsBooleanArrayUsed = true;
                }
                return _BooleanArray;
            }
            set { _BooleanArray = value; }
        }


		[Column]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        [Column(Name = "DateTimeArray")] public string DateTimeArrayData;

		private bool _IsDateTimeArrayUsed = false;
        private List<DateTime> _DateTimeArray = null;
        public List<DateTime> DateTimeArray
        {
            get
            {
                if (_DateTimeArray == null && DateTimeArrayData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<DateTime[]>(DateTimeArrayData);
                    _DateTimeArray = new List<DateTime>(arrayData);
					_IsDateTimeArrayUsed = true;
                }
                return _DateTimeArray;
            }
            set { _DateTimeArray = value; }
        }


		[Column]
		public GenericObject Object { get; set; }
		// private GenericObject _unmodified_Object;
        [Column(Name = "ObjectArray")] public string ObjectArrayData;

		private bool _IsObjectArrayUsed = false;
        private List<GenericObject> _ObjectArray = null;
        public List<GenericObject> ObjectArray
        {
            get
            {
                if (_ObjectArray == null && ObjectArrayData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<GenericObject[]>(ObjectArrayData);
                    _ObjectArray = new List<GenericObject>(arrayData);
					_IsObjectArrayUsed = true;
                }
                return _ObjectArray;
            }
            set { _ObjectArray = value; }
        }


		[Column]
		public string IndexingInfo { get; set; }
		// private string _unmodified_IndexingInfo;
        public void PrepareForStoring()
        {
		
            if (_IsStringArrayUsed)
            {
                if (_StringArray == null)
                    StringArrayData = null;
                else
                {
                    var dataToStore = _StringArray.ToArray();
                    StringArrayData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsNumberArrayUsed)
            {
                if (_NumberArray == null)
                    NumberArrayData = null;
                else
                {
                    var dataToStore = _NumberArray.ToArray();
                    NumberArrayData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsBooleanArrayUsed)
            {
                if (_BooleanArray == null)
                    BooleanArrayData = null;
                else
                {
                    var dataToStore = _BooleanArray.ToArray();
                    BooleanArrayData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsDateTimeArrayUsed)
            {
                if (_DateTimeArray == null)
                    DateTimeArrayData = null;
                else
                {
                    var dataToStore = _DateTimeArray.ToArray();
                    DateTimeArrayData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsObjectArrayUsed)
            {
                if (_ObjectArray == null)
                    ObjectArrayData = null;
                else
                {
                    var dataToStore = _ObjectArray.ToArray();
                    ObjectArrayData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
 } 
