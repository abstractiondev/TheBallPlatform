 

namespace SER.TheBall.Index { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Index.INT")]
			public partial class UserQuery
			{
				[DataMember]
				public string QueryString { get; set; }
				[DataMember]
				public string DefaultFieldName { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Index.INT")]
			public partial class QueryToken
			{
				[DataMember]
				public string QueryRequestObjectDomainName { get; set; }
				[DataMember]
				public string QueryRequestObjectName { get; set; }
				[DataMember]
				public string QueryRequestObjectID { get; set; }
			}

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Index")] 
			[Serializable]
			public partial class IndexingRequest 
			{

				public IndexingRequest()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "IndexingRequest";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(IndexingRequest));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static IndexingRequest DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(IndexingRequest));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (IndexingRequest) serializer.ReadObject(xmlReader);
					}
            
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
			private string _unmodified_IndexName;
			[DataMember] 
			public List< string > ObjectLocations = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Index")] 
			[Serializable]
			public partial class QueryRequest 
			{

				public QueryRequest()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "QueryRequest";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryRequest));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static QueryRequest DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryRequest));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (QueryRequest) serializer.ReadObject(xmlReader);
					}
            
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
			private string _unmodified_QueryString;
			[DataMember] 
			public string DefaultFieldName { get; set; }
			private string _unmodified_DefaultFieldName;
			[DataMember] 
			public string IndexName { get; set; }
			private string _unmodified_IndexName;
			[DataMember] 
			public bool IsQueryCompleted { get; set; }
			private bool _unmodified_IsQueryCompleted;
			[DataMember] 
			public DateTime LastRequestTime { get; set; }
			private DateTime _unmodified_LastRequestTime;
			[DataMember] 
			public DateTime LastCompletionTime { get; set; }
			private DateTime _unmodified_LastCompletionTime;
			[DataMember] 
			public long LastCompletionDurationMs { get; set; }
			private long _unmodified_LastCompletionDurationMs;
			[DataMember] 
			public List< QueryResultItem > QueryResultObjects = new List< QueryResultItem >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Index")] 
			[Serializable]
			public partial class QueryResultItem 
			{

				public QueryResultItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Index";
				    this.Name = "QueryResultItem";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryResultItem));
					using (var output = new StringWriter())
					{
						using (var writer = new XmlTextWriter(output))
						{
                            if(noFormatting == false)
						        writer.Formatting = Formatting.Indented;
							serializer.WriteObject(writer, this);
						}
						return output.GetStringBuilder().ToString();
					}
				}

				public static QueryResultItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(QueryResultItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (QueryResultItem) serializer.ReadObject(xmlReader);
					}
            
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



				private void CopyContentFrom(QueryResultItem sourceObject)
				{
					ObjectDomainName = sourceObject.ObjectDomainName;
					ObjectName = sourceObject.ObjectName;
					ObjectID = sourceObject.ObjectID;
					Rank = sourceObject.Rank;
				}
				



			[DataMember] 
			public string ObjectDomainName { get; set; }
			private string _unmodified_ObjectDomainName;
			[DataMember] 
			public string ObjectName { get; set; }
			private string _unmodified_ObjectName;
			[DataMember] 
			public string ObjectID { get; set; }
			private string _unmodified_ObjectID;
			[DataMember] 
			public double Rank { get; set; }
			private double _unmodified_Rank;
			
			}
 } 