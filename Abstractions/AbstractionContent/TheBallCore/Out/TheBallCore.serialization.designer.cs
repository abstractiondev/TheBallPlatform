 

namespace SER.TheBall.CORE { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE.INT")]
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

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE.INT")]
			public partial class ContentItemLocationWithMD5
			{
				[DataMember]
				public string ContentLocation { get; set; }
				[DataMember]
				public string ContentMD5 { get; set; }
				[DataMember]
				public ItemData[] ItemDatas { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE.INT")]
			public partial class ItemData
			{
				[DataMember]
				public string DataName { get; set; }
				[DataMember]
				public string ItemTextData { get; set; }
			}

 }             [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] [ProtoContract]
			[Serializable]
			public partial class Account 
			{

				public Account()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Account";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Account));
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

				public static Account DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Account));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Account) serializer.ReadObject(xmlReader);
					}
            
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
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] [ProtoContract]
			[Serializable]
			public partial class Group 
			{

				public Group()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Group";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Group));
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

				public static Group DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Group));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Group) serializer.ReadObject(xmlReader);
					}
            
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
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] [ProtoContract]
			[Serializable]
			public partial class GroupMembership 
			{

				public GroupMembership()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "GroupMembership";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupMembership));
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

				public static GroupMembership DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(GroupMembership));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (GroupMembership) serializer.ReadObject(xmlReader);
					}
            
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



				private void CopyContentFrom(GroupMembership sourceObject)
				{
					Account = sourceObject.Account;
					Group = sourceObject.Group;
					Role = sourceObject.Role;
				}
				



			[DataMember] [ProtoMember(1)]
			public string Account { get; set; }
			private string _unmodified_Account;
			[DataMember] [ProtoMember(2)]
			public string Group { get; set; }
			private string _unmodified_Group;
			[DataMember] [ProtoMember(3)]
			public string Role { get; set; }
			private string _unmodified_Role;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ContentPackageCollection 
			{

				public ContentPackageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ContentPackageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContentPackageCollection));
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

				public static ContentPackageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContentPackageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ContentPackageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<ContentPackage> CollectionContent = new List<ContentPackage>();
				private ContentPackage[] _unmodified_CollectionContent;

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

				public ContentPackage[] GetIDSelectedArray()
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

				private void CopyContentFrom(ContentPackageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ContentPackage 
			{

				public ContentPackage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ContentPackage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContentPackage));
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

				public static ContentPackage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ContentPackage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ContentPackage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(ContentPackage sourceObject)
				{
					PackageType = sourceObject.PackageType;
					PackageName = sourceObject.PackageName;
					Description = sourceObject.Description;
					PackageRootFolder = sourceObject.PackageRootFolder;
					CreationTime = sourceObject.CreationTime;
				}
				



			[DataMember] 
			public string PackageType { get; set; }
			private string _unmodified_PackageType;
			[DataMember] 
			public string PackageName { get; set; }
			private string _unmodified_PackageName;
			[DataMember] 
			public string Description { get; set; }
			private string _unmodified_Description;
			[DataMember] 
			public string PackageRootFolder { get; set; }
			private string _unmodified_PackageRootFolder;
			[DataMember] 
			public DateTime CreationTime { get; set; }
			private DateTime _unmodified_CreationTime;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InformationInputCollection 
			{

				public InformationInputCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationInputCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationInputCollection));
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

				public static InformationInputCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationInputCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationInputCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InformationInput> CollectionContent = new List<InformationInput>();
				private InformationInput[] _unmodified_CollectionContent;

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

				public InformationInput[] GetIDSelectedArray()
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

				private void CopyContentFrom(InformationInputCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InformationInput 
			{

				public InformationInput()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationInput";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationInput));
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

				public static InformationInput DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationInput));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationInput) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InformationInput sourceObject)
				{
					InputDescription = sourceObject.InputDescription;
					LocationURL = sourceObject.LocationURL;
					LocalContentName = sourceObject.LocalContentName;
					AuthenticatedDeviceID = sourceObject.AuthenticatedDeviceID;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				



			[DataMember] 
			public string InputDescription { get; set; }
			private string _unmodified_InputDescription;
			[DataMember] 
			public string LocationURL { get; set; }
			private string _unmodified_LocationURL;
			[DataMember] 
			public string LocalContentName { get; set; }
			private string _unmodified_LocalContentName;
			[DataMember] 
			public string AuthenticatedDeviceID { get; set; }
			private string _unmodified_AuthenticatedDeviceID;
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			private bool _unmodified_IsValidatedAndActive;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InformationOutputCollection 
			{

				public InformationOutputCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOutputCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOutputCollection));
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

				public static InformationOutputCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOutputCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationOutputCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InformationOutput> CollectionContent = new List<InformationOutput>();
				private InformationOutput[] _unmodified_CollectionContent;

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

				public InformationOutput[] GetIDSelectedArray()
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

				private void CopyContentFrom(InformationOutputCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InformationOutput 
			{

				public InformationOutput()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOutput";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOutput));
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

				public static InformationOutput DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOutput));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationOutput) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InformationOutput sourceObject)
				{
					OutputDescription = sourceObject.OutputDescription;
					DestinationURL = sourceObject.DestinationURL;
					DestinationContentName = sourceObject.DestinationContentName;
					LocalContentURL = sourceObject.LocalContentURL;
					AuthenticatedDeviceID = sourceObject.AuthenticatedDeviceID;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				



			[DataMember] 
			public string OutputDescription { get; set; }
			private string _unmodified_OutputDescription;
			[DataMember] 
			public string DestinationURL { get; set; }
			private string _unmodified_DestinationURL;
			[DataMember] 
			public string DestinationContentName { get; set; }
			private string _unmodified_DestinationContentName;
			[DataMember] 
			public string LocalContentURL { get; set; }
			private string _unmodified_LocalContentURL;
			[DataMember] 
			public string AuthenticatedDeviceID { get; set; }
			private string _unmodified_AuthenticatedDeviceID;
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			private bool _unmodified_IsValidatedAndActive;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class AuthenticatedAsActiveDeviceCollection 
			{

				public AuthenticatedAsActiveDeviceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "AuthenticatedAsActiveDeviceCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AuthenticatedAsActiveDeviceCollection));
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

				public static AuthenticatedAsActiveDeviceCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AuthenticatedAsActiveDeviceCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AuthenticatedAsActiveDeviceCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<AuthenticatedAsActiveDevice> CollectionContent = new List<AuthenticatedAsActiveDevice>();
				private AuthenticatedAsActiveDevice[] _unmodified_CollectionContent;

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

				public AuthenticatedAsActiveDevice[] GetIDSelectedArray()
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

				private void CopyContentFrom(AuthenticatedAsActiveDeviceCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class AuthenticatedAsActiveDevice 
			{

				public AuthenticatedAsActiveDevice()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "AuthenticatedAsActiveDevice";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AuthenticatedAsActiveDevice));
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

				public static AuthenticatedAsActiveDevice DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(AuthenticatedAsActiveDevice));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (AuthenticatedAsActiveDevice) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(AuthenticatedAsActiveDevice sourceObject)
				{
					AuthenticationDescription = sourceObject.AuthenticationDescription;
					SharedSecret = sourceObject.SharedSecret;
					ActiveSymmetricAESKey = sourceObject.ActiveSymmetricAESKey;
					EstablishedTrustID = sourceObject.EstablishedTrustID;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
					NegotiationURL = sourceObject.NegotiationURL;
					ConnectionURL = sourceObject.ConnectionURL;
				}
				



			[DataMember] 
			public string AuthenticationDescription { get; set; }
			private string _unmodified_AuthenticationDescription;
			[DataMember] 
			public string SharedSecret { get; set; }
			private string _unmodified_SharedSecret;
			[DataMember] 
			public byte[] ActiveSymmetricAESKey { get; set; }
			private byte[] _unmodified_ActiveSymmetricAESKey;
			[DataMember] 
			public string EstablishedTrustID { get; set; }
			private string _unmodified_EstablishedTrustID;
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			private bool _unmodified_IsValidatedAndActive;
			[DataMember] 
			public string NegotiationURL { get; set; }
			private string _unmodified_NegotiationURL;
			[DataMember] 
			public string ConnectionURL { get; set; }
			private string _unmodified_ConnectionURL;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class DeviceMembershipCollection 
			{

				public DeviceMembershipCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "DeviceMembershipCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(DeviceMembershipCollection));
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

				public static DeviceMembershipCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(DeviceMembershipCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (DeviceMembershipCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<DeviceMembership> CollectionContent = new List<DeviceMembership>();
				private DeviceMembership[] _unmodified_CollectionContent;

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

				public DeviceMembership[] GetIDSelectedArray()
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

				private void CopyContentFrom(DeviceMembershipCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class DeviceMembership 
			{

				public DeviceMembership()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "DeviceMembership";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(DeviceMembership));
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

				public static DeviceMembership DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(DeviceMembership));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (DeviceMembership) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(DeviceMembership sourceObject)
				{
					DeviceDescription = sourceObject.DeviceDescription;
					SharedSecret = sourceObject.SharedSecret;
					ActiveSymmetricAESKey = sourceObject.ActiveSymmetricAESKey;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				



			[DataMember] 
			public string DeviceDescription { get; set; }
			private string _unmodified_DeviceDescription;
			[DataMember] 
			public string SharedSecret { get; set; }
			private string _unmodified_SharedSecret;
			[DataMember] 
			public byte[] ActiveSymmetricAESKey { get; set; }
			private byte[] _unmodified_ActiveSymmetricAESKey;
			[DataMember] 
			public bool IsValidatedAndActive { get; set; }
			private bool _unmodified_IsValidatedAndActive;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceFiscalExportSummary 
			{

				public InvoiceFiscalExportSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceFiscalExportSummary";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceFiscalExportSummary));
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

				public static InvoiceFiscalExportSummary DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceFiscalExportSummary));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceFiscalExportSummary) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceFiscalExportSummary sourceObject)
				{
					FiscalInclusiveStartDate = sourceObject.FiscalInclusiveStartDate;
					FiscalInclusiveEndDate = sourceObject.FiscalInclusiveEndDate;
					ExportedInvoices = sourceObject.ExportedInvoices;
				}
				



			[DataMember] 
			public DateTime FiscalInclusiveStartDate { get; set; }
			private DateTime _unmodified_FiscalInclusiveStartDate;
			[DataMember] 
			public DateTime FiscalInclusiveEndDate { get; set; }
			private DateTime _unmodified_FiscalInclusiveEndDate;
			[DataMember] 
			public InvoiceCollection ExportedInvoices { get; set; }
			private InvoiceCollection _unmodified_ExportedInvoices;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceSummaryContainer 
			{

				public InvoiceSummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceSummaryContainer";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceSummaryContainer));
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

				public static InvoiceSummaryContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceSummaryContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceSummaryContainer) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceSummaryContainer sourceObject)
				{
					OpenInvoices = sourceObject.OpenInvoices;
					PredictedInvoices = sourceObject.PredictedInvoices;
					PaidInvoicesActiveYear = sourceObject.PaidInvoicesActiveYear;
					PaidInvoicesLast12Months = sourceObject.PaidInvoicesLast12Months;
				}
				



			[DataMember] 
			public InvoiceCollection OpenInvoices { get; set; }
			private InvoiceCollection _unmodified_OpenInvoices;
			[DataMember] 
			public InvoiceCollection PredictedInvoices { get; set; }
			private InvoiceCollection _unmodified_PredictedInvoices;
			[DataMember] 
			public InvoiceCollection PaidInvoicesActiveYear { get; set; }
			private InvoiceCollection _unmodified_PaidInvoicesActiveYear;
			[DataMember] 
			public InvoiceCollection PaidInvoicesLast12Months { get; set; }
			private InvoiceCollection _unmodified_PaidInvoicesLast12Months;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceCollection 
			{

				public InvoiceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceCollection));
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

				public static InvoiceCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<Invoice> CollectionContent = new List<Invoice>();
				private Invoice[] _unmodified_CollectionContent;

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

				public Invoice[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class Invoice 
			{

				public Invoice()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Invoice";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Invoice));
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

				public static Invoice DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Invoice));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Invoice) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(Invoice sourceObject)
				{
					InvoiceName = sourceObject.InvoiceName;
					InvoiceID = sourceObject.InvoiceID;
					InvoicedAmount = sourceObject.InvoicedAmount;
					CreateDate = sourceObject.CreateDate;
					DueDate = sourceObject.DueDate;
					PaidAmount = sourceObject.PaidAmount;
					FeesAndInterestAmount = sourceObject.FeesAndInterestAmount;
					UnpaidAmount = sourceObject.UnpaidAmount;
					InvoiceDetails = sourceObject.InvoiceDetails;
					InvoiceUsers = sourceObject.InvoiceUsers;
				}
				



			[DataMember] 
			public string InvoiceName { get; set; }
			private string _unmodified_InvoiceName;
			[DataMember] 
			public string InvoiceID { get; set; }
			private string _unmodified_InvoiceID;
			[DataMember] 
			public string InvoicedAmount { get; set; }
			private string _unmodified_InvoicedAmount;
			[DataMember] 
			public DateTime CreateDate { get; set; }
			private DateTime _unmodified_CreateDate;
			[DataMember] 
			public DateTime DueDate { get; set; }
			private DateTime _unmodified_DueDate;
			[DataMember] 
			public string PaidAmount { get; set; }
			private string _unmodified_PaidAmount;
			[DataMember] 
			public string FeesAndInterestAmount { get; set; }
			private string _unmodified_FeesAndInterestAmount;
			[DataMember] 
			public string UnpaidAmount { get; set; }
			private string _unmodified_UnpaidAmount;
			[DataMember] 
			public InvoiceDetails InvoiceDetails { get; set; }
			private InvoiceDetails _unmodified_InvoiceDetails;
			[DataMember] 
			public InvoiceUserCollection InvoiceUsers { get; set; }
			private InvoiceUserCollection _unmodified_InvoiceUsers;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceDetails 
			{

				public InvoiceDetails()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceDetails";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceDetails));
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

				public static InvoiceDetails DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceDetails));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceDetails) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceDetails sourceObject)
				{
					MonthlyFeesTotal = sourceObject.MonthlyFeesTotal;
					OneTimeFeesTotal = sourceObject.OneTimeFeesTotal;
					UsageFeesTotal = sourceObject.UsageFeesTotal;
					InterestFeesTotal = sourceObject.InterestFeesTotal;
					PenaltyFeesTotal = sourceObject.PenaltyFeesTotal;
					TotalFeesTotal = sourceObject.TotalFeesTotal;
				}
				



			[DataMember] 
			public string MonthlyFeesTotal { get; set; }
			private string _unmodified_MonthlyFeesTotal;
			[DataMember] 
			public string OneTimeFeesTotal { get; set; }
			private string _unmodified_OneTimeFeesTotal;
			[DataMember] 
			public string UsageFeesTotal { get; set; }
			private string _unmodified_UsageFeesTotal;
			[DataMember] 
			public string InterestFeesTotal { get; set; }
			private string _unmodified_InterestFeesTotal;
			[DataMember] 
			public string PenaltyFeesTotal { get; set; }
			private string _unmodified_PenaltyFeesTotal;
			[DataMember] 
			public string TotalFeesTotal { get; set; }
			private string _unmodified_TotalFeesTotal;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceUserCollection 
			{

				public InvoiceUserCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUserCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUserCollection));
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

				public static InvoiceUserCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUserCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceUserCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InvoiceUser> CollectionContent = new List<InvoiceUser>();
				private InvoiceUser[] _unmodified_CollectionContent;

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

				public InvoiceUser[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceUserCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceUser 
			{

				public InvoiceUser()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUser";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUser));
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

				public static InvoiceUser DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceUser));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceUser) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceUser sourceObject)
				{
					UserName = sourceObject.UserName;
					UserID = sourceObject.UserID;
					UserPhoneNumber = sourceObject.UserPhoneNumber;
					UserSubscriptionNumber = sourceObject.UserSubscriptionNumber;
					UserInvoiceTotalAmount = sourceObject.UserInvoiceTotalAmount;
					InvoiceRowGroupCollection = sourceObject.InvoiceRowGroupCollection;
					InvoiceEventDetailGroupCollection = sourceObject.InvoiceEventDetailGroupCollection;
				}
				



			[DataMember] 
			public string UserName { get; set; }
			private string _unmodified_UserName;
			[DataMember] 
			public string UserID { get; set; }
			private string _unmodified_UserID;
			[DataMember] 
			public string UserPhoneNumber { get; set; }
			private string _unmodified_UserPhoneNumber;
			[DataMember] 
			public string UserSubscriptionNumber { get; set; }
			private string _unmodified_UserSubscriptionNumber;
			[DataMember] 
			public string UserInvoiceTotalAmount { get; set; }
			private string _unmodified_UserInvoiceTotalAmount;
			[DataMember] 
			public InvoiceRowGroupCollection InvoiceRowGroupCollection { get; set; }
			private InvoiceRowGroupCollection _unmodified_InvoiceRowGroupCollection;
			[DataMember] 
			public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection { get; set; }
			private InvoiceEventDetailGroupCollection _unmodified_InvoiceEventDetailGroupCollection;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceRowGroupCollection 
			{

				public InvoiceRowGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroupCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroupCollection));
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

				public static InvoiceRowGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowGroupCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InvoiceRowGroup> CollectionContent = new List<InvoiceRowGroup>();
				private InvoiceRowGroup[] _unmodified_CollectionContent;

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

				public InvoiceRowGroup[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceRowGroupCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceEventDetailGroupCollection 
			{

				public InvoiceEventDetailGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroupCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroupCollection));
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

				public static InvoiceEventDetailGroupCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroupCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailGroupCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InvoiceEventDetailGroup> CollectionContent = new List<InvoiceEventDetailGroup>();
				private InvoiceEventDetailGroup[] _unmodified_CollectionContent;

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

				public InvoiceEventDetailGroup[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceEventDetailGroupCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceRowGroup 
			{

				public InvoiceRowGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroup";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroup));
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

				public static InvoiceRowGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowGroup) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceRowGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					GroupTotalPriceWithoutTaxes = sourceObject.GroupTotalPriceWithoutTaxes;
					GroupTotalTaxes = sourceObject.GroupTotalTaxes;
					GroupTotalPriceWithTaxes = sourceObject.GroupTotalPriceWithTaxes;
					InvoiceRowCollection = sourceObject.InvoiceRowCollection;
				}
				



			[DataMember] 
			public string GroupName { get; set; }
			private string _unmodified_GroupName;
			[DataMember] 
			public string GroupTotalPriceWithoutTaxes { get; set; }
			private string _unmodified_GroupTotalPriceWithoutTaxes;
			[DataMember] 
			public string GroupTotalTaxes { get; set; }
			private string _unmodified_GroupTotalTaxes;
			[DataMember] 
			public string GroupTotalPriceWithTaxes { get; set; }
			private string _unmodified_GroupTotalPriceWithTaxes;
			[DataMember] 
			public InvoiceRowCollection InvoiceRowCollection { get; set; }
			private InvoiceRowCollection _unmodified_InvoiceRowCollection;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceEventDetailGroup 
			{

				public InvoiceEventDetailGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroup";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroup));
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

				public static InvoiceEventDetailGroup DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailGroup));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailGroup) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceEventDetailGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					InvoiceEventDetailCollection = sourceObject.InvoiceEventDetailCollection;
				}
				



			[DataMember] 
			public string GroupName { get; set; }
			private string _unmodified_GroupName;
			[DataMember] 
			public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
			private InvoiceEventDetailCollection _unmodified_InvoiceEventDetailCollection;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceEventDetailCollection 
			{

				public InvoiceEventDetailCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailCollection));
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

				public static InvoiceEventDetailCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetailCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetailCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InvoiceEventDetail> CollectionContent = new List<InvoiceEventDetail>();
				private InvoiceEventDetail[] _unmodified_CollectionContent;

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

				public InvoiceEventDetail[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceEventDetailCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceRowCollection 
			{

				public InvoiceRowCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowCollection));
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

				public static InvoiceRowCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRowCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRowCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<InvoiceRow> CollectionContent = new List<InvoiceRow>();
				private InvoiceRow[] _unmodified_CollectionContent;

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

				public InvoiceRow[] GetIDSelectedArray()
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

				private void CopyContentFrom(InvoiceRowCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceEventDetail 
			{

				public InvoiceEventDetail()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetail";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetail));
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

				public static InvoiceEventDetail DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceEventDetail));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceEventDetail) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceEventDetail sourceObject)
				{
					IndentMode = sourceObject.IndentMode;
					EventStartDateTime = sourceObject.EventStartDateTime;
					EventEndDateTime = sourceObject.EventEndDateTime;
					ReceivingParty = sourceObject.ReceivingParty;
					AmountOfUnits = sourceObject.AmountOfUnits;
					Duration = sourceObject.Duration;
					UnitPrice = sourceObject.UnitPrice;
					PriceWithoutTaxes = sourceObject.PriceWithoutTaxes;
					Taxes = sourceObject.Taxes;
					PriceWithTaxes = sourceObject.PriceWithTaxes;
				}
				



			[DataMember] 
			public string IndentMode { get; set; }
			private string _unmodified_IndentMode;
			[DataMember] 
			public DateTime EventStartDateTime { get; set; }
			private DateTime _unmodified_EventStartDateTime;
			[DataMember] 
			public DateTime EventEndDateTime { get; set; }
			private DateTime _unmodified_EventEndDateTime;
			[DataMember] 
			public string ReceivingParty { get; set; }
			private string _unmodified_ReceivingParty;
			[DataMember] 
			public string AmountOfUnits { get; set; }
			private string _unmodified_AmountOfUnits;
			[DataMember] 
			public string Duration { get; set; }
			private string _unmodified_Duration;
			[DataMember] 
			public string UnitPrice { get; set; }
			private string _unmodified_UnitPrice;
			[DataMember] 
			public string PriceWithoutTaxes { get; set; }
			private string _unmodified_PriceWithoutTaxes;
			[DataMember] 
			public string Taxes { get; set; }
			private string _unmodified_Taxes;
			[DataMember] 
			public string PriceWithTaxes { get; set; }
			private string _unmodified_PriceWithTaxes;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InvoiceRow 
			{

				public InvoiceRow()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRow";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRow));
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

				public static InvoiceRow DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InvoiceRow));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InvoiceRow) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InvoiceRow sourceObject)
				{
					IndentMode = sourceObject.IndentMode;
					AmountOfUnits = sourceObject.AmountOfUnits;
					Duration = sourceObject.Duration;
					UnitPrice = sourceObject.UnitPrice;
					PriceWithoutTaxes = sourceObject.PriceWithoutTaxes;
					Taxes = sourceObject.Taxes;
					PriceWithTaxes = sourceObject.PriceWithTaxes;
				}
				



			[DataMember] 
			public string IndentMode { get; set; }
			private string _unmodified_IndentMode;
			[DataMember] 
			public string AmountOfUnits { get; set; }
			private string _unmodified_AmountOfUnits;
			[DataMember] 
			public string Duration { get; set; }
			private string _unmodified_Duration;
			[DataMember] 
			public string UnitPrice { get; set; }
			private string _unmodified_UnitPrice;
			[DataMember] 
			public string PriceWithoutTaxes { get; set; }
			private string _unmodified_PriceWithoutTaxes;
			[DataMember] 
			public string Taxes { get; set; }
			private string _unmodified_Taxes;
			[DataMember] 
			public string PriceWithTaxes { get; set; }
			private string _unmodified_PriceWithTaxes;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class CategoryCollection 
			{

				public CategoryCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "CategoryCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryCollection));
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

				public static CategoryCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (CategoryCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<Category> CollectionContent = new List<Category>();
				private Category[] _unmodified_CollectionContent;

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

				public Category[] GetIDSelectedArray()
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

				private void CopyContentFrom(CategoryCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class Category 
			{

				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
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
					CategoryName = sourceObject.CategoryName;
				}
				



			[DataMember] 
			public string CategoryName { get; set; }
			private string _unmodified_CategoryName;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ProcessContainer 
			{

				public ProcessContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessContainer";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessContainer));
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

				public static ProcessContainer DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessContainer));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProcessContainer) serializer.ReadObject(xmlReader);
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
			public List< string > ProcessIDs = new List< string >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class Process 
			{

				public Process()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Process";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Process));
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

				public static Process DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Process));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Process) serializer.ReadObject(xmlReader);
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
			public string ProcessDescription { get; set; }
			private string _unmodified_ProcessDescription;
			[DataMember] 
			public SemanticInformationItem ExecutingOperation { get; set; }
			private SemanticInformationItem _unmodified_ExecutingOperation;
			[DataMember] 
			public List< SemanticInformationItem > InitialArguments = new List< SemanticInformationItem >();
			[DataMember] 
			public List< ProcessItem > ProcessItems = new List< ProcessItem >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ProcessItem 
			{

				public ProcessItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessItem";
				}

		

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessItem));
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

				public static ProcessItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProcessItem) serializer.ReadObject(xmlReader);
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
			public List< SemanticInformationItem > Outputs = new List< SemanticInformationItem >();
			[DataMember] 
			public List< SemanticInformationItem > Inputs = new List< SemanticInformationItem >();
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class SemanticInformationItem 
			{

				public SemanticInformationItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "SemanticInformationItem";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SemanticInformationItem));
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

				public static SemanticInformationItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(SemanticInformationItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (SemanticInformationItem) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(SemanticInformationItem sourceObject)
				{
					ItemFullType = sourceObject.ItemFullType;
					ItemValue = sourceObject.ItemValue;
				}
				



			[DataMember] 
			public string ItemFullType { get; set; }
			private string _unmodified_ItemFullType;
			[DataMember] 
			public string ItemValue { get; set; }
			private string _unmodified_ItemValue;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class InformationOwnerInfo 
			{

				public InformationOwnerInfo()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOwnerInfo";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOwnerInfo));
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

				public static InformationOwnerInfo DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(InformationOwnerInfo));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (InformationOwnerInfo) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(InformationOwnerInfo sourceObject)
				{
					OwnerType = sourceObject.OwnerType;
					OwnerIdentifier = sourceObject.OwnerIdentifier;
				}
				



			[DataMember] 
			public string OwnerType { get; set; }
			private string _unmodified_OwnerType;
			[DataMember] 
			public string OwnerIdentifier { get; set; }
			private string _unmodified_OwnerIdentifier;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class UsageSummary 
			{

				public UsageSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "UsageSummary";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UsageSummary));
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

				public static UsageSummary DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UsageSummary));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (UsageSummary) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(UsageSummary sourceObject)
				{
					SummaryName = sourceObject.SummaryName;
					SummaryMonitoringItem = sourceObject.SummaryMonitoringItem;
				}
				



			[DataMember] 
			public string SummaryName { get; set; }
			private string _unmodified_SummaryName;
			[DataMember] 
			public UsageMonitorItem SummaryMonitoringItem { get; set; }
			private UsageMonitorItem _unmodified_SummaryMonitoringItem;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class UsageMonitorItem 
			{

				public UsageMonitorItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "UsageMonitorItem";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UsageMonitorItem));
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

				public static UsageMonitorItem DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(UsageMonitorItem));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (UsageMonitorItem) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(UsageMonitorItem sourceObject)
				{
					OwnerInfo = sourceObject.OwnerInfo;
					TimeRangeInclusiveStartExclusiveEnd = sourceObject.TimeRangeInclusiveStartExclusiveEnd;
					StepSizeInMinutes = sourceObject.StepSizeInMinutes;
					ProcessorUsages = sourceObject.ProcessorUsages;
					StorageTransactionUsages = sourceObject.StorageTransactionUsages;
					StorageUsages = sourceObject.StorageUsages;
					NetworkUsages = sourceObject.NetworkUsages;
				}
				



			[DataMember] 
			public InformationOwnerInfo OwnerInfo { get; set; }
			private InformationOwnerInfo _unmodified_OwnerInfo;
			[DataMember] 
			public TimeRange TimeRangeInclusiveStartExclusiveEnd { get; set; }
			private TimeRange _unmodified_TimeRangeInclusiveStartExclusiveEnd;
			[DataMember] 
			public long StepSizeInMinutes { get; set; }
			private long _unmodified_StepSizeInMinutes;
			[DataMember] 
			public ProcessorUsageCollection ProcessorUsages { get; set; }
			private ProcessorUsageCollection _unmodified_ProcessorUsages;
			[DataMember] 
			public StorageTransactionUsageCollection StorageTransactionUsages { get; set; }
			private StorageTransactionUsageCollection _unmodified_StorageTransactionUsages;
			[DataMember] 
			public StorageUsageCollection StorageUsages { get; set; }
			private StorageUsageCollection _unmodified_StorageUsages;
			[DataMember] 
			public NetworkUsageCollection NetworkUsages { get; set; }
			private NetworkUsageCollection _unmodified_NetworkUsages;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class RequestResourceUsageCollection 
			{

				public RequestResourceUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "RequestResourceUsageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RequestResourceUsageCollection));
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

				public static RequestResourceUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RequestResourceUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (RequestResourceUsageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<RequestResourceUsage> CollectionContent = new List<RequestResourceUsage>();
				private RequestResourceUsage[] _unmodified_CollectionContent;

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

				public RequestResourceUsage[] GetIDSelectedArray()
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

				private void CopyContentFrom(RequestResourceUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class RequestResourceUsage 
			{

				public RequestResourceUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "RequestResourceUsage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RequestResourceUsage));
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

				public static RequestResourceUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(RequestResourceUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (RequestResourceUsage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(RequestResourceUsage sourceObject)
				{
					OwnerInfo = sourceObject.OwnerInfo;
					ProcessorUsage = sourceObject.ProcessorUsage;
					StorageTransactionUsage = sourceObject.StorageTransactionUsage;
					NetworkUsage = sourceObject.NetworkUsage;
					RequestDetails = sourceObject.RequestDetails;
				}
				



			[DataMember] 
			public InformationOwnerInfo OwnerInfo { get; set; }
			private InformationOwnerInfo _unmodified_OwnerInfo;
			[DataMember] 
			public ProcessorUsage ProcessorUsage { get; set; }
			private ProcessorUsage _unmodified_ProcessorUsage;
			[DataMember] 
			public StorageTransactionUsage StorageTransactionUsage { get; set; }
			private StorageTransactionUsage _unmodified_StorageTransactionUsage;
			[DataMember] 
			public NetworkUsage NetworkUsage { get; set; }
			private NetworkUsage _unmodified_NetworkUsage;
			[DataMember] 
			public HTTPActivityDetails RequestDetails { get; set; }
			private HTTPActivityDetails _unmodified_RequestDetails;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ProcessorUsageCollection 
			{

				public ProcessorUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessorUsageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessorUsageCollection));
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

				public static ProcessorUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessorUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProcessorUsageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<ProcessorUsage> CollectionContent = new List<ProcessorUsage>();
				private ProcessorUsage[] _unmodified_CollectionContent;

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

				public ProcessorUsage[] GetIDSelectedArray()
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

				private void CopyContentFrom(ProcessorUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class ProcessorUsage 
			{

				public ProcessorUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessorUsage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessorUsage));
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

				public static ProcessorUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(ProcessorUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (ProcessorUsage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(ProcessorUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfTicks = sourceObject.AmountOfTicks;
					FrequencyTicksPerSecond = sourceObject.FrequencyTicksPerSecond;
					Milliseconds = sourceObject.Milliseconds;
				}
				



			[DataMember] 
			public TimeRange TimeRange { get; set; }
			private TimeRange _unmodified_TimeRange;
			[DataMember] 
			public string UsageType { get; set; }
			private string _unmodified_UsageType;
			[DataMember] 
			public double AmountOfTicks { get; set; }
			private double _unmodified_AmountOfTicks;
			[DataMember] 
			public double FrequencyTicksPerSecond { get; set; }
			private double _unmodified_FrequencyTicksPerSecond;
			[DataMember] 
			public long Milliseconds { get; set; }
			private long _unmodified_Milliseconds;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class StorageTransactionUsageCollection 
			{

				public StorageTransactionUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageTransactionUsageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageTransactionUsageCollection));
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

				public static StorageTransactionUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageTransactionUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StorageTransactionUsageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<StorageTransactionUsage> CollectionContent = new List<StorageTransactionUsage>();
				private StorageTransactionUsage[] _unmodified_CollectionContent;

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

				public StorageTransactionUsage[] GetIDSelectedArray()
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

				private void CopyContentFrom(StorageTransactionUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class StorageTransactionUsage 
			{

				public StorageTransactionUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageTransactionUsage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageTransactionUsage));
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

				public static StorageTransactionUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageTransactionUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StorageTransactionUsage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(StorageTransactionUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfTransactions = sourceObject.AmountOfTransactions;
				}
				



			[DataMember] 
			public TimeRange TimeRange { get; set; }
			private TimeRange _unmodified_TimeRange;
			[DataMember] 
			public string UsageType { get; set; }
			private string _unmodified_UsageType;
			[DataMember] 
			public long AmountOfTransactions { get; set; }
			private long _unmodified_AmountOfTransactions;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class StorageUsageCollection 
			{

				public StorageUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageUsageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageUsageCollection));
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

				public static StorageUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StorageUsageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<StorageUsage> CollectionContent = new List<StorageUsage>();
				private StorageUsage[] _unmodified_CollectionContent;

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

				public StorageUsage[] GetIDSelectedArray()
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

				private void CopyContentFrom(StorageUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class StorageUsage 
			{

				public StorageUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageUsage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageUsage));
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

				public static StorageUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StorageUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (StorageUsage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(StorageUsage sourceObject)
				{
					SnapshotTime = sourceObject.SnapshotTime;
					UsageType = sourceObject.UsageType;
					UsageUnit = sourceObject.UsageUnit;
					AmountOfUnits = sourceObject.AmountOfUnits;
				}
				



			[DataMember] 
			public DateTime SnapshotTime { get; set; }
			private DateTime _unmodified_SnapshotTime;
			[DataMember] 
			public string UsageType { get; set; }
			private string _unmodified_UsageType;
			[DataMember] 
			public string UsageUnit { get; set; }
			private string _unmodified_UsageUnit;
			[DataMember] 
			public double AmountOfUnits { get; set; }
			private double _unmodified_AmountOfUnits;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class NetworkUsageCollection 
			{

				public NetworkUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "NetworkUsageCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NetworkUsageCollection));
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

				public static NetworkUsageCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NetworkUsageCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (NetworkUsageCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<NetworkUsage> CollectionContent = new List<NetworkUsage>();
				private NetworkUsage[] _unmodified_CollectionContent;

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

				public NetworkUsage[] GetIDSelectedArray()
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

				private void CopyContentFrom(NetworkUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class NetworkUsage 
			{

				public NetworkUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "NetworkUsage";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NetworkUsage));
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

				public static NetworkUsage DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(NetworkUsage));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (NetworkUsage) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(NetworkUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfBytes = sourceObject.AmountOfBytes;
				}
				



			[DataMember] 
			public TimeRange TimeRange { get; set; }
			private TimeRange _unmodified_TimeRange;
			[DataMember] 
			public string UsageType { get; set; }
			private string _unmodified_UsageType;
			[DataMember] 
			public long AmountOfBytes { get; set; }
			private long _unmodified_AmountOfBytes;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class TimeRange 
			{

				public TimeRange()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "TimeRange";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TimeRange));
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

				public static TimeRange DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TimeRange));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (TimeRange) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(TimeRange sourceObject)
				{
					StartTime = sourceObject.StartTime;
					EndTime = sourceObject.EndTime;
				}
				



			[DataMember] 
			public DateTime StartTime { get; set; }
			private DateTime _unmodified_StartTime;
			[DataMember] 
			public DateTime EndTime { get; set; }
			private DateTime _unmodified_EndTime;
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class HTTPActivityDetailsCollection 
			{

				public HTTPActivityDetailsCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "HTTPActivityDetailsCollection";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(HTTPActivityDetailsCollection));
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

				public static HTTPActivityDetailsCollection DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(HTTPActivityDetailsCollection));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (HTTPActivityDetailsCollection) serializer.ReadObject(xmlReader);
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


				
				[DataMember] public List<HTTPActivityDetails> CollectionContent = new List<HTTPActivityDetails>();
				private HTTPActivityDetails[] _unmodified_CollectionContent;

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

				public HTTPActivityDetails[] GetIDSelectedArray()
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

				private void CopyContentFrom(HTTPActivityDetailsCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
			
			}
            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.CORE")] 
			[Serializable]
			public partial class HTTPActivityDetails 
			{

				public HTTPActivityDetails()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "HTTPActivityDetails";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(HTTPActivityDetails));
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

				public static HTTPActivityDetails DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(HTTPActivityDetails));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (HTTPActivityDetails) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(HTTPActivityDetails sourceObject)
				{
					RemoteIPAddress = sourceObject.RemoteIPAddress;
					RemoteEndpointUserName = sourceObject.RemoteEndpointUserName;
					UserID = sourceObject.UserID;
					UTCDateTime = sourceObject.UTCDateTime;
					RequestLine = sourceObject.RequestLine;
					HTTPStatusCode = sourceObject.HTTPStatusCode;
					ReturnedContentLength = sourceObject.ReturnedContentLength;
				}
				



			[DataMember] 
			public string RemoteIPAddress { get; set; }
			private string _unmodified_RemoteIPAddress;
			[DataMember] 
			public string RemoteEndpointUserName { get; set; }
			private string _unmodified_RemoteEndpointUserName;
			[DataMember] 
			public string UserID { get; set; }
			private string _unmodified_UserID;
			[DataMember] 
			public DateTime UTCDateTime { get; set; }
			private DateTime _unmodified_UTCDateTime;
			[DataMember] 
			public string RequestLine { get; set; }
			private string _unmodified_RequestLine;
			[DataMember] 
			public long HTTPStatusCode { get; set; }
			private long _unmodified_HTTPStatusCode;
			[DataMember] 
			public long ReturnedContentLength { get; set; }
			private long _unmodified_ReturnedContentLength;
			
			}
 } 