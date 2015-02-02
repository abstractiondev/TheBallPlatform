 


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
using System.ComponentModel.DataAnnotations;


namespace SQLite.TheBall.Interface { 
		
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
				tableCreationCommands.Add(WizardTask.GetCreateTableSQL());
				tableCreationCommands.Add(Connection.GetCreateTableSQL());
				tableCreationCommands.Add(TransferPackage.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryLink.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(StatusSummary.GetCreateTableSQL());
				tableCreationCommands.Add(InformationChangeItem.GetCreateTableSQL());
				tableCreationCommands.Add(OperationExecutionItem.GetCreateTableSQL());
				tableCreationCommands.Add(GenericObject.GetCreateTableSQL());
				tableCreationCommands.Add(GenericValue.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "WizardTask")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.WizardTask.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = WizardTaskTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TaskName = serializedObject.TaskName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.InputType = serializedObject.InputType;
		            return;
		        } 
		        if (updateData.ObjectType == "Connection")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.Connection.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ConnectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OutputInformationID = serializedObject.OutputInformationID;
		            existingObject.Description = serializedObject.Description;
		            existingObject.DeviceID = serializedObject.DeviceID;
		            existingObject.IsActiveParty = serializedObject.IsActiveParty;
		            existingObject.OtherSideConnectionID = serializedObject.OtherSideConnectionID;
		            existingObject.OperationNameToListPackageContents = serializedObject.OperationNameToListPackageContents;
		            existingObject.OperationNameToProcessReceived = serializedObject.OperationNameToProcessReceived;
		            existingObject.OperationNameToUpdateThisSideCategories = serializedObject.OperationNameToUpdateThisSideCategories;
		            existingObject.ProcessIDToListPackageContents = serializedObject.ProcessIDToListPackageContents;
		            existingObject.ProcessIDToProcessReceived = serializedObject.ProcessIDToProcessReceived;
		            existingObject.ProcessIDToUpdateThisSideCategories = serializedObject.ProcessIDToUpdateThisSideCategories;
		            return;
		        } 
		        if (updateData.ObjectType == "TransferPackage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.TransferPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TransferPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ConnectionID = serializedObject.ConnectionID;
		            existingObject.PackageDirection = serializedObject.PackageDirection;
		            existingObject.PackageType = serializedObject.PackageType;
		            existingObject.IsProcessed = serializedObject.IsProcessed;
                    existingObject.PackageContentBlobs.Clear();
					if(serializedObject.PackageContentBlobs != null)
	                    serializedObject.PackageContentBlobs.ForEach(item => existingObject.PackageContentBlobs.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "CategoryLink")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.CategoryLink.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryLinkTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceCategoryID = serializedObject.SourceCategoryID;
		            existingObject.TargetCategoryID = serializedObject.TargetCategoryID;
		            existingObject.LinkingType = serializedObject.LinkingType;
		            return;
		        } 
		        if (updateData.ObjectType == "Category")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.Category.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.NativeCategoryID = serializedObject.NativeCategoryID;
		            existingObject.NativeCategoryDomainName = serializedObject.NativeCategoryDomainName;
		            existingObject.NativeCategoryObjectName = serializedObject.NativeCategoryObjectName;
		            existingObject.NativeCategoryTitle = serializedObject.NativeCategoryTitle;
		            existingObject.IdentifyingCategoryName = serializedObject.IdentifyingCategoryName;
		            existingObject.ParentCategoryID = serializedObject.ParentCategoryID;
		            return;
		        } 
		        if (updateData.ObjectType == "StatusSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StatusSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.ChangeItemTrackingList.Clear();
					if(serializedObject.ChangeItemTrackingList != null)
	                    serializedObject.ChangeItemTrackingList.ForEach(item => existingObject.ChangeItemTrackingList.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "InformationChangeItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.InformationChangeItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationChangeItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StartTimeUTC = serializedObject.StartTimeUTC;
		            existingObject.EndTimeUTC = serializedObject.EndTimeUTC;
                    existingObject.ChangedObjectIDList.Clear();
					if(serializedObject.ChangedObjectIDList != null)
	                    serializedObject.ChangedObjectIDList.ForEach(item => existingObject.ChangedObjectIDList.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "OperationExecutionItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.OperationExecutionItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OperationExecutionItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OperationName = serializedObject.OperationName;
		            existingObject.OperationDomain = serializedObject.OperationDomain;
		            existingObject.OperationID = serializedObject.OperationID;
		            existingObject.CallerProvidedInfo = serializedObject.CallerProvidedInfo;
		            existingObject.CreationTime = serializedObject.CreationTime;
		            existingObject.ExecutionBeginTime = serializedObject.ExecutionBeginTime;
		            existingObject.ExecutionCompletedTime = serializedObject.ExecutionCompletedTime;
		            existingObject.ExecutionStatus = serializedObject.ExecutionStatus;
		            return;
		        } 
		        if (updateData.ObjectType == "GenericObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IncludeInCollection = serializedObject.IncludeInCollection;
		            existingObject.OptionalCollectionName = serializedObject.OptionalCollectionName;
		            return;
		        } 
		        if (updateData.ObjectType == "GenericValue")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.Interface.GenericValue.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericValueTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ValueName = serializedObject.ValueName;
		            existingObject.String = serializedObject.String;
                    existingObject.StringArray.Clear();
					if(serializedObject.StringArray != null)
	                    serializedObject.StringArray.ForEach(item => existingObject.StringArray.Add(item));
					
		            existingObject.Number = serializedObject.Number;
                    existingObject.NumberArray.Clear();
					if(serializedObject.NumberArray != null)
	                    serializedObject.NumberArray.ForEach(item => existingObject.NumberArray.Add(item));
					
		            existingObject.Boolean = serializedObject.Boolean;
                    existingObject.BooleanArray.Clear();
					if(serializedObject.BooleanArray != null)
	                    serializedObject.BooleanArray.ForEach(item => existingObject.BooleanArray.Add(item));
					
		            existingObject.DateTime = serializedObject.DateTime;
                    existingObject.DateTimeArray.Clear();
					if(serializedObject.DateTimeArray != null)
	                    serializedObject.DateTimeArray.ForEach(item => existingObject.DateTimeArray.Add(item));
					
		            existingObject.IndexingInfo = serializedObject.IndexingInfo;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "WizardTask")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.WizardTask.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new WizardTask {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TaskName = serializedObject.TaskName;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.InputType = serializedObject.InputType;
					WizardTaskTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Connection")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.Connection.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Connection {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OutputInformationID = serializedObject.OutputInformationID;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.DeviceID = serializedObject.DeviceID;
		            objectToAdd.IsActiveParty = serializedObject.IsActiveParty;
		            objectToAdd.OtherSideConnectionID = serializedObject.OtherSideConnectionID;
		            objectToAdd.OperationNameToListPackageContents = serializedObject.OperationNameToListPackageContents;
		            objectToAdd.OperationNameToProcessReceived = serializedObject.OperationNameToProcessReceived;
		            objectToAdd.OperationNameToUpdateThisSideCategories = serializedObject.OperationNameToUpdateThisSideCategories;
		            objectToAdd.ProcessIDToListPackageContents = serializedObject.ProcessIDToListPackageContents;
		            objectToAdd.ProcessIDToProcessReceived = serializedObject.ProcessIDToProcessReceived;
		            objectToAdd.ProcessIDToUpdateThisSideCategories = serializedObject.ProcessIDToUpdateThisSideCategories;
					ConnectionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TransferPackage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.TransferPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TransferPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ConnectionID = serializedObject.ConnectionID;
		            objectToAdd.PackageDirection = serializedObject.PackageDirection;
		            objectToAdd.PackageType = serializedObject.PackageType;
		            objectToAdd.IsProcessed = serializedObject.IsProcessed;
					if(serializedObject.PackageContentBlobs != null)
						serializedObject.PackageContentBlobs.ForEach(item => objectToAdd.PackageContentBlobs.Add(item));
					TransferPackageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CategoryLink")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.CategoryLink.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CategoryLink {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceCategoryID = serializedObject.SourceCategoryID;
		            objectToAdd.TargetCategoryID = serializedObject.TargetCategoryID;
		            objectToAdd.LinkingType = serializedObject.LinkingType;
					CategoryLinkTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Category")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.Category.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.NativeCategoryID = serializedObject.NativeCategoryID;
		            objectToAdd.NativeCategoryDomainName = serializedObject.NativeCategoryDomainName;
		            objectToAdd.NativeCategoryObjectName = serializedObject.NativeCategoryObjectName;
		            objectToAdd.NativeCategoryTitle = serializedObject.NativeCategoryTitle;
		            objectToAdd.IdentifyingCategoryName = serializedObject.IdentifyingCategoryName;
		            objectToAdd.ParentCategoryID = serializedObject.ParentCategoryID;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "StatusSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StatusSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ChangeItemTrackingList != null)
						serializedObject.ChangeItemTrackingList.ForEach(item => objectToAdd.ChangeItemTrackingList.Add(item));
					StatusSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationChangeItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.InformationChangeItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationChangeItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StartTimeUTC = serializedObject.StartTimeUTC;
		            objectToAdd.EndTimeUTC = serializedObject.EndTimeUTC;
					if(serializedObject.ChangedObjectIDList != null)
						serializedObject.ChangedObjectIDList.ForEach(item => objectToAdd.ChangedObjectIDList.Add(item));
					InformationChangeItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "OperationExecutionItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.OperationExecutionItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OperationExecutionItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OperationName = serializedObject.OperationName;
		            objectToAdd.OperationDomain = serializedObject.OperationDomain;
		            objectToAdd.OperationID = serializedObject.OperationID;
		            objectToAdd.CallerProvidedInfo = serializedObject.CallerProvidedInfo;
		            objectToAdd.CreationTime = serializedObject.CreationTime;
		            objectToAdd.ExecutionBeginTime = serializedObject.ExecutionBeginTime;
		            objectToAdd.ExecutionCompletedTime = serializedObject.ExecutionCompletedTime;
		            objectToAdd.ExecutionStatus = serializedObject.ExecutionStatus;
					OperationExecutionItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GenericObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IncludeInCollection = serializedObject.IncludeInCollection;
		            objectToAdd.OptionalCollectionName = serializedObject.OptionalCollectionName;
					GenericObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GenericValue")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericValue.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericValue {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ValueName = serializedObject.ValueName;
		            objectToAdd.String = serializedObject.String;
					if(serializedObject.StringArray != null)
						serializedObject.StringArray.ForEach(item => objectToAdd.StringArray.Add(item));
		            objectToAdd.Number = serializedObject.Number;
					if(serializedObject.NumberArray != null)
						serializedObject.NumberArray.ForEach(item => objectToAdd.NumberArray.Add(item));
		            objectToAdd.Boolean = serializedObject.Boolean;
					if(serializedObject.BooleanArray != null)
						serializedObject.BooleanArray.ForEach(item => objectToAdd.BooleanArray.Add(item));
		            objectToAdd.DateTime = serializedObject.DateTime;
					if(serializedObject.DateTimeArray != null)
						serializedObject.DateTimeArray.ForEach(item => objectToAdd.DateTimeArray.Add(item));
		            objectToAdd.IndexingInfo = serializedObject.IndexingInfo;
					GenericValueTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "WizardTask")
		        {
		            var objectToDelete = new WizardTask {ID = deleteData.ID};
                    WizardTaskTable.Attach(objectToDelete);
                    WizardTaskTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Connection")
		        {
		            var objectToDelete = new Connection {ID = deleteData.ID};
                    ConnectionTable.Attach(objectToDelete);
                    ConnectionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TransferPackage")
		        {
		            var objectToDelete = new TransferPackage {ID = deleteData.ID};
                    TransferPackageTable.Attach(objectToDelete);
                    TransferPackageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CategoryLink")
		        {
		            var objectToDelete = new CategoryLink {ID = deleteData.ID};
                    CategoryLinkTable.Attach(objectToDelete);
                    CategoryLinkTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Category")
		        {
		            var objectToDelete = new Category {ID = deleteData.ID};
                    CategoryTable.Attach(objectToDelete);
                    CategoryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "StatusSummary")
		        {
		            var objectToDelete = new StatusSummary {ID = deleteData.ID};
                    StatusSummaryTable.Attach(objectToDelete);
                    StatusSummaryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InformationChangeItem")
		        {
		            var objectToDelete = new InformationChangeItem {ID = deleteData.ID};
                    InformationChangeItemTable.Attach(objectToDelete);
                    InformationChangeItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "OperationExecutionItem")
		        {
		            var objectToDelete = new OperationExecutionItem {ID = deleteData.ID};
                    OperationExecutionItemTable.Attach(objectToDelete);
                    OperationExecutionItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GenericObject")
		        {
		            var objectToDelete = new GenericObject {ID = deleteData.ID};
                    GenericObjectTable.Attach(objectToDelete);
                    GenericObjectTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GenericValue")
		        {
		            var objectToDelete = new GenericValue {ID = deleteData.ID};
                    GenericValueTable.Attach(objectToDelete);
                    GenericValueTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		    }


			public Table<WizardTask> WizardTaskTable {
				get {
					return this.GetTable<WizardTask>();
				}
			}
			public Table<Connection> ConnectionTable {
				get {
					return this.GetTable<Connection>();
				}
			}
			public Table<TransferPackage> TransferPackageTable {
				get {
					return this.GetTable<TransferPackage>();
				}
			}
			public Table<CategoryLink> CategoryLinkTable {
				get {
					return this.GetTable<CategoryLink>();
				}
			}
			public Table<Category> CategoryTable {
				get {
					return this.GetTable<Category>();
				}
			}
			public Table<StatusSummary> StatusSummaryTable {
				get {
					return this.GetTable<StatusSummary>();
				}
			}
			public Table<InformationChangeItem> InformationChangeItemTable {
				get {
					return this.GetTable<InformationChangeItem>();
				}
			}
			public Table<OperationExecutionItem> OperationExecutionItemTable {
				get {
					return this.GetTable<OperationExecutionItem>();
				}
			}
			public Table<GenericObject> GenericObjectTable {
				get {
					return this.GetTable<GenericObject>();
				}
			}
			public Table<GenericValue> GenericValueTable {
				get {
					return this.GetTable<GenericValue>();
				}
			}
        }

    [Table(Name = "WizardTask")]
	[ScaffoldTable(true)]
	public class WizardTask : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [WizardTask](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[TaskName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[InputType] TEXT NOT NULL
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


		public WizardTask() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string TaskName { get; set; }
		// private string _unmodified_TaskName;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string InputType { get; set; }
		// private string _unmodified_InputType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TaskName == null)
				TaskName = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(InputType == null)
				InputType = string.Empty;
		}
	}
    [Table(Name = "Connection")]
	[ScaffoldTable(true)]
	public class Connection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Connection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OutputInformationID] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[DeviceID] TEXT NOT NULL, 
[IsActiveParty] INTEGER NOT NULL, 
[OtherSideConnectionID] TEXT NOT NULL, 
[OperationNameToListPackageContents] TEXT NOT NULL, 
[OperationNameToProcessReceived] TEXT NOT NULL, 
[OperationNameToUpdateThisSideCategories] TEXT NOT NULL, 
[ProcessIDToListPackageContents] TEXT NOT NULL, 
[ProcessIDToProcessReceived] TEXT NOT NULL, 
[ProcessIDToUpdateThisSideCategories] TEXT NOT NULL
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


		public Connection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OutputInformationID { get; set; }
		// private string _unmodified_OutputInformationID;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string DeviceID { get; set; }
		// private string _unmodified_DeviceID;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsActiveParty { get; set; }
		// private bool _unmodified_IsActiveParty;

		[Column]
        [ScaffoldColumn(true)]
		public string OtherSideConnectionID { get; set; }
		// private string _unmodified_OtherSideConnectionID;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationNameToListPackageContents { get; set; }
		// private string _unmodified_OperationNameToListPackageContents;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationNameToProcessReceived { get; set; }
		// private string _unmodified_OperationNameToProcessReceived;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationNameToUpdateThisSideCategories { get; set; }
		// private string _unmodified_OperationNameToUpdateThisSideCategories;

		[Column]
        [ScaffoldColumn(true)]
		public string ProcessIDToListPackageContents { get; set; }
		// private string _unmodified_ProcessIDToListPackageContents;

		[Column]
        [ScaffoldColumn(true)]
		public string ProcessIDToProcessReceived { get; set; }
		// private string _unmodified_ProcessIDToProcessReceived;

		[Column]
        [ScaffoldColumn(true)]
		public string ProcessIDToUpdateThisSideCategories { get; set; }
		// private string _unmodified_ProcessIDToUpdateThisSideCategories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OutputInformationID == null)
				OutputInformationID = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(DeviceID == null)
				DeviceID = string.Empty;
			if(OtherSideConnectionID == null)
				OtherSideConnectionID = string.Empty;
			if(OperationNameToListPackageContents == null)
				OperationNameToListPackageContents = string.Empty;
			if(OperationNameToProcessReceived == null)
				OperationNameToProcessReceived = string.Empty;
			if(OperationNameToUpdateThisSideCategories == null)
				OperationNameToUpdateThisSideCategories = string.Empty;
			if(ProcessIDToListPackageContents == null)
				ProcessIDToListPackageContents = string.Empty;
			if(ProcessIDToProcessReceived == null)
				ProcessIDToProcessReceived = string.Empty;
			if(ProcessIDToUpdateThisSideCategories == null)
				ProcessIDToUpdateThisSideCategories = string.Empty;
		}
	}
    [Table(Name = "TransferPackage")]
	[ScaffoldTable(true)]
	public class TransferPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TransferPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ConnectionID] TEXT NOT NULL, 
[PackageDirection] TEXT NOT NULL, 
[PackageType] TEXT NOT NULL, 
[IsProcessed] INTEGER NOT NULL, 
[PackageContentBlobs] TEXT NOT NULL
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


		public TransferPackage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ConnectionID { get; set; }
		// private string _unmodified_ConnectionID;

		[Column]
        [ScaffoldColumn(true)]
		public string PackageDirection { get; set; }
		// private string _unmodified_PackageDirection;

		[Column]
        [ScaffoldColumn(true)]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsProcessed { get; set; }
		// private bool _unmodified_IsProcessed;
        [Column(Name = "PackageContentBlobs")] 
        [ScaffoldColumn(true)]
		public string PackageContentBlobsData { get; set; }

        private bool _IsPackageContentBlobsRetrieved = false;
        private bool _IsPackageContentBlobsChanged = false;
        private ObservableCollection<string> _PackageContentBlobs = null;
        public ObservableCollection<string> PackageContentBlobs
        {
            get
            {
                if (!_IsPackageContentBlobsRetrieved)
                {
                    if (PackageContentBlobsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(PackageContentBlobsData);
                        _PackageContentBlobs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _PackageContentBlobs = new ObservableCollection<string>();
						PackageContentBlobsData = Guid.NewGuid().ToString();
						_IsPackageContentBlobsChanged = true;
                    }
                    _IsPackageContentBlobsRetrieved = true;
                    _PackageContentBlobs.CollectionChanged += (sender, args) =>
						{
							PackageContentBlobsData = Guid.NewGuid().ToString();
							_IsPackageContentBlobsChanged = true;
						};
                }
                return _PackageContentBlobs;
            }
            set 
			{ 
				_PackageContentBlobs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsPackageContentBlobsRetrieved = true;
                PackageContentBlobsData = Guid.NewGuid().ToString();
                _IsPackageContentBlobsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ConnectionID == null)
				ConnectionID = string.Empty;
			if(PackageDirection == null)
				PackageDirection = string.Empty;
			if(PackageType == null)
				PackageType = string.Empty;
            if (_IsPackageContentBlobsChanged || isInitialInsert)
            {
                var dataToStore = PackageContentBlobs.ToArray();
                PackageContentBlobsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "CategoryLink")]
	[ScaffoldTable(true)]
	public class CategoryLink : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CategoryLink](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[SourceCategoryID] TEXT NOT NULL, 
[TargetCategoryID] TEXT NOT NULL, 
[LinkingType] TEXT NOT NULL
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


		public CategoryLink() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SourceCategoryID { get; set; }
		// private string _unmodified_SourceCategoryID;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetCategoryID { get; set; }
		// private string _unmodified_TargetCategoryID;

		[Column]
        [ScaffoldColumn(true)]
		public string LinkingType { get; set; }
		// private string _unmodified_LinkingType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SourceCategoryID == null)
				SourceCategoryID = string.Empty;
			if(TargetCategoryID == null)
				TargetCategoryID = string.Empty;
			if(LinkingType == null)
				LinkingType = string.Empty;
		}
	}
    [Table(Name = "Category")]
	[ScaffoldTable(true)]
	public class Category : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Category](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[NativeCategoryID] TEXT NOT NULL, 
[NativeCategoryDomainName] TEXT NOT NULL, 
[NativeCategoryObjectName] TEXT NOT NULL, 
[NativeCategoryTitle] TEXT NOT NULL, 
[IdentifyingCategoryName] TEXT NOT NULL, 
[ParentCategoryID] TEXT NOT NULL
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


		public Category() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string NativeCategoryID { get; set; }
		// private string _unmodified_NativeCategoryID;

		[Column]
        [ScaffoldColumn(true)]
		public string NativeCategoryDomainName { get; set; }
		// private string _unmodified_NativeCategoryDomainName;

		[Column]
        [ScaffoldColumn(true)]
		public string NativeCategoryObjectName { get; set; }
		// private string _unmodified_NativeCategoryObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string NativeCategoryTitle { get; set; }
		// private string _unmodified_NativeCategoryTitle;

		[Column]
        [ScaffoldColumn(true)]
		public string IdentifyingCategoryName { get; set; }
		// private string _unmodified_IdentifyingCategoryName;

		[Column]
        [ScaffoldColumn(true)]
		public string ParentCategoryID { get; set; }
		// private string _unmodified_ParentCategoryID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(NativeCategoryID == null)
				NativeCategoryID = string.Empty;
			if(NativeCategoryDomainName == null)
				NativeCategoryDomainName = string.Empty;
			if(NativeCategoryObjectName == null)
				NativeCategoryObjectName = string.Empty;
			if(NativeCategoryTitle == null)
				NativeCategoryTitle = string.Empty;
			if(IdentifyingCategoryName == null)
				IdentifyingCategoryName = string.Empty;
			if(ParentCategoryID == null)
				ParentCategoryID = string.Empty;
		}
	}
    [Table(Name = "StatusSummary")]
	[ScaffoldTable(true)]
	public class StatusSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StatusSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ChangeItemTrackingList] TEXT NOT NULL
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


		public StatusSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        [Column(Name = "ChangeItemTrackingList")] 
        [ScaffoldColumn(true)]
		public string ChangeItemTrackingListData { get; set; }

        private bool _IsChangeItemTrackingListRetrieved = false;
        private bool _IsChangeItemTrackingListChanged = false;
        private ObservableCollection<string> _ChangeItemTrackingList = null;
        public ObservableCollection<string> ChangeItemTrackingList
        {
            get
            {
                if (!_IsChangeItemTrackingListRetrieved)
                {
                    if (ChangeItemTrackingListData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ChangeItemTrackingListData);
                        _ChangeItemTrackingList = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ChangeItemTrackingList = new ObservableCollection<string>();
						ChangeItemTrackingListData = Guid.NewGuid().ToString();
						_IsChangeItemTrackingListChanged = true;
                    }
                    _IsChangeItemTrackingListRetrieved = true;
                    _ChangeItemTrackingList.CollectionChanged += (sender, args) =>
						{
							ChangeItemTrackingListData = Guid.NewGuid().ToString();
							_IsChangeItemTrackingListChanged = true;
						};
                }
                return _ChangeItemTrackingList;
            }
            set 
			{ 
				_ChangeItemTrackingList = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsChangeItemTrackingListRetrieved = true;
                ChangeItemTrackingListData = Guid.NewGuid().ToString();
                _IsChangeItemTrackingListChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsChangeItemTrackingListChanged || isInitialInsert)
            {
                var dataToStore = ChangeItemTrackingList.ToArray();
                ChangeItemTrackingListData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "InformationChangeItem")]
	[ScaffoldTable(true)]
	public class InformationChangeItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationChangeItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[StartTimeUTC] TEXT NOT NULL, 
[EndTimeUTC] TEXT NOT NULL, 
[ChangedObjectIDList] TEXT NOT NULL
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


		public InformationChangeItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
        [Column(Name = "ChangedObjectIDList")] 
        [ScaffoldColumn(true)]
		public string ChangedObjectIDListData { get; set; }

        private bool _IsChangedObjectIDListRetrieved = false;
        private bool _IsChangedObjectIDListChanged = false;
        private ObservableCollection<string> _ChangedObjectIDList = null;
        public ObservableCollection<string> ChangedObjectIDList
        {
            get
            {
                if (!_IsChangedObjectIDListRetrieved)
                {
                    if (ChangedObjectIDListData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ChangedObjectIDListData);
                        _ChangedObjectIDList = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ChangedObjectIDList = new ObservableCollection<string>();
						ChangedObjectIDListData = Guid.NewGuid().ToString();
						_IsChangedObjectIDListChanged = true;
                    }
                    _IsChangedObjectIDListRetrieved = true;
                    _ChangedObjectIDList.CollectionChanged += (sender, args) =>
						{
							ChangedObjectIDListData = Guid.NewGuid().ToString();
							_IsChangedObjectIDListChanged = true;
						};
                }
                return _ChangedObjectIDList;
            }
            set 
			{ 
				_ChangedObjectIDList = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsChangedObjectIDListRetrieved = true;
                ChangedObjectIDListData = Guid.NewGuid().ToString();
                _IsChangedObjectIDListChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsChangedObjectIDListChanged || isInitialInsert)
            {
                var dataToStore = ChangedObjectIDList.ToArray();
                ChangedObjectIDListData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "OperationExecutionItem")]
	[ScaffoldTable(true)]
	public class OperationExecutionItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OperationExecutionItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OperationName] TEXT NOT NULL, 
[OperationDomain] TEXT NOT NULL, 
[OperationID] TEXT NOT NULL, 
[CallerProvidedInfo] TEXT NOT NULL, 
[CreationTime] TEXT NOT NULL, 
[ExecutionBeginTime] TEXT NOT NULL, 
[ExecutionCompletedTime] TEXT NOT NULL, 
[ExecutionStatus] TEXT NOT NULL
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


		public OperationExecutionItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OperationName { get; set; }
		// private string _unmodified_OperationName;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationDomain { get; set; }
		// private string _unmodified_OperationDomain;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationID { get; set; }
		// private string _unmodified_OperationID;

		[Column]
        [ScaffoldColumn(true)]
		public string CallerProvidedInfo { get; set; }
		// private string _unmodified_CallerProvidedInfo;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ExecutionBeginTime { get; set; }
		// private DateTime _unmodified_ExecutionBeginTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ExecutionCompletedTime { get; set; }
		// private DateTime _unmodified_ExecutionCompletedTime;

		[Column]
        [ScaffoldColumn(true)]
		public string ExecutionStatus { get; set; }
		// private string _unmodified_ExecutionStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OperationName == null)
				OperationName = string.Empty;
			if(OperationDomain == null)
				OperationDomain = string.Empty;
			if(OperationID == null)
				OperationID = string.Empty;
			if(CallerProvidedInfo == null)
				CallerProvidedInfo = string.Empty;
			if(ExecutionStatus == null)
				ExecutionStatus = string.Empty;
		}
	}
    [Table(Name = "GenericObject")]
	[ScaffoldTable(true)]
	public class GenericObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[IncludeInCollection] INTEGER NOT NULL, 
[OptionalCollectionName] TEXT NOT NULL
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


		public GenericObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		[Column]
        [ScaffoldColumn(true)]
		public string OptionalCollectionName { get; set; }
		// private string _unmodified_OptionalCollectionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OptionalCollectionName == null)
				OptionalCollectionName = string.Empty;
		}
	}
    [Table(Name = "GenericValue")]
	[ScaffoldTable(true)]
	public class GenericValue : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericValue](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ValueName] TEXT NOT NULL, 
[String] TEXT NOT NULL, 
[StringArray] TEXT NOT NULL, 
[Number] REAL NOT NULL, 
[NumberArray] TEXT NOT NULL, 
[Boolean] INTEGER NOT NULL, 
[BooleanArray] TEXT NOT NULL, 
[DateTime] TEXT NOT NULL, 
[DateTimeArray] TEXT NOT NULL, 
[IndexingInfo] TEXT NOT NULL
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


		public GenericValue() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		[Column]
        [ScaffoldColumn(true)]
		public string String { get; set; }
		// private string _unmodified_String;
        [Column(Name = "StringArray")] 
        [ScaffoldColumn(true)]
		public string StringArrayData { get; set; }

        private bool _IsStringArrayRetrieved = false;
        private bool _IsStringArrayChanged = false;
        private ObservableCollection<string> _StringArray = null;
        public ObservableCollection<string> StringArray
        {
            get
            {
                if (!_IsStringArrayRetrieved)
                {
                    if (StringArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(StringArrayData);
                        _StringArray = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _StringArray = new ObservableCollection<string>();
						StringArrayData = Guid.NewGuid().ToString();
						_IsStringArrayChanged = true;
                    }
                    _IsStringArrayRetrieved = true;
                    _StringArray.CollectionChanged += (sender, args) =>
						{
							StringArrayData = Guid.NewGuid().ToString();
							_IsStringArrayChanged = true;
						};
                }
                return _StringArray;
            }
            set 
			{ 
				_StringArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsStringArrayRetrieved = true;
                StringArrayData = Guid.NewGuid().ToString();
                _IsStringArrayChanged = true;

			}
        }


		[Column]
        [ScaffoldColumn(true)]
		public double Number { get; set; }
		// private double _unmodified_Number;
        [Column(Name = "NumberArray")] 
        [ScaffoldColumn(true)]
		public string NumberArrayData { get; set; }

        private bool _IsNumberArrayRetrieved = false;
        private bool _IsNumberArrayChanged = false;
        private ObservableCollection<double> _NumberArray = null;
        public ObservableCollection<double> NumberArray
        {
            get
            {
                if (!_IsNumberArrayRetrieved)
                {
                    if (NumberArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<double[]>(NumberArrayData);
                        _NumberArray = new ObservableCollection<double>(arrayData);
                    }
                    else
                    {
                        _NumberArray = new ObservableCollection<double>();
						NumberArrayData = Guid.NewGuid().ToString();
						_IsNumberArrayChanged = true;
                    }
                    _IsNumberArrayRetrieved = true;
                    _NumberArray.CollectionChanged += (sender, args) =>
						{
							NumberArrayData = Guid.NewGuid().ToString();
							_IsNumberArrayChanged = true;
						};
                }
                return _NumberArray;
            }
            set 
			{ 
				_NumberArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsNumberArrayRetrieved = true;
                NumberArrayData = Guid.NewGuid().ToString();
                _IsNumberArrayChanged = true;

			}
        }


		[Column]
        [ScaffoldColumn(true)]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        [Column(Name = "BooleanArray")] 
        [ScaffoldColumn(true)]
		public string BooleanArrayData { get; set; }

        private bool _IsBooleanArrayRetrieved = false;
        private bool _IsBooleanArrayChanged = false;
        private ObservableCollection<bool> _BooleanArray = null;
        public ObservableCollection<bool> BooleanArray
        {
            get
            {
                if (!_IsBooleanArrayRetrieved)
                {
                    if (BooleanArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<bool[]>(BooleanArrayData);
                        _BooleanArray = new ObservableCollection<bool>(arrayData);
                    }
                    else
                    {
                        _BooleanArray = new ObservableCollection<bool>();
						BooleanArrayData = Guid.NewGuid().ToString();
						_IsBooleanArrayChanged = true;
                    }
                    _IsBooleanArrayRetrieved = true;
                    _BooleanArray.CollectionChanged += (sender, args) =>
						{
							BooleanArrayData = Guid.NewGuid().ToString();
							_IsBooleanArrayChanged = true;
						};
                }
                return _BooleanArray;
            }
            set 
			{ 
				_BooleanArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsBooleanArrayRetrieved = true;
                BooleanArrayData = Guid.NewGuid().ToString();
                _IsBooleanArrayChanged = true;

			}
        }


		[Column]
        [ScaffoldColumn(true)]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        [Column(Name = "DateTimeArray")] 
        [ScaffoldColumn(true)]
		public string DateTimeArrayData { get; set; }

        private bool _IsDateTimeArrayRetrieved = false;
        private bool _IsDateTimeArrayChanged = false;
        private ObservableCollection<DateTime> _DateTimeArray = null;
        public ObservableCollection<DateTime> DateTimeArray
        {
            get
            {
                if (!_IsDateTimeArrayRetrieved)
                {
                    if (DateTimeArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<DateTime[]>(DateTimeArrayData);
                        _DateTimeArray = new ObservableCollection<DateTime>(arrayData);
                    }
                    else
                    {
                        _DateTimeArray = new ObservableCollection<DateTime>();
						DateTimeArrayData = Guid.NewGuid().ToString();
						_IsDateTimeArrayChanged = true;
                    }
                    _IsDateTimeArrayRetrieved = true;
                    _DateTimeArray.CollectionChanged += (sender, args) =>
						{
							DateTimeArrayData = Guid.NewGuid().ToString();
							_IsDateTimeArrayChanged = true;
						};
                }
                return _DateTimeArray;
            }
            set 
			{ 
				_DateTimeArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsDateTimeArrayRetrieved = true;
                DateTimeArrayData = Guid.NewGuid().ToString();
                _IsDateTimeArrayChanged = true;

			}
        }


		[Column]
        [ScaffoldColumn(true)]
		public string IndexingInfo { get; set; }
		// private string _unmodified_IndexingInfo;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ValueName == null)
				ValueName = string.Empty;
			if(String == null)
				String = string.Empty;
			if(IndexingInfo == null)
				IndexingInfo = string.Empty;
            if (_IsStringArrayChanged || isInitialInsert)
            {
                var dataToStore = StringArray.ToArray();
                StringArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsNumberArrayChanged || isInitialInsert)
            {
                var dataToStore = NumberArray.ToArray();
                NumberArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsBooleanArrayChanged || isInitialInsert)
            {
                var dataToStore = BooleanArray.ToArray();
                BooleanArrayData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsDateTimeArrayChanged || isInitialInsert)
            {
                var dataToStore = DateTimeArray.ToArray();
                DateTimeArrayData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
 } 
