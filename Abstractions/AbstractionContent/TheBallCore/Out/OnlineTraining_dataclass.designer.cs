 


using DOM=ProBroz.OnlineTraining;


namespace ProBroz.OnlineTraining { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
#if !DEVEMU
using System.Runtime.Serialization;
using ProtoBuf;
#endif


namespace INT { 
					[DataContract]
			public partial class Member
			{
				[DataMember]
				public string ID { get; set; }
				[DataMember]
				public string ETag { get; set; }
				[DataMember]
				public string FirstName { get; set; }
				[DataMember]
				public string LastName { get; set; }
				[DataMember]
				public string MiddleName { get; set; }
				[DataMember]
				public DateTime BirthDay { get; set; }
				[DataMember]
				public string Email { get; set; }
				[DataMember]
				public string PhoneNumber { get; set; }
				[DataMember]
				public string Address { get; set; }
				[DataMember]
				public string Address2 { get; set; }
				[DataMember]
				public string ZipCode { get; set; }
				[DataMember]
				public string PostOffice { get; set; }
				[DataMember]
				public string Country { get; set; }
				[DataMember]
				public string FederationLicense { get; set; }
				[DataMember]
				public bool PhotoPermission { get; set; }
				[DataMember]
				public bool VideoPermission { get; set; }
			}

 } 			[DataContract] 
			[Serializable]
			public partial class MemberCollection 
			{
				public MemberCollection() 
				{
					Name = "MemberCollection";
					SemanticDomainName = "ProBroz.OnlineTraining";
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


				
				[DataMember] public List<Member> CollectionContent = new List<Member>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Member 
			{
				public Member() 
				{
					Name = "Member";
					SemanticDomainName = "ProBroz.OnlineTraining";
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
			public string FirstName { get; set; }
			[DataMember] 
			public string LastName { get; set; }
			[DataMember] 
			public string MiddleName { get; set; }
			[DataMember] 
			public DateTime BirthDay { get; set; }
			[DataMember] 
			public string Email { get; set; }
			[DataMember] 
			public string PhoneNumber { get; set; }
			[DataMember] 
			public string Address { get; set; }
			[DataMember] 
			public string Address2 { get; set; }
			[DataMember] 
			public string ZipCode { get; set; }
			[DataMember] 
			public string PostOffice { get; set; }
			[DataMember] 
			public string Country { get; set; }
			[DataMember] 
			public string FederationLicense { get; set; }
			[DataMember] 
			public bool PhotoPermission { get; set; }
			[DataMember] 
			public bool VideoPermission { get; set; }
			[DataMember] 
			public List< string > Subscriptions = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class MembershipPlanCollection 
			{
				public MembershipPlanCollection() 
				{
					Name = "MembershipPlanCollection";
					SemanticDomainName = "ProBroz.OnlineTraining";
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


				
				[DataMember] public List<MembershipPlan> CollectionContent = new List<MembershipPlan>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class MembershipPlan 
			{
				public MembershipPlan() 
				{
					Name = "MembershipPlan";
					SemanticDomainName = "ProBroz.OnlineTraining";
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
			public List< string > PaymentOptions = new List< string >();
			[DataMember] 
			public string Gym { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class PaymentOptionCollection 
			{
				public PaymentOptionCollection() 
				{
					Name = "PaymentOptionCollection";
					SemanticDomainName = "ProBroz.OnlineTraining";
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


				
				[DataMember] public List<PaymentOption> CollectionContent = new List<PaymentOption>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class PaymentOption 
			{
				public PaymentOption() 
				{
					Name = "PaymentOption";
					SemanticDomainName = "ProBroz.OnlineTraining";
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
			public string OptionName { get; set; }
			[DataMember] 
			public long PeriodInMonths { get; set; }
			[DataMember] 
			public double Price { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class SubscriptionCollection 
			{
				public SubscriptionCollection() 
				{
					Name = "SubscriptionCollection";
					SemanticDomainName = "ProBroz.OnlineTraining";
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


				
				[DataMember] public List<Subscription> CollectionContent = new List<Subscription>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Subscription 
			{
				public Subscription() 
				{
					Name = "Subscription";
					SemanticDomainName = "ProBroz.OnlineTraining";
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
			public string Plan { get; set; }
			[DataMember] 
			public string PaymentOption { get; set; }
			[DataMember] 
			public DateTime Created { get; set; }
			[DataMember] 
			public DateTime ValidFrom { get; set; }
			[DataMember] 
			public DateTime ValidTo { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TenantGymCollection 
			{
				public TenantGymCollection() 
				{
					Name = "TenantGymCollection";
					SemanticDomainName = "ProBroz.OnlineTraining";
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


				
				[DataMember] public List<TenantGym> CollectionContent = new List<TenantGym>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TenantGym 
			{
				public TenantGym() 
				{
					Name = "TenantGym";
					SemanticDomainName = "ProBroz.OnlineTraining";
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
			public string GymName { get; set; }
			[DataMember] 
			public string Email { get; set; }
			[DataMember] 
			public string PhoneNumber { get; set; }
			[DataMember] 
			public string Address { get; set; }
			[DataMember] 
			public string Address2 { get; set; }
			[DataMember] 
			public string ZipCode { get; set; }
			[DataMember] 
			public string PostOffice { get; set; }
			[DataMember] 
			public string Country { get; set; }
			
			}
 } 