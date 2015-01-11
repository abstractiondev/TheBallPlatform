 


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

namespace SQLite.TheBall.Interface { 
		
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
	public class WizardContainer
	{
		[Column]
		public string ID { get; set; }

		public List< WizardTask > ActiveTasks = new List< WizardTask >();
	}
    [Table(Name = "WizardTask")]
	public class WizardTask
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
	}
    [Table(Name = "Connection")]
	public class Connection
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
		public List< Category > ThisSideCategories = new List< Category >();
		public List< Category > OtherSideCategories = new List< Category >();
		public List< CategoryLink > CategoryLinks = new List< CategoryLink >();
		public List< TransferPackage > IncomingPackages = new List< TransferPackage >();
		public List< TransferPackage > OutgoingPackages = new List< TransferPackage >();

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
	}
    [Table(Name = "TransferPackage")]
	public class TransferPackage
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
		public List< string > PackageContentBlobs = new List< string >();
	}
    [Table(Name = "CategoryLink")]
	public class CategoryLink
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
	}
    [Table(Name = "Category")]
	public class Category
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
	}
    [Table(Name = "StatusSummary")]
	public class StatusSummary
	{
		[Column]
		public string ID { get; set; }

		public List< OperationExecutionItem > PendingOperations = new List< OperationExecutionItem >();
		public List< OperationExecutionItem > ExecutingOperations = new List< OperationExecutionItem >();
		public List< OperationExecutionItem > RecentCompletedOperations = new List< OperationExecutionItem >();
		public List< string > ChangeItemTrackingList = new List< string >();
	}
    [Table(Name = "InformationChangeItem")]
	public class InformationChangeItem
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		[Column]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
		public List< string > ChangedObjectIDList = new List< string >();
	}
    [Table(Name = "OperationExecutionItem")]
	public class OperationExecutionItem
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
	}
    [Table(Name = "GenericCollectionableObject")]
	public class GenericCollectionableObject
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public GenericObject ValueObject { get; set; }
		// private GenericObject _unmodified_ValueObject;
	}
    [Table(Name = "GenericObject")]
	public class GenericObject
	{
		[Column]
		public string ID { get; set; }

		public List< GenericValue > Values = new List< GenericValue >();

		[Column]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		[Column]
		public string OptionalCollectionName { get; set; }
		// private string _unmodified_OptionalCollectionName;
	}
    [Table(Name = "GenericValue")]
	public class GenericValue
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		[Column]
		public string String { get; set; }
		// private string _unmodified_String;
		public List< string > StringArray = new List< string >();

		[Column]
		public double Number { get; set; }
		// private double _unmodified_Number;
		public List< double > NumberArray = new List< double >();

		[Column]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
		public List< bool > BooleanArray = new List< bool >();

		[Column]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
		public List< DateTime > DateTimeArray = new List< DateTime >();

		[Column]
		public GenericObject Object { get; set; }
		// private GenericObject _unmodified_Object;
		public List< GenericObject > ObjectArray = new List< GenericObject >();

		[Column]
		public string IndexingInfo { get; set; }
		// private string _unmodified_IndexingInfo;
	}
 } 
