 


using DOM=TheBall.Infrastructure;
using System.Threading.Tasks;

namespace TheBall.Core {
	public static partial class OwnerInitializer
	{
		private static async Task DOMAININIT_TheBall_Infrastructure(IContainerOwner owner)
		{
			await DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			await DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.Infrastructure { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;
using ProtoBuf;
using TheBall;
using TheBall.Core;
using TheBall.Core.StorageCore;

namespace INT { 
					[DataContract]
			public partial class UpdateConfig
			{
				[DataMember]
				public UpdateConfigItem[] PackageData { get; set; }
			}

			[DataContract]
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

			[DataContract]
			public partial class StatusInfo
			{
				[DataMember]
				public double TestResult { get; set; }
				[DataMember]
				public DateTime TestedAt { get; set; }
				[DataMember]
				public DateTime InstalledAt { get; set; }
			}

			[DataContract]
			public partial class AccessInfo
			{
				[DataMember]
				public string AccountName { get; set; }
				[DataMember]
				public string ShareName { get; set; }
				[DataMember]
				public string SASToken { get; set; }
			}

			[DataContract]
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

			[DataContract]
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

			[DataContract]
			public partial class InstanceUIConfig
			{
				[DataMember]
				public BaseUIConfigSet DesiredConfig { get; set; }
				[DataMember]
				public BaseUIConfigSet ConfigInTesting { get; set; }
				[DataMember]
				public BaseUIConfigSet EffectiveConfig { get; set; }
			}

			[DataContract]
			public partial class MaturityBindingItem
			{
				[DataMember]
				public string MaturityLevel { get; set; }
				[DataMember]
				public string[] Instances { get; set; }
			}

			[DataContract]
			public partial class DeploymentPackages
			{
				[DataMember]
				public UpdateConfigItem[] PackageData { get; set; }
			}

 } 		public static class DomainInformationSupport
		{
            public static async Task EnsureMasterCollections(IContainerOwner owner)
            {
            }

            public static async Task RefreshMasterCollections(IContainerOwner owner)
            {
            }
		}
 } 