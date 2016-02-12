 

namespace SER.ProBroz.OnlineTraining { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;


namespace INT { 
		 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ProBroz.OnlineTraining")] 
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
					PeriodInDays = sourceObject.PeriodInDays;
					Price = sourceObject.Price;
				}
				



			[DataMember] 
			public string OptionName { get; set; }
			private string _unmodified_OptionName;
			[DataMember] 
			public long PeriodInDays { get; set; }
			private long _unmodified_PeriodInDays;
			[DataMember] 
			public double Price { get; set; }
			private double _unmodified_Price;
			
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
 } 