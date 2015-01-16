 

namespace Caloom.Housing { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;


namespace INT { 
		 } 			[DataContract]
			[Serializable]
			public partial class House 
			{

				public House()
				{
					this.ID = Guid.NewGuid().ToString();
				    this.SemanticDomainName = "Caloom.Housing";
				    this.Name = "House";
				}


				public string SerializeToXml(bool noFormatting = false)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(House));
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

				public static House DeserializeFromXml(string xmlString)
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(House));
					using(StringReader reader = new StringReader(xmlString))
					{
						using (var xmlReader = new XmlTextReader(reader))
							return (House) serializer.ReadObject(xmlReader);
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



				private void CopyContentFrom(House sourceObject)
				{
					ImageBaseUrl = sourceObject.ImageBaseUrl;
					Title = sourceObject.Title;
					Excerpt = sourceObject.Excerpt;
					Description = sourceObject.Description;
				}
				



			[DataMember]
			public string ImageBaseUrl { get; set; }
			private string _unmodified_ImageBaseUrl;
			[DataMember]
			public string Title { get; set; }
			private string _unmodified_Title;
			[DataMember]
			public string Excerpt { get; set; }
			private string _unmodified_Excerpt;
			[DataMember]
			public string Description { get; set; }
			private string _unmodified_Description;
			
			}
 } 