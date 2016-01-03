using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using DiagnosticsUtils;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.Index;
using TheBall.Interface;

namespace TheBall
{
    public class InformationContext
    {
        private const string KEYNAME = "INFOCTX";
        public string CurrentGroupRole;

        public readonly long SerialNumber;
        private static long CurrentSerial = 0;
        public InformationContext()
        {
            SerialNumber = Interlocked.Increment(ref CurrentSerial);
        }

        public static bool AllowStatic { get; private set; }

        public static DeviceMembership CurrentExecutingForDevice
        {
            get { return Current.ExecutingForDevice; }
        }

        public static IContainerOwner CurrentOwner
        {
            get { return Current.Owner; }
        }

        public static IAccountInfo CurrentAccount
        {
            get { return Current.Account; }
        }

        public static InformationContext InitializeToLogicalContext()
        {
            var logicalContext = CallContext.LogicalGetData(KEYNAME);
            if(logicalContext != null)
                throw new InvalidOperationException("LogicalContext already initialized");
            var ctx = InformationContext.Create();
            CallContext.LogicalSetData(KEYNAME, ctx);
            return ctx;
        }

        public static void RemoveFromLogicalContext()
        {
            var current = CallContext.LogicalGetData(KEYNAME);
            if (current == null)
                throw new InvalidOperationException("LogicalContext is expected to be initialized");
            CallContext.LogicalSetData(KEYNAME, null);
        }

        public static InformationContext Current
        {
            get
            {
                var httpContext = HttpContext.Current;
                if(httpContext != null)
                {
                    if (httpContext.Items.Contains(KEYNAME))
                        return (InformationContext) httpContext.Items[KEYNAME];
                    InformationContext informationContext = InformationContext.Create();
                    httpContext.Items.Add(KEYNAME, informationContext);
                    return informationContext;
                }

                var logicalContext = CallContext.LogicalGetData(KEYNAME);
                if (logicalContext != null)
                    return (InformationContext) logicalContext;

                throw new InvalidOperationException("InitializeToLogicalContext required when HttpContext is not available");
            }
        }

        public static void updateStatusSummary()
        {
            try
            {
                var changedList = InformationContext.Current.GetChangedObjectIDs();
                if (changedList.Length > 0)
                {
                    UpdateStatusSummaryParameters parameters = new UpdateStatusSummaryParameters
                    {
                        Owner = InformationContext.Current.Owner,
                        UpdateTime = DateTime.UtcNow,
                        ChangedIDList = changedList,
                        RemoveExpiredEntriesSeconds = InfraSharedConfig.HARDCODED_StatusUpdateExpireSeconds,
                    };
                    UpdateStatusSummary.Execute(parameters);
                }
            }
            catch (Exception ex) // DO NOT FAIL HERE
            {

            }
            
        }


        public static void ProcessAndClearCurrent()
        {
            Current.PerformFinalizingActions();
            ClearCurrent();
        }

        public static void ClearCurrent()
        {
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                if (httpContext.Items.Contains(KEYNAME))
                {
                    httpContext.Items.Remove(KEYNAME);
                    return;
                }
            }

            if(CallContext.LogicalGetData(KEYNAME) != null)
                CallContext.LogicalSetData(KEYNAME, null);

            throw new InvalidOperationException("InformationContext ClearCurrent failed - no active context set");
        }

        private static InformationContext Create()
        {
            return new InformationContext();
        }

        public void PerformFinalizingActions()
        {
            updateStatusSummary();
            createIndexingRequest();

            if (isResourceMeasuring)
            {
                // Complete resource measuring - add one more transaction because the RRU item is stored itself
                AddStorageTransactionToCurrent();
                CompleteResourceMeasuring();
                //var owner = _owner;
                //if (owner == null)
                //    owner = TBSystem.CurrSystem;
                // Resources usage items need to be under system, because they are handled as a batch
                // The refined logging/storage is then saved under owner's context
                var measurementOwner = TBSystem.CurrSystem;
                var resourceUser = _owner ?? measurementOwner;
                RequestResourceUsage.OwnerInfo.OwnerType = resourceUser.ContainerName;
                RequestResourceUsage.OwnerInfo.OwnerIdentifier = resourceUser.LocationPrefix;
                var now = RequestResourceUsage.ProcessorUsage.TimeRange.EndTime;
                string uniquePostFix = Guid.NewGuid().ToString("N");
                string itemName = now.ToString("yyyyMMddHHmmssfff") + "_" + uniquePostFix;
                RequestResourceUsage.SetLocationAsOwnerContent(measurementOwner, itemName);
                RequestResourceUsage.StoreInformation();
            }
        }

        private void createIndexingRequest()
        {
            if (_owner != null && IndexedIDInfos.Count > 0)
            {
                FilterAndSubmitIndexingRequests.Execute(new FilterAndSubmitIndexingRequestsParameters { CandidateObjectLocations = IndexedIDInfos.ToArray() });
            }
        }

        private RuntimeConfiguration _instanceConfiguration;
        public static RuntimeConfiguration InstanceConfiguration
        {
            get
            {
                if (Current._instanceConfiguration == null)
                    Current._instanceConfiguration = RuntimeConfiguration.GetConfiguration(Current.InstanceName);
                return Current._instanceConfiguration;
            }
        }

        public string InstanceName { get; set; }

        public string InitializedContainerName { get; private set; }
        public void InitializeCloudStorageAccess(string containerName, bool reinitialize = false)
        {
            if(containerName == null)
                throw new ArgumentNullException("containerName");
            if(containerName == "")
                throw new ArgumentException("Invalid container name", "containerName");
            if (reinitialize)
                UninitializeCloudStorageAccess();
            if(InitializedContainerName != null)
            {
                if (containerName == InitializedContainerName)
                    return;
                if(containerName != InitializedContainerName)
                    throw new NotSupportedException("InitializeCloudStorageAccess already initialized with container name: " 
                        + InitializedContainerName + " (tried to initialize with: "
                        + containerName + ")");
            }
            CloudBlobClient blobClient = StorageSupport.CurrStorageAccount.CreateCloudBlobClient();
            blobClient.RetryPolicy = RetryPolicies.Retry(10, TimeSpan.FromMilliseconds(300));
            CurrBlobClient = blobClient;

            var activeContainer = blobClient.GetContainerReference(containerName.ToLower());
            activeContainer.CreateIfNotExist();
            CurrActiveContainer = activeContainer;
            InitializedContainerName = containerName;
        }

        public void UninitializeCloudStorageAccess()
        {
            CurrBlobClient = null;
            CurrActiveContainer = null;
            InitializedContainerName = null;
        }

        public void ReinitializeCloudStorageAccess(string containerName)
        {
            UninitializeCloudStorageAccess();
        }

        private CloudBlobContainer _currActiveContainer;
        public CloudBlobContainer CurrActiveContainer { 
            get
            {
                if(_currActiveContainer == null)
                    throw new NotSupportedException("CurrActiveContainer needs to be initialized with InitializeCloudStorageAccess method");
                return _currActiveContainer;
            }
            private set
            {
                if(_currActiveContainer != null && value != null)
                    throw new NotSupportedException("CurrActiveContainer can only be set once");
                _currActiveContainer = value;
            }
        }

        private CloudBlobClient _currBlobClient;
        public bool IsExecutingSubscriptions;

        public CloudBlobClient CurrBlobClient { 
            get
            {
                if(_currBlobClient == null)
                    throw new NotSupportedException(
                        "CurrBlobClient needs to be initialized with InitializeCloudStorageAccess method");
                return _currBlobClient;
            }
            private set
            {
                if(_currBlobClient != null && value != null)
                    throw new NotSupportedException("CurrBlobClient can only be set once");
                _currBlobClient = value;
            }
        }

        private IAccountInfo _account;
        public IAccountInfo Account
        {
            set { _account = value; }
            get { return _account; }
        }

        private IContainerOwner _owner;
        public bool IsOwnerDefined { get { return _owner != null; } }
        public IContainerOwner Owner
        {
            set { _owner = value; }
            get
            {
                if(_owner == null)
                    throw new InvalidDataException("Owner not set, but still requested in active information context");
                return _owner;
            }
        }

        private DeviceMembership _executingForDevice;
        public DeviceMembership ExecutingForDevice
        {
            set { _executingForDevice = value; }
            get
            {
                if(_executingForDevice == null)
                    throw new InvalidDataException("ExecutingForDevice not set, but still requested in active information context");
                return _executingForDevice;
            }
        }

        private Dictionary<string, object> KeyValueDictionary = new Dictionary<string, object>();
        public void AccessLockedItems(Action<Dictionary<string, object>> action)
        {
            lock (KeyValueDictionary)
            {
                action(KeyValueDictionary);
            }
        }

        public RequestResourceUsage RequestResourceUsage;
        private ExecutionStopwatch executionStopwatch = new ExecutionStopwatch();
        private Stopwatch realtimeStopwatch = new Stopwatch();
        private DateTime startTimeInaccurate;
        private bool isResourceMeasuring = false;
        private string UsageTypeString;
        private void StartResourceMeasuring(ResourceUsageType usageType)
        {
            UsageTypeString = usageType.ToString();
            //Debugger.Break();
            startTimeInaccurate = DateTime.UtcNow;
            executionStopwatch.Start();
            realtimeStopwatch.Start();
            isResourceMeasuring = true;
            RequestResourceUsage = new RequestResourceUsage
                {
                    ProcessorUsage = new ProcessorUsage
                        {
                            TimeRange = new TimeRange(),
                            UsageType = UsageTypeString,
                        },
                    OwnerInfo = new InformationOwnerInfo(),
                    NetworkUsage = new NetworkUsage() { UsageType = UsageTypeString},
                    RequestDetails = new HTTPActivityDetails(),
                    StorageTransactionUsage = new StorageTransactionUsage { UsageType = UsageTypeString},
                };
        }

        private void CompleteResourceMeasuring()
        {
            realtimeStopwatch.Stop();
            executionStopwatch.Stop();
            var processorUsage = RequestResourceUsage.ProcessorUsage;
            processorUsage.TimeRange.StartTime = startTimeInaccurate;
            processorUsage.TimeRange.EndTime = startTimeInaccurate.AddTicks(realtimeStopwatch.ElapsedTicks);
            RequestResourceUsage.ProcessorUsage.Milliseconds = (long) executionStopwatch.Elapsed.TotalMilliseconds;
        }

        public static void AddStorageTransactionToCurrent()
        {
            if (Current == null || Current.RequestResourceUsage == null ||
                Current.RequestResourceUsage.StorageTransactionUsage == null)
                return;
            Current.RequestResourceUsage.StorageTransactionUsage.AmountOfTransactions++;
        }

        public static void AddNetworkOutputByteAmountToCurrent(long bytes)
        {
            if (Current == null || Current.RequestResourceUsage == null ||
                Current.RequestResourceUsage.NetworkUsage == null)
                return;
            Current.RequestResourceUsage.NetworkUsage.AmountOfBytes += bytes;
        }

        public enum ResourceUsageType
        {
            WebRole,
            WorkerRole,
            WorkerIndexing,
            WorkerQuery,
        }

        public static void StartResourceMeasuringOnCurrent(ResourceUsageType usageType)
        {
            Current.StartResourceMeasuring(usageType);
        }

        public enum ObjectChangeType
        {
            M_Modified = 1,
            N_New = 2,
            D_Deleted = 3,
        }

        // Currently being used for listing changed IDs
        private HashSet<string> ChangedIDInfos = new HashSet<string>();
        private HashSet<string> IndexedIDInfos = new HashSet<string>();
        private string[] trackedDomainNames = new[] { "AaltoGlobalImpact.OIP", "TheBall.Interface", "TheBall.Index" };
        public void ObjectStoredNotification(IInformationObject informationObject, ObjectChangeType changeType)
        {
            IIndexedDocument iDoc = informationObject as IIndexedDocument;
            if (iDoc != null && _owner != null)
            {
                VirtualOwner owner = VirtualOwner.FigureOwner(informationObject);
                if (owner.IsEqualOwner(_owner))
                {
                    IndexedIDInfos.Add(informationObject.RelativeLocation);
                }
            }

            if (trackedDomainNames.Contains(informationObject.SemanticDomainName) == false)
                return;
            string changeTypeValue;
            switch (changeType)
            {
                case ObjectChangeType.D_Deleted:
                    changeTypeValue = "D";
                    break;
                case ObjectChangeType.M_Modified:
                    changeTypeValue = "M";
                    break;
                case ObjectChangeType.N_New:
                    changeTypeValue = "N";
                    break;
                default:
                    throw new NotSupportedException("ChangeType handling not implemented: " + changeType.ToString());

            }
            string changedIDInfo = changeTypeValue + ":" + informationObject.ID;
            ChangedIDInfos.Add(changedIDInfo);
        }

        public string[] GetChangedObjectIDs()
        {
            List<string> idList = new List<string>();
            foreach (string id in ChangedIDInfos)
                idList.Add(id);
            return idList.ToArray();
        }
    }
}