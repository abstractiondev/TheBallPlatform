 


using DOM=TheBall.Interface;


namespace TheBall.Interface { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;
using TheBall;
using TheBall.CORE;



namespace INT { 
					[DataContract]
			public partial class ShareCollabParams
			{
				[DataMember]
				public CollaborationPartner Partner { get; set; }
				[DataMember]
				public string FileName { get; set; }
			}

			[DataContract]
			public partial class CollaborationPartner
			{
				[DataMember]
				public string PartnerType { get; set; }
				[DataMember]
				public string PartnerID { get; set; }
			}

			[DataContract]
			public partial class InterfaceJSONData
			{
				[DataMember]
				public string Name { get; set; }
				[DataMember]
				public System.Dynamic.ExpandoObject Data { get; set; }
			}

			[DataContract]
			public partial class CollaborationPartnerSummary
			{
				[DataMember]
				public PartnerSummaryItem[] PartnerData { get; set; }
			}

			[DataContract]
			public partial class PartnerSummaryItem
			{
				[DataMember]
				public CollaborationPartner Partner { get; set; }
				[DataMember]
				public string ShareInfoSummaryMD5 { get; set; }
			}

			[DataContract]
			public partial class ShareInfoSummary
			{
				[DataMember]
				public ShareInfo[] SharedByMe { get; set; }
				[DataMember]
				public ShareInfo[] SharedForMe { get; set; }
			}

			[DataContract]
			public partial class ShareInfo
			{
				[DataMember]
				public string ItemName { get; set; }
				[DataMember]
				public string ContentMD5 { get; set; }
				[DataMember]
				public DateTime Modified { get; set; }
				[DataMember]
				public double Length { get; set; }
			}

			[DataContract]
			public partial class ConnectionCommunicationData
			{
				[DataMember]
				public string ActiveSideConnectionID { get; set; }
				[DataMember]
				public string ReceivingSideConnectionID { get; set; }
				[DataMember]
				public string ProcessRequest { get; set; }
				[DataMember]
				public string ProcessParametersString { get; set; }
				[DataMember]
				public string ProcessResultString { get; set; }
				[DataMember]
				public string[] ProcessResultArray { get; set; }
				[DataMember]
				public CategoryInfo[] CategoryCollectionData { get; set; }
				[DataMember]
				public CategoryLinkItem[] LinkItems { get; set; }
			}

			[DataContract]
			public partial class CategoryInfo
			{
				[DataMember]
				public string CategoryID { get; set; }
				[DataMember]
				public string NativeCategoryID { get; set; }
				[DataMember]
				public string NativeCategoryDomainName { get; set; }
				[DataMember]
				public string NativeCategoryObjectName { get; set; }
				[DataMember]
				public string NativeCategoryTitle { get; set; }
				[DataMember]
				public string IdentifyingCategoryName { get; set; }
				[DataMember]
				public string ParentCategoryID { get; set; }
			}

			[DataContract]
			public partial class CategoryLinkParameters
			{
				[DataMember]
				public string ConnectionID { get; set; }
				[DataMember]
				public CategoryLinkItem[] LinkItems { get; set; }
			}

			[DataContract]
			public partial class CategoryLinkItem
			{
				[DataMember]
				public string SourceCategoryID { get; set; }
				[DataMember]
				public string TargetCategoryID { get; set; }
				[DataMember]
				public string LinkingType { get; set; }
			}

			[DataContract]
			public partial class AccountMembershipData
			{
				[DataMember]
				public AccountMembershipItem[] Memberships { get; set; }
			}

			[DataContract]
			public partial class AccountDetails
			{
				[DataMember]
				public string EmailAddress { get; set; }
			}

			[DataContract]
			public partial class AccountMembershipItem
			{
				[DataMember]
				public string GroupID { get; set; }
				[DataMember]
				public string Role { get; set; }
				[DataMember]
				public GroupDetails Details { get; set; }
			}

			[DataContract]
			public partial class GroupMembershipData
			{
				[DataMember]
				public GroupMembershipItem[] Memberships { get; set; }
			}

			[DataContract]
			public partial class GroupDetails
			{
				[DataMember]
				public string GroupName { get; set; }
			}

			[DataContract]
			public partial class GroupMembershipItem
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string Role { get; set; }
				[DataMember]
				public AccountDetails Details { get; set; }
			}

			[DataContract]
			public partial class EmailPackage
			{
				[DataMember]
				public string[] RecipientAccountIDs { get; set; }
				[DataMember]
				public string Subject { get; set; }
				[DataMember]
				public string BodyText { get; set; }
				[DataMember]
				public string BodyHtml { get; set; }
				[DataMember]
				public EmailAttachment[] Attachments { get; set; }
			}

			[DataContract]
			public partial class EmailAttachment
			{
				[DataMember]
				public string FileName { get; set; }
				[DataMember]
				public string InterfaceDataName { get; set; }
				[DataMember]
				public string TextDataContent { get; set; }
				[DataMember]
				public string Base64Content { get; set; }
			}

 } 			[DataContract] 
			[Serializable]
			public partial class InterfaceOperation 
			{
				public InterfaceOperation() 
				{
					Name = "InterfaceOperation";
					SemanticDomainName = "TheBall.Interface";
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
			public string OperationName { get; set; }
			[DataMember] 
			public string Status { get; set; }
			[DataMember] 
			public string OperationDataType { get; set; }
			[DataMember] 
			public DateTime Created { get; set; }
			[DataMember] 
			public DateTime Started { get; set; }
			[DataMember] 
			public double Progress { get; set; }
			[DataMember] 
			public DateTime Finished { get; set; }
			[DataMember] 
			public string ErrorCode { get; set; }
			[DataMember] 
			public string ErrorMessage { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ConnectionCollection 
			{
				public ConnectionCollection() 
				{
					Name = "ConnectionCollection";
					SemanticDomainName = "TheBall.Interface";
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


				
				[DataMember] public List<Connection> CollectionContent = new List<Connection>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Connection 
			{
				public Connection() 
				{
					Name = "Connection";
					SemanticDomainName = "TheBall.Interface";
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
			public string OutputInformationID { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public string DeviceID { get; set; }
			[DataMember] 
			public bool IsActiveParty { get; set; }
			[DataMember] 
			public string OtherSideConnectionID { get; set; }
			[DataMember] 
			public List< Category > ThisSideCategories = new List< Category >();
			[DataMember] 
			public List< Category > OtherSideCategories = new List< Category >();
			[DataMember] 
			public List< CategoryLink > CategoryLinks = new List< CategoryLink >();
			[DataMember] 
			public List< TransferPackage > IncomingPackages = new List< TransferPackage >();
			[DataMember] 
			public List< TransferPackage > OutgoingPackages = new List< TransferPackage >();
			[DataMember] 
			public string OperationNameToListPackageContents { get; set; }
			[DataMember] 
			public string OperationNameToProcessReceived { get; set; }
			[DataMember] 
			public string OperationNameToUpdateThisSideCategories { get; set; }
			[DataMember] 
			public string ProcessIDToListPackageContents { get; set; }
			[DataMember] 
			public string ProcessIDToProcessReceived { get; set; }
			[DataMember] 
			public string ProcessIDToUpdateThisSideCategories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TransferPackage 
			{
				public TransferPackage() 
				{
					Name = "TransferPackage";
					SemanticDomainName = "TheBall.Interface";
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
			public string ConnectionID { get; set; }
			[DataMember] 
			public string PackageDirection { get; set; }
			[DataMember] 
			public string PackageType { get; set; }
			[DataMember] 
			public bool IsProcessed { get; set; }
			[DataMember] 
			public List< string > PackageContentBlobs = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class CategoryLink 
			{
				public CategoryLink() 
				{
					Name = "CategoryLink";
					SemanticDomainName = "TheBall.Interface";
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
			public string SourceCategoryID { get; set; }
			[DataMember] 
			public string TargetCategoryID { get; set; }
			[DataMember] 
			public string LinkingType { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Category 
			{
				public Category() 
				{
					Name = "Category";
					SemanticDomainName = "TheBall.Interface";
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
			public string NativeCategoryID { get; set; }
			[DataMember] 
			public string NativeCategoryDomainName { get; set; }
			[DataMember] 
			public string NativeCategoryObjectName { get; set; }
			[DataMember] 
			public string NativeCategoryTitle { get; set; }
			[DataMember] 
			public string IdentifyingCategoryName { get; set; }
			[DataMember] 
			public string ParentCategoryID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class StatusSummary 
			{
				public StatusSummary() 
				{
					Name = "StatusSummary";
					SemanticDomainName = "TheBall.Interface";
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
			public List< OperationExecutionItem > PendingOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< OperationExecutionItem > ExecutingOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< OperationExecutionItem > RecentCompletedOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< string > ChangeItemTrackingList = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class InformationChangeItem 
			{
				public InformationChangeItem() 
				{
					Name = "InformationChangeItem";
					SemanticDomainName = "TheBall.Interface";
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
			public DateTime StartTimeUTC { get; set; }
			[DataMember] 
			public DateTime EndTimeUTC { get; set; }
			[DataMember] 
			public List< string > ChangedObjectIDList = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class OperationExecutionItem 
			{
				public OperationExecutionItem() 
				{
					Name = "OperationExecutionItem";
					SemanticDomainName = "TheBall.Interface";
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
			public string OperationName { get; set; }
			[DataMember] 
			public string OperationDomain { get; set; }
			[DataMember] 
			public string OperationID { get; set; }
			[DataMember] 
			public string CallerProvidedInfo { get; set; }
			[DataMember] 
			public DateTime CreationTime { get; set; }
			[DataMember] 
			public DateTime ExecutionBeginTime { get; set; }
			[DataMember] 
			public DateTime ExecutionCompletedTime { get; set; }
			[DataMember] 
			public string ExecutionStatus { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GenericObjectCollection 
			{
				public GenericObjectCollection() 
				{
					Name = "GenericObjectCollection";
					SemanticDomainName = "TheBall.Interface";
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


				
				[DataMember] public List<GenericCollectionableObject> CollectionContent = new List<GenericCollectionableObject>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class GenericCollectionableObject 
			{
				public GenericCollectionableObject() 
				{
					Name = "GenericCollectionableObject";
					SemanticDomainName = "TheBall.Interface";
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
			public GenericObject ValueObject { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GenericObject 
			{
				public GenericObject() 
				{
					Name = "GenericObject";
					SemanticDomainName = "TheBall.Interface";
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
			public List< GenericValue > Values = new List< GenericValue >();
			[DataMember] 
			public bool IncludeInCollection { get; set; }
			[DataMember] 
			public string OptionalCollectionName { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GenericValue 
			{
				public GenericValue() 
				{
					Name = "GenericValue";
					SemanticDomainName = "TheBall.Interface";
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
			public string ValueName { get; set; }
			[DataMember] 
			public string String { get; set; }
			[DataMember] 
			public List< string > StringArray = new List< string >();
			[DataMember] 
			public double Number { get; set; }
			[DataMember] 
			public List< double > NumberArray = new List< double >();
			[DataMember] 
			public bool Boolean { get; set; }
			[DataMember] 
			public List< bool > BooleanArray = new List< bool >();
			[DataMember] 
			public DateTime DateTime { get; set; }
			[DataMember] 
			public List< DateTime > DateTimeArray = new List< DateTime >();
			[DataMember] 
			public GenericObject Object { get; set; }
			[DataMember] 
			public List< GenericObject > ObjectArray = new List< GenericObject >();
			[DataMember] 
			public string IndexingInfo { get; set; }
			
			}
 } 