 

namespace SER.TheBall.Payments { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments.INT")]
			public partial class StripeWebhookData
			{
				[DataMember]
				public string id { get; set; }
				[DataMember]
				public bool livemode { get; set; }
				[DataMember]
				public string type { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments.INT")]
			public partial class PaymentToken
			{
				[DataMember]
				public string id { get; set; }
				[DataMember]
				public string currentproduct { get; set; }
				[DataMember]
				public string email { get; set; }
				[DataMember]
				public BillingAddress card { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments.INT")]
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

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments")]
			[Serializable]
			public partial class GroupSubscriptionPlanCollection 
			{

				public GroupSubscriptionPlanCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Payments";
				    this.Name = "GroupSubscriptionPlanCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSubscriptionPlanCollection));
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

				public static GroupSubscriptionPlanCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSubscriptionPlanCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupSubscriptionPlanCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<GroupSubscriptionPlan> CollectionContent = new List<GroupSubscriptionPlan>();
				private GroupSubscriptionPlan[] _unmodified_CollectionContent;

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

				public GroupSubscriptionPlan[] GetIDSelectedArray()
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

				private void CopyContentFrom(GroupSubscriptionPlanCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments")]
			[Serializable]
			public partial class GroupSubscriptionPlan 
			{

				public GroupSubscriptionPlan()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Payments";
				    this.Name = "GroupSubscriptionPlan";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSubscriptionPlan));
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

				public static GroupSubscriptionPlan DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupSubscriptionPlan));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupSubscriptionPlan) serializer.ReadObject(xmlReader);
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
			public List< string > GroupIDs = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments")]
			[Serializable]
			public partial class SubscriptionPlanStatus 
			{

				public SubscriptionPlanStatus()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Payments";
				    this.Name = "SubscriptionPlanStatus";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionPlanStatus));
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

				public static SubscriptionPlanStatus DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionPlanStatus));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SubscriptionPlanStatus) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(SubscriptionPlanStatus sourceObject)
				{
					SubscriptionPlan = sourceObject.SubscriptionPlan;
					ValidUntil = sourceObject.ValidUntil;
				}
				



			[DataMember]
			public string SubscriptionPlan { get; set; }
			private string _unmodified_SubscriptionPlan;
			[DataMember]
			public DateTime ValidUntil { get; set; }
			private DateTime _unmodified_ValidUntil;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments")]
			[Serializable]
			public partial class CustomerAccountCollection 
			{

				public CustomerAccountCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Payments";
				    this.Name = "CustomerAccountCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerAccountCollection));
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

				public static CustomerAccountCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerAccountCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CustomerAccountCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<CustomerAccount> CollectionContent = new List<CustomerAccount>();
				private CustomerAccount[] _unmodified_CollectionContent;

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

				public CustomerAccount[] GetIDSelectedArray()
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

				private void CopyContentFrom(CustomerAccountCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Payments")]
			[Serializable]
			public partial class CustomerAccount 
			{

				public CustomerAccount()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Payments";
				    this.Name = "CustomerAccount";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerAccount));
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

				public static CustomerAccount DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerAccount));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CustomerAccount) serializer.ReadObject(xmlReader);
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
			public string StripeID { get; set; }
			private string _unmodified_StripeID;
			[DataMember]
			public string EmailAddress { get; set; }
			private string _unmodified_EmailAddress;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember]
			public List< string > ActivePlans = new List< string >();
			
			}
 } 