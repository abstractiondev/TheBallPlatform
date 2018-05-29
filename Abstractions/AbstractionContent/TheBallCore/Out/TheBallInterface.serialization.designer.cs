 

namespace SER.TheBall.Interface { 
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
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class ShareCollabParams
			{
				[DataMember]
				public CollaborationPartner Partner { get; set; }
				[DataMember]
				public string FileName { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class CollaborationPartner
			{
				[DataMember]
				public string PartnerType { get; set; }
				[DataMember]
				public string PartnerID { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class InterfaceJSONData
			{
				[DataMember]
				public string Name { get; set; }
				[DataMember]
				public System.Dynamic.ExpandoObject Data { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class CollaborationPartnerSummary
			{
				[DataMember]
				public PartnerSummaryItem[] PartnerData { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class PartnerSummaryItem
			{
				[DataMember]
				public CollaborationPartner Partner { get; set; }
				[DataMember]
				public string ShareInfoSummaryMD5 { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class ShareInfoSummary
			{
				[DataMember]
				public ShareInfo[] SharedByMe { get; set; }
				[DataMember]
				public ShareInfo[] SharedForMe { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
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

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
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

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
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

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class CategoryLinkParameters
			{
				[DataMember]
				public string ConnectionID { get; set; }
				[DataMember]
				public CategoryLinkItem[] LinkItems { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class CategoryLinkItem
			{
				[DataMember]
				public string SourceCategoryID { get; set; }
				[DataMember]
				public string TargetCategoryID { get; set; }
				[DataMember]
				public string LinkingType { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class AccountMembershipData
			{
				[DataMember]
				public AccountMembershipItem[] Memberships { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class AccountDetails
			{
				[DataMember]
				public string EmailAddress { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class AccountMembershipItem
			{
				[DataMember]
				public string GroupID { get; set; }
				[DataMember]
				public string Role { get; set; }
				[DataMember]
				public GroupDetails Details { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class GroupMembershipData
			{
				[DataMember]
				public GroupMembershipItem[] Memberships { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class GroupDetails
			{
				[DataMember]
				public string GroupName { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
			public partial class GroupMembershipItem
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string Role { get; set; }
				[DataMember]
				public AccountDetails Details { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
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

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface.INT")]
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

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class InterfaceOperation 
			{

				public InterfaceOperation()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "InterfaceOperation";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InterfaceOperation));
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

				public static InterfaceOperation DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InterfaceOperation));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InterfaceOperation) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InterfaceOperation sourceObject)
				{
					OperationName = sourceObject.OperationName;
					Status = sourceObject.Status;
					OperationDataType = sourceObject.OperationDataType;
					Created = sourceObject.Created;
					Started = sourceObject.Started;
					Progress = sourceObject.Progress;
					Finished = sourceObject.Finished;
					ErrorCode = sourceObject.ErrorCode;
					ErrorMessage = sourceObject.ErrorMessage;
				}
				



			[DataMember] 
			public string OperationName { get; set; }
			private string _unmodified_OperationName;
			[DataMember] 
			public string Status { get; set; }
			private string _unmodified_Status;
			[DataMember] 
			public string OperationDataType { get; set; }
			private string _unmodified_OperationDataType;
			[DataMember] 
			public DateTime Created { get; set; }
			private DateTime _unmodified_Created;
			[DataMember] 
			public DateTime Started { get; set; }
			private DateTime _unmodified_Started;
			[DataMember] 
			public double Progress { get; set; }
			private double _unmodified_Progress;
			[DataMember] 
			public DateTime Finished { get; set; }
			private DateTime _unmodified_Finished;
			[DataMember] 
			public string ErrorCode { get; set; }
			private string _unmodified_ErrorCode;
			[DataMember] 
			public string ErrorMessage { get; set; }
			private string _unmodified_ErrorMessage;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class ConnectionCollection 
			{

				public ConnectionCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "ConnectionCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectionCollection));
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

				public static ConnectionCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectionCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ConnectionCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<Connection> CollectionContent = new List<Connection>();
				private Connection[] _unmodified_CollectionContent;

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

				public Connection[] GetIDSelectedArray()
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

				private void CopyContentFrom(ConnectionCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class Connection 
			{

				public Connection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "Connection";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Connection));
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

				public static Connection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Connection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Connection) serializer.ReadObject(xmlReader);
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
			public string OutputInformationID { get; set; }
			private string _unmodified_OutputInformationID;
			[DataMember] 
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember] 
			public string DeviceID { get; set; }
			private string _unmodified_DeviceID;
			[DataMember] 
			public bool IsActiveParty { get; set; }
			private bool _unmodified_IsActiveParty;
			[DataMember] 
			public string OtherSideConnectionID { get; set; }
			private string _unmodified_OtherSideConnectionID;
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
			private string _unmodified_OperationNameToListPackageContents;
			[DataMember] 
			public string OperationNameToProcessReceived { get; set; }
			private string _unmodified_OperationNameToProcessReceived;
			[DataMember] 
			public string OperationNameToUpdateThisSideCategories { get; set; }
			private string _unmodified_OperationNameToUpdateThisSideCategories;
			[DataMember] 
			public string ProcessIDToListPackageContents { get; set; }
			private string _unmodified_ProcessIDToListPackageContents;
			[DataMember] 
			public string ProcessIDToProcessReceived { get; set; }
			private string _unmodified_ProcessIDToProcessReceived;
			[DataMember] 
			public string ProcessIDToUpdateThisSideCategories { get; set; }
			private string _unmodified_ProcessIDToUpdateThisSideCategories;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class TransferPackage 
			{

				public TransferPackage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "TransferPackage";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TransferPackage));
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

				public static TransferPackage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TransferPackage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TransferPackage) serializer.ReadObject(xmlReader);
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
			public string ConnectionID { get; set; }
			private string _unmodified_ConnectionID;
			[DataMember] 
			public string PackageDirection { get; set; }
			private string _unmodified_PackageDirection;
			[DataMember] 
			public string PackageType { get; set; }
			private string _unmodified_PackageType;
			[DataMember] 
			public bool IsProcessed { get; set; }
			private bool _unmodified_IsProcessed;
			[DataMember] 
			public List< string > PackageContentBlobs = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class CategoryLink 
			{

				public CategoryLink()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "CategoryLink";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryLink));
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

				public static CategoryLink DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryLink));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CategoryLink) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(CategoryLink sourceObject)
				{
					SourceCategoryID = sourceObject.SourceCategoryID;
					TargetCategoryID = sourceObject.TargetCategoryID;
					LinkingType = sourceObject.LinkingType;
				}
				



			[DataMember] 
			public string SourceCategoryID { get; set; }
			private string _unmodified_SourceCategoryID;
			[DataMember] 
			public string TargetCategoryID { get; set; }
			private string _unmodified_TargetCategoryID;
			[DataMember] 
			public string LinkingType { get; set; }
			private string _unmodified_LinkingType;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class Category 
			{

				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "Category";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Category));
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

				public static Category DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Category));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Category) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(Category sourceObject)
				{
					NativeCategoryID = sourceObject.NativeCategoryID;
					NativeCategoryDomainName = sourceObject.NativeCategoryDomainName;
					NativeCategoryObjectName = sourceObject.NativeCategoryObjectName;
					NativeCategoryTitle = sourceObject.NativeCategoryTitle;
					IdentifyingCategoryName = sourceObject.IdentifyingCategoryName;
					ParentCategoryID = sourceObject.ParentCategoryID;
				}
				



			[DataMember] 
			public string NativeCategoryID { get; set; }
			private string _unmodified_NativeCategoryID;
			[DataMember] 
			public string NativeCategoryDomainName { get; set; }
			private string _unmodified_NativeCategoryDomainName;
			[DataMember] 
			public string NativeCategoryObjectName { get; set; }
			private string _unmodified_NativeCategoryObjectName;
			[DataMember] 
			public string NativeCategoryTitle { get; set; }
			private string _unmodified_NativeCategoryTitle;
			[DataMember] 
			public string IdentifyingCategoryName { get; set; }
			private string _unmodified_IdentifyingCategoryName;
			[DataMember] 
			public string ParentCategoryID { get; set; }
			private string _unmodified_ParentCategoryID;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class StatusSummary 
			{

				public StatusSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "StatusSummary";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StatusSummary));
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

				public static StatusSummary DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StatusSummary));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StatusSummary) serializer.ReadObject(xmlReader);
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
			public List< OperationExecutionItem > PendingOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< OperationExecutionItem > ExecutingOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< OperationExecutionItem > RecentCompletedOperations = new List< OperationExecutionItem >();
			[DataMember] 
			public List< string > ChangeItemTrackingList = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class InformationChangeItem 
			{

				public InformationChangeItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "InformationChangeItem";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationChangeItem));
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

				public static InformationChangeItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationChangeItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationChangeItem) serializer.ReadObject(xmlReader);
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
			public DateTime StartTimeUTC { get; set; }
			private DateTime _unmodified_StartTimeUTC;
			[DataMember] 
			public DateTime EndTimeUTC { get; set; }
			private DateTime _unmodified_EndTimeUTC;
			[DataMember] 
			public List< string > ChangedObjectIDList = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class OperationExecutionItem 
			{

				public OperationExecutionItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "OperationExecutionItem";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OperationExecutionItem));
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

				public static OperationExecutionItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(OperationExecutionItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (OperationExecutionItem) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(OperationExecutionItem sourceObject)
				{
					OperationName = sourceObject.OperationName;
					OperationDomain = sourceObject.OperationDomain;
					OperationID = sourceObject.OperationID;
					CallerProvidedInfo = sourceObject.CallerProvidedInfo;
					CreationTime = sourceObject.CreationTime;
					ExecutionBeginTime = sourceObject.ExecutionBeginTime;
					ExecutionCompletedTime = sourceObject.ExecutionCompletedTime;
					ExecutionStatus = sourceObject.ExecutionStatus;
				}
				



			[DataMember] 
			public string OperationName { get; set; }
			private string _unmodified_OperationName;
			[DataMember] 
			public string OperationDomain { get; set; }
			private string _unmodified_OperationDomain;
			[DataMember] 
			public string OperationID { get; set; }
			private string _unmodified_OperationID;
			[DataMember] 
			public string CallerProvidedInfo { get; set; }
			private string _unmodified_CallerProvidedInfo;
			[DataMember] 
			public DateTime CreationTime { get; set; }
			private DateTime _unmodified_CreationTime;
			[DataMember] 
			public DateTime ExecutionBeginTime { get; set; }
			private DateTime _unmodified_ExecutionBeginTime;
			[DataMember] 
			public DateTime ExecutionCompletedTime { get; set; }
			private DateTime _unmodified_ExecutionCompletedTime;
			[DataMember] 
			public string ExecutionStatus { get; set; }
			private string _unmodified_ExecutionStatus;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class GenericObjectCollection 
			{

				public GenericObjectCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericObjectCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObjectCollection));
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

				public static GenericObjectCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObjectCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericObjectCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<GenericCollectionableObject> CollectionContent = new List<GenericCollectionableObject>();
				private GenericCollectionableObject[] _unmodified_CollectionContent;

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

				public GenericCollectionableObject[] GetIDSelectedArray()
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

				private void CopyContentFrom(GenericObjectCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class GenericCollectionableObject 
			{

				public GenericCollectionableObject()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericCollectionableObject";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericCollectionableObject));
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

				public static GenericCollectionableObject DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericCollectionableObject));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericCollectionableObject) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(GenericCollectionableObject sourceObject)
				{
					ValueObject = sourceObject.ValueObject;
				}
				



			[DataMember] 
			public GenericObject ValueObject { get; set; }
			private GenericObject _unmodified_ValueObject;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class GenericObject 
			{

				public GenericObject()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericObject";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObject));
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

				public static GenericObject DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericObject));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericObject) serializer.ReadObject(xmlReader);
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
			public List< GenericValue > Values = new List< GenericValue >();
			[DataMember] 
			public bool IncludeInCollection { get; set; }
			private bool _unmodified_IncludeInCollection;
			[DataMember] 
			public string OptionalCollectionName { get; set; }
			private string _unmodified_OptionalCollectionName;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Interface")] 
			[Serializable]
			public partial class GenericValue 
			{

				public GenericValue()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.Interface";
				    this.Name = "GenericValue";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericValue));
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

				public static GenericValue DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GenericValue));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GenericValue) serializer.ReadObject(xmlReader);
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
			public string ValueName { get; set; }
			private string _unmodified_ValueName;
			[DataMember] 
			public string String { get; set; }
			private string _unmodified_String;
			[DataMember] 
			public List< string > StringArray = new List< string >();
			[DataMember] 
			public double Number { get; set; }
			private double _unmodified_Number;
			[DataMember] 
			public List< double > NumberArray = new List< double >();
			[DataMember] 
			public bool Boolean { get; set; }
			private bool _unmodified_Boolean;
			[DataMember] 
			public List< bool > BooleanArray = new List< bool >();
			[DataMember] 
			public DateTime DateTime { get; set; }
			private DateTime _unmodified_DateTime;
			[DataMember] 
			public List< DateTime > DateTimeArray = new List< DateTime >();
			[DataMember] 
			public GenericObject Object { get; set; }
			private GenericObject _unmodified_Object;
			[DataMember] 
			public List< GenericObject > ObjectArray = new List< GenericObject >();
			[DataMember] 
			public string IndexingInfo { get; set; }
			private string _unmodified_IndexingInfo;
			
			}
	#region Operation Calls
	public partial class Server 
	{

		// TODO: Implement in partial 
		//public static async Task ExecuteOperation(string operationName, object parameters) 

		// TODO: Implement in partial 

		// TODO: Implement in partial 


		public static async Task SendEmail(INT.EmailPackage param) 
		{
			await ExecuteOperation("TheBall.Interface.SendEmail", param);
		}

		public static async Task SetCategoryLinkingForConnection() 
		{
			await ExecuteOperation("TheBall.Interface.SetCategoryLinkingForConnection");
		}

		public static async Task ExecuteLegacyHttpPostRequest() 
		{
			await ExecuteOperation("TheBall.Interface.ExecuteLegacyHttpPostRequest");
		}

		public static async Task CreateReceivingConnectionStructures(INT.ConnectionCommunicationData param) 
		{
			await ExecuteOperation("TheBall.Interface.CreateReceivingConnectionStructures", param);
		}

		public static async Task ShareCollabInterfaceObject(INT.ShareCollabParams param) 
		{
			await ExecuteOperation("TheBall.Interface.ShareCollabInterfaceObject", param);
		}

		public static async Task PushSyncNotification(INT.CollaborationPartner param) 
		{
			await ExecuteOperation("TheBall.Interface.PushSyncNotification", param);
		}

		public static async Task PullSyncData(INT.CollaborationPartner param) 
		{
			await ExecuteOperation("TheBall.Interface.PullSyncData", param);
		}

		public static async Task UpdateSharedDataSummaryData(INT.CollaborationPartner param) 
		{
			await ExecuteOperation("TheBall.Interface.UpdateSharedDataSummaryData", param);
		}

		public static async Task DeleteInterfaceJSON(INT.InterfaceJSONData param) 
		{
			await ExecuteOperation("TheBall.Interface.DeleteInterfaceJSON", param);
		}

		public static async Task SaveInterfaceJSON(INT.InterfaceJSONData param) 
		{
			await ExecuteOperation("TheBall.Interface.SaveInterfaceJSON", param);
		}

		public static async Task SaveGroupDetails(INT.GroupDetails param) 
		{
			await ExecuteOperation("TheBall.Interface.SaveGroupDetails", param);
		}
		public static async Task<InterfaceOperation> GetInterfaceOperation(string id = null)
		{
			var result = await GetInformationObject<InterfaceOperation>(id);
			return result;
		}
		public static async Task<ConnectionCollection> GetConnectionCollection(string id = null)
		{
			var result = await GetInformationObject<ConnectionCollection>(id);
			return result;
		}
		public static async Task<Connection> GetConnection(string id = null)
		{
			var result = await GetInformationObject<Connection>(id);
			return result;
		}
		public static async Task<TransferPackage> GetTransferPackage(string id = null)
		{
			var result = await GetInformationObject<TransferPackage>(id);
			return result;
		}
		public static async Task<CategoryLink> GetCategoryLink(string id = null)
		{
			var result = await GetInformationObject<CategoryLink>(id);
			return result;
		}
		public static async Task<Category> GetCategory(string id = null)
		{
			var result = await GetInformationObject<Category>(id);
			return result;
		}
		public static async Task<StatusSummary> GetStatusSummary(string id = null)
		{
			var result = await GetInformationObject<StatusSummary>(id);
			return result;
		}
		public static async Task<InformationChangeItem> GetInformationChangeItem(string id = null)
		{
			var result = await GetInformationObject<InformationChangeItem>(id);
			return result;
		}
		public static async Task<OperationExecutionItem> GetOperationExecutionItem(string id = null)
		{
			var result = await GetInformationObject<OperationExecutionItem>(id);
			return result;
		}
		public static async Task<GenericObjectCollection> GetGenericObjectCollection(string id = null)
		{
			var result = await GetInformationObject<GenericObjectCollection>(id);
			return result;
		}
		public static async Task<GenericCollectionableObject> GetGenericCollectionableObject(string id = null)
		{
			var result = await GetInformationObject<GenericCollectionableObject>(id);
			return result;
		}
		public static async Task<GenericObject> GetGenericObject(string id = null)
		{
			var result = await GetInformationObject<GenericObject>(id);
			return result;
		}
		public static async Task<GenericValue> GetGenericValue(string id = null)
		{
			var result = await GetInformationObject<GenericValue>(id);
			return result;
		}
		public static async Task<INT.ShareCollabParams> GetShareCollabParams(string id = null)
		{
			var result = await GetInterfaceObject<INT.ShareCollabParams>(id);
			return result;
		}
		public static async Task<INT.CollaborationPartner> GetCollaborationPartner(string id = null)
		{
			var result = await GetInterfaceObject<INT.CollaborationPartner>(id);
			return result;
		}
		public static async Task<INT.InterfaceJSONData> GetInterfaceJSONData(string id = null)
		{
			var result = await GetInterfaceObject<INT.InterfaceJSONData>(id);
			return result;
		}
		public static async Task<INT.CollaborationPartnerSummary> GetCollaborationPartnerSummary(string id = null)
		{
			var result = await GetInterfaceObject<INT.CollaborationPartnerSummary>(id);
			return result;
		}
		public static async Task<INT.PartnerSummaryItem> GetPartnerSummaryItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.PartnerSummaryItem>(id);
			return result;
		}
		public static async Task<INT.ShareInfoSummary> GetShareInfoSummary(string id = null)
		{
			var result = await GetInterfaceObject<INT.ShareInfoSummary>(id);
			return result;
		}
		public static async Task<INT.ShareInfo> GetShareInfo(string id = null)
		{
			var result = await GetInterfaceObject<INT.ShareInfo>(id);
			return result;
		}
		public static async Task<INT.ConnectionCommunicationData> GetConnectionCommunicationData(string id = null)
		{
			var result = await GetInterfaceObject<INT.ConnectionCommunicationData>(id);
			return result;
		}
		public static async Task<INT.CategoryInfo> GetCategoryInfo(string id = null)
		{
			var result = await GetInterfaceObject<INT.CategoryInfo>(id);
			return result;
		}
		public static async Task<INT.CategoryLinkParameters> GetCategoryLinkParameters(string id = null)
		{
			var result = await GetInterfaceObject<INT.CategoryLinkParameters>(id);
			return result;
		}
		public static async Task<INT.CategoryLinkItem> GetCategoryLinkItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.CategoryLinkItem>(id);
			return result;
		}
		public static async Task<INT.AccountMembershipData> GetAccountMembershipData(string id = null)
		{
			var result = await GetInterfaceObject<INT.AccountMembershipData>(id);
			return result;
		}
		public static async Task<INT.AccountDetails> GetAccountDetails(string id = null)
		{
			var result = await GetInterfaceObject<INT.AccountDetails>(id);
			return result;
		}
		public static async Task<INT.AccountMembershipItem> GetAccountMembershipItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.AccountMembershipItem>(id);
			return result;
		}
		public static async Task<INT.GroupMembershipData> GetGroupMembershipData(string id = null)
		{
			var result = await GetInterfaceObject<INT.GroupMembershipData>(id);
			return result;
		}
		public static async Task<INT.GroupDetails> GetGroupDetails(string id = null)
		{
			var result = await GetInterfaceObject<INT.GroupDetails>(id);
			return result;
		}
		public static async Task<INT.GroupMembershipItem> GetGroupMembershipItem(string id = null)
		{
			var result = await GetInterfaceObject<INT.GroupMembershipItem>(id);
			return result;
		}
		public static async Task<INT.EmailPackage> GetEmailPackage(string id = null)
		{
			var result = await GetInterfaceObject<INT.EmailPackage>(id);
			return result;
		}
		public static async Task<INT.EmailAttachment> GetEmailAttachment(string id = null)
		{
			var result = await GetInterfaceObject<INT.EmailAttachment>(id);
			return result;
		}
	}
#endregion
 } 