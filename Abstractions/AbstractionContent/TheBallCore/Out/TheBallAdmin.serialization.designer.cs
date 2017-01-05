 

namespace SER.TheBall.Admin { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Admin.INT")]
			public partial class UsersData
			{
				[DataMember]
				public AccountInfo[] AccountInfos { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Admin.INT")]
			public partial class AccountInfo
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string EmailAddress { get; set; }
			}

 }  } 