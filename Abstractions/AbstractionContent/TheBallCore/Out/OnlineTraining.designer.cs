 


using DOM=ProBroz.OnlineTraining;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static void DOMAININIT_ProBroz_OnlineTraining(IContainerOwner owner)
		{
			DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace ProBroz.OnlineTraining { 
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



namespace INT { 
		 } 		public static class DomainInformationSupport
		{
            public static void EnsureMasterCollections(IContainerOwner owner)
            {
            }

            public static void RefreshMasterCollections(IContainerOwner owner)
            {
            }
		}
			[DataContract] 
			[Serializable]
			public partial class Member : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Member()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "Member";
					UpdateRelativeLocationFromID();
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "ProBroz.OnlineTraining/Member/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Member), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Member>(ID);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Member");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Member), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/ProBroz.OnlineTraining/Member/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "ProBroz.OnlineTraining/Member/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("ProBroz.OnlineTraining", "Member", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "ProBroz.OnlineTraining", "Member", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Member customDemoObject);



				public static Member CreateDefault()
				{
					var result = new Member();
					return result;
				}
				/*
				public static Member CreateDemoDefault()
				{
					Member customDemo = null;
					Member.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Member();
					result.FirstName = @"Member.FirstName";

					result.LastName = @"Member.LastName";

					result.MiddleName = @"Member.MiddleName";

					result.Email = @"Member.Email";

					result.PhoneNumber = @"Member.PhoneNumber";

					result.Address = @"Member.Address";

					result.Address2 = @"Member.Address2";

					result.ZipCode = @"Member.ZipCode";

					result.PostOffice = @"Member.PostOffice";

					result.Country = @"Member.Country";

					result.FederationLicense = @"Member.FederationLicense
Member.FederationLicense
Member.FederationLicense
Member.FederationLicense
Member.FederationLicense
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

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
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
						case "FirstName":
							FirstName = value;
							break;
						case "LastName":
							LastName = value;
							break;
						case "MiddleName":
							MiddleName = value;
							break;
						case "BirthDay":
							BirthDay = DateTime.Parse(value);
							break;
						case "Email":
							Email = value;
							break;
						case "PhoneNumber":
							PhoneNumber = value;
							break;
						case "Address":
							Address = value;
							break;
						case "Address2":
							Address2 = value;
							break;
						case "ZipCode":
							ZipCode = value;
							break;
						case "PostOffice":
							PostOffice = value;
							break;
						case "Country":
							Country = value;
							break;
						case "FederationLicense":
							FederationLicense = value;
							break;
						case "PhotoPermission":
							PhotoPermission = bool.Parse(value);
							break;
						case "VideoPermission":
							VideoPermission = bool.Parse(value);
							break;
						case "Subscriptions":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
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
			[DataContract] 
			[Serializable]
			public partial class MembershipPlan : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public MembershipPlan()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "MembershipPlan";
					UpdateRelativeLocationFromID();
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "ProBroz.OnlineTraining/MembershipPlan/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(MembershipPlan), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<MembershipPlan>(ID);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: MembershipPlan");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(MembershipPlan), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/ProBroz.OnlineTraining/MembershipPlan/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "ProBroz.OnlineTraining/MembershipPlan/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("ProBroz.OnlineTraining", "MembershipPlan", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "ProBroz.OnlineTraining", "MembershipPlan", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref MembershipPlan customDemoObject);



				public static MembershipPlan CreateDefault()
				{
					var result = new MembershipPlan();
					return result;
				}
				/*
				public static MembershipPlan CreateDemoDefault()
				{
					MembershipPlan customDemo = null;
					MembershipPlan.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new MembershipPlan();
					result.PlanName = @"MembershipPlan.PlanName";

					result.Description = @"MembershipPlan.Description
MembershipPlan.Description
MembershipPlan.Description
MembershipPlan.Description
MembershipPlan.Description
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

			
                void IInformationObject.SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
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
						case "PlanName":
							PlanName = value;
							break;
						case "Description":
							Description = value;
							break;
						case "PaymentOptions":
							throw new NotImplementedException("Parsing collection types is not implemented for item collections");
							break;
						case "Gym":
							Gym = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
	        }
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
			[DataContract] 
			[Serializable]
			public partial class PaymentOption : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public PaymentOption()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "PaymentOption";
					UpdateRelativeLocationFromID();
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "ProBroz.OnlineTraining/PaymentOption/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(PaymentOption), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<PaymentOption>(ID);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: PaymentOption");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(PaymentOption), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/ProBroz.OnlineTraining/PaymentOption/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "ProBroz.OnlineTraining/PaymentOption/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
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
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
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
					CopyContentFrom((PaymentOption) sourceMaster);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("ProBroz.OnlineTraining", "PaymentOption", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "ProBroz.OnlineTraining", "PaymentOption", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref PaymentOption customDemoObject);



				public static PaymentOption CreateDefault()
				{
					var result = new PaymentOption();
					return result;
				}
				/*
				public static PaymentOption CreateDemoDefault()
				{
					PaymentOption customDemo = null;
					PaymentOption.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new PaymentOption();
					result.OptionName = @"PaymentOption.OptionName";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
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

						if(OptionName != _unmodified_OptionName)
							return true;
						if(PeriodInDays != _unmodified_PeriodInDays)
							return true;
						if(Price != _unmodified_Price)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(PaymentOption sourceObject)
				{
					OptionName = sourceObject.OptionName;
					PeriodInDays = sourceObject.PeriodInDays;
					Price = sourceObject.Price;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_OptionName = OptionName;
					_unmodified_PeriodInDays = PeriodInDays;
					_unmodified_Price = Price;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "OptionName":
							OptionName = value;
							break;
						case "PeriodInDays":
							PeriodInDays = long.Parse(value);
							break;
						case "Price":
							Price = double.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class Subscription : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public Subscription()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "Subscription";
					UpdateRelativeLocationFromID();
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "ProBroz.OnlineTraining/Subscription/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(Subscription), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<Subscription>(ID);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: Subscription");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(Subscription), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/ProBroz.OnlineTraining/Subscription/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "ProBroz.OnlineTraining/Subscription/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
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
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
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
					CopyContentFrom((Subscription) sourceMaster);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("ProBroz.OnlineTraining", "Subscription", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "ProBroz.OnlineTraining", "Subscription", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref Subscription customDemoObject);



				public static Subscription CreateDefault()
				{
					var result = new Subscription();
					return result;
				}
				/*
				public static Subscription CreateDemoDefault()
				{
					Subscription customDemo = null;
					Subscription.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new Subscription();
				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
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

						if(Plan != _unmodified_Plan)
							return true;
						if(PaymentOption != _unmodified_PaymentOption)
							return true;
						if(Created != _unmodified_Created)
							return true;
						if(ValidFrom != _unmodified_ValidFrom)
							return true;
						if(ValidTo != _unmodified_ValidTo)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


				private void CopyContentFrom(Subscription sourceObject)
				{
					Plan = sourceObject.Plan;
					PaymentOption = sourceObject.PaymentOption;
					Created = sourceObject.Created;
					ValidFrom = sourceObject.ValidFrom;
					ValidTo = sourceObject.ValidTo;
				}
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_Plan = Plan;
					_unmodified_PaymentOption = PaymentOption;
					_unmodified_Created = Created;
					_unmodified_ValidFrom = ValidFrom;
					_unmodified_ValidTo = ValidTo;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "Plan":
							Plan = value;
							break;
						case "PaymentOption":
							PaymentOption = value;
							break;
						case "Created":
							Created = DateTime.Parse(value);
							break;
						case "ValidFrom":
							ValidFrom = DateTime.Parse(value);
							break;
						case "ValidTo":
							ValidTo = DateTime.Parse(value);
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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
			[DataContract] 
			[Serializable]
			public partial class TenantGym : IInformationObject 
			{
		        public static StorageSerializationType ClassStorageSerializationType { 
					get {
						return StorageSerializationType.XML;
					}
				}

				public TenantGym()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.OwnerID = StorageSupport.ActiveOwnerID;
				    this.SemanticDomainName = "ProBroz.OnlineTraining";
				    this.Name = "TenantGym";
					UpdateRelativeLocationFromID();
				}

				public static IInformationObject[] RetrieveCollectionFromOwnerContent(IContainerOwner owner)
				{
					//string contentTypeName = ""; // SemanticDomainName + "." + Name
					string contentTypeName = "ProBroz.OnlineTraining/TenantGym/";
					List<IInformationObject> informationObjects = new List<IInformationObject>();
					var blobListing = StorageSupport.GetContentBlobListing(owner, contentType: contentTypeName);
					foreach(CloudBlockBlob blob in blobListing)
					{
						if (blob.GetBlobInformationType() != StorageSupport.InformationType_InformationObjectValue)
							continue;
						IInformationObject informationObject = StorageSupport.RetrieveInformation(blob.Name, typeof(TenantGym), null, owner);
					    informationObject.MasterETag = informationObject.ETag;
						informationObjects.Add(informationObject);
					}
					return informationObjects.ToArray();
				}

				public void UpdateRelativeLocationFromID()
				{
					RelativeLocation = ObjectStorage.GetRelativeLocationFromID<TenantGym>(ID);
				}

				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing, out bool initiated)
				{
					IInformationObject iObject = (IInformationObject) this;
					if(iObject.IsIndependentMaster == false)
						throw new NotSupportedException("Cannot retrieve master for non-master type: TenantGym");
					initiated = false;
					VirtualOwner owner = VirtualOwner.FigureOwner(this);
					var master = StorageSupport.RetrieveInformation(RelativeLocation, typeof(TenantGym), null, owner);
					if(master == null && initiateIfMissing)
					{
						StorageSupport.StoreInformation(this, owner);
						master = this;
						initiated = true;
					}
					return master;
				}


				IInformationObject IInformationObject.RetrieveMaster(bool initiateIfMissing)
				{
					bool initiated;
					IInformationObject iObject = this;
					return iObject.RetrieveMaster(initiateIfMissing, out initiated);
				}

				public void SetLocationAsOwnerContent(IContainerOwner containerOwner, string contentName)
                {
                    // RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "Content/ProBroz.OnlineTraining/TenantGym/" + contentName);
                    RelativeLocation = StorageSupport.GetOwnerContentLocation(containerOwner, "ProBroz.OnlineTraining/TenantGym/" + contentName);
                }

				partial void DoPostStoringExecute(IContainerOwner owner);

				public void PostStoringExecute(IContainerOwner owner)
				{
					DoPostStoringExecute(owner);
				}

				partial void DoPostDeleteExecute(IContainerOwner owner);

				public void PostDeleteExecute(IContainerOwner owner)
				{
					DoPostDeleteExecute(owner);
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
                        dynamic dyn = targetObject;
                        dyn.ParsePropertyValue(propertyName, propertyValue);
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
					CopyContentFrom((TenantGym) sourceMaster);
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

				public void SetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					RelativeLocation = GetRelativeLocationAsMetadataTo(masterRelativeLocation);
				}

				public static string GetRelativeLocationAsMetadataTo(string masterRelativeLocation)
				{
					return Path.Combine("ProBroz.OnlineTraining", "TenantGym", masterRelativeLocation + ".metadata").Replace("\\", "/"); 
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
                    relativeLocation = Path.Combine(contentRootLocation, "ProBroz.OnlineTraining", "TenantGym", sourceName).Replace("\\", "/");
                    return relativeLocation;
                }

				static partial void CreateCustomDemo(ref TenantGym customDemoObject);



				public static TenantGym CreateDefault()
				{
					var result = new TenantGym();
					return result;
				}
				/*
				public static TenantGym CreateDemoDefault()
				{
					TenantGym customDemo = null;
					TenantGym.CreateCustomDemo(ref customDemo);
					if(customDemo != null)
						return customDemo;
					var result = new TenantGym();
					result.GymName = @"TenantGym.GymName";

					result.Email = @"TenantGym.Email";

					result.PhoneNumber = @"TenantGym.PhoneNumber";

					result.Address = @"TenantGym.Address";

					result.Address2 = @"TenantGym.Address2";

					result.ZipCode = @"TenantGym.ZipCode";

					result.PostOffice = @"TenantGym.PostOffice";

					result.Country = @"TenantGym.Country";

				
					return result;
				}
				*/

				void IInformationObject.UpdateCollections(IInformationCollection masterInstance)
				{
					//Type collType = masterInstance.GetType();
					//string typeName = collType.Name;
				}

                public void SetMediaContent(IContainerOwner containerOwner, string contentObjectID, object mediaContent)
                {
                    IInformationObject targetObject = (IInformationObject) FindObjectByID(contentObjectID);
                    if (targetObject == null)
                        return;
					if(targetObject == this)
						throw new InvalidDataException("SetMediaContent referring to self (not media container)");
                    targetObject.SetMediaContent(containerOwner, contentObjectID, mediaContent);
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

						if(GymName != _unmodified_GymName)
							return true;
						if(Email != _unmodified_Email)
							return true;
						if(PhoneNumber != _unmodified_PhoneNumber)
							return true;
						if(Address != _unmodified_Address)
							return true;
						if(Address2 != _unmodified_Address2)
							return true;
						if(ZipCode != _unmodified_ZipCode)
							return true;
						if(PostOffice != _unmodified_PostOffice)
							return true;
						if(Country != _unmodified_Country)
							return true;
				
						return false;
					}
				}

				void IInformationObject.ReplaceObjectInTree(IInformationObject replacingObject)
				{
				}


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
				


				void IInformationObject.SetInstanceTreeValuesAsUnmodified()
				{
					_unmodified_GymName = GymName;
					_unmodified_Email = Email;
					_unmodified_PhoneNumber = PhoneNumber;
					_unmodified_Address = Address;
					_unmodified_Address2 = Address2;
					_unmodified_ZipCode = ZipCode;
					_unmodified_PostOffice = PostOffice;
					_unmodified_Country = Country;
				
				
				}


				public void ParsePropertyValue(string propertyName, string value)
				{
					switch (propertyName)
					{
						case "GymName":
							GymName = value;
							break;
						case "Email":
							Email = value;
							break;
						case "PhoneNumber":
							PhoneNumber = value;
							break;
						case "Address":
							Address = value;
							break;
						case "Address2":
							Address2 = value;
							break;
						case "ZipCode":
							ZipCode = value;
							break;
						case "PostOffice":
							PostOffice = value;
							break;
						case "Country":
							Country = value;
							break;
						default:
							throw new InvalidDataException("Primitive parseable data type property not found: " + propertyName);
					}
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