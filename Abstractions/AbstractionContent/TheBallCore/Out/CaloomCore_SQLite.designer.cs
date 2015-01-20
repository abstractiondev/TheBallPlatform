 


using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using SQLiteSupport;


namespace SQLite.Caloom.CORE { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DataContext, IStorageSyncableDataContext
		{
            // Track whether Dispose has been called. 
            private bool disposed = false;
		    protected override void Dispose(bool disposing)
		    {
		        if (disposed)
		            return;
                base.Dispose(disposing);
                GC.Collect();
                GC.WaitForPendingFinalizers();
		        disposed = true;
		    }

		    public static TheBallDataContext CreateOrAttachToExistingDB(string pathToDBFile)
		    {
		        SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0}", pathToDBFile));
                var dataContext = new TheBallDataContext(connection);
				dataContext.CreateDomainDatabaseTablesIfNotExists();
				return dataContext;
		    }

            public TheBallDataContext(SQLiteConnection connection) : base(connection)
		    {
                if(connection.State != ConnectionState.Open)
                    connection.Open();
		    }

            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var insertsToProcess = changeSet.Inserts.Where(insert => insert is ITheBallDataContextStorable).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in insertsToProcess)
                    itemToProcess.PrepareForStoring(true);
                var updatesToProcess = changeSet.Updates.Where(update => update is ITheBallDataContextStorable).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in updatesToProcess)
                    itemToProcess.PrepareForStoring(false);
                base.SubmitChanges(failureMode);
            }

			public void CreateDomainDatabaseTablesIfNotExists()
			{
				List<string> tableCreationCommands = new List<string>();
                tableCreationCommands.AddRange(InformationObjectMetaData.GetMetaDataTableCreateSQLs());
				tableCreationCommands.Add(Who.GetCreateTableSQL());
				tableCreationCommands.Add(ProductForWhom.GetCreateTableSQL());
				tableCreationCommands.Add(Product.GetCreateTableSQL());
				tableCreationCommands.Add(ProductUsage.GetCreateTableSQL());
				tableCreationCommands.Add(NodeSummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(RenderedNode.GetCreateTableSQL());
				tableCreationCommands.Add(ShortTextObject.GetCreateTableSQL());
			    var connection = this.Connection;
				foreach (string commandText in tableCreationCommands)
			    {
			        var command = connection.CreateCommand();
			        command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
			        command.ExecuteNonQuery();
			    }
			}

			public Table<InformationObjectMetaData> InformationObjectMetaDataTable {
				get {
					return this.GetTable<InformationObjectMetaData>();
				}
			}

			public void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "Caloom.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "Who")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.Who.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = WhoTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "ProductForWhom")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.ProductForWhom.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProductForWhomTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "Product")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.Product.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProductTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "ProductUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.ProductUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProductUsageTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.UsageAmountInDecimal = serializedObject.UsageAmountInDecimal;
		            return;
		        } 
		        if (updateData.ObjectType == "NodeSummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.NodeSummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NodeSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
		            return;
		        } 
		        if (updateData.ObjectType == "RenderedNode")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.RenderedNode.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RenderedNodeTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.TechnicalSource = serializedObject.TechnicalSource;
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.ActualContentUrl = serializedObject.ActualContentUrl;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.TimestampText = serializedObject.TimestampText;
		            existingObject.MainSortableText = serializedObject.MainSortableText;
		            return;
		        } 
		        if (updateData.ObjectType == "ShortTextObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.ShortTextObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ShortTextObjectTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Content = serializedObject.Content;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "Caloom.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "Who")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.Who.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Who {ID = insertData.ObjectID};
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
					WhoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProductForWhom")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.ProductForWhom.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProductForWhom {ID = insertData.ObjectID};
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
					ProductForWhomTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Product")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.Product.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Product {ID = insertData.ObjectID};
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
					ProductTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProductUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.ProductUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProductUsage {ID = insertData.ObjectID};
		            objectToAdd.UsageAmountInDecimal = serializedObject.UsageAmountInDecimal;
					ProductUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "NodeSummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.NodeSummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NodeSummaryContainer {ID = insertData.ObjectID};
					NodeSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RenderedNode")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.RenderedNode.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RenderedNode {ID = insertData.ObjectID};
		            objectToAdd.TechnicalSource = serializedObject.TechnicalSource;
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.ActualContentUrl = serializedObject.ActualContentUrl;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.TimestampText = serializedObject.TimestampText;
		            objectToAdd.MainSortableText = serializedObject.MainSortableText;
					RenderedNodeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ShortTextObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.ShortTextObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID};
		            objectToAdd.Content = serializedObject.Content;
					ShortTextObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "Caloom.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "Who")
		        {
                    WhoTable.DeleteOnSubmit(new Who { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ProductForWhom")
		        {
                    ProductForWhomTable.DeleteOnSubmit(new ProductForWhom { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Product")
		        {
                    ProductTable.DeleteOnSubmit(new Product { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ProductUsage")
		        {
                    ProductUsageTable.DeleteOnSubmit(new ProductUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "NodeSummaryContainer")
		        {
                    NodeSummaryContainerTable.DeleteOnSubmit(new NodeSummaryContainer { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "RenderedNode")
		        {
                    RenderedNodeTable.DeleteOnSubmit(new RenderedNode { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ShortTextObject")
		        {
                    ShortTextObjectTable.DeleteOnSubmit(new ShortTextObject { ID = deleteData.ObjectID });
		            return;
		        }
		    }


			public Table<Who> WhoTable {
				get {
					return this.GetTable<Who>();
				}
			}
			public Table<ProductForWhom> ProductForWhomTable {
				get {
					return this.GetTable<ProductForWhom>();
				}
			}
			public Table<Product> ProductTable {
				get {
					return this.GetTable<Product>();
				}
			}
			public Table<ProductUsage> ProductUsageTable {
				get {
					return this.GetTable<ProductUsage>();
				}
			}
			public Table<NodeSummaryContainer> NodeSummaryContainerTable {
				get {
					return this.GetTable<NodeSummaryContainer>();
				}
			}
			public Table<RenderedNode> RenderedNodeTable {
				get {
					return this.GetTable<RenderedNode>();
				}
			}
			public Table<ShortTextObject> ShortTextObjectTable {
				get {
					return this.GetTable<ShortTextObject>();
				}
			}
        }

    [Table(Name = "Who")]
	public class Who : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Who(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "ProductForWhom")]
	public class ProductForWhom : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProductForWhom(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "Product")]
	public class Product : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Product(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "ProductUsage")]
	public class ProductUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProductUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[UsageAmountInDecimal] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public double UsageAmountInDecimal { get; set; }
		// private double _unmodified_UsageAmountInDecimal;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS NodeSummaryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 

)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	public class RenderedNode : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS RenderedNode(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TechnicalSource] TEXT NOT NULL, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[ActualContentUrl] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[TimestampText] TEXT NOT NULL, 
[MainSortableText] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TechnicalSource { get; set; }
		// private string _unmodified_TechnicalSource;

		[Column]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string ActualContentUrl { get; set; }
		// private string _unmodified_ActualContentUrl;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string TimestampText { get; set; }
		// private string _unmodified_TimestampText;

		[Column]
		public string MainSortableText { get; set; }
		// private string _unmodified_MainSortableText;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TechnicalSource == null)
				TechnicalSource = string.Empty;
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(ActualContentUrl == null)
				ActualContentUrl = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(TimestampText == null)
				TimestampText = string.Empty;
			if(MainSortableText == null)
				MainSortableText = string.Empty;
		}
	}
    [Table(Name = "ShortTextObject")]
	public class ShortTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ShortTextObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Content == null)
				Content = string.Empty;
		}
	}
 } 
