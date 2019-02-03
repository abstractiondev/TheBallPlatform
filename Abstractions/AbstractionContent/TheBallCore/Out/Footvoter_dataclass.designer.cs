 


using DOM=Footvoter.Services;


namespace Footvoter.Services { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;

namespace INT { 
					[DataContract]
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

			[DataContract]
			public partial class CompanyFollowData
			{
				[DataMember]
				public FollowDataItem[] FollowDataItems { get; set; }
			}

			[DataContract]
			public partial class FollowDataItem
			{
				[DataMember]
				public string IDToFollow { get; set; }
				[DataMember]
				public double FollowingLevel { get; set; }
			}

			[DataContract]
			public partial class VoteData
			{
				[DataMember]
				public VoteItem[] Votes { get; set; }
			}

			[DataContract]
			public partial class VoteItem
			{
				[DataMember]
				public string companyID { get; set; }
				[DataMember]
				public bool voteValue { get; set; }
			}

			[DataContract]
			public partial class VotedEntry
			{
				[DataMember]
				public string VotedForID { get; set; }
				[DataMember]
				public DateTime VoteTime { get; set; }
			}

			[DataContract]
			public partial class VotingSummary
			{
				[DataMember]
				public VotedEntry[] VotedEntries { get; set; }
			}

			[DataContract]
			public partial class CompanySearchCriteria
			{
				[DataMember]
				public string namePart { get; set; }
				[DataMember]
				public GpsLocation gpsLocation { get; set; }
			}

			[DataContract]
			public partial class GpsLocation
			{
				[DataMember]
				public double latitude { get; set; }
				[DataMember]
				public double longitude { get; set; }
			}

 } 			[DataContract] 
			//[Serializable]
			public partial class Company 
			{
				public Company() 
				{
					Name = "Company";
					SemanticDomainName = "Footvoter.Services";
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
			[DataMember] 
			public string Details { get; set; }
			[DataMember] 
			public double Footprint { get; set; }
			[DataMember] 
			public List< double > Footpath = new List< double >();
			
			}
			[DataContract] 
			//[Serializable]
			public partial class Vote 
			{
				public Vote() 
				{
					Name = "Vote";
					SemanticDomainName = "Footvoter.Services";
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
			public string CompanyID { get; set; }
			[DataMember] 
			public bool VoteValue { get; set; }
			[DataMember] 
			public DateTime VoteTime { get; set; }
			
			}
 } 