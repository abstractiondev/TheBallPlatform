 


namespace TheBall.Payments { 
		
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;



			[DataContract]
			public partial class CancelSubscriptionParams 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string PlanName;

			
			}
			[DataContract]
			public partial class StripeWebhookData 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string id;

			[DataMember]
			public bool livemode;

			[DataMember]
			public string type;

			
			}
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
			public double expectedprice;

			[DataMember]
			public string email;

			[DataMember]
			public BillingAddress card;

			
			}
			[DataContract]
			public partial class BillingAddress 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string name;

			[DataMember]
			public string address_city;

			[DataMember]
			public string address_country;

			[DataMember]
			public string address_line1;

			[DataMember]
			public string address_line1_check;

			[DataMember]
			public string address_zip;

			[DataMember]
			public string address_zip_check;

			[DataMember]
			public string cvc_check;

			[DataMember]
			public string exp_month;

			[DataMember]
			public string exp_year;

			[DataMember]
			public string last4;

			
			}
			[DataContract]
			public partial class CustomerActivePlans 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public List<PlanStatus> PlanStatuses= new List<PlanStatus>();

			
			}
			[DataContract]
			public partial class PlanStatus 
			{
				[DataMember]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

			[DataMember]
			public string name;

			[DataMember]
			public DateTime validuntil;

			[DataMember]
			public bool cancelatperiodend;

			
			}
 } 