 


namespace Footvoter.Services { 
		
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class UserProfile 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string firstName;

			[DataMember]
			public string lastName;

			[DataMember]
			public string description;

			[DataMember]
			public DateTime dateOfBirth;

			
			}
			[DataContract]
			public partial class CompanyFollowData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<FollowDataItem> FollowDataItems= new List<FollowDataItem>();

			
			}
			[DataContract]
			public partial class FollowDataItem 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string IDToFollow;

			[DataMember]
			public double FollowingLevel;

			
			}
			[DataContract]
			public partial class VoteData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<VoteItem> Votes= new List<VoteItem>();

			
			}
			[DataContract]
			public partial class VoteItem 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string companyID;

			[DataMember]
			public bool voteValue;

			
			}
			[DataContract]
			public partial class VotedEntry 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string VotedForID;

			[DataMember]
			public DateTime VoteTime;

			
			}
			[DataContract]
			public partial class VotingSummary 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<VotedEntry> VotedEntries= new List<VotedEntry>();

			
			}
			[DataContract]
			public partial class CompanySearchCriteria 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string namePart;

			[DataMember]
			public GpsLocation gpsLocation;

			
			}
			[DataContract]
			public partial class GpsLocation 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public double latitude;

			[DataMember]
			public double longitude;

			
			}
 } 