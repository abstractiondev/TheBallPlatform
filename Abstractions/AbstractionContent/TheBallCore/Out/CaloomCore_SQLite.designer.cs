 


using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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
using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
using ScaffoldTable=System.ComponentModel.DataAnnotations.ScaffoldTableAttribute;
using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;


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

            public static Func<DbConnection> GetCurrentConnectionFunc { get; set; }

		    public TheBallDataContext() : base(GetCurrentConnectionFunc())
		    {
		        
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
					existingObject.ETag = updateData.ETag;
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
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Product != null)
						existingObject.ProductID = serializedObject.Product.ID;
					else
						existingObject.ProductID = null;
					if(serializedObject.Who != null)
						existingObject.WhoID = serializedObject.Who.ID;
					else
						existingObject.WhoID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "Product")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.Product.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProductTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
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
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Product != null)
						existingObject.ProductID = serializedObject.Product.ID;
					else
						existingObject.ProductID = null;
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
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "RenderedNode")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.Caloom.CORE.RenderedNode.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RenderedNodeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
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
					existingObject.ETag = updateData.ETag;
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
                    var objectToAdd = new Who {ID = insertData.ObjectID, ETag = insertData.ETag};
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
                    var objectToAdd = new ProductForWhom {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Product != null)
						objectToAdd.ProductID = serializedObject.Product.ID;
					else
						objectToAdd.ProductID = null;
					if(serializedObject.Who != null)
						objectToAdd.WhoID = serializedObject.Who.ID;
					else
						objectToAdd.WhoID = null;
					ProductForWhomTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Product")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.Product.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Product {ID = insertData.ObjectID, ETag = insertData.ETag};
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
                    var objectToAdd = new ProductUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Product != null)
						objectToAdd.ProductID = serializedObject.Product.ID;
					else
						objectToAdd.ProductID = null;
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
                    var objectToAdd = new NodeSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					NodeSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RenderedNode")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Caloom.CORE.RenderedNode.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RenderedNode {ID = insertData.ObjectID, ETag = insertData.ETag};
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
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
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
		            var objectToDelete = new Who {ID = deleteData.ID};
                    WhoTable.Attach(objectToDelete);
                    WhoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProductForWhom")
		        {
		            var objectToDelete = new ProductForWhom {ID = deleteData.ID};
                    ProductForWhomTable.Attach(objectToDelete);
                    ProductForWhomTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Product")
		        {
		            var objectToDelete = new Product {ID = deleteData.ID};
                    ProductTable.Attach(objectToDelete);
                    ProductTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProductUsage")
		        {
		            var objectToDelete = new ProductUsage {ID = deleteData.ID};
                    ProductUsageTable.Attach(objectToDelete);
                    ProductUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "NodeSummaryContainer")
		        {
		            var objectToDelete = new NodeSummaryContainer {ID = deleteData.ID};
                    NodeSummaryContainerTable.Attach(objectToDelete);
                    NodeSummaryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "RenderedNode")
		        {
		            var objectToDelete = new RenderedNode {ID = deleteData.ID};
                    RenderedNodeTable.Attach(objectToDelete);
                    RenderedNodeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ShortTextObject")
		        {
		            var objectToDelete = new ShortTextObject {ID = deleteData.ID};
                    ShortTextObjectTable.Attach(objectToDelete);
                    ShortTextObjectTable.DeleteOnSubmit(objectToDelete);
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
	[ScaffoldTable(true)]
	public class Who : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Who](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Who() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
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
	[ScaffoldTable(true)]
	public class ProductForWhom : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProductForWhom](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ProductID] TEXT NULL, 
[WhoID] TEXT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ProductForWhom() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
			[Column]
			public string ProductID { get; set; }
			private EntityRef< Product > _Product;
			[Association(Storage = "_Product", ThisKey = "ProductID")]
			public Product Product
			{
				get { return this._Product.Entity; }
				set { this._Product.Entity = value; }
			}

			[Column]
			public string WhoID { get; set; }
			private EntityRef< Who > _Who;
			[Association(Storage = "_Who", ThisKey = "WhoID")]
			public Who Who
			{
				get { return this._Who.Entity; }
				set { this._Who.Entity = value; }
			}

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
	[ScaffoldTable(true)]
	public class Product : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Product](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[SubProductsID] TEXT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Product() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
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
	[ScaffoldTable(true)]
	public class ProductUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProductUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ProductID] TEXT NULL, 
[UsageAmountInDecimal] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ProductUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string ProductID { get; set; }
			private EntityRef< Product > _Product;
			[Association(Storage = "_Product", ThisKey = "ProductID")]
			public Product Product
			{
				get { return this._Product.Entity; }
				set { this._Product.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public double UsageAmountInDecimal { get; set; }
		// private double _unmodified_UsageAmountInDecimal;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	[ScaffoldTable(true)]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NodeSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[NodesID] TEXT NULL, 
[NodeSourceProductsID] TEXT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public NodeSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	[ScaffoldTable(true)]
	public class RenderedNode : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RenderedNode](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[TechnicalSource] TEXT NOT NULL, 
[ImageBaseUrl] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[ActualContentUrl] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[TimestampText] TEXT NOT NULL, 
[MainSortableText] TEXT NOT NULL, 
[CategoriesID] TEXT NULL, 
[AuthorsID] TEXT NULL, 
[LocationsID] TEXT NULL, 
[FiltersID] TEXT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public RenderedNode() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string TechnicalSource { get; set; }
		// private string _unmodified_TechnicalSource;

		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string ActualContentUrl { get; set; }
		// private string _unmodified_ActualContentUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string TimestampText { get; set; }
		// private string _unmodified_TimestampText;

		[Column]
        [ScaffoldColumn(true)]
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
	[ScaffoldTable(true)]
	public class ShortTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ShortTextObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ShortTextObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Content == null)
				Content = string.Empty;
		}
	}
 } 
