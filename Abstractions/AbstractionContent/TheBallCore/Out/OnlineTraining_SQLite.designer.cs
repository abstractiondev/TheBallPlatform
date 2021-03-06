 


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
using System.ComponentModel.DataAnnotations.Schema;
using Key=System.ComponentModel.DataAnnotations.KeyAttribute;
//using ScaffoldColumn=System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute;
//using Editable=System.ComponentModel.DataAnnotations.EditableAttribute;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SQLite.ProBroz.OnlineTraining { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DbContext, IStorageSyncableDataContext
		{
		    protected override void OnModelCreating(ModelBuilder modelBuilder)
		    {
				Member.EntityConfig(modelBuilder);
				MemberSubscriptions.EntityConfig(modelBuilder);
				MembershipPlan.EntityConfig(modelBuilder);
				MembershipPlanPaymentOptions.EntityConfig(modelBuilder);
				MembershipPlanGym.EntityConfig(modelBuilder);
				PaymentOption.EntityConfig(modelBuilder);
				Subscription.EntityConfig(modelBuilder);
				SubscriptionPlan.EntityConfig(modelBuilder);
				SubscriptionPaymentOption.EntityConfig(modelBuilder);
				TenantGym.EntityConfig(modelBuilder);
				MemberCollection.EntityConfig(modelBuilder);
				MembershipPlanCollection.EntityConfig(modelBuilder);
				PaymentOptionCollection.EntityConfig(modelBuilder);
				SubscriptionCollection.EntityConfig(modelBuilder);
				TenantGymCollection.EntityConfig(modelBuilder);

		    }

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
				tableCreationCommands.Add(Member.GetCreateTableSQL());
				tableCreationCommands.Add(MemberSubscriptions.GetCreateTableSQL());
				tableCreationCommands.Add(MembershipPlan.GetCreateTableSQL());
				tableCreationCommands.Add(MembershipPlanPaymentOptions.GetCreateTableSQL());
				tableCreationCommands.Add(MembershipPlanGym.GetCreateTableSQL());
				tableCreationCommands.Add(PaymentOption.GetCreateTableSQL());
				tableCreationCommands.Add(Subscription.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionPlan.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionPaymentOption.GetCreateTableSQL());
				tableCreationCommands.Add(TenantGym.GetCreateTableSQL());
				tableCreationCommands.Add(MemberCollection.GetCreateTableSQL());
				tableCreationCommands.Add(MembershipPlanCollection.GetCreateTableSQL());
				tableCreationCommands.Add(PaymentOptionCollection.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TenantGymCollection.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "Member":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.Member.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MemberTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
		            existingObject.MiddleName = serializedObject.MiddleName;
		            existingObject.BirthDay = serializedObject.BirthDay;
		            existingObject.Email = serializedObject.Email;
		            existingObject.PhoneNumber = serializedObject.PhoneNumber;
		            existingObject.Address = serializedObject.Address;
		            existingObject.Address2 = serializedObject.Address2;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.PostOffice = serializedObject.PostOffice;
		            existingObject.Country = serializedObject.Country;
		            existingObject.FederationLicense = serializedObject.FederationLicense;
		            existingObject.PhotoPermission = serializedObject.PhotoPermission;
		            existingObject.VideoPermission = serializedObject.VideoPermission;
                    if (serializedObject.Subscriptions != null)
                    {
						existingObject.Subscriptions.Clear();
                        serializedObject.Subscriptions.ForEach(
                            item =>
                            {
                                var relationObject = new MemberSubscriptions
                                {
                                    MemberID = existingObject.ID,
                                    SubscriptionID = item
                                };
                                MemberSubscriptionsTable.Add(relationObject);
                                existingObject.Subscriptions.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "MembershipPlan":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.MembershipPlan.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MembershipPlanTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PlanName = serializedObject.PlanName;
		            existingObject.Description = serializedObject.Description;
                    if (serializedObject.PaymentOptions != null)
                    {
						existingObject.PaymentOptions.Clear();
                        serializedObject.PaymentOptions.ForEach(
                            item =>
                            {
                                var relationObject = new MembershipPlanPaymentOptions
                                {
                                    MembershipPlanID = existingObject.ID,
                                    PaymentOptionID = item
                                };
                                MembershipPlanPaymentOptionsTable.Add(relationObject);
                                existingObject.PaymentOptions.Add(relationObject);

                            });
                    }

                    if (serializedObject.Gym != null)
                    {
                            var relationObject = new MembershipPlanGym
                            {
                                MembershipPlanID = existingObject.ID,
                                TenantGymID = serializedObject.Gym
                            };
                            MembershipPlanGymTable.Add(relationObject);
							existingObject.Gym = relationObject;
                    }

		            break;
		        } 
		        case "PaymentOption":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.PaymentOption.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PaymentOptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OptionName = serializedObject.OptionName;
		            existingObject.PeriodInMonths = serializedObject.PeriodInMonths;
		            existingObject.Price = serializedObject.Price;
		            break;
		        } 
		        case "Subscription":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.Subscription.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Plan != null)
                    {
                            var relationObject = new SubscriptionPlan
                            {
                                SubscriptionID = existingObject.ID,
                                MembershipPlanID = serializedObject.Plan
                            };
                            SubscriptionPlanTable.Add(relationObject);
							existingObject.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = existingObject.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.Add(relationObject);
							existingObject.PaymentOption = relationObject;
                    }

		            existingObject.Created = serializedObject.Created;
		            existingObject.ValidFrom = serializedObject.ValidFrom;
		            existingObject.ValidTo = serializedObject.ValidTo;
		            break;
		        } 
		        case "TenantGym":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.TenantGym.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TenantGymTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GymName = serializedObject.GymName;
		            existingObject.Email = serializedObject.Email;
		            existingObject.PhoneNumber = serializedObject.PhoneNumber;
		            existingObject.Address = serializedObject.Address;
		            existingObject.Address2 = serializedObject.Address2;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.PostOffice = serializedObject.PostOffice;
		            existingObject.Country = serializedObject.Country;
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "Member":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.Member.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MemberTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
		            existingObject.MiddleName = serializedObject.MiddleName;
		            existingObject.BirthDay = serializedObject.BirthDay;
		            existingObject.Email = serializedObject.Email;
		            existingObject.PhoneNumber = serializedObject.PhoneNumber;
		            existingObject.Address = serializedObject.Address;
		            existingObject.Address2 = serializedObject.Address2;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.PostOffice = serializedObject.PostOffice;
		            existingObject.Country = serializedObject.Country;
		            existingObject.FederationLicense = serializedObject.FederationLicense;
		            existingObject.PhotoPermission = serializedObject.PhotoPermission;
		            existingObject.VideoPermission = serializedObject.VideoPermission;
                    if (serializedObject.Subscriptions != null)
                    {
						existingObject.Subscriptions.Clear();
                        serializedObject.Subscriptions.ForEach(
                            item =>
                            {
                                var relationObject = new MemberSubscriptions
                                {
                                    MemberID = existingObject.ID,
                                    SubscriptionID = item
                                };
                                MemberSubscriptionsTable.Add(relationObject);
                                existingObject.Subscriptions.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "MembershipPlan":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.MembershipPlan.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MembershipPlanTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PlanName = serializedObject.PlanName;
		            existingObject.Description = serializedObject.Description;
                    if (serializedObject.PaymentOptions != null)
                    {
						existingObject.PaymentOptions.Clear();
                        serializedObject.PaymentOptions.ForEach(
                            item =>
                            {
                                var relationObject = new MembershipPlanPaymentOptions
                                {
                                    MembershipPlanID = existingObject.ID,
                                    PaymentOptionID = item
                                };
                                MembershipPlanPaymentOptionsTable.Add(relationObject);
                                existingObject.PaymentOptions.Add(relationObject);

                            });
                    }

                    if (serializedObject.Gym != null)
                    {
                            var relationObject = new MembershipPlanGym
                            {
                                MembershipPlanID = existingObject.ID,
                                TenantGymID = serializedObject.Gym
                            };
                            MembershipPlanGymTable.Add(relationObject);
							existingObject.Gym = relationObject;
                    }

		            break;
		        } 
		        case "PaymentOption":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.PaymentOption.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = PaymentOptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OptionName = serializedObject.OptionName;
		            existingObject.PeriodInMonths = serializedObject.PeriodInMonths;
		            existingObject.Price = serializedObject.Price;
		            break;
		        } 
		        case "Subscription":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.Subscription.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = SubscriptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Plan != null)
                    {
                            var relationObject = new SubscriptionPlan
                            {
                                SubscriptionID = existingObject.ID,
                                MembershipPlanID = serializedObject.Plan
                            };
                            SubscriptionPlanTable.Add(relationObject);
							existingObject.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = existingObject.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.Add(relationObject);
							existingObject.PaymentOption = relationObject;
                    }

		            existingObject.Created = serializedObject.Created;
		            existingObject.ValidFrom = serializedObject.ValidFrom;
		            existingObject.ValidTo = serializedObject.ValidTo;
		            break;
		        } 
		        case "TenantGym":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.ProBroz.OnlineTraining.TenantGym.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TenantGymTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GymName = serializedObject.GymName;
		            existingObject.Email = serializedObject.Email;
		            existingObject.PhoneNumber = serializedObject.PhoneNumber;
		            existingObject.Address = serializedObject.Address;
		            existingObject.Address2 = serializedObject.Address2;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.PostOffice = serializedObject.PostOffice;
		            existingObject.Country = serializedObject.Country;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "Member":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.Member.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Member {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
		            objectToAdd.MiddleName = serializedObject.MiddleName;
		            objectToAdd.BirthDay = serializedObject.BirthDay;
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.PhoneNumber = serializedObject.PhoneNumber;
		            objectToAdd.Address = serializedObject.Address;
		            objectToAdd.Address2 = serializedObject.Address2;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.PostOffice = serializedObject.PostOffice;
		            objectToAdd.Country = serializedObject.Country;
		            objectToAdd.FederationLicense = serializedObject.FederationLicense;
		            objectToAdd.PhotoPermission = serializedObject.PhotoPermission;
		            objectToAdd.VideoPermission = serializedObject.VideoPermission;
                    if (serializedObject.Subscriptions != null)
                    {
                        serializedObject.Subscriptions.ForEach(
                            item =>
                            {
                                var relationObject = new MemberSubscriptions
                                {
                                    MemberID = objectToAdd.ID,
                                    SubscriptionID = item
                                };
                                MemberSubscriptionsTable.Add(relationObject);
                                objectToAdd.Subscriptions.Add(relationObject);

                            });
                    }

					MemberTable.Add(objectToAdd);
                    break;
                }
                case "MembershipPlan":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.MembershipPlan.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MembershipPlan {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PlanName = serializedObject.PlanName;
		            objectToAdd.Description = serializedObject.Description;
                    if (serializedObject.PaymentOptions != null)
                    {
                        serializedObject.PaymentOptions.ForEach(
                            item =>
                            {
                                var relationObject = new MembershipPlanPaymentOptions
                                {
                                    MembershipPlanID = objectToAdd.ID,
                                    PaymentOptionID = item
                                };
                                MembershipPlanPaymentOptionsTable.Add(relationObject);
                                objectToAdd.PaymentOptions.Add(relationObject);

                            });
                    }

                    if (serializedObject.Gym != null)
                    {
                            var relationObject = new MembershipPlanGym
                            {
                                MembershipPlanID = objectToAdd.ID,
                                TenantGymID = serializedObject.Gym
                            };
                            MembershipPlanGymTable.Add(relationObject);
                            objectToAdd.Gym = relationObject;
                    }

					MembershipPlanTable.Add(objectToAdd);
                    break;
                }
                case "PaymentOption":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.PaymentOption.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PaymentOption {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OptionName = serializedObject.OptionName;
		            objectToAdd.PeriodInMonths = serializedObject.PeriodInMonths;
		            objectToAdd.Price = serializedObject.Price;
					PaymentOptionTable.Add(objectToAdd);
                    break;
                }
                case "Subscription":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.Subscription.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Subscription {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Plan != null)
                    {
                            var relationObject = new SubscriptionPlan
                            {
                                SubscriptionID = objectToAdd.ID,
                                MembershipPlanID = serializedObject.Plan
                            };
                            SubscriptionPlanTable.Add(relationObject);
                            objectToAdd.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = objectToAdd.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.Add(relationObject);
                            objectToAdd.PaymentOption = relationObject;
                    }

		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.ValidFrom = serializedObject.ValidFrom;
		            objectToAdd.ValidTo = serializedObject.ValidTo;
					SubscriptionTable.Add(objectToAdd);
                    break;
                }
                case "TenantGym":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.TenantGym.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TenantGym {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GymName = serializedObject.GymName;
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.PhoneNumber = serializedObject.PhoneNumber;
		            objectToAdd.Address = serializedObject.Address;
		            objectToAdd.Address2 = serializedObject.Address2;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.PostOffice = serializedObject.PostOffice;
		            objectToAdd.Country = serializedObject.Country;
					TenantGymTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "Member":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.Member.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Member {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
		            objectToAdd.MiddleName = serializedObject.MiddleName;
		            objectToAdd.BirthDay = serializedObject.BirthDay;
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.PhoneNumber = serializedObject.PhoneNumber;
		            objectToAdd.Address = serializedObject.Address;
		            objectToAdd.Address2 = serializedObject.Address2;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.PostOffice = serializedObject.PostOffice;
		            objectToAdd.Country = serializedObject.Country;
		            objectToAdd.FederationLicense = serializedObject.FederationLicense;
		            objectToAdd.PhotoPermission = serializedObject.PhotoPermission;
		            objectToAdd.VideoPermission = serializedObject.VideoPermission;
                    if (serializedObject.Subscriptions != null)
                    {
                        serializedObject.Subscriptions.ForEach(
                            item =>
                            {
                                var relationObject = new MemberSubscriptions
                                {
                                    MemberID = objectToAdd.ID,
                                    SubscriptionID = item
                                };
                                MemberSubscriptionsTable.Add(relationObject);
                                objectToAdd.Subscriptions.Add(relationObject);

                            });
                    }

					MemberTable.Add(objectToAdd);
                    break;
                }
                case "MembershipPlan":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.MembershipPlan.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new MembershipPlan {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PlanName = serializedObject.PlanName;
		            objectToAdd.Description = serializedObject.Description;
                    if (serializedObject.PaymentOptions != null)
                    {
                        serializedObject.PaymentOptions.ForEach(
                            item =>
                            {
                                var relationObject = new MembershipPlanPaymentOptions
                                {
                                    MembershipPlanID = objectToAdd.ID,
                                    PaymentOptionID = item
                                };
                                MembershipPlanPaymentOptionsTable.Add(relationObject);
                                objectToAdd.PaymentOptions.Add(relationObject);

                            });
                    }

                    if (serializedObject.Gym != null)
                    {
                            var relationObject = new MembershipPlanGym
                            {
                                MembershipPlanID = objectToAdd.ID,
                                TenantGymID = serializedObject.Gym
                            };
                            MembershipPlanGymTable.Add(relationObject);
                            objectToAdd.Gym = relationObject;
                    }

					MembershipPlanTable.Add(objectToAdd);
                    break;
                }
                case "PaymentOption":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.PaymentOption.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new PaymentOption {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OptionName = serializedObject.OptionName;
		            objectToAdd.PeriodInMonths = serializedObject.PeriodInMonths;
		            objectToAdd.Price = serializedObject.Price;
					PaymentOptionTable.Add(objectToAdd);
                    break;
                }
                case "Subscription":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.Subscription.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Subscription {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Plan != null)
                    {
                            var relationObject = new SubscriptionPlan
                            {
                                SubscriptionID = objectToAdd.ID,
                                MembershipPlanID = serializedObject.Plan
                            };
                            SubscriptionPlanTable.Add(relationObject);
                            objectToAdd.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = objectToAdd.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.Add(relationObject);
                            objectToAdd.PaymentOption = relationObject;
                    }

		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.ValidFrom = serializedObject.ValidFrom;
		            objectToAdd.ValidTo = serializedObject.ValidTo;
					SubscriptionTable.Add(objectToAdd);
                    break;
                }
                case "TenantGym":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.ProBroz.OnlineTraining.TenantGym.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TenantGym {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GymName = serializedObject.GymName;
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.PhoneNumber = serializedObject.PhoneNumber;
		            objectToAdd.Address = serializedObject.Address;
		            objectToAdd.Address2 = serializedObject.Address2;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.PostOffice = serializedObject.PostOffice;
		            objectToAdd.Country = serializedObject.Country;
					TenantGymTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Member":
					{
						//var objectToDelete = new Member {ID = deleteData.ObjectID};
						//MemberTable.Attach(objectToDelete);
						var objectToDelete = MemberTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberTable.Remove(objectToDelete);
						break;
					}
					case "MembershipPlan":
					{
						//var objectToDelete = new MembershipPlan {ID = deleteData.ObjectID};
						//MembershipPlanTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanTable.Remove(objectToDelete);
						break;
					}
					case "PaymentOption":
					{
						//var objectToDelete = new PaymentOption {ID = deleteData.ObjectID};
						//PaymentOptionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionTable.Remove(objectToDelete);
						break;
					}
					case "Subscription":
					{
						//var objectToDelete = new Subscription {ID = deleteData.ObjectID};
						//SubscriptionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionTable.Remove(objectToDelete);
						break;
					}
					case "TenantGym":
					{
						//var objectToDelete = new TenantGym {ID = deleteData.ObjectID};
						//TenantGymTable.Attach(objectToDelete);
						var objectToDelete = TenantGymTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymTable.Remove(objectToDelete);
						break;
					}
					case "MemberCollection":
					{
						//var objectToDelete = new MemberCollection {ID = deleteData.ObjectID};
						//MemberCollectionTable.Attach(objectToDelete);
						var objectToDelete = MemberCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberCollectionTable.Remove(objectToDelete);
						break;
					}
					case "MembershipPlanCollection":
					{
						//var objectToDelete = new MembershipPlanCollection {ID = deleteData.ObjectID};
						//MembershipPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanCollectionTable.Remove(objectToDelete);
						break;
					}
					case "PaymentOptionCollection":
					{
						//var objectToDelete = new PaymentOptionCollection {ID = deleteData.ObjectID};
						//PaymentOptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "SubscriptionCollection":
					{
						//var objectToDelete = new SubscriptionCollection {ID = deleteData.ObjectID};
						//SubscriptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "TenantGymCollection":
					{
						//var objectToDelete = new TenantGymCollection {ID = deleteData.ObjectID};
						//TenantGymCollectionTable.Attach(objectToDelete);
						var objectToDelete = TenantGymCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Member":
					{
						//var objectToDelete = new Member {ID = deleteData.ObjectID};
						//MemberTable.Attach(objectToDelete);
						var objectToDelete = MemberTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberTable.Remove(objectToDelete);
						break;
					}
					case "MembershipPlan":
					{
						//var objectToDelete = new MembershipPlan {ID = deleteData.ObjectID};
						//MembershipPlanTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanTable.Remove(objectToDelete);
						break;
					}
					case "PaymentOption":
					{
						//var objectToDelete = new PaymentOption {ID = deleteData.ObjectID};
						//PaymentOptionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionTable.Remove(objectToDelete);
						break;
					}
					case "Subscription":
					{
						//var objectToDelete = new Subscription {ID = deleteData.ObjectID};
						//SubscriptionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionTable.Remove(objectToDelete);
						break;
					}
					case "TenantGym":
					{
						//var objectToDelete = new TenantGym {ID = deleteData.ObjectID};
						//TenantGymTable.Attach(objectToDelete);
						var objectToDelete = TenantGymTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymTable.Remove(objectToDelete);
						break;
					}
					case "MemberCollection":
					{
						//var objectToDelete = new MemberCollection {ID = deleteData.ObjectID};
						//MemberCollectionTable.Attach(objectToDelete);
						var objectToDelete = MemberCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberCollectionTable.Remove(objectToDelete);
						break;
					}
					case "MembershipPlanCollection":
					{
						//var objectToDelete = new MembershipPlanCollection {ID = deleteData.ObjectID};
						//MembershipPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanCollectionTable.Remove(objectToDelete);
						break;
					}
					case "PaymentOptionCollection":
					{
						//var objectToDelete = new PaymentOptionCollection {ID = deleteData.ObjectID};
						//PaymentOptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "SubscriptionCollection":
					{
						//var objectToDelete = new SubscriptionCollection {ID = deleteData.ObjectID};
						//SubscriptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "TenantGymCollection":
					{
						//var objectToDelete = new TenantGymCollection {ID = deleteData.ObjectID};
						//TenantGymCollectionTable.Attach(objectToDelete);
						var objectToDelete = TenantGymCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



			public DbSet<Member> MemberTable { get; set; }
			public DbSet<MemberSubscriptions> MemberSubscriptionsTable { get; set; }
			public DbSet<MembershipPlan> MembershipPlanTable { get; set; }
			public DbSet<MembershipPlanPaymentOptions> MembershipPlanPaymentOptionsTable { get; set; }
			public DbSet<MembershipPlanGym> MembershipPlanGymTable { get; set; }
			public DbSet<PaymentOption> PaymentOptionTable { get; set; }
			public DbSet<Subscription> SubscriptionTable { get; set; }
			public DbSet<SubscriptionPlan> SubscriptionPlanTable { get; set; }
			public DbSet<SubscriptionPaymentOption> SubscriptionPaymentOptionTable { get; set; }
			public DbSet<TenantGym> TenantGymTable { get; set; }
			public DbSet<MemberCollection> MemberCollectionTable { get; set; }
			public DbSet<MembershipPlanCollection> MembershipPlanCollectionTable { get; set; }
			public DbSet<PaymentOptionCollection> PaymentOptionCollectionTable { get; set; }
			public DbSet<SubscriptionCollection> SubscriptionCollectionTable { get; set; }
			public DbSet<TenantGymCollection> TenantGymCollectionTable { get; set; }
        }

    [Table("Member")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Member: {ID}")]
	public class Member : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public Member() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Member](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[FirstName] TEXT DEFAULT '', 
[LastName] TEXT DEFAULT '', 
[MiddleName] TEXT DEFAULT '', 
[BirthDay] TEXT DEFAULT '', 
[Email] TEXT DEFAULT '', 
[PhoneNumber] TEXT DEFAULT '', 
[Address] TEXT DEFAULT '', 
[Address2] TEXT DEFAULT '', 
[ZipCode] TEXT DEFAULT '', 
[PostOffice] TEXT DEFAULT '', 
[Country] TEXT DEFAULT '', 
[FederationLicense] TEXT DEFAULT '', 
[PhotoPermission] INTEGER NOT NULL, 
[VideoPermission] INTEGER NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string FirstName { get; set; }
		// private string _unmodified_FirstName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string LastName { get; set; }
		// private string _unmodified_LastName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string MiddleName { get; set; }
		// private string _unmodified_MiddleName;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime BirthDay { get; set; }
		// private DateTime _unmodified_BirthDay;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Email { get; set; }
		// private string _unmodified_Email;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PhoneNumber { get; set; }
		// private string _unmodified_PhoneNumber;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Address { get; set; }
		// private string _unmodified_Address;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Address2 { get; set; }
		// private string _unmodified_Address2;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PostOffice { get; set; }
		// private string _unmodified_PostOffice;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Country { get; set; }
		// private string _unmodified_Country;

		//[Column]
        //[ScaffoldColumn(true)]
		public string FederationLicense { get; set; }
		// private string _unmodified_FederationLicense;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool PhotoPermission { get; set; }
		// private bool _unmodified_PhotoPermission;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool VideoPermission { get; set; }
		// private bool _unmodified_VideoPermission;
		//private obsoleted<MemberSubscriptions> _Subscriptions = new obsoleted<MemberSubscriptions>();
        //[Association(ThisKey = "ID", OtherKey = "MemberID", Storage="_Subscriptions")]
        public List<MemberSubscriptions> Subscriptions { 
			get; 
			set;
			//get { return _Subscriptions; }
			//set { _Subscriptions.Assign(value); }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(FirstName == null)
				FirstName = string.Empty;
			if(LastName == null)
				LastName = string.Empty;
			if(MiddleName == null)
				MiddleName = string.Empty;
			if(Email == null)
				Email = string.Empty;
			if(PhoneNumber == null)
				PhoneNumber = string.Empty;
			if(Address == null)
				Address = string.Empty;
			if(Address2 == null)
				Address2 = string.Empty;
			if(ZipCode == null)
				ZipCode = string.Empty;
			if(PostOffice == null)
				PostOffice = string.Empty;
			if(Country == null)
				Country = string.Empty;
			if(FederationLicense == null)
				FederationLicense = string.Empty;
		}
	}
    [Table("MembershipPlan")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlan: {ID}")]
	public class MembershipPlan : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public MembershipPlan() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MembershipPlan](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PlanName] TEXT DEFAULT '', 
[Description] TEXT DEFAULT ''
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
		//private obsoleted<MembershipPlanPaymentOptions> _PaymentOptions = new obsoleted<MembershipPlanPaymentOptions>();
        //[Association(ThisKey = "ID", OtherKey = "MembershipPlanID", Storage="_PaymentOptions")]
        public List<MembershipPlanPaymentOptions> PaymentOptions { 
			get; 
			set;
			//get { return _PaymentOptions; }
			//set { _PaymentOptions.Assign(value); }
		}

		//private obsoleted<MembershipPlanGym> _Gym = new obsoleted<MembershipPlanGym>();
        //[Association(ThisKey = "ID", OtherKey = "MembershipPlanID", Storage="_Gym")]
        public MembershipPlanGym Gym 
		{ 
			get;
			set;
			//get { return _Gym.Entity; }
			//set { _Gym.Entity = value; }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PlanName == null)
				PlanName = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table("PaymentOption")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("PaymentOption: {ID}")]
	public class PaymentOption : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public PaymentOption() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PaymentOption](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OptionName] TEXT DEFAULT '', 
[PeriodInMonths] INTEGER NOT NULL, 
[Price] REAL NOT NULL
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string OptionName { get; set; }
		// private string _unmodified_OptionName;

		//[Column]
        //[ScaffoldColumn(true)]
		public long PeriodInMonths { get; set; }
		// private long _unmodified_PeriodInMonths;

		//[Column]
        //[ScaffoldColumn(true)]
		public double Price { get; set; }
		// private double _unmodified_Price;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OptionName == null)
				OptionName = string.Empty;
		}
	}
    [Table("Subscription")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Subscription: {ID}")]
	public class Subscription : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public Subscription() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Subscription](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Created] TEXT DEFAULT '', 
[ValidFrom] TEXT DEFAULT '', 
[ValidTo] TEXT DEFAULT ''
)";
        }

		//private obsoleted<SubscriptionPlan> _Plan = new obsoleted<SubscriptionPlan>();
        //[Association(ThisKey = "ID", OtherKey = "SubscriptionID", Storage="_Plan")]
        public SubscriptionPlan Plan 
		{ 
			get;
			set;
			//get { return _Plan.Entity; }
			//set { _Plan.Entity = value; }
		}

		//private obsoleted<SubscriptionPaymentOption> _PaymentOption = new obsoleted<SubscriptionPaymentOption>();
        //[Association(ThisKey = "ID", OtherKey = "SubscriptionID", Storage="_PaymentOption")]
        public SubscriptionPaymentOption PaymentOption 
		{ 
			get;
			set;
			//get { return _PaymentOption.Entity; }
			//set { _PaymentOption.Entity = value; }
		}


		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime Created { get; set; }
		// private DateTime _unmodified_Created;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime ValidFrom { get; set; }
		// private DateTime _unmodified_ValidFrom;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime ValidTo { get; set; }
		// private DateTime _unmodified_ValidTo;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table("TenantGym")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("TenantGym: {ID}")]
	public class TenantGym : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public TenantGym() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TenantGym](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GymName] TEXT DEFAULT '', 
[Email] TEXT DEFAULT '', 
[PhoneNumber] TEXT DEFAULT '', 
[Address] TEXT DEFAULT '', 
[Address2] TEXT DEFAULT '', 
[ZipCode] TEXT DEFAULT '', 
[PostOffice] TEXT DEFAULT '', 
[Country] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string GymName { get; set; }
		// private string _unmodified_GymName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Email { get; set; }
		// private string _unmodified_Email;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PhoneNumber { get; set; }
		// private string _unmodified_PhoneNumber;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Address { get; set; }
		// private string _unmodified_Address;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Address2 { get; set; }
		// private string _unmodified_Address2;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PostOffice { get; set; }
		// private string _unmodified_PostOffice;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Country { get; set; }
		// private string _unmodified_Country;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GymName == null)
				GymName = string.Empty;
			if(Email == null)
				Email = string.Empty;
			if(PhoneNumber == null)
				PhoneNumber = string.Empty;
			if(Address == null)
				Address = string.Empty;
			if(Address2 == null)
				Address2 = string.Empty;
			if(ZipCode == null)
				ZipCode = string.Empty;
			if(PostOffice == null)
				PostOffice = string.Empty;
			if(Country == null)
				Country = string.Empty;
		}
	}
    [Table("MemberCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MemberCollection: {ID}")]
	public class MemberCollection : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public MemberCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MemberCollection](
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
    [Table("MembershipPlanCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlanCollection: {ID}")]
	public class MembershipPlanCollection : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public MembershipPlanCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MembershipPlanCollection](
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
    [Table("PaymentOptionCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("PaymentOptionCollection: {ID}")]
	public class PaymentOptionCollection : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public PaymentOptionCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PaymentOptionCollection](
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
    [Table("SubscriptionCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionCollection: {ID}")]
	public class SubscriptionCollection : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public SubscriptionCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionCollection](
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
    [Table("TenantGymCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("TenantGymCollection: {ID}")]
	public class TenantGymCollection : ITheBallDataContextStorable
	{
		public static void EntityConfig(ModelBuilder modelBuilder) {
		}

		//[Column(IsPrimaryKey = true)]
        //[ScaffoldColumn(true)]
		[Key]
        //[Editable(false)]
		public string ID { get; set; }

		//[Column]
        //[ScaffoldColumn(true)]
        //[Editable(false)]
		public string ETag { get; set; }


		public TenantGymCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TenantGymCollection](
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
    [Table("MemberSubscriptions")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MemberSubscriptions: {MemberID} - {SubscriptionID}")]
	public class MemberSubscriptions // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MemberSubscriptions](
[MemberID] TEXT NOT NULL, 
[SubscriptionID] TEXT NOT NULL,
PRIMARY KEY (MemberID, SubscriptionID)
)";
        }

		public static void EntityConfig(ModelBuilder modelBuilder) {
		    modelBuilder.Entity<MemberSubscriptions>()
		        .HasKey(c => new { c.MemberID, c.SubscriptionID});
			
		}


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MemberID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }


        //private EntityRef<Member> _Member = new EntityRef<Member>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MemberID", OtherKey = "ID", 
		//	Storage = "_Member", IsUnique = false)]
        public Member Member { get; set; }

        //private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
		//	Storage = "_Subscription")]
		public Subscription Subscription { get; set; }

    }

    [Table("MembershipPlanPaymentOptions")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlanPaymentOptions: {MembershipPlanID} - {PaymentOptionID}")]
	public class MembershipPlanPaymentOptions // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MembershipPlanPaymentOptions](
[MembershipPlanID] TEXT NOT NULL, 
[PaymentOptionID] TEXT NOT NULL,
PRIMARY KEY (MembershipPlanID, PaymentOptionID)
)";
        }

		public static void EntityConfig(ModelBuilder modelBuilder) {
		    modelBuilder.Entity<MembershipPlanPaymentOptions>()
		        .HasKey(c => new { c.MembershipPlanID, c.PaymentOptionID});
			
		}


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string PaymentOptionID { get; set; }


        //private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
		//	Storage = "_MembershipPlan", IsUnique = false)]
        public MembershipPlan MembershipPlan { get; set; }

        //private EntityRef<PaymentOption> _PaymentOption = new EntityRef<PaymentOption>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "PaymentOptionID", OtherKey = "ID", 
		//	Storage = "_PaymentOption")]
		public PaymentOption PaymentOption { get; set; }

    }

    [Table("MembershipPlanGym")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlanGym: {MembershipPlanID} - {TenantGymID}")]
	public class MembershipPlanGym // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MembershipPlanGym](
[MembershipPlanID] TEXT NOT NULL, 
[TenantGymID] TEXT NOT NULL,
PRIMARY KEY (MembershipPlanID, TenantGymID)
)";
        }

		public static void EntityConfig(ModelBuilder modelBuilder) {
		    modelBuilder.Entity<MembershipPlanGym>()
		        .HasKey(c => new { c.MembershipPlanID, c.TenantGymID});
			
		}


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string TenantGymID { get; set; }


        //private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
		//	Storage = "_MembershipPlan", IsUnique = true)]
        public MembershipPlan MembershipPlan { get; set; }

        //private EntityRef<TenantGym> _TenantGym = new EntityRef<TenantGym>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "TenantGymID", OtherKey = "ID", 
		//	Storage = "_TenantGym")]
		public TenantGym TenantGym { get; set; }

    }

    [Table("SubscriptionPlan")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionPlan: {SubscriptionID} - {MembershipPlanID}")]
	public class SubscriptionPlan // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionPlan](
[SubscriptionID] TEXT NOT NULL, 
[MembershipPlanID] TEXT NOT NULL,
PRIMARY KEY (SubscriptionID, MembershipPlanID)
)";
        }

		public static void EntityConfig(ModelBuilder modelBuilder) {
		    modelBuilder.Entity<SubscriptionPlan>()
		        .HasKey(c => new { c.SubscriptionID, c.MembershipPlanID});
			
		}


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }


        //private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
		//	Storage = "_Subscription", IsUnique = true)]
        public Subscription Subscription { get; set; }

        //private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
		//	Storage = "_MembershipPlan")]
		public MembershipPlan MembershipPlan { get; set; }

    }

    [Table("SubscriptionPaymentOption")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionPaymentOption: {SubscriptionID} - {PaymentOptionID}")]
	public class SubscriptionPaymentOption // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionPaymentOption](
[SubscriptionID] TEXT NOT NULL, 
[PaymentOptionID] TEXT NOT NULL,
PRIMARY KEY (SubscriptionID, PaymentOptionID)
)";
        }

		public static void EntityConfig(ModelBuilder modelBuilder) {
		    modelBuilder.Entity<SubscriptionPaymentOption>()
		        .HasKey(c => new { c.SubscriptionID, c.PaymentOptionID});
			
		}


        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }
        //[Column(IsPrimaryKey = true, CanBeNull = false)]
        public string PaymentOptionID { get; set; }


        //private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
		//	Storage = "_Subscription", IsUnique = true)]
        public Subscription Subscription { get; set; }

        //private EntityRef<PaymentOption> _PaymentOption = new EntityRef<PaymentOption>();
        //[Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "PaymentOptionID", OtherKey = "ID", 
		//	Storage = "_PaymentOption")]
		public PaymentOption PaymentOption { get; set; }

    }

 } 
