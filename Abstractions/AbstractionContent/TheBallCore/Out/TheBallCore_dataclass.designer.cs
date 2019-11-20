 


using DOM=TheBall.Core;


namespace TheBall.Core { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;

namespace INT { 
					[DataContract]
			public partial class LoginInfo
			{
				[DataMember]
				public string EmailAddress { get; set; }
				[DataMember]
				public string Password { get; set; }
			}

			[DataContract]
			public partial class ConfirmedLoginInfo
			{
				[DataMember]
				public string ConfirmationCode { get; set; }
				[DataMember]
				public LoginInfo LoginInfo { get; set; }
			}

			[DataContract]
			public partial class LoginRegistrationResult
			{
				[DataMember]
				public bool Success { get; set; }
			}

			[DataContract]
			public partial class AccountMetadata
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public System.Dynamic.ExpandoObject Data { get; set; }
			}

			[DataContract]
			public partial class DeviceOperationData
			{
				[DataMember]
				public string OperationRequestString { get; set; }
				[DataMember]
				public string[] OperationParameters { get; set; }
				[DataMember]
				public string[] OperationReturnValues { get; set; }
				[DataMember]
				public bool OperationResult { get; set; }
				[DataMember]
				public ContentItemLocationWithMD5[] OperationSpecificContentData { get; set; }
			}

			[DataContract]
			public partial class ContentItemLocationWithMD5
			{
				[DataMember]
				public string ContentLocation { get; set; }
				[DataMember]
				public string ContentMD5 { get; set; }
				[DataMember]
				public ItemData[] ItemDatas { get; set; }
			}

			[DataContract]
			public partial class ItemData
			{
				[DataMember]
				public string DataName { get; set; }
				[DataMember]
				public string ItemTextData { get; set; }
			}

 } 			[DataContract] [ProtoContract]
			//[Serializable]
			public partial class Login 
			{
				public Login() 
				{
					Name = "Login";
					SemanticDomainName = "TheBall.Core";
				}

				[DataMember] [ProtoMember(2000)]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] [ProtoMember(2001)]
                public string Name { get; set; }

                [DataMember] [ProtoMember(2002)]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] [ProtoMember(1)]
			public string LoginName { get; set; }
			[DataMember] [ProtoMember(2)]
			public string PasswordHash { get; set; }
			[DataMember] [ProtoMember(3)]
			public string PasswordSalt { get; set; }
			[DataMember] [ProtoMember(4)]
			public string Account { get; set; }
			
			}
			[DataContract] [ProtoContract]
			//[Serializable]
			public partial class Email 
			{
				public Email() 
				{
					Name = "Email";
					SemanticDomainName = "TheBall.Core";
				}

				[DataMember] [ProtoMember(2000)]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] [ProtoMember(2001)]
                public string Name { get; set; }

                [DataMember] [ProtoMember(2002)]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] [ProtoMember(1)]
			public string EmailAddress { get; set; }
			[DataMember] [ProtoMember(2)]
			public string Account { get; set; }
			[DataMember] [ProtoMember(3)]
			public bool PendingValidation { get; set; }
			[DataMember] [ProtoMember(4)]
			public string ValidationKey { get; set; }
			[DataMember] [ProtoMember(5)]
			public DateTime ValidationProcessExpiration { get; set; }
			
			}
			[DataContract] [ProtoContract]
			//[Serializable]
			public partial class Account 
			{
				public Account() 
				{
					Name = "Account";
					SemanticDomainName = "TheBall.Core";
				}

				[DataMember] [ProtoMember(2000)]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] [ProtoMember(2001)]
                public string Name { get; set; }

                [DataMember] [ProtoMember(2002)]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] [ProtoMember(1)]
			public List< string > Emails = new List< string >();
			[DataMember] [ProtoMember(2)]
			public List< string > Logins = new List< string >();
			[DataMember] [ProtoMember(3)]
			public List< string > GroupMemberships = new List< string >();
			[DataMember] [ProtoMember(4)]
			public string ServerMetadataJSON { get; set; }
			[DataMember] [ProtoMember(5)]
			public string ClientMetadataJSON { get; set; }
			
			}
			[DataContract] [ProtoContract]
			//[Serializable]
			public partial class Group 
			{
				public Group() 
				{
					Name = "Group";
					SemanticDomainName = "TheBall.Core";
				}

				[DataMember] [ProtoMember(2000)]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] [ProtoMember(2001)]
                public string Name { get; set; }

                [DataMember] [ProtoMember(2002)]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] [ProtoMember(1)]
			public List< string > GroupMemberships = new List< string >();
			
			}
			[DataContract] [ProtoContract]
			//[Serializable]
			public partial class GroupMembership 
			{
				public GroupMembership() 
				{
					Name = "GroupMembership";
					SemanticDomainName = "TheBall.Core";
				}

				[DataMember] [ProtoMember(2000)]
				public string ID { get; set; }

			    [IgnoreDataMember]
                public string ETag { get; set; }

                [DataMember]
                public Guid OwnerID { get; set; }

                [DataMember]
                public string RelativeLocation { get; set; }

                [DataMember] [ProtoMember(2001)]
                public string Name { get; set; }

                [DataMember] [ProtoMember(2002)]
                public string SemanticDomainName { get; set; }

				[DataMember]
				public string MasterETag { get; set; }

				[DataMember]
				public string GeneratedByProcessID { get; set; }


			[DataMember] [ProtoMember(1)]
			public string Account { get; set; }
			[DataMember] [ProtoMember(2)]
			public string Group { get; set; }
			[DataMember] [ProtoMember(3)]
			public string Role { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class ContentPackageCollection 
			{
				public ContentPackageCollection() 
				{
					Name = "ContentPackageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<ContentPackage> CollectionContent = new List<ContentPackage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class ContentPackage 
			{
				public ContentPackage() 
				{
					Name = "ContentPackage";
					SemanticDomainName = "TheBall.Core";
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
			public string PackageType { get; set; }
			[DataMember] 
			public string PackageName { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public string PackageRootFolder { get; set; }
			[DataMember] 
			public DateTime CreationTime { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InformationInputCollection 
			{
				public InformationInputCollection() 
				{
					Name = "InformationInputCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InformationInput> CollectionContent = new List<InformationInput>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InformationInput 
			{
				public InformationInput() 
				{
					Name = "InformationInput";
					SemanticDomainName = "TheBall.Core";
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
			public string InputDescription { get; set; }
			[DataMember] 
			public string LocationURL { get; set; }
			[DataMember] 
			public string LocalContentName { get; set; }
			[DataMember] 
			public string AuthenticatedDeviceID { get; set; }
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InformationOutputCollection 
			{
				public InformationOutputCollection() 
				{
					Name = "InformationOutputCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InformationOutput> CollectionContent = new List<InformationOutput>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InformationOutput 
			{
				public InformationOutput() 
				{
					Name = "InformationOutput";
					SemanticDomainName = "TheBall.Core";
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
			public string OutputDescription { get; set; }
			[DataMember] 
			public string DestinationURL { get; set; }
			[DataMember] 
			public string DestinationContentName { get; set; }
			[DataMember] 
			public string LocalContentURL { get; set; }
			[DataMember] 
			public string AuthenticatedDeviceID { get; set; }
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class AuthenticatedAsActiveDeviceCollection 
			{
				public AuthenticatedAsActiveDeviceCollection() 
				{
					Name = "AuthenticatedAsActiveDeviceCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<AuthenticatedAsActiveDevice> CollectionContent = new List<AuthenticatedAsActiveDevice>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class AuthenticatedAsActiveDevice 
			{
				public AuthenticatedAsActiveDevice() 
				{
					Name = "AuthenticatedAsActiveDevice";
					SemanticDomainName = "TheBall.Core";
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
			public string AuthenticationDescription { get; set; }
			[DataMember] 
			public string SharedSecret { get; set; }
			[DataMember] 
			public byte[] ActiveSymmetricAESKey { get; set; }
			[DataMember] 
			public string EstablishedTrustID { get; set; }
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			[DataMember] 
			public string NegotiationURL { get; set; }
			[DataMember] 
			public string ConnectionURL { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class DeviceMembershipCollection 
			{
				public DeviceMembershipCollection() 
				{
					Name = "DeviceMembershipCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<DeviceMembership> CollectionContent = new List<DeviceMembership>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class DeviceMembership 
			{
				public DeviceMembership() 
				{
					Name = "DeviceMembership";
					SemanticDomainName = "TheBall.Core";
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
			public string DeviceDescription { get; set; }
			[DataMember] 
			public string SharedSecret { get; set; }
			[DataMember] 
			public byte[] ActiveSymmetricAESKey { get; set; }
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceFiscalExportSummary 
			{
				public InvoiceFiscalExportSummary() 
				{
					Name = "InvoiceFiscalExportSummary";
					SemanticDomainName = "TheBall.Core";
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
			public DateTime FiscalInclusiveStartDate { get; set; }
			[DataMember] 
			public DateTime FiscalInclusiveEndDate { get; set; }
			[DataMember] 
			public InvoiceCollection ExportedInvoices { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceSummaryContainer 
			{
				public InvoiceSummaryContainer() 
				{
					Name = "InvoiceSummaryContainer";
					SemanticDomainName = "TheBall.Core";
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
			public InvoiceCollection OpenInvoices { get; set; }
			[DataMember] 
			public InvoiceCollection PredictedInvoices { get; set; }
			[DataMember] 
			public InvoiceCollection PaidInvoicesActiveYear { get; set; }
			[DataMember] 
			public InvoiceCollection PaidInvoicesLast12Months { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceCollection 
			{
				public InvoiceCollection() 
				{
					Name = "InvoiceCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<Invoice> CollectionContent = new List<Invoice>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class Invoice 
			{
				public Invoice() 
				{
					Name = "Invoice";
					SemanticDomainName = "TheBall.Core";
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
			public string InvoiceName { get; set; }
			[DataMember] 
			public string InvoiceID { get; set; }
			[DataMember] 
			public string InvoicedAmount { get; set; }
			[DataMember] 
			public DateTime CreateDate { get; set; }
			[DataMember] 
			public DateTime DueDate { get; set; }
			[DataMember] 
			public string PaidAmount { get; set; }
			[DataMember] 
			public string FeesAndInterestAmount { get; set; }
			[DataMember] 
			public string UnpaidAmount { get; set; }
			[DataMember] 
			public InvoiceDetails InvoiceDetails { get; set; }
			[DataMember] 
			public InvoiceUserCollection InvoiceUsers { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceDetails 
			{
				public InvoiceDetails() 
				{
					Name = "InvoiceDetails";
					SemanticDomainName = "TheBall.Core";
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
			public string MonthlyFeesTotal { get; set; }
			[DataMember] 
			public string OneTimeFeesTotal { get; set; }
			[DataMember] 
			public string UsageFeesTotal { get; set; }
			[DataMember] 
			public string InterestFeesTotal { get; set; }
			[DataMember] 
			public string PenaltyFeesTotal { get; set; }
			[DataMember] 
			public string TotalFeesTotal { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceUserCollection 
			{
				public InvoiceUserCollection() 
				{
					Name = "InvoiceUserCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InvoiceUser> CollectionContent = new List<InvoiceUser>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceUser 
			{
				public InvoiceUser() 
				{
					Name = "InvoiceUser";
					SemanticDomainName = "TheBall.Core";
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
			public string UserName { get; set; }
			[DataMember] 
			public string UserID { get; set; }
			[DataMember] 
			public string UserPhoneNumber { get; set; }
			[DataMember] 
			public string UserSubscriptionNumber { get; set; }
			[DataMember] 
			public string UserInvoiceTotalAmount { get; set; }
			[DataMember] 
			public InvoiceRowGroupCollection InvoiceRowGroupCollection { get; set; }
			[DataMember] 
			public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceRowGroupCollection 
			{
				public InvoiceRowGroupCollection() 
				{
					Name = "InvoiceRowGroupCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InvoiceRowGroup> CollectionContent = new List<InvoiceRowGroup>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceEventDetailGroupCollection 
			{
				public InvoiceEventDetailGroupCollection() 
				{
					Name = "InvoiceEventDetailGroupCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InvoiceEventDetailGroup> CollectionContent = new List<InvoiceEventDetailGroup>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceRowGroup 
			{
				public InvoiceRowGroup() 
				{
					Name = "InvoiceRowGroup";
					SemanticDomainName = "TheBall.Core";
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
			public string GroupName { get; set; }
			[DataMember] 
			public string GroupTotalPriceWithoutTaxes { get; set; }
			[DataMember] 
			public string GroupTotalTaxes { get; set; }
			[DataMember] 
			public string GroupTotalPriceWithTaxes { get; set; }
			[DataMember] 
			public InvoiceRowCollection InvoiceRowCollection { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceEventDetailGroup 
			{
				public InvoiceEventDetailGroup() 
				{
					Name = "InvoiceEventDetailGroup";
					SemanticDomainName = "TheBall.Core";
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
			public string GroupName { get; set; }
			[DataMember] 
			public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceEventDetailCollection 
			{
				public InvoiceEventDetailCollection() 
				{
					Name = "InvoiceEventDetailCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InvoiceEventDetail> CollectionContent = new List<InvoiceEventDetail>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceRowCollection 
			{
				public InvoiceRowCollection() 
				{
					Name = "InvoiceRowCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<InvoiceRow> CollectionContent = new List<InvoiceRow>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceEventDetail 
			{
				public InvoiceEventDetail() 
				{
					Name = "InvoiceEventDetail";
					SemanticDomainName = "TheBall.Core";
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
			public string IndentMode { get; set; }
			[DataMember] 
			public DateTime EventStartDateTime { get; set; }
			[DataMember] 
			public DateTime EventEndDateTime { get; set; }
			[DataMember] 
			public string ReceivingParty { get; set; }
			[DataMember] 
			public string AmountOfUnits { get; set; }
			[DataMember] 
			public string Duration { get; set; }
			[DataMember] 
			public string UnitPrice { get; set; }
			[DataMember] 
			public string PriceWithoutTaxes { get; set; }
			[DataMember] 
			public string Taxes { get; set; }
			[DataMember] 
			public string PriceWithTaxes { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InvoiceRow 
			{
				public InvoiceRow() 
				{
					Name = "InvoiceRow";
					SemanticDomainName = "TheBall.Core";
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
			public string IndentMode { get; set; }
			[DataMember] 
			public string AmountOfUnits { get; set; }
			[DataMember] 
			public string Duration { get; set; }
			[DataMember] 
			public string UnitPrice { get; set; }
			[DataMember] 
			public string PriceWithoutTaxes { get; set; }
			[DataMember] 
			public string Taxes { get; set; }
			[DataMember] 
			public string PriceWithTaxes { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class CategoryCollection 
			{
				public CategoryCollection() 
				{
					Name = "CategoryCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<Category> CollectionContent = new List<Category>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class Category 
			{
				public Category() 
				{
					Name = "Category";
					SemanticDomainName = "TheBall.Core";
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
			public string CategoryName { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class ProcessContainer 
			{
				public ProcessContainer() 
				{
					Name = "ProcessContainer";
					SemanticDomainName = "TheBall.Core";
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
			public List< string > ProcessIDs = new List< string >();
			
			}
			[DataContract] 
			//[Serializable]
			public partial class Process 
			{
				public Process() 
				{
					Name = "Process";
					SemanticDomainName = "TheBall.Core";
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
			public string ProcessDescription { get; set; }
			[DataMember] 
			public SemanticInformationItem ExecutingOperation { get; set; }
			[DataMember] 
			public List< SemanticInformationItem > InitialArguments = new List< SemanticInformationItem >();
			[DataMember] 
			public List< ProcessItem > ProcessItems = new List< ProcessItem >();
			
			}
			[DataContract] 
			//[Serializable]
			public partial class ProcessItem 
			{
				public ProcessItem() 
				{
					Name = "ProcessItem";
					SemanticDomainName = "TheBall.Core";
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
			public List< SemanticInformationItem > Outputs = new List< SemanticInformationItem >();
			[DataMember] 
			public List< SemanticInformationItem > Inputs = new List< SemanticInformationItem >();
			
			}
			[DataContract] 
			//[Serializable]
			public partial class SemanticInformationItem 
			{
				public SemanticInformationItem() 
				{
					Name = "SemanticInformationItem";
					SemanticDomainName = "TheBall.Core";
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
			public string ItemFullType { get; set; }
			[DataMember] 
			public string ItemValue { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class InformationOwnerInfo 
			{
				public InformationOwnerInfo() 
				{
					Name = "InformationOwnerInfo";
					SemanticDomainName = "TheBall.Core";
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
			public string OwnerType { get; set; }
			[DataMember] 
			public string OwnerIdentifier { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class UsageSummary 
			{
				public UsageSummary() 
				{
					Name = "UsageSummary";
					SemanticDomainName = "TheBall.Core";
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
			public string SummaryName { get; set; }
			[DataMember] 
			public UsageMonitorItem SummaryMonitoringItem { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class UsageMonitorItem 
			{
				public UsageMonitorItem() 
				{
					Name = "UsageMonitorItem";
					SemanticDomainName = "TheBall.Core";
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
			public InformationOwnerInfo OwnerInfo { get; set; }
			[DataMember] 
			public TimeRange TimeRangeInclusiveStartExclusiveEnd { get; set; }
			[DataMember] 
			public long StepSizeInMinutes { get; set; }
			[DataMember] 
			public ProcessorUsageCollection ProcessorUsages { get; set; }
			[DataMember] 
			public StorageTransactionUsageCollection StorageTransactionUsages { get; set; }
			[DataMember] 
			public StorageUsageCollection StorageUsages { get; set; }
			[DataMember] 
			public NetworkUsageCollection NetworkUsages { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class RequestResourceUsageCollection 
			{
				public RequestResourceUsageCollection() 
				{
					Name = "RequestResourceUsageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<RequestResourceUsage> CollectionContent = new List<RequestResourceUsage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class RequestResourceUsage 
			{
				public RequestResourceUsage() 
				{
					Name = "RequestResourceUsage";
					SemanticDomainName = "TheBall.Core";
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
			public InformationOwnerInfo OwnerInfo { get; set; }
			[DataMember] 
			public ProcessorUsage ProcessorUsage { get; set; }
			[DataMember] 
			public StorageTransactionUsage StorageTransactionUsage { get; set; }
			[DataMember] 
			public NetworkUsage NetworkUsage { get; set; }
			[DataMember] 
			public HTTPActivityDetails RequestDetails { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class ProcessorUsageCollection 
			{
				public ProcessorUsageCollection() 
				{
					Name = "ProcessorUsageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<ProcessorUsage> CollectionContent = new List<ProcessorUsage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class ProcessorUsage 
			{
				public ProcessorUsage() 
				{
					Name = "ProcessorUsage";
					SemanticDomainName = "TheBall.Core";
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
			public TimeRange TimeRange { get; set; }
			[DataMember] 
			public string UsageType { get; set; }
			[DataMember] 
			public double AmountOfTicks { get; set; }
			[DataMember] 
			public double FrequencyTicksPerSecond { get; set; }
			[DataMember] 
			public long Milliseconds { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class StorageTransactionUsageCollection 
			{
				public StorageTransactionUsageCollection() 
				{
					Name = "StorageTransactionUsageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<StorageTransactionUsage> CollectionContent = new List<StorageTransactionUsage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class StorageTransactionUsage 
			{
				public StorageTransactionUsage() 
				{
					Name = "StorageTransactionUsage";
					SemanticDomainName = "TheBall.Core";
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
			public TimeRange TimeRange { get; set; }
			[DataMember] 
			public string UsageType { get; set; }
			[DataMember] 
			public long AmountOfTransactions { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class StorageUsageCollection 
			{
				public StorageUsageCollection() 
				{
					Name = "StorageUsageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<StorageUsage> CollectionContent = new List<StorageUsage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class StorageUsage 
			{
				public StorageUsage() 
				{
					Name = "StorageUsage";
					SemanticDomainName = "TheBall.Core";
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
			public DateTime SnapshotTime { get; set; }
			[DataMember] 
			public string UsageType { get; set; }
			[DataMember] 
			public string UsageUnit { get; set; }
			[DataMember] 
			public double AmountOfUnits { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class NetworkUsageCollection 
			{
				public NetworkUsageCollection() 
				{
					Name = "NetworkUsageCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<NetworkUsage> CollectionContent = new List<NetworkUsage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class NetworkUsage 
			{
				public NetworkUsage() 
				{
					Name = "NetworkUsage";
					SemanticDomainName = "TheBall.Core";
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
			public TimeRange TimeRange { get; set; }
			[DataMember] 
			public string UsageType { get; set; }
			[DataMember] 
			public long AmountOfBytes { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class TimeRange 
			{
				public TimeRange() 
				{
					Name = "TimeRange";
					SemanticDomainName = "TheBall.Core";
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
			public DateTime StartTime { get; set; }
			[DataMember] 
			public DateTime EndTime { get; set; }
			
			}
			[DataContract] 
			//[Serializable]
			public partial class HTTPActivityDetailsCollection 
			{
				public HTTPActivityDetailsCollection() 
				{
					Name = "HTTPActivityDetailsCollection";
					SemanticDomainName = "TheBall.Core";
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


				
				[DataMember] public List<HTTPActivityDetails> CollectionContent = new List<HTTPActivityDetails>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			//[Serializable]
			public partial class HTTPActivityDetails 
			{
				public HTTPActivityDetails() 
				{
					Name = "HTTPActivityDetails";
					SemanticDomainName = "TheBall.Core";
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
			public string RemoteIPAddress { get; set; }
			[DataMember] 
			public string RemoteEndpointUserName { get; set; }
			[DataMember] 
			public string UserID { get; set; }
			[DataMember] 
			public DateTime UTCDateTime { get; set; }
			[DataMember] 
			public string RequestLine { get; set; }
			[DataMember] 
			public long HTTPStatusCode { get; set; }
			[DataMember] 
			public long ReturnedContentLength { get; set; }
			
			}
 } 