 


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


namespace SQLite.TheBall.Interface { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DbContext, IStorageSyncableDataContext
		{
		    protected override void OnModelCreating(ModelBuilder modelBuilder)
		    {
				InterfaceOperation.EntityConfig(modelBuilder);
				Connection.EntityConfig(modelBuilder);
				TransferPackage.EntityConfig(modelBuilder);
				CategoryLink.EntityConfig(modelBuilder);
				Category.EntityConfig(modelBuilder);
				StatusSummary.EntityConfig(modelBuilder);
				InformationChangeItem.EntityConfig(modelBuilder);
				OperationExecutionItem.EntityConfig(modelBuilder);
				GenericCollectionableObject.EntityConfig(modelBuilder);
				GenericObject.EntityConfig(modelBuilder);
				GenericValue.EntityConfig(modelBuilder);
				ConnectionCollection.EntityConfig(modelBuilder);
				GenericObjectCollection.EntityConfig(modelBuilder);

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
				tableCreationCommands.Add(InterfaceOperation.GetCreateTableSQL());
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
				tableCreationCommands.Add(ConnectionCollection.GetCreateTableSQL());
				tableCreationCommands.Add(GenericObjectCollection.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "InterfaceOperation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.InterfaceOperation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InterfaceOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OperationName = serializedObject.OperationName;
		            existingObject.Status = serializedObject.Status;
		            existingObject.OperationDataType = serializedObject.OperationDataType;
		            existingObject.Created = serializedObject.Created;
		            existingObject.Started = serializedObject.Started;
		            existingObject.Progress = serializedObject.Progress;
		            existingObject.Finished = serializedObject.Finished;
		            existingObject.ErrorCode = serializedObject.ErrorCode;
		            existingObject.ErrorMessage = serializedObject.ErrorMessage;
		            break;
		        } 
		        case "Connection":
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
		            break;
		        } 
		        case "TransferPackage":
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
					
		            break;
		        } 
		        case "CategoryLink":
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
		            break;
		        } 
		        case "Category":
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
		            break;
		        } 
		        case "StatusSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StatusSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
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
					
		            break;
		        } 
		        case "InformationChangeItem":
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
					
		            break;
		        } 
		        case "OperationExecutionItem":
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
		            break;
		        } 
		        case "GenericCollectionableObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.GenericCollectionableObject.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericCollectionableObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ValueObject != null)
						existingObject.ValueObjectID = serializedObject.ValueObject.ID;
					else
						existingObject.ValueObjectID = null;
		            break;
		        } 
		        case "GenericObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GenericObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.Values.Clear();
					if(serializedObject.Values != null)
	                    serializedObject.Values.ForEach(item => existingObject.Values.Add(item));
					
		            existingObject.IncludeInCollection = serializedObject.IncludeInCollection;
		            existingObject.OptionalCollectionName = serializedObject.OptionalCollectionName;
		            break;
		        } 
		        case "GenericValue":
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
					
					if(serializedObject.Object != null)
						existingObject.ObjectID = serializedObject.Object.ID;
					else
						existingObject.ObjectID = null;
                    existingObject.ObjectArray.Clear();
					if(serializedObject.ObjectArray != null)
	                    serializedObject.ObjectArray.ForEach(item => existingObject.ObjectArray.Add(item));
					
		            existingObject.IndexingInfo = serializedObject.IndexingInfo;
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "InterfaceOperation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.InterfaceOperation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InterfaceOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OperationName = serializedObject.OperationName;
		            existingObject.Status = serializedObject.Status;
		            existingObject.OperationDataType = serializedObject.OperationDataType;
		            existingObject.Created = serializedObject.Created;
		            existingObject.Started = serializedObject.Started;
		            existingObject.Progress = serializedObject.Progress;
		            existingObject.Finished = serializedObject.Finished;
		            existingObject.ErrorCode = serializedObject.ErrorCode;
		            existingObject.ErrorMessage = serializedObject.ErrorMessage;
		            break;
		        } 
		        case "Connection":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.Connection.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ConnectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
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
		            break;
		        } 
		        case "TransferPackage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.TransferPackage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TransferPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ConnectionID = serializedObject.ConnectionID;
		            existingObject.PackageDirection = serializedObject.PackageDirection;
		            existingObject.PackageType = serializedObject.PackageType;
		            existingObject.IsProcessed = serializedObject.IsProcessed;
                    existingObject.PackageContentBlobs.Clear();
					if(serializedObject.PackageContentBlobs != null)
	                    serializedObject.PackageContentBlobs.ForEach(item => existingObject.PackageContentBlobs.Add(item));
					
		            break;
		        } 
		        case "CategoryLink":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.CategoryLink.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CategoryLinkTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceCategoryID = serializedObject.SourceCategoryID;
		            existingObject.TargetCategoryID = serializedObject.TargetCategoryID;
		            existingObject.LinkingType = serializedObject.LinkingType;
		            break;
		        } 
		        case "Category":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.Category.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.NativeCategoryID = serializedObject.NativeCategoryID;
		            existingObject.NativeCategoryDomainName = serializedObject.NativeCategoryDomainName;
		            existingObject.NativeCategoryObjectName = serializedObject.NativeCategoryObjectName;
		            existingObject.NativeCategoryTitle = serializedObject.NativeCategoryTitle;
		            existingObject.IdentifyingCategoryName = serializedObject.IdentifyingCategoryName;
		            break;
		        } 
		        case "StatusSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = StatusSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
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
					
		            break;
		        } 
		        case "InformationChangeItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.InformationChangeItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InformationChangeItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StartTimeUTC = serializedObject.StartTimeUTC;
		            existingObject.EndTimeUTC = serializedObject.EndTimeUTC;
                    existingObject.ChangedObjectIDList.Clear();
					if(serializedObject.ChangedObjectIDList != null)
	                    serializedObject.ChangedObjectIDList.ForEach(item => existingObject.ChangedObjectIDList.Add(item));
					
		            break;
		        } 
		        case "OperationExecutionItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.OperationExecutionItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
		            break;
		        } 
		        case "GenericCollectionableObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.GenericCollectionableObject.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GenericCollectionableObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ValueObject != null)
						existingObject.ValueObjectID = serializedObject.ValueObject.ID;
					else
						existingObject.ValueObjectID = null;
		            break;
		        } 
		        case "GenericObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GenericObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.Values.Clear();
					if(serializedObject.Values != null)
	                    serializedObject.Values.ForEach(item => existingObject.Values.Add(item));
					
		            existingObject.IncludeInCollection = serializedObject.IncludeInCollection;
		            existingObject.OptionalCollectionName = serializedObject.OptionalCollectionName;
		            break;
		        } 
		        case "GenericValue":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.Interface.GenericValue.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					
					if(serializedObject.Object != null)
						existingObject.ObjectID = serializedObject.Object.ID;
					else
						existingObject.ObjectID = null;
                    existingObject.ObjectArray.Clear();
					if(serializedObject.ObjectArray != null)
	                    serializedObject.ObjectArray.ForEach(item => existingObject.ObjectArray.Add(item));
					
		            existingObject.IndexingInfo = serializedObject.IndexingInfo;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "InterfaceOperation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.InterfaceOperation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InterfaceOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OperationName = serializedObject.OperationName;
		            objectToAdd.Status = serializedObject.Status;
		            objectToAdd.OperationDataType = serializedObject.OperationDataType;
		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.Started = serializedObject.Started;
		            objectToAdd.Progress = serializedObject.Progress;
		            objectToAdd.Finished = serializedObject.Finished;
		            objectToAdd.ErrorCode = serializedObject.ErrorCode;
		            objectToAdd.ErrorMessage = serializedObject.ErrorMessage;
					InterfaceOperationTable.Add(objectToAdd);
                    break;
                }
                case "Connection":
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
					ConnectionTable.Add(objectToAdd);
                    break;
                }
                case "TransferPackage":
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
					TransferPackageTable.Add(objectToAdd);
                    break;
                }
                case "CategoryLink":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.CategoryLink.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CategoryLink {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceCategoryID = serializedObject.SourceCategoryID;
		            objectToAdd.TargetCategoryID = serializedObject.TargetCategoryID;
		            objectToAdd.LinkingType = serializedObject.LinkingType;
					CategoryLinkTable.Add(objectToAdd);
                    break;
                }
                case "Category":
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
					CategoryTable.Add(objectToAdd);
                    break;
                }
                case "StatusSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StatusSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.PendingOperations != null)
						serializedObject.PendingOperations.ForEach(item => objectToAdd.PendingOperations.Add(item));
					if(serializedObject.ExecutingOperations != null)
						serializedObject.ExecutingOperations.ForEach(item => objectToAdd.ExecutingOperations.Add(item));
					if(serializedObject.RecentCompletedOperations != null)
						serializedObject.RecentCompletedOperations.ForEach(item => objectToAdd.RecentCompletedOperations.Add(item));
					if(serializedObject.ChangeItemTrackingList != null)
						serializedObject.ChangeItemTrackingList.ForEach(item => objectToAdd.ChangeItemTrackingList.Add(item));
					StatusSummaryTable.Add(objectToAdd);
                    break;
                }
                case "InformationChangeItem":
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
					InformationChangeItemTable.Add(objectToAdd);
                    break;
                }
                case "OperationExecutionItem":
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
					OperationExecutionItemTable.Add(objectToAdd);
                    break;
                }
                case "GenericCollectionableObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericCollectionableObject.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericCollectionableObject {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ValueObject != null)
						objectToAdd.ValueObjectID = serializedObject.ValueObject.ID;
					else
						objectToAdd.ValueObjectID = null;
					GenericCollectionableObjectTable.Add(objectToAdd);
                    break;
                }
                case "GenericObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GenericObject {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Values != null)
						serializedObject.Values.ForEach(item => objectToAdd.Values.Add(item));
		            objectToAdd.IncludeInCollection = serializedObject.IncludeInCollection;
		            objectToAdd.OptionalCollectionName = serializedObject.OptionalCollectionName;
					GenericObjectTable.Add(objectToAdd);
                    break;
                }
                case "GenericValue":
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
					if(serializedObject.Object != null)
						objectToAdd.ObjectID = serializedObject.Object.ID;
					else
						objectToAdd.ObjectID = null;
					if(serializedObject.ObjectArray != null)
						serializedObject.ObjectArray.ForEach(item => objectToAdd.ObjectArray.Add(item));
		            objectToAdd.IndexingInfo = serializedObject.IndexingInfo;
					GenericValueTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.Add(insertData);

				switch(insertData.ObjectType)
				{
                case "InterfaceOperation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.InterfaceOperation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InterfaceOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OperationName = serializedObject.OperationName;
		            objectToAdd.Status = serializedObject.Status;
		            objectToAdd.OperationDataType = serializedObject.OperationDataType;
		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.Started = serializedObject.Started;
		            objectToAdd.Progress = serializedObject.Progress;
		            objectToAdd.Finished = serializedObject.Finished;
		            objectToAdd.ErrorCode = serializedObject.ErrorCode;
		            objectToAdd.ErrorMessage = serializedObject.ErrorMessage;
					InterfaceOperationTable.Add(objectToAdd);
                    break;
                }
                case "Connection":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.Connection.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Connection {ID = insertData.ObjectID, ETag = insertData.ETag};
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
					ConnectionTable.Add(objectToAdd);
                    break;
                }
                case "TransferPackage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.TransferPackage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TransferPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ConnectionID = serializedObject.ConnectionID;
		            objectToAdd.PackageDirection = serializedObject.PackageDirection;
		            objectToAdd.PackageType = serializedObject.PackageType;
		            objectToAdd.IsProcessed = serializedObject.IsProcessed;
					if(serializedObject.PackageContentBlobs != null)
						serializedObject.PackageContentBlobs.ForEach(item => objectToAdd.PackageContentBlobs.Add(item));
					TransferPackageTable.Add(objectToAdd);
                    break;
                }
                case "CategoryLink":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.CategoryLink.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new CategoryLink {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceCategoryID = serializedObject.SourceCategoryID;
		            objectToAdd.TargetCategoryID = serializedObject.TargetCategoryID;
		            objectToAdd.LinkingType = serializedObject.LinkingType;
					CategoryLinkTable.Add(objectToAdd);
                    break;
                }
                case "Category":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.Category.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.NativeCategoryID = serializedObject.NativeCategoryID;
		            objectToAdd.NativeCategoryDomainName = serializedObject.NativeCategoryDomainName;
		            objectToAdd.NativeCategoryObjectName = serializedObject.NativeCategoryObjectName;
		            objectToAdd.NativeCategoryTitle = serializedObject.NativeCategoryTitle;
		            objectToAdd.IdentifyingCategoryName = serializedObject.IdentifyingCategoryName;
					CategoryTable.Add(objectToAdd);
                    break;
                }
                case "StatusSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.StatusSummary.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new StatusSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.PendingOperations != null)
						serializedObject.PendingOperations.ForEach(item => objectToAdd.PendingOperations.Add(item));
					if(serializedObject.ExecutingOperations != null)
						serializedObject.ExecutingOperations.ForEach(item => objectToAdd.ExecutingOperations.Add(item));
					if(serializedObject.RecentCompletedOperations != null)
						serializedObject.RecentCompletedOperations.ForEach(item => objectToAdd.RecentCompletedOperations.Add(item));
					if(serializedObject.ChangeItemTrackingList != null)
						serializedObject.ChangeItemTrackingList.ForEach(item => objectToAdd.ChangeItemTrackingList.Add(item));
					StatusSummaryTable.Add(objectToAdd);
                    break;
                }
                case "InformationChangeItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.InformationChangeItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InformationChangeItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StartTimeUTC = serializedObject.StartTimeUTC;
		            objectToAdd.EndTimeUTC = serializedObject.EndTimeUTC;
					if(serializedObject.ChangedObjectIDList != null)
						serializedObject.ChangedObjectIDList.ForEach(item => objectToAdd.ChangedObjectIDList.Add(item));
					InformationChangeItemTable.Add(objectToAdd);
                    break;
                }
                case "OperationExecutionItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.OperationExecutionItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new OperationExecutionItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OperationName = serializedObject.OperationName;
		            objectToAdd.OperationDomain = serializedObject.OperationDomain;
		            objectToAdd.OperationID = serializedObject.OperationID;
		            objectToAdd.CallerProvidedInfo = serializedObject.CallerProvidedInfo;
		            objectToAdd.CreationTime = serializedObject.CreationTime;
		            objectToAdd.ExecutionBeginTime = serializedObject.ExecutionBeginTime;
		            objectToAdd.ExecutionCompletedTime = serializedObject.ExecutionCompletedTime;
		            objectToAdd.ExecutionStatus = serializedObject.ExecutionStatus;
					OperationExecutionItemTable.Add(objectToAdd);
                    break;
                }
                case "GenericCollectionableObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericCollectionableObject.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GenericCollectionableObject {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ValueObject != null)
						objectToAdd.ValueObjectID = serializedObject.ValueObject.ID;
					else
						objectToAdd.ValueObjectID = null;
					GenericCollectionableObjectTable.Add(objectToAdd);
                    break;
                }
                case "GenericObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericObject.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GenericObject {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Values != null)
						serializedObject.Values.ForEach(item => objectToAdd.Values.Add(item));
		            objectToAdd.IncludeInCollection = serializedObject.IncludeInCollection;
		            objectToAdd.OptionalCollectionName = serializedObject.OptionalCollectionName;
					GenericObjectTable.Add(objectToAdd);
                    break;
                }
                case "GenericValue":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.Interface.GenericValue.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					if(serializedObject.Object != null)
						objectToAdd.ObjectID = serializedObject.Object.ID;
					else
						objectToAdd.ObjectID = null;
					if(serializedObject.ObjectArray != null)
						serializedObject.ObjectArray.ForEach(item => objectToAdd.ObjectArray.Add(item));
		            objectToAdd.IndexingInfo = serializedObject.IndexingInfo;
					GenericValueTable.Add(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "InterfaceOperation":
					{
						//var objectToDelete = new InterfaceOperation {ID = deleteData.ObjectID};
						//InterfaceOperationTable.Attach(objectToDelete);
						var objectToDelete = InterfaceOperationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InterfaceOperationTable.Remove(objectToDelete);
						break;
					}
					case "Connection":
					{
						//var objectToDelete = new Connection {ID = deleteData.ObjectID};
						//ConnectionTable.Attach(objectToDelete);
						var objectToDelete = ConnectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ConnectionTable.Remove(objectToDelete);
						break;
					}
					case "TransferPackage":
					{
						//var objectToDelete = new TransferPackage {ID = deleteData.ObjectID};
						//TransferPackageTable.Attach(objectToDelete);
						var objectToDelete = TransferPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TransferPackageTable.Remove(objectToDelete);
						break;
					}
					case "CategoryLink":
					{
						//var objectToDelete = new CategoryLink {ID = deleteData.ObjectID};
						//CategoryLinkTable.Attach(objectToDelete);
						var objectToDelete = CategoryLinkTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryLinkTable.Remove(objectToDelete);
						break;
					}
					case "Category":
					{
						//var objectToDelete = new Category {ID = deleteData.ObjectID};
						//CategoryTable.Attach(objectToDelete);
						var objectToDelete = CategoryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryTable.Remove(objectToDelete);
						break;
					}
					case "StatusSummary":
					{
						//var objectToDelete = new StatusSummary {ID = deleteData.ObjectID};
						//StatusSummaryTable.Attach(objectToDelete);
						var objectToDelete = StatusSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StatusSummaryTable.Remove(objectToDelete);
						break;
					}
					case "InformationChangeItem":
					{
						//var objectToDelete = new InformationChangeItem {ID = deleteData.ObjectID};
						//InformationChangeItemTable.Attach(objectToDelete);
						var objectToDelete = InformationChangeItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationChangeItemTable.Remove(objectToDelete);
						break;
					}
					case "OperationExecutionItem":
					{
						//var objectToDelete = new OperationExecutionItem {ID = deleteData.ObjectID};
						//OperationExecutionItemTable.Attach(objectToDelete);
						var objectToDelete = OperationExecutionItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							OperationExecutionItemTable.Remove(objectToDelete);
						break;
					}
					case "GenericCollectionableObject":
					{
						//var objectToDelete = new GenericCollectionableObject {ID = deleteData.ObjectID};
						//GenericCollectionableObjectTable.Attach(objectToDelete);
						var objectToDelete = GenericCollectionableObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericCollectionableObjectTable.Remove(objectToDelete);
						break;
					}
					case "GenericObject":
					{
						//var objectToDelete = new GenericObject {ID = deleteData.ObjectID};
						//GenericObjectTable.Attach(objectToDelete);
						var objectToDelete = GenericObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericObjectTable.Remove(objectToDelete);
						break;
					}
					case "GenericValue":
					{
						//var objectToDelete = new GenericValue {ID = deleteData.ObjectID};
						//GenericValueTable.Attach(objectToDelete);
						var objectToDelete = GenericValueTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericValueTable.Remove(objectToDelete);
						break;
					}
					case "ConnectionCollection":
					{
						//var objectToDelete = new ConnectionCollection {ID = deleteData.ObjectID};
						//ConnectionCollectionTable.Attach(objectToDelete);
						var objectToDelete = ConnectionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ConnectionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "GenericObjectCollection":
					{
						//var objectToDelete = new GenericObjectCollection {ID = deleteData.ObjectID};
						//GenericObjectCollectionTable.Attach(objectToDelete);
						var objectToDelete = GenericObjectCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericObjectCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.Interface")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.Remove(deleteData);

				switch(deleteData.ObjectType)
				{
					case "InterfaceOperation":
					{
						//var objectToDelete = new InterfaceOperation {ID = deleteData.ObjectID};
						//InterfaceOperationTable.Attach(objectToDelete);
						var objectToDelete = InterfaceOperationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InterfaceOperationTable.Remove(objectToDelete);
						break;
					}
					case "Connection":
					{
						//var objectToDelete = new Connection {ID = deleteData.ObjectID};
						//ConnectionTable.Attach(objectToDelete);
						var objectToDelete = ConnectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ConnectionTable.Remove(objectToDelete);
						break;
					}
					case "TransferPackage":
					{
						//var objectToDelete = new TransferPackage {ID = deleteData.ObjectID};
						//TransferPackageTable.Attach(objectToDelete);
						var objectToDelete = TransferPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TransferPackageTable.Remove(objectToDelete);
						break;
					}
					case "CategoryLink":
					{
						//var objectToDelete = new CategoryLink {ID = deleteData.ObjectID};
						//CategoryLinkTable.Attach(objectToDelete);
						var objectToDelete = CategoryLinkTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryLinkTable.Remove(objectToDelete);
						break;
					}
					case "Category":
					{
						//var objectToDelete = new Category {ID = deleteData.ObjectID};
						//CategoryTable.Attach(objectToDelete);
						var objectToDelete = CategoryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryTable.Remove(objectToDelete);
						break;
					}
					case "StatusSummary":
					{
						//var objectToDelete = new StatusSummary {ID = deleteData.ObjectID};
						//StatusSummaryTable.Attach(objectToDelete);
						var objectToDelete = StatusSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StatusSummaryTable.Remove(objectToDelete);
						break;
					}
					case "InformationChangeItem":
					{
						//var objectToDelete = new InformationChangeItem {ID = deleteData.ObjectID};
						//InformationChangeItemTable.Attach(objectToDelete);
						var objectToDelete = InformationChangeItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationChangeItemTable.Remove(objectToDelete);
						break;
					}
					case "OperationExecutionItem":
					{
						//var objectToDelete = new OperationExecutionItem {ID = deleteData.ObjectID};
						//OperationExecutionItemTable.Attach(objectToDelete);
						var objectToDelete = OperationExecutionItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							OperationExecutionItemTable.Remove(objectToDelete);
						break;
					}
					case "GenericCollectionableObject":
					{
						//var objectToDelete = new GenericCollectionableObject {ID = deleteData.ObjectID};
						//GenericCollectionableObjectTable.Attach(objectToDelete);
						var objectToDelete = GenericCollectionableObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericCollectionableObjectTable.Remove(objectToDelete);
						break;
					}
					case "GenericObject":
					{
						//var objectToDelete = new GenericObject {ID = deleteData.ObjectID};
						//GenericObjectTable.Attach(objectToDelete);
						var objectToDelete = GenericObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericObjectTable.Remove(objectToDelete);
						break;
					}
					case "GenericValue":
					{
						//var objectToDelete = new GenericValue {ID = deleteData.ObjectID};
						//GenericValueTable.Attach(objectToDelete);
						var objectToDelete = GenericValueTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericValueTable.Remove(objectToDelete);
						break;
					}
					case "ConnectionCollection":
					{
						//var objectToDelete = new ConnectionCollection {ID = deleteData.ObjectID};
						//ConnectionCollectionTable.Attach(objectToDelete);
						var objectToDelete = ConnectionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ConnectionCollectionTable.Remove(objectToDelete);
						break;
					}
					case "GenericObjectCollection":
					{
						//var objectToDelete = new GenericObjectCollection {ID = deleteData.ObjectID};
						//GenericObjectCollectionTable.Attach(objectToDelete);
						var objectToDelete = GenericObjectCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GenericObjectCollectionTable.Remove(objectToDelete);
						break;
					}
				}
			}



			public DbSet<InterfaceOperation> InterfaceOperationTable { get; set; }
			public DbSet<Connection> ConnectionTable { get; set; }
			public DbSet<TransferPackage> TransferPackageTable { get; set; }
			public DbSet<CategoryLink> CategoryLinkTable { get; set; }
			public DbSet<Category> CategoryTable { get; set; }
			public DbSet<StatusSummary> StatusSummaryTable { get; set; }
			public DbSet<InformationChangeItem> InformationChangeItemTable { get; set; }
			public DbSet<OperationExecutionItem> OperationExecutionItemTable { get; set; }
			public DbSet<GenericCollectionableObject> GenericCollectionableObjectTable { get; set; }
			public DbSet<GenericObject> GenericObjectTable { get; set; }
			public DbSet<GenericValue> GenericValueTable { get; set; }
			public DbSet<ConnectionCollection> ConnectionCollectionTable { get; set; }
			public DbSet<GenericObjectCollection> GenericObjectCollectionTable { get; set; }
        }

    [Table("InterfaceOperation")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("InterfaceOperation: {ID}")]
	public class InterfaceOperation : ITheBallDataContextStorable
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


		public InterfaceOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InterfaceOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OperationName] TEXT DEFAULT '', 
[Status] TEXT DEFAULT '', 
[OperationDataType] TEXT DEFAULT '', 
[Created] TEXT DEFAULT '', 
[Started] TEXT DEFAULT '', 
[Progress] REAL NOT NULL, 
[Finished] TEXT DEFAULT '', 
[ErrorCode] TEXT DEFAULT '', 
[ErrorMessage] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationName { get; set; }
		// private string _unmodified_OperationName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Status { get; set; }
		// private string _unmodified_Status;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationDataType { get; set; }
		// private string _unmodified_OperationDataType;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime Created { get; set; }
		// private DateTime _unmodified_Created;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime Started { get; set; }
		// private DateTime _unmodified_Started;

		//[Column]
        //[ScaffoldColumn(true)]
		public double Progress { get; set; }
		// private double _unmodified_Progress;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime Finished { get; set; }
		// private DateTime _unmodified_Finished;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ErrorCode { get; set; }
		// private string _unmodified_ErrorCode;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ErrorMessage { get; set; }
		// private string _unmodified_ErrorMessage;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OperationName == null)
				OperationName = string.Empty;
			if(Status == null)
				Status = string.Empty;
			if(OperationDataType == null)
				OperationDataType = string.Empty;
			if(ErrorCode == null)
				ErrorCode = string.Empty;
			if(ErrorMessage == null)
				ErrorMessage = string.Empty;
		}
	}
    [Table("Connection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Connection: {ID}")]
	public class Connection : ITheBallDataContextStorable
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


		public Connection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Connection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OutputInformationID] TEXT DEFAULT '', 
[Description] TEXT DEFAULT '', 
[DeviceID] TEXT DEFAULT '', 
[IsActiveParty] INTEGER NOT NULL, 
[OtherSideConnectionID] TEXT DEFAULT '', 
[ThisSideCategoriesID] TEXT DEFAULT '', 
[OtherSideCategoriesID] TEXT DEFAULT '', 
[CategoryLinksID] TEXT DEFAULT '', 
[IncomingPackagesID] TEXT DEFAULT '', 
[OutgoingPackagesID] TEXT DEFAULT '', 
[OperationNameToListPackageContents] TEXT DEFAULT '', 
[OperationNameToProcessReceived] TEXT DEFAULT '', 
[OperationNameToUpdateThisSideCategories] TEXT DEFAULT '', 
[ProcessIDToListPackageContents] TEXT DEFAULT '', 
[ProcessIDToProcessReceived] TEXT DEFAULT '', 
[ProcessIDToUpdateThisSideCategories] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string OutputInformationID { get; set; }
		// private string _unmodified_OutputInformationID;

		//[Column]
        //[ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		//[Column]
        //[ScaffoldColumn(true)]
		public string DeviceID { get; set; }
		// private string _unmodified_DeviceID;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool IsActiveParty { get; set; }
		// private bool _unmodified_IsActiveParty;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OtherSideConnectionID { get; set; }
		// private string _unmodified_OtherSideConnectionID;
        //[Column(Name = "ThisSideCategories")] 
        //[ScaffoldColumn(true)]
		public string ThisSideCategoriesData { get; set; }

        private bool _IsThisSideCategoriesRetrieved = false;
        private bool _IsThisSideCategoriesChanged = false;
        private ObservableCollection<SER.TheBall.Interface.Category> _ThisSideCategories = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.Category> ThisSideCategories
        {
            get
            {
                if (!_IsThisSideCategoriesRetrieved)
                {
                    if (ThisSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.Category[]>(ThisSideCategoriesData);
                        _ThisSideCategories = new ObservableCollection<SER.TheBall.Interface.Category>(arrayData);
                    }
                    else
                    {
                        _ThisSideCategories = new ObservableCollection<SER.TheBall.Interface.Category>();
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

        //[Column(Name = "OtherSideCategories")] 
        //[ScaffoldColumn(true)]
		public string OtherSideCategoriesData { get; set; }

        private bool _IsOtherSideCategoriesRetrieved = false;
        private bool _IsOtherSideCategoriesChanged = false;
        private ObservableCollection<SER.TheBall.Interface.Category> _OtherSideCategories = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.Category> OtherSideCategories
        {
            get
            {
                if (!_IsOtherSideCategoriesRetrieved)
                {
                    if (OtherSideCategoriesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.Category[]>(OtherSideCategoriesData);
                        _OtherSideCategories = new ObservableCollection<SER.TheBall.Interface.Category>(arrayData);
                    }
                    else
                    {
                        _OtherSideCategories = new ObservableCollection<SER.TheBall.Interface.Category>();
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

        //[Column(Name = "CategoryLinks")] 
        //[ScaffoldColumn(true)]
		public string CategoryLinksData { get; set; }

        private bool _IsCategoryLinksRetrieved = false;
        private bool _IsCategoryLinksChanged = false;
        private ObservableCollection<SER.TheBall.Interface.CategoryLink> _CategoryLinks = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.CategoryLink> CategoryLinks
        {
            get
            {
                if (!_IsCategoryLinksRetrieved)
                {
                    if (CategoryLinksData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.CategoryLink[]>(CategoryLinksData);
                        _CategoryLinks = new ObservableCollection<SER.TheBall.Interface.CategoryLink>(arrayData);
                    }
                    else
                    {
                        _CategoryLinks = new ObservableCollection<SER.TheBall.Interface.CategoryLink>();
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

        //[Column(Name = "IncomingPackages")] 
        //[ScaffoldColumn(true)]
		public string IncomingPackagesData { get; set; }

        private bool _IsIncomingPackagesRetrieved = false;
        private bool _IsIncomingPackagesChanged = false;
        private ObservableCollection<SER.TheBall.Interface.TransferPackage> _IncomingPackages = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.TransferPackage> IncomingPackages
        {
            get
            {
                if (!_IsIncomingPackagesRetrieved)
                {
                    if (IncomingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.TransferPackage[]>(IncomingPackagesData);
                        _IncomingPackages = new ObservableCollection<SER.TheBall.Interface.TransferPackage>(arrayData);
                    }
                    else
                    {
                        _IncomingPackages = new ObservableCollection<SER.TheBall.Interface.TransferPackage>();
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

        //[Column(Name = "OutgoingPackages")] 
        //[ScaffoldColumn(true)]
		public string OutgoingPackagesData { get; set; }

        private bool _IsOutgoingPackagesRetrieved = false;
        private bool _IsOutgoingPackagesChanged = false;
        private ObservableCollection<SER.TheBall.Interface.TransferPackage> _OutgoingPackages = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.TransferPackage> OutgoingPackages
        {
            get
            {
                if (!_IsOutgoingPackagesRetrieved)
                {
                    if (OutgoingPackagesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.TransferPackage[]>(OutgoingPackagesData);
                        _OutgoingPackages = new ObservableCollection<SER.TheBall.Interface.TransferPackage>(arrayData);
                    }
                    else
                    {
                        _OutgoingPackages = new ObservableCollection<SER.TheBall.Interface.TransferPackage>();
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


		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationNameToListPackageContents { get; set; }
		// private string _unmodified_OperationNameToListPackageContents;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationNameToProcessReceived { get; set; }
		// private string _unmodified_OperationNameToProcessReceived;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationNameToUpdateThisSideCategories { get; set; }
		// private string _unmodified_OperationNameToUpdateThisSideCategories;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ProcessIDToListPackageContents { get; set; }
		// private string _unmodified_ProcessIDToListPackageContents;

		//[Column]
        //[ScaffoldColumn(true)]
		public string ProcessIDToProcessReceived { get; set; }
		// private string _unmodified_ProcessIDToProcessReceived;

		//[Column]
        //[ScaffoldColumn(true)]
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
    [Table("TransferPackage")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("TransferPackage: {ID}")]
	public class TransferPackage : ITheBallDataContextStorable
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


		public TransferPackage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TransferPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ConnectionID] TEXT DEFAULT '', 
[PackageDirection] TEXT DEFAULT '', 
[PackageType] TEXT DEFAULT '', 
[IsProcessed] INTEGER NOT NULL, 
[PackageContentBlobs] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string ConnectionID { get; set; }
		// private string _unmodified_ConnectionID;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PackageDirection { get; set; }
		// private string _unmodified_PackageDirection;

		//[Column]
        //[ScaffoldColumn(true)]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		//[Column]
        //[ScaffoldColumn(true)]
		public bool IsProcessed { get; set; }
		// private bool _unmodified_IsProcessed;
        //[Column(Name = "PackageContentBlobs")] 
        //[ScaffoldColumn(true)]
		public string PackageContentBlobsData { get; set; }

        private bool _IsPackageContentBlobsRetrieved = false;
        private bool _IsPackageContentBlobsChanged = false;
        private ObservableCollection<string> _PackageContentBlobs = null;
        [NotMapped]
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
    [Table("CategoryLink")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("CategoryLink: {ID}")]
	public class CategoryLink : ITheBallDataContextStorable
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


		public CategoryLink() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CategoryLink](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SourceCategoryID] TEXT DEFAULT '', 
[TargetCategoryID] TEXT DEFAULT '', 
[LinkingType] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string SourceCategoryID { get; set; }
		// private string _unmodified_SourceCategoryID;

		//[Column]
        //[ScaffoldColumn(true)]
		public string TargetCategoryID { get; set; }
		// private string _unmodified_TargetCategoryID;

		//[Column]
        //[ScaffoldColumn(true)]
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
    [Table("Category")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("Category: {ID}")]
	public class Category : ITheBallDataContextStorable
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


		public Category() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Category](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[NativeCategoryID] TEXT DEFAULT '', 
[NativeCategoryDomainName] TEXT DEFAULT '', 
[NativeCategoryObjectName] TEXT DEFAULT '', 
[NativeCategoryTitle] TEXT DEFAULT '', 
[IdentifyingCategoryName] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string NativeCategoryID { get; set; }
		// private string _unmodified_NativeCategoryID;

		//[Column]
        //[ScaffoldColumn(true)]
		public string NativeCategoryDomainName { get; set; }
		// private string _unmodified_NativeCategoryDomainName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string NativeCategoryObjectName { get; set; }
		// private string _unmodified_NativeCategoryObjectName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string NativeCategoryTitle { get; set; }
		// private string _unmodified_NativeCategoryTitle;

		//[Column]
        //[ScaffoldColumn(true)]
		public string IdentifyingCategoryName { get; set; }
		// private string _unmodified_IdentifyingCategoryName;
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
		}
	}
    [Table("StatusSummary")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("StatusSummary: {ID}")]
	public class StatusSummary : ITheBallDataContextStorable
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


		public StatusSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StatusSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PendingOperationsID] TEXT DEFAULT '', 
[ExecutingOperationsID] TEXT DEFAULT '', 
[RecentCompletedOperationsID] TEXT DEFAULT '', 
[ChangeItemTrackingList] TEXT DEFAULT ''
)";
        }

        //[Column(Name = "PendingOperations")] 
        //[ScaffoldColumn(true)]
		public string PendingOperationsData { get; set; }

        private bool _IsPendingOperationsRetrieved = false;
        private bool _IsPendingOperationsChanged = false;
        private ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> _PendingOperations = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> PendingOperations
        {
            get
            {
                if (!_IsPendingOperationsRetrieved)
                {
                    if (PendingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.OperationExecutionItem[]>(PendingOperationsData);
                        _PendingOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _PendingOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>();
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

        //[Column(Name = "ExecutingOperations")] 
        //[ScaffoldColumn(true)]
		public string ExecutingOperationsData { get; set; }

        private bool _IsExecutingOperationsRetrieved = false;
        private bool _IsExecutingOperationsChanged = false;
        private ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> _ExecutingOperations = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> ExecutingOperations
        {
            get
            {
                if (!_IsExecutingOperationsRetrieved)
                {
                    if (ExecutingOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.OperationExecutionItem[]>(ExecutingOperationsData);
                        _ExecutingOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _ExecutingOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>();
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

        //[Column(Name = "RecentCompletedOperations")] 
        //[ScaffoldColumn(true)]
		public string RecentCompletedOperationsData { get; set; }

        private bool _IsRecentCompletedOperationsRetrieved = false;
        private bool _IsRecentCompletedOperationsChanged = false;
        private ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> _RecentCompletedOperations = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.OperationExecutionItem> RecentCompletedOperations
        {
            get
            {
                if (!_IsRecentCompletedOperationsRetrieved)
                {
                    if (RecentCompletedOperationsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.OperationExecutionItem[]>(RecentCompletedOperationsData);
                        _RecentCompletedOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>(arrayData);
                    }
                    else
                    {
                        _RecentCompletedOperations = new ObservableCollection<SER.TheBall.Interface.OperationExecutionItem>();
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

        //[Column(Name = "ChangeItemTrackingList")] 
        //[ScaffoldColumn(true)]
		public string ChangeItemTrackingListData { get; set; }

        private bool _IsChangeItemTrackingListRetrieved = false;
        private bool _IsChangeItemTrackingListChanged = false;
        private ObservableCollection<string> _ChangeItemTrackingList = null;
        [NotMapped]
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
    [Table("InformationChangeItem")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("InformationChangeItem: {ID}")]
	public class InformationChangeItem : ITheBallDataContextStorable
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


		public InformationChangeItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationChangeItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[StartTimeUTC] TEXT DEFAULT '', 
[EndTimeUTC] TEXT DEFAULT '', 
[ChangedObjectIDList] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime StartTimeUTC { get; set; }
		// private DateTime _unmodified_StartTimeUTC;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime EndTimeUTC { get; set; }
		// private DateTime _unmodified_EndTimeUTC;
        //[Column(Name = "ChangedObjectIDList")] 
        //[ScaffoldColumn(true)]
		public string ChangedObjectIDListData { get; set; }

        private bool _IsChangedObjectIDListRetrieved = false;
        private bool _IsChangedObjectIDListChanged = false;
        private ObservableCollection<string> _ChangedObjectIDList = null;
        [NotMapped]
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
    [Table("OperationExecutionItem")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("OperationExecutionItem: {ID}")]
	public class OperationExecutionItem : ITheBallDataContextStorable
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


		public OperationExecutionItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OperationExecutionItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OperationName] TEXT DEFAULT '', 
[OperationDomain] TEXT DEFAULT '', 
[OperationID] TEXT DEFAULT '', 
[CallerProvidedInfo] TEXT DEFAULT '', 
[CreationTime] TEXT DEFAULT '', 
[ExecutionBeginTime] TEXT DEFAULT '', 
[ExecutionCompletedTime] TEXT DEFAULT '', 
[ExecutionStatus] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationName { get; set; }
		// private string _unmodified_OperationName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationDomain { get; set; }
		// private string _unmodified_OperationDomain;

		//[Column]
        //[ScaffoldColumn(true)]
		public string OperationID { get; set; }
		// private string _unmodified_OperationID;

		//[Column]
        //[ScaffoldColumn(true)]
		public string CallerProvidedInfo { get; set; }
		// private string _unmodified_CallerProvidedInfo;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime ExecutionBeginTime { get; set; }
		// private DateTime _unmodified_ExecutionBeginTime;

		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime ExecutionCompletedTime { get; set; }
		// private DateTime _unmodified_ExecutionCompletedTime;

		//[Column]
        //[ScaffoldColumn(true)]
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
    [Table("GenericCollectionableObject")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GenericCollectionableObject: {ID}")]
	public class GenericCollectionableObject : ITheBallDataContextStorable
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


		public GenericCollectionableObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericCollectionableObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ValueObjectID] TEXT DEFAULT ''
)";
        }

			//[Column]
			public string ValueObjectID { get; set; }
			private EntityRef< GenericObject > _ValueObject;
			[Association(Storage = "_ValueObject", ThisKey = "ValueObjectID")]
			public GenericObject ValueObject
			{
				get { return this._ValueObject.Entity; }
				set { this._ValueObject.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table("GenericObject")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GenericObject: {ID}")]
	public class GenericObject : ITheBallDataContextStorable
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


		public GenericObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ValuesID] TEXT DEFAULT '', 
[IncludeInCollection] INTEGER NOT NULL, 
[OptionalCollectionName] TEXT DEFAULT ''
)";
        }

        //[Column(Name = "Values")] 
        //[ScaffoldColumn(true)]
		public string ValuesData { get; set; }

        private bool _IsValuesRetrieved = false;
        private bool _IsValuesChanged = false;
        private ObservableCollection<SER.TheBall.Interface.GenericValue> _Values = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.GenericValue> Values
        {
            get
            {
                if (!_IsValuesRetrieved)
                {
                    if (ValuesData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.GenericValue[]>(ValuesData);
                        _Values = new ObservableCollection<SER.TheBall.Interface.GenericValue>(arrayData);
                    }
                    else
                    {
                        _Values = new ObservableCollection<SER.TheBall.Interface.GenericValue>();
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


		//[Column]
        //[ScaffoldColumn(true)]
		public bool IncludeInCollection { get; set; }
		// private bool _unmodified_IncludeInCollection;

		//[Column]
        //[ScaffoldColumn(true)]
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
    [Table("GenericValue")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GenericValue: {ID}")]
	public class GenericValue : ITheBallDataContextStorable
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


		public GenericValue() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericValue](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ValueName] TEXT DEFAULT '', 
[String] TEXT DEFAULT '', 
[StringArray] TEXT DEFAULT '', 
[Number] REAL NOT NULL, 
[NumberArray] TEXT DEFAULT '', 
[Boolean] INTEGER NOT NULL, 
[BooleanArray] TEXT DEFAULT '', 
[DateTime] TEXT DEFAULT '', 
[DateTimeArray] TEXT DEFAULT '', 
[ObjectID] TEXT DEFAULT '', 
[ObjectArrayID] TEXT DEFAULT '', 
[IndexingInfo] TEXT DEFAULT ''
)";
        }


		//[Column]
        //[ScaffoldColumn(true)]
		public string ValueName { get; set; }
		// private string _unmodified_ValueName;

		//[Column]
        //[ScaffoldColumn(true)]
		public string String { get; set; }
		// private string _unmodified_String;
        //[Column(Name = "StringArray")] 
        //[ScaffoldColumn(true)]
		public string StringArrayData { get; set; }

        private bool _IsStringArrayRetrieved = false;
        private bool _IsStringArrayChanged = false;
        private ObservableCollection<string> _StringArray = null;
        [NotMapped]
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


		//[Column]
        //[ScaffoldColumn(true)]
		public double Number { get; set; }
		// private double _unmodified_Number;
        //[Column(Name = "NumberArray")] 
        //[ScaffoldColumn(true)]
		public string NumberArrayData { get; set; }

        private bool _IsNumberArrayRetrieved = false;
        private bool _IsNumberArrayChanged = false;
        private ObservableCollection<double> _NumberArray = null;
        [NotMapped]
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


		//[Column]
        //[ScaffoldColumn(true)]
		public bool Boolean { get; set; }
		// private bool _unmodified_Boolean;
        //[Column(Name = "BooleanArray")] 
        //[ScaffoldColumn(true)]
		public string BooleanArrayData { get; set; }

        private bool _IsBooleanArrayRetrieved = false;
        private bool _IsBooleanArrayChanged = false;
        private ObservableCollection<bool> _BooleanArray = null;
        [NotMapped]
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


		//[Column]
        //[ScaffoldColumn(true)]
		public DateTime DateTime { get; set; }
		// private DateTime _unmodified_DateTime;
        //[Column(Name = "DateTimeArray")] 
        //[ScaffoldColumn(true)]
		public string DateTimeArrayData { get; set; }

        private bool _IsDateTimeArrayRetrieved = false;
        private bool _IsDateTimeArrayChanged = false;
        private ObservableCollection<DateTime> _DateTimeArray = null;
        [NotMapped]
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

			//[Column]
			public string ObjectID { get; set; }
			private EntityRef< GenericObject > _Object;
			[Association(Storage = "_Object", ThisKey = "ObjectID")]
			public GenericObject Object
			{
				get { return this._Object.Entity; }
				set { this._Object.Entity = value; }
			}

        //[Column(Name = "ObjectArray")] 
        //[ScaffoldColumn(true)]
		public string ObjectArrayData { get; set; }

        private bool _IsObjectArrayRetrieved = false;
        private bool _IsObjectArrayChanged = false;
        private ObservableCollection<SER.TheBall.Interface.GenericObject> _ObjectArray = null;
        [NotMapped]
		public ObservableCollection<SER.TheBall.Interface.GenericObject> ObjectArray
        {
            get
            {
                if (!_IsObjectArrayRetrieved)
                {
                    if (ObjectArrayData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.Interface.GenericObject[]>(ObjectArrayData);
                        _ObjectArray = new ObservableCollection<SER.TheBall.Interface.GenericObject>(arrayData);
                    }
                    else
                    {
                        _ObjectArray = new ObservableCollection<SER.TheBall.Interface.GenericObject>();
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


		//[Column]
        //[ScaffoldColumn(true)]
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
    [Table("ConnectionCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("ConnectionCollection: {ID}")]
	public class ConnectionCollection : ITheBallDataContextStorable
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


		public ConnectionCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ConnectionCollection](
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
    [Table("GenericObjectCollection")]
	//[ScaffoldTable(true)]
	[DebuggerDisplay("GenericObjectCollection: {ID}")]
	public class GenericObjectCollection : ITheBallDataContextStorable
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


		public GenericObjectCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GenericObjectCollection](
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
 } 
