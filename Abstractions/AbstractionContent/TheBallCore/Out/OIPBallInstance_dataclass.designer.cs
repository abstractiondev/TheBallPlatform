 


using DOM=AaltoGlobalImpact.OIP;


namespace AaltoGlobalImpact.OIP { 
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
			public partial class CategoryChildRanking
			{
				[DataMember]
				public string CategoryID { get; set; }
				[DataMember]
				public RankingItem[] RankingItems { get; set; }
			}

			[DataContract]
			public partial class RankingItem
			{
				[DataMember]
				public string ContentID { get; set; }
				[DataMember]
				public string ContentSemanticType { get; set; }
				[DataMember]
				public string RankName { get; set; }
				[DataMember]
				public string RankValue { get; set; }
			}

			[DataContract]
			public partial class ParentToChildren
			{
				[DataMember]
				public string id { get; set; }
				[DataMember]
				public ParentToChildren[] children { get; set; }
			}

 } 			[DataContract] 
			[Serializable]
			public partial class TBSystem 
			{
				public TBSystem() 
				{
					Name = "TBSystem";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string InstanceName { get; set; }
			[DataMember] 
			public string AdminGroupID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class WebPublishInfo 
			{
				public WebPublishInfo() 
				{
					Name = "WebPublishInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string PublishType { get; set; }
			[DataMember] 
			public string PublishContainer { get; set; }
			[DataMember] 
			public PublicationPackage ActivePublication { get; set; }
			[DataMember] 
			public PublicationPackageCollection Publications { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class PublicationPackageCollection 
			{
				public PublicationPackageCollection() 
				{
					Name = "PublicationPackageCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<PublicationPackage> CollectionContent = new List<PublicationPackage>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class PublicationPackage 
			{
				public PublicationPackage() 
				{
					Name = "PublicationPackage";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string PackageName { get; set; }
			[DataMember] 
			public DateTime PublicationTime { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBRLoginRoot 
			{
				public TBRLoginRoot() 
				{
					Name = "TBRLoginRoot";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string DomainName { get; set; }
			[DataMember] 
			public TBAccount Account { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBRAccountRoot 
			{
				public TBRAccountRoot() 
				{
					Name = "TBRAccountRoot";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBAccount Account { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBRGroupRoot 
			{
				public TBRGroupRoot() 
				{
					Name = "TBRGroupRoot";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBCollaboratingGroup Group { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBRLoginGroupRoot 
			{
				public TBRLoginGroupRoot() 
				{
					Name = "TBRLoginGroupRoot";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Role { get; set; }
			[DataMember] 
			public string GroupID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBREmailRoot 
			{
				public TBREmailRoot() 
				{
					Name = "TBREmailRoot";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBAccount Account { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBAccount 
			{
				public TBAccount() 
				{
					Name = "TBAccount";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBEmailCollection Emails { get; set; }
			[DataMember] 
			public TBLoginInfoCollection Logins { get; set; }
			[DataMember] 
			public TBAccountCollaborationGroupCollection GroupRoleCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBAccountCollaborationGroup 
			{
				public TBAccountCollaborationGroup() 
				{
					Name = "TBAccountCollaborationGroup";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string GroupID { get; set; }
			[DataMember] 
			public string GroupRole { get; set; }
			[DataMember] 
			public string RoleStatus { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBAccountCollaborationGroupCollection 
			{
				public TBAccountCollaborationGroupCollection() 
				{
					Name = "TBAccountCollaborationGroupCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<TBAccountCollaborationGroup> CollectionContent = new List<TBAccountCollaborationGroup>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TBLoginInfo 
			{
				public TBLoginInfo() 
				{
					Name = "TBLoginInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string OpenIDUrl { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBLoginInfoCollection 
			{
				public TBLoginInfoCollection() 
				{
					Name = "TBLoginInfoCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<TBLoginInfo> CollectionContent = new List<TBLoginInfo>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TBEmail 
			{
				public TBEmail() 
				{
					Name = "TBEmail";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string EmailAddress { get; set; }
			[DataMember] 
			public DateTime ValidatedAt { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBEmailCollection 
			{
				public TBEmailCollection() 
				{
					Name = "TBEmailCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<TBEmail> CollectionContent = new List<TBEmail>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TBCollaboratorRole 
			{
				public TBCollaboratorRole() 
				{
					Name = "TBCollaboratorRole";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBEmail Email { get; set; }
			[DataMember] 
			public string Role { get; set; }
			[DataMember] 
			public string RoleStatus { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBCollaboratorRoleCollection 
			{
				public TBCollaboratorRoleCollection() 
				{
					Name = "TBCollaboratorRoleCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<TBCollaboratorRole> CollectionContent = new List<TBCollaboratorRole>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TBCollaboratingGroup 
			{
				public TBCollaboratingGroup() 
				{
					Name = "TBCollaboratingGroup";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Title { get; set; }
			[DataMember] 
			public TBCollaboratorRoleCollection Roles { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBEmailValidation 
			{
				public TBEmailValidation() 
				{
					Name = "TBEmailValidation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Email { get; set; }
			[DataMember] 
			public string AccountID { get; set; }
			[DataMember] 
			public DateTime ValidUntil { get; set; }
			[DataMember] 
			public TBGroupJoinConfirmation GroupJoinConfirmation { get; set; }
			[DataMember] 
			public TBDeviceJoinConfirmation DeviceJoinConfirmation { get; set; }
			[DataMember] 
			public TBInformationInputConfirmation InformationInputConfirmation { get; set; }
			[DataMember] 
			public TBInformationOutputConfirmation InformationOutputConfirmation { get; set; }
			[DataMember] 
			public TBMergeAccountConfirmation MergeAccountsConfirmation { get; set; }
			[DataMember] 
			public string RedirectUrlAfterValidation { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBMergeAccountConfirmation 
			{
				public TBMergeAccountConfirmation() 
				{
					Name = "TBMergeAccountConfirmation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string AccountToBeMergedID { get; set; }
			[DataMember] 
			public string AccountToMergeToID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBGroupJoinConfirmation 
			{
				public TBGroupJoinConfirmation() 
				{
					Name = "TBGroupJoinConfirmation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string GroupID { get; set; }
			[DataMember] 
			public string InvitationMode { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBDeviceJoinConfirmation 
			{
				public TBDeviceJoinConfirmation() 
				{
					Name = "TBDeviceJoinConfirmation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string GroupID { get; set; }
			[DataMember] 
			public string AccountID { get; set; }
			[DataMember] 
			public string DeviceMembershipID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBInformationInputConfirmation 
			{
				public TBInformationInputConfirmation() 
				{
					Name = "TBInformationInputConfirmation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string GroupID { get; set; }
			[DataMember] 
			public string AccountID { get; set; }
			[DataMember] 
			public string InformationInputID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TBInformationOutputConfirmation 
			{
				public TBInformationOutputConfirmation() 
				{
					Name = "TBInformationOutputConfirmation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string GroupID { get; set; }
			[DataMember] 
			public string AccountID { get; set; }
			[DataMember] 
			public string InformationOutputID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LoginProvider 
			{
				public LoginProvider() 
				{
					Name = "LoginProvider";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ProviderName { get; set; }
			[DataMember] 
			public string ProviderIconClass { get; set; }
			[DataMember] 
			public string ProviderType { get; set; }
			[DataMember] 
			public string ProviderUrl { get; set; }
			[DataMember] 
			public string ReturnUrl { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LoginProviderCollection 
			{
				public LoginProviderCollection() 
				{
					Name = "LoginProviderCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<LoginProvider> CollectionContent = new List<LoginProvider>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TBPRegisterEmail 
			{
				public TBPRegisterEmail() 
				{
					Name = "TBPRegisterEmail";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string EmailAddress { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountSummary 
			{
				public AccountSummary() 
				{
					Name = "AccountSummary";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public GroupSummaryContainer GroupSummary { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountContainer 
			{
				public AccountContainer() 
				{
					Name = "AccountContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public AccountModule AccountModule { get; set; }
			[DataMember] 
			public AccountSummary AccountSummary { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountModule 
			{
				public AccountModule() 
				{
					Name = "AccountModule";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public AccountProfile Profile { get; set; }
			[DataMember] 
			public AccountSecurity Security { get; set; }
			[DataMember] 
			public AccountRoles Roles { get; set; }
			[DataMember] 
			public AddressAndLocationCollection LocationCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LocationContainer 
			{
				public LocationContainer() 
				{
					Name = "LocationContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public AddressAndLocationCollection Locations { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddressAndLocationCollection 
			{
				public AddressAndLocationCollection() 
				{
					Name = "AddressAndLocationCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<AddressAndLocation> CollectionContent = new List<AddressAndLocation>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class AddressAndLocation 
			{
				public AddressAndLocation() 
				{
					Name = "AddressAndLocation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public ReferenceToInformation ReferenceToInformation { get; set; }
			[DataMember] 
			public StreetAddress Address { get; set; }
			[DataMember] 
			public Location Location { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class StreetAddress 
			{
				public StreetAddress() 
				{
					Name = "StreetAddress";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Street { get; set; }
			[DataMember] 
			public string ZipCode { get; set; }
			[DataMember] 
			public string Town { get; set; }
			[DataMember] 
			public string Country { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountProfile 
			{
				public AccountProfile() 
				{
					Name = "AccountProfile";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public Image ProfileImage { get; set; }
			[DataMember] 
			public string FirstName { get; set; }
			[DataMember] 
			public string LastName { get; set; }
			[DataMember] 
			public StreetAddress Address { get; set; }
			[DataMember] 
			public bool IsSimplifiedAccount { get; set; }
			[DataMember] 
			public string SimplifiedAccountEmail { get; set; }
			[DataMember] 
			public string SimplifiedAccountGroupID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountSecurity 
			{
				public AccountSecurity() 
				{
					Name = "AccountSecurity";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public TBLoginInfoCollection LoginInfoCollection { get; set; }
			[DataMember] 
			public TBEmailCollection EmailCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AccountRoles 
			{
				public AccountRoles() 
				{
					Name = "AccountRoles";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public ReferenceCollection ModeratorInGroups { get; set; }
			[DataMember] 
			public ReferenceCollection MemberInGroups { get; set; }
			[DataMember] 
			public string OrganizationsImPartOf { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class PersonalInfoVisibility 
			{
				public PersonalInfoVisibility() 
				{
					Name = "PersonalInfoVisibility";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string NoOne_Network_All { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ReferenceToInformation 
			{
				public ReferenceToInformation() 
				{
					Name = "ReferenceToInformation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Title { get; set; }
			[DataMember] 
			public string URL { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ReferenceCollection 
			{
				public ReferenceCollection() 
				{
					Name = "ReferenceCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<ReferenceToInformation> CollectionContent = new List<ReferenceToInformation>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class NodeSummaryContainer 
			{
				public NodeSummaryContainer() 
				{
					Name = "NodeSummaryContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public RenderedNodeCollection Nodes { get; set; }
			[DataMember] 
			public TextContentCollection NodeSourceTextContent { get; set; }
			[DataMember] 
			public LinkToContentCollection NodeSourceLinkToContent { get; set; }
			[DataMember] 
			public EmbeddedContentCollection NodeSourceEmbeddedContent { get; set; }
			[DataMember] 
			public ImageCollection NodeSourceImages { get; set; }
			[DataMember] 
			public BinaryFileCollection NodeSourceBinaryFiles { get; set; }
			[DataMember] 
			public CategoryCollection NodeSourceCategories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class RenderedNodeCollection 
			{
				public RenderedNodeCollection() 
				{
					Name = "RenderedNodeCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<RenderedNode> CollectionContent = new List<RenderedNode>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class RenderedNode 
			{
				public RenderedNode() 
				{
					Name = "RenderedNode";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string OriginalContentID { get; set; }
			[DataMember] 
			public string TechnicalSource { get; set; }
			[DataMember] 
			public string ImageBaseUrl { get; set; }
			[DataMember] 
			public string ImageExt { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string OpenNodeTitle { get; set; }
			[DataMember] 
			public string ActualContentUrl { get; set; }
			[DataMember] 
			public string Excerpt { get; set; }
			[DataMember] 
			public string TimestampText { get; set; }
			[DataMember] 
			public string MainSortableText { get; set; }
			[DataMember] 
			public bool IsCategoryFilteringNode { get; set; }
			[DataMember] 
			public ShortTextCollection CategoryFilters { get; set; }
			[DataMember] 
			public ShortTextCollection CategoryNames { get; set; }
			[DataMember] 
			public ShortTextCollection Categories { get; set; }
			[DataMember] 
			public string CategoryIDList { get; set; }
			[DataMember] 
			public ShortTextCollection Authors { get; set; }
			[DataMember] 
			public ShortTextCollection Locations { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ShortTextCollection 
			{
				public ShortTextCollection() 
				{
					Name = "ShortTextCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<ShortTextObject> CollectionContent = new List<ShortTextObject>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class ShortTextObject 
			{
				public ShortTextObject() 
				{
					Name = "ShortTextObject";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Content { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LongTextCollection 
			{
				public LongTextCollection() 
				{
					Name = "LongTextCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<LongTextObject> CollectionContent = new List<LongTextObject>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class LongTextObject 
			{
				public LongTextObject() 
				{
					Name = "LongTextObject";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Content { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class MapMarker 
			{
				public MapMarker() 
				{
					Name = "MapMarker";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string IconUrl { get; set; }
			[DataMember] 
			public string MarkerSource { get; set; }
			[DataMember] 
			public string CategoryName { get; set; }
			[DataMember] 
			public string LocationText { get; set; }
			[DataMember] 
			public string PopupTitle { get; set; }
			[DataMember] 
			public string PopupContent { get; set; }
			[DataMember] 
			public Location Location { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class MapMarkerCollection 
			{
				public MapMarkerCollection() 
				{
					Name = "MapMarkerCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<MapMarker> CollectionContent = new List<MapMarker>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class ModeratorCollection 
			{
				public ModeratorCollection() 
				{
					Name = "ModeratorCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Moderator> CollectionContent = new List<Moderator>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Moderator 
			{
				public Moderator() 
				{
					Name = "Moderator";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ModeratorName { get; set; }
			[DataMember] 
			public string ProfileUrl { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class CollaboratorCollection 
			{
				public CollaboratorCollection() 
				{
					Name = "CollaboratorCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Collaborator> CollectionContent = new List<Collaborator>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Collaborator 
			{
				public Collaborator() 
				{
					Name = "Collaborator";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string AccountID { get; set; }
			[DataMember] 
			public string EmailAddress { get; set; }
			[DataMember] 
			public string CollaboratorName { get; set; }
			[DataMember] 
			public string Role { get; set; }
			[DataMember] 
			public string ProfileUrl { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GroupSummaryContainer 
			{
				public GroupSummaryContainer() 
				{
					Name = "GroupSummaryContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string SummaryBody { get; set; }
			[DataMember] 
			public Introduction Introduction { get; set; }
			[DataMember] 
			public GroupIndex GroupSummaryIndex { get; set; }
			[DataMember] 
			public GroupCollection GroupCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GroupContainer 
			{
				public GroupContainer() 
				{
					Name = "GroupContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public GroupIndex GroupIndex { get; set; }
			[DataMember] 
			public Group GroupProfile { get; set; }
			[DataMember] 
			public CollaboratorCollection Collaborators { get; set; }
			[DataMember] 
			public CollaboratorCollection PendingCollaborators { get; set; }
			[DataMember] 
			public AddressAndLocationCollection LocationCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class GroupIndex 
			{
				public GroupIndex() 
				{
					Name = "GroupIndex";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public Image Icon { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Introduction { get; set; }
			[DataMember] 
			public string Summary { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddAddressAndLocationInfo 
			{
				public AddAddressAndLocationInfo() 
				{
					Name = "AddAddressAndLocationInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string LocationName { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddImageInfo 
			{
				public AddImageInfo() 
				{
					Name = "AddImageInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ImageTitle { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddImageGroupInfo 
			{
				public AddImageGroupInfo() 
				{
					Name = "AddImageGroupInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ImageGroupTitle { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddEmailAddressInfo 
			{
				public AddEmailAddressInfo() 
				{
					Name = "AddEmailAddressInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string EmailAddress { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class CreateGroupInfo 
			{
				public CreateGroupInfo() 
				{
					Name = "CreateGroupInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddActivityInfo 
			{
				public AddActivityInfo() 
				{
					Name = "AddActivityInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ActivityName { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddBlogPostInfo 
			{
				public AddBlogPostInfo() 
				{
					Name = "AddBlogPostInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Title { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AddCategoryInfo 
			{
				public AddCategoryInfo() 
				{
					Name = "AddCategoryInfo";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			[Serializable]
			public partial class GroupCollection 
			{
				public GroupCollection() 
				{
					Name = "GroupCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Group> CollectionContent = new List<Group>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Group 
			{
				public Group() 
				{
					Name = "Group";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public ReferenceToInformation ReferenceToInformation { get; set; }
			[DataMember] 
			public Image ProfileImage { get; set; }
			[DataMember] 
			public Image IconImage { get; set; }
			[DataMember] 
			public string GroupName { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public string OrganizationsAndGroupsLinkedToUs { get; set; }
			[DataMember] 
			public string WwwSiteToPublishTo { get; set; }
			[DataMember] 
			public ShortTextCollection CustomUICollection { get; set; }
			[DataMember] 
			public ModeratorCollection Moderators { get; set; }
			[DataMember] 
			public CategoryCollection CategoryCollection { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Introduction 
			{
				public Introduction() 
				{
					Name = "Introduction";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Title { get; set; }
			[DataMember] 
			public string Body { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ContentCategoryRankCollection 
			{
				public ContentCategoryRankCollection() 
				{
					Name = "ContentCategoryRankCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<ContentCategoryRank> CollectionContent = new List<ContentCategoryRank>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class ContentCategoryRank 
			{
				public ContentCategoryRank() 
				{
					Name = "ContentCategoryRank";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string ContentID { get; set; }
			[DataMember] 
			public string ContentSemanticType { get; set; }
			[DataMember] 
			public string CategoryID { get; set; }
			[DataMember] 
			public string RankName { get; set; }
			[DataMember] 
			public string RankValue { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LinkToContentCollection 
			{
				public LinkToContentCollection() 
				{
					Name = "LinkToContentCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<LinkToContent> CollectionContent = new List<LinkToContent>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class LinkToContent 
			{
				public LinkToContent() 
				{
					Name = "LinkToContent";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string URL { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public DateTime Published { get; set; }
			[DataMember] 
			public string Author { get; set; }
			[DataMember] 
			public MediaContent ImageData { get; set; }
			[DataMember] 
			public AddressAndLocationCollection Locations { get; set; }
			[DataMember] 
			public CategoryCollection Categories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class EmbeddedContentCollection 
			{
				public EmbeddedContentCollection() 
				{
					Name = "EmbeddedContentCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<EmbeddedContent> CollectionContent = new List<EmbeddedContent>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class EmbeddedContent 
			{
				public EmbeddedContent() 
				{
					Name = "EmbeddedContent";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string IFrameTagContents { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public DateTime Published { get; set; }
			[DataMember] 
			public string Author { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public AddressAndLocationCollection Locations { get; set; }
			[DataMember] 
			public CategoryCollection Categories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class DynamicContentGroupCollection 
			{
				public DynamicContentGroupCollection() 
				{
					Name = "DynamicContentGroupCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<DynamicContentGroup> CollectionContent = new List<DynamicContentGroup>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class DynamicContentGroup 
			{
				public DynamicContentGroup() 
				{
					Name = "DynamicContentGroup";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string HostName { get; set; }
			[DataMember] 
			public string GroupHeader { get; set; }
			[DataMember] 
			public string SortValue { get; set; }
			[DataMember] 
			public string PageLocation { get; set; }
			[DataMember] 
			public string ContentItemNames { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class DynamicContentCollection 
			{
				public DynamicContentCollection() 
				{
					Name = "DynamicContentCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<DynamicContent> CollectionContent = new List<DynamicContent>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class DynamicContent 
			{
				public DynamicContent() 
				{
					Name = "DynamicContent";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string HostName { get; set; }
			[DataMember] 
			public string ContentName { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public string ElementQuery { get; set; }
			[DataMember] 
			public string Content { get; set; }
			[DataMember] 
			public string RawContent { get; set; }
			[DataMember] 
			public MediaContent ImageData { get; set; }
			[DataMember] 
			public bool IsEnabled { get; set; }
			[DataMember] 
			public bool ApplyActively { get; set; }
			[DataMember] 
			public string EditType { get; set; }
			[DataMember] 
			public string PageLocation { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class AttachedToObjectCollection 
			{
				public AttachedToObjectCollection() 
				{
					Name = "AttachedToObjectCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<AttachedToObject> CollectionContent = new List<AttachedToObject>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class AttachedToObject 
			{
				public AttachedToObject() 
				{
					Name = "AttachedToObject";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string SourceObjectID { get; set; }
			[DataMember] 
			public string SourceObjectName { get; set; }
			[DataMember] 
			public string SourceObjectDomain { get; set; }
			[DataMember] 
			public string TargetObjectID { get; set; }
			[DataMember] 
			public string TargetObjectName { get; set; }
			[DataMember] 
			public string TargetObjectDomain { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class CommentCollection 
			{
				public CommentCollection() 
				{
					Name = "CommentCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Comment> CollectionContent = new List<Comment>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Comment 
			{
				public Comment() 
				{
					Name = "Comment";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string TargetObjectID { get; set; }
			[DataMember] 
			public string TargetObjectName { get; set; }
			[DataMember] 
			public string TargetObjectDomain { get; set; }
			[DataMember] 
			public string CommentText { get; set; }
			[DataMember] 
			public DateTime Created { get; set; }
			[DataMember] 
			public string OriginalAuthorName { get; set; }
			[DataMember] 
			public string OriginalAuthorEmail { get; set; }
			[DataMember] 
			public string OriginalAuthorAccountID { get; set; }
			[DataMember] 
			public DateTime LastModified { get; set; }
			[DataMember] 
			public string LastAuthorName { get; set; }
			[DataMember] 
			public string LastAuthorEmail { get; set; }
			[DataMember] 
			public string LastAuthorAccountID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class SelectionCollection 
			{
				public SelectionCollection() 
				{
					Name = "SelectionCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Selection> CollectionContent = new List<Selection>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Selection 
			{
				public Selection() 
				{
					Name = "Selection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string TargetObjectID { get; set; }
			[DataMember] 
			public string TargetObjectName { get; set; }
			[DataMember] 
			public string TargetObjectDomain { get; set; }
			[DataMember] 
			public string SelectionCategory { get; set; }
			[DataMember] 
			public string TextValue { get; set; }
			[DataMember] 
			public bool BooleanValue { get; set; }
			[DataMember] 
			public double DoubleValue { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class TextContentCollection 
			{
				public TextContentCollection() 
				{
					Name = "TextContentCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<TextContent> CollectionContent = new List<TextContent>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class TextContent 
			{
				public TextContent() 
				{
					Name = "TextContent";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public MediaContent ImageData { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string OpenArticleTitle { get; set; }
			[DataMember] 
			public string SubTitle { get; set; }
			[DataMember] 
			public DateTime Published { get; set; }
			[DataMember] 
			public string Author { get; set; }
			[DataMember] 
			public MediaContent ArticleImageData { get; set; }
			[DataMember] 
			public string Excerpt { get; set; }
			[DataMember] 
			public string Body { get; set; }
			[DataMember] 
			public AddressAndLocationCollection Locations { get; set; }
			[DataMember] 
			public CategoryCollection Categories { get; set; }
			[DataMember] 
			public double SortOrderNumber { get; set; }
			[DataMember] 
			public string IFrameSources { get; set; }
			[DataMember] 
			public string RawHtmlContent { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Map 
			{
				public Map() 
				{
					Name = "Map";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string Title { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class MapCollection 
			{
				public MapCollection() 
				{
					Name = "MapCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Map> CollectionContent = new List<Map>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class MapResult 
			{
				public MapResult() 
				{
					Name = "MapResult";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public Location Location { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class MapResultCollection 
			{
				public MapResultCollection() 
				{
					Name = "MapResultCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<MapResult> CollectionContent = new List<MapResult>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class MapResultsCollection 
			{
				public MapResultsCollection() 
				{
					Name = "MapResultsCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public MapResultCollection ResultByDate { get; set; }
			[DataMember] 
			public MapResultCollection ResultByAuthor { get; set; }
			[DataMember] 
			public MapResultCollection ResultByProximity { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Video 
			{
				public Video() 
				{
					Name = "Video";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public MediaContent VideoData { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Caption { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class ImageCollection 
			{
				public ImageCollection() 
				{
					Name = "ImageCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Image> CollectionContent = new List<Image>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Image 
			{
				public Image() 
				{
					Name = "Image";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public ReferenceToInformation ReferenceToInformation { get; set; }
			[DataMember] 
			public MediaContent ImageData { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Caption { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public AddressAndLocationCollection Locations { get; set; }
			[DataMember] 
			public CategoryCollection Categories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class BinaryFileCollection 
			{
				public BinaryFileCollection() 
				{
					Name = "BinaryFileCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<BinaryFile> CollectionContent = new List<BinaryFile>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class BinaryFile 
			{
				public BinaryFile() 
				{
					Name = "BinaryFile";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string OriginalFileName { get; set; }
			[DataMember] 
			public MediaContent Data { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Description { get; set; }
			[DataMember] 
			public CategoryCollection Categories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class MediaContent 
			{
				public MediaContent() 
				{
					Name = "MediaContent";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


			
			}
			[DataContract] 
			[Serializable]
			public partial class Longitude 
			{
				public Longitude() 
				{
					Name = "Longitude";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string TextValue { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Latitude 
			{
				public Latitude() 
				{
					Name = "Latitude";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string TextValue { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Location 
			{
				public Location() 
				{
					Name = "Location";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string LocationName { get; set; }
			[DataMember] 
			public Longitude Longitude { get; set; }
			[DataMember] 
			public Latitude Latitude { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class LocationCollection 
			{
				public LocationCollection() 
				{
					Name = "LocationCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<Location> CollectionContent = new List<Location>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
			[DataContract] 
			[Serializable]
			public partial class Date 
			{
				public Date() 
				{
					Name = "Date";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public DateTime Day { get; set; }
			[DataMember] 
			public DateTime Week { get; set; }
			[DataMember] 
			public DateTime Month { get; set; }
			[DataMember] 
			public DateTime Year { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class CategoryContainer 
			{
				public CategoryContainer() 
				{
					Name = "CategoryContainer";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public CategoryCollection Categories { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class Category 
			{
				public Category() 
				{
					Name = "Category";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public ReferenceToInformation ReferenceToInformation { get; set; }
			[DataMember] 
			public string CategoryName { get; set; }
			[DataMember] 
			public MediaContent ImageData { get; set; }
			[DataMember] 
			public string Title { get; set; }
			[DataMember] 
			public string Excerpt { get; set; }
			[DataMember] 
			public Category ParentCategory { get; set; }
			[DataMember] 
			public string ParentCategoryID { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class CategoryCollection 
			{
				public CategoryCollection() 
				{
					Name = "CategoryCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			[Serializable]
			public partial class UpdateWebContentOperation 
			{
				public UpdateWebContentOperation() 
				{
					Name = "UpdateWebContentOperation";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string SourceContainerName { get; set; }
			[DataMember] 
			public string SourcePathRoot { get; set; }
			[DataMember] 
			public string TargetContainerName { get; set; }
			[DataMember] 
			public string TargetPathRoot { get; set; }
			[DataMember] 
			public bool RenderWhileSync { get; set; }
			[DataMember] 
			public UpdateWebContentHandlerCollection Handlers { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class UpdateWebContentHandlerItem 
			{
				public UpdateWebContentHandlerItem() 
				{
					Name = "UpdateWebContentHandlerItem";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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
			public string InformationTypeName { get; set; }
			[DataMember] 
			public string OptionName { get; set; }
			
			}
			[DataContract] 
			[Serializable]
			public partial class UpdateWebContentHandlerCollection 
			{
				public UpdateWebContentHandlerCollection() 
				{
					Name = "UpdateWebContentHandlerCollection";
					SemanticDomainName = "AaltoGlobalImpact.OIP";
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


				
				[DataMember] public List<UpdateWebContentHandlerItem> CollectionContent = new List<UpdateWebContentHandlerItem>();

				[DataMember] public bool IsCollectionFiltered;
				
				[DataMember] public List<string> OrderFilterIDList = new List<string>();


			
			}
 } 