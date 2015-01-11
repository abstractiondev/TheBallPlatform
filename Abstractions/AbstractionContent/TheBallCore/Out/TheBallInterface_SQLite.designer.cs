 


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

        [Column(Name = "ActiveTasks")] public byte[] ActiveTasksData;

		private bool _IsActiveTasksUsed = false;
        private List<WizardTask> _ActiveTasks = null;
        public List<WizardTask> ActiveTasks
        {
            get
            {
                if (_ActiveTasks == null && ActiveTasksData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    WizardTask[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ActiveTasksData))
                        objectArray = (WizardTask[]) bf.Deserialize(memStream);
                    _ActiveTasks = new List<WizardTask>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ActiveTasks.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ActiveTasksData = memStream.ToArray();
                    }
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
        [Column(Name = "ThisSideCategories")] public byte[] ThisSideCategoriesData;

		private bool _IsThisSideCategoriesUsed = false;
        private List<Category> _ThisSideCategories = null;
        public List<Category> ThisSideCategories
        {
            get
            {
                if (_ThisSideCategories == null && ThisSideCategoriesData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Category[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ThisSideCategoriesData))
                        objectArray = (Category[]) bf.Deserialize(memStream);
                    _ThisSideCategories = new List<Category>(objectArray);
					_IsThisSideCategoriesUsed = true;
                }
                return _ThisSideCategories;
            }
            set { _ThisSideCategories = value; }
        }

        [Column(Name = "OtherSideCategories")] public byte[] OtherSideCategoriesData;

		private bool _IsOtherSideCategoriesUsed = false;
        private List<Category> _OtherSideCategories = null;
        public List<Category> OtherSideCategories
        {
            get
            {
                if (_OtherSideCategories == null && OtherSideCategoriesData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Category[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(OtherSideCategoriesData))
                        objectArray = (Category[]) bf.Deserialize(memStream);
                    _OtherSideCategories = new List<Category>(objectArray);
					_IsOtherSideCategoriesUsed = true;
                }
                return _OtherSideCategories;
            }
            set { _OtherSideCategories = value; }
        }

        [Column(Name = "CategoryLinks")] public byte[] CategoryLinksData;

		private bool _IsCategoryLinksUsed = false;
        private List<CategoryLink> _CategoryLinks = null;
        public List<CategoryLink> CategoryLinks
        {
            get
            {
                if (_CategoryLinks == null && CategoryLinksData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    CategoryLink[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(CategoryLinksData))
                        objectArray = (CategoryLink[]) bf.Deserialize(memStream);
                    _CategoryLinks = new List<CategoryLink>(objectArray);
					_IsCategoryLinksUsed = true;
                }
                return _CategoryLinks;
            }
            set { _CategoryLinks = value; }
        }

        [Column(Name = "IncomingPackages")] public byte[] IncomingPackagesData;

		private bool _IsIncomingPackagesUsed = false;
        private List<TransferPackage> _IncomingPackages = null;
        public List<TransferPackage> IncomingPackages
        {
            get
            {
                if (_IncomingPackages == null && IncomingPackagesData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    TransferPackage[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(IncomingPackagesData))
                        objectArray = (TransferPackage[]) bf.Deserialize(memStream);
                    _IncomingPackages = new List<TransferPackage>(objectArray);
					_IsIncomingPackagesUsed = true;
                }
                return _IncomingPackages;
            }
            set { _IncomingPackages = value; }
        }

        [Column(Name = "OutgoingPackages")] public byte[] OutgoingPackagesData;

		private bool _IsOutgoingPackagesUsed = false;
        private List<TransferPackage> _OutgoingPackages = null;
        public List<TransferPackage> OutgoingPackages
        {
            get
            {
                if (_OutgoingPackages == null && OutgoingPackagesData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    TransferPackage[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(OutgoingPackagesData))
                        objectArray = (TransferPackage[]) bf.Deserialize(memStream);
                    _OutgoingPackages = new List<TransferPackage>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ThisSideCategories.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ThisSideCategoriesData = memStream.ToArray();
                    }
                }
            }

            if (_IsOtherSideCategoriesUsed)
            {
                if (_OtherSideCategories == null)
                    OtherSideCategoriesData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _OtherSideCategories.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        OtherSideCategoriesData = memStream.ToArray();
                    }
                }
            }

            if (_IsCategoryLinksUsed)
            {
                if (_CategoryLinks == null)
                    CategoryLinksData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _CategoryLinks.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        CategoryLinksData = memStream.ToArray();
                    }
                }
            }

            if (_IsIncomingPackagesUsed)
            {
                if (_IncomingPackages == null)
                    IncomingPackagesData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _IncomingPackages.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        IncomingPackagesData = memStream.ToArray();
                    }
                }
            }

            if (_IsOutgoingPackagesUsed)
            {
                if (_OutgoingPackages == null)
                    OutgoingPackagesData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _OutgoingPackages.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        OutgoingPackagesData = memStream.ToArray();
                    }
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
        [Column(Name = "PackageContentBlobs")] public byte[] PackageContentBlobsData;

		private bool _IsPackageContentBlobsUsed = false;
        private List<string> _PackageContentBlobs = null;
        public List<string> PackageContentBlobs
        {
            get
            {
                if (_PackageContentBlobs == null && PackageContentBlobsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(PackageContentBlobsData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _PackageContentBlobs = new List<string>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _PackageContentBlobs.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        PackageContentBlobsData = memStream.ToArray();
                    }
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

        [Column(Name = "PendingOperations")] public byte[] PendingOperationsData;

		private bool _IsPendingOperationsUsed = false;
        private List<OperationExecutionItem> _PendingOperations = null;
        public List<OperationExecutionItem> PendingOperations
        {
            get
            {
                if (_PendingOperations == null && PendingOperationsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    OperationExecutionItem[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(PendingOperationsData))
                        objectArray = (OperationExecutionItem[]) bf.Deserialize(memStream);
                    _PendingOperations = new List<OperationExecutionItem>(objectArray);
					_IsPendingOperationsUsed = true;
                }
                return _PendingOperations;
            }
            set { _PendingOperations = value; }
        }

        [Column(Name = "ExecutingOperations")] public byte[] ExecutingOperationsData;

		private bool _IsExecutingOperationsUsed = false;
        private List<OperationExecutionItem> _ExecutingOperations = null;
        public List<OperationExecutionItem> ExecutingOperations
        {
            get
            {
                if (_ExecutingOperations == null && ExecutingOperationsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    OperationExecutionItem[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ExecutingOperationsData))
                        objectArray = (OperationExecutionItem[]) bf.Deserialize(memStream);
                    _ExecutingOperations = new List<OperationExecutionItem>(objectArray);
					_IsExecutingOperationsUsed = true;
                }
                return _ExecutingOperations;
            }
            set { _ExecutingOperations = value; }
        }

        [Column(Name = "RecentCompletedOperations")] public byte[] RecentCompletedOperationsData;

		private bool _IsRecentCompletedOperationsUsed = false;
        private List<OperationExecutionItem> _RecentCompletedOperations = null;
        public List<OperationExecutionItem> RecentCompletedOperations
        {
            get
            {
                if (_RecentCompletedOperations == null && RecentCompletedOperationsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    OperationExecutionItem[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(RecentCompletedOperationsData))
                        objectArray = (OperationExecutionItem[]) bf.Deserialize(memStream);
                    _RecentCompletedOperations = new List<OperationExecutionItem>(objectArray);
					_IsRecentCompletedOperationsUsed = true;
                }
                return _RecentCompletedOperations;
            }
            set { _RecentCompletedOperations = value; }
        }

        [Column(Name = "ChangeItemTrackingList")] public byte[] ChangeItemTrackingListData;

		private bool _IsChangeItemTrackingListUsed = false;
        private List<string> _ChangeItemTrackingList = null;
        public List<string> ChangeItemTrackingList
        {
            get
            {
                if (_ChangeItemTrackingList == null && ChangeItemTrackingListData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ChangeItemTrackingListData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _ChangeItemTrackingList = new List<string>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _PendingOperations.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        PendingOperationsData = memStream.ToArray();
                    }
                }
            }

            if (_IsExecutingOperationsUsed)
            {
                if (_ExecutingOperations == null)
                    ExecutingOperationsData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ExecutingOperations.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ExecutingOperationsData = memStream.ToArray();
                    }
                }
            }

            if (_IsRecentCompletedOperationsUsed)
            {
                if (_RecentCompletedOperations == null)
                    RecentCompletedOperationsData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _RecentCompletedOperations.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        RecentCompletedOperationsData = memStream.ToArray();
                    }
                }
            }

            if (_IsChangeItemTrackingListUsed)
            {
                if (_ChangeItemTrackingList == null)
                    ChangeItemTrackingListData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ChangeItemTrackingList.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ChangeItemTrackingListData = memStream.ToArray();
                    }
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
        [Column(Name = "ChangedObjectIDList")] public byte[] ChangedObjectIDListData;

		private bool _IsChangedObjectIDListUsed = false;
        private List<string> _ChangedObjectIDList = null;
        public List<string> ChangedObjectIDList
        {
            get
            {
                if (_ChangedObjectIDList == null && ChangedObjectIDListData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ChangedObjectIDListData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _ChangedObjectIDList = new List<string>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ChangedObjectIDList.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ChangedObjectIDListData = memStream.ToArray();
                    }
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

        [Column(Name = "Values")] public byte[] ValuesData;

		private bool _IsValuesUsed = false;
        private List<GenericValue> _Values = null;
        public List<GenericValue> Values
        {
            get
            {
                if (_Values == null && ValuesData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    GenericValue[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ValuesData))
                        objectArray = (GenericValue[]) bf.Deserialize(memStream);
                    _Values = new List<GenericValue>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _Values.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ValuesData = memStream.ToArray();
                    }
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
        [Column(Name = "StringArray")] public byte[] StringArrayData;

		private bool _IsStringArrayUsed = false;
        private List<string> _StringArray = null;
        public List<string> StringArray
        {
            get
            {
                if (_StringArray == null && StringArrayData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(StringArrayData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _StringArray = new List<string>(objectArray);
					_IsStringArrayUsed = true;
                }
                return _StringArray;
            }
            set { _StringArray = value; }
        }


		[Column]
		public double Number { get; set; }
		// private double _unmodified_Number;
        [Column(Name = "NumberArray")] public byte[] NumberArrayData;

		private bool _IsNumberArrayUsed = false;
        private List<double> _NumberArray = null;
        public List<double> NumberArray
        {
            get
            {
                if (_NumberArray == null && NumberArrayData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    double[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(NumberArrayData))
                        objectArray = (double[]) bf.Deserialize(memStream);
                    _NumberArray = new List<double>(objectArray);
					_IsNumberArrayUsed = true;
                }
                return _NumberArray;
            }
            set { _NumberArray = value; }
        }


		[Column]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        [Column(Name = "BooleanArray")] public byte[] BooleanArrayData;

		private bool _IsBooleanArrayUsed = false;
        private List<bool> _BooleanArray = null;
        public List<bool> BooleanArray
        {
            get
            {
                if (_BooleanArray == null && BooleanArrayData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bool[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(BooleanArrayData))
                        objectArray = (bool[]) bf.Deserialize(memStream);
                    _BooleanArray = new List<bool>(objectArray);
					_IsBooleanArrayUsed = true;
                }
                return _BooleanArray;
            }
            set { _BooleanArray = value; }
        }


		[Column]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        [Column(Name = "DateTimeArray")] public byte[] DateTimeArrayData;

		private bool _IsDateTimeArrayUsed = false;
        private List<DateTime> _DateTimeArray = null;
        public List<DateTime> DateTimeArray
        {
            get
            {
                if (_DateTimeArray == null && DateTimeArrayData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    DateTime[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(DateTimeArrayData))
                        objectArray = (DateTime[]) bf.Deserialize(memStream);
                    _DateTimeArray = new List<DateTime>(objectArray);
					_IsDateTimeArrayUsed = true;
                }
                return _DateTimeArray;
            }
            set { _DateTimeArray = value; }
        }


		[Column]
		public GenericObject Object { get; set; }
		// private GenericObject _unmodified_Object;
        [Column(Name = "ObjectArray")] public byte[] ObjectArrayData;

		private bool _IsObjectArrayUsed = false;
        private List<GenericObject> _ObjectArray = null;
        public List<GenericObject> ObjectArray
        {
            get
            {
                if (_ObjectArray == null && ObjectArrayData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    GenericObject[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ObjectArrayData))
                        objectArray = (GenericObject[]) bf.Deserialize(memStream);
                    _ObjectArray = new List<GenericObject>(objectArray);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _StringArray.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        StringArrayData = memStream.ToArray();
                    }
                }
            }

            if (_IsNumberArrayUsed)
            {
                if (_NumberArray == null)
                    NumberArrayData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _NumberArray.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        NumberArrayData = memStream.ToArray();
                    }
                }
            }

            if (_IsBooleanArrayUsed)
            {
                if (_BooleanArray == null)
                    BooleanArrayData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _BooleanArray.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        BooleanArrayData = memStream.ToArray();
                    }
                }
            }

            if (_IsDateTimeArrayUsed)
            {
                if (_DateTimeArray == null)
                    DateTimeArrayData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _DateTimeArray.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        DateTimeArrayData = memStream.ToArray();
                    }
                }
            }

            if (_IsObjectArrayUsed)
            {
                if (_ObjectArray == null)
                    ObjectArrayData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ObjectArray.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ObjectArrayData = memStream.ToArray();
                    }
                }
            }

		}
	}
 } 
