 


namespace ProBroz.OnlineTraining { 
		
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class Member 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string ID;

			[DataMember]
			public string ETag;

			[DataMember]
			public string FirstName;

			[DataMember]
			public string LastName;

			[DataMember]
			public string MiddleName;

			[DataMember]
			public DateTime BirthDay;

			[DataMember]
			public string Email;

			[DataMember]
			public string PhoneNumber;

			[DataMember]
			public string Address;

			[DataMember]
			public string Address2;

			[DataMember]
			public string ZipCode;

			[DataMember]
			public string PostOffice;

			[DataMember]
			public string Country;

			[DataMember]
			public string FederationLicense;

			[DataMember]
			public bool PhotoPermission;

			[DataMember]
			public bool VideoPermission;

			
			}
 } 