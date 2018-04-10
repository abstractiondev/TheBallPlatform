 

namespace SER.Footvoter.Services { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class UserProfile
			{
				[DataMember]
				public string firstName { get; set; }
				[DataMember]
				public string lastName { get; set; }
				[DataMember]
				public string description { get; set; }
				[DataMember]
				public DateTime dateOfBirth { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class CompanyFollowData
			{
				[DataMember]
				public FollowDataItem[] FollowDataItems { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class FollowDataItem
			{
				[DataMember]
				public string IDToFollow { get; set; }
				[DataMember]
				public double FollowingLevel { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class VoteData
			{
				[DataMember]
				public VoteItem[] Votes { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class VoteItem
			{
				[DataMember]
				public string companyID { get; set; }
				[DataMember]
				public bool voteValue { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class VotedEntry
			{
				[DataMember]
				public string VotedForID { get; set; }
				[DataMember]
				public DateTime VoteTime { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class VotingSummary
			{
				[DataMember]
				public VotedEntry[] VotedEntries { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class CompanySearchCriteria
			{
				[DataMember]
				public string namePart { get; set; }
				[DataMember]
				public GpsLocation gpsLocation { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services.INT")]
			public partial class GpsLocation
			{
				[DataMember]
				public double latitude { get; set; }
				[DataMember]
				public double longitude { get; set; }
			}

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services")] 
			[Serializable]
			public partial class Company 
			{

				public Company()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "Footvoter.Services";
				    this.Name = "Company";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Company));
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

				public static Company DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Company));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Company) serializer.ReadObject(xmlReader);
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
			public string CompanyName { get; set; }
			private string _unmodified_CompanyName;
			[DataMember] 
			public string Details { get; set; }
			private string _unmodified_Details;
			[DataMember] 
			public double Footprint { get; set; }
			private double _unmodified_Footprint;
			[DataMember] 
			public List< double > Footpath = new List< double >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Footvoter.Services")] 
			[Serializable]
			public partial class Vote 
			{

				public Vote()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "Footvoter.Services";
				    this.Name = "Vote";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Vote));
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

				public static Vote DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Vote));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Vote) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(Vote sourceObject)
				{
					CompanyID = sourceObject.CompanyID;
					VoteValue = sourceObject.VoteValue;
					VoteTime = sourceObject.VoteTime;
				}
				



			[DataMember] 
			public string CompanyID { get; set; }
			private string _unmodified_CompanyID;
			[DataMember] 
			public bool VoteValue { get; set; }
			private bool _unmodified_VoteValue;
			[DataMember] 
			public DateTime VoteTime { get; set; }
			private DateTime _unmodified_VoteTime;
			
			}
 } 