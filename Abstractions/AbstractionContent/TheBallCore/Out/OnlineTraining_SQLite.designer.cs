 


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
using System.Diagnostics;
using System.Threading.Tasks;


namespace SQLite.ProBroz.OnlineTraining { 
		
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
		        using (var transaction = connection.BeginTransaction())
		        {
                    dataContext.CreateDomainDatabaseTablesIfNotExists();
                    transaction.Commit();
		        }
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

		    public async Task SubmitChangesAsync()
		    {
		        await Task.Run(() => SubmitChanges());
		    }

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
                                MemberSubscriptionsTable.InsertOnSubmit(relationObject);
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
                                MembershipPlanPaymentOptionsTable.InsertOnSubmit(relationObject);
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
                            MembershipPlanGymTable.InsertOnSubmit(relationObject);
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
		            existingObject.PeriodInDays = serializedObject.PeriodInDays;
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
                            SubscriptionPlanTable.InsertOnSubmit(relationObject);
							existingObject.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = existingObject.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.InsertOnSubmit(relationObject);
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
                                MemberSubscriptionsTable.InsertOnSubmit(relationObject);
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
                                MembershipPlanPaymentOptionsTable.InsertOnSubmit(relationObject);
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
                            MembershipPlanGymTable.InsertOnSubmit(relationObject);
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
		            existingObject.PeriodInDays = serializedObject.PeriodInDays;
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
                            SubscriptionPlanTable.InsertOnSubmit(relationObject);
							existingObject.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = existingObject.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.InsertOnSubmit(relationObject);
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
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

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
                                MemberSubscriptionsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Subscriptions.Add(relationObject);

                            });
                    }

					MemberTable.InsertOnSubmit(objectToAdd);
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
                                MembershipPlanPaymentOptionsTable.InsertOnSubmit(relationObject);
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
                            MembershipPlanGymTable.InsertOnSubmit(relationObject);
                            objectToAdd.Gym = relationObject;
                    }

					MembershipPlanTable.InsertOnSubmit(objectToAdd);
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
		            objectToAdd.PeriodInDays = serializedObject.PeriodInDays;
		            objectToAdd.Price = serializedObject.Price;
					PaymentOptionTable.InsertOnSubmit(objectToAdd);
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
                            SubscriptionPlanTable.InsertOnSubmit(relationObject);
                            objectToAdd.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = objectToAdd.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.InsertOnSubmit(relationObject);
                            objectToAdd.PaymentOption = relationObject;
                    }

		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.ValidFrom = serializedObject.ValidFrom;
		            objectToAdd.ValidTo = serializedObject.ValidTo;
					SubscriptionTable.InsertOnSubmit(objectToAdd);
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
					TenantGymTable.InsertOnSubmit(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

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
                                MemberSubscriptionsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Subscriptions.Add(relationObject);

                            });
                    }

					MemberTable.InsertOnSubmit(objectToAdd);
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
                                MembershipPlanPaymentOptionsTable.InsertOnSubmit(relationObject);
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
                            MembershipPlanGymTable.InsertOnSubmit(relationObject);
                            objectToAdd.Gym = relationObject;
                    }

					MembershipPlanTable.InsertOnSubmit(objectToAdd);
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
		            objectToAdd.PeriodInDays = serializedObject.PeriodInDays;
		            objectToAdd.Price = serializedObject.Price;
					PaymentOptionTable.InsertOnSubmit(objectToAdd);
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
                            SubscriptionPlanTable.InsertOnSubmit(relationObject);
                            objectToAdd.Plan = relationObject;
                    }

                    if (serializedObject.PaymentOption != null)
                    {
                            var relationObject = new SubscriptionPaymentOption
                            {
                                SubscriptionID = objectToAdd.ID,
                                PaymentOptionID = serializedObject.PaymentOption
                            };
                            SubscriptionPaymentOptionTable.InsertOnSubmit(relationObject);
                            objectToAdd.PaymentOption = relationObject;
                    }

		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.ValidFrom = serializedObject.ValidFrom;
		            objectToAdd.ValidTo = serializedObject.ValidTo;
					SubscriptionTable.InsertOnSubmit(objectToAdd);
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
					TenantGymTable.InsertOnSubmit(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Member":
					{
						//var objectToDelete = new Member {ID = deleteData.ObjectID};
						//MemberTable.Attach(objectToDelete);
						var objectToDelete = MemberTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MembershipPlan":
					{
						//var objectToDelete = new MembershipPlan {ID = deleteData.ObjectID};
						//MembershipPlanTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PaymentOption":
					{
						//var objectToDelete = new PaymentOption {ID = deleteData.ObjectID};
						//PaymentOptionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Subscription":
					{
						//var objectToDelete = new Subscription {ID = deleteData.ObjectID};
						//SubscriptionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TenantGym":
					{
						//var objectToDelete = new TenantGym {ID = deleteData.ObjectID};
						//TenantGymTable.Attach(objectToDelete);
						var objectToDelete = TenantGymTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MemberCollection":
					{
						//var objectToDelete = new MemberCollection {ID = deleteData.ObjectID};
						//MemberCollectionTable.Attach(objectToDelete);
						var objectToDelete = MemberCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MembershipPlanCollection":
					{
						//var objectToDelete = new MembershipPlanCollection {ID = deleteData.ObjectID};
						//MembershipPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PaymentOptionCollection":
					{
						//var objectToDelete = new PaymentOptionCollection {ID = deleteData.ObjectID};
						//PaymentOptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SubscriptionCollection":
					{
						//var objectToDelete = new SubscriptionCollection {ID = deleteData.ObjectID};
						//SubscriptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TenantGymCollection":
					{
						//var objectToDelete = new TenantGymCollection {ID = deleteData.ObjectID};
						//TenantGymCollectionTable.Attach(objectToDelete);
						var objectToDelete = TenantGymCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "ProBroz.OnlineTraining")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Member":
					{
						//var objectToDelete = new Member {ID = deleteData.ObjectID};
						//MemberTable.Attach(objectToDelete);
						var objectToDelete = MemberTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MembershipPlan":
					{
						//var objectToDelete = new MembershipPlan {ID = deleteData.ObjectID};
						//MembershipPlanTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PaymentOption":
					{
						//var objectToDelete = new PaymentOption {ID = deleteData.ObjectID};
						//PaymentOptionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Subscription":
					{
						//var objectToDelete = new Subscription {ID = deleteData.ObjectID};
						//SubscriptionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TenantGym":
					{
						//var objectToDelete = new TenantGym {ID = deleteData.ObjectID};
						//TenantGymTable.Attach(objectToDelete);
						var objectToDelete = TenantGymTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MemberCollection":
					{
						//var objectToDelete = new MemberCollection {ID = deleteData.ObjectID};
						//MemberCollectionTable.Attach(objectToDelete);
						var objectToDelete = MemberCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MemberCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MembershipPlanCollection":
					{
						//var objectToDelete = new MembershipPlanCollection {ID = deleteData.ObjectID};
						//MembershipPlanCollectionTable.Attach(objectToDelete);
						var objectToDelete = MembershipPlanCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MembershipPlanCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PaymentOptionCollection":
					{
						//var objectToDelete = new PaymentOptionCollection {ID = deleteData.ObjectID};
						//PaymentOptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = PaymentOptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PaymentOptionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SubscriptionCollection":
					{
						//var objectToDelete = new SubscriptionCollection {ID = deleteData.ObjectID};
						//SubscriptionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SubscriptionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SubscriptionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TenantGymCollection":
					{
						//var objectToDelete = new TenantGymCollection {ID = deleteData.ObjectID};
						//TenantGymCollectionTable.Attach(objectToDelete);
						var objectToDelete = TenantGymCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TenantGymCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



			public Table<Member> MemberTable {
				get {
					return this.GetTable<Member>();
				}
			}
			public Table<MemberSubscriptions> MemberSubscriptionsTable {
				get {
					return this.GetTable<MemberSubscriptions>();
				}
			}

			public Table<MembershipPlan> MembershipPlanTable {
				get {
					return this.GetTable<MembershipPlan>();
				}
			}
			public Table<MembershipPlanPaymentOptions> MembershipPlanPaymentOptionsTable {
				get {
					return this.GetTable<MembershipPlanPaymentOptions>();
				}
			}

			public Table<MembershipPlanGym> MembershipPlanGymTable {
				get {
					return this.GetTable<MembershipPlanGym>();
				}
			}

			public Table<PaymentOption> PaymentOptionTable {
				get {
					return this.GetTable<PaymentOption>();
				}
			}
			public Table<Subscription> SubscriptionTable {
				get {
					return this.GetTable<Subscription>();
				}
			}
			public Table<SubscriptionPlan> SubscriptionPlanTable {
				get {
					return this.GetTable<SubscriptionPlan>();
				}
			}

			public Table<SubscriptionPaymentOption> SubscriptionPaymentOptionTable {
				get {
					return this.GetTable<SubscriptionPaymentOption>();
				}
			}

			public Table<TenantGym> TenantGymTable {
				get {
					return this.GetTable<TenantGym>();
				}
			}
			public Table<MemberCollection> MemberCollectionTable {
				get {
					return this.GetTable<MemberCollection>();
				}
			}
			public Table<MembershipPlanCollection> MembershipPlanCollectionTable {
				get {
					return this.GetTable<MembershipPlanCollection>();
				}
			}
			public Table<PaymentOptionCollection> PaymentOptionCollectionTable {
				get {
					return this.GetTable<PaymentOptionCollection>();
				}
			}
			public Table<SubscriptionCollection> SubscriptionCollectionTable {
				get {
					return this.GetTable<SubscriptionCollection>();
				}
			}
			public Table<TenantGymCollection> TenantGymCollectionTable {
				get {
					return this.GetTable<TenantGymCollection>();
				}
			}
        }

    [Table(Name = "Member")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Member: {ID}")]
	public class Member : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[MiddleName] TEXT NOT NULL, 
[BirthDay] TEXT NOT NULL, 
[Email] TEXT NOT NULL, 
[PhoneNumber] TEXT NOT NULL, 
[Address] TEXT NOT NULL, 
[Address2] TEXT NOT NULL, 
[ZipCode] TEXT NOT NULL, 
[PostOffice] TEXT NOT NULL, 
[Country] TEXT NOT NULL, 
[FederationLicense] TEXT NOT NULL, 
[PhotoPermission] INTEGER NOT NULL, 
[VideoPermission] INTEGER NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string FirstName { get; set; }
		// private string _unmodified_FirstName;

		[Column]
        [ScaffoldColumn(true)]
		public string LastName { get; set; }
		// private string _unmodified_LastName;

		[Column]
        [ScaffoldColumn(true)]
		public string MiddleName { get; set; }
		// private string _unmodified_MiddleName;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime BirthDay { get; set; }
		// private DateTime _unmodified_BirthDay;

		[Column]
        [ScaffoldColumn(true)]
		public string Email { get; set; }
		// private string _unmodified_Email;

		[Column]
        [ScaffoldColumn(true)]
		public string PhoneNumber { get; set; }
		// private string _unmodified_PhoneNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string Address { get; set; }
		// private string _unmodified_Address;

		[Column]
        [ScaffoldColumn(true)]
		public string Address2 { get; set; }
		// private string _unmodified_Address2;

		[Column]
        [ScaffoldColumn(true)]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		[Column]
        [ScaffoldColumn(true)]
		public string PostOffice { get; set; }
		// private string _unmodified_PostOffice;

		[Column]
        [ScaffoldColumn(true)]
		public string Country { get; set; }
		// private string _unmodified_Country;

		[Column]
        [ScaffoldColumn(true)]
		public string FederationLicense { get; set; }
		// private string _unmodified_FederationLicense;

		[Column]
        [ScaffoldColumn(true)]
		public bool PhotoPermission { get; set; }
		// private bool _unmodified_PhotoPermission;

		[Column]
        [ScaffoldColumn(true)]
		public bool VideoPermission { get; set; }
		// private bool _unmodified_VideoPermission;
		private EntitySet<MemberSubscriptions> _Subscriptions = new EntitySet<MemberSubscriptions>();
        [Association(ThisKey = "ID", OtherKey = "MemberID", Storage="_Subscriptions")]
        public EntitySet<MemberSubscriptions> Subscriptions { 
			get { return _Subscriptions; }
			set { _Subscriptions.Assign(value); }
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
    [Table(Name = "MembershipPlan")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlan: {ID}")]
	public class MembershipPlan : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[PlanName] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
		private EntitySet<MembershipPlanPaymentOptions> _PaymentOptions = new EntitySet<MembershipPlanPaymentOptions>();
        [Association(ThisKey = "ID", OtherKey = "MembershipPlanID", Storage="_PaymentOptions")]
        public EntitySet<MembershipPlanPaymentOptions> PaymentOptions { 
			get { return _PaymentOptions; }
			set { _PaymentOptions.Assign(value); }
		}

		private EntityRef<MembershipPlanGym> _Gym = new EntityRef<MembershipPlanGym>();
        [Association(ThisKey = "ID", OtherKey = "MembershipPlanID", Storage="_Gym")]
        public MembershipPlanGym Gym 
		{ 
			get { return _Gym.Entity; }
			set { _Gym.Entity = value; }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PlanName == null)
				PlanName = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "PaymentOption")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("PaymentOption: {ID}")]
	public class PaymentOption : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[OptionName] TEXT NOT NULL, 
[PeriodInDays] INTEGER NOT NULL, 
[Price] REAL NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string OptionName { get; set; }
		// private string _unmodified_OptionName;

		[Column]
        [ScaffoldColumn(true)]
		public long PeriodInDays { get; set; }
		// private long _unmodified_PeriodInDays;

		[Column]
        [ScaffoldColumn(true)]
		public double Price { get; set; }
		// private double _unmodified_Price;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OptionName == null)
				OptionName = string.Empty;
		}
	}
    [Table(Name = "Subscription")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Subscription: {ID}")]
	public class Subscription : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[Created] TEXT NOT NULL, 
[ValidFrom] TEXT NOT NULL, 
[ValidTo] TEXT NOT NULL
)";
        }

		private EntityRef<SubscriptionPlan> _Plan = new EntityRef<SubscriptionPlan>();
        [Association(ThisKey = "ID", OtherKey = "SubscriptionID", Storage="_Plan")]
        public SubscriptionPlan Plan 
		{ 
			get { return _Plan.Entity; }
			set { _Plan.Entity = value; }
		}

		private EntityRef<SubscriptionPaymentOption> _PaymentOption = new EntityRef<SubscriptionPaymentOption>();
        [Association(ThisKey = "ID", OtherKey = "SubscriptionID", Storage="_PaymentOption")]
        public SubscriptionPaymentOption PaymentOption 
		{ 
			get { return _PaymentOption.Entity; }
			set { _PaymentOption.Entity = value; }
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime Created { get; set; }
		// private DateTime _unmodified_Created;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ValidFrom { get; set; }
		// private DateTime _unmodified_ValidFrom;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ValidTo { get; set; }
		// private DateTime _unmodified_ValidTo;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TenantGym")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TenantGym: {ID}")]
	public class TenantGym : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[GymName] TEXT NOT NULL, 
[Email] TEXT NOT NULL, 
[PhoneNumber] TEXT NOT NULL, 
[Address] TEXT NOT NULL, 
[Address2] TEXT NOT NULL, 
[ZipCode] TEXT NOT NULL, 
[PostOffice] TEXT NOT NULL, 
[Country] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GymName { get; set; }
		// private string _unmodified_GymName;

		[Column]
        [ScaffoldColumn(true)]
		public string Email { get; set; }
		// private string _unmodified_Email;

		[Column]
        [ScaffoldColumn(true)]
		public string PhoneNumber { get; set; }
		// private string _unmodified_PhoneNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string Address { get; set; }
		// private string _unmodified_Address;

		[Column]
        [ScaffoldColumn(true)]
		public string Address2 { get; set; }
		// private string _unmodified_Address2;

		[Column]
        [ScaffoldColumn(true)]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		[Column]
        [ScaffoldColumn(true)]
		public string PostOffice { get; set; }
		// private string _unmodified_PostOffice;

		[Column]
        [ScaffoldColumn(true)]
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
    [Table(Name = "MemberCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MemberCollection: {ID}")]
	public class MemberCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "MembershipPlanCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MembershipPlanCollection: {ID}")]
	public class MembershipPlanCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "PaymentOptionCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("PaymentOptionCollection: {ID}")]
	public class PaymentOptionCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "SubscriptionCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("SubscriptionCollection: {ID}")]
	public class SubscriptionCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "TenantGymCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TenantGymCollection: {ID}")]
	public class TenantGymCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string CollectionItemID { get; set; }
	}
    [Table(Name = "MemberSubscriptions")]
	[ScaffoldTable(true)]
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


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MemberID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }


        private EntityRef<Member> _Member = new EntityRef<Member>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MemberID", OtherKey = "ID", 
			Storage = "_Member", IsUnique = false)]
        public Member Member 
		{ 
			get { return _Member.Entity; }
			set { _Member.Entity = value; }
		}

        private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
			Storage = "_Subscription")]
		public Subscription Subscription 
		{ 
			get { return _Subscription.Entity; }
			set { _Subscription.Entity = value; }
		}

    }

    [Table(Name = "MembershipPlanPaymentOptions")]
	[ScaffoldTable(true)]
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


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string PaymentOptionID { get; set; }


        private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
			Storage = "_MembershipPlan", IsUnique = false)]
        public MembershipPlan MembershipPlan 
		{ 
			get { return _MembershipPlan.Entity; }
			set { _MembershipPlan.Entity = value; }
		}

        private EntityRef<PaymentOption> _PaymentOption = new EntityRef<PaymentOption>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "PaymentOptionID", OtherKey = "ID", 
			Storage = "_PaymentOption")]
		public PaymentOption PaymentOption 
		{ 
			get { return _PaymentOption.Entity; }
			set { _PaymentOption.Entity = value; }
		}

    }

    [Table(Name = "MembershipPlanGym")]
	[ScaffoldTable(true)]
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


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string TenantGymID { get; set; }


        private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
			Storage = "_MembershipPlan", IsUnique = true)]
        public MembershipPlan MembershipPlan 
		{ 
			get { return _MembershipPlan.Entity; }
			set { _MembershipPlan.Entity = value; }
		}

        private EntityRef<TenantGym> _TenantGym = new EntityRef<TenantGym>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "TenantGymID", OtherKey = "ID", 
			Storage = "_TenantGym")]
		public TenantGym TenantGym 
		{ 
			get { return _TenantGym.Entity; }
			set { _TenantGym.Entity = value; }
		}

    }

    [Table(Name = "SubscriptionPlan")]
	[ScaffoldTable(true)]
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


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string MembershipPlanID { get; set; }


        private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
			Storage = "_Subscription", IsUnique = true)]
        public Subscription Subscription 
		{ 
			get { return _Subscription.Entity; }
			set { _Subscription.Entity = value; }
		}

        private EntityRef<MembershipPlan> _MembershipPlan = new EntityRef<MembershipPlan>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "MembershipPlanID", OtherKey = "ID", 
			Storage = "_MembershipPlan")]
		public MembershipPlan MembershipPlan 
		{ 
			get { return _MembershipPlan.Entity; }
			set { _MembershipPlan.Entity = value; }
		}

    }

    [Table(Name = "SubscriptionPaymentOption")]
	[ScaffoldTable(true)]
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


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string SubscriptionID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string PaymentOptionID { get; set; }


        private EntityRef<Subscription> _Subscription = new EntityRef<Subscription>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "SubscriptionID", OtherKey = "ID", 
			Storage = "_Subscription", IsUnique = true)]
        public Subscription Subscription 
		{ 
			get { return _Subscription.Entity; }
			set { _Subscription.Entity = value; }
		}

        private EntityRef<PaymentOption> _PaymentOption = new EntityRef<PaymentOption>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "PaymentOptionID", OtherKey = "ID", 
			Storage = "_PaymentOption")]
		public PaymentOption PaymentOption 
		{ 
			get { return _PaymentOption.Entity; }
			set { _PaymentOption.Entity = value; }
		}

    }

 } 
