 


using System;
using System.Data;
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
		void PrepareForStoring();
	}


		public class TheBallDataContext : DataContext
		{

            public TheBallDataContext(IDbConnection connection) : base(connection)
		    {

		    }

            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var requiringBeforeSaveProcessing = changeSet.Inserts.Concat(changeSet.Updates).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in requiringBeforeSaveProcessing)
                    itemToProcess.PrepareForStoring();
                base.SubmitChanges(failureMode);
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "ProductForWhom")]
	public class ProductForWhom : ITheBallDataContextStorable
	{
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "Product")]
	public class Product : ITheBallDataContextStorable
	{
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "ProductUsage")]
	public class ProductUsage : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Product Product { get; set; }
		// private Product _unmodified_Product;

		[Column]
		public double UsageAmountInDecimal { get; set; }
		// private double _unmodified_UsageAmountInDecimal;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public RenderedNodeCollection Nodes { get; set; }
		// private RenderedNodeCollection _unmodified_Nodes;

		[Column]
		public ProductCollection NodeSourceProducts { get; set; }
		// private ProductCollection _unmodified_NodeSourceProducts;
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	public class RenderedNode : ITheBallDataContextStorable
	{
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
        public void PrepareForStoring()
        {
		
		}
	}
    [Table(Name = "ShortTextObject")]
	public class ShortTextObject : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring()
        {
		
		}
	}
 } 
