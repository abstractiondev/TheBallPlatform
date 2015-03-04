 


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


namespace SQLite.TheBall.CORE { 
		
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
				tableCreationCommands.Add(ContentPackage.GetCreateTableSQL());
				tableCreationCommands.Add(InformationInput.GetCreateTableSQL());
				tableCreationCommands.Add(InformationOutput.GetCreateTableSQL());
				tableCreationCommands.Add(AuthenticatedAsActiveDevice.GetCreateTableSQL());
				tableCreationCommands.Add(DeviceMembership.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceFiscalExportSummary.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceSummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(Invoice.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceDetails.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceUser.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceRowGroup.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceEventDetailGroup.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceEventDetail.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceRow.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(ProcessContainer.GetCreateTableSQL());
				tableCreationCommands.Add(Process.GetCreateTableSQL());
				tableCreationCommands.Add(ProcessItem.GetCreateTableSQL());
				tableCreationCommands.Add(SemanticInformationItem.GetCreateTableSQL());
				tableCreationCommands.Add(InformationOwnerInfo.GetCreateTableSQL());
				tableCreationCommands.Add(UsageSummary.GetCreateTableSQL());
				tableCreationCommands.Add(UsageMonitorItem.GetCreateTableSQL());
				tableCreationCommands.Add(RequestResourceUsage.GetCreateTableSQL());
				tableCreationCommands.Add(ProcessorUsage.GetCreateTableSQL());
				tableCreationCommands.Add(StorageTransactionUsage.GetCreateTableSQL());
				tableCreationCommands.Add(StorageUsage.GetCreateTableSQL());
				tableCreationCommands.Add(NetworkUsage.GetCreateTableSQL());
				tableCreationCommands.Add(TimeRange.GetCreateTableSQL());
				tableCreationCommands.Add(HTTPActivityDetails.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "ContentPackage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.ContentPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContentPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PackageType = serializedObject.PackageType;
		            existingObject.PackageName = serializedObject.PackageName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.PackageRootFolder = serializedObject.PackageRootFolder;
		            existingObject.CreationTime = serializedObject.CreationTime;
		            return;
		        } 
		        if (updateData.ObjectType == "InformationInput")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InformationInput.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationInputTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InputDescription = serializedObject.InputDescription;
		            existingObject.LocationURL = serializedObject.LocationURL;
		            existingObject.LocalContentName = serializedObject.LocalContentName;
		            existingObject.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            return;
		        } 
		        if (updateData.ObjectType == "InformationOutput")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InformationOutput.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationOutputTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OutputDescription = serializedObject.OutputDescription;
		            existingObject.DestinationURL = serializedObject.DestinationURL;
		            existingObject.DestinationContentName = serializedObject.DestinationContentName;
		            existingObject.LocalContentURL = serializedObject.LocalContentURL;
		            existingObject.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            return;
		        } 
		        if (updateData.ObjectType == "AuthenticatedAsActiveDevice")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.AuthenticatedAsActiveDevice.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AuthenticatedAsActiveDeviceTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AuthenticationDescription = serializedObject.AuthenticationDescription;
		            existingObject.SharedSecret = serializedObject.SharedSecret;
		            existingObject.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            existingObject.EstablishedTrustID = serializedObject.EstablishedTrustID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            existingObject.NegotiationURL = serializedObject.NegotiationURL;
		            existingObject.ConnectionURL = serializedObject.ConnectionURL;
		            return;
		        } 
		        if (updateData.ObjectType == "DeviceMembership")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.DeviceMembership.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DeviceMembershipTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.DeviceDescription = serializedObject.DeviceDescription;
		            existingObject.SharedSecret = serializedObject.SharedSecret;
		            existingObject.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceFiscalExportSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceFiscalExportSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            existingObject.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceSummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "Invoice")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.Invoice.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InvoiceName = serializedObject.InvoiceName;
		            existingObject.InvoiceID = serializedObject.InvoiceID;
		            existingObject.InvoicedAmount = serializedObject.InvoicedAmount;
		            existingObject.CreateDate = serializedObject.CreateDate;
		            existingObject.DueDate = serializedObject.DueDate;
		            existingObject.PaidAmount = serializedObject.PaidAmount;
		            existingObject.FeesAndInterestAmount = serializedObject.FeesAndInterestAmount;
		            existingObject.UnpaidAmount = serializedObject.UnpaidAmount;
					if(serializedObject.InvoiceDetails != null)
						existingObject.InvoiceDetailsID = serializedObject.InvoiceDetails.ID;
					else
						existingObject.InvoiceDetailsID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceDetails")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceDetails.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceDetailsTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.MonthlyFeesTotal = serializedObject.MonthlyFeesTotal;
		            existingObject.OneTimeFeesTotal = serializedObject.OneTimeFeesTotal;
		            existingObject.UsageFeesTotal = serializedObject.UsageFeesTotal;
		            existingObject.InterestFeesTotal = serializedObject.InterestFeesTotal;
		            existingObject.PenaltyFeesTotal = serializedObject.PenaltyFeesTotal;
		            existingObject.TotalFeesTotal = serializedObject.TotalFeesTotal;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceUser")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceUser.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceUserTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.UserName = serializedObject.UserName;
		            existingObject.UserID = serializedObject.UserID;
		            existingObject.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            existingObject.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            existingObject.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceRowGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceRowGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceRowGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            existingObject.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            existingObject.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceEventDetailGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceEventDetail")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceEventDetail.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IndentMode = serializedObject.IndentMode;
		            existingObject.EventStartDateTime = serializedObject.EventStartDateTime;
		            existingObject.EventEndDateTime = serializedObject.EventEndDateTime;
		            existingObject.ReceivingParty = serializedObject.ReceivingParty;
		            existingObject.AmountOfUnits = serializedObject.AmountOfUnits;
		            existingObject.Duration = serializedObject.Duration;
		            existingObject.UnitPrice = serializedObject.UnitPrice;
		            existingObject.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            existingObject.Taxes = serializedObject.Taxes;
		            existingObject.PriceWithTaxes = serializedObject.PriceWithTaxes;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceRow")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InvoiceRow.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceRowTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IndentMode = serializedObject.IndentMode;
		            existingObject.AmountOfUnits = serializedObject.AmountOfUnits;
		            existingObject.Duration = serializedObject.Duration;
		            existingObject.UnitPrice = serializedObject.UnitPrice;
		            existingObject.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            existingObject.Taxes = serializedObject.Taxes;
		            existingObject.PriceWithTaxes = serializedObject.PriceWithTaxes;
		            return;
		        } 
		        if (updateData.ObjectType == "Category")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.Category.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            return;
		        } 
		        if (updateData.ObjectType == "ProcessContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.ProcessContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.ProcessIDs.Clear();
					if(serializedObject.ProcessIDs != null)
	                    serializedObject.ProcessIDs.ForEach(item => existingObject.ProcessIDs.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "Process")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.Process.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ProcessDescription = serializedObject.ProcessDescription;
					if(serializedObject.ExecutingOperation != null)
						existingObject.ExecutingOperationID = serializedObject.ExecutingOperation.ID;
					else
						existingObject.ExecutingOperationID = null;
                    existingObject.InitialArguments.Clear();
					if(serializedObject.InitialArguments != null)
	                    serializedObject.InitialArguments.ForEach(item => existingObject.InitialArguments.Add(item));
					
                    existingObject.ProcessItems.Clear();
					if(serializedObject.ProcessItems != null)
	                    serializedObject.ProcessItems.ForEach(item => existingObject.ProcessItems.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "ProcessItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.ProcessItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.Outputs.Clear();
					if(serializedObject.Outputs != null)
	                    serializedObject.Outputs.ForEach(item => existingObject.Outputs.Add(item));
					
                    existingObject.Inputs.Clear();
					if(serializedObject.Inputs != null)
	                    serializedObject.Inputs.ForEach(item => existingObject.Inputs.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "SemanticInformationItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SemanticInformationItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ItemFullType = serializedObject.ItemFullType;
		            existingObject.ItemValue = serializedObject.ItemValue;
		            return;
		        } 
		        if (updateData.ObjectType == "InformationOwnerInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationOwnerInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OwnerType = serializedObject.OwnerType;
		            existingObject.OwnerIdentifier = serializedObject.OwnerIdentifier;
		            return;
		        } 
		        if (updateData.ObjectType == "UsageSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.UsageSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UsageSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SummaryName = serializedObject.SummaryName;
					if(serializedObject.SummaryMonitoringItem != null)
						existingObject.SummaryMonitoringItemID = serializedObject.SummaryMonitoringItem.ID;
					else
						existingObject.SummaryMonitoringItemID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "UsageMonitorItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.UsageMonitorItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UsageMonitorItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.OwnerInfo != null)
						existingObject.OwnerInfoID = serializedObject.OwnerInfo.ID;
					else
						existingObject.OwnerInfoID = null;
					if(serializedObject.TimeRangeInclusiveStartExclusiveEnd != null)
						existingObject.TimeRangeInclusiveStartExclusiveEndID = serializedObject.TimeRangeInclusiveStartExclusiveEnd.ID;
					else
						existingObject.TimeRangeInclusiveStartExclusiveEndID = null;
		            existingObject.StepSizeInMinutes = serializedObject.StepSizeInMinutes;
		            return;
		        } 
		        if (updateData.ObjectType == "RequestResourceUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.RequestResourceUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RequestResourceUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.OwnerInfo != null)
						existingObject.OwnerInfoID = serializedObject.OwnerInfo.ID;
					else
						existingObject.OwnerInfoID = null;
					if(serializedObject.ProcessorUsage != null)
						existingObject.ProcessorUsageID = serializedObject.ProcessorUsage.ID;
					else
						existingObject.ProcessorUsageID = null;
					if(serializedObject.StorageTransactionUsage != null)
						existingObject.StorageTransactionUsageID = serializedObject.StorageTransactionUsage.ID;
					else
						existingObject.StorageTransactionUsageID = null;
					if(serializedObject.NetworkUsage != null)
						existingObject.NetworkUsageID = serializedObject.NetworkUsage.ID;
					else
						existingObject.NetworkUsageID = null;
					if(serializedObject.RequestDetails != null)
						existingObject.RequestDetailsID = serializedObject.RequestDetails.ID;
					else
						existingObject.RequestDetailsID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "ProcessorUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.ProcessorUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessorUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.TimeRange != null)
						existingObject.TimeRangeID = serializedObject.TimeRange.ID;
					else
						existingObject.TimeRangeID = null;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfTicks = serializedObject.AmountOfTicks;
		            existingObject.FrequencyTicksPerSecond = serializedObject.FrequencyTicksPerSecond;
		            existingObject.Milliseconds = serializedObject.Milliseconds;
		            return;
		        } 
		        if (updateData.ObjectType == "StorageTransactionUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.StorageTransactionUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StorageTransactionUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.TimeRange != null)
						existingObject.TimeRangeID = serializedObject.TimeRange.ID;
					else
						existingObject.TimeRangeID = null;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfTransactions = serializedObject.AmountOfTransactions;
		            return;
		        } 
		        if (updateData.ObjectType == "StorageUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.StorageUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StorageUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SnapshotTime = serializedObject.SnapshotTime;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.UsageUnit = serializedObject.UsageUnit;
		            existingObject.AmountOfUnits = serializedObject.AmountOfUnits;
		            return;
		        } 
		        if (updateData.ObjectType == "NetworkUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.NetworkUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NetworkUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.TimeRange != null)
						existingObject.TimeRangeID = serializedObject.TimeRange.ID;
					else
						existingObject.TimeRangeID = null;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfBytes = serializedObject.AmountOfBytes;
		            return;
		        } 
		        if (updateData.ObjectType == "TimeRange")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TimeRangeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StartTime = serializedObject.StartTime;
		            existingObject.EndTime = serializedObject.EndTime;
		            return;
		        } 
		        if (updateData.ObjectType == "HTTPActivityDetails")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.TheBall.CORE.HTTPActivityDetails.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = HTTPActivityDetailsTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.RemoteIPAddress = serializedObject.RemoteIPAddress;
		            existingObject.RemoteEndpointUserName = serializedObject.RemoteEndpointUserName;
		            existingObject.UserID = serializedObject.UserID;
		            existingObject.UTCDateTime = serializedObject.UTCDateTime;
		            existingObject.RequestLine = serializedObject.RequestLine;
		            existingObject.HTTPStatusCode = serializedObject.HTTPStatusCode;
		            existingObject.ReturnedContentLength = serializedObject.ReturnedContentLength;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "ContentPackage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ContentPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContentPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PackageType = serializedObject.PackageType;
		            objectToAdd.PackageName = serializedObject.PackageName;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.PackageRootFolder = serializedObject.PackageRootFolder;
		            objectToAdd.CreationTime = serializedObject.CreationTime;
					ContentPackageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationInput")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationInput.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationInput {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InputDescription = serializedObject.InputDescription;
		            objectToAdd.LocationURL = serializedObject.LocationURL;
		            objectToAdd.LocalContentName = serializedObject.LocalContentName;
		            objectToAdd.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					InformationInputTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationOutput")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationOutput.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationOutput {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OutputDescription = serializedObject.OutputDescription;
		            objectToAdd.DestinationURL = serializedObject.DestinationURL;
		            objectToAdd.DestinationContentName = serializedObject.DestinationContentName;
		            objectToAdd.LocalContentURL = serializedObject.LocalContentURL;
		            objectToAdd.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					InformationOutputTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AuthenticatedAsActiveDevice")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.AuthenticatedAsActiveDevice.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AuthenticatedAsActiveDevice {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AuthenticationDescription = serializedObject.AuthenticationDescription;
		            objectToAdd.SharedSecret = serializedObject.SharedSecret;
		            objectToAdd.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            objectToAdd.EstablishedTrustID = serializedObject.EstablishedTrustID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            objectToAdd.NegotiationURL = serializedObject.NegotiationURL;
		            objectToAdd.ConnectionURL = serializedObject.ConnectionURL;
					AuthenticatedAsActiveDeviceTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "DeviceMembership")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.DeviceMembership.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DeviceMembership {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.DeviceDescription = serializedObject.DeviceDescription;
		            objectToAdd.SharedSecret = serializedObject.SharedSecret;
		            objectToAdd.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					DeviceMembershipTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceFiscalExportSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceFiscalExportSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            objectToAdd.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
					InvoiceFiscalExportSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceSummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					InvoiceSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Invoice")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Invoice.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Invoice {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InvoiceName = serializedObject.InvoiceName;
		            objectToAdd.InvoiceID = serializedObject.InvoiceID;
		            objectToAdd.InvoicedAmount = serializedObject.InvoicedAmount;
		            objectToAdd.CreateDate = serializedObject.CreateDate;
		            objectToAdd.DueDate = serializedObject.DueDate;
		            objectToAdd.PaidAmount = serializedObject.PaidAmount;
		            objectToAdd.FeesAndInterestAmount = serializedObject.FeesAndInterestAmount;
		            objectToAdd.UnpaidAmount = serializedObject.UnpaidAmount;
					if(serializedObject.InvoiceDetails != null)
						objectToAdd.InvoiceDetailsID = serializedObject.InvoiceDetails.ID;
					else
						objectToAdd.InvoiceDetailsID = null;
					InvoiceTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceDetails")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceDetails.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceDetails {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.MonthlyFeesTotal = serializedObject.MonthlyFeesTotal;
		            objectToAdd.OneTimeFeesTotal = serializedObject.OneTimeFeesTotal;
		            objectToAdd.UsageFeesTotal = serializedObject.UsageFeesTotal;
		            objectToAdd.InterestFeesTotal = serializedObject.InterestFeesTotal;
		            objectToAdd.PenaltyFeesTotal = serializedObject.PenaltyFeesTotal;
		            objectToAdd.TotalFeesTotal = serializedObject.TotalFeesTotal;
					InvoiceDetailsTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceUser")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceUser.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceUser {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.UserName = serializedObject.UserName;
		            objectToAdd.UserID = serializedObject.UserID;
		            objectToAdd.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            objectToAdd.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            objectToAdd.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
					InvoiceUserTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceRowGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceRowGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceRowGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            objectToAdd.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            objectToAdd.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
					InvoiceRowGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceEventDetailGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetailGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					InvoiceEventDetailGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceEventDetail")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceEventDetail.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndentMode = serializedObject.IndentMode;
		            objectToAdd.EventStartDateTime = serializedObject.EventStartDateTime;
		            objectToAdd.EventEndDateTime = serializedObject.EventEndDateTime;
		            objectToAdd.ReceivingParty = serializedObject.ReceivingParty;
		            objectToAdd.AmountOfUnits = serializedObject.AmountOfUnits;
		            objectToAdd.Duration = serializedObject.Duration;
		            objectToAdd.UnitPrice = serializedObject.UnitPrice;
		            objectToAdd.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            objectToAdd.Taxes = serializedObject.Taxes;
		            objectToAdd.PriceWithTaxes = serializedObject.PriceWithTaxes;
					InvoiceEventDetailTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceRow")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceRow.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceRow {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndentMode = serializedObject.IndentMode;
		            objectToAdd.AmountOfUnits = serializedObject.AmountOfUnits;
		            objectToAdd.Duration = serializedObject.Duration;
		            objectToAdd.UnitPrice = serializedObject.UnitPrice;
		            objectToAdd.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            objectToAdd.Taxes = serializedObject.Taxes;
		            objectToAdd.PriceWithTaxes = serializedObject.PriceWithTaxes;
					InvoiceRowTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Category")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Category.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProcessContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProcessIDs != null)
						serializedObject.ProcessIDs.ForEach(item => objectToAdd.ProcessIDs.Add(item));
					ProcessContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Process")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Process.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Process {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ProcessDescription = serializedObject.ProcessDescription;
					if(serializedObject.ExecutingOperation != null)
						objectToAdd.ExecutingOperationID = serializedObject.ExecutingOperation.ID;
					else
						objectToAdd.ExecutingOperationID = null;
					if(serializedObject.InitialArguments != null)
						serializedObject.InitialArguments.ForEach(item => objectToAdd.InitialArguments.Add(item));
					if(serializedObject.ProcessItems != null)
						serializedObject.ProcessItems.ForEach(item => objectToAdd.ProcessItems.Add(item));
					ProcessTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProcessItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessItem {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Outputs != null)
						serializedObject.Outputs.ForEach(item => objectToAdd.Outputs.Add(item));
					if(serializedObject.Inputs != null)
						serializedObject.Inputs.ForEach(item => objectToAdd.Inputs.Add(item));
					ProcessItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SemanticInformationItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SemanticInformationItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ItemFullType = serializedObject.ItemFullType;
		            objectToAdd.ItemValue = serializedObject.ItemValue;
					SemanticInformationItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationOwnerInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationOwnerInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OwnerType = serializedObject.OwnerType;
		            objectToAdd.OwnerIdentifier = serializedObject.OwnerIdentifier;
					InformationOwnerInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UsageSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.UsageSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UsageSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SummaryName = serializedObject.SummaryName;
					if(serializedObject.SummaryMonitoringItem != null)
						objectToAdd.SummaryMonitoringItemID = serializedObject.SummaryMonitoringItem.ID;
					else
						objectToAdd.SummaryMonitoringItemID = null;
					UsageSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UsageMonitorItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.UsageMonitorItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UsageMonitorItem {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.OwnerInfo != null)
						objectToAdd.OwnerInfoID = serializedObject.OwnerInfo.ID;
					else
						objectToAdd.OwnerInfoID = null;
					if(serializedObject.TimeRangeInclusiveStartExclusiveEnd != null)
						objectToAdd.TimeRangeInclusiveStartExclusiveEndID = serializedObject.TimeRangeInclusiveStartExclusiveEnd.ID;
					else
						objectToAdd.TimeRangeInclusiveStartExclusiveEndID = null;
		            objectToAdd.StepSizeInMinutes = serializedObject.StepSizeInMinutes;
					UsageMonitorItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RequestResourceUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.RequestResourceUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RequestResourceUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.OwnerInfo != null)
						objectToAdd.OwnerInfoID = serializedObject.OwnerInfo.ID;
					else
						objectToAdd.OwnerInfoID = null;
					if(serializedObject.ProcessorUsage != null)
						objectToAdd.ProcessorUsageID = serializedObject.ProcessorUsage.ID;
					else
						objectToAdd.ProcessorUsageID = null;
					if(serializedObject.StorageTransactionUsage != null)
						objectToAdd.StorageTransactionUsageID = serializedObject.StorageTransactionUsage.ID;
					else
						objectToAdd.StorageTransactionUsageID = null;
					if(serializedObject.NetworkUsage != null)
						objectToAdd.NetworkUsageID = serializedObject.NetworkUsage.ID;
					else
						objectToAdd.NetworkUsageID = null;
					if(serializedObject.RequestDetails != null)
						objectToAdd.RequestDetailsID = serializedObject.RequestDetails.ID;
					else
						objectToAdd.RequestDetailsID = null;
					RequestResourceUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProcessorUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessorUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessorUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.TimeRange != null)
						objectToAdd.TimeRangeID = serializedObject.TimeRange.ID;
					else
						objectToAdd.TimeRangeID = null;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfTicks = serializedObject.AmountOfTicks;
		            objectToAdd.FrequencyTicksPerSecond = serializedObject.FrequencyTicksPerSecond;
		            objectToAdd.Milliseconds = serializedObject.Milliseconds;
					ProcessorUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "StorageTransactionUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.StorageTransactionUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StorageTransactionUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.TimeRange != null)
						objectToAdd.TimeRangeID = serializedObject.TimeRange.ID;
					else
						objectToAdd.TimeRangeID = null;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfTransactions = serializedObject.AmountOfTransactions;
					StorageTransactionUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "StorageUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.StorageUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StorageUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SnapshotTime = serializedObject.SnapshotTime;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.UsageUnit = serializedObject.UsageUnit;
		            objectToAdd.AmountOfUnits = serializedObject.AmountOfUnits;
					StorageUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "NetworkUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.NetworkUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NetworkUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.TimeRange != null)
						objectToAdd.TimeRangeID = serializedObject.TimeRange.ID;
					else
						objectToAdd.TimeRangeID = null;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfBytes = serializedObject.AmountOfBytes;
					NetworkUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TimeRange")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TimeRange {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StartTime = serializedObject.StartTime;
		            objectToAdd.EndTime = serializedObject.EndTime;
					TimeRangeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "HTTPActivityDetails")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.HTTPActivityDetails.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new HTTPActivityDetails {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.RemoteIPAddress = serializedObject.RemoteIPAddress;
		            objectToAdd.RemoteEndpointUserName = serializedObject.RemoteEndpointUserName;
		            objectToAdd.UserID = serializedObject.UserID;
		            objectToAdd.UTCDateTime = serializedObject.UTCDateTime;
		            objectToAdd.RequestLine = serializedObject.RequestLine;
		            objectToAdd.HTTPStatusCode = serializedObject.HTTPStatusCode;
		            objectToAdd.ReturnedContentLength = serializedObject.ReturnedContentLength;
					HTTPActivityDetailsTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "ContentPackage")
		        {
		            var objectToDelete = new ContentPackage {ID = deleteData.ID};
                    ContentPackageTable.Attach(objectToDelete);
                    ContentPackageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InformationInput")
		        {
		            var objectToDelete = new InformationInput {ID = deleteData.ID};
                    InformationInputTable.Attach(objectToDelete);
                    InformationInputTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InformationOutput")
		        {
		            var objectToDelete = new InformationOutput {ID = deleteData.ID};
                    InformationOutputTable.Attach(objectToDelete);
                    InformationOutputTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AuthenticatedAsActiveDevice")
		        {
		            var objectToDelete = new AuthenticatedAsActiveDevice {ID = deleteData.ID};
                    AuthenticatedAsActiveDeviceTable.Attach(objectToDelete);
                    AuthenticatedAsActiveDeviceTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "DeviceMembership")
		        {
		            var objectToDelete = new DeviceMembership {ID = deleteData.ID};
                    DeviceMembershipTable.Attach(objectToDelete);
                    DeviceMembershipTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceFiscalExportSummary")
		        {
		            var objectToDelete = new InvoiceFiscalExportSummary {ID = deleteData.ID};
                    InvoiceFiscalExportSummaryTable.Attach(objectToDelete);
                    InvoiceFiscalExportSummaryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceSummaryContainer")
		        {
		            var objectToDelete = new InvoiceSummaryContainer {ID = deleteData.ID};
                    InvoiceSummaryContainerTable.Attach(objectToDelete);
                    InvoiceSummaryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Invoice")
		        {
		            var objectToDelete = new Invoice {ID = deleteData.ID};
                    InvoiceTable.Attach(objectToDelete);
                    InvoiceTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceDetails")
		        {
		            var objectToDelete = new InvoiceDetails {ID = deleteData.ID};
                    InvoiceDetailsTable.Attach(objectToDelete);
                    InvoiceDetailsTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceUser")
		        {
		            var objectToDelete = new InvoiceUser {ID = deleteData.ID};
                    InvoiceUserTable.Attach(objectToDelete);
                    InvoiceUserTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceRowGroup")
		        {
		            var objectToDelete = new InvoiceRowGroup {ID = deleteData.ID};
                    InvoiceRowGroupTable.Attach(objectToDelete);
                    InvoiceRowGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceEventDetailGroup")
		        {
		            var objectToDelete = new InvoiceEventDetailGroup {ID = deleteData.ID};
                    InvoiceEventDetailGroupTable.Attach(objectToDelete);
                    InvoiceEventDetailGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceEventDetail")
		        {
		            var objectToDelete = new InvoiceEventDetail {ID = deleteData.ID};
                    InvoiceEventDetailTable.Attach(objectToDelete);
                    InvoiceEventDetailTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceRow")
		        {
		            var objectToDelete = new InvoiceRow {ID = deleteData.ID};
                    InvoiceRowTable.Attach(objectToDelete);
                    InvoiceRowTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Category")
		        {
		            var objectToDelete = new Category {ID = deleteData.ID};
                    CategoryTable.Attach(objectToDelete);
                    CategoryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessContainer")
		        {
		            var objectToDelete = new ProcessContainer {ID = deleteData.ID};
                    ProcessContainerTable.Attach(objectToDelete);
                    ProcessContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Process")
		        {
		            var objectToDelete = new Process {ID = deleteData.ID};
                    ProcessTable.Attach(objectToDelete);
                    ProcessTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessItem")
		        {
		            var objectToDelete = new ProcessItem {ID = deleteData.ID};
                    ProcessItemTable.Attach(objectToDelete);
                    ProcessItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SemanticInformationItem")
		        {
		            var objectToDelete = new SemanticInformationItem {ID = deleteData.ID};
                    SemanticInformationItemTable.Attach(objectToDelete);
                    SemanticInformationItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InformationOwnerInfo")
		        {
		            var objectToDelete = new InformationOwnerInfo {ID = deleteData.ID};
                    InformationOwnerInfoTable.Attach(objectToDelete);
                    InformationOwnerInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "UsageSummary")
		        {
		            var objectToDelete = new UsageSummary {ID = deleteData.ID};
                    UsageSummaryTable.Attach(objectToDelete);
                    UsageSummaryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "UsageMonitorItem")
		        {
		            var objectToDelete = new UsageMonitorItem {ID = deleteData.ID};
                    UsageMonitorItemTable.Attach(objectToDelete);
                    UsageMonitorItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "RequestResourceUsage")
		        {
		            var objectToDelete = new RequestResourceUsage {ID = deleteData.ID};
                    RequestResourceUsageTable.Attach(objectToDelete);
                    RequestResourceUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessorUsage")
		        {
		            var objectToDelete = new ProcessorUsage {ID = deleteData.ID};
                    ProcessorUsageTable.Attach(objectToDelete);
                    ProcessorUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "StorageTransactionUsage")
		        {
		            var objectToDelete = new StorageTransactionUsage {ID = deleteData.ID};
                    StorageTransactionUsageTable.Attach(objectToDelete);
                    StorageTransactionUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "StorageUsage")
		        {
		            var objectToDelete = new StorageUsage {ID = deleteData.ID};
                    StorageUsageTable.Attach(objectToDelete);
                    StorageUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "NetworkUsage")
		        {
		            var objectToDelete = new NetworkUsage {ID = deleteData.ID};
                    NetworkUsageTable.Attach(objectToDelete);
                    NetworkUsageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TimeRange")
		        {
		            var objectToDelete = new TimeRange {ID = deleteData.ID};
                    TimeRangeTable.Attach(objectToDelete);
                    TimeRangeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "HTTPActivityDetails")
		        {
		            var objectToDelete = new HTTPActivityDetails {ID = deleteData.ID};
                    HTTPActivityDetailsTable.Attach(objectToDelete);
                    HTTPActivityDetailsTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		    }


			public Table<ContentPackage> ContentPackageTable {
				get {
					return this.GetTable<ContentPackage>();
				}
			}
			public Table<InformationInput> InformationInputTable {
				get {
					return this.GetTable<InformationInput>();
				}
			}
			public Table<InformationOutput> InformationOutputTable {
				get {
					return this.GetTable<InformationOutput>();
				}
			}
			public Table<AuthenticatedAsActiveDevice> AuthenticatedAsActiveDeviceTable {
				get {
					return this.GetTable<AuthenticatedAsActiveDevice>();
				}
			}
			public Table<DeviceMembership> DeviceMembershipTable {
				get {
					return this.GetTable<DeviceMembership>();
				}
			}
			public Table<InvoiceFiscalExportSummary> InvoiceFiscalExportSummaryTable {
				get {
					return this.GetTable<InvoiceFiscalExportSummary>();
				}
			}
			public Table<InvoiceSummaryContainer> InvoiceSummaryContainerTable {
				get {
					return this.GetTable<InvoiceSummaryContainer>();
				}
			}
			public Table<Invoice> InvoiceTable {
				get {
					return this.GetTable<Invoice>();
				}
			}
			public Table<InvoiceDetails> InvoiceDetailsTable {
				get {
					return this.GetTable<InvoiceDetails>();
				}
			}
			public Table<InvoiceUser> InvoiceUserTable {
				get {
					return this.GetTable<InvoiceUser>();
				}
			}
			public Table<InvoiceRowGroup> InvoiceRowGroupTable {
				get {
					return this.GetTable<InvoiceRowGroup>();
				}
			}
			public Table<InvoiceEventDetailGroup> InvoiceEventDetailGroupTable {
				get {
					return this.GetTable<InvoiceEventDetailGroup>();
				}
			}
			public Table<InvoiceEventDetail> InvoiceEventDetailTable {
				get {
					return this.GetTable<InvoiceEventDetail>();
				}
			}
			public Table<InvoiceRow> InvoiceRowTable {
				get {
					return this.GetTable<InvoiceRow>();
				}
			}
			public Table<Category> CategoryTable {
				get {
					return this.GetTable<Category>();
				}
			}
			public Table<ProcessContainer> ProcessContainerTable {
				get {
					return this.GetTable<ProcessContainer>();
				}
			}
			public Table<Process> ProcessTable {
				get {
					return this.GetTable<Process>();
				}
			}
			public Table<ProcessItem> ProcessItemTable {
				get {
					return this.GetTable<ProcessItem>();
				}
			}
			public Table<SemanticInformationItem> SemanticInformationItemTable {
				get {
					return this.GetTable<SemanticInformationItem>();
				}
			}
			public Table<InformationOwnerInfo> InformationOwnerInfoTable {
				get {
					return this.GetTable<InformationOwnerInfo>();
				}
			}
			public Table<UsageSummary> UsageSummaryTable {
				get {
					return this.GetTable<UsageSummary>();
				}
			}
			public Table<UsageMonitorItem> UsageMonitorItemTable {
				get {
					return this.GetTable<UsageMonitorItem>();
				}
			}
			public Table<RequestResourceUsage> RequestResourceUsageTable {
				get {
					return this.GetTable<RequestResourceUsage>();
				}
			}
			public Table<ProcessorUsage> ProcessorUsageTable {
				get {
					return this.GetTable<ProcessorUsage>();
				}
			}
			public Table<StorageTransactionUsage> StorageTransactionUsageTable {
				get {
					return this.GetTable<StorageTransactionUsage>();
				}
			}
			public Table<StorageUsage> StorageUsageTable {
				get {
					return this.GetTable<StorageUsage>();
				}
			}
			public Table<NetworkUsage> NetworkUsageTable {
				get {
					return this.GetTable<NetworkUsage>();
				}
			}
			public Table<TimeRange> TimeRangeTable {
				get {
					return this.GetTable<TimeRange>();
				}
			}
			public Table<HTTPActivityDetails> HTTPActivityDetailsTable {
				get {
					return this.GetTable<HTTPActivityDetails>();
				}
			}
        }

    [Table(Name = "ContentPackage")]
	[ScaffoldTable(true)]
	public class ContentPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[PackageType] TEXT NOT NULL, 
[PackageName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[PackageRootFolder] TEXT NOT NULL, 
[CreationTime] TEXT NOT NULL
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


		public ContentPackage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
        [ScaffoldColumn(true)]
		public string PackageName { get; set; }
		// private string _unmodified_PackageName;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string PackageRootFolder { get; set; }
		// private string _unmodified_PackageRootFolder;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime CreationTime { get; set; }
		// private DateTime _unmodified_CreationTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PackageType == null)
				PackageType = string.Empty;
			if(PackageName == null)
				PackageName = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(PackageRootFolder == null)
				PackageRootFolder = string.Empty;
		}
	}
    [Table(Name = "InformationInput")]
	[ScaffoldTable(true)]
	public class InformationInput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationInput](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[InputDescription] TEXT NOT NULL, 
[LocationURL] TEXT NOT NULL, 
[LocalContentName] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
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


		public InformationInput() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string InputDescription { get; set; }
		// private string _unmodified_InputDescription;

		[Column]
        [ScaffoldColumn(true)]
		public string LocationURL { get; set; }
		// private string _unmodified_LocationURL;

		[Column]
        [ScaffoldColumn(true)]
		public string LocalContentName { get; set; }
		// private string _unmodified_LocalContentName;

		[Column]
        [ScaffoldColumn(true)]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(InputDescription == null)
				InputDescription = string.Empty;
			if(LocationURL == null)
				LocationURL = string.Empty;
			if(LocalContentName == null)
				LocalContentName = string.Empty;
			if(AuthenticatedDeviceID == null)
				AuthenticatedDeviceID = string.Empty;
		}
	}
    [Table(Name = "InformationOutput")]
	[ScaffoldTable(true)]
	public class InformationOutput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationOutput](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OutputDescription] TEXT NOT NULL, 
[DestinationURL] TEXT NOT NULL, 
[DestinationContentName] TEXT NOT NULL, 
[LocalContentURL] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
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


		public InformationOutput() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OutputDescription { get; set; }
		// private string _unmodified_OutputDescription;

		[Column]
        [ScaffoldColumn(true)]
		public string DestinationURL { get; set; }
		// private string _unmodified_DestinationURL;

		[Column]
        [ScaffoldColumn(true)]
		public string DestinationContentName { get; set; }
		// private string _unmodified_DestinationContentName;

		[Column]
        [ScaffoldColumn(true)]
		public string LocalContentURL { get; set; }
		// private string _unmodified_LocalContentURL;

		[Column]
        [ScaffoldColumn(true)]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OutputDescription == null)
				OutputDescription = string.Empty;
			if(DestinationURL == null)
				DestinationURL = string.Empty;
			if(DestinationContentName == null)
				DestinationContentName = string.Empty;
			if(LocalContentURL == null)
				LocalContentURL = string.Empty;
			if(AuthenticatedDeviceID == null)
				AuthenticatedDeviceID = string.Empty;
		}
	}
    [Table(Name = "AuthenticatedAsActiveDevice")]
	[ScaffoldTable(true)]
	public class AuthenticatedAsActiveDevice : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AuthenticatedAsActiveDevice](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[AuthenticationDescription] TEXT NOT NULL, 
[SharedSecret] TEXT NOT NULL, 
[ActiveSymmetricAESKey] BLOB NOT NULL, 
[EstablishedTrustID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL, 
[NegotiationURL] TEXT NOT NULL, 
[ConnectionURL] TEXT NOT NULL
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


		public AuthenticatedAsActiveDevice() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string AuthenticationDescription { get; set; }
		// private string _unmodified_AuthenticationDescription;

		[Column]
        [ScaffoldColumn(true)]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
        [ScaffoldColumn(true)]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
        [ScaffoldColumn(true)]
		public string EstablishedTrustID { get; set; }
		// private string _unmodified_EstablishedTrustID;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;

		[Column]
        [ScaffoldColumn(true)]
		public string NegotiationURL { get; set; }
		// private string _unmodified_NegotiationURL;

		[Column]
        [ScaffoldColumn(true)]
		public string ConnectionURL { get; set; }
		// private string _unmodified_ConnectionURL;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(AuthenticationDescription == null)
				AuthenticationDescription = string.Empty;
			if(SharedSecret == null)
				SharedSecret = string.Empty;
			if(EstablishedTrustID == null)
				EstablishedTrustID = string.Empty;
			if(NegotiationURL == null)
				NegotiationURL = string.Empty;
			if(ConnectionURL == null)
				ConnectionURL = string.Empty;
		}
	}
    [Table(Name = "DeviceMembership")]
	[ScaffoldTable(true)]
	public class DeviceMembership : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeviceMembership](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[DeviceDescription] TEXT NOT NULL, 
[SharedSecret] TEXT NOT NULL, 
[ActiveSymmetricAESKey] BLOB NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
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


		public DeviceMembership() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string DeviceDescription { get; set; }
		// private string _unmodified_DeviceDescription;

		[Column]
        [ScaffoldColumn(true)]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
        [ScaffoldColumn(true)]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(DeviceDescription == null)
				DeviceDescription = string.Empty;
			if(SharedSecret == null)
				SharedSecret = string.Empty;
		}
	}
    [Table(Name = "InvoiceFiscalExportSummary")]
	[ScaffoldTable(true)]
	public class InvoiceFiscalExportSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceFiscalExportSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[FiscalInclusiveStartDate] TEXT NOT NULL, 
[FiscalInclusiveEndDate] TEXT NOT NULL, 
[ExportedInvoicesID] TEXT NULL
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


		public InvoiceFiscalExportSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime FiscalInclusiveStartDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveStartDate;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime FiscalInclusiveEndDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveEndDate;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "InvoiceSummaryContainer")]
	[ScaffoldTable(true)]
	public class InvoiceSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OpenInvoicesID] TEXT NULL, 
[PredictedInvoicesID] TEXT NULL, 
[PaidInvoicesActiveYearID] TEXT NULL, 
[PaidInvoicesLast12MonthsID] TEXT NULL
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


		public InvoiceSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Invoice")]
	[ScaffoldTable(true)]
	public class Invoice : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Invoice](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[InvoiceName] TEXT NOT NULL, 
[InvoiceID] TEXT NOT NULL, 
[InvoicedAmount] TEXT NOT NULL, 
[CreateDate] TEXT NOT NULL, 
[DueDate] TEXT NOT NULL, 
[PaidAmount] TEXT NOT NULL, 
[FeesAndInterestAmount] TEXT NOT NULL, 
[UnpaidAmount] TEXT NOT NULL, 
[InvoiceDetailsID] TEXT NULL, 
[InvoiceUsersID] TEXT NULL
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


		public Invoice() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string InvoiceName { get; set; }
		// private string _unmodified_InvoiceName;

		[Column]
        [ScaffoldColumn(true)]
		public string InvoiceID { get; set; }
		// private string _unmodified_InvoiceID;

		[Column]
        [ScaffoldColumn(true)]
		public string InvoicedAmount { get; set; }
		// private string _unmodified_InvoicedAmount;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime CreateDate { get; set; }
		// private DateTime _unmodified_CreateDate;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime DueDate { get; set; }
		// private DateTime _unmodified_DueDate;

		[Column]
        [ScaffoldColumn(true)]
		public string PaidAmount { get; set; }
		// private string _unmodified_PaidAmount;

		[Column]
        [ScaffoldColumn(true)]
		public string FeesAndInterestAmount { get; set; }
		// private string _unmodified_FeesAndInterestAmount;

		[Column]
        [ScaffoldColumn(true)]
		public string UnpaidAmount { get; set; }
		// private string _unmodified_UnpaidAmount;
			[Column]
			public string InvoiceDetailsID { get; set; }
			private EntityRef< InvoiceDetails > _InvoiceDetails;
			[Association(Storage = "_InvoiceDetails", ThisKey = "InvoiceDetailsID")]
			public InvoiceDetails InvoiceDetails
			{
				get { return this._InvoiceDetails.Entity; }
				set { this._InvoiceDetails.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(InvoiceName == null)
				InvoiceName = string.Empty;
			if(InvoiceID == null)
				InvoiceID = string.Empty;
			if(InvoicedAmount == null)
				InvoicedAmount = string.Empty;
			if(PaidAmount == null)
				PaidAmount = string.Empty;
			if(FeesAndInterestAmount == null)
				FeesAndInterestAmount = string.Empty;
			if(UnpaidAmount == null)
				UnpaidAmount = string.Empty;
		}
	}
    [Table(Name = "InvoiceDetails")]
	[ScaffoldTable(true)]
	public class InvoiceDetails : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceDetails](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[MonthlyFeesTotal] TEXT NOT NULL, 
[OneTimeFeesTotal] TEXT NOT NULL, 
[UsageFeesTotal] TEXT NOT NULL, 
[InterestFeesTotal] TEXT NOT NULL, 
[PenaltyFeesTotal] TEXT NOT NULL, 
[TotalFeesTotal] TEXT NOT NULL
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


		public InvoiceDetails() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string MonthlyFeesTotal { get; set; }
		// private string _unmodified_MonthlyFeesTotal;

		[Column]
        [ScaffoldColumn(true)]
		public string OneTimeFeesTotal { get; set; }
		// private string _unmodified_OneTimeFeesTotal;

		[Column]
        [ScaffoldColumn(true)]
		public string UsageFeesTotal { get; set; }
		// private string _unmodified_UsageFeesTotal;

		[Column]
        [ScaffoldColumn(true)]
		public string InterestFeesTotal { get; set; }
		// private string _unmodified_InterestFeesTotal;

		[Column]
        [ScaffoldColumn(true)]
		public string PenaltyFeesTotal { get; set; }
		// private string _unmodified_PenaltyFeesTotal;

		[Column]
        [ScaffoldColumn(true)]
		public string TotalFeesTotal { get; set; }
		// private string _unmodified_TotalFeesTotal;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(MonthlyFeesTotal == null)
				MonthlyFeesTotal = string.Empty;
			if(OneTimeFeesTotal == null)
				OneTimeFeesTotal = string.Empty;
			if(UsageFeesTotal == null)
				UsageFeesTotal = string.Empty;
			if(InterestFeesTotal == null)
				InterestFeesTotal = string.Empty;
			if(PenaltyFeesTotal == null)
				PenaltyFeesTotal = string.Empty;
			if(TotalFeesTotal == null)
				TotalFeesTotal = string.Empty;
		}
	}
    [Table(Name = "InvoiceUser")]
	[ScaffoldTable(true)]
	public class InvoiceUser : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceUser](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[UserName] TEXT NOT NULL, 
[UserID] TEXT NOT NULL, 
[UserPhoneNumber] TEXT NOT NULL, 
[UserSubscriptionNumber] TEXT NOT NULL, 
[UserInvoiceTotalAmount] TEXT NOT NULL, 
[InvoiceRowGroupCollectionID] TEXT NULL, 
[InvoiceEventDetailGroupCollectionID] TEXT NULL
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


		public InvoiceUser() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string UserName { get; set; }
		// private string _unmodified_UserName;

		[Column]
        [ScaffoldColumn(true)]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
        [ScaffoldColumn(true)]
		public string UserPhoneNumber { get; set; }
		// private string _unmodified_UserPhoneNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string UserSubscriptionNumber { get; set; }
		// private string _unmodified_UserSubscriptionNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string UserInvoiceTotalAmount { get; set; }
		// private string _unmodified_UserInvoiceTotalAmount;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UserName == null)
				UserName = string.Empty;
			if(UserID == null)
				UserID = string.Empty;
			if(UserPhoneNumber == null)
				UserPhoneNumber = string.Empty;
			if(UserSubscriptionNumber == null)
				UserSubscriptionNumber = string.Empty;
			if(UserInvoiceTotalAmount == null)
				UserInvoiceTotalAmount = string.Empty;
		}
	}
    [Table(Name = "InvoiceRowGroup")]
	[ScaffoldTable(true)]
	public class InvoiceRowGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRowGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[GroupName] TEXT NOT NULL, 
[GroupTotalPriceWithoutTaxes] TEXT NOT NULL, 
[GroupTotalTaxes] TEXT NOT NULL, 
[GroupTotalPriceWithTaxes] TEXT NOT NULL, 
[InvoiceRowCollectionID] TEXT NULL
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


		public InvoiceRowGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupTotalPriceWithoutTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithoutTaxes;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupTotalTaxes { get; set; }
		// private string _unmodified_GroupTotalTaxes;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupTotalPriceWithTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithTaxes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupName == null)
				GroupName = string.Empty;
			if(GroupTotalPriceWithoutTaxes == null)
				GroupTotalPriceWithoutTaxes = string.Empty;
			if(GroupTotalTaxes == null)
				GroupTotalTaxes = string.Empty;
			if(GroupTotalPriceWithTaxes == null)
				GroupTotalPriceWithTaxes = string.Empty;
		}
	}
    [Table(Name = "InvoiceEventDetailGroup")]
	[ScaffoldTable(true)]
	public class InvoiceEventDetailGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetailGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[GroupName] TEXT NOT NULL, 
[InvoiceEventDetailCollectionID] TEXT NULL
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


		public InvoiceEventDetailGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupName == null)
				GroupName = string.Empty;
		}
	}
    [Table(Name = "InvoiceEventDetail")]
	[ScaffoldTable(true)]
	public class InvoiceEventDetail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[IndentMode] TEXT NOT NULL, 
[EventStartDateTime] TEXT NOT NULL, 
[EventEndDateTime] TEXT NOT NULL, 
[ReceivingParty] TEXT NOT NULL, 
[AmountOfUnits] TEXT NOT NULL, 
[Duration] TEXT NOT NULL, 
[UnitPrice] TEXT NOT NULL, 
[PriceWithoutTaxes] TEXT NOT NULL, 
[Taxes] TEXT NOT NULL, 
[PriceWithTaxes] TEXT NOT NULL
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


		public InvoiceEventDetail() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime EventStartDateTime { get; set; }
		// private DateTime _unmodified_EventStartDateTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime EventEndDateTime { get; set; }
		// private DateTime _unmodified_EventEndDateTime;

		[Column]
        [ScaffoldColumn(true)]
		public string ReceivingParty { get; set; }
		// private string _unmodified_ReceivingParty;

		[Column]
        [ScaffoldColumn(true)]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
        [ScaffoldColumn(true)]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
        [ScaffoldColumn(true)]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
        [ScaffoldColumn(true)]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
        [ScaffoldColumn(true)]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
        [ScaffoldColumn(true)]
		public string PriceWithTaxes { get; set; }
		// private string _unmodified_PriceWithTaxes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(IndentMode == null)
				IndentMode = string.Empty;
			if(ReceivingParty == null)
				ReceivingParty = string.Empty;
			if(AmountOfUnits == null)
				AmountOfUnits = string.Empty;
			if(Duration == null)
				Duration = string.Empty;
			if(UnitPrice == null)
				UnitPrice = string.Empty;
			if(PriceWithoutTaxes == null)
				PriceWithoutTaxes = string.Empty;
			if(Taxes == null)
				Taxes = string.Empty;
			if(PriceWithTaxes == null)
				PriceWithTaxes = string.Empty;
		}
	}
    [Table(Name = "InvoiceRow")]
	[ScaffoldTable(true)]
	public class InvoiceRow : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRow](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[IndentMode] TEXT NOT NULL, 
[AmountOfUnits] TEXT NOT NULL, 
[Duration] TEXT NOT NULL, 
[UnitPrice] TEXT NOT NULL, 
[PriceWithoutTaxes] TEXT NOT NULL, 
[Taxes] TEXT NOT NULL, 
[PriceWithTaxes] TEXT NOT NULL
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


		public InvoiceRow() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
        [ScaffoldColumn(true)]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
        [ScaffoldColumn(true)]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
        [ScaffoldColumn(true)]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
        [ScaffoldColumn(true)]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
        [ScaffoldColumn(true)]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
        [ScaffoldColumn(true)]
		public string PriceWithTaxes { get; set; }
		// private string _unmodified_PriceWithTaxes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(IndentMode == null)
				IndentMode = string.Empty;
			if(AmountOfUnits == null)
				AmountOfUnits = string.Empty;
			if(Duration == null)
				Duration = string.Empty;
			if(UnitPrice == null)
				UnitPrice = string.Empty;
			if(PriceWithoutTaxes == null)
				PriceWithoutTaxes = string.Empty;
			if(Taxes == null)
				Taxes = string.Empty;
			if(PriceWithTaxes == null)
				PriceWithTaxes = string.Empty;
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
[CategoryName] TEXT NOT NULL
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
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CategoryName == null)
				CategoryName = string.Empty;
		}
	}
    [Table(Name = "ProcessContainer")]
	[ScaffoldTable(true)]
	public class ProcessContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ProcessIDs] TEXT NOT NULL
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


		public ProcessContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        [Column(Name = "ProcessIDs")] 
        [ScaffoldColumn(true)]
		public string ProcessIDsData { get; set; }

        private bool _IsProcessIDsRetrieved = false;
        private bool _IsProcessIDsChanged = false;
        private ObservableCollection<string> _ProcessIDs = null;
        public ObservableCollection<string> ProcessIDs
        {
            get
            {
                if (!_IsProcessIDsRetrieved)
                {
                    if (ProcessIDsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ProcessIDsData);
                        _ProcessIDs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ProcessIDs = new ObservableCollection<string>();
						ProcessIDsData = Guid.NewGuid().ToString();
						_IsProcessIDsChanged = true;
                    }
                    _IsProcessIDsRetrieved = true;
                    _ProcessIDs.CollectionChanged += (sender, args) =>
						{
							ProcessIDsData = Guid.NewGuid().ToString();
							_IsProcessIDsChanged = true;
						};
                }
                return _ProcessIDs;
            }
            set 
			{ 
				_ProcessIDs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsProcessIDsRetrieved = true;
                ProcessIDsData = Guid.NewGuid().ToString();
                _IsProcessIDsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsProcessIDsChanged || isInitialInsert)
            {
                var dataToStore = ProcessIDs.ToArray();
                ProcessIDsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "Process")]
	[ScaffoldTable(true)]
	public class Process : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Process](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ProcessDescription] TEXT NOT NULL, 
[ExecutingOperationID] TEXT NULL, 
[InitialArgumentsID] TEXT NULL, 
[ProcessItemsID] TEXT NULL
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


		public Process() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ProcessDescription { get; set; }
		// private string _unmodified_ProcessDescription;
			[Column]
			public string ExecutingOperationID { get; set; }
			private EntityRef< SemanticInformationItem > _ExecutingOperation;
			[Association(Storage = "_ExecutingOperation", ThisKey = "ExecutingOperationID")]
			public SemanticInformationItem ExecutingOperation
			{
				get { return this._ExecutingOperation.Entity; }
				set { this._ExecutingOperation.Entity = value; }
			}

        [Column(Name = "InitialArguments")] 
        [ScaffoldColumn(true)]
		public string InitialArgumentsData { get; set; }

        private bool _IsInitialArgumentsRetrieved = false;
        private bool _IsInitialArgumentsChanged = false;
        private ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> _InitialArguments = null;
        public ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> InitialArguments
        {
            get
            {
                if (!_IsInitialArgumentsRetrieved)
                {
                    if (InitialArgumentsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.CORE.SemanticInformationItem[]>(InitialArgumentsData);
                        _InitialArguments = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _InitialArguments = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>();
						InitialArgumentsData = Guid.NewGuid().ToString();
						_IsInitialArgumentsChanged = true;
                    }
                    _IsInitialArgumentsRetrieved = true;
                    _InitialArguments.CollectionChanged += (sender, args) =>
						{
							InitialArgumentsData = Guid.NewGuid().ToString();
							_IsInitialArgumentsChanged = true;
						};
                }
                return _InitialArguments;
            }
            set 
			{ 
				_InitialArguments = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsInitialArgumentsRetrieved = true;
                InitialArgumentsData = Guid.NewGuid().ToString();
                _IsInitialArgumentsChanged = true;

			}
        }

        [Column(Name = "ProcessItems")] 
        [ScaffoldColumn(true)]
		public string ProcessItemsData { get; set; }

        private bool _IsProcessItemsRetrieved = false;
        private bool _IsProcessItemsChanged = false;
        private ObservableCollection<SER.TheBall.CORE.ProcessItem> _ProcessItems = null;
        public ObservableCollection<SER.TheBall.CORE.ProcessItem> ProcessItems
        {
            get
            {
                if (!_IsProcessItemsRetrieved)
                {
                    if (ProcessItemsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.CORE.ProcessItem[]>(ProcessItemsData);
                        _ProcessItems = new ObservableCollection<SER.TheBall.CORE.ProcessItem>(arrayData);
                    }
                    else
                    {
                        _ProcessItems = new ObservableCollection<SER.TheBall.CORE.ProcessItem>();
						ProcessItemsData = Guid.NewGuid().ToString();
						_IsProcessItemsChanged = true;
                    }
                    _IsProcessItemsRetrieved = true;
                    _ProcessItems.CollectionChanged += (sender, args) =>
						{
							ProcessItemsData = Guid.NewGuid().ToString();
							_IsProcessItemsChanged = true;
						};
                }
                return _ProcessItems;
            }
            set 
			{ 
				_ProcessItems = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsProcessItemsRetrieved = true;
                ProcessItemsData = Guid.NewGuid().ToString();
                _IsProcessItemsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ProcessDescription == null)
				ProcessDescription = string.Empty;
            if (_IsInitialArgumentsChanged || isInitialInsert)
            {
                var dataToStore = InitialArguments.ToArray();
                InitialArgumentsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsProcessItemsChanged || isInitialInsert)
            {
                var dataToStore = ProcessItems.ToArray();
                ProcessItemsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "ProcessItem")]
	[ScaffoldTable(true)]
	public class ProcessItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OutputsID] TEXT NULL, 
[InputsID] TEXT NULL
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


		public ProcessItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        [Column(Name = "Outputs")] 
        [ScaffoldColumn(true)]
		public string OutputsData { get; set; }

        private bool _IsOutputsRetrieved = false;
        private bool _IsOutputsChanged = false;
        private ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> _Outputs = null;
        public ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> Outputs
        {
            get
            {
                if (!_IsOutputsRetrieved)
                {
                    if (OutputsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.CORE.SemanticInformationItem[]>(OutputsData);
                        _Outputs = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _Outputs = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>();
						OutputsData = Guid.NewGuid().ToString();
						_IsOutputsChanged = true;
                    }
                    _IsOutputsRetrieved = true;
                    _Outputs.CollectionChanged += (sender, args) =>
						{
							OutputsData = Guid.NewGuid().ToString();
							_IsOutputsChanged = true;
						};
                }
                return _Outputs;
            }
            set 
			{ 
				_Outputs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsOutputsRetrieved = true;
                OutputsData = Guid.NewGuid().ToString();
                _IsOutputsChanged = true;

			}
        }

        [Column(Name = "Inputs")] 
        [ScaffoldColumn(true)]
		public string InputsData { get; set; }

        private bool _IsInputsRetrieved = false;
        private bool _IsInputsChanged = false;
        private ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> _Inputs = null;
        public ObservableCollection<SER.TheBall.CORE.SemanticInformationItem> Inputs
        {
            get
            {
                if (!_IsInputsRetrieved)
                {
                    if (InputsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SER.TheBall.CORE.SemanticInformationItem[]>(InputsData);
                        _Inputs = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _Inputs = new ObservableCollection<SER.TheBall.CORE.SemanticInformationItem>();
						InputsData = Guid.NewGuid().ToString();
						_IsInputsChanged = true;
                    }
                    _IsInputsRetrieved = true;
                    _Inputs.CollectionChanged += (sender, args) =>
						{
							InputsData = Guid.NewGuid().ToString();
							_IsInputsChanged = true;
						};
                }
                return _Inputs;
            }
            set 
			{ 
				_Inputs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsInputsRetrieved = true;
                InputsData = Guid.NewGuid().ToString();
                _IsInputsChanged = true;

			}
        }

        public void PrepareForStoring(bool isInitialInsert)
        {
		
            if (_IsOutputsChanged || isInitialInsert)
            {
                var dataToStore = Outputs.ToArray();
                OutputsData = JsonConvert.SerializeObject(dataToStore);
            }

            if (_IsInputsChanged || isInitialInsert)
            {
                var dataToStore = Inputs.ToArray();
                InputsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "SemanticInformationItem")]
	[ScaffoldTable(true)]
	public class SemanticInformationItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SemanticInformationItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[ItemFullType] TEXT NOT NULL, 
[ItemValue] TEXT NOT NULL
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


		public SemanticInformationItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ItemFullType { get; set; }
		// private string _unmodified_ItemFullType;

		[Column]
        [ScaffoldColumn(true)]
		public string ItemValue { get; set; }
		// private string _unmodified_ItemValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ItemFullType == null)
				ItemFullType = string.Empty;
			if(ItemValue == null)
				ItemValue = string.Empty;
		}
	}
    [Table(Name = "InformationOwnerInfo")]
	[ScaffoldTable(true)]
	public class InformationOwnerInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationOwnerInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OwnerType] TEXT NOT NULL, 
[OwnerIdentifier] TEXT NOT NULL
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


		public InformationOwnerInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OwnerType { get; set; }
		// private string _unmodified_OwnerType;

		[Column]
        [ScaffoldColumn(true)]
		public string OwnerIdentifier { get; set; }
		// private string _unmodified_OwnerIdentifier;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OwnerType == null)
				OwnerType = string.Empty;
			if(OwnerIdentifier == null)
				OwnerIdentifier = string.Empty;
		}
	}
    [Table(Name = "UsageSummary")]
	[ScaffoldTable(true)]
	public class UsageSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UsageSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[SummaryName] TEXT NOT NULL, 
[SummaryMonitoringItemID] TEXT NULL
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


		public UsageSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SummaryName { get; set; }
		// private string _unmodified_SummaryName;
			[Column]
			public string SummaryMonitoringItemID { get; set; }
			private EntityRef< UsageMonitorItem > _SummaryMonitoringItem;
			[Association(Storage = "_SummaryMonitoringItem", ThisKey = "SummaryMonitoringItemID")]
			public UsageMonitorItem SummaryMonitoringItem
			{
				get { return this._SummaryMonitoringItem.Entity; }
				set { this._SummaryMonitoringItem.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryName == null)
				SummaryName = string.Empty;
		}
	}
    [Table(Name = "UsageMonitorItem")]
	[ScaffoldTable(true)]
	public class UsageMonitorItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UsageMonitorItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OwnerInfoID] TEXT NULL, 
[TimeRangeInclusiveStartExclusiveEndID] TEXT NULL, 
[StepSizeInMinutes] INTEGER NOT NULL, 
[ProcessorUsagesID] TEXT NULL, 
[StorageTransactionUsagesID] TEXT NULL, 
[StorageUsagesID] TEXT NULL, 
[NetworkUsagesID] TEXT NULL
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


		public UsageMonitorItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string OwnerInfoID { get; set; }
			private EntityRef< InformationOwnerInfo > _OwnerInfo;
			[Association(Storage = "_OwnerInfo", ThisKey = "OwnerInfoID")]
			public InformationOwnerInfo OwnerInfo
			{
				get { return this._OwnerInfo.Entity; }
				set { this._OwnerInfo.Entity = value; }
			}

			[Column]
			public string TimeRangeInclusiveStartExclusiveEndID { get; set; }
			private EntityRef< TimeRange > _TimeRangeInclusiveStartExclusiveEnd;
			[Association(Storage = "_TimeRangeInclusiveStartExclusiveEnd", ThisKey = "TimeRangeInclusiveStartExclusiveEndID")]
			public TimeRange TimeRangeInclusiveStartExclusiveEnd
			{
				get { return this._TimeRangeInclusiveStartExclusiveEnd.Entity; }
				set { this._TimeRangeInclusiveStartExclusiveEnd.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public long StepSizeInMinutes { get; set; }
		// private long _unmodified_StepSizeInMinutes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RequestResourceUsage")]
	[ScaffoldTable(true)]
	public class RequestResourceUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RequestResourceUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[OwnerInfoID] TEXT NULL, 
[ProcessorUsageID] TEXT NULL, 
[StorageTransactionUsageID] TEXT NULL, 
[NetworkUsageID] TEXT NULL, 
[RequestDetailsID] TEXT NULL
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


		public RequestResourceUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string OwnerInfoID { get; set; }
			private EntityRef< InformationOwnerInfo > _OwnerInfo;
			[Association(Storage = "_OwnerInfo", ThisKey = "OwnerInfoID")]
			public InformationOwnerInfo OwnerInfo
			{
				get { return this._OwnerInfo.Entity; }
				set { this._OwnerInfo.Entity = value; }
			}

			[Column]
			public string ProcessorUsageID { get; set; }
			private EntityRef< ProcessorUsage > _ProcessorUsage;
			[Association(Storage = "_ProcessorUsage", ThisKey = "ProcessorUsageID")]
			public ProcessorUsage ProcessorUsage
			{
				get { return this._ProcessorUsage.Entity; }
				set { this._ProcessorUsage.Entity = value; }
			}

			[Column]
			public string StorageTransactionUsageID { get; set; }
			private EntityRef< StorageTransactionUsage > _StorageTransactionUsage;
			[Association(Storage = "_StorageTransactionUsage", ThisKey = "StorageTransactionUsageID")]
			public StorageTransactionUsage StorageTransactionUsage
			{
				get { return this._StorageTransactionUsage.Entity; }
				set { this._StorageTransactionUsage.Entity = value; }
			}

			[Column]
			public string NetworkUsageID { get; set; }
			private EntityRef< NetworkUsage > _NetworkUsage;
			[Association(Storage = "_NetworkUsage", ThisKey = "NetworkUsageID")]
			public NetworkUsage NetworkUsage
			{
				get { return this._NetworkUsage.Entity; }
				set { this._NetworkUsage.Entity = value; }
			}

			[Column]
			public string RequestDetailsID { get; set; }
			private EntityRef< HTTPActivityDetails > _RequestDetails;
			[Association(Storage = "_RequestDetails", ThisKey = "RequestDetailsID")]
			public HTTPActivityDetails RequestDetails
			{
				get { return this._RequestDetails.Entity; }
				set { this._RequestDetails.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ProcessorUsage")]
	[ScaffoldTable(true)]
	public class ProcessorUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessorUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTicks] REAL NOT NULL, 
[FrequencyTicksPerSecond] REAL NOT NULL, 
[Milliseconds] INTEGER NOT NULL
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


		public ProcessorUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string TimeRangeID { get; set; }
			private EntityRef< TimeRange > _TimeRange;
			[Association(Storage = "_TimeRange", ThisKey = "TimeRangeID")]
			public TimeRange TimeRange
			{
				get { return this._TimeRange.Entity; }
				set { this._TimeRange.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
        [ScaffoldColumn(true)]
		public double AmountOfTicks { get; set; }
		// private double _unmodified_AmountOfTicks;

		[Column]
        [ScaffoldColumn(true)]
		public double FrequencyTicksPerSecond { get; set; }
		// private double _unmodified_FrequencyTicksPerSecond;

		[Column]
        [ScaffoldColumn(true)]
		public long Milliseconds { get; set; }
		// private long _unmodified_Milliseconds;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "StorageTransactionUsage")]
	[ScaffoldTable(true)]
	public class StorageTransactionUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageTransactionUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTransactions] INTEGER NOT NULL
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


		public StorageTransactionUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string TimeRangeID { get; set; }
			private EntityRef< TimeRange > _TimeRange;
			[Association(Storage = "_TimeRange", ThisKey = "TimeRangeID")]
			public TimeRange TimeRange
			{
				get { return this._TimeRange.Entity; }
				set { this._TimeRange.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
        [ScaffoldColumn(true)]
		public long AmountOfTransactions { get; set; }
		// private long _unmodified_AmountOfTransactions;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "StorageUsage")]
	[ScaffoldTable(true)]
	public class StorageUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[SnapshotTime] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[UsageUnit] TEXT NOT NULL, 
[AmountOfUnits] REAL NOT NULL
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


		public StorageUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime SnapshotTime { get; set; }
		// private DateTime _unmodified_SnapshotTime;

		[Column]
        [ScaffoldColumn(true)]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
        [ScaffoldColumn(true)]
		public string UsageUnit { get; set; }
		// private string _unmodified_UsageUnit;

		[Column]
        [ScaffoldColumn(true)]
		public double AmountOfUnits { get; set; }
		// private double _unmodified_AmountOfUnits;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
			if(UsageUnit == null)
				UsageUnit = string.Empty;
		}
	}
    [Table(Name = "NetworkUsage")]
	[ScaffoldTable(true)]
	public class NetworkUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NetworkUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfBytes] INTEGER NOT NULL
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


		public NetworkUsage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string TimeRangeID { get; set; }
			private EntityRef< TimeRange > _TimeRange;
			[Association(Storage = "_TimeRange", ThisKey = "TimeRangeID")]
			public TimeRange TimeRange
			{
				get { return this._TimeRange.Entity; }
				set { this._TimeRange.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
        [ScaffoldColumn(true)]
		public long AmountOfBytes { get; set; }
		// private long _unmodified_AmountOfBytes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "TimeRange")]
	[ScaffoldTable(true)]
	public class TimeRange : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TimeRange](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[StartTime] TEXT NOT NULL, 
[EndTime] TEXT NOT NULL
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


		public TimeRange() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime StartTime { get; set; }
		// private DateTime _unmodified_StartTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime EndTime { get; set; }
		// private DateTime _unmodified_EndTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "HTTPActivityDetails")]
	[ScaffoldTable(true)]
	public class HTTPActivityDetails : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [HTTPActivityDetails](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL,
[RemoteIPAddress] TEXT NOT NULL, 
[RemoteEndpointUserName] TEXT NOT NULL, 
[UserID] TEXT NOT NULL, 
[UTCDateTime] TEXT NOT NULL, 
[RequestLine] TEXT NOT NULL, 
[HTTPStatusCode] INTEGER NOT NULL, 
[ReturnedContentLength] INTEGER NOT NULL
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


		public HTTPActivityDetails() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string RemoteIPAddress { get; set; }
		// private string _unmodified_RemoteIPAddress;

		[Column]
        [ScaffoldColumn(true)]
		public string RemoteEndpointUserName { get; set; }
		// private string _unmodified_RemoteEndpointUserName;

		[Column]
        [ScaffoldColumn(true)]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime UTCDateTime { get; set; }
		// private DateTime _unmodified_UTCDateTime;

		[Column]
        [ScaffoldColumn(true)]
		public string RequestLine { get; set; }
		// private string _unmodified_RequestLine;

		[Column]
        [ScaffoldColumn(true)]
		public long HTTPStatusCode { get; set; }
		// private long _unmodified_HTTPStatusCode;

		[Column]
        [ScaffoldColumn(true)]
		public long ReturnedContentLength { get; set; }
		// private long _unmodified_ReturnedContentLength;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(RemoteIPAddress == null)
				RemoteIPAddress = string.Empty;
			if(RemoteEndpointUserName == null)
				RemoteEndpointUserName = string.Empty;
			if(UserID == null)
				UserID = string.Empty;
			if(RequestLine == null)
				RequestLine = string.Empty;
		}
	}
 } 
