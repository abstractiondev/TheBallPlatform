 


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
				tableCreationCommands.Add(WizardContainer.GetCreateTableSQL());
				tableCreationCommands.Add(WizardTask.GetCreateTableSQL());
				tableCreationCommands.Add(Connection.GetCreateTableSQL());
				tableCreationCommands.Add(TransferPackage.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryLink.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(StatusSummary.GetCreateTableSQL());
				tableCreationCommands.Add(InformationChangeItem.GetCreateTableSQL());
				tableCreationCommands.Add(OperationExecutionItem.GetCreateTableSQL());
				tableCreationCommands.Add(GenericCollectionableObject.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "WizardContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.WizardContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = WizardContainerTable.Single(item => item.ID == updateData.ObjectID);
                    existingObject.ActiveTasks.Clear();
					if(serializedObject.ActiveTasks != null)
	                    serializedObject.ActiveTasks.ForEach(item => existingObject.ActiveTasks.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "WizardTask")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.WizardTask.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = WizardTaskTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.TaskName = serializedObject.TaskName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.InputType = serializedObject.InputType;
		            return;
		        } 
		        if (updateData.ObjectType == "Connection")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.Connection.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ConnectionTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.OutputInformationID = serializedObject.OutputInformationID;
		            existingObject.Description = serializedObject.Description;
		            existingObject.DeviceID = serializedObject.DeviceID;
		            existingObject.IsActiveParty = serializedObject.IsActiveParty;
		            existingObject.OtherSideConnectionID = serializedObject.OtherSideConnectionID;
                    existingObject.ThisSideCategories.Clear();
					if(serializedObject.ThisSideCategories != null)
	                    serializedObject.ThisSideCategories.ForEach(item => existingObject.ThisSideCategories.Add(item));
					
                    existingObject.OtherSideCategories.Clear();
					if(serializedObject.OtherSideCategories != null)
	                    serializedObject.OtherSideCategories.ForEach(item => existingObject.OtherSideCategories.Add(item));
					
                    existingObject.CategoryLinks.Clear();
					if(serializedObject.CategoryLinks != null)
	                    serializedObject.CategoryLinks.ForEach(item => existingObject.CategoryLinks.Add(item));
					
                    existingObject.IncomingPackages.Clear();
					if(serializedObject.IncomingPackages != null)
	                    serializedObject.IncomingPackages.ForEach(item => existingObject.IncomingPackages.Add(item));
					
                    existingObject.OutgoingPackages.Clear();
					if(serializedObject.OutgoingPackages != null)
	                    serializedObject.OutgoingPackages.ForEach(item => existingObject.OutgoingPackages.Add(item));
					
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
		                global::TheBall.Payments.TransferPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TransferPackageTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.CategoryLink.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryLinkTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.SourceCategoryID = serializedObject.SourceCategoryID;
		            existingObject.TargetCategoryID = serializedObject.TargetCategoryID;
		            existingObject.LinkingType = serializedObject.LinkingType;
		            return;
		        } 
		        if (updateData.ObjectType == "Category")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.Category.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.StatusSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StatusSummaryTable.Single(item => item.ID == updateData.ObjectID);
                    existingObject.PendingOperations.Clear();
					if(serializedObject.PendingOperations != null)
	                    serializedObject.PendingOperations.ForEach(item => existingObject.PendingOperations.Add(item));
					
                    existingObject.ExecutingOperations.Clear();
					if(serializedObject.ExecutingOperations != null)
	                    serializedObject.ExecutingOperations.ForEach(item => existingObject.ExecutingOperations.Add(item));
					
                    existingObject.RecentCompletedOperations.Clear();
					if(serializedObject.RecentCompletedOperations != null)
	                    serializedObject.RecentCompletedOperations.ForEach(item => existingObject.RecentCompletedOperations.Add(item));
					
                    existingObject.ChangeItemTrackingList.Clear();
					if(serializedObject.ChangeItemTrackingList != null)
	                    serializedObject.ChangeItemTrackingList.ForEach(item => existingObject.ChangeItemTrackingList.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "InformationChangeItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InformationChangeItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationChangeItemTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.OperationExecutionItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OperationExecutionItemTable.Single(item => item.ID == updateData.ObjectID);
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
		        if (updateData.ObjectType == "GenericCollectionableObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.GenericCollectionableObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericCollectionableObjectTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ValueObject = serializedObject.ValueObject;
		            return;
		        } 
		        if (updateData.ObjectType == "GenericObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.GenericObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericObjectTable.Single(item => item.ID == updateData.ObjectID);
                    existingObject.Values.Clear();
					if(serializedObject.Values != null)
	                    serializedObject.Values.ForEach(item => existingObject.Values.Add(item));
					
		            existingObject.IncludeInCollection = serializedObject.IncludeInCollection;
		            existingObject.OptionalCollectionName = serializedObject.OptionalCollectionName;
		            return;
		        } 
		        if (updateData.ObjectType == "GenericValue")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.GenericValue.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericValueTable.Single(item => item.ID == updateData.ObjectID);
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
					
		            existingObject.Object = serializedObject.Object;
                    existingObject.ObjectArray.Clear();
					if(serializedObject.ObjectArray != null)
	                    serializedObject.ObjectArray.ForEach(item => existingObject.ObjectArray.Add(item));
					
		            existingObject.IndexingInfo = serializedObject.IndexingInfo;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "WizardContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.WizardContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new WizardContainer {ID = insertData.ObjectID};
					if(serializedObject.ActiveTasks != null)
						serializedObject.ActiveTasks.ForEach(item => objectToAdd.ActiveTasks.Add(item));
					WizardContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "WizardTask")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.WizardTask.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new WizardTask {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.Connection.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Connection {ID = insertData.ObjectID};
		            objectToAdd.OutputInformationID = serializedObject.OutputInformationID;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.DeviceID = serializedObject.DeviceID;
		            objectToAdd.IsActiveParty = serializedObject.IsActiveParty;
		            objectToAdd.OtherSideConnectionID = serializedObject.OtherSideConnectionID;
					if(serializedObject.ThisSideCategories != null)
						serializedObject.ThisSideCategories.ForEach(item => objectToAdd.ThisSideCategories.Add(item));
					if(serializedObject.OtherSideCategories != null)
						serializedObject.OtherSideCategories.ForEach(item => objectToAdd.OtherSideCategories.Add(item));
					if(serializedObject.CategoryLinks != null)
						serializedObject.CategoryLinks.ForEach(item => objectToAdd.CategoryLinks.Add(item));
					if(serializedObject.IncomingPackages != null)
						serializedObject.IncomingPackages.ForEach(item => objectToAdd.IncomingPackages.Add(item));
					if(serializedObject.OutgoingPackages != null)
						serializedObject.OutgoingPackages.ForEach(item => objectToAdd.OutgoingPackages.Add(item));
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
                        global::TheBall.Payments.TransferPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TransferPackage {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.CategoryLink.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CategoryLink {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.Category.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.StatusSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StatusSummary {ID = insertData.ObjectID};
					if(serializedObject.PendingOperations != null)
						serializedObject.PendingOperations.ForEach(item => objectToAdd.PendingOperations.Add(item));
					if(serializedObject.ExecutingOperations != null)
						serializedObject.ExecutingOperations.ForEach(item => objectToAdd.ExecutingOperations.Add(item));
					if(serializedObject.RecentCompletedOperations != null)
						serializedObject.RecentCompletedOperations.ForEach(item => objectToAdd.RecentCompletedOperations.Add(item));
					if(serializedObject.ChangeItemTrackingList != null)
						serializedObject.ChangeItemTrackingList.ForEach(item => objectToAdd.ChangeItemTrackingList.Add(item));
					StatusSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationChangeItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InformationChangeItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationChangeItem {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.OperationExecutionItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OperationExecutionItem {ID = insertData.ObjectID};
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
                if (insertData.ObjectType == "GenericCollectionableObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.GenericCollectionableObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericCollectionableObject {ID = insertData.ObjectID};
		            objectToAdd.ValueObject = serializedObject.ValueObject;
					GenericCollectionableObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GenericObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.GenericObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericObject {ID = insertData.ObjectID};
					if(serializedObject.Values != null)
						serializedObject.Values.ForEach(item => objectToAdd.Values.Add(item));
		            objectToAdd.IncludeInCollection = serializedObject.IncludeInCollection;
		            objectToAdd.OptionalCollectionName = serializedObject.OptionalCollectionName;
					GenericObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GenericValue")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.GenericValue.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericValue {ID = insertData.ObjectID};
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
		            objectToAdd.Object = serializedObject.Object;
					if(serializedObject.ObjectArray != null)
						serializedObject.ObjectArray.ForEach(item => objectToAdd.ObjectArray.Add(item));
		            objectToAdd.IndexingInfo = serializedObject.IndexingInfo;
					GenericValueTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "WizardContainer")
		        {
                    WizardContainerTable.DeleteOnSubmit(new WizardContainer { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "WizardTask")
		        {
                    WizardTaskTable.DeleteOnSubmit(new WizardTask { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Connection")
		        {
                    ConnectionTable.DeleteOnSubmit(new Connection { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "TransferPackage")
		        {
                    TransferPackageTable.DeleteOnSubmit(new TransferPackage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "CategoryLink")
		        {
                    CategoryLinkTable.DeleteOnSubmit(new CategoryLink { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Category")
		        {
                    CategoryTable.DeleteOnSubmit(new Category { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "StatusSummary")
		        {
                    StatusSummaryTable.DeleteOnSubmit(new StatusSummary { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InformationChangeItem")
		        {
                    InformationChangeItemTable.DeleteOnSubmit(new InformationChangeItem { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "OperationExecutionItem")
		        {
                    OperationExecutionItemTable.DeleteOnSubmit(new OperationExecutionItem { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "GenericCollectionableObject")
		        {
                    GenericCollectionableObjectTable.DeleteOnSubmit(new GenericCollectionableObject { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "GenericObject")
		        {
                    GenericObjectTable.DeleteOnSubmit(new GenericObject { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "GenericValue")
		        {
                    GenericValueTable.DeleteOnSubmit(new GenericValue { ID = deleteData.ObjectID });
		            return;
		        }
		    }


			public Table<WizardContainer> WizardContainerTable {
				get {
					return this.GetTable<WizardContainer>();
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
			public Table<GenericCollectionableObject> GenericCollectionableObjectTable {
				get {
					return this.GetTable<GenericCollectionableObject>();
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

    [Table(Name = "WizardContainer")]
	public class WizardContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS WizardContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActiveTasks] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "ActiveTasks")] public string ActiveTasksData;

        private bool _IsActiveTasksRetrieved = false;
        private bool _IsActiveTasksChanged = false;
        private ObservableCollection<WizardTask> _ActiveTasks = null;
        public ObservableCollection<WizardTask> ActiveTasks
        {
            get
            {
                if (!_IsActiveTasksRetrieved)
                {
                    if (ActiveTasksData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<WizardTask[]>(ActiveTasksData);
                        _ActiveTasks = new ObservableCollection<WizardTask>(arrayData);
                    }
                    else
                    {
                        _ActiveTasks = new ObservableCollection<WizardTask>();
						ActiveTasksData = Guid.NewGuid().ToString();
						_IsActiveTasksChanged = true;
                    }
                    _IsActiveTasksRetrieved = true;
                    _ActiveTasks.CollectionChanged += (sender, args) =>
						{
							ActiveTasksData = Guid.NewGuid().ToString();
							_IsActiveTasksChanged = true;
						};
                }
                return _ActiveTasks;
            }
            set 
			{ 
				_ActiveTasks = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsActiveTasksRetrieved = true;
                ActiveTasksData = Guid.NewGuid().ToString();
                _IsActiveTasksChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsActiveTasksChanged || isInitialInsert)
            {
                var dataToStore = ActiveTasks.ToArray();
                ActiveTasksData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "WizardTask")]
	public class WizardTask : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS WizardTask(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TaskName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[InputType] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TaskName { get; set; }
		// private string _unmodified_TaskName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
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
	public class Connection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Connection(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OutputInformationID] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[DeviceID] TEXT NOT NULL, 
[IsActiveParty] INTEGER NOT NULL, 
[OtherSideConnectionID] TEXT NOT NULL, 
[ThisSideCategories] TEXT NOT NULL, 
[OtherSideCategories] TEXT NOT NULL, 
[CategoryLinks] TEXT NOT NULL, 
[IncomingPackages] TEXT NOT NULL, 
[OutgoingPackages] TEXT NOT NULL, 
[OperationNameToListPackageContents] TEXT NOT NULL, 
[OperationNameToProcessReceived] TEXT NOT NULL, 
[OperationNameToUpdateThisSideCategories] TEXT NOT NULL, 
[ProcessIDToListPackageContents] TEXT NOT NULL, 
[ProcessIDToProcessReceived] TEXT NOT NULL, 
[ProcessIDToUpdateThisSideCategories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OutputInformationID { get; set; }
		// private string _unmodified_OutputInformationID;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string DeviceID { get; set; }
		// private string _unmodified_DeviceID;

		[Column]
		public bool IsActiveParty { get; set; }
		// private bool _unmodified_IsActiveParty;

		[Column]
		public string OtherSideConnectionID { get; set; }
		// private string _unmodified_OtherSideConnectionID;
        [Column(Name = "ThisSideCategories")] public string ThisSideCategoriesData;

        private bool _IsThisSideCategoriesRetrieved = false;
        private bool _IsThisSideCategoriesChanged = false;
        private ObservableCollection<Category> _ThisSideCategories = null;
        public ObservableCollection<Category> ThisSideCategories
        {
            get
            {
                if (!_IsThisSideCategoriesRetrieved)
                {
                    if (ThisSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<Category[]>(ThisSideCategoriesData);
                        _ThisSideCategories = new ObservableCollection<Category>(arrayData);
                    }
                    else
                    {
                        _ThisSideCategories = new ObservableCollection<Category>();
						ThisSideCategoriesData = Guid.NewGuid().ToString();
						_IsThisSideCategoriesChanged = true;
                    }
                    _IsThisSideCategoriesRetrieved = true;
                    _ThisSideCategories.CollectionChanged += (sender, args) =>
						{
							ThisSideCategoriesData = Guid.NewGuid().ToString();
							_IsThisSideCategoriesChanged = true;
						};
                }
                return _ThisSideCategories;
            }
            set 
			{ 
				_ThisSideCategories = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsThisSideCategoriesRetrieved = true;
                ThisSideCategoriesData = Guid.NewGuid().ToString();
                _IsThisSideCategoriesChanged = true;

			}
        }

        [Column(Name = "OtherSideCategories")] public string OtherSideCategoriesData;

        private bool _IsOtherSideCategoriesRetrieved = false;
        private bool _IsOtherSideCategoriesChanged = false;
        private ObservableCollection<Category> _OtherSideCategories = null;
        public ObservableCollection<Category> OtherSideCategories
        {
            get
            {
                if (!_IsOtherSideCategoriesRetrieved)
                {
                    if (OtherSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<Category[]>(OtherSideCategoriesData);
                        _OtherSideCategories = new ObservableCollection<Category>(arrayData);
                    }
                    else
                    {
                        _OtherSideCategories = new ObservableCollection<Category>();
						OtherSideCategoriesData = Guid.NewGuid().ToString();
						_IsOtherSideCategoriesChanged = true;
                    }
                    _IsOtherSideCategoriesRetrieved = true;
                    _OtherSideCategories.CollectionChanged += (sender, args) =>
						{
							OtherSideCategoriesData = Guid.NewGuid().ToString();
							_IsOtherSideCategoriesChanged = true;
						};
                }
                return _OtherSideCategories;
            }
            set 
			{ 
				_OtherSideCategories = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsOtherSideCategoriesRetrieved = true;
                OtherSideCategoriesData = Guid.NewGuid().ToString();
                _IsOtherSideCategoriesChanged = true;

			}
        }

        [Column(Name = "CategoryLinks")] public string CategoryLinksData;

        private bool _IsCategoryLinksRetrieved = false;
        private bool _IsCategoryLinksChanged = false;
        private ObservableCollection<CategoryLink> _CategoryLinks = null;
        public ObservableCollection<CategoryLink> CategoryLinks
        {
            get
            {
                if (!_IsCategoryLinksRetrieved)
                {
                    if (CategoryLinksData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<CategoryLink[]>(CategoryLinksData);
                        _CategoryLinks = new ObservableCollection<CategoryLink>(arrayData);
                    }
                    else
                    {
                        _CategoryLinks = new ObservableCollection<CategoryLink>();
						CategoryLinksData = Guid.NewGuid().ToString();
						_IsCategoryLinksChanged = true;
                    }
                    _IsCategoryLinksRetrieved = true;
                    _CategoryLinks.CollectionChanged += (sender, args) =>
						{
							CategoryLinksData = Guid.NewGuid().ToString();
							_IsCategoryLinksChanged = true;
						};
                }
                return _CategoryLinks;
            }
            set 
			{ 
				_CategoryLinks = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsCategoryLinksRetrieved = true;
                CategoryLinksData = Guid.NewGuid().ToString();
                _IsCategoryLinksChanged = true;

			}
        }

        [Column(Name = "IncomingPackages")] public string IncomingPackagesData;

        private bool _IsIncomingPackagesRetrieved = false;
        private bool _IsIncomingPackagesChanged = false;
        private ObservableCollection<TransferPackage> _IncomingPackages = null;
        public ObservableCollection<TransferPackage> IncomingPackages
        {
            get
            {
                if (!_IsIncomingPackagesRetrieved)
                {
                    if (IncomingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(IncomingPackagesData);
                        _IncomingPackages = new ObservableCollection<TransferPackage>(arrayData);
                    }
                    else
                    {
                        _IncomingPackages = new ObservableCollection<TransferPackage>();
						IncomingPackagesData = Guid.NewGuid().ToString();
						_IsIncomingPackagesChanged = true;
                    }
                    _IsIncomingPackagesRetrieved = true;
                    _IncomingPackages.CollectionChanged += (sender, args) =>
						{
							IncomingPackagesData = Guid.NewGuid().ToString();
							_IsIncomingPackagesChanged = true;
						};
                }
                return _IncomingPackages;
            }
            set 
			{ 
				_IncomingPackages = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsIncomingPackagesRetrieved = true;
                IncomingPackagesData = Guid.NewGuid().ToString();
                _IsIncomingPackagesChanged = true;

			}
        }

        [Column(Name = "OutgoingPackages")] public string OutgoingPackagesData;

        private bool _IsOutgoingPackagesRetrieved = false;
        private bool _IsOutgoingPackagesChanged = false;
        private ObservableCollection<TransferPackage> _OutgoingPackages = null;
        public ObservableCollection<TransferPackage> OutgoingPackages
        {
            get
            {
                if (!_IsOutgoingPackagesRetrieved)
                {
                    if (OutgoingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<TransferPackage[]>(OutgoingPackagesData);
                        _OutgoingPackages = new ObservableCollection<TransferPackage>(arrayData);
                    }
                    else
                    {
                        _OutgoingPackages = new ObservableCollection<TransferPackage>();
						OutgoingPackagesData = Guid.NewGuid().ToString();
						_IsOutgoingPackagesChanged = true;
                    }
                    _IsOutgoingPackagesRetrieved = true;
                    _OutgoingPackages.CollectionChanged += (sender, args) =>
						{
							OutgoingPackagesData = Guid.NewGuid().ToString();
							_IsOutgoingPackagesChanged = true;
						};
                }
                return _OutgoingPackages;
            }
            set 
			{ 
				_OutgoingPackages = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsOutgoingPackagesRetrieved = true;
                OutgoingPackagesData = Guid.NewGuid().ToString();
                _IsOutgoingPackagesChanged = true;

			}
        }


		[Column]
		public string OperationNameToListPackageContents { get; set; }
		// private string _unmodified_OperationNameToListPackageContents;

		[Column]
		public string OperationNameToProcessReceived { get; set; }
		// private string _unmodified_OperationNameToProcessReceived;

		[Column]
		public string OperationNameToUpdateThisSideCategories { get; set; }
		// private string _unmodified_OperationNameToUpdateThisSideCategories;

		[Column]
		public string ProcessIDToListPackageContents { get; set; }
		// private string _unmodified_ProcessIDToListPackageContents;

		[Column]
		public string ProcessIDToProcessReceived { get; set; }
		// private string _unmodified_ProcessIDToProcessReceived;

		[Column]
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
            if (_IsThisSideCategoriesChanged || isInitialInsert)
            {
                var dataToStore = ThisSideCategories.ToArray();
                ThisSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsOtherSideCategoriesChanged || isInitialInsert)
            {
                var dataToStore = OtherSideCategories.ToArray();
                OtherSideCategoriesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsCategoryLinksChanged || isInitialInsert)
            {
                var dataToStore = CategoryLinks.ToArray();
                CategoryLinksData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsIncomingPackagesChanged || isInitialInsert)
            {
                var dataToStore = IncomingPackages.ToArray();
                IncomingPackagesData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsOutgoingPackagesChanged || isInitialInsert)
            {
                var dataToStore = OutgoingPackages.ToArray();
                OutgoingPackagesData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "TransferPackage")]
	public class TransferPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TransferPackage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ConnectionID] TEXT NOT NULL, 
[PackageDirection] TEXT NOT NULL, 
[PackageType] TEXT NOT NULL, 
[IsProcessed] INTEGER NOT NULL, 
[PackageContentBlobs] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ConnectionID { get; set; }
		// private string _unmodified_ConnectionID;

		[Column]
		public string PackageDirection { get; set; }
		// private string _unmodified_PackageDirection;

		[Column]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
		public bool IsProcessed { get; set; }
		// private bool _unmodified_IsProcessed;
        [Column(Name = "PackageContentBlobs")] public string PackageContentBlobsData;

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
	public class CategoryLink : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CategoryLink(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SourceCategoryID] TEXT NOT NULL, 
[TargetCategoryID] TEXT NOT NULL, 
[LinkingType] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SourceCategoryID { get; set; }
		// private string _unmodified_SourceCategoryID;

		[Column]
		public string TargetCategoryID { get; set; }
		// private string _unmodified_TargetCategoryID;

		[Column]
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
	public class Category : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Category(
[ID] TEXT NOT NULL PRIMARY KEY, 
[NativeCategoryID] TEXT NOT NULL, 
[NativeCategoryDomainName] TEXT NOT NULL, 
[NativeCategoryObjectName] TEXT NOT NULL, 
[NativeCategoryTitle] TEXT NOT NULL, 
[IdentifyingCategoryName] TEXT NOT NULL, 
[ParentCategoryID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string NativeCategoryID { get; set; }
		// private string _unmodified_NativeCategoryID;

		[Column]
		public string NativeCategoryDomainName { get; set; }
		// private string _unmodified_NativeCategoryDomainName;

		[Column]
		public string NativeCategoryObjectName { get; set; }
		// private string _unmodified_NativeCategoryObjectName;

		[Column]
		public string NativeCategoryTitle { get; set; }
		// private string _unmodified_NativeCategoryTitle;

		[Column]
		public string IdentifyingCategoryName { get; set; }
		// private string _unmodified_IdentifyingCategoryName;

		[Column]
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
	public class StatusSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS StatusSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PendingOperations] TEXT NOT NULL, 
[ExecutingOperations] TEXT NOT NULL, 
[RecentCompletedOperations] TEXT NOT NULL, 
[ChangeItemTrackingList] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "PendingOperations")] public string PendingOperationsData;

        private bool _IsPendingOperationsRetrieved = false;
        private bool _IsPendingOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _PendingOperations = null;
        public ObservableCollection<OperationExecutionItem> PendingOperations
        {
            get
            {
                if (!_IsPendingOperationsRetrieved)
                {
                    if (PendingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(PendingOperationsData);
                        _PendingOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _PendingOperations = new ObservableCollection<OperationExecutionItem>();
						PendingOperationsData = Guid.NewGuid().ToString();
						_IsPendingOperationsChanged = true;
                    }
                    _IsPendingOperationsRetrieved = true;
                    _PendingOperations.CollectionChanged += (sender, args) =>
						{
							PendingOperationsData = Guid.NewGuid().ToString();
							_IsPendingOperationsChanged = true;
						};
                }
                return _PendingOperations;
            }
            set 
			{ 
				_PendingOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsPendingOperationsRetrieved = true;
                PendingOperationsData = Guid.NewGuid().ToString();
                _IsPendingOperationsChanged = true;

			}
        }

        [Column(Name = "ExecutingOperations")] public string ExecutingOperationsData;

        private bool _IsExecutingOperationsRetrieved = false;
        private bool _IsExecutingOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _ExecutingOperations = null;
        public ObservableCollection<OperationExecutionItem> ExecutingOperations
        {
            get
            {
                if (!_IsExecutingOperationsRetrieved)
                {
                    if (ExecutingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(ExecutingOperationsData);
                        _ExecutingOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _ExecutingOperations = new ObservableCollection<OperationExecutionItem>();
						ExecutingOperationsData = Guid.NewGuid().ToString();
						_IsExecutingOperationsChanged = true;
                    }
                    _IsExecutingOperationsRetrieved = true;
                    _ExecutingOperations.CollectionChanged += (sender, args) =>
						{
							ExecutingOperationsData = Guid.NewGuid().ToString();
							_IsExecutingOperationsChanged = true;
						};
                }
                return _ExecutingOperations;
            }
            set 
			{ 
				_ExecutingOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsExecutingOperationsRetrieved = true;
                ExecutingOperationsData = Guid.NewGuid().ToString();
                _IsExecutingOperationsChanged = true;

			}
        }

        [Column(Name = "RecentCompletedOperations")] public string RecentCompletedOperationsData;

        private bool _IsRecentCompletedOperationsRetrieved = false;
        private bool _IsRecentCompletedOperationsChanged = false;
        private ObservableCollection<OperationExecutionItem> _RecentCompletedOperations = null;
        public ObservableCollection<OperationExecutionItem> RecentCompletedOperations
        {
            get
            {
                if (!_IsRecentCompletedOperationsRetrieved)
                {
                    if (RecentCompletedOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<OperationExecutionItem[]>(RecentCompletedOperationsData);
                        _RecentCompletedOperations = new ObservableCollection<OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _RecentCompletedOperations = new ObservableCollection<OperationExecutionItem>();
						RecentCompletedOperationsData = Guid.NewGuid().ToString();
						_IsRecentCompletedOperationsChanged = true;
                    }
                    _IsRecentCompletedOperationsRetrieved = true;
                    _RecentCompletedOperations.CollectionChanged += (sender, args) =>
						{
							RecentCompletedOperationsData = Guid.NewGuid().ToString();
							_IsRecentCompletedOperationsChanged = true;
						};
                }
                return _RecentCompletedOperations;
            }
            set 
			{ 
				_RecentCompletedOperations = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsRecentCompletedOperationsRetrieved = true;
                RecentCompletedOperationsData = Guid.NewGuid().ToString();
                _IsRecentCompletedOperationsChanged = true;

			}
        }

        [Column(Name = "ChangeItemTrackingList")] public string ChangeItemTrackingListData;

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
		
            if (_IsPendingOperationsChanged || isInitialInsert)
            {
                var dataToStore = PendingOperations.ToArray();
                PendingOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsExecutingOperationsChanged || isInitialInsert)
            {
                var dataToStore = ExecutingOperations.ToArray();
                ExecutingOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsRecentCompletedOperationsChanged || isInitialInsert)
            {
                var dataToStore = RecentCompletedOperations.ToArray();
                RecentCompletedOperationsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsChangeItemTrackingListChanged || isInitialInsert)
            {
                var dataToStore = ChangeItemTrackingList.ToArray();
                ChangeItemTrackingListData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "InformationChangeItem")]
	public class InformationChangeItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationChangeItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[StartTimeUTC] TEXT NOT NULL, 
[EndTimeUTC] TEXT NOT NULL, 
[ChangedObjectIDList] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		[Column]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
        [Column(Name = "ChangedObjectIDList")] public string ChangedObjectIDListData;

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
	public class OperationExecutionItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS OperationExecutionItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
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
		public string ID { get; set; }


		[Column]
		public string OperationName { get; set; }
		// private string _unmodified_OperationName;

		[Column]
		public string OperationDomain { get; set; }
		// private string _unmodified_OperationDomain;

		[Column]
		public string OperationID { get; set; }
		// private string _unmodified_OperationID;

		[Column]
		public string CallerProvidedInfo { get; set; }
		// private string _unmodified_CallerProvidedInfo;

		[Column]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;

		[Column]
		public DateTime ExecutionBeginTime { get; set; }
		// private DateTime _unmodified_ExecutionBeginTime;

		[Column]
		public DateTime ExecutionCompletedTime { get; set; }
		// private DateTime _unmodified_ExecutionCompletedTime;

		[Column]
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
    [Table(Name = "GenericCollectionableObject")]
	public class GenericCollectionableObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericCollectionableObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ValueObject] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public GenericObject ValueObject { get; set; }
		// private GenericObject _unmodified_ValueObject;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GenericObject")]
	public class GenericObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Values] TEXT NOT NULL, 
[IncludeInCollection] INTEGER NOT NULL, 
[OptionalCollectionName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "Values")] public string ValuesData;

        private bool _IsValuesRetrieved = false;
        private bool _IsValuesChanged = false;
        private ObservableCollection<GenericValue> _Values = null;
        public ObservableCollection<GenericValue> Values
        {
            get
            {
                if (!_IsValuesRetrieved)
                {
                    if (ValuesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<GenericValue[]>(ValuesData);
                        _Values = new ObservableCollection<GenericValue>(arrayData);
                    }
                    else
                    {
                        _Values = new ObservableCollection<GenericValue>();
						ValuesData = Guid.NewGuid().ToString();
						_IsValuesChanged = true;
                    }
                    _IsValuesRetrieved = true;
                    _Values.CollectionChanged += (sender, args) =>
						{
							ValuesData = Guid.NewGuid().ToString();
							_IsValuesChanged = true;
						};
                }
                return _Values;
            }
            set 
			{ 
				_Values = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsValuesRetrieved = true;
                ValuesData = Guid.NewGuid().ToString();
                _IsValuesChanged = true;

			}
        }


		[Column]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		[Column]
		public string OptionalCollectionName { get; set; }
		// private string _unmodified_OptionalCollectionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OptionalCollectionName == null)
				OptionalCollectionName = string.Empty;
            if (_IsValuesChanged || isInitialInsert)
            {
                var dataToStore = Values.ToArray();
                ValuesData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "GenericValue")]
	public class GenericValue : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GenericValue(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ValueName] TEXT NOT NULL, 
[String] TEXT NOT NULL, 
[StringArray] TEXT NOT NULL, 
[Number] REAL NOT NULL, 
[NumberArray] TEXT NOT NULL, 
[Boolean] INTEGER NOT NULL, 
[BooleanArray] TEXT NOT NULL, 
[DateTime] TEXT NOT NULL, 
[DateTimeArray] TEXT NOT NULL, 
[Object] TEXT NOT NULL, 
[ObjectArray] TEXT NOT NULL, 
[IndexingInfo] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		[Column]
		public string String { get; set; }
		// private string _unmodified_String;
        [Column(Name = "StringArray")] public string StringArrayData;

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
		public double Number { get; set; }
		// private double _unmodified_Number;
        [Column(Name = "NumberArray")] public string NumberArrayData;

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
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        [Column(Name = "BooleanArray")] public string BooleanArrayData;

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
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        [Column(Name = "DateTimeArray")] public string DateTimeArrayData;

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
		public GenericObject Object { get; set; }
		// private GenericObject _unmodified_Object;
        [Column(Name = "ObjectArray")] public string ObjectArrayData;

        private bool _IsObjectArrayRetrieved = false;
        private bool _IsObjectArrayChanged = false;
        private ObservableCollection<GenericObject> _ObjectArray = null;
        public ObservableCollection<GenericObject> ObjectArray
        {
            get
            {
                if (!_IsObjectArrayRetrieved)
                {
                    if (ObjectArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<GenericObject[]>(ObjectArrayData);
                        _ObjectArray = new ObservableCollection<GenericObject>(arrayData);
                    }
                    else
                    {
                        _ObjectArray = new ObservableCollection<GenericObject>();
						ObjectArrayData = Guid.NewGuid().ToString();
						_IsObjectArrayChanged = true;
                    }
                    _IsObjectArrayRetrieved = true;
                    _ObjectArray.CollectionChanged += (sender, args) =>
						{
							ObjectArrayData = Guid.NewGuid().ToString();
							_IsObjectArrayChanged = true;
						};
                }
                return _ObjectArray;
            }
            set 
			{ 
				_ObjectArray = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsObjectArrayRetrieved = true;
                ObjectArrayData = Guid.NewGuid().ToString();
                _IsObjectArrayChanged = true;

			}
        }


		[Column]
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

            if (_IsObjectArrayChanged || isInitialInsert)
            {
                var dataToStore = ObjectArray.ToArray();
                ObjectArrayData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
 } 
