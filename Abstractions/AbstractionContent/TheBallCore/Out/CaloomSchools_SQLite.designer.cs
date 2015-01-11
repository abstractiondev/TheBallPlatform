 


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

namespace SQLite.Caloom.Schools { 
		
		public class TheBallDataContext : DataContext
		{

            public TheBallDataContext(IDbConnection connection) : base(connection)
		    {

		    }

			public Table<TrainingModule> TrainingModuleTable {
				get {
					return this.GetTable<TrainingModule>();
				}
			}
        }

    [Table(Name = "TrainingModule")]
	public class TrainingModule
	{
		[Column]
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
		public TrainingModuleCollection TrainingModules { get; set; }
		// private TrainingModuleCollection _unmodified_TrainingModules;
	}
 } 
