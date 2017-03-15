 


using DOM=TheBall.Payments;


namespace TheBall.Payments { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;

namespace INT { 
					[DataContract]
			public partial class CancelSubscriptionParams
			{
				[DataMember]
				public string PlanName { get; set; }
			}

			[DataContract]
			public partial class StripeWebhookData
			{
				[DataMember]
				public string id { get; set; }
				[DataMember]
				public bool livemode { get; set; }
				[DataMember]
				public string type { get; set; }
			}

			[DataContract]
			public partial class ProductPurchaseInfo
			{
				[DataMember]
				public string currentproduct { get; set; }
				[DataMember]
				public double expectedprice { get; set; }
				[DataMember]
				public string currency { get; set; }
				[DataMember]
				public bool isTestMode { get; set; }
			}

			[DataContract]
			public partial class PaymentToken
			{
				[DataMember]
				public string id { get; set; }
				[DataMember]
				public string currentproduct { get; set; }
				[DataMember]
				public double expectedprice { get; set; }
				[DataMember]
				public string currency { get; set; }
				[DataMember]
				public string email { get; set; }
				[DataMember]
				public bool isTestMode { get; set; }
				[DataMember]
				public BillingAddress card { get; set; }
			}

			[DataContract]
			public partial class BillingAddress
			{
				[DataMember]
				public string name { get; set; }
				[DataMember]
				public string address_city { get; set; }
				[DataMember]
				public string address_country { get; set; }
				[DataMember]
				public string address_line1 { get; set; }
				[DataMember]
				public string address_line1_check { get; set; }
				[DataMember]
				public string address_zip { get; set; }
				[DataMember]
				public string address_zip_check { get; set; }
				[DataMember]
				public string cvc_check { get; set; }
				[DataMember]
				public string exp_month { get; set; }
				[DataMember]
				public string exp_year { get; set; }
				[DataMember]
				public string last4 { get; set; }
			}

			[DataContract]
			public partial class CustomerActivePlans
			{
				[DataMember]
				public PlanStatus[] PlanStatuses { get; set; }
			}

			[DataContract]
			public partial class PlanStatus
			{
				[DataMember]
				public string name { get; set; }
				[DataMember]
				public DateTime validuntil { get; set; }
				[DataMember]
				public bool cancelatperiodend { get; set; }
			}

 } 			[DataContract] 
			//[Serializable]
			public partial class GroupSubscriptionPlanCollection 
			{
				public GroupSubscriptionPlanCollection() 
				{
					Name = "GroupSubscriptionPlanCollection";
					SemanticDomainName = "TheBall.Payments";
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


				
				[DataMember] public List<GroupSubscriptionPlan> CollectionContent = new List<GroupSubscriptionPlan>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class GroupSubscriptionPlan 
			{
				public GroupSubscriptionPlan() 
				{
					Name = "GroupSubscriptionPlan";
					SemanticDomainName = "TheBall.Payments";
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
			public string PlanName { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public List< string > GroupIDs = new List< string >();
			
			}
			[DataContract] 
			//[Serializable]
			public partial class SubscriptionPlanStatus 
			{
				public SubscriptionPlanStatus() 
				{
					Name = "SubscriptionPlanStatus";
					SemanticDomainName = "TheBall.Payments";
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
			public string SubscriptionPlan { get; set; }
			[DataMember] 
			public DateTime ValidUntil { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class CustomerAccountCollection 
			{
				public CustomerAccountCollection() 
				{
					Name = "CustomerAccountCollection";
					SemanticDomainName = "TheBall.Payments";
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


				
				[DataMember] public List<CustomerAccount> CollectionContent = new List<CustomerAccount>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class CustomerAccount 
			{
				public CustomerAccount() 
				{
					Name = "CustomerAccount";
					SemanticDomainName = "TheBall.Payments";
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
			public string StripeID { get; set; }
			[DataMember] 
			public bool IsTestAccount { get; set; }
			[DataMember] 
			public string EmailAddress { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public List< string > ActivePlans = new List< string >();
			
			}
 } 