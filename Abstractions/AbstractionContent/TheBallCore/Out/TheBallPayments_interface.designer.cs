 


namespace TheBall.Payments { 
		
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class PaymentToken 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string id;

			[DataMember]
			public string currentproduct;

			[DataMember]
			public string email;

			
			}
 } 