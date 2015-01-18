 


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

namespace SQLite.Caloom.CORE { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		[Flags]
		public enum SerializationType 
		{
			Undefined = 0,
			XML = 1,
			JSON = 2,
			XML_AND_JSON = XML | JSON
		}

		[Table]
		public class InformationObjectMetaData
		{
			[Column(IsPrimaryKey = true)]
			public string ID { get; set; }

			[Column]
			public string SemanticDomain { get; set; }
			[Column]
			public string ObjectType { get; set; }
			[Column]
			public string ObjectID { get; set; }
			[Column]
			public string MD5 { get; set; }
			[Column]
			public string LastWriteTime { get; set; }
			[Column]
			public long FileLength { get; set; }
			[Column]
			public SerializationType SerializationType { get; set; }

            public ChangeAction CurrentChangeAction { get; set; }
		}


		public class TheBallDataContext : DataContext
		{

		    public static string[] GetMetaDataTableCreateSQLs()
		    {
		        return new string[]
		        {
		            @"
CREATE TABLE IF NOT EXISTS InformationObjectMetaData(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SemanticDomain] TEXT NOT NULL, 
[ObjectType] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL,
[MD5] TEXT NOT NULL,
[LastWriteTime] TEXT NOT NULL,
[FileLength] INTEGER NOT NULL,
[SerializationType] INTEGER NOT NULL
)",
		            @"
CREATE UNIQUE INDEX ObjectIX ON InformationObjectMetaData (
SemanticDomain, 
ObjectType, 
ObjectID
)"
		        };
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
                tableCreationCommands.AddRange(GetMetaDataTableCreateSQLs());
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
[Description] TEXT NOT NULL, 
[Product] TEXT NOT NULL, 
[Who] TEXT NOT NULL
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

		[Column]
		public Product Product { get; set; }
		// private Product _unmodified_Product;

		[Column]
		public Who Who { get; set; }
		// private Who _unmodified_Who;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
[Description] TEXT NOT NULL, 
[SubProducts] TEXT NOT NULL
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

		[Column]
		public ProductUsageCollection SubProducts { get; set; }
		// private ProductUsageCollection _unmodified_SubProducts;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
[Product] TEXT NOT NULL, 
[UsageAmountInDecimal] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Product Product { get; set; }
		// private Product _unmodified_Product;

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
[Nodes] TEXT NOT NULL, 
[NodeSourceProducts] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public RenderedNodeCollection Nodes { get; set; }
		// private RenderedNodeCollection _unmodified_Nodes;

		[Column]
		public ProductCollection NodeSourceProducts { get; set; }
		// private ProductCollection _unmodified_NodeSourceProducts;
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
[MainSortableText] TEXT NOT NULL, 
[Categories] TEXT NOT NULL, 
[Authors] TEXT NOT NULL, 
[Locations] TEXT NOT NULL, 
[Filters] TEXT NOT NULL
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

		[Column]
		public ShortTextCollection Categories { get; set; }
		// private ShortTextCollection _unmodified_Categories;

		[Column]
		public ShortTextCollection Authors { get; set; }
		// private ShortTextCollection _unmodified_Authors;

		[Column]
		public ShortTextCollection Locations { get; set; }
		// private ShortTextCollection _unmodified_Locations;

		[Column]
		public ShortTextCollection Filters { get; set; }
		// private ShortTextCollection _unmodified_Filters;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
		
		}
	}
 } 
