 

namespace SER.TheBall.Infrastructure { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
//using ProtoBuf;
using System.Threading.Tasks;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class UpdateConfig
			{
				[DataMember]
				public UpdateConfigItem[] PackageData { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class UpdateConfigItem
			{
				[DataMember]
				public AccessInfo AccessInfo { get; set; }
				[DataMember]
				public string Name { get; set; }
				[DataMember]
				public string MaturityLevel { get; set; }
				[DataMember]
				public string BuildNumber { get; set; }
				[DataMember]
				public string Commit { get; set; }
				[DataMember]
				public StatusInfo Status { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class StatusInfo
			{
				[DataMember]
				public double TestResult { get; set; }
				[DataMember]
				public DateTime TestedAt { get; set; }
				[DataMember]
				public DateTime InstalledAt { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class AccessInfo
			{
				[DataMember]
				public string AccountName { get; set; }
				[DataMember]
				public string ShareName { get; set; }
				[DataMember]
				public string SASToken { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class WebConsoleConfig
			{
				[DataMember]
				public double PollingIntervalSeconds { get; set; }
				[DataMember]
				public UpdateConfigItem[] PackageData { get; set; }
				[DataMember]
				public MaturityBindingItem[] InstanceBindings { get; set; }
				[DataMember]
				public string WwwSitesMaturityLevel { get; set; }
				[DataMember]
				public string[] WwwSiteHostNames { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class BaseUIConfigSet
			{
				[DataMember]
				public UpdateConfigItem AboutConfig { get; set; }
				[DataMember]
				public UpdateConfigItem AccountConfig { get; set; }
				[DataMember]
				public UpdateConfigItem GroupConfig { get; set; }
				[DataMember]
				public StatusInfo StatusSummary { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class InstanceUIConfig
			{
				[DataMember]
				public BaseUIConfigSet DesiredConfig { get; set; }
				[DataMember]
				public BaseUIConfigSet ConfigInTesting { get; set; }
				[DataMember]
				public BaseUIConfigSet EffectiveConfig { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class MaturityBindingItem
			{
				[DataMember]
				public string MaturityLevel { get; set; }
				[DataMember]
				public string[] Instances { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Infrastructure.INT")]
			public partial class DeploymentPackages
			{
				[DataMember]
				public UpdateConfigItem[] PackageData { get; set; }
			}

 } 	#region Operation Calls
	public partial class Server 
	{
	    public delegate Task ExecuteOperationFunc(string operationName, object parameters = null);

	    public static ExecuteOperationFunc ExecuteOperation;

	    public delegate Task<object> GetObjectFunc(Type type, string id);

	    public static GetObjectFunc GetInformationObjectImplementation;
	    public static GetObjectFunc GetInterfaceObjectImplementation;


        private static async Task<T> GetInformationObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInformationObjectImplementation(type, id);
	        return (T) objResult;
	    }

	    private static async Task<T> GetInterfaceObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInterfaceObjectImplementation(type, id);
	        return (T)objResult;
	    }


		public static async Task UpdateInfraDataInterfaceObjects() 
		{
			await ExecuteOperation("TheBall.Infrastructure.UpdateInfraDataInterfaceObjects");
		}

		public static async Task SetRuntimeVersions(INT.UpdateConfig param) 
		{
			await ExecuteOperation("TheBall.Infrastructure.SetRuntimeVersions", param);
		}

		public static async Task CreateCloudDrive() 
		{
			await ExecuteOperation("TheBall.Infrastructure.CreateCloudDrive");
		}

		public static async Task MountCloudDrive() 
		{
			await ExecuteOperation("TheBall.Infrastructure.MountCloudDrive");
		}
		public static async Task<INT.UpdateConfig> GetUpdateConfig(string id = null)
		{
			var result = await GetInterfaceObject<INT.UpdateConfig>(id);
			return result;
		}
		public static async Task<INT.UpdateConfigItem> GetUpdateConfigItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.UpdateConfigItem>(id);
			return result;
		}
		public static async Task<INT.StatusInfo> GetStatusInfo(string id = null)
		{
			var result = await GetInterfaceObject<INT.StatusInfo>(id);
			return result;
		}
		public static async Task<INT.AccessInfo> GetAccessInfo(string id = null)
		{
			var result = await GetInterfaceObject<INT.AccessInfo>(id);
			return result;
		}
		public static async Task<INT.WebConsoleConfig> GetWebConsoleConfig(string id = null)
		{
			var result = await GetInterfaceObject<INT.WebConsoleConfig>(id);
			return result;
		}
		public static async Task<INT.BaseUIConfigSet> GetBaseUIConfigSet(string id = null)
		{
			var result = await GetInterfaceObject<INT.BaseUIConfigSet>(id);
			return result;
		}
		public static async Task<INT.InstanceUIConfig> GetInstanceUIConfig(string id = null)
		{
			var result = await GetInterfaceObject<INT.InstanceUIConfig>(id);
			return result;
		}
		public static async Task<INT.MaturityBindingItem> GetMaturityBindingItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.MaturityBindingItem>(id);
			return result;
		}
		public static async Task<INT.DeploymentPackages> GetDeploymentPackages(string id = null)
		{
			var result = await GetInterfaceObject<INT.DeploymentPackages>(id);
			return result;
		}
	}
#endregion
 } 