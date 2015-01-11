 


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

namespace SQLite.TheBall.CORE { 
		
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

			public Table<ContentPackage> ContentPackageTable {
				get {
					return this.GetTable<ContentPackage>();
				}
			}
			public Table<InformationInput> InformationInputTable {
				get {
					return this.GetTable<InformationInput>();
				}
			}
			public Table<InformationOutput> InformationOutputTable {
				get {
					return this.GetTable<InformationOutput>();
				}
			}
			public Table<AuthenticatedAsActiveDevice> AuthenticatedAsActiveDeviceTable {
				get {
					return this.GetTable<AuthenticatedAsActiveDevice>();
				}
			}
			public Table<DeviceMembership> DeviceMembershipTable {
				get {
					return this.GetTable<DeviceMembership>();
				}
			}
			public Table<InvoiceFiscalExportSummary> InvoiceFiscalExportSummaryTable {
				get {
					return this.GetTable<InvoiceFiscalExportSummary>();
				}
			}
			public Table<InvoiceSummaryContainer> InvoiceSummaryContainerTable {
				get {
					return this.GetTable<InvoiceSummaryContainer>();
				}
			}
			public Table<Invoice> InvoiceTable {
				get {
					return this.GetTable<Invoice>();
				}
			}
			public Table<InvoiceDetails> InvoiceDetailsTable {
				get {
					return this.GetTable<InvoiceDetails>();
				}
			}
			public Table<InvoiceUser> InvoiceUserTable {
				get {
					return this.GetTable<InvoiceUser>();
				}
			}
			public Table<InvoiceRowGroup> InvoiceRowGroupTable {
				get {
					return this.GetTable<InvoiceRowGroup>();
				}
			}
			public Table<InvoiceEventDetailGroup> InvoiceEventDetailGroupTable {
				get {
					return this.GetTable<InvoiceEventDetailGroup>();
				}
			}
			public Table<InvoiceEventDetail> InvoiceEventDetailTable {
				get {
					return this.GetTable<InvoiceEventDetail>();
				}
			}
			public Table<InvoiceRow> InvoiceRowTable {
				get {
					return this.GetTable<InvoiceRow>();
				}
			}
			public Table<Category> CategoryTable {
				get {
					return this.GetTable<Category>();
				}
			}
			public Table<ProcessContainer> ProcessContainerTable {
				get {
					return this.GetTable<ProcessContainer>();
				}
			}
			public Table<Process> ProcessTable {
				get {
					return this.GetTable<Process>();
				}
			}
			public Table<ProcessItem> ProcessItemTable {
				get {
					return this.GetTable<ProcessItem>();
				}
			}
			public Table<SemanticInformationItem> SemanticInformationItemTable {
				get {
					return this.GetTable<SemanticInformationItem>();
				}
			}
			public Table<InformationOwnerInfo> InformationOwnerInfoTable {
				get {
					return this.GetTable<InformationOwnerInfo>();
				}
			}
			public Table<UsageSummary> UsageSummaryTable {
				get {
					return this.GetTable<UsageSummary>();
				}
			}
			public Table<UsageMonitorItem> UsageMonitorItemTable {
				get {
					return this.GetTable<UsageMonitorItem>();
				}
			}
			public Table<RequestResourceUsage> RequestResourceUsageTable {
				get {
					return this.GetTable<RequestResourceUsage>();
				}
			}
			public Table<ProcessorUsage> ProcessorUsageTable {
				get {
					return this.GetTable<ProcessorUsage>();
				}
			}
			public Table<StorageTransactionUsage> StorageTransactionUsageTable {
				get {
					return this.GetTable<StorageTransactionUsage>();
				}
			}
			public Table<StorageUsage> StorageUsageTable {
				get {
					return this.GetTable<StorageUsage>();
				}
			}
			public Table<NetworkUsage> NetworkUsageTable {
				get {
					return this.GetTable<NetworkUsage>();
				}
			}
			public Table<TimeRange> TimeRangeTable {
				get {
					return this.GetTable<TimeRange>();
				}
			}
			public Table<HTTPActivityDetails> HTTPActivityDetailsTable {
				get {
					return this.GetTable<HTTPActivityDetails>();
				}
			}
        }

    [Table(Name = "ContentPackage")]
	public class ContentPackage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
		public string PackageName { get; set; }
		// private string _unmodified_PackageName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string PackageRootFolder { get; set; }
		// private string _unmodified_PackageRootFolder;

		[Column]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InformationInput")]
	public class InformationInput : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InputDescription { get; set; }
		// private string _unmodified_InputDescription;

		[Column]
		public string LocationURL { get; set; }
		// private string _unmodified_LocationURL;

		[Column]
		public string LocalContentName { get; set; }
		// private string _unmodified_LocalContentName;

		[Column]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InformationOutput")]
	public class InformationOutput : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OutputDescription { get; set; }
		// private string _unmodified_OutputDescription;

		[Column]
		public string DestinationURL { get; set; }
		// private string _unmodified_DestinationURL;

		[Column]
		public string DestinationContentName { get; set; }
		// private string _unmodified_DestinationContentName;

		[Column]
		public string LocalContentURL { get; set; }
		// private string _unmodified_LocalContentURL;

		[Column]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "AuthenticatedAsActiveDevice")]
	public class AuthenticatedAsActiveDevice : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string AuthenticationDescription { get; set; }
		// private string _unmodified_AuthenticationDescription;

		[Column]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
		public string EstablishedTrustID { get; set; }
		// private string _unmodified_EstablishedTrustID;

		[Column]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;

		[Column]
		public string NegotiationURL { get; set; }
		// private string _unmodified_NegotiationURL;

		[Column]
		public string ConnectionURL { get; set; }
		// private string _unmodified_ConnectionURL;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "DeviceMembership")]
	public class DeviceMembership : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string DeviceDescription { get; set; }
		// private string _unmodified_DeviceDescription;

		[Column]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceFiscalExportSummary")]
	public class InvoiceFiscalExportSummary : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime FiscalInclusiveStartDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveStartDate;

		[Column]
		public DateTime FiscalInclusiveEndDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveEndDate;

		[Column]
		public InvoiceCollection ExportedInvoices { get; set; }
		// private InvoiceCollection _unmodified_ExportedInvoices;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceSummaryContainer")]
	public class InvoiceSummaryContainer : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InvoiceCollection OpenInvoices { get; set; }
		// private InvoiceCollection _unmodified_OpenInvoices;

		[Column]
		public InvoiceCollection PredictedInvoices { get; set; }
		// private InvoiceCollection _unmodified_PredictedInvoices;

		[Column]
		public InvoiceCollection PaidInvoicesActiveYear { get; set; }
		// private InvoiceCollection _unmodified_PaidInvoicesActiveYear;

		[Column]
		public InvoiceCollection PaidInvoicesLast12Months { get; set; }
		// private InvoiceCollection _unmodified_PaidInvoicesLast12Months;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "Invoice")]
	public class Invoice : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InvoiceName { get; set; }
		// private string _unmodified_InvoiceName;

		[Column]
		public string InvoiceID { get; set; }
		// private string _unmodified_InvoiceID;

		[Column]
		public string InvoicedAmount { get; set; }
		// private string _unmodified_InvoicedAmount;

		[Column]
		public DateTime CreateDate { get; set; }
		// private DateTime _unmodified_CreateDate;

		[Column]
		public DateTime DueDate { get; set; }
		// private DateTime _unmodified_DueDate;

		[Column]
		public string PaidAmount { get; set; }
		// private string _unmodified_PaidAmount;

		[Column]
		public string FeesAndInterestAmount { get; set; }
		// private string _unmodified_FeesAndInterestAmount;

		[Column]
		public string UnpaidAmount { get; set; }
		// private string _unmodified_UnpaidAmount;

		[Column]
		public InvoiceDetails InvoiceDetails { get; set; }
		// private InvoiceDetails _unmodified_InvoiceDetails;

		[Column]
		public InvoiceUserCollection InvoiceUsers { get; set; }
		// private InvoiceUserCollection _unmodified_InvoiceUsers;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceDetails")]
	public class InvoiceDetails : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string MonthlyFeesTotal { get; set; }
		// private string _unmodified_MonthlyFeesTotal;

		[Column]
		public string OneTimeFeesTotal { get; set; }
		// private string _unmodified_OneTimeFeesTotal;

		[Column]
		public string UsageFeesTotal { get; set; }
		// private string _unmodified_UsageFeesTotal;

		[Column]
		public string InterestFeesTotal { get; set; }
		// private string _unmodified_InterestFeesTotal;

		[Column]
		public string PenaltyFeesTotal { get; set; }
		// private string _unmodified_PenaltyFeesTotal;

		[Column]
		public string TotalFeesTotal { get; set; }
		// private string _unmodified_TotalFeesTotal;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceUser")]
	public class InvoiceUser : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string UserName { get; set; }
		// private string _unmodified_UserName;

		[Column]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
		public string UserPhoneNumber { get; set; }
		// private string _unmodified_UserPhoneNumber;

		[Column]
		public string UserSubscriptionNumber { get; set; }
		// private string _unmodified_UserSubscriptionNumber;

		[Column]
		public string UserInvoiceTotalAmount { get; set; }
		// private string _unmodified_UserInvoiceTotalAmount;

		[Column]
		public InvoiceRowGroupCollection InvoiceRowGroupCollection { get; set; }
		// private InvoiceRowGroupCollection _unmodified_InvoiceRowGroupCollection;

		[Column]
		public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection { get; set; }
		// private InvoiceEventDetailGroupCollection _unmodified_InvoiceEventDetailGroupCollection;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceRowGroup")]
	public class InvoiceRowGroup : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public string GroupTotalPriceWithoutTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithoutTaxes;

		[Column]
		public string GroupTotalTaxes { get; set; }
		// private string _unmodified_GroupTotalTaxes;

		[Column]
		public string GroupTotalPriceWithTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithTaxes;

		[Column]
		public InvoiceRowCollection InvoiceRowCollection { get; set; }
		// private InvoiceRowCollection _unmodified_InvoiceRowCollection;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceEventDetailGroup")]
	public class InvoiceEventDetailGroup : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
		// private InvoiceEventDetailCollection _unmodified_InvoiceEventDetailCollection;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceEventDetail")]
	public class InvoiceEventDetail : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
		public DateTime EventStartDateTime { get; set; }
		// private DateTime _unmodified_EventStartDateTime;

		[Column]
		public DateTime EventEndDateTime { get; set; }
		// private DateTime _unmodified_EventEndDateTime;

		[Column]
		public string ReceivingParty { get; set; }
		// private string _unmodified_ReceivingParty;

		[Column]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
		public string PriceWithTaxes { get; set; }
		// private string _unmodified_PriceWithTaxes;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InvoiceRow")]
	public class InvoiceRow : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
		public string PriceWithTaxes { get; set; }
		// private string _unmodified_PriceWithTaxes;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "Category")]
	public class Category : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "ProcessContainer")]
	public class ProcessContainer : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "ProcessIDs")] public string ProcessIDsData;

		private bool _IsProcessIDsUsed = false;
        private List<string> _ProcessIDs = null;
        public List<string> ProcessIDs
        {
            get
            {
                if (_ProcessIDs == null && ProcessIDsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<string[]>(ProcessIDsData);
                    _ProcessIDs = new List<string>(arrayData);
					_IsProcessIDsUsed = true;
                }
                return _ProcessIDs;
            }
            set { _ProcessIDs = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsProcessIDsUsed)
            {
                if (_ProcessIDs == null)
                    ProcessIDsData = null;
                else
                {
                    var dataToStore = _ProcessIDs.ToArray();
                    ProcessIDsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "Process")]
	public class Process : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ProcessDescription { get; set; }
		// private string _unmodified_ProcessDescription;

		[Column]
		public SemanticInformationItem ExecutingOperation { get; set; }
		// private SemanticInformationItem _unmodified_ExecutingOperation;
        [Column(Name = "InitialArguments")] public string InitialArgumentsData;

		private bool _IsInitialArgumentsUsed = false;
        private List<SemanticInformationItem> _InitialArguments = null;
        public List<SemanticInformationItem> InitialArguments
        {
            get
            {
                if (_InitialArguments == null && InitialArgumentsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(InitialArgumentsData);
                    _InitialArguments = new List<SemanticInformationItem>(arrayData);
					_IsInitialArgumentsUsed = true;
                }
                return _InitialArguments;
            }
            set { _InitialArguments = value; }
        }

        [Column(Name = "ProcessItems")] public string ProcessItemsData;

		private bool _IsProcessItemsUsed = false;
        private List<ProcessItem> _ProcessItems = null;
        public List<ProcessItem> ProcessItems
        {
            get
            {
                if (_ProcessItems == null && ProcessItemsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<ProcessItem[]>(ProcessItemsData);
                    _ProcessItems = new List<ProcessItem>(arrayData);
					_IsProcessItemsUsed = true;
                }
                return _ProcessItems;
            }
            set { _ProcessItems = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsInitialArgumentsUsed)
            {
                if (_InitialArguments == null)
                    InitialArgumentsData = null;
                else
                {
                    var dataToStore = _InitialArguments.ToArray();
                    InitialArgumentsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsProcessItemsUsed)
            {
                if (_ProcessItems == null)
                    ProcessItemsData = null;
                else
                {
                    var dataToStore = _ProcessItems.ToArray();
                    ProcessItemsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "ProcessItem")]
	public class ProcessItem : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "Outputs")] public string OutputsData;

		private bool _IsOutputsUsed = false;
        private List<SemanticInformationItem> _Outputs = null;
        public List<SemanticInformationItem> Outputs
        {
            get
            {
                if (_Outputs == null && OutputsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(OutputsData);
                    _Outputs = new List<SemanticInformationItem>(arrayData);
					_IsOutputsUsed = true;
                }
                return _Outputs;
            }
            set { _Outputs = value; }
        }

        [Column(Name = "Inputs")] public string InputsData;

		private bool _IsInputsUsed = false;
        private List<SemanticInformationItem> _Inputs = null;
        public List<SemanticInformationItem> Inputs
        {
            get
            {
                if (_Inputs == null && InputsData != null)
                {
                    var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(InputsData);
                    _Inputs = new List<SemanticInformationItem>(arrayData);
					_IsInputsUsed = true;
                }
                return _Inputs;
            }
            set { _Inputs = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsOutputsUsed)
            {
                if (_Outputs == null)
                    OutputsData = null;
                else
                {
                    var dataToStore = _Outputs.ToArray();
                    OutputsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

            if (_IsInputsUsed)
            {
                if (_Inputs == null)
                    InputsData = null;
                else
                {
                    var dataToStore = _Inputs.ToArray();
                    InputsData = JsonConvert.SerializeObject(dataToStore);
                }
            }

		}
	}
    [Table(Name = "SemanticInformationItem")]
	public class SemanticInformationItem : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ItemFullType { get; set; }
		// private string _unmodified_ItemFullType;

		[Column]
		public string ItemValue { get; set; }
		// private string _unmodified_ItemValue;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "InformationOwnerInfo")]
	public class InformationOwnerInfo : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OwnerType { get; set; }
		// private string _unmodified_OwnerType;

		[Column]
		public string OwnerIdentifier { get; set; }
		// private string _unmodified_OwnerIdentifier;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "UsageSummary")]
	public class UsageSummary : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SummaryName { get; set; }
		// private string _unmodified_SummaryName;

		[Column]
		public UsageMonitorItem SummaryMonitoringItem { get; set; }
		// private UsageMonitorItem _unmodified_SummaryMonitoringItem;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "UsageMonitorItem")]
	public class UsageMonitorItem : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InformationOwnerInfo OwnerInfo { get; set; }
		// private InformationOwnerInfo _unmodified_OwnerInfo;

		[Column]
		public TimeRange TimeRangeInclusiveStartExclusiveEnd { get; set; }
		// private TimeRange _unmodified_TimeRangeInclusiveStartExclusiveEnd;

		[Column]
		public long StepSizeInMinutes { get; set; }
		// private long _unmodified_StepSizeInMinutes;

		[Column]
		public ProcessorUsageCollection ProcessorUsages { get; set; }
		// private ProcessorUsageCollection _unmodified_ProcessorUsages;

		[Column]
		public StorageTransactionUsageCollection StorageTransactionUsages { get; set; }
		// private StorageTransactionUsageCollection _unmodified_StorageTransactionUsages;

		[Column]
		public StorageUsageCollection StorageUsages { get; set; }
		// private StorageUsageCollection _unmodified_StorageUsages;

		[Column]
		public NetworkUsageCollection NetworkUsages { get; set; }
		// private NetworkUsageCollection _unmodified_NetworkUsages;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "RequestResourceUsage")]
	public class RequestResourceUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InformationOwnerInfo OwnerInfo { get; set; }
		// private InformationOwnerInfo _unmodified_OwnerInfo;

		[Column]
		public ProcessorUsage ProcessorUsage { get; set; }
		// private ProcessorUsage _unmodified_ProcessorUsage;

		[Column]
		public StorageTransactionUsage StorageTransactionUsage { get; set; }
		// private StorageTransactionUsage _unmodified_StorageTransactionUsage;

		[Column]
		public NetworkUsage NetworkUsage { get; set; }
		// private NetworkUsage _unmodified_NetworkUsage;

		[Column]
		public HTTPActivityDetails RequestDetails { get; set; }
		// private HTTPActivityDetails _unmodified_RequestDetails;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "ProcessorUsage")]
	public class ProcessorUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public double AmountOfTicks { get; set; }
		// private double _unmodified_AmountOfTicks;

		[Column]
		public double FrequencyTicksPerSecond { get; set; }
		// private double _unmodified_FrequencyTicksPerSecond;

		[Column]
		public long Milliseconds { get; set; }
		// private long _unmodified_Milliseconds;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "StorageTransactionUsage")]
	public class StorageTransactionUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public long AmountOfTransactions { get; set; }
		// private long _unmodified_AmountOfTransactions;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "StorageUsage")]
	public class StorageUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime SnapshotTime { get; set; }
		// private DateTime _unmodified_SnapshotTime;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public string UsageUnit { get; set; }
		// private string _unmodified_UsageUnit;

		[Column]
		public double AmountOfUnits { get; set; }
		// private double _unmodified_AmountOfUnits;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "NetworkUsage")]
	public class NetworkUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public long AmountOfBytes { get; set; }
		// private long _unmodified_AmountOfBytes;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "TimeRange")]
	public class TimeRange : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime StartTime { get; set; }
		// private DateTime _unmodified_StartTime;

		[Column]
		public DateTime EndTime { get; set; }
		// private DateTime _unmodified_EndTime;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "HTTPActivityDetails")]
	public class HTTPActivityDetails : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string RemoteIPAddress { get; set; }
		// private string _unmodified_RemoteIPAddress;

		[Column]
		public string RemoteEndpointUserName { get; set; }
		// private string _unmodified_RemoteEndpointUserName;

		[Column]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
		public DateTime UTCDateTime { get; set; }
		// private DateTime _unmodified_UTCDateTime;

		[Column]
		public string RequestLine { get; set; }
		// private string _unmodified_RequestLine;

		[Column]
		public long HTTPStatusCode { get; set; }
		// private long _unmodified_HTTPStatusCode;

		[Column]
		public long ReturnedContentLength { get; set; }
		// private long _unmodified_ReturnedContentLength;
        public void PrepareForStoring()
        {
		
		}
	}
 } 
