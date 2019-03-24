 

namespace SER.ProBroz.OnlineTraining { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
//using ProtoBuf;
using System.Threading.Tasks;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining.INT")]
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

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class MemberCollection 
			{

				public MemberCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "MemberCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MemberCollection));
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

				public static MemberCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MemberCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MemberCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<Member> CollectionContent = new List<Member>();
				private Member[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public Member[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				private void CopyContentFrom(MemberCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class Member 
			{

				public Member()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "Member";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Member));
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

				public static Member DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Member));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Member) serializer.ReadObject(xmlReader);
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
			public string FirstName { get; set; }
			private string _unmodified_FirstName;
			[DataMember] 
			public string LastName { get; set; }
			private string _unmodified_LastName;
			[DataMember] 
			public string MiddleName { get; set; }
			private string _unmodified_MiddleName;
			[DataMember] 
			public DateTime BirthDay { get; set; }
			private DateTime _unmodified_BirthDay;
			[DataMember] 
			public string Email { get; set; }
			private string _unmodified_Email;
			[DataMember] 
			public string PhoneNumber { get; set; }
			private string _unmodified_PhoneNumber;
			[DataMember] 
			public string Address { get; set; }
			private string _unmodified_Address;
			[DataMember] 
			public string Address2 { get; set; }
			private string _unmodified_Address2;
			[DataMember] 
			public string ZipCode { get; set; }
			private string _unmodified_ZipCode;
			[DataMember] 
			public string PostOffice { get; set; }
			private string _unmodified_PostOffice;
			[DataMember] 
			public string Country { get; set; }
			private string _unmodified_Country;
			[DataMember] 
			public string FederationLicense { get; set; }
			private string _unmodified_FederationLicense;
			[DataMember] 
			public bool PhotoPermission { get; set; }
			private bool _unmodified_PhotoPermission;
			[DataMember] 
			public bool VideoPermission { get; set; }
			private bool _unmodified_VideoPermission;
			[DataMember] 
			public List< string > Subscriptions = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class MembershipPlanCollection 
			{

				public MembershipPlanCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "MembershipPlanCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MembershipPlanCollection));
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

				public static MembershipPlanCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MembershipPlanCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MembershipPlanCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<MembershipPlan> CollectionContent = new List<MembershipPlan>();
				private MembershipPlan[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public MembershipPlan[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				private void CopyContentFrom(MembershipPlanCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class MembershipPlan 
			{

				public MembershipPlan()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "MembershipPlan";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MembershipPlan));
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

				public static MembershipPlan DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MembershipPlan));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (MembershipPlan) serializer.ReadObject(xmlReader);
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
			public string PlanName { get; set; }
			private string _unmodified_PlanName;
			[DataMember] 
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember] 
			public List< string > PaymentOptions = new List< string >();
			[DataMember] 
			public string Gym { get; set; }
			private string _unmodified_Gym;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class PaymentOptionCollection 
			{

				public PaymentOptionCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "PaymentOptionCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PaymentOptionCollection));
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

				public static PaymentOptionCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PaymentOptionCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (PaymentOptionCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<PaymentOption> CollectionContent = new List<PaymentOption>();
				private PaymentOption[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public PaymentOption[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				private void CopyContentFrom(PaymentOptionCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class PaymentOption 
			{

				public PaymentOption()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "PaymentOption";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PaymentOption));
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

				public static PaymentOption DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(PaymentOption));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (PaymentOption) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(PaymentOption sourceObject)
				{
					OptionName = sourceObject.OptionName;
					PeriodInMonths = sourceObject.PeriodInMonths;
					Price = sourceObject.Price;
				}
				



			[DataMember] 
			public string OptionName { get; set; }
			private string _unmodified_OptionName;
			[DataMember] 
			public long PeriodInMonths { get; set; }
			private long _unmodified_PeriodInMonths;
			[DataMember] 
			public double Price { get; set; }
			private double _unmodified_Price;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class SubscriptionCollection 
			{

				public SubscriptionCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "SubscriptionCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection));
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

				public static SubscriptionCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SubscriptionCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<Subscription> CollectionContent = new List<Subscription>();
				private Subscription[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public Subscription[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				private void CopyContentFrom(SubscriptionCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class Subscription 
			{

				public Subscription()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "Subscription";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Subscription));
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

				public static Subscription DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Subscription));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Subscription) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(Subscription sourceObject)
				{
					Plan = sourceObject.Plan;
					PaymentOption = sourceObject.PaymentOption;
					Created = sourceObject.Created;
					ValidFrom = sourceObject.ValidFrom;
					ValidTo = sourceObject.ValidTo;
				}
				



			[DataMember] 
			public string Plan { get; set; }
			private string _unmodified_Plan;
			[DataMember] 
			public string PaymentOption { get; set; }
			private string _unmodified_PaymentOption;
			[DataMember] 
			public DateTime Created { get; set; }
			private DateTime _unmodified_Created;
			[DataMember] 
			public DateTime ValidFrom { get; set; }
			private DateTime _unmodified_ValidFrom;
			[DataMember] 
			public DateTime ValidTo { get; set; }
			private DateTime _unmodified_ValidTo;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class TenantGymCollection 
			{

				public TenantGymCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "TenantGymCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TenantGymCollection));
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

				public static TenantGymCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TenantGymCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TenantGymCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<TenantGym> CollectionContent = new List<TenantGym>();
				private TenantGym[] _unmodified_CollectionContent;

				[DataMember] public bool IsCollectionFiltered;
				private bool _unmodified_IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();
				private string[] _unmodified_OrderFilterIDList;

				public string SelectedIDCommaSeparated
				{
					get
					{
						string[] sourceArray;
						if (OrderFilterIDList != null)
							sourceArray = OrderFilterIDList.ToArray();
						else
							sourceArray = CollectionContent.Select(item => item.ID).ToArray();
						return String.Join(",", sourceArray);
					}
					set 
					{
						if (value == null)
							return;
						string[] valueArray = value.Split(',');
						OrderFilterIDList = new List<string>();
						OrderFilterIDList.AddRange(valueArray);
						OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
					}
				}

				public TenantGym[] GetIDSelectedArray()
				{
					if (IsCollectionFiltered == false || this.OrderFilterIDList == null)
						return CollectionContent.ToArray();
					return
						this.OrderFilterIDList.Select(id => CollectionContent.FirstOrDefault(item => item.ID == id)).Where(item => item != null).ToArray();
				}

				public void RefreshOrderAndFilterListFromContent()
                {
                    if (OrderFilterIDList == null)
                        return;
                    OrderFilterIDList.RemoveAll(item => CollectionContent.Any(colItem => colItem.ID == item) == false);
                }

				private void CopyContentFrom(TenantGymCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
			[Serializable]
			public partial class TenantGym 
			{

				public TenantGym()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "TenantGym";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TenantGym));
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

				public static TenantGym DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TenantGym));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TenantGym) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(TenantGym sourceObject)
				{
					GymName = sourceObject.GymName;
					Email = sourceObject.Email;
					PhoneNumber = sourceObject.PhoneNumber;
					Address = sourceObject.Address;
					Address2 = sourceObject.Address2;
					ZipCode = sourceObject.ZipCode;
					PostOffice = sourceObject.PostOffice;
					Country = sourceObject.Country;
				}
				



			[DataMember] 
			public string GymName { get; set; }
			private string _unmodified_GymName;
			[DataMember] 
			public string Email { get; set; }
			private string _unmodified_Email;
			[DataMember] 
			public string PhoneNumber { get; set; }
			private string _unmodified_PhoneNumber;
			[DataMember] 
			public string Address { get; set; }
			private string _unmodified_Address;
			[DataMember] 
			public string Address2 { get; set; }
			private string _unmodified_Address2;
			[DataMember] 
			public string ZipCode { get; set; }
			private string _unmodified_ZipCode;
			[DataMember] 
			public string PostOffice { get; set; }
			private string _unmodified_PostOffice;
			[DataMember] 
			public string Country { get; set; }
			private string _unmodified_Country;
			
			}
	
	#region Operation Calls
	public partial class Server 
	{
	    public delegate Task ExecuteOperationFunc(string operationName, object parameters = null);

	    public static ExecuteOperationFunc ExecuteOperation;

	    public delegate Task<object> GetObjectFunc(Type type, string id);

	    public static GetObjectFunc GetInformationObjectImplementation;
	    public static GetObjectFunc GetInterfaceObjectImplementation;


        private static async Task<T> GetInformationObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInformationObjectImplementation(type, id);
	        return (T) objResult;
	    }

	    private static async Task<T> GetInterfaceObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInterfaceObjectImplementation(type, id);
	        return (T)objResult;
	    }


		public static async Task SyncPlansAndPaymentOptionsFromStripe() 
		{
			await ExecuteOperation("ProBroz.OnlineTraining.SyncPlansAndPaymentOptionsFromStripe");
		}

		public static async Task GetOrInitiateDefaultGym() 
		{
			await ExecuteOperation("ProBroz.OnlineTraining.GetOrInitiateDefaultGym");
		}

		public static async Task CreateMember(INT.Member param) 
		{
			await ExecuteOperation("ProBroz.OnlineTraining.CreateMember", param);
		}

		public static async Task SaveMember(INT.Member param) 
		{
			await ExecuteOperation("ProBroz.OnlineTraining.SaveMember", param);
		}

		public static async Task DeleteMember(INT.Member param) 
		{
			await ExecuteOperation("ProBroz.OnlineTraining.DeleteMember", param);
		}
		public static async Task<MemberCollection> GetMemberCollection(string id = null)
		{
			var result = await GetInformationObject<MemberCollection>(id);
			return result;
		}
		public static async Task<Member> GetMember(string id = null)
		{
			var result = await GetInformationObject<Member>(id);
			return result;
		}
		public static async Task<MembershipPlanCollection> GetMembershipPlanCollection(string id = null)
		{
			var result = await GetInformationObject<MembershipPlanCollection>(id);
			return result;
		}
		public static async Task<MembershipPlan> GetMembershipPlan(string id = null)
		{
			var result = await GetInformationObject<MembershipPlan>(id);
			return result;
		}
		public static async Task<PaymentOptionCollection> GetPaymentOptionCollection(string id = null)
		{
			var result = await GetInformationObject<PaymentOptionCollection>(id);
			return result;
		}
		public static async Task<PaymentOption> GetPaymentOption(string id = null)
		{
			var result = await GetInformationObject<PaymentOption>(id);
			return result;
		}
		public static async Task<SubscriptionCollection> GetSubscriptionCollection(string id = null)
		{
			var result = await GetInformationObject<SubscriptionCollection>(id);
			return result;
		}
		public static async Task<Subscription> GetSubscription(string id = null)
		{
			var result = await GetInformationObject<Subscription>(id);
			return result;
		}
		public static async Task<TenantGymCollection> GetTenantGymCollection(string id = null)
		{
			var result = await GetInformationObject<TenantGymCollection>(id);
			return result;
		}
		public static async Task<TenantGym> GetTenantGym(string id = null)
		{
			var result = await GetInformationObject<TenantGym>(id);
			return result;
		}
		public static async Task<INT.Member> GetMember(string id = null)
		{
			var result = await GetInterfaceObject<INT.Member>(id);
			return result;
		}
	}
#endregion
 } 