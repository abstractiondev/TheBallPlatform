 


using DOM=TheBall.CORE;
using System.Threading.Tasks;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static async Task DOMAININIT_TheBall_CORE(IContainerOwner owner)
		{
			await DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			await DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.CORE { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;
using ProtoBuf;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.Storage;

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

 } 		public static class DomainInformationSupport
		{
            public static async Task EnsureMasterCollections(IContainerOwner owner)
            {
                {
                    var masterCollection = await ContentPackageCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = ContentPackageCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            ContentPackageCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
                {
                    var masterCollection = await InformationInputCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = InformationInputCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            InformationInputCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
                {
                    var masterCollection = await InformationOutputCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = InformationOutputCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            InformationOutputCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
                {
                    var masterCollection = await AuthenticatedAsActiveDeviceCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = AuthenticatedAsActiveDeviceCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            AuthenticatedAsActiveDeviceCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
                {
                    var masterCollection = await DeviceMembershipCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = DeviceMembershipCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            DeviceMembershipCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
                {
                    var masterCollection = await InvoiceCollection.GetMasterCollectionInstanceAsync(owner);
                    if(masterCollection == null)
                    {
                        masterCollection = InvoiceCollection.CreateDefault();
                        masterCollection.RelativeLocation =
                            InvoiceCollection.GetMasterCollectionLocation(owner);
                        await StorageSupport.StoreInformationAsync(masterCollection, owner);
                    }
					IInformationCollection collection = masterCollection;
                }
            }

            public static async Task RefreshMasterCollections(IContainerOwner owner)
            {
                {
                    IInformationCollection masterCollection = await ContentPackageCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection ContentPackageCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = await InformationInputCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection InformationInputCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = await InformationOutputCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection InformationOutputCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = await AuthenticatedAsActiveDeviceCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection AuthenticatedAsActiveDeviceCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = await DeviceMembershipCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection DeviceMembershipCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
                {
                    IInformationCollection masterCollection = await InvoiceCollection.GetMasterCollectionInstanceAsync(owner);
                    if (masterCollection == null)
                        throw new InvalidDataException("Master collection InvoiceCollection missing for owner");
                    await masterCollection.RefreshContentAsync();
                    await StorageSupport.StoreInformationAsync((IInformationObject) masterCollection, owner);
                }
            }
		}
			[DataContract] [ProtoContract]
			[Serializable]
			public partial class Login : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.ProtoBuf;
					}
				}

				public Login()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Login";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Login/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Login), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Login>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Login");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Login), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Login/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Login/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((Login) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Login));
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

				public static Login DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Login));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Login) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Login", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Login", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Login customDemoObject);



				public static Login CreateDefault()
				{
					var result = new Login();
					return result;
				}
				/*
				public static Login CreateDemoDefault()
				{
					Login customDemo = null;
					Login.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Login();
					result.LoginName = @"Login.LoginName";

					result.PasswordHash = @"Login.PasswordHash";

					result.PasswordSalt = @"Login.PasswordSalt";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(LoginName != _unmodified_LoginName)
							return true;
						if(PasswordHash != _unmodified_PasswordHash)
							return true;
						if(PasswordSalt != _unmodified_PasswordSalt)
							return true;
						if(Account != _unmodified_Account)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Login sourceObject)
				{
					LoginName = sourceObject.LoginName;
					PasswordHash = sourceObject.PasswordHash;
					PasswordSalt = sourceObject.PasswordSalt;
					Account = sourceObject.Account;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_LoginName = LoginName;
					_unmodified_PasswordHash = PasswordHash;
					_unmodified_PasswordSalt = PasswordSalt;
					_unmodified_Account = Account;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "LoginName":
							LoginName = value;
							break;
						case "PasswordHash":
							PasswordHash = value;
							break;
						case "PasswordSalt":
							PasswordSalt = value;
							break;
						case "Account":
							Account = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] [ProtoMember(1)]
			public string LoginName { get; set; }
			private string _unmodified_LoginName;
			[DataMember] [ProtoMember(2)]
			public string PasswordHash { get; set; }
			private string _unmodified_PasswordHash;
			[DataMember] [ProtoMember(3)]
			public string PasswordSalt { get; set; }
			private string _unmodified_PasswordSalt;
			[DataMember] [ProtoMember(4)]
			public string Account { get; set; }
			private string _unmodified_Account;
			
			}
			[DataContract] [ProtoContract]
			[Serializable]
			public partial class Email : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.ProtoBuf;
					}
				}

				public Email()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Email";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Email/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Email), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Email>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Email");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Email), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Email/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Email/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((Email) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
				}

				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Email));
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

				public static Email DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Email));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Email) serializer.ReadObject(xmlReader);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Email", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Email", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Email customDemoObject);



				public static Email CreateDefault()
				{
					var result = new Email();
					return result;
				}
				/*
				public static Email CreateDemoDefault()
				{
					Email customDemo = null;
					Email.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Email();
					result.EmailAddress = @"Email.EmailAddress";

					result.ValidationKey = @"Email.ValidationKey";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(EmailAddress != _unmodified_EmailAddress)
							return true;
						if(Account != _unmodified_Account)
							return true;
						if(PendingValidation != _unmodified_PendingValidation)
							return true;
						if(ValidationKey != _unmodified_ValidationKey)
							return true;
						if(ValidationProcessExpiration != _unmodified_ValidationProcessExpiration)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Email sourceObject)
				{
					EmailAddress = sourceObject.EmailAddress;
					Account = sourceObject.Account;
					PendingValidation = sourceObject.PendingValidation;
					ValidationKey = sourceObject.ValidationKey;
					ValidationProcessExpiration = sourceObject.ValidationProcessExpiration;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_EmailAddress = EmailAddress;
					_unmodified_Account = Account;
					_unmodified_PendingValidation = PendingValidation;
					_unmodified_ValidationKey = ValidationKey;
					_unmodified_ValidationProcessExpiration = ValidationProcessExpiration;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "EmailAddress":
							EmailAddress = value;
							break;
						case "Account":
							Account = value;
							break;
						case "PendingValidation":
							PendingValidation = bool.Parse(value);
							break;
						case "ValidationKey":
							ValidationKey = value;
							break;
						case "ValidationProcessExpiration":
							ValidationProcessExpiration = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] [ProtoMember(1)]
			public string EmailAddress { get; set; }
			private string _unmodified_EmailAddress;
			[DataMember] [ProtoMember(2)]
			public string Account { get; set; }
			private string _unmodified_Account;
			[DataMember] [ProtoMember(3)]
			public bool PendingValidation { get; set; }
			private bool _unmodified_PendingValidation;
			[DataMember] [ProtoMember(4)]
			public string ValidationKey { get; set; }
			private string _unmodified_ValidationKey;
			[DataMember] [ProtoMember(5)]
			public DateTime ValidationProcessExpiration { get; set; }
			private DateTime _unmodified_ValidationProcessExpiration;
			
			}
			[DataContract] [ProtoContract]
			[Serializable]
			public partial class Account : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.ProtoBuf;
					}
				}

				public Account()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Account";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Account/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Account), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Account>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Account");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Account), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Account/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Account/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Account", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Account", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Account customDemoObject);



				public static Account CreateDefault()
				{
					var result = new Account();
					return result;
				}
				/*
				public static Account CreateDemoDefault()
				{
					Account customDemo = null;
					Account.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Account();
					result.ServerMetadataJSON = @"Account.ServerMetadataJSON
Account.ServerMetadataJSON
Account.ServerMetadataJSON
Account.ServerMetadataJSON
Account.ServerMetadataJSON
";

					result.ClientMetadataJSON = @"Account.ClientMetadataJSON
Account.ClientMetadataJSON
Account.ClientMetadataJSON
Account.ClientMetadataJSON
Account.ClientMetadataJSON
";

				
					return result;
				}
				*/


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                Task IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Emails":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "Logins":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "GroupMemberships":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "ServerMetadataJSON":
							ServerMetadataJSON = value;
							break;
						case "ClientMetadataJSON":
							ClientMetadataJSON = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] [ProtoMember(1)]
			public List< string > Emails = new List< string >();
			[DataMember] [ProtoMember(2)]
			public List< string > Logins = new List< string >();
			[DataMember] [ProtoMember(3)]
			public List< string > GroupMemberships = new List< string >();
			[DataMember] [ProtoMember(4)]
			public string ServerMetadataJSON { get; set; }
			private string _unmodified_ServerMetadataJSON;
			[DataMember] [ProtoMember(5)]
			public string ClientMetadataJSON { get; set; }
			private string _unmodified_ClientMetadataJSON;
			
			}
			[DataContract] [ProtoContract]
			[Serializable]
			public partial class Group : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.ProtoBuf;
					}
				}

				public Group()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Group";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Group/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Group), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Group>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Group");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Group), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Group/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Group/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Group", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Group", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Group customDemoObject);



				public static Group CreateDefault()
				{
					var result = new Group();
					return result;
				}
				/*
				public static Group CreateDemoDefault()
				{
					Group customDemo = null;
					Group.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Group();
				
					return result;
				}
				*/


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                Task IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupMemberships":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] [ProtoMember(1)]
			public List< string > GroupMemberships = new List< string >();
			
			}
			[DataContract] [ProtoContract]
			[Serializable]
			public partial class GroupMembership : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.ProtoBuf;
					}
				}

				public GroupMembership()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "GroupMembership";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/GroupMembership/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(GroupMembership), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<GroupMembership>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: GroupMembership");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(GroupMembership), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/GroupMembership/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/GroupMembership/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((GroupMembership) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "GroupMembership", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "GroupMembership", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref GroupMembership customDemoObject);



				public static GroupMembership CreateDefault()
				{
					var result = new GroupMembership();
					return result;
				}
				/*
				public static GroupMembership CreateDemoDefault()
				{
					GroupMembership customDemo = null;
					GroupMembership.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new GroupMembership();
					result.Role = @"GroupMembership.Role";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(Account != _unmodified_Account)
							return true;
						if(Group != _unmodified_Group)
							return true;
						if(Role != _unmodified_Role)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(GroupMembership sourceObject)
				{
					Account = sourceObject.Account;
					Group = sourceObject.Group;
					Role = sourceObject.Role;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_Account = Account;
					_unmodified_Group = Group;
					_unmodified_Role = Role;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Account":
							Account = value;
							break;
						case "Group":
							Group = value;
							break;
						case "Role":
							Role = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class ContentPackageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ContentPackageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ContentPackageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ContentPackageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ContentPackageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ContentPackageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ContentPackageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ContentPackageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ContentPackageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ContentPackageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((ContentPackageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ContentPackageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ContentPackageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ContentPackageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<ContentPackage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(ContentPackage));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (ContentPackage) obj));
            
				}

				public static async Task<ContentPackageCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<ContentPackageCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/ContentPackageCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static ContentPackageCollection CreateDefault()
				{
					var result = new ContentPackageCollection();
					return result;
				}

				/*
				public static ContentPackageCollection CreateDemoDefault()
				{
					ContentPackageCollection customDemo = null;
					ContentPackageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ContentPackageCollection();
					result.CollectionContent.Add(ContentPackage.CreateDemoDefault());
					//result.CollectionContent.Add(ContentPackage.CreateDemoDefault());
					//result.CollectionContent.Add(ContentPackage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (ContentPackage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(ContentPackageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class ContentPackage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ContentPackage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ContentPackage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ContentPackage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ContentPackage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ContentPackage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ContentPackage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ContentPackage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ContentPackage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ContentPackage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((ContentPackage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ContentPackage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ContentPackage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ContentPackage customDemoObject);



				public static ContentPackage CreateDefault()
				{
					var result = new ContentPackage();
					return result;
				}
				/*
				public static ContentPackage CreateDemoDefault()
				{
					ContentPackage customDemo = null;
					ContentPackage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ContentPackage();
					result.PackageType = @"ContentPackage.PackageType";

					result.PackageName = @"ContentPackage.PackageName";

					result.Description = @"ContentPackage.Description
ContentPackage.Description
ContentPackage.Description
ContentPackage.Description
ContentPackage.Description
";

					result.PackageRootFolder = @"ContentPackage.PackageRootFolder";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(PackageType != _unmodified_PackageType)
							return true;
						if(PackageName != _unmodified_PackageName)
							return true;
						if(Description != _unmodified_Description)
							return true;
						if(PackageRootFolder != _unmodified_PackageRootFolder)
							return true;
						if(CreationTime != _unmodified_CreationTime)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(ContentPackage sourceObject)
				{
					PackageType = sourceObject.PackageType;
					PackageName = sourceObject.PackageName;
					Description = sourceObject.Description;
					PackageRootFolder = sourceObject.PackageRootFolder;
					CreationTime = sourceObject.CreationTime;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_PackageType = PackageType;
					_unmodified_PackageName = PackageName;
					_unmodified_Description = Description;
					_unmodified_PackageRootFolder = PackageRootFolder;
					_unmodified_CreationTime = CreationTime;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "PackageType":
							PackageType = value;
							break;
						case "PackageName":
							PackageName = value;
							break;
						case "Description":
							Description = value;
							break;
						case "PackageRootFolder":
							PackageRootFolder = value;
							break;
						case "CreationTime":
							CreationTime = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InformationInputCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InformationInputCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationInputCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InformationInputCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InformationInputCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InformationInputCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationInputCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InformationInputCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InformationInputCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InformationInputCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InformationInputCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InformationInputCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InformationInputCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationInputCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InformationInput>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(InformationInput));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (InformationInput) obj));
            
				}

				public static async Task<InformationInputCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<InformationInputCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/InformationInputCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InformationInputCollection CreateDefault()
				{
					var result = new InformationInputCollection();
					return result;
				}

				/*
				public static InformationInputCollection CreateDemoDefault()
				{
					InformationInputCollection customDemo = null;
					InformationInputCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InformationInputCollection();
					result.CollectionContent.Add(InformationInput.CreateDemoDefault());
					//result.CollectionContent.Add(InformationInput.CreateDemoDefault());
					//result.CollectionContent.Add(InformationInput.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InformationInput )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InformationInputCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InformationInput : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InformationInput()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationInput";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InformationInput/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InformationInput), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InformationInput>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationInput");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InformationInput), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InformationInput/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InformationInput/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InformationInput) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InformationInput", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InformationInput", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationInput customDemoObject);



				public static InformationInput CreateDefault()
				{
					var result = new InformationInput();
					return result;
				}
				/*
				public static InformationInput CreateDemoDefault()
				{
					InformationInput customDemo = null;
					InformationInput.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InformationInput();
					result.InputDescription = @"InformationInput.InputDescription";

					result.LocationURL = @"InformationInput.LocationURL";

					result.LocalContentName = @"InformationInput.LocalContentName";

					result.AuthenticatedDeviceID = @"InformationInput.AuthenticatedDeviceID
InformationInput.AuthenticatedDeviceID
InformationInput.AuthenticatedDeviceID
InformationInput.AuthenticatedDeviceID
InformationInput.AuthenticatedDeviceID
";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(InputDescription != _unmodified_InputDescription)
							return true;
						if(LocationURL != _unmodified_LocationURL)
							return true;
						if(LocalContentName != _unmodified_LocalContentName)
							return true;
						if(AuthenticatedDeviceID != _unmodified_AuthenticatedDeviceID)
							return true;
						if(IsValidatedAndActive != _unmodified_IsValidatedAndActive)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InformationInput sourceObject)
				{
					InputDescription = sourceObject.InputDescription;
					LocationURL = sourceObject.LocationURL;
					LocalContentName = sourceObject.LocalContentName;
					AuthenticatedDeviceID = sourceObject.AuthenticatedDeviceID;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_InputDescription = InputDescription;
					_unmodified_LocationURL = LocationURL;
					_unmodified_LocalContentName = LocalContentName;
					_unmodified_AuthenticatedDeviceID = AuthenticatedDeviceID;
					_unmodified_IsValidatedAndActive = IsValidatedAndActive;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InputDescription":
							InputDescription = value;
							break;
						case "LocationURL":
							LocationURL = value;
							break;
						case "LocalContentName":
							LocalContentName = value;
							break;
						case "AuthenticatedDeviceID":
							AuthenticatedDeviceID = value;
							break;
						case "IsValidatedAndActive":
							IsValidatedAndActive = bool.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InformationOutputCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InformationOutputCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOutputCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InformationOutputCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InformationOutputCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InformationOutputCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationOutputCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InformationOutputCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InformationOutputCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InformationOutputCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InformationOutputCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InformationOutputCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InformationOutputCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationOutputCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InformationOutput>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(InformationOutput));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (InformationOutput) obj));
            
				}

				public static async Task<InformationOutputCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<InformationOutputCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/InformationOutputCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InformationOutputCollection CreateDefault()
				{
					var result = new InformationOutputCollection();
					return result;
				}

				/*
				public static InformationOutputCollection CreateDemoDefault()
				{
					InformationOutputCollection customDemo = null;
					InformationOutputCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InformationOutputCollection();
					result.CollectionContent.Add(InformationOutput.CreateDemoDefault());
					//result.CollectionContent.Add(InformationOutput.CreateDemoDefault());
					//result.CollectionContent.Add(InformationOutput.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InformationOutput )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InformationOutputCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InformationOutput : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InformationOutput()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOutput";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InformationOutput/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InformationOutput), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InformationOutput>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationOutput");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InformationOutput), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InformationOutput/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InformationOutput/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InformationOutput) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InformationOutput", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InformationOutput", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationOutput customDemoObject);



				public static InformationOutput CreateDefault()
				{
					var result = new InformationOutput();
					return result;
				}
				/*
				public static InformationOutput CreateDemoDefault()
				{
					InformationOutput customDemo = null;
					InformationOutput.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InformationOutput();
					result.OutputDescription = @"InformationOutput.OutputDescription";

					result.DestinationURL = @"InformationOutput.DestinationURL";

					result.DestinationContentName = @"InformationOutput.DestinationContentName";

					result.LocalContentURL = @"InformationOutput.LocalContentURL";

					result.AuthenticatedDeviceID = @"InformationOutput.AuthenticatedDeviceID";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OutputDescription != _unmodified_OutputDescription)
							return true;
						if(DestinationURL != _unmodified_DestinationURL)
							return true;
						if(DestinationContentName != _unmodified_DestinationContentName)
							return true;
						if(LocalContentURL != _unmodified_LocalContentURL)
							return true;
						if(AuthenticatedDeviceID != _unmodified_AuthenticatedDeviceID)
							return true;
						if(IsValidatedAndActive != _unmodified_IsValidatedAndActive)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InformationOutput sourceObject)
				{
					OutputDescription = sourceObject.OutputDescription;
					DestinationURL = sourceObject.DestinationURL;
					DestinationContentName = sourceObject.DestinationContentName;
					LocalContentURL = sourceObject.LocalContentURL;
					AuthenticatedDeviceID = sourceObject.AuthenticatedDeviceID;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_OutputDescription = OutputDescription;
					_unmodified_DestinationURL = DestinationURL;
					_unmodified_DestinationContentName = DestinationContentName;
					_unmodified_LocalContentURL = LocalContentURL;
					_unmodified_AuthenticatedDeviceID = AuthenticatedDeviceID;
					_unmodified_IsValidatedAndActive = IsValidatedAndActive;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OutputDescription":
							OutputDescription = value;
							break;
						case "DestinationURL":
							DestinationURL = value;
							break;
						case "DestinationContentName":
							DestinationContentName = value;
							break;
						case "LocalContentURL":
							LocalContentURL = value;
							break;
						case "AuthenticatedDeviceID":
							AuthenticatedDeviceID = value;
							break;
						case "IsValidatedAndActive":
							IsValidatedAndActive = bool.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class AuthenticatedAsActiveDeviceCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public AuthenticatedAsActiveDeviceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "AuthenticatedAsActiveDeviceCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/AuthenticatedAsActiveDeviceCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(AuthenticatedAsActiveDeviceCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<AuthenticatedAsActiveDeviceCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: AuthenticatedAsActiveDeviceCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(AuthenticatedAsActiveDeviceCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/AuthenticatedAsActiveDeviceCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/AuthenticatedAsActiveDeviceCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((AuthenticatedAsActiveDeviceCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "AuthenticatedAsActiveDeviceCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "AuthenticatedAsActiveDeviceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref AuthenticatedAsActiveDeviceCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<AuthenticatedAsActiveDevice>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(AuthenticatedAsActiveDevice));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (AuthenticatedAsActiveDevice) obj));
            
				}

				public static async Task<AuthenticatedAsActiveDeviceCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<AuthenticatedAsActiveDeviceCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/AuthenticatedAsActiveDeviceCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static AuthenticatedAsActiveDeviceCollection CreateDefault()
				{
					var result = new AuthenticatedAsActiveDeviceCollection();
					return result;
				}

				/*
				public static AuthenticatedAsActiveDeviceCollection CreateDemoDefault()
				{
					AuthenticatedAsActiveDeviceCollection customDemo = null;
					AuthenticatedAsActiveDeviceCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new AuthenticatedAsActiveDeviceCollection();
					result.CollectionContent.Add(AuthenticatedAsActiveDevice.CreateDemoDefault());
					//result.CollectionContent.Add(AuthenticatedAsActiveDevice.CreateDemoDefault());
					//result.CollectionContent.Add(AuthenticatedAsActiveDevice.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (AuthenticatedAsActiveDevice )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(AuthenticatedAsActiveDeviceCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class AuthenticatedAsActiveDevice : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public AuthenticatedAsActiveDevice()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "AuthenticatedAsActiveDevice";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/AuthenticatedAsActiveDevice/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(AuthenticatedAsActiveDevice), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<AuthenticatedAsActiveDevice>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: AuthenticatedAsActiveDevice");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(AuthenticatedAsActiveDevice), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/AuthenticatedAsActiveDevice/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/AuthenticatedAsActiveDevice/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((AuthenticatedAsActiveDevice) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "AuthenticatedAsActiveDevice", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "AuthenticatedAsActiveDevice", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref AuthenticatedAsActiveDevice customDemoObject);



				public static AuthenticatedAsActiveDevice CreateDefault()
				{
					var result = new AuthenticatedAsActiveDevice();
					return result;
				}
				/*
				public static AuthenticatedAsActiveDevice CreateDemoDefault()
				{
					AuthenticatedAsActiveDevice customDemo = null;
					AuthenticatedAsActiveDevice.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new AuthenticatedAsActiveDevice();
					result.AuthenticationDescription = @"AuthenticatedAsActiveDevice.AuthenticationDescription";

					result.SharedSecret = @"AuthenticatedAsActiveDevice.SharedSecret";

					result.EstablishedTrustID = @"AuthenticatedAsActiveDevice.EstablishedTrustID";

					result.NegotiationURL = @"AuthenticatedAsActiveDevice.NegotiationURL";

					result.ConnectionURL = @"AuthenticatedAsActiveDevice.ConnectionURL";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(AuthenticationDescription != _unmodified_AuthenticationDescription)
							return true;
						if(SharedSecret != _unmodified_SharedSecret)
							return true;
						if(ActiveSymmetricAESKey != _unmodified_ActiveSymmetricAESKey)
							return true;
						if(EstablishedTrustID != _unmodified_EstablishedTrustID)
							return true;
						if(IsValidatedAndActive != _unmodified_IsValidatedAndActive)
							return true;
						if(NegotiationURL != _unmodified_NegotiationURL)
							return true;
						if(ConnectionURL != _unmodified_ConnectionURL)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_AuthenticationDescription = AuthenticationDescription;
					_unmodified_SharedSecret = SharedSecret;
					_unmodified_ActiveSymmetricAESKey = ActiveSymmetricAESKey;
					_unmodified_EstablishedTrustID = EstablishedTrustID;
					_unmodified_IsValidatedAndActive = IsValidatedAndActive;
					_unmodified_NegotiationURL = NegotiationURL;
					_unmodified_ConnectionURL = ConnectionURL;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "AuthenticationDescription":
							AuthenticationDescription = value;
							break;
						case "SharedSecret":
							SharedSecret = value;
							break;
						case "EstablishedTrustID":
							EstablishedTrustID = value;
							break;
						case "IsValidatedAndActive":
							IsValidatedAndActive = bool.Parse(value);
							break;
						case "NegotiationURL":
							NegotiationURL = value;
							break;
						case "ConnectionURL":
							ConnectionURL = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class DeviceMembershipCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public DeviceMembershipCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "DeviceMembershipCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/DeviceMembershipCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(DeviceMembershipCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<DeviceMembershipCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: DeviceMembershipCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(DeviceMembershipCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/DeviceMembershipCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/DeviceMembershipCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((DeviceMembershipCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "DeviceMembershipCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "DeviceMembershipCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref DeviceMembershipCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<DeviceMembership>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(DeviceMembership));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (DeviceMembership) obj));
            
				}

				public static async Task<DeviceMembershipCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<DeviceMembershipCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/DeviceMembershipCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static DeviceMembershipCollection CreateDefault()
				{
					var result = new DeviceMembershipCollection();
					return result;
				}

				/*
				public static DeviceMembershipCollection CreateDemoDefault()
				{
					DeviceMembershipCollection customDemo = null;
					DeviceMembershipCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new DeviceMembershipCollection();
					result.CollectionContent.Add(DeviceMembership.CreateDemoDefault());
					//result.CollectionContent.Add(DeviceMembership.CreateDemoDefault());
					//result.CollectionContent.Add(DeviceMembership.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (DeviceMembership )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(DeviceMembershipCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class DeviceMembership : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public DeviceMembership()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "DeviceMembership";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/DeviceMembership/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(DeviceMembership), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<DeviceMembership>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: DeviceMembership");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(DeviceMembership), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/DeviceMembership/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/DeviceMembership/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((DeviceMembership) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "DeviceMembership", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "DeviceMembership", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref DeviceMembership customDemoObject);



				public static DeviceMembership CreateDefault()
				{
					var result = new DeviceMembership();
					return result;
				}
				/*
				public static DeviceMembership CreateDemoDefault()
				{
					DeviceMembership customDemo = null;
					DeviceMembership.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new DeviceMembership();
					result.DeviceDescription = @"DeviceMembership.DeviceDescription";

					result.SharedSecret = @"DeviceMembership.SharedSecret";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(DeviceDescription != _unmodified_DeviceDescription)
							return true;
						if(SharedSecret != _unmodified_SharedSecret)
							return true;
						if(ActiveSymmetricAESKey != _unmodified_ActiveSymmetricAESKey)
							return true;
						if(IsValidatedAndActive != _unmodified_IsValidatedAndActive)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(DeviceMembership sourceObject)
				{
					DeviceDescription = sourceObject.DeviceDescription;
					SharedSecret = sourceObject.SharedSecret;
					ActiveSymmetricAESKey = sourceObject.ActiveSymmetricAESKey;
					IsValidatedAndActive = sourceObject.IsValidatedAndActive;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_DeviceDescription = DeviceDescription;
					_unmodified_SharedSecret = SharedSecret;
					_unmodified_ActiveSymmetricAESKey = ActiveSymmetricAESKey;
					_unmodified_IsValidatedAndActive = IsValidatedAndActive;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "DeviceDescription":
							DeviceDescription = value;
							break;
						case "SharedSecret":
							SharedSecret = value;
							break;
						case "IsValidatedAndActive":
							IsValidatedAndActive = bool.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceFiscalExportSummary : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceFiscalExportSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceFiscalExportSummary";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceFiscalExportSummary/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceFiscalExportSummary), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceFiscalExportSummary>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceFiscalExportSummary");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceFiscalExportSummary), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceFiscalExportSummary/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceFiscalExportSummary/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceFiscalExportSummary) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceFiscalExportSummary", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceFiscalExportSummary", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceFiscalExportSummary customDemoObject);



				public static InvoiceFiscalExportSummary CreateDefault()
				{
					var result = new InvoiceFiscalExportSummary();
					result.ExportedInvoices = InvoiceCollection.CreateDefault();
					return result;
				}
				/*
				public static InvoiceFiscalExportSummary CreateDemoDefault()
				{
					InvoiceFiscalExportSummary customDemo = null;
					InvoiceFiscalExportSummary.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceFiscalExportSummary();
					result.ExportedInvoices = InvoiceCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(masterInstance is InvoiceCollection) {
						TheBall.CORE.CollectionUpdateImplementation.Update_InvoiceFiscalExportSummary_ExportedInvoices(this, localCollection:ExportedInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(ExportedInvoices != null) {
						((IInformationObject) ExportedInvoices).UpdateCollections(masterInstance);
					}
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = ExportedInvoices;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = ExportedInvoices;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) ExportedInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(FiscalInclusiveStartDate != _unmodified_FiscalInclusiveStartDate)
							return true;
						if(FiscalInclusiveEndDate != _unmodified_FiscalInclusiveEndDate)
							return true;
						if(ExportedInvoices != _unmodified_ExportedInvoices)
							return true;
						{
							IInformationObject item = (IInformationObject) ExportedInvoices;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(ExportedInvoices != null) {
						if(ExportedInvoices.ID == replacingObject.ID)
							ExportedInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = ExportedInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceFiscalExportSummary sourceObject)
				{
					FiscalInclusiveStartDate = sourceObject.FiscalInclusiveStartDate;
					FiscalInclusiveEndDate = sourceObject.FiscalInclusiveEndDate;
					ExportedInvoices = sourceObject.ExportedInvoices;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_FiscalInclusiveStartDate = FiscalInclusiveStartDate;
					_unmodified_FiscalInclusiveEndDate = FiscalInclusiveEndDate;
				
					_unmodified_ExportedInvoices = ExportedInvoices;
					if(ExportedInvoices != null)
						((IInformationObject) ExportedInvoices).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "FiscalInclusiveStartDate":
							FiscalInclusiveStartDate = DateTime.Parse(value);
							break;
						case "FiscalInclusiveEndDate":
							FiscalInclusiveEndDate = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceSummaryContainer : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceSummaryContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceSummaryContainer";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceSummaryContainer/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceSummaryContainer), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceSummaryContainer>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceSummaryContainer");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceSummaryContainer), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceSummaryContainer/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceSummaryContainer/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceSummaryContainer) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceSummaryContainer", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceSummaryContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceSummaryContainer customDemoObject);



				public static InvoiceSummaryContainer CreateDefault()
				{
					var result = new InvoiceSummaryContainer();
					result.OpenInvoices = InvoiceCollection.CreateDefault();
					result.PredictedInvoices = InvoiceCollection.CreateDefault();
					result.PaidInvoicesActiveYear = InvoiceCollection.CreateDefault();
					result.PaidInvoicesLast12Months = InvoiceCollection.CreateDefault();
					return result;
				}
				/*
				public static InvoiceSummaryContainer CreateDemoDefault()
				{
					InvoiceSummaryContainer customDemo = null;
					InvoiceSummaryContainer.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceSummaryContainer();
					result.OpenInvoices = InvoiceCollection.CreateDemoDefault();
					result.PredictedInvoices = InvoiceCollection.CreateDemoDefault();
					result.PaidInvoicesActiveYear = InvoiceCollection.CreateDemoDefault();
					result.PaidInvoicesLast12Months = InvoiceCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(masterInstance is InvoiceCollection) {
						TheBall.CORE.CollectionUpdateImplementation.Update_InvoiceSummaryContainer_OpenInvoices(this, localCollection:OpenInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(OpenInvoices != null) {
						((IInformationObject) OpenInvoices).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						TheBall.CORE.CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PredictedInvoices(this, localCollection:PredictedInvoices, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PredictedInvoices != null) {
						((IInformationObject) PredictedInvoices).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						TheBall.CORE.CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PaidInvoicesActiveYear(this, localCollection:PaidInvoicesActiveYear, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PaidInvoicesActiveYear != null) {
						((IInformationObject) PaidInvoicesActiveYear).UpdateCollections(masterInstance);
					}
					if(masterInstance is InvoiceCollection) {
						TheBall.CORE.CollectionUpdateImplementation.Update_InvoiceSummaryContainer_PaidInvoicesLast12Months(this, localCollection:PaidInvoicesLast12Months, masterCollection:(InvoiceCollection) masterInstance);
					} else if(PaidInvoicesLast12Months != null) {
						((IInformationObject) PaidInvoicesLast12Months).UpdateCollections(masterInstance);
					}
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = OpenInvoices;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PredictedInvoices;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PaidInvoicesActiveYear;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = PaidInvoicesLast12Months;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = OpenInvoices;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PredictedInvoices;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PaidInvoicesActiveYear;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = PaidInvoicesLast12Months;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) OpenInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PredictedInvoices;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PaidInvoicesActiveYear;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) PaidInvoicesLast12Months;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OpenInvoices != _unmodified_OpenInvoices)
							return true;
						if(PredictedInvoices != _unmodified_PredictedInvoices)
							return true;
						if(PaidInvoicesActiveYear != _unmodified_PaidInvoicesActiveYear)
							return true;
						if(PaidInvoicesLast12Months != _unmodified_PaidInvoicesLast12Months)
							return true;
						{
							IInformationObject item = (IInformationObject) OpenInvoices;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PredictedInvoices;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PaidInvoicesActiveYear;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) PaidInvoicesLast12Months;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(OpenInvoices != null) {
						if(OpenInvoices.ID == replacingObject.ID)
							OpenInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = OpenInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PredictedInvoices != null) {
						if(PredictedInvoices.ID == replacingObject.ID)
							PredictedInvoices = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PredictedInvoices;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PaidInvoicesActiveYear != null) {
						if(PaidInvoicesActiveYear.ID == replacingObject.ID)
							PaidInvoicesActiveYear = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PaidInvoicesActiveYear;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(PaidInvoicesLast12Months != null) {
						if(PaidInvoicesLast12Months.ID == replacingObject.ID)
							PaidInvoicesLast12Months = (InvoiceCollection) replacingObject;
						else {
							IInformationObject iObject = PaidInvoicesLast12Months;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceSummaryContainer sourceObject)
				{
					OpenInvoices = sourceObject.OpenInvoices;
					PredictedInvoices = sourceObject.PredictedInvoices;
					PaidInvoicesActiveYear = sourceObject.PaidInvoicesActiveYear;
					PaidInvoicesLast12Months = sourceObject.PaidInvoicesLast12Months;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
				
					_unmodified_OpenInvoices = OpenInvoices;
					if(OpenInvoices != null)
						((IInformationObject) OpenInvoices).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PredictedInvoices = PredictedInvoices;
					if(PredictedInvoices != null)
						((IInformationObject) PredictedInvoices).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PaidInvoicesActiveYear = PaidInvoicesActiveYear;
					if(PaidInvoicesActiveYear != null)
						((IInformationObject) PaidInvoicesActiveYear).SetInstanceTreeValuesAsUnmodified();

					_unmodified_PaidInvoicesLast12Months = PaidInvoicesLast12Months;
					if(PaidInvoicesLast12Months != null)
						((IInformationObject) PaidInvoicesLast12Months).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return true;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return GetMasterCollectionLocation(owner);
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					var owner = VirtualOwner.FigureOwner(this);
					return await GetMasterCollectionInstanceAsync(owner);
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<Invoice>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
					// DirectoryToMaster
					string itemDirectory = GetItemDirectory();
					IInformationObject[] informationObjects = await StorageSupport.RetrieveInformationObjectsAsync(itemDirectory,
																								 typeof(Invoice));
                    Array.ForEach(informationObjects, io => io.MasterETag = io.ETag);
					CollectionContent.Clear();
					CollectionContent.AddRange(informationObjects.Select(obj => (Invoice) obj));
            
				}

				public static async Task<InvoiceCollection> GetMasterCollectionInstanceAsync(IContainerOwner owner)
				{
					return await ObjectStorage.RetrieveFromOwnerContentA<InvoiceCollection>(owner, "MasterCollection");
				}
				public static string GetMasterCollectionLocation(IContainerOwner owner)
				{
					return StorageSupport.GetOwnerContentLocation(owner, "TheBall.CORE/InvoiceCollection/" + "MasterCollection");
				}



                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceCollection CreateDefault()
				{
					var result = new InvoiceCollection();
					return result;
				}

				/*
				public static InvoiceCollection CreateDemoDefault()
				{
					InvoiceCollection customDemo = null;
					InvoiceCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceCollection();
					result.CollectionContent.Add(Invoice.CreateDemoDefault());
					//result.CollectionContent.Add(Invoice.CreateDemoDefault());
					//result.CollectionContent.Add(Invoice.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (Invoice )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class Invoice : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Invoice()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Invoice";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Invoice/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Invoice), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Invoice>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Invoice");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Invoice), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Invoice/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Invoice/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((Invoice) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Invoice", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Invoice", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Invoice customDemoObject);



				public static Invoice CreateDefault()
				{
					var result = new Invoice();
					result.InvoiceDetails = InvoiceDetails.CreateDefault();
					result.InvoiceUsers = InvoiceUserCollection.CreateDefault();
					return result;
				}
				/*
				public static Invoice CreateDemoDefault()
				{
					Invoice customDemo = null;
					Invoice.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Invoice();
					result.InvoiceName = @"Invoice.InvoiceName";

					result.InvoiceID = @"Invoice.InvoiceID";

					result.InvoicedAmount = @"Invoice.InvoicedAmount";

					result.PaidAmount = @"Invoice.PaidAmount";

					result.FeesAndInterestAmount = @"Invoice.FeesAndInterestAmount";

					result.UnpaidAmount = @"Invoice.UnpaidAmount";

					result.InvoiceDetails = InvoiceDetails.CreateDemoDefault();
					result.InvoiceUsers = InvoiceUserCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceDetails != null) {
						((IInformationObject) InvoiceDetails).UpdateCollections(masterInstance);
					}

					if(InvoiceUsers != null) {
						((IInformationObject) InvoiceUsers).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceDetails;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceUsers;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = InvoiceDetails;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = InvoiceUsers;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) InvoiceDetails;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) InvoiceUsers;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(InvoiceName != _unmodified_InvoiceName)
							return true;
						if(InvoiceID != _unmodified_InvoiceID)
							return true;
						if(InvoicedAmount != _unmodified_InvoicedAmount)
							return true;
						if(CreateDate != _unmodified_CreateDate)
							return true;
						if(DueDate != _unmodified_DueDate)
							return true;
						if(PaidAmount != _unmodified_PaidAmount)
							return true;
						if(FeesAndInterestAmount != _unmodified_FeesAndInterestAmount)
							return true;
						if(UnpaidAmount != _unmodified_UnpaidAmount)
							return true;
						if(InvoiceDetails != _unmodified_InvoiceDetails)
							return true;
						if(InvoiceUsers != _unmodified_InvoiceUsers)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceDetails;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) InvoiceUsers;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(InvoiceDetails != null) {
						if(InvoiceDetails.ID == replacingObject.ID)
							InvoiceDetails = (InvoiceDetails) replacingObject;
						else {
							IInformationObject iObject = InvoiceDetails;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(InvoiceUsers != null) {
						if(InvoiceUsers.ID == replacingObject.ID)
							InvoiceUsers = (InvoiceUserCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceUsers;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_InvoiceName = InvoiceName;
					_unmodified_InvoiceID = InvoiceID;
					_unmodified_InvoicedAmount = InvoicedAmount;
					_unmodified_CreateDate = CreateDate;
					_unmodified_DueDate = DueDate;
					_unmodified_PaidAmount = PaidAmount;
					_unmodified_FeesAndInterestAmount = FeesAndInterestAmount;
					_unmodified_UnpaidAmount = UnpaidAmount;
				
					_unmodified_InvoiceDetails = InvoiceDetails;
					if(InvoiceDetails != null)
						((IInformationObject) InvoiceDetails).SetInstanceTreeValuesAsUnmodified();

					_unmodified_InvoiceUsers = InvoiceUsers;
					if(InvoiceUsers != null)
						((IInformationObject) InvoiceUsers).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "InvoiceName":
							InvoiceName = value;
							break;
						case "InvoiceID":
							InvoiceID = value;
							break;
						case "InvoicedAmount":
							InvoicedAmount = value;
							break;
						case "CreateDate":
							CreateDate = DateTime.Parse(value);
							break;
						case "DueDate":
							DueDate = DateTime.Parse(value);
							break;
						case "PaidAmount":
							PaidAmount = value;
							break;
						case "FeesAndInterestAmount":
							FeesAndInterestAmount = value;
							break;
						case "UnpaidAmount":
							UnpaidAmount = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceDetails : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceDetails()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceDetails";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceDetails/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceDetails), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceDetails>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceDetails");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceDetails), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceDetails/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceDetails/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceDetails) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceDetails", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceDetails", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceDetails customDemoObject);



				public static InvoiceDetails CreateDefault()
				{
					var result = new InvoiceDetails();
					return result;
				}
				/*
				public static InvoiceDetails CreateDemoDefault()
				{
					InvoiceDetails customDemo = null;
					InvoiceDetails.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceDetails();
					result.MonthlyFeesTotal = @"InvoiceDetails.MonthlyFeesTotal";

					result.OneTimeFeesTotal = @"InvoiceDetails.OneTimeFeesTotal";

					result.UsageFeesTotal = @"InvoiceDetails.UsageFeesTotal";

					result.InterestFeesTotal = @"InvoiceDetails.InterestFeesTotal";

					result.PenaltyFeesTotal = @"InvoiceDetails.PenaltyFeesTotal";

					result.TotalFeesTotal = @"InvoiceDetails.TotalFeesTotal";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(MonthlyFeesTotal != _unmodified_MonthlyFeesTotal)
							return true;
						if(OneTimeFeesTotal != _unmodified_OneTimeFeesTotal)
							return true;
						if(UsageFeesTotal != _unmodified_UsageFeesTotal)
							return true;
						if(InterestFeesTotal != _unmodified_InterestFeesTotal)
							return true;
						if(PenaltyFeesTotal != _unmodified_PenaltyFeesTotal)
							return true;
						if(TotalFeesTotal != _unmodified_TotalFeesTotal)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InvoiceDetails sourceObject)
				{
					MonthlyFeesTotal = sourceObject.MonthlyFeesTotal;
					OneTimeFeesTotal = sourceObject.OneTimeFeesTotal;
					UsageFeesTotal = sourceObject.UsageFeesTotal;
					InterestFeesTotal = sourceObject.InterestFeesTotal;
					PenaltyFeesTotal = sourceObject.PenaltyFeesTotal;
					TotalFeesTotal = sourceObject.TotalFeesTotal;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_MonthlyFeesTotal = MonthlyFeesTotal;
					_unmodified_OneTimeFeesTotal = OneTimeFeesTotal;
					_unmodified_UsageFeesTotal = UsageFeesTotal;
					_unmodified_InterestFeesTotal = InterestFeesTotal;
					_unmodified_PenaltyFeesTotal = PenaltyFeesTotal;
					_unmodified_TotalFeesTotal = TotalFeesTotal;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "MonthlyFeesTotal":
							MonthlyFeesTotal = value;
							break;
						case "OneTimeFeesTotal":
							OneTimeFeesTotal = value;
							break;
						case "UsageFeesTotal":
							UsageFeesTotal = value;
							break;
						case "InterestFeesTotal":
							InterestFeesTotal = value;
							break;
						case "PenaltyFeesTotal":
							PenaltyFeesTotal = value;
							break;
						case "TotalFeesTotal":
							TotalFeesTotal = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceUserCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceUserCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUserCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceUserCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceUserCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceUserCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceUserCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceUserCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceUserCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceUserCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceUserCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceUserCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceUserCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceUserCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceUser>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceUserCollection CreateDefault()
				{
					var result = new InvoiceUserCollection();
					return result;
				}

				/*
				public static InvoiceUserCollection CreateDemoDefault()
				{
					InvoiceUserCollection customDemo = null;
					InvoiceUserCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceUserCollection();
					result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceUser.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InvoiceUser )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceUserCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceUser : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceUser()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceUser";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceUser/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceUser), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceUser>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceUser");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceUser), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceUser/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceUser/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceUser) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceUser", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceUser", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceUser customDemoObject);



				public static InvoiceUser CreateDefault()
				{
					var result = new InvoiceUser();
					result.InvoiceRowGroupCollection = InvoiceRowGroupCollection.CreateDefault();
					result.InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection.CreateDefault();
					return result;
				}
				/*
				public static InvoiceUser CreateDemoDefault()
				{
					InvoiceUser customDemo = null;
					InvoiceUser.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceUser();
					result.UserName = @"InvoiceUser.UserName";

					result.UserID = @"InvoiceUser.UserID";

					result.UserPhoneNumber = @"InvoiceUser.UserPhoneNumber";

					result.UserSubscriptionNumber = @"InvoiceUser.UserSubscriptionNumber";

					result.UserInvoiceTotalAmount = @"InvoiceUser.UserInvoiceTotalAmount";

					result.InvoiceRowGroupCollection = InvoiceRowGroupCollection.CreateDemoDefault();
					result.InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceRowGroupCollection != null) {
						((IInformationObject) InvoiceRowGroupCollection).UpdateCollections(masterInstance);
					}

					if(InvoiceEventDetailGroupCollection != null) {
						((IInformationObject) InvoiceEventDetailGroupCollection).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceRowGroupCollection;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceEventDetailGroupCollection;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = InvoiceRowGroupCollection;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = InvoiceEventDetailGroupCollection;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) InvoiceRowGroupCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) InvoiceEventDetailGroupCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(UserName != _unmodified_UserName)
							return true;
						if(UserID != _unmodified_UserID)
							return true;
						if(UserPhoneNumber != _unmodified_UserPhoneNumber)
							return true;
						if(UserSubscriptionNumber != _unmodified_UserSubscriptionNumber)
							return true;
						if(UserInvoiceTotalAmount != _unmodified_UserInvoiceTotalAmount)
							return true;
						if(InvoiceRowGroupCollection != _unmodified_InvoiceRowGroupCollection)
							return true;
						if(InvoiceEventDetailGroupCollection != _unmodified_InvoiceEventDetailGroupCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceRowGroupCollection;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) InvoiceEventDetailGroupCollection;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(InvoiceRowGroupCollection != null) {
						if(InvoiceRowGroupCollection.ID == replacingObject.ID)
							InvoiceRowGroupCollection = (InvoiceRowGroupCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceRowGroupCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(InvoiceEventDetailGroupCollection != null) {
						if(InvoiceEventDetailGroupCollection.ID == replacingObject.ID)
							InvoiceEventDetailGroupCollection = (InvoiceEventDetailGroupCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceEventDetailGroupCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UserName = UserName;
					_unmodified_UserID = UserID;
					_unmodified_UserPhoneNumber = UserPhoneNumber;
					_unmodified_UserSubscriptionNumber = UserSubscriptionNumber;
					_unmodified_UserInvoiceTotalAmount = UserInvoiceTotalAmount;
				
					_unmodified_InvoiceRowGroupCollection = InvoiceRowGroupCollection;
					if(InvoiceRowGroupCollection != null)
						((IInformationObject) InvoiceRowGroupCollection).SetInstanceTreeValuesAsUnmodified();

					_unmodified_InvoiceEventDetailGroupCollection = InvoiceEventDetailGroupCollection;
					if(InvoiceEventDetailGroupCollection != null)
						((IInformationObject) InvoiceEventDetailGroupCollection).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UserName":
							UserName = value;
							break;
						case "UserID":
							UserID = value;
							break;
						case "UserPhoneNumber":
							UserPhoneNumber = value;
							break;
						case "UserSubscriptionNumber":
							UserSubscriptionNumber = value;
							break;
						case "UserInvoiceTotalAmount":
							UserInvoiceTotalAmount = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceRowGroupCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceRowGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroupCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowGroupCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceRowGroupCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRowGroupCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowGroupCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceRowGroupCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceRowGroupCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceRowGroupCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceRowGroupCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceRowGroupCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowGroupCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRowGroup>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceRowGroupCollection CreateDefault()
				{
					var result = new InvoiceRowGroupCollection();
					return result;
				}

				/*
				public static InvoiceRowGroupCollection CreateDemoDefault()
				{
					InvoiceRowGroupCollection customDemo = null;
					InvoiceRowGroupCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowGroupCollection();
					result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRowGroup.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InvoiceRowGroup )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceRowGroupCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceEventDetailGroupCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceEventDetailGroupCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroupCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailGroupCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceEventDetailGroupCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetailGroupCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailGroupCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceEventDetailGroupCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceEventDetailGroupCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceEventDetailGroupCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroupCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailGroupCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailGroupCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetailGroup>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceEventDetailGroupCollection CreateDefault()
				{
					var result = new InvoiceEventDetailGroupCollection();
					return result;
				}

				/*
				public static InvoiceEventDetailGroupCollection CreateDemoDefault()
				{
					InvoiceEventDetailGroupCollection customDemo = null;
					InvoiceEventDetailGroupCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailGroupCollection();
					result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetailGroup.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InvoiceEventDetailGroup )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceEventDetailGroupCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceRowGroup : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceRowGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowGroup";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowGroup/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceRowGroup), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRowGroup>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowGroup");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceRowGroup), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceRowGroup/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceRowGroup/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceRowGroup) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceRowGroup", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowGroup customDemoObject);



				public static InvoiceRowGroup CreateDefault()
				{
					var result = new InvoiceRowGroup();
					result.InvoiceRowCollection = InvoiceRowCollection.CreateDefault();
					return result;
				}
				/*
				public static InvoiceRowGroup CreateDemoDefault()
				{
					InvoiceRowGroup customDemo = null;
					InvoiceRowGroup.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowGroup();
					result.GroupName = @"InvoiceRowGroup.GroupName";

					result.GroupTotalPriceWithoutTaxes = @"InvoiceRowGroup.GroupTotalPriceWithoutTaxes";

					result.GroupTotalTaxes = @"InvoiceRowGroup.GroupTotalTaxes";

					result.GroupTotalPriceWithTaxes = @"InvoiceRowGroup.GroupTotalPriceWithTaxes";

					result.InvoiceRowCollection = InvoiceRowCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceRowCollection != null) {
						((IInformationObject) InvoiceRowCollection).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceRowCollection;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = InvoiceRowCollection;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) InvoiceRowCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(GroupName != _unmodified_GroupName)
							return true;
						if(GroupTotalPriceWithoutTaxes != _unmodified_GroupTotalPriceWithoutTaxes)
							return true;
						if(GroupTotalTaxes != _unmodified_GroupTotalTaxes)
							return true;
						if(GroupTotalPriceWithTaxes != _unmodified_GroupTotalPriceWithTaxes)
							return true;
						if(InvoiceRowCollection != _unmodified_InvoiceRowCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceRowCollection;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(InvoiceRowCollection != null) {
						if(InvoiceRowCollection.ID == replacingObject.ID)
							InvoiceRowCollection = (InvoiceRowCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceRowCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceRowGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					GroupTotalPriceWithoutTaxes = sourceObject.GroupTotalPriceWithoutTaxes;
					GroupTotalTaxes = sourceObject.GroupTotalTaxes;
					GroupTotalPriceWithTaxes = sourceObject.GroupTotalPriceWithTaxes;
					InvoiceRowCollection = sourceObject.InvoiceRowCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_GroupName = GroupName;
					_unmodified_GroupTotalPriceWithoutTaxes = GroupTotalPriceWithoutTaxes;
					_unmodified_GroupTotalTaxes = GroupTotalTaxes;
					_unmodified_GroupTotalPriceWithTaxes = GroupTotalPriceWithTaxes;
				
					_unmodified_InvoiceRowCollection = InvoiceRowCollection;
					if(InvoiceRowCollection != null)
						((IInformationObject) InvoiceRowCollection).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupName":
							GroupName = value;
							break;
						case "GroupTotalPriceWithoutTaxes":
							GroupTotalPriceWithoutTaxes = value;
							break;
						case "GroupTotalTaxes":
							GroupTotalTaxes = value;
							break;
						case "GroupTotalPriceWithTaxes":
							GroupTotalPriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceEventDetailGroup : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceEventDetailGroup()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailGroup";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailGroup/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceEventDetailGroup), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetailGroup>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailGroup");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceEventDetailGroup), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailGroup/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceEventDetailGroup/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceEventDetailGroup) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailGroup", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailGroup", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailGroup customDemoObject);



				public static InvoiceEventDetailGroup CreateDefault()
				{
					var result = new InvoiceEventDetailGroup();
					result.InvoiceEventDetailCollection = InvoiceEventDetailCollection.CreateDefault();
					return result;
				}
				/*
				public static InvoiceEventDetailGroup CreateDemoDefault()
				{
					InvoiceEventDetailGroup customDemo = null;
					InvoiceEventDetailGroup.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailGroup();
					result.GroupName = @"InvoiceEventDetailGroup.GroupName";

					result.InvoiceEventDetailCollection = InvoiceEventDetailCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(InvoiceEventDetailCollection != null) {
						((IInformationObject) InvoiceEventDetailCollection).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = InvoiceEventDetailCollection;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = InvoiceEventDetailCollection;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) InvoiceEventDetailCollection;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(GroupName != _unmodified_GroupName)
							return true;
						if(InvoiceEventDetailCollection != _unmodified_InvoiceEventDetailCollection)
							return true;
						{
							IInformationObject item = (IInformationObject) InvoiceEventDetailCollection;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(InvoiceEventDetailCollection != null) {
						if(InvoiceEventDetailCollection.ID == replacingObject.ID)
							InvoiceEventDetailCollection = (InvoiceEventDetailCollection) replacingObject;
						else {
							IInformationObject iObject = InvoiceEventDetailCollection;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(InvoiceEventDetailGroup sourceObject)
				{
					GroupName = sourceObject.GroupName;
					InvoiceEventDetailCollection = sourceObject.InvoiceEventDetailCollection;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_GroupName = GroupName;
				
					_unmodified_InvoiceEventDetailCollection = InvoiceEventDetailCollection;
					if(InvoiceEventDetailCollection != null)
						((IInformationObject) InvoiceEventDetailCollection).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GroupName":
							GroupName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string GroupName { get; set; }
			private string _unmodified_GroupName;
			[DataMember] 
			public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
			private InvoiceEventDetailCollection _unmodified_InvoiceEventDetailCollection;
			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceEventDetailCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceEventDetailCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetailCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetailCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceEventDetailCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetailCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetailCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceEventDetailCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceEventDetailCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceEventDetailCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceEventDetailCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceEventDetailCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetailCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetailCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetail>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceEventDetailCollection CreateDefault()
				{
					var result = new InvoiceEventDetailCollection();
					return result;
				}

				/*
				public static InvoiceEventDetailCollection CreateDemoDefault()
				{
					InvoiceEventDetailCollection customDemo = null;
					InvoiceEventDetailCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetailCollection();
					result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceEventDetail.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InvoiceEventDetail )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceEventDetailCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceRowCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceRowCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRowCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRowCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceRowCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRowCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRowCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceRowCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceRowCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceRowCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceRowCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceRowCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRowCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRowCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRow>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static InvoiceRowCollection CreateDefault()
				{
					var result = new InvoiceRowCollection();
					return result;
				}

				/*
				public static InvoiceRowCollection CreateDemoDefault()
				{
					InvoiceRowCollection customDemo = null;
					InvoiceRowCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRowCollection();
					result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					//result.CollectionContent.Add(InvoiceRow.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (InvoiceRow )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(InvoiceRowCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class InvoiceEventDetail : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceEventDetail()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceEventDetail";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceEventDetail/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceEventDetail), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceEventDetail>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceEventDetail");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceEventDetail), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceEventDetail/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceEventDetail/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceEventDetail) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceEventDetail", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceEventDetail", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceEventDetail customDemoObject);



				public static InvoiceEventDetail CreateDefault()
				{
					var result = new InvoiceEventDetail();
					return result;
				}
				/*
				public static InvoiceEventDetail CreateDemoDefault()
				{
					InvoiceEventDetail customDemo = null;
					InvoiceEventDetail.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceEventDetail();
					result.IndentMode = @"InvoiceEventDetail.IndentMode";

					result.ReceivingParty = @"InvoiceEventDetail.ReceivingParty";

					result.AmountOfUnits = @"InvoiceEventDetail.AmountOfUnits";

					result.Duration = @"InvoiceEventDetail.Duration";

					result.UnitPrice = @"InvoiceEventDetail.UnitPrice";

					result.PriceWithoutTaxes = @"InvoiceEventDetail.PriceWithoutTaxes";

					result.Taxes = @"InvoiceEventDetail.Taxes";

					result.PriceWithTaxes = @"InvoiceEventDetail.PriceWithTaxes";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(IndentMode != _unmodified_IndentMode)
							return true;
						if(EventStartDateTime != _unmodified_EventStartDateTime)
							return true;
						if(EventEndDateTime != _unmodified_EventEndDateTime)
							return true;
						if(ReceivingParty != _unmodified_ReceivingParty)
							return true;
						if(AmountOfUnits != _unmodified_AmountOfUnits)
							return true;
						if(Duration != _unmodified_Duration)
							return true;
						if(UnitPrice != _unmodified_UnitPrice)
							return true;
						if(PriceWithoutTaxes != _unmodified_PriceWithoutTaxes)
							return true;
						if(Taxes != _unmodified_Taxes)
							return true;
						if(PriceWithTaxes != _unmodified_PriceWithTaxes)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_IndentMode = IndentMode;
					_unmodified_EventStartDateTime = EventStartDateTime;
					_unmodified_EventEndDateTime = EventEndDateTime;
					_unmodified_ReceivingParty = ReceivingParty;
					_unmodified_AmountOfUnits = AmountOfUnits;
					_unmodified_Duration = Duration;
					_unmodified_UnitPrice = UnitPrice;
					_unmodified_PriceWithoutTaxes = PriceWithoutTaxes;
					_unmodified_Taxes = Taxes;
					_unmodified_PriceWithTaxes = PriceWithTaxes;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "IndentMode":
							IndentMode = value;
							break;
						case "EventStartDateTime":
							EventStartDateTime = DateTime.Parse(value);
							break;
						case "EventEndDateTime":
							EventEndDateTime = DateTime.Parse(value);
							break;
						case "ReceivingParty":
							ReceivingParty = value;
							break;
						case "AmountOfUnits":
							AmountOfUnits = value;
							break;
						case "Duration":
							Duration = value;
							break;
						case "UnitPrice":
							UnitPrice = value;
							break;
						case "PriceWithoutTaxes":
							PriceWithoutTaxes = value;
							break;
						case "Taxes":
							Taxes = value;
							break;
						case "PriceWithTaxes":
							PriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class InvoiceRow : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InvoiceRow()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InvoiceRow";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InvoiceRow/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InvoiceRow), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InvoiceRow>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InvoiceRow");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InvoiceRow), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InvoiceRow/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InvoiceRow/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InvoiceRow) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InvoiceRow", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InvoiceRow", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InvoiceRow customDemoObject);



				public static InvoiceRow CreateDefault()
				{
					var result = new InvoiceRow();
					return result;
				}
				/*
				public static InvoiceRow CreateDemoDefault()
				{
					InvoiceRow customDemo = null;
					InvoiceRow.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InvoiceRow();
					result.IndentMode = @"InvoiceRow.IndentMode";

					result.AmountOfUnits = @"InvoiceRow.AmountOfUnits";

					result.Duration = @"InvoiceRow.Duration";

					result.UnitPrice = @"InvoiceRow.UnitPrice";

					result.PriceWithoutTaxes = @"InvoiceRow.PriceWithoutTaxes";

					result.Taxes = @"InvoiceRow.Taxes";

					result.PriceWithTaxes = @"InvoiceRow.PriceWithTaxes";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(IndentMode != _unmodified_IndentMode)
							return true;
						if(AmountOfUnits != _unmodified_AmountOfUnits)
							return true;
						if(Duration != _unmodified_Duration)
							return true;
						if(UnitPrice != _unmodified_UnitPrice)
							return true;
						if(PriceWithoutTaxes != _unmodified_PriceWithoutTaxes)
							return true;
						if(Taxes != _unmodified_Taxes)
							return true;
						if(PriceWithTaxes != _unmodified_PriceWithTaxes)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_IndentMode = IndentMode;
					_unmodified_AmountOfUnits = AmountOfUnits;
					_unmodified_Duration = Duration;
					_unmodified_UnitPrice = UnitPrice;
					_unmodified_PriceWithoutTaxes = PriceWithoutTaxes;
					_unmodified_Taxes = Taxes;
					_unmodified_PriceWithTaxes = PriceWithTaxes;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "IndentMode":
							IndentMode = value;
							break;
						case "AmountOfUnits":
							AmountOfUnits = value;
							break;
						case "Duration":
							Duration = value;
							break;
						case "UnitPrice":
							UnitPrice = value;
							break;
						case "PriceWithoutTaxes":
							PriceWithoutTaxes = value;
							break;
						case "Taxes":
							Taxes = value;
							break;
						case "PriceWithTaxes":
							PriceWithTaxes = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class CategoryCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public CategoryCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "CategoryCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/CategoryCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(CategoryCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<CategoryCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: CategoryCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(CategoryCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/CategoryCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/CategoryCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((CategoryCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "CategoryCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "CategoryCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref CategoryCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<Category>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static CategoryCollection CreateDefault()
				{
					var result = new CategoryCollection();
					return result;
				}

				/*
				public static CategoryCollection CreateDemoDefault()
				{
					CategoryCollection customDemo = null;
					CategoryCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new CategoryCollection();
					result.CollectionContent.Add(Category.CreateDemoDefault());
					//result.CollectionContent.Add(Category.CreateDemoDefault());
					//result.CollectionContent.Add(Category.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (Category )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(CategoryCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class Category : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Category()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Category";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Category/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Category), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Category>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Category");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Category), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Category/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Category/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((Category) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Category", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Category", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Category customDemoObject);



				public static Category CreateDefault()
				{
					var result = new Category();
					return result;
				}
				/*
				public static Category CreateDemoDefault()
				{
					Category customDemo = null;
					Category.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Category();
					result.CategoryName = @"Category.CategoryName";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(CategoryName != _unmodified_CategoryName)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Category sourceObject)
				{
					CategoryName = sourceObject.CategoryName;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CategoryName = CategoryName;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CategoryName":
							CategoryName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string CategoryName { get; set; }
			private string _unmodified_CategoryName;
			
			}
			[DataContract] 
			[Serializable]
			public partial class ProcessContainer : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProcessContainer()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessContainer";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ProcessContainer/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ProcessContainer), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ProcessContainer>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProcessContainer");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ProcessContainer), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ProcessContainer/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ProcessContainer/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ProcessContainer", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ProcessContainer", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProcessContainer customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                Task IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ProcessIDs":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public List< string > ProcessIDs = new List< string >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class Process : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Process()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "Process";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/Process/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Process), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Process>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Process");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Process), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/Process/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/Process/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "Process", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "Process", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Process customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                Task IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ProcessDescription":
							ProcessDescription = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
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
			[DataContract] 
			[Serializable]
			public partial class ProcessItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProcessItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessItem";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ProcessItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ProcessItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ProcessItem>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProcessItem");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ProcessItem), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ProcessItem/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ProcessItem/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
				}

				void IInformationObject.SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
					throw new NotImplementedException("Collection item objects do not support tree functions for now");
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ProcessItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ProcessItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProcessItem customDemoObject);




				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					// Remove exception if basic functionality starts to have issues
					//throw new NotImplementedException("Item level collections do not support object tree operations right now");
					if(filterOnFalse(this))
						result.Add(this);
				}

				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					throw new NotImplementedException("Object tree support not implemented for item level collection objects");


				}

			
                Task IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}
	

				bool IInformationObject.IsInstanceTreeModified {
					get { 
						// Remove exception if some basic functionality is broken due to it
						throw new NotImplementedException("Collection items do not support instance tree queries as of now");
					}
				}
				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					// Remove exception if some basic functionality is broken due to it
					//throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					// Remove exception if some basic functionality is broken due to it
					throw new NotImplementedException("Collection items do not support instance tree queries as of now");
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public List< SemanticInformationItem > Outputs = new List< SemanticInformationItem >();
			[DataMember] 
			public List< SemanticInformationItem > Inputs = new List< SemanticInformationItem >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class SemanticInformationItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public SemanticInformationItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "SemanticInformationItem";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/SemanticInformationItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(SemanticInformationItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<SemanticInformationItem>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: SemanticInformationItem");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(SemanticInformationItem), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/SemanticInformationItem/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/SemanticInformationItem/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((SemanticInformationItem) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "SemanticInformationItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "SemanticInformationItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref SemanticInformationItem customDemoObject);



				public static SemanticInformationItem CreateDefault()
				{
					var result = new SemanticInformationItem();
					return result;
				}
				/*
				public static SemanticInformationItem CreateDemoDefault()
				{
					SemanticInformationItem customDemo = null;
					SemanticInformationItem.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new SemanticInformationItem();
					result.ItemFullType = @"SemanticInformationItem.ItemFullType";

					result.ItemValue = @"SemanticInformationItem.ItemValue";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(ItemFullType != _unmodified_ItemFullType)
							return true;
						if(ItemValue != _unmodified_ItemValue)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(SemanticInformationItem sourceObject)
				{
					ItemFullType = sourceObject.ItemFullType;
					ItemValue = sourceObject.ItemValue;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_ItemFullType = ItemFullType;
					_unmodified_ItemValue = ItemValue;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "ItemFullType":
							ItemFullType = value;
							break;
						case "ItemValue":
							ItemValue = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string ItemFullType { get; set; }
			private string _unmodified_ItemFullType;
			[DataMember] 
			public string ItemValue { get; set; }
			private string _unmodified_ItemValue;
			
			}
			[DataContract] 
			[Serializable]
			public partial class InformationOwnerInfo : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public InformationOwnerInfo()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "InformationOwnerInfo";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/InformationOwnerInfo/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(InformationOwnerInfo), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<InformationOwnerInfo>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: InformationOwnerInfo");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(InformationOwnerInfo), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/InformationOwnerInfo/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/InformationOwnerInfo/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((InformationOwnerInfo) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "InformationOwnerInfo", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "InformationOwnerInfo", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref InformationOwnerInfo customDemoObject);



				public static InformationOwnerInfo CreateDefault()
				{
					var result = new InformationOwnerInfo();
					return result;
				}
				/*
				public static InformationOwnerInfo CreateDemoDefault()
				{
					InformationOwnerInfo customDemo = null;
					InformationOwnerInfo.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new InformationOwnerInfo();
					result.OwnerType = @"InformationOwnerInfo.OwnerType";

					result.OwnerIdentifier = @"InformationOwnerInfo.OwnerIdentifier";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OwnerType != _unmodified_OwnerType)
							return true;
						if(OwnerIdentifier != _unmodified_OwnerIdentifier)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(InformationOwnerInfo sourceObject)
				{
					OwnerType = sourceObject.OwnerType;
					OwnerIdentifier = sourceObject.OwnerIdentifier;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_OwnerType = OwnerType;
					_unmodified_OwnerIdentifier = OwnerIdentifier;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OwnerType":
							OwnerType = value;
							break;
						case "OwnerIdentifier":
							OwnerIdentifier = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string OwnerType { get; set; }
			private string _unmodified_OwnerType;
			[DataMember] 
			public string OwnerIdentifier { get; set; }
			private string _unmodified_OwnerIdentifier;
			
			}
			[DataContract] 
			[Serializable]
			public partial class UsageSummary : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.Binary;
					}
				}

				public UsageSummary()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "UsageSummary";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/UsageSummary/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(UsageSummary), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<UsageSummary>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: UsageSummary");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(UsageSummary), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/UsageSummary/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/UsageSummary/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((UsageSummary) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "UsageSummary", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "UsageSummary", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref UsageSummary customDemoObject);



				public static UsageSummary CreateDefault()
				{
					var result = new UsageSummary();
					result.SummaryMonitoringItem = UsageMonitorItem.CreateDefault();
					return result;
				}
				/*
				public static UsageSummary CreateDemoDefault()
				{
					UsageSummary customDemo = null;
					UsageSummary.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new UsageSummary();
					result.SummaryName = @"UsageSummary.SummaryName";

					result.SummaryMonitoringItem = UsageMonitorItem.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(SummaryMonitoringItem != null) {
						((IInformationObject) SummaryMonitoringItem).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = SummaryMonitoringItem;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = SummaryMonitoringItem;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) SummaryMonitoringItem;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(SummaryName != _unmodified_SummaryName)
							return true;
						if(SummaryMonitoringItem != _unmodified_SummaryMonitoringItem)
							return true;
						{
							IInformationObject item = (IInformationObject) SummaryMonitoringItem;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(SummaryMonitoringItem != null) {
						if(SummaryMonitoringItem.ID == replacingObject.ID)
							SummaryMonitoringItem = (UsageMonitorItem) replacingObject;
						else {
							IInformationObject iObject = SummaryMonitoringItem;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(UsageSummary sourceObject)
				{
					SummaryName = sourceObject.SummaryName;
					SummaryMonitoringItem = sourceObject.SummaryMonitoringItem;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_SummaryName = SummaryName;
				
					_unmodified_SummaryMonitoringItem = SummaryMonitoringItem;
					if(SummaryMonitoringItem != null)
						((IInformationObject) SummaryMonitoringItem).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SummaryName":
							SummaryName = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string SummaryName { get; set; }
			private string _unmodified_SummaryName;
			[DataMember] 
			public UsageMonitorItem SummaryMonitoringItem { get; set; }
			private UsageMonitorItem _unmodified_SummaryMonitoringItem;
			
			}
			[DataContract] 
			[Serializable]
			public partial class UsageMonitorItem : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.Binary;
					}
				}

				public UsageMonitorItem()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "UsageMonitorItem";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/UsageMonitorItem/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(UsageMonitorItem), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<UsageMonitorItem>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: UsageMonitorItem");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(UsageMonitorItem), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/UsageMonitorItem/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/UsageMonitorItem/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((UsageMonitorItem) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "UsageMonitorItem", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "UsageMonitorItem", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref UsageMonitorItem customDemoObject);



				public static UsageMonitorItem CreateDefault()
				{
					var result = new UsageMonitorItem();
					result.OwnerInfo = InformationOwnerInfo.CreateDefault();
					result.TimeRangeInclusiveStartExclusiveEnd = TimeRange.CreateDefault();
					result.ProcessorUsages = ProcessorUsageCollection.CreateDefault();
					result.StorageTransactionUsages = StorageTransactionUsageCollection.CreateDefault();
					result.StorageUsages = StorageUsageCollection.CreateDefault();
					result.NetworkUsages = NetworkUsageCollection.CreateDefault();
					return result;
				}
				/*
				public static UsageMonitorItem CreateDemoDefault()
				{
					UsageMonitorItem customDemo = null;
					UsageMonitorItem.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new UsageMonitorItem();
					result.OwnerInfo = InformationOwnerInfo.CreateDemoDefault();
					result.TimeRangeInclusiveStartExclusiveEnd = TimeRange.CreateDemoDefault();
					result.ProcessorUsages = ProcessorUsageCollection.CreateDemoDefault();
					result.StorageTransactionUsages = StorageTransactionUsageCollection.CreateDemoDefault();
					result.StorageUsages = StorageUsageCollection.CreateDemoDefault();
					result.NetworkUsages = NetworkUsageCollection.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(OwnerInfo != null) {
						((IInformationObject) OwnerInfo).UpdateCollections(masterInstance);
					}

					if(TimeRangeInclusiveStartExclusiveEnd != null) {
						((IInformationObject) TimeRangeInclusiveStartExclusiveEnd).UpdateCollections(masterInstance);
					}

					if(ProcessorUsages != null) {
						((IInformationObject) ProcessorUsages).UpdateCollections(masterInstance);
					}

					if(StorageTransactionUsages != null) {
						((IInformationObject) StorageTransactionUsages).UpdateCollections(masterInstance);
					}

					if(StorageUsages != null) {
						((IInformationObject) StorageUsages).UpdateCollections(masterInstance);
					}

					if(NetworkUsages != null) {
						((IInformationObject) NetworkUsages).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = OwnerInfo;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = TimeRangeInclusiveStartExclusiveEnd;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = ProcessorUsages;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = StorageTransactionUsages;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = StorageUsages;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = NetworkUsages;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = OwnerInfo;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = TimeRangeInclusiveStartExclusiveEnd;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = ProcessorUsages;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = StorageTransactionUsages;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = StorageUsages;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = NetworkUsages;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) OwnerInfo;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) TimeRangeInclusiveStartExclusiveEnd;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) ProcessorUsages;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) StorageTransactionUsages;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) StorageUsages;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) NetworkUsages;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OwnerInfo != _unmodified_OwnerInfo)
							return true;
						if(TimeRangeInclusiveStartExclusiveEnd != _unmodified_TimeRangeInclusiveStartExclusiveEnd)
							return true;
						if(StepSizeInMinutes != _unmodified_StepSizeInMinutes)
							return true;
						if(ProcessorUsages != _unmodified_ProcessorUsages)
							return true;
						if(StorageTransactionUsages != _unmodified_StorageTransactionUsages)
							return true;
						if(StorageUsages != _unmodified_StorageUsages)
							return true;
						if(NetworkUsages != _unmodified_NetworkUsages)
							return true;
						{
							IInformationObject item = (IInformationObject) OwnerInfo;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) TimeRangeInclusiveStartExclusiveEnd;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) ProcessorUsages;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) StorageTransactionUsages;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) StorageUsages;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) NetworkUsages;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(OwnerInfo != null) {
						if(OwnerInfo.ID == replacingObject.ID)
							OwnerInfo = (InformationOwnerInfo) replacingObject;
						else {
							IInformationObject iObject = OwnerInfo;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(TimeRangeInclusiveStartExclusiveEnd != null) {
						if(TimeRangeInclusiveStartExclusiveEnd.ID == replacingObject.ID)
							TimeRangeInclusiveStartExclusiveEnd = (TimeRange) replacingObject;
						else {
							IInformationObject iObject = TimeRangeInclusiveStartExclusiveEnd;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(ProcessorUsages != null) {
						if(ProcessorUsages.ID == replacingObject.ID)
							ProcessorUsages = (ProcessorUsageCollection) replacingObject;
						else {
							IInformationObject iObject = ProcessorUsages;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(StorageTransactionUsages != null) {
						if(StorageTransactionUsages.ID == replacingObject.ID)
							StorageTransactionUsages = (StorageTransactionUsageCollection) replacingObject;
						else {
							IInformationObject iObject = StorageTransactionUsages;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(StorageUsages != null) {
						if(StorageUsages.ID == replacingObject.ID)
							StorageUsages = (StorageUsageCollection) replacingObject;
						else {
							IInformationObject iObject = StorageUsages;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(NetworkUsages != null) {
						if(NetworkUsages.ID == replacingObject.ID)
							NetworkUsages = (NetworkUsageCollection) replacingObject;
						else {
							IInformationObject iObject = NetworkUsages;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_StepSizeInMinutes = StepSizeInMinutes;
				
					_unmodified_OwnerInfo = OwnerInfo;
					if(OwnerInfo != null)
						((IInformationObject) OwnerInfo).SetInstanceTreeValuesAsUnmodified();

					_unmodified_TimeRangeInclusiveStartExclusiveEnd = TimeRangeInclusiveStartExclusiveEnd;
					if(TimeRangeInclusiveStartExclusiveEnd != null)
						((IInformationObject) TimeRangeInclusiveStartExclusiveEnd).SetInstanceTreeValuesAsUnmodified();

					_unmodified_ProcessorUsages = ProcessorUsages;
					if(ProcessorUsages != null)
						((IInformationObject) ProcessorUsages).SetInstanceTreeValuesAsUnmodified();

					_unmodified_StorageTransactionUsages = StorageTransactionUsages;
					if(StorageTransactionUsages != null)
						((IInformationObject) StorageTransactionUsages).SetInstanceTreeValuesAsUnmodified();

					_unmodified_StorageUsages = StorageUsages;
					if(StorageUsages != null)
						((IInformationObject) StorageUsages).SetInstanceTreeValuesAsUnmodified();

					_unmodified_NetworkUsages = NetworkUsages;
					if(NetworkUsages != null)
						((IInformationObject) NetworkUsages).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "StepSizeInMinutes":
							StepSizeInMinutes = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class RequestResourceUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.Binary;
					}
				}

				public RequestResourceUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "RequestResourceUsageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/RequestResourceUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(RequestResourceUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<RequestResourceUsageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: RequestResourceUsageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(RequestResourceUsageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/RequestResourceUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/RequestResourceUsageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((RequestResourceUsageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "RequestResourceUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "RequestResourceUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref RequestResourceUsageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<RequestResourceUsage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static RequestResourceUsageCollection CreateDefault()
				{
					var result = new RequestResourceUsageCollection();
					return result;
				}

				/*
				public static RequestResourceUsageCollection CreateDemoDefault()
				{
					RequestResourceUsageCollection customDemo = null;
					RequestResourceUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new RequestResourceUsageCollection();
					result.CollectionContent.Add(RequestResourceUsage.CreateDemoDefault());
					//result.CollectionContent.Add(RequestResourceUsage.CreateDemoDefault());
					//result.CollectionContent.Add(RequestResourceUsage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (RequestResourceUsage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(RequestResourceUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class RequestResourceUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.Binary;
					}
				}

				public RequestResourceUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "RequestResourceUsage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/RequestResourceUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(RequestResourceUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<RequestResourceUsage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: RequestResourceUsage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(RequestResourceUsage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/RequestResourceUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/RequestResourceUsage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((RequestResourceUsage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "RequestResourceUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "RequestResourceUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref RequestResourceUsage customDemoObject);



				public static RequestResourceUsage CreateDefault()
				{
					var result = new RequestResourceUsage();
					result.OwnerInfo = InformationOwnerInfo.CreateDefault();
					result.ProcessorUsage = ProcessorUsage.CreateDefault();
					result.StorageTransactionUsage = StorageTransactionUsage.CreateDefault();
					result.NetworkUsage = NetworkUsage.CreateDefault();
					result.RequestDetails = HTTPActivityDetails.CreateDefault();
					return result;
				}
				/*
				public static RequestResourceUsage CreateDemoDefault()
				{
					RequestResourceUsage customDemo = null;
					RequestResourceUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new RequestResourceUsage();
					result.OwnerInfo = InformationOwnerInfo.CreateDemoDefault();
					result.ProcessorUsage = ProcessorUsage.CreateDemoDefault();
					result.StorageTransactionUsage = StorageTransactionUsage.CreateDemoDefault();
					result.NetworkUsage = NetworkUsage.CreateDemoDefault();
					result.RequestDetails = HTTPActivityDetails.CreateDemoDefault();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(OwnerInfo != null) {
						((IInformationObject) OwnerInfo).UpdateCollections(masterInstance);
					}

					if(ProcessorUsage != null) {
						((IInformationObject) ProcessorUsage).UpdateCollections(masterInstance);
					}

					if(StorageTransactionUsage != null) {
						((IInformationObject) StorageTransactionUsage).UpdateCollections(masterInstance);
					}

					if(NetworkUsage != null) {
						((IInformationObject) NetworkUsage).UpdateCollections(masterInstance);
					}

					if(RequestDetails != null) {
						((IInformationObject) RequestDetails).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = OwnerInfo;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = ProcessorUsage;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = StorageTransactionUsage;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = NetworkUsage;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					{ // Scoping block for variable name reusability
						IInformationObject item = RequestDetails;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = OwnerInfo;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = ProcessorUsage;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = StorageTransactionUsage;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = NetworkUsage;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					{
						var item = RequestDetails;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) OwnerInfo;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) ProcessorUsage;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) StorageTransactionUsage;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) NetworkUsage;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
					{
						var item = (IInformationObject) RequestDetails;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(OwnerInfo != _unmodified_OwnerInfo)
							return true;
						if(ProcessorUsage != _unmodified_ProcessorUsage)
							return true;
						if(StorageTransactionUsage != _unmodified_StorageTransactionUsage)
							return true;
						if(NetworkUsage != _unmodified_NetworkUsage)
							return true;
						if(RequestDetails != _unmodified_RequestDetails)
							return true;
						{
							IInformationObject item = (IInformationObject) OwnerInfo;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) ProcessorUsage;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) StorageTransactionUsage;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) NetworkUsage;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
						{
							IInformationObject item = (IInformationObject) RequestDetails;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(OwnerInfo != null) {
						if(OwnerInfo.ID == replacingObject.ID)
							OwnerInfo = (InformationOwnerInfo) replacingObject;
						else {
							IInformationObject iObject = OwnerInfo;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(ProcessorUsage != null) {
						if(ProcessorUsage.ID == replacingObject.ID)
							ProcessorUsage = (ProcessorUsage) replacingObject;
						else {
							IInformationObject iObject = ProcessorUsage;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(StorageTransactionUsage != null) {
						if(StorageTransactionUsage.ID == replacingObject.ID)
							StorageTransactionUsage = (StorageTransactionUsage) replacingObject;
						else {
							IInformationObject iObject = StorageTransactionUsage;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(NetworkUsage != null) {
						if(NetworkUsage.ID == replacingObject.ID)
							NetworkUsage = (NetworkUsage) replacingObject;
						else {
							IInformationObject iObject = NetworkUsage;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
					if(RequestDetails != null) {
						if(RequestDetails.ID == replacingObject.ID)
							RequestDetails = (HTTPActivityDetails) replacingObject;
						else {
							IInformationObject iObject = RequestDetails;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(RequestResourceUsage sourceObject)
				{
					OwnerInfo = sourceObject.OwnerInfo;
					ProcessorUsage = sourceObject.ProcessorUsage;
					StorageTransactionUsage = sourceObject.StorageTransactionUsage;
					NetworkUsage = sourceObject.NetworkUsage;
					RequestDetails = sourceObject.RequestDetails;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
				
					_unmodified_OwnerInfo = OwnerInfo;
					if(OwnerInfo != null)
						((IInformationObject) OwnerInfo).SetInstanceTreeValuesAsUnmodified();

					_unmodified_ProcessorUsage = ProcessorUsage;
					if(ProcessorUsage != null)
						((IInformationObject) ProcessorUsage).SetInstanceTreeValuesAsUnmodified();

					_unmodified_StorageTransactionUsage = StorageTransactionUsage;
					if(StorageTransactionUsage != null)
						((IInformationObject) StorageTransactionUsage).SetInstanceTreeValuesAsUnmodified();

					_unmodified_NetworkUsage = NetworkUsage;
					if(NetworkUsage != null)
						((IInformationObject) NetworkUsage).SetInstanceTreeValuesAsUnmodified();

					_unmodified_RequestDetails = RequestDetails;
					if(RequestDetails != null)
						((IInformationObject) RequestDetails).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class ProcessorUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProcessorUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessorUsageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ProcessorUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ProcessorUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ProcessorUsageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProcessorUsageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ProcessorUsageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ProcessorUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ProcessorUsageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((ProcessorUsageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ProcessorUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ProcessorUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProcessorUsageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<ProcessorUsage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static ProcessorUsageCollection CreateDefault()
				{
					var result = new ProcessorUsageCollection();
					return result;
				}

				/*
				public static ProcessorUsageCollection CreateDemoDefault()
				{
					ProcessorUsageCollection customDemo = null;
					ProcessorUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProcessorUsageCollection();
					result.CollectionContent.Add(ProcessorUsage.CreateDemoDefault());
					//result.CollectionContent.Add(ProcessorUsage.CreateDemoDefault());
					//result.CollectionContent.Add(ProcessorUsage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (ProcessorUsage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(ProcessorUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class ProcessorUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public ProcessorUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "ProcessorUsage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/ProcessorUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(ProcessorUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<ProcessorUsage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: ProcessorUsage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(ProcessorUsage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/ProcessorUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/ProcessorUsage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((ProcessorUsage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "ProcessorUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "ProcessorUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref ProcessorUsage customDemoObject);



				public static ProcessorUsage CreateDefault()
				{
					var result = new ProcessorUsage();
					result.TimeRange = TimeRange.CreateDefault();
					return result;
				}
				/*
				public static ProcessorUsage CreateDemoDefault()
				{
					ProcessorUsage customDemo = null;
					ProcessorUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new ProcessorUsage();
					result.TimeRange = TimeRange.CreateDemoDefault();
					result.UsageType = @"ProcessorUsage.UsageType";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(TimeRange != null) {
						((IInformationObject) TimeRange).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = TimeRange;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = TimeRange;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) TimeRange;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(TimeRange != _unmodified_TimeRange)
							return true;
						if(UsageType != _unmodified_UsageType)
							return true;
						if(AmountOfTicks != _unmodified_AmountOfTicks)
							return true;
						if(FrequencyTicksPerSecond != _unmodified_FrequencyTicksPerSecond)
							return true;
						if(Milliseconds != _unmodified_Milliseconds)
							return true;
						{
							IInformationObject item = (IInformationObject) TimeRange;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(TimeRange != null) {
						if(TimeRange.ID == replacingObject.ID)
							TimeRange = (TimeRange) replacingObject;
						else {
							IInformationObject iObject = TimeRange;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(ProcessorUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfTicks = sourceObject.AmountOfTicks;
					FrequencyTicksPerSecond = sourceObject.FrequencyTicksPerSecond;
					Milliseconds = sourceObject.Milliseconds;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UsageType = UsageType;
					_unmodified_AmountOfTicks = AmountOfTicks;
					_unmodified_FrequencyTicksPerSecond = FrequencyTicksPerSecond;
					_unmodified_Milliseconds = Milliseconds;
				
					_unmodified_TimeRange = TimeRange;
					if(TimeRange != null)
						((IInformationObject) TimeRange).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UsageType":
							UsageType = value;
							break;
						case "AmountOfTicks":
							AmountOfTicks = double.Parse(value);
							break;
						case "FrequencyTicksPerSecond":
							FrequencyTicksPerSecond = double.Parse(value);
							break;
						case "Milliseconds":
							Milliseconds = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class StorageTransactionUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public StorageTransactionUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageTransactionUsageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/StorageTransactionUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(StorageTransactionUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<StorageTransactionUsageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: StorageTransactionUsageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(StorageTransactionUsageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/StorageTransactionUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/StorageTransactionUsageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((StorageTransactionUsageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "StorageTransactionUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "StorageTransactionUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StorageTransactionUsageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<StorageTransactionUsage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static StorageTransactionUsageCollection CreateDefault()
				{
					var result = new StorageTransactionUsageCollection();
					return result;
				}

				/*
				public static StorageTransactionUsageCollection CreateDemoDefault()
				{
					StorageTransactionUsageCollection customDemo = null;
					StorageTransactionUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new StorageTransactionUsageCollection();
					result.CollectionContent.Add(StorageTransactionUsage.CreateDemoDefault());
					//result.CollectionContent.Add(StorageTransactionUsage.CreateDemoDefault());
					//result.CollectionContent.Add(StorageTransactionUsage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (StorageTransactionUsage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(StorageTransactionUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class StorageTransactionUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public StorageTransactionUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageTransactionUsage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/StorageTransactionUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(StorageTransactionUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<StorageTransactionUsage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: StorageTransactionUsage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(StorageTransactionUsage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/StorageTransactionUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/StorageTransactionUsage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((StorageTransactionUsage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "StorageTransactionUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "StorageTransactionUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StorageTransactionUsage customDemoObject);



				public static StorageTransactionUsage CreateDefault()
				{
					var result = new StorageTransactionUsage();
					result.TimeRange = TimeRange.CreateDefault();
					return result;
				}
				/*
				public static StorageTransactionUsage CreateDemoDefault()
				{
					StorageTransactionUsage customDemo = null;
					StorageTransactionUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new StorageTransactionUsage();
					result.TimeRange = TimeRange.CreateDemoDefault();
					result.UsageType = @"StorageTransactionUsage.UsageType";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(TimeRange != null) {
						((IInformationObject) TimeRange).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = TimeRange;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = TimeRange;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) TimeRange;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(TimeRange != _unmodified_TimeRange)
							return true;
						if(UsageType != _unmodified_UsageType)
							return true;
						if(AmountOfTransactions != _unmodified_AmountOfTransactions)
							return true;
						{
							IInformationObject item = (IInformationObject) TimeRange;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(TimeRange != null) {
						if(TimeRange.ID == replacingObject.ID)
							TimeRange = (TimeRange) replacingObject;
						else {
							IInformationObject iObject = TimeRange;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(StorageTransactionUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfTransactions = sourceObject.AmountOfTransactions;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UsageType = UsageType;
					_unmodified_AmountOfTransactions = AmountOfTransactions;
				
					_unmodified_TimeRange = TimeRange;
					if(TimeRange != null)
						((IInformationObject) TimeRange).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UsageType":
							UsageType = value;
							break;
						case "AmountOfTransactions":
							AmountOfTransactions = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class StorageUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public StorageUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageUsageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/StorageUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(StorageUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<StorageUsageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: StorageUsageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(StorageUsageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/StorageUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/StorageUsageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((StorageUsageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "StorageUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "StorageUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StorageUsageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<StorageUsage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static StorageUsageCollection CreateDefault()
				{
					var result = new StorageUsageCollection();
					return result;
				}

				/*
				public static StorageUsageCollection CreateDemoDefault()
				{
					StorageUsageCollection customDemo = null;
					StorageUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new StorageUsageCollection();
					result.CollectionContent.Add(StorageUsage.CreateDemoDefault());
					//result.CollectionContent.Add(StorageUsage.CreateDemoDefault());
					//result.CollectionContent.Add(StorageUsage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (StorageUsage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(StorageUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class StorageUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public StorageUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "StorageUsage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/StorageUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(StorageUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<StorageUsage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: StorageUsage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(StorageUsage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/StorageUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/StorageUsage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((StorageUsage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "StorageUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "StorageUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref StorageUsage customDemoObject);



				public static StorageUsage CreateDefault()
				{
					var result = new StorageUsage();
					return result;
				}
				/*
				public static StorageUsage CreateDemoDefault()
				{
					StorageUsage customDemo = null;
					StorageUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new StorageUsage();
					result.UsageType = @"StorageUsage.UsageType";

					result.UsageUnit = @"StorageUsage.UsageUnit";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(SnapshotTime != _unmodified_SnapshotTime)
							return true;
						if(UsageType != _unmodified_UsageType)
							return true;
						if(UsageUnit != _unmodified_UsageUnit)
							return true;
						if(AmountOfUnits != _unmodified_AmountOfUnits)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(StorageUsage sourceObject)
				{
					SnapshotTime = sourceObject.SnapshotTime;
					UsageType = sourceObject.UsageType;
					UsageUnit = sourceObject.UsageUnit;
					AmountOfUnits = sourceObject.AmountOfUnits;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_SnapshotTime = SnapshotTime;
					_unmodified_UsageType = UsageType;
					_unmodified_UsageUnit = UsageUnit;
					_unmodified_AmountOfUnits = AmountOfUnits;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "SnapshotTime":
							SnapshotTime = DateTime.Parse(value);
							break;
						case "UsageType":
							UsageType = value;
							break;
						case "UsageUnit":
							UsageUnit = value;
							break;
						case "AmountOfUnits":
							AmountOfUnits = double.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class NetworkUsageCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public NetworkUsageCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "NetworkUsageCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/NetworkUsageCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(NetworkUsageCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<NetworkUsageCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: NetworkUsageCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(NetworkUsageCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/NetworkUsageCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/NetworkUsageCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((NetworkUsageCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "NetworkUsageCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "NetworkUsageCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref NetworkUsageCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<NetworkUsage>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static NetworkUsageCollection CreateDefault()
				{
					var result = new NetworkUsageCollection();
					return result;
				}

				/*
				public static NetworkUsageCollection CreateDemoDefault()
				{
					NetworkUsageCollection customDemo = null;
					NetworkUsageCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new NetworkUsageCollection();
					result.CollectionContent.Add(NetworkUsage.CreateDemoDefault());
					//result.CollectionContent.Add(NetworkUsage.CreateDemoDefault());
					//result.CollectionContent.Add(NetworkUsage.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (NetworkUsage )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(NetworkUsageCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class NetworkUsage : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public NetworkUsage()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "NetworkUsage";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/NetworkUsage/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(NetworkUsage), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<NetworkUsage>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: NetworkUsage");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(NetworkUsage), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/NetworkUsage/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/NetworkUsage/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((NetworkUsage) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "NetworkUsage", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "NetworkUsage", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref NetworkUsage customDemoObject);



				public static NetworkUsage CreateDefault()
				{
					var result = new NetworkUsage();
					result.TimeRange = TimeRange.CreateDefault();
					return result;
				}
				/*
				public static NetworkUsage CreateDemoDefault()
				{
					NetworkUsage customDemo = null;
					NetworkUsage.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new NetworkUsage();
					result.TimeRange = TimeRange.CreateDemoDefault();
					result.UsageType = @"NetworkUsage.UsageType";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
					if(TimeRange != null) {
						((IInformationObject) TimeRange).UpdateCollections(masterInstance);
					}

				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					{ // Scoping block for variable name reusability
						IInformationObject item = TimeRange;
						if(item != null)
						{
							item.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
						}
					} // Scoping block end

					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					{
						var item = TimeRange;
						if(item != null)
						{
							object result = item.FindObjectByID(objectId);
							if(result != null)
								return result;
						}
					}
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					{
						var item = (IInformationObject) TimeRange;
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(TimeRange != _unmodified_TimeRange)
							return true;
						if(UsageType != _unmodified_UsageType)
							return true;
						if(AmountOfBytes != _unmodified_AmountOfBytes)
							return true;
						{
							IInformationObject item = (IInformationObject) TimeRange;
							if(item != null) 
							{
								bool isItemTreeModified = item.IsInstanceTreeModified;
								if(isItemTreeModified)
									return true;
							}
						}
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					if(TimeRange != null) {
						if(TimeRange.ID == replacingObject.ID)
							TimeRange = (TimeRange) replacingObject;
						else {
							IInformationObject iObject = TimeRange;
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}


				private void CopyContentFrom(NetworkUsage sourceObject)
				{
					TimeRange = sourceObject.TimeRange;
					UsageType = sourceObject.UsageType;
					AmountOfBytes = sourceObject.AmountOfBytes;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_UsageType = UsageType;
					_unmodified_AmountOfBytes = AmountOfBytes;
				
					_unmodified_TimeRange = TimeRange;
					if(TimeRange != null)
						((IInformationObject) TimeRange).SetInstanceTreeValuesAsUnmodified();

				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "UsageType":
							UsageType = value;
							break;
						case "AmountOfBytes":
							AmountOfBytes = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class TimeRange : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public TimeRange()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "TimeRange";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/TimeRange/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(TimeRange), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<TimeRange>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: TimeRange");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(TimeRange), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/TimeRange/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/TimeRange/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((TimeRange) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "TimeRange", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "TimeRange", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref TimeRange customDemoObject);



				public static TimeRange CreateDefault()
				{
					var result = new TimeRange();
					return result;
				}
				/*
				public static TimeRange CreateDemoDefault()
				{
					TimeRange customDemo = null;
					TimeRange.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new TimeRange();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(StartTime != _unmodified_StartTime)
							return true;
						if(EndTime != _unmodified_EndTime)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(TimeRange sourceObject)
				{
					StartTime = sourceObject.StartTime;
					EndTime = sourceObject.EndTime;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_StartTime = StartTime;
					_unmodified_EndTime = EndTime;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "StartTime":
							StartTime = DateTime.Parse(value);
							break;
						case "EndTime":
							EndTime = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public DateTime StartTime { get; set; }
			private DateTime _unmodified_StartTime;
			[DataMember] 
			public DateTime EndTime { get; set; }
			private DateTime _unmodified_EndTime;
			
			}
			[DataContract] 
			[Serializable]
			public partial class HTTPActivityDetailsCollection : IInformationObject , IInformationCollection
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public HTTPActivityDetailsCollection()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "HTTPActivityDetailsCollection";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/HTTPActivityDetailsCollection/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(HTTPActivityDetailsCollection), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<HTTPActivityDetailsCollection>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: HTTPActivityDetailsCollection");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(HTTPActivityDetailsCollection), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/HTTPActivityDetailsCollection/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/HTTPActivityDetailsCollection/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((HTTPActivityDetailsCollection) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "HTTPActivityDetailsCollection", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "HTTPActivityDetailsCollection", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref HTTPActivityDetailsCollection customDemoObject);


				
				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.UpdateCollections(masterInstance);
					}
				}



				bool IInformationCollection.IsMasterCollection {
					get {
						return false;
					}
				}

				string IInformationCollection.GetMasterLocation()
				{
					throw new NotSupportedException("Master collection location only supported for master collections");
					
				}

				async Task<IInformationCollection> IInformationCollection.GetMasterInstanceAsync()
				{
					throw new NotSupportedException("Master collection instance only supported for master collections");
					
				}


				public string GetItemDirectory()
				{
					string dummyItemLocation = ObjectStorage.GetRelativeLocationFromID<HTTPActivityDetails>("dummy");
					string nonOwnerDirectoryLocation = StorageSupport.GetParentDirectoryTarget(dummyItemLocation);
					var owner = VirtualOwner.FigureOwner(this);
					string ownerDirectoryLocation = StorageSupport.GetOwnerContentLocation(owner, nonOwnerDirectoryLocation);
					return ownerDirectoryLocation;
				}

				public async Task RefreshContentAsync()
				{
				}




                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }

				
		
				public static HTTPActivityDetailsCollection CreateDefault()
				{
					var result = new HTTPActivityDetailsCollection();
					return result;
				}

				/*
				public static HTTPActivityDetailsCollection CreateDemoDefault()
				{
					HTTPActivityDetailsCollection customDemo = null;
					HTTPActivityDetailsCollection.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new HTTPActivityDetailsCollection();
					result.CollectionContent.Add(HTTPActivityDetails.CreateDemoDefault());
					//result.CollectionContent.Add(HTTPActivityDetails.CreateDemoDefault());
					//result.CollectionContent.Add(HTTPActivityDetails.CreateDemoDefault());
					return result;
				}
				*/

		
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

				public void ParsePropertyValue(string propertyName, string propertyValue)
				{
					switch(propertyName)
					{
						case "SelectedIDCommaSeparated":
							SelectedIDCommaSeparated = propertyValue;
							break;
						case "IsCollectionFiltered":
							IsCollectionFiltered = bool.Parse(propertyValue);
							break;
						default:
							throw new NotSupportedException("No ParsePropertyValue supported for property: " + propertyName);
					}
				}


				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
					for(int i = 0; i < CollectionContent.Count; i++) // >
					{
						if(CollectionContent[i].ID == replacingObject.ID)
							CollectionContent[i] = (HTTPActivityDetails )replacingObject;
						else { // Cannot have circular reference, so can be in else branch
							IInformationObject iObject = CollectionContent[i];
							iObject.ReplaceObjectInTree(replacingObject);
						}
					}
				}

				
				bool IInformationObject.IsInstanceTreeModified {
					get {
						bool collectionModified = CollectionContent.SequenceEqual(_unmodified_CollectionContent) == false;
						if(collectionModified)
							return true;
						//if((OrderFilterIDList == null && _unmodified_OrderFilterIDList != null) || _unmodified_OrderFilterIDList
						if(IsCollectionFiltered != _unmodified_IsCollectionFiltered)
							return true;
						// For non-master content
						foreach(IInformationObject item in CollectionContent)
						{
							bool itemTreeModified = item.IsInstanceTreeModified;
							if(itemTreeModified)
								return true;
						}
							
						return false;
					}
				}
				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CollectionContent = CollectionContent.ToArray();
					_unmodified_IsCollectionFiltered = IsCollectionFiltered;
					if(OrderFilterIDList == null)
						_unmodified_OrderFilterIDList = null;
					else
						_unmodified_OrderFilterIDList = OrderFilterIDList.ToArray();
					foreach(IInformationObject iObject in CollectionContent)
						iObject.SetInstanceTreeValuesAsUnmodified();
				}

				private void CopyContentFrom(HTTPActivityDetailsCollection sourceObject)
				{
					CollectionContent = sourceObject.CollectionContent;
					_unmodified_CollectionContent = sourceObject._unmodified_CollectionContent;
				}
				
				private object FindFromObjectTree(string objectId)
				{
					foreach(var item in CollectionContent)
					{
						object result = item.FindObjectByID(objectId);
						if(result != null)
							return result;
					}
					return null;
				}

				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					foreach(IInformationObject iObject in CollectionContent)
						iObject.FindObjectsFromTree(result, filterOnFalse, searchWithinCurrentMasterOnly);
				}


				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						bool doAdd = true;
						if(filterOnFalse != null)
							doAdd = filterOnFalse(iObject);
						if(doAdd) {
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}
					foreach(IInformationObject item in CollectionContent)
					{
						if(item != null)
							item.CollectMasterObjectsFromTree(result, filterOnFalse);
					}
				}


			
			}
			[DataContract] 
			[Serializable]
			public partial class HTTPActivityDetails : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public HTTPActivityDetails()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "TheBall.CORE";
				    this.Name = "HTTPActivityDetails";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "TheBall.CORE/HTTPActivityDetails/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = await BlobStorage.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(HTTPActivityDetails), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<HTTPActivityDetails>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: HTTPActivityDetails");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(this);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(HTTPActivityDetails), null, owner);
					if(master == null && initiateIfMissing)
					{
						await StorageSupport.StoreInformationAsync(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}

				/*
				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return await iObject.RetrieveMasterAsync(initiateIfMissing, out initiated);
				}*/

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/TheBall.CORE/HTTPActivityDetails/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "TheBall.CORE/HTTPActivityDetails/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner, ref Task task);

				public async Task PostStoringExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostStoringExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}

				partial void DoPostDeleteExecute(IContainerOwner owner, ref Task task);

				public async Task PostDeleteExecute(IContainerOwner owner)
				{
					Task postTask = null;
					DoPostDeleteExecute(owner, ref postTask);
					if(postTask != null)
						await postTask;
				}


				bool IInformationObject.IsIndependentMaster { 
					get {
						return false;
					}
				}


			    public void SetValuesToObjects(NameValueCollection nameValueCollection)
			    {
                    foreach(string key in nameValueCollection.AllKeys)
                    {
                        if (key.StartsWith("Root"))
                            continue;
                        int indexOfUnderscore = key.IndexOf("_");
						if (indexOfUnderscore < 0) // >
                            continue;
                        string objectID = key.Substring(0, indexOfUnderscore);
                        object targetObject = FindObjectByID(objectID);
                        if (targetObject == null)
                            continue;
                        string propertyName = key.Substring(indexOfUnderscore + 1);
                        string propertyValue = nameValueCollection[key];
						throw new NotSupportedException("Fix dynamic call");
                        //dynamic dyn = targetObject;
                        //dyn.ParsePropertyValue(propertyName, propertyValue);
                    }
			    }

			    public object FindObjectByID(string objectId)
			    {
                    if (objectId == ID)
                        return this;
			        return FindFromObjectTree(objectId);
			    }

				void IInformationObject.UpdateMasterValueTreeFromOtherInstance(IInformationObject sourceMaster)
				{
					if (sourceMaster == null)
						throw new ArgumentNullException("sourceMaster");
					if (GetType() != sourceMaster.GetType())
						throw new InvalidDataException("Type mismatch in UpdateMasterValueTree");
					IInformationObject iObject = this;
					if(iObject.IsIndependentMaster == false)
						throw new InvalidDataException("UpdateMasterValueTree called on non-master type");
					if(ID != sourceMaster.ID)
						throw new InvalidDataException("UpdateMasterValueTree is supported only on masters with same ID");
					CopyContentFrom((HTTPActivityDetails) sourceMaster);
				}


				Dictionary<string, List<IInformationObject>> IInformationObject.CollectMasterObjects(Predicate<IInformationObject> filterOnFalse)
				{
					Dictionary<string, List<IInformationObject>> result = new Dictionary<string, List<IInformationObject>>();
					IInformationObject iObject = (IInformationObject) this;
					iObject.CollectMasterObjectsFromTree(result, filterOnFalse);
					return result;
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("TheBall.CORE", "HTTPActivityDetails", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
				}

				public void SetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
				{
				    RelativeLocation = GetLocationRelativeToContentRoot(referenceLocation, sourceName);
				}

                public string GetLocationRelativeToContentRoot(string referenceLocation, string sourceName)
                {
                    string relativeLocation;
                    if (String.IsNullOrEmpty(sourceName))
                        sourceName = "default";
                    string contentRootLocation = StorageSupport.GetContentRootLocation(referenceLocation);
                    relativeLocation = Path.Combine(contentRootLocation, "TheBall.CORE", "HTTPActivityDetails", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref HTTPActivityDetails customDemoObject);



				public static HTTPActivityDetails CreateDefault()
				{
					var result = new HTTPActivityDetails();
					return result;
				}
				/*
				public static HTTPActivityDetails CreateDemoDefault()
				{
					HTTPActivityDetails customDemo = null;
					HTTPActivityDetails.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new HTTPActivityDetails();
					result.RemoteIPAddress = @"HTTPActivityDetails.RemoteIPAddress";

					result.RemoteEndpointUserName = @"HTTPActivityDetails.RemoteEndpointUserName";

					result.UserID = @"HTTPActivityDetails.UserID";

					result.RequestLine = @"HTTPActivityDetails.RequestLine";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public async Task SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    await targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
                }


				void IInformationObject.FindObjectsFromTree(List<IInformationObject> result, Predicate<IInformationObject> filterOnFalse, bool searchWithinCurrentMasterOnly)
				{
					if(filterOnFalse(this))
						result.Add(this);
					if(searchWithinCurrentMasterOnly == false)
					{
					}					
				}

				private object FindFromObjectTree(string objectId)
				{
					return null;
				}
				void IInformationObject.CollectMasterObjectsFromTree(Dictionary<string, List<IInformationObject>> result, Predicate<IInformationObject> filterOnFalse)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster)
					{
						if(filterOnFalse == null || filterOnFalse(iObject)) 
						{
							string key = iObject.ID;
							List<IInformationObject> existingValue;
							bool keyFound = result.TryGetValue(key, out existingValue);
							if(keyFound == false) {
								existingValue = new List<IInformationObject>();
								result.Add(key, existingValue);
							}
							existingValue.Add(iObject);
						}
					}

				}

				bool IInformationObject.IsInstanceTreeModified {
					get { 

						if(RemoteIPAddress != _unmodified_RemoteIPAddress)
							return true;
						if(RemoteEndpointUserName != _unmodified_RemoteEndpointUserName)
							return true;
						if(UserID != _unmodified_UserID)
							return true;
						if(UTCDateTime != _unmodified_UTCDateTime)
							return true;
						if(RequestLine != _unmodified_RequestLine)
							return true;
						if(HTTPStatusCode != _unmodified_HTTPStatusCode)
							return true;
						if(ReturnedContentLength != _unmodified_ReturnedContentLength)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_RemoteIPAddress = RemoteIPAddress;
					_unmodified_RemoteEndpointUserName = RemoteEndpointUserName;
					_unmodified_UserID = UserID;
					_unmodified_UTCDateTime = UTCDateTime;
					_unmodified_RequestLine = RequestLine;
					_unmodified_HTTPStatusCode = HTTPStatusCode;
					_unmodified_ReturnedContentLength = ReturnedContentLength;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "RemoteIPAddress":
							RemoteIPAddress = value;
							break;
						case "RemoteEndpointUserName":
							RemoteEndpointUserName = value;
							break;
						case "UserID":
							UserID = value;
							break;
						case "UTCDateTime":
							UTCDateTime = DateTime.Parse(value);
							break;
						case "RequestLine":
							RequestLine = value;
							break;
						case "HTTPStatusCode":
							HTTPStatusCode = long.Parse(value);
							break;
						case "ReturnedContentLength":
							ReturnedContentLength = long.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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