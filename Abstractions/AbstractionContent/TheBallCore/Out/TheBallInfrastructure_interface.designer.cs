 


namespace TheBall.Infrastructure { 
		
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class UpdateConfig 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<UpdateConfigItem> PackageData= new List<UpdateConfigItem>();

			
			}
			[DataContract]
			public partial class UpdateConfigItem 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public AccessInfo AccessInfo;

			[DataMember]
			public string Name;

			[DataMember]
			public string MaturityLevel;

			[DataMember]
			public string BuildNumber;

			[DataMember]
			public string Commit;

			
			}
			[DataContract]
			public partial class AccessInfo 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string AccountName;

			[DataMember]
			public string ShareName;

			[DataMember]
			public string SASToken;

			
			}
			[DataContract]
			public partial class WebConsoleConfig 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public double PollingIntervalSeconds;

			[DataMember]
			public List<UpdateConfigItem> PackageData= new List<UpdateConfigItem>();

			[DataMember]
			public List<MaturityBindingItem> InstanceBindings= new List<MaturityBindingItem>();

			[DataMember]
			public string WwwSitesMaturityLevel;

			[DataMember]
			public List<string> WwwSiteHostNames= new List<string>();

			
			}
			[DataContract]
			public partial class MaturityBindingItem 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string MaturityLevel;

			[DataMember]
			public List<string> Instances= new List<string>();

			
			}
			[DataContract]
			public partial class DeploymentPackages 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<UpdateConfigItem> PackageData= new List<UpdateConfigItem>();

			
			}
 } 