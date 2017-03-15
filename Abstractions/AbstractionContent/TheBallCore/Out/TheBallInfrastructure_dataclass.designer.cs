 


using DOM=TheBall.Infrastructure;


namespace TheBall.Infrastructure { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
#if !DEVEMU
using System.Runtime.Serialization;
using ProtoBuf;
#endif


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

 }  } 