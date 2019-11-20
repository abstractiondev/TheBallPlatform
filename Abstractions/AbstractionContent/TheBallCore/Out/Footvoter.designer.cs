 


using DOM=Footvoter.Services;
using System.Threading.Tasks;

namespace TheBall.Core {
	public static partial class OwnerInitializer
	{
		private static async Task DOMAININIT_Footvoter_Services(IContainerOwner owner)
		{
			await DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			await DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace Footvoter.Services { 
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
using TheBall.Core;
using TheBall.Core.StorageCore;

namespace INT { 
					[DataContract]
			public partial class UserProfile
			{
				[DataMember]
				public string firstName { get; set; }
				[DataMember]
				public string lastName { get; set; }
				[DataMember]
				public string description { get; set; }
				[DataMember]
				public DateTime dateOfBirth { get; set; }
			}

			[DataContract]
			public partial class CompanyFollowData
			{
				[DataMember]
				public FollowDataItem[] FollowDataItems { get; set; }
			}

			[DataContract]
			public partial class FollowDataItem
			{
				[DataMember]
				public string IDToFollow { get; set; }
				[DataMember]
				public double FollowingLevel { get; set; }
			}

			[DataContract]
			public partial class VoteData
			{
				[DataMember]
				public VoteItem[] Votes { get; set; }
			}

			[DataContract]
			public partial class VoteItem
			{
				[DataMember]
				public string companyID { get; set; }
				[DataMember]
				public bool voteValue { get; set; }
			}

			[DataContract]
			public partial class VotedEntry
			{
				[DataMember]
				public string VotedForID { get; set; }
				[DataMember]
				public DateTime VoteTime { get; set; }
			}

			[DataContract]
			public partial class VotingSummary
			{
				[DataMember]
				public VotedEntry[] VotedEntries { get; set; }
			}

			[DataContract]
			public partial class CompanySearchCriteria
			{
				[DataMember]
				public string namePart { get; set; }
				[DataMember]
				public GpsLocation gpsLocation { get; set; }
			}

			[DataContract]
			public partial class GpsLocation
			{
				[DataMember]
				public double latitude { get; set; }
				[DataMember]
				public double longitude { get; set; }
			}

 } 		public static class DomainInformationSupport
		{
            public static async Task EnsureMasterCollections(IContainerOwner owner)
            {
            }

            public static async Task RefreshMasterCollections(IContainerOwner owner)
            {
            }
		}
			[DataContract] 
			[Serializable]
			public partial class Company : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Company()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Footvoter.Services";
				    this.Name = "Company";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Footvoter.Services/Company/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
                    var storageService = CoreServices.GetCurrent<IStorageService>();
					var blobListing = await storageService.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Company), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Company>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Company");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(RelativeLocation);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Company), null, owner);
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
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Footvoter.Services/Company/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Footvoter.Services/Company/" + contentName);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Company));
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

				public static Company DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Company));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Company) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Footvoter.Services", "Company", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Footvoter.Services", "Company", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Company customDemoObject);




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
						case "CompanyName":
							CompanyName = value;
							break;
						case "Details":
							Details = value;
							break;
						case "Footprint":
							Footprint = double.Parse(value);
							break;
						case "Footpath":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string CompanyName { get; set; }
			private string _unmodified_CompanyName;
			[DataMember] 
			public string Details { get; set; }
			private string _unmodified_Details;
			[DataMember] 
			public double Footprint { get; set; }
			private double _unmodified_Footprint;
			[DataMember] 
			public List< double > Footpath = new List< double >();
			
			}
			[DataContract] 
			[Serializable]
			public partial class Vote : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Vote()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "Footvoter.Services";
				    this.Name = "Vote";
					UpdateRelativeLocationFromID();
				}

				public static async Task<IInformationObject[]> RetrieveCollectionFromOwnerContentAsync(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "Footvoter.Services/Vote/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
                    var storageService = CoreServices.GetCurrent<IStorageService>();
					var blobListing = await storageService.GetBlobItemsA(owner, contentTypeName);
					foreach(var blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = await StorageSupport.RetrieveInformationA(blob.Name, typeof(Vote), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Vote>(ID);
				}

				async Task<IInformationObject> IInformationObject.RetrieveMasterAsync(bool initiateIfMissing)
				{
					bool initiated = false;
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Vote");
					initiated = false;
					var owner = VirtualOwner.FigureOwner(RelativeLocation);
					var master = await StorageSupport.RetrieveInformationA(RelativeLocation, typeof(Vote), null, owner);
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
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/Footvoter.Services/Vote/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Footvoter.Services/Vote/" + contentName);
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
					CopyContentFrom((Vote) sourceMaster);
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
					DataContractSerializer serializer = new DataContractSerializer(typeof(Vote));
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

				public static Vote DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(Vote));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (Vote) serializer.ReadObject(xmlReader);
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
					return Path.Combine("Footvoter.Services", "Vote", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "Footvoter.Services", "Vote", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Vote customDemoObject);



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

						if(CompanyID != _unmodified_CompanyID)
							return true;
						if(VoteValue != _unmodified_VoteValue)
							return true;
						if(VoteTime != _unmodified_VoteTime)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Vote sourceObject)
				{
					CompanyID = sourceObject.CompanyID;
					VoteValue = sourceObject.VoteValue;
					VoteTime = sourceObject.VoteTime;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_CompanyID = CompanyID;
					_unmodified_VoteValue = VoteValue;
					_unmodified_VoteTime = VoteTime;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "CompanyID":
							CompanyID = value;
							break;
						case "VoteValue":
							VoteValue = bool.Parse(value);
							break;
						case "VoteTime":
							VoteTime = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
			[DataMember] 
			public string CompanyID { get; set; }
			private string _unmodified_CompanyID;
			[DataMember] 
			public bool VoteValue { get; set; }
			private bool _unmodified_VoteValue;
			[DataMember] 
			public DateTime VoteTime { get; set; }
			private DateTime _unmodified_VoteTime;
			
			}
 } 