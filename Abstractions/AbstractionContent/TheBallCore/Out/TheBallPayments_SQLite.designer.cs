 


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


namespace SQLite.TheBall.Payments { 
		
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
				tableCreationCommands.Add(GroupSubscriptionPlan.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionPlanStatus.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionPlanStatusSubscriptionPlan.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccount.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccountActivePlans.GetCreateTableSQL());
				tableCreationCommands.Add(GroupSubscriptionPlanCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CustomerAccountCollection.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "GroupSubscriptionPlan":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupSubscriptionPlanTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PlanName = serializedObject.PlanName;
		            existingObject.Description = serializedObject.Description;
                    existingObject.GroupIDs.Clear();
					if(serializedObject.GroupIDs != null)
	                    serializedObject.GroupIDs.ForEach(item => existingObject.GroupIDs.Add(item));
					
		            break;
		        } 
		        case "SubscriptionPlanStatus":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.SubscriptionPlanStatus.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionPlanStatusTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.SubscriptionPlan != null)
                    {
                            var relationObject = new SubscriptionPlanStatusSubscriptionPlan
                            {
                                SubscriptionPlanStatusID = existingObject.ID,
                                GroupSubscriptionPlanID = serializedObject.SubscriptionPlan
                            };
                            SubscriptionPlanStatusSubscriptionPlanTable.Add(relationObject);
							existingObject.SubscriptionPlan = relationObject;
                    }

		            existingObject.ValidUntil = serializedObject.ValidUntil;
		            break;
		        } 
		        case "CustomerAccount":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CustomerAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StripeID = serializedObject.StripeID;
		            existingObject.IsTestAccount = serializedObject.IsTestAccount;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.Description = serializedObject.Description;
                    if (serializedObject.ActivePlans != null)
                    {
						existingObject.ActivePlans.Clear();
                        serializedObject.ActivePlans.ForEach(
                            item =>
                            {
                                var relationObject = new CustomerAccountActivePlans
                                {
                                    CustomerAccountID = existingObject.ID,
                                    SubscriptionPlanStatusID = item
                                };
                                CustomerAccountActivePlansTable.Add(relationObject);
                                existingObject.ActivePlans.Add(relationObject);

                            });
                    }

		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "GroupSubscriptionPlan":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupSubscriptionPlanTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PlanName = serializedObject.PlanName;
		            existingObject.Description = serializedObject.Description;
                    existingObject.GroupIDs.Clear();
					if(serializedObject.GroupIDs != null)
	                    serializedObject.GroupIDs.ForEach(item => existingObject.GroupIDs.Add(item));
					
		            break;
		        } 
		        case "SubscriptionPlanStatus":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.SubscriptionPlanStatus.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = SubscriptionPlanStatusTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.SubscriptionPlan != null)
                    {
                            var relationObject = new SubscriptionPlanStatusSubscriptionPlan
                            {
                                SubscriptionPlanStatusID = existingObject.ID,
                                GroupSubscriptionPlanID = serializedObject.SubscriptionPlan
                            };
                            SubscriptionPlanStatusSubscriptionPlanTable.Add(relationObject);
							existingObject.SubscriptionPlan = relationObject;
                    }

		            existingObject.ValidUntil = serializedObject.ValidUntil;
		            break;
		        } 
		        case "CustomerAccount":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CustomerAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StripeID = serializedObject.StripeID;
		            existingObject.IsTestAccount = serializedObject.IsTestAccount;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.Description = serializedObject.Description;
                    if (serializedObject.ActivePlans != null)
                    {
						existingObject.ActivePlans.Clear();
                        serializedObject.ActivePlans.ForEach(
                            item =>
                            {
                                var relationObject = new CustomerAccountActivePlans
                                {
                                    CustomerAccountID = existingObject.ID,
                                    SubscriptionPlanStatusID = item
                                };
                                CustomerAccountActivePlansTable.Add(relationObject);
                                existingObject.ActivePlans.Add(relationObject);

                            });
                    }

		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "GroupSubscriptionPlan":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupSubscriptionPlan {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PlanName = serializedObject.PlanName;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.GroupIDs != null)
						serializedObject.GroupIDs.ForEach(item => objectToAdd.GroupIDs.Add(item));
					GroupSubscriptionPlanTable.Add(objectToAdd);
                    break;
                }
                case "SubscriptionPlanStatus":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.SubscriptionPlanStatus.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SubscriptionPlanStatus {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.SubscriptionPlan != null)
                    {
                            var relationObject = new SubscriptionPlanStatusSubscriptionPlan
                            {
                                SubscriptionPlanStatusID = objectToAdd.ID,
                                GroupSubscriptionPlanID = serializedObject.SubscriptionPlan
                            };
                            SubscriptionPlanStatusSubscriptionPlanTable.Add(relationObject);
                            objectToAdd.SubscriptionPlan = relationObject;
                    }

		            objectToAdd.ValidUntil = serializedObject.ValidUntil;
					SubscriptionPlanStatusTable.Add(objectToAdd);
                    break;
                }
                case "CustomerAccount":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CustomerAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StripeID = serializedObject.StripeID;
		            objectToAdd.IsTestAccount = serializedObject.IsTestAccount;
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.Description = serializedObject.Description;
                    if (serializedObject.ActivePlans != null)
                    {
                        serializedObject.ActivePlans.ForEach(
                            item =>
                            {
                                var relationObject = new CustomerAccountActivePlans
                                {
                                    CustomerAccountID = objectToAdd.ID,
                                    SubscriptionPlanStatusID = item
                                };
                                CustomerAccountActivePlansTable.Add(relationObject);
                                objectToAdd.ActivePlans.Add(relationObject);

                            });
                    }

					CustomerAccountTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "GroupSubscriptionPlan":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.GroupSubscriptionPlan.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GroupSubscriptionPlan {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PlanName = serializedObject.PlanName;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.GroupIDs != null)
						serializedObject.GroupIDs.ForEach(item => objectToAdd.GroupIDs.Add(item));
					GroupSubscriptionPlanTable.Add(objectToAdd);
                    break;
                }
                case "SubscriptionPlanStatus":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.SubscriptionPlanStatus.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new SubscriptionPlanStatus {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.SubscriptionPlan != null)
                    {
                            var relationObject = new SubscriptionPlanStatusSubscriptionPlan
                            {
                                SubscriptionPlanStatusID = objectToAdd.ID,
                                GroupSubscriptionPlanID = serializedObject.SubscriptionPlan
                            };
                            SubscriptionPlanStatusSubscriptionPlanTable.Add(relationObject);
                            objectToAdd.SubscriptionPlan = relationObject;
                    }

		            objectToAdd.ValidUntil = serializedObject.ValidUntil;
					SubscriptionPlanStatusTable.Add(objectToAdd);
                    break;
                }
                case "CustomerAccount":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Payments.CustomerAccount.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new CustomerAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StripeID = serializedObject.StripeID;
		            objectToAdd.IsTestAccount = serializedObject.IsTestAccount;
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.Description = serializedObject.Description;
                    if (serializedObject.ActivePlans != null)
                    {
                        serializedObject.ActivePlans.ForEach(
                            item =>
                            {
                                var relationObject = new CustomerAccountActivePlans
                                {
                                    CustomerAccountID = objectToAdd.ID,
                                    SubscriptionPlanStatusID = item
                                };
                                CustomerAccountActivePlansTable.Add(relationObject);
                                objectToAdd.ActivePlans.Add(relationObject);

                            });
                    }

					CustomerAccountTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "GroupSubscriptionPlan":
					{
						//var objectToDelete = new GroupSubscriptionPlan {ID = deleteData.ObjectID};
						//GroupSubscriptionPlanTable.Attach(objectToDelete);
						var objectToDelete = GroupSubscriptionPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSubscriptionPlanTable.Remove(objectToDelete);
						break;
					}
					case "SubscriptionPlanStatus":
					{
						//var objectToDelete = new SubscriptionPlanStatus {ID = deleteData.ObjectID};
						//SubscriptionPlanStatusTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionPlanStatusTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionPlanStatusTable.Remove(objectToDelete);
						break;
					}
					case "CustomerAccount":
					{
						//var objectToDelete = new CustomerAccount {ID = deleteData.ObjectID};
						//CustomerAccountTable.Attach(objectToDelete);
						var objectToDelete = CustomerAccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CustomerAccountTable.Remove(objectToDelete);
						break;
					}
					case "GroupSubscriptionPlanCollection":
					{
						//var objectToDelete = new GroupSubscriptionPlanCollection {ID = deleteData.ObjectID};
						//GroupSubscriptionPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = GroupSubscriptionPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSubscriptionPlanCollectionTable.Remove(objectToDelete);
						break;
					}
					case "CustomerAccountCollection":
					{
						//var objectToDelete = new CustomerAccountCollection {ID = deleteData.ObjectID};
						//CustomerAccountCollectionTable.Attach(objectToDelete);
						var objectToDelete = CustomerAccountCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CustomerAccountCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "GroupSubscriptionPlan":
					{
						//var objectToDelete = new GroupSubscriptionPlan {ID = deleteData.ObjectID};
						//GroupSubscriptionPlanTable.Attach(objectToDelete);
						var objectToDelete = GroupSubscriptionPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSubscriptionPlanTable.Remove(objectToDelete);
						break;
					}
					case "SubscriptionPlanStatus":
					{
						//var objectToDelete = new SubscriptionPlanStatus {ID = deleteData.ObjectID};
						//SubscriptionPlanStatusTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionPlanStatusTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionPlanStatusTable.Remove(objectToDelete);
						break;
					}
					case "CustomerAccount":
					{
						//var objectToDelete = new CustomerAccount {ID = deleteData.ObjectID};
						//CustomerAccountTable.Attach(objectToDelete);
						var objectToDelete = CustomerAccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CustomerAccountTable.Remove(objectToDelete);
						break;
					}
					case "GroupSubscriptionPlanCollection":
					{
						//var objectToDelete = new GroupSubscriptionPlanCollection {ID = deleteData.ObjectID};
						//GroupSubscriptionPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = GroupSubscriptionPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSubscriptionPlanCollectionTable.Remove(objectToDelete);
						break;
					}
					case "CustomerAccountCollection":
					{
						//var objectToDelete = new CustomerAccountCollection {ID = deleteData.ObjectID};
						//CustomerAccountCollectionTable.Attach(objectToDelete);
						var objectToDelete = CustomerAccountCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CustomerAccountCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



			public DbSet<GroupSubscriptionPlan> GroupSubscriptionPlanTable { get; set; }
			public DbSet<SubscriptionPlanStatus> SubscriptionPlanStatusTable { get; set; }
			public DbSet<SubscriptionPlanStatusSubscriptionPlan> SubscriptionPlanStatusSubscriptionPlanTable { get; set; }
			public DbSet<CustomerAccount> CustomerAccountTable { get; set; }
			public DbSet<CustomerAccountActivePlans> CustomerAccountActivePlansTable { get; set; }
			public DbSet<GroupSubscriptionPlanCollection> GroupSubscriptionPlanCollectionTable { get; set; }
			public DbSet<CustomerAccountCollection> CustomerAccountCollectionTable { get; set; }
        }

    //[Table(Name = "GroupSubscriptionPlan")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GroupSubscriptionPlan: {ID}")]
	public class GroupSubscriptionPlan : ITheBallDataContextStorable
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


		public GroupSubscriptionPlan() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSubscriptionPlan](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PlanName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[GroupIDs] TEXT NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        //[Column(Name = "GroupIDs")] 
        //[ScaffoldColumn(true)]
		public string GroupIDsData { get; set; }

        private bool _IsGroupIDsRetrieved = false;
        private bool _IsGroupIDsChanged = false;
        private ObservableCollection<string> _GroupIDs = null;
        public ObservableCollection<string> GroupIDs
        {
            get
            {
                if (!_IsGroupIDsRetrieved)
                {
                    if (GroupIDsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(GroupIDsData);
                        _GroupIDs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _GroupIDs = new ObservableCollection<string>();
						GroupIDsData = Guid.NewGuid().ToString();
						_IsGroupIDsChanged = true;
                    }
                    _IsGroupIDsRetrieved = true;
                    _GroupIDs.CollectionChanged += (sender, args) =>
						{
							GroupIDsData = Guid.NewGuid().ToString();
							_IsGroupIDsChanged = true;
						};
                }
                return _GroupIDs;
            }
            set 
			{ 
				_GroupIDs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsGroupIDsRetrieved = true;
                GroupIDsData = Guid.NewGuid().ToString();
                _IsGroupIDsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PlanName == null)
				PlanName = string.Empty;
			if(Description == null)
				Description = string.Empty;
            if (_IsGroupIDsChanged || isInitialInsert)
            {
                var dataToStore = GroupIDs.ToArray();
                GroupIDsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    //[Table(Name = "SubscriptionPlanStatus")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionPlanStatus: {ID}")]
	public class SubscriptionPlanStatus : ITheBallDataContextStorable
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


		public SubscriptionPlanStatus() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionPlanStatus](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ValidUntil] TEXT NOT NULL
)";
        }

		//private obsoleted<SubscriptionPlanStatusSubscriptionPlan> _SubscriptionPlan = new obsoleted<SubscriptionPlanStatusSubscriptionPlan>();
        //[Association(ThisKey = "ID", OtherKey = "SubscriptionPlanStatusID", Storage="_SubscriptionPlan")]
        public SubscriptionPlanStatusSubscriptionPlan SubscriptionPlan 
		{ 
			get;
			set;
			//get { return _SubscriptionPlan.Entity; }
			//set { _SubscriptionPlan.Entity = value; }
		}


		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime ValidUntil { get; set; }
		// private DateTime _unmodified_ValidUntil;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    //[Table(Name = "CustomerAccount")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("CustomerAccount: {ID}")]
	public class CustomerAccount : ITheBallDataContextStorable
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


		public CustomerAccount() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CustomerAccount](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[StripeID] TEXT NOT NULL, 
[IsTestAccount] INTEGER NOT NULL, 
[EmailAddress] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string StripeID { get; set; }
		// private string _unmodified_StripeID;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool IsTestAccount { get; set; }
		// private bool _unmodified_IsTestAccount;

		//[Column]
        //[ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
		//private obsoleted<CustomerAccountActivePlans> _ActivePlans = new obsoleted<CustomerAccountActivePlans>();
        //[Association(ThisKey = "ID", OtherKey = "CustomerAccountID", Storage="_ActivePlans")]
        public List<CustomerAccountActivePlans> ActivePlans { 
			get; 
			set;
			//get { return _ActivePlans; }
			//set { _ActivePlans.Assign(value); }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(StripeID == null)
				StripeID = string.Empty;
			if(EmailAddress == null)
				EmailAddress = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    //[Table(Name = "GroupSubscriptionPlanCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GroupSubscriptionPlanCollection: {ID}")]
	public class GroupSubscriptionPlanCollection : ITheBallDataContextStorable
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


		public GroupSubscriptionPlanCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSubscriptionPlanCollection](
[ID] TEXT NOT NULL, 
[CollectionItemID] TEXT NOT NULL, 
[ETag] TEXT NOT NULL,
	PRIMARY KEY (ID) )";
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		}
		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string CollectionItemID { get; set; }
	}
    //[Table(Name = "CustomerAccountCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("CustomerAccountCollection: {ID}")]
	public class CustomerAccountCollection : ITheBallDataContextStorable
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


		public CustomerAccountCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CustomerAccountCollection](
[ID] TEXT NOT NULL, 
[CollectionItemID] TEXT NOT NULL, 
[ETag] TEXT NOT NULL,
	PRIMARY KEY (ID) )";
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		}
		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string CollectionItemID { get; set; }
	}
    //[Table(Name = "SubscriptionPlanStatusSubscriptionPlan")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionPlanStatusSubscriptionPlan: {SubscriptionPlanStatusID} - {GroupSubscriptionPlanID}")]
	public class SubscriptionPlanStatusSubscriptionPlan // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionPlanStatusSubscriptionPlan](
[SubscriptionPlanStatusID] TEXT NOT NULL, 
[GroupSubscriptionPlanID] TEXT NOT NULL,
PRIMARY KEY (SubscriptionPlanStatusID, GroupSubscriptionPlanID)
)";
        }


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionPlanStatusID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupSubscriptionPlanID { get; set; }


        //private EntityRef<SubscriptionPlanStatus> _SubscriptionPlanStatus = new EntityRef<SubscriptionPlanStatus>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionPlanStatusID", OtherKey = "ID", 
		//	Storage = "_SubscriptionPlanStatus", IsUnique = true)]
        public SubscriptionPlanStatus SubscriptionPlanStatus { get; set; }

        //private EntityRef<GroupSubscriptionPlan> _GroupSubscriptionPlan = new EntityRef<GroupSubscriptionPlan>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupSubscriptionPlanID", OtherKey = "ID", 
		//	Storage = "_GroupSubscriptionPlan")]
		public GroupSubscriptionPlan GroupSubscriptionPlan { get; set; }

    }

    //[Table(Name = "CustomerAccountActivePlans")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("CustomerAccountActivePlans: {CustomerAccountID} - {SubscriptionPlanStatusID}")]
	public class CustomerAccountActivePlans // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CustomerAccountActivePlans](
[CustomerAccountID] TEXT NOT NULL, 
[SubscriptionPlanStatusID] TEXT NOT NULL,
PRIMARY KEY (CustomerAccountID, SubscriptionPlanStatusID)
)";
        }


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string CustomerAccountID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionPlanStatusID { get; set; }


        //private EntityRef<CustomerAccount> _CustomerAccount = new EntityRef<CustomerAccount>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "CustomerAccountID", OtherKey = "ID", 
		//	Storage = "_CustomerAccount", IsUnique = false)]
        public CustomerAccount CustomerAccount { get; set; }

        //private EntityRef<SubscriptionPlanStatus> _SubscriptionPlanStatus = new EntityRef<SubscriptionPlanStatus>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionPlanStatusID", OtherKey = "ID", 
		//	Storage = "_SubscriptionPlanStatus")]
		public SubscriptionPlanStatus SubscriptionPlanStatus { get; set; }

    }

 } 
