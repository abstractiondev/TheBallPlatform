 


using DOM=TheBall.Index;


namespace TheBall.Index { 
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
			public partial class UserQuery
			{
				[DataMember]
				public string QueryString { get; set; }
				[DataMember]
				public string DefaultFieldName { get; set; }
			}

			[DataContract]
			public partial class QueryToken
			{
				[DataMember]
				public string QueryRequestObjectDomainName { get; set; }
				[DataMember]
				public string QueryRequestObjectName { get; set; }
				[DataMember]
				public string QueryRequestObjectID { get; set; }
			}

 } 			[DataContract] 
			[Serializable]
			public partial class IndexingRequest 
			{
				public IndexingRequest() 
				{
					Name = "IndexingRequest";
					SemanticDomainName = "TheBall.Index";
				}

				[DataMember] 
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] 
                public string Name { get; set; }

                [DataMember] 
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] 
			public string IndexName { get; set; }
			[DataMember] 
			public List< string > ObjectLocations = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class QueryRequest 
			{
				public QueryRequest() 
				{
					Name = "QueryRequest";
					SemanticDomainName = "TheBall.Index";
				}

				[DataMember] 
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] 
                public string Name { get; set; }

                [DataMember] 
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] 
			public string QueryString { get; set; }
			[DataMember] 
			public string DefaultFieldName { get; set; }
			[DataMember] 
			public string IndexName { get; set; }
			[DataMember] 
			public bool IsQueryCompleted { get; set; }
			[DataMember] 
			public DateTime LastRequestTime { get; set; }
			[DataMember] 
			public DateTime LastCompletionTime { get; set; }
			[DataMember] 
			public long LastCompletionDurationMs { get; set; }
			[DataMember] 
			public List< QueryResultItem > QueryResultObjects = new List< QueryResultItem >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class QueryResultItem 
			{
				public QueryResultItem() 
				{
					Name = "QueryResultItem";
					SemanticDomainName = "TheBall.Index";
				}

				[DataMember] 
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] 
                public string Name { get; set; }

                [DataMember] 
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] 
			public string ObjectDomainName { get; set; }
			[DataMember] 
			public string ObjectName { get; set; }
			[DataMember] 
			public string ObjectID { get; set; }
			[DataMember] 
			public double Rank { get; set; }
			
			}
 } 