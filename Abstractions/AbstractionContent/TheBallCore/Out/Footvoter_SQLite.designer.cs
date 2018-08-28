 


using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using SQLiteSupport;
using Key=System.ComponentModel.DataAnnotations.KeyAttribute;
//using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
//using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SQLite.Footvoter.Services { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DbContext, IStorageSyncableDataContext
		{
            // Track whether Dispose has been called. 
            private bool disposed = false;
		    void IDisposable.Dispose()
		    {
		        if (disposed)
		            return;
                GC.Collect();
                GC.WaitForPendingFinalizers();
		        disposed = true;
		    }

            public static Func<DbConnection> GetCurrentConnectionFunc { get; set; }

		    public TheBallDataContext() : base(new DbContextOptions<TheBallDataContext>())
		    {
		        
		    }

		    public readonly string SQLiteDBPath;
		    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		    {
		        optionsBuilder.UseSqlite($"Filename={SQLiteDBPath}");
		    }

		    public static TheBallDataContext CreateOrAttachToExistingDB(string pathToDBFile)
		    {
		        var sqliteConnectionString = $"{pathToDBFile}";
                var dataContext = new TheBallDataContext(sqliteConnectionString);
		        var db = dataContext.Database;
                db.OpenConnection();
		        using (var transaction = db.BeginTransaction())
		        {
                    dataContext.CreateDomainDatabaseTablesIfNotExists();
                    transaction.Commit();
		        }
                return dataContext;
		    }

            public TheBallDataContext(string sqLiteDBPath) : base()
            {
                SQLiteDBPath = sqLiteDBPath;
            }

		    public override int SaveChanges(bool acceptAllChangesOnSuccess)
		    {
                //if(connection.State != ConnectionState.Open)
                //    connection.Open();
		        return base.SaveChanges(acceptAllChangesOnSuccess);
		    }

			/*
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

		    public async Task SubmitChangesAsync()
		    {
		        await Task.Run(() => SubmitChanges());
		    }
			*/

			public void CreateDomainDatabaseTablesIfNotExists()
			{
				List<string> tableCreationCommands = new List<string>();
                tableCreationCommands.AddRange(InformationObjectMetaData.GetMetaDataTableCreateSQLs());
				tableCreationCommands.Add(Company.GetCreateTableSQL());
				tableCreationCommands.Add(Vote.GetCreateTableSQL());
			    //var connection = this.Connection;
			    var db = this.Database;
			    var connection = db.GetDbConnection();
				foreach (string commandText in tableCreationCommands)
			    {
			        var command = connection.CreateCommand();
			        command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
			        command.ExecuteNonQuery();
			    }
			}

			public DbSet<InformationObjectMetaData> InformationObjectMetaDataTable {
				get {
					return this.Set<InformationObjectMetaData>();
				}
			}


			public void PerformUpdate(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "Company":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.Footvoter.Services.Company.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CompanyTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CompanyName = serializedObject.CompanyName;
		            existingObject.Details = serializedObject.Details;
		            existingObject.Footprint = serializedObject.Footprint;
                    existingObject.Footpath.Clear();
					if(serializedObject.Footpath != null)
	                    serializedObject.Footpath.ForEach(item => existingObject.Footpath.Add(item));
					
		            break;
		        } 
		        case "Vote":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.Footvoter.Services.Vote.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = VoteTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CompanyID = serializedObject.CompanyID;
		            existingObject.VoteValue = serializedObject.VoteValue;
		            existingObject.VoteTime = serializedObject.VoteTime;
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "Company":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.Footvoter.Services.Company.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CompanyTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CompanyName = serializedObject.CompanyName;
		            existingObject.Details = serializedObject.Details;
		            existingObject.Footprint = serializedObject.Footprint;
                    existingObject.Footpath.Clear();
					if(serializedObject.Footpath != null)
	                    serializedObject.Footpath.ForEach(item => existingObject.Footpath.Add(item));
					
		            break;
		        } 
		        case "Vote":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.Footvoter.Services.Vote.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = VoteTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CompanyID = serializedObject.CompanyID;
		            existingObject.VoteValue = serializedObject.VoteValue;
		            existingObject.VoteTime = serializedObject.VoteTime;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "Company":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Footvoter.Services.Company.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Company {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CompanyName = serializedObject.CompanyName;
		            objectToAdd.Details = serializedObject.Details;
		            objectToAdd.Footprint = serializedObject.Footprint;
					if(serializedObject.Footpath != null)
						serializedObject.Footpath.ForEach(item => objectToAdd.Footpath.Add(item));
					CompanyTable.Add(objectToAdd);
                    break;
                }
                case "Vote":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Footvoter.Services.Vote.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Vote {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CompanyID = serializedObject.CompanyID;
		            objectToAdd.VoteValue = serializedObject.VoteValue;
		            objectToAdd.VoteTime = serializedObject.VoteTime;
					VoteTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "Company":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Footvoter.Services.Company.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Company {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CompanyName = serializedObject.CompanyName;
		            objectToAdd.Details = serializedObject.Details;
		            objectToAdd.Footprint = serializedObject.Footprint;
					if(serializedObject.Footpath != null)
						serializedObject.Footpath.ForEach(item => objectToAdd.Footpath.Add(item));
					CompanyTable.Add(objectToAdd);
                    break;
                }
                case "Vote":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.Footvoter.Services.Vote.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Vote {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CompanyID = serializedObject.CompanyID;
		            objectToAdd.VoteValue = serializedObject.VoteValue;
		            objectToAdd.VoteTime = serializedObject.VoteTime;
					VoteTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Company":
					{
						//var objectToDelete = new Company {ID = deleteData.ObjectID};
						//CompanyTable.Attach(objectToDelete);
						var objectToDelete = CompanyTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CompanyTable.Remove(objectToDelete);
						break;
					}
					case "Vote":
					{
						//var objectToDelete = new Vote {ID = deleteData.ObjectID};
						//VoteTable.Attach(objectToDelete);
						var objectToDelete = VoteTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							VoteTable.Remove(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "Footvoter.Services")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Company":
					{
						//var objectToDelete = new Company {ID = deleteData.ObjectID};
						//CompanyTable.Attach(objectToDelete);
						var objectToDelete = CompanyTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CompanyTable.Remove(objectToDelete);
						break;
					}
					case "Vote":
					{
						//var objectToDelete = new Vote {ID = deleteData.ObjectID};
						//VoteTable.Attach(objectToDelete);
						var objectToDelete = VoteTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							VoteTable.Remove(objectToDelete);
						break;
					}
				}
			}



			public DbSet<Company> CompanyTable { get; set; }
			public DbSet<Vote> VoteTable { get; set; }
        }

    //[Table(Name = "Company")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Company: {ID}")]
	public class Company : ITheBallDataContextStorable
	{

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public Company() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Company](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CompanyName] TEXT NOT NULL, 
[Details] TEXT NOT NULL, 
[Footprint] REAL NOT NULL, 
[Footpath] TEXT NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string CompanyName { get; set; }
		// private string _unmodified_CompanyName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Details { get; set; }
		// private string _unmodified_Details;

		//[Column]
        //[ScaffoldColumn(true)]
		public double Footprint { get; set; }
		// private double _unmodified_Footprint;
        //[Column(Name = "Footpath")] 
        //[ScaffoldColumn(true)]
		public string FootpathData { get; set; }

        private bool _IsFootpathRetrieved = false;
        private bool _IsFootpathChanged = false;
        private ObservableCollection<double> _Footpath = null;
        public ObservableCollection<double> Footpath
        {
            get
            {
                if (!_IsFootpathRetrieved)
                {
                    if (FootpathData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<double[]>(FootpathData);
                        _Footpath = new ObservableCollection<double>(arrayData);
                    }
                    else
                    {
                        _Footpath = new ObservableCollection<double>();
						FootpathData = Guid.NewGuid().ToString();
						_IsFootpathChanged = true;
                    }
                    _IsFootpathRetrieved = true;
                    _Footpath.CollectionChanged += (sender, args) =>
						{
							FootpathData = Guid.NewGuid().ToString();
							_IsFootpathChanged = true;
						};
                }
                return _Footpath;
            }
            set 
			{ 
				_Footpath = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsFootpathRetrieved = true;
                FootpathData = Guid.NewGuid().ToString();
                _IsFootpathChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CompanyName == null)
				CompanyName = string.Empty;
			if(Details == null)
				Details = string.Empty;
            if (_IsFootpathChanged || isInitialInsert)
            {
                var dataToStore = Footpath.ToArray();
                FootpathData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    //[Table(Name = "Vote")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Vote: {ID}")]
	public class Vote : ITheBallDataContextStorable
	{

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public Vote() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Vote](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CompanyID] TEXT NOT NULL, 
[VoteValue] INTEGER NOT NULL, 
[VoteTime] TEXT NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string CompanyID { get; set; }
		// private string _unmodified_CompanyID;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool VoteValue { get; set; }
		// private bool _unmodified_VoteValue;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime VoteTime { get; set; }
		// private DateTime _unmodified_VoteTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CompanyID == null)
				CompanyID = string.Empty;
		}
	}
 } 
