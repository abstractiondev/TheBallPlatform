 


using DOM=TheBall.Admin;


namespace TheBall.Admin { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;

namespace INT { 
					[DataContract]
			public partial class UsersData
			{
				[DataMember]
				public AccountInfo[] AccountInfos { get; set; }
			}

			[DataContract]
			public partial class AccountInfo
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string EmailAddress { get; set; }
			}

 }  } 