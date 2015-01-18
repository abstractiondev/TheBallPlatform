 


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


namespace SQLite.TheBall.CORE { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		public class TheBallDataContext : DataContext, IStorageSyncableDataContext
		{

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
                if(updateData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
		        if (updateData.ObjectType == "ContentPackage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.ContentPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContentPackageTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.InformationInput.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationInputTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.InformationOutput.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationOutputTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.AuthenticatedAsActiveDevice.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AuthenticatedAsActiveDeviceTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.DeviceMembership.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DeviceMembershipTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.InvoiceFiscalExportSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceFiscalExportSummaryTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            existingObject.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
		            existingObject.ExportedInvoices = serializedObject.ExportedInvoices;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceSummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InvoiceSummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.OpenInvoices = serializedObject.OpenInvoices;
		            existingObject.PredictedInvoices = serializedObject.PredictedInvoices;
		            existingObject.PaidInvoicesActiveYear = serializedObject.PaidInvoicesActiveYear;
		            existingObject.PaidInvoicesLast12Months = serializedObject.PaidInvoicesLast12Months;
		            return;
		        } 
		        if (updateData.ObjectType == "Invoice")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.Invoice.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.InvoiceName = serializedObject.InvoiceName;
		            existingObject.InvoiceID = serializedObject.InvoiceID;
		            existingObject.InvoicedAmount = serializedObject.InvoicedAmount;
		            existingObject.CreateDate = serializedObject.CreateDate;
		            existingObject.DueDate = serializedObject.DueDate;
		            existingObject.PaidAmount = serializedObject.PaidAmount;
		            existingObject.FeesAndInterestAmount = serializedObject.FeesAndInterestAmount;
		            existingObject.UnpaidAmount = serializedObject.UnpaidAmount;
		            existingObject.InvoiceDetails = serializedObject.InvoiceDetails;
		            existingObject.InvoiceUsers = serializedObject.InvoiceUsers;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceDetails")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InvoiceDetails.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceDetailsTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.InvoiceUser.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceUserTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.UserName = serializedObject.UserName;
		            existingObject.UserID = serializedObject.UserID;
		            existingObject.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            existingObject.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            existingObject.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
		            existingObject.InvoiceRowGroupCollection = serializedObject.InvoiceRowGroupCollection;
		            existingObject.InvoiceEventDetailGroupCollection = serializedObject.InvoiceEventDetailGroupCollection;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceRowGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InvoiceRowGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceRowGroupTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            existingObject.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            existingObject.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
		            existingObject.InvoiceRowCollection = serializedObject.InvoiceRowCollection;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceEventDetailGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InvoiceEventDetailGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailGroupTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.InvoiceEventDetailCollection = serializedObject.InvoiceEventDetailCollection;
		            return;
		        } 
		        if (updateData.ObjectType == "InvoiceEventDetail")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InvoiceEventDetail.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.InvoiceRow.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceRowTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.Category.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.CategoryName = serializedObject.CategoryName;
		            return;
		        } 
		        if (updateData.ObjectType == "ProcessContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.ProcessContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessContainerTable.Single(item => item.ID == updateData.ObjectID);
                    existingObject.ProcessIDs.Clear();
					if(serializedObject.ProcessIDs != null)
	                    serializedObject.ProcessIDs.ForEach(item => existingObject.ProcessIDs.Add(item));
					
		            return;
		        } 
		        if (updateData.ObjectType == "Process")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.Process.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ProcessDescription = serializedObject.ProcessDescription;
		            existingObject.ExecutingOperation = serializedObject.ExecutingOperation;
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
		                global::TheBall.Payments.ProcessItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessItemTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.SemanticInformationItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SemanticInformationItemTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ItemFullType = serializedObject.ItemFullType;
		            existingObject.ItemValue = serializedObject.ItemValue;
		            return;
		        } 
		        if (updateData.ObjectType == "InformationOwnerInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.InformationOwnerInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationOwnerInfoTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.OwnerType = serializedObject.OwnerType;
		            existingObject.OwnerIdentifier = serializedObject.OwnerIdentifier;
		            return;
		        } 
		        if (updateData.ObjectType == "UsageSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.UsageSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UsageSummaryTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.SummaryName = serializedObject.SummaryName;
		            existingObject.SummaryMonitoringItem = serializedObject.SummaryMonitoringItem;
		            return;
		        } 
		        if (updateData.ObjectType == "UsageMonitorItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.UsageMonitorItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UsageMonitorItemTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.OwnerInfo = serializedObject.OwnerInfo;
		            existingObject.TimeRangeInclusiveStartExclusiveEnd = serializedObject.TimeRangeInclusiveStartExclusiveEnd;
		            existingObject.StepSizeInMinutes = serializedObject.StepSizeInMinutes;
		            existingObject.ProcessorUsages = serializedObject.ProcessorUsages;
		            existingObject.StorageTransactionUsages = serializedObject.StorageTransactionUsages;
		            existingObject.StorageUsages = serializedObject.StorageUsages;
		            existingObject.NetworkUsages = serializedObject.NetworkUsages;
		            return;
		        } 
		        if (updateData.ObjectType == "RequestResourceUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.RequestResourceUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RequestResourceUsageTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.OwnerInfo = serializedObject.OwnerInfo;
		            existingObject.ProcessorUsage = serializedObject.ProcessorUsage;
		            existingObject.StorageTransactionUsage = serializedObject.StorageTransactionUsage;
		            existingObject.NetworkUsage = serializedObject.NetworkUsage;
		            existingObject.RequestDetails = serializedObject.RequestDetails;
		            return;
		        } 
		        if (updateData.ObjectType == "ProcessorUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.ProcessorUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProcessorUsageTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.TimeRange = serializedObject.TimeRange;
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
		                global::TheBall.Payments.StorageTransactionUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StorageTransactionUsageTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.TimeRange = serializedObject.TimeRange;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfTransactions = serializedObject.AmountOfTransactions;
		            return;
		        } 
		        if (updateData.ObjectType == "StorageUsage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.StorageUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StorageUsageTable.Single(item => item.ID == updateData.ObjectID);
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
		                global::TheBall.Payments.NetworkUsage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NetworkUsageTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.TimeRange = serializedObject.TimeRange;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfBytes = serializedObject.AmountOfBytes;
		            return;
		        } 
		        if (updateData.ObjectType == "TimeRange")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.TimeRange.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TimeRangeTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.StartTime = serializedObject.StartTime;
		            existingObject.EndTime = serializedObject.EndTime;
		            return;
		        } 
		        if (updateData.ObjectType == "HTTPActivityDetails")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::TheBall.Payments.HTTPActivityDetails.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = HTTPActivityDetailsTable.Single(item => item.ID == updateData.ObjectID);
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
                if (insertData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "ContentPackage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.ContentPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContentPackage {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.InformationInput.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationInput {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.InformationOutput.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationOutput {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.AuthenticatedAsActiveDevice.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AuthenticatedAsActiveDevice {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.DeviceMembership.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DeviceMembership {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.InvoiceFiscalExportSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceFiscalExportSummary {ID = insertData.ObjectID};
		            objectToAdd.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            objectToAdd.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
		            objectToAdd.ExportedInvoices = serializedObject.ExportedInvoices;
					InvoiceFiscalExportSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceSummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InvoiceSummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceSummaryContainer {ID = insertData.ObjectID};
		            objectToAdd.OpenInvoices = serializedObject.OpenInvoices;
		            objectToAdd.PredictedInvoices = serializedObject.PredictedInvoices;
		            objectToAdd.PaidInvoicesActiveYear = serializedObject.PaidInvoicesActiveYear;
		            objectToAdd.PaidInvoicesLast12Months = serializedObject.PaidInvoicesLast12Months;
					InvoiceSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Invoice")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.Invoice.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Invoice {ID = insertData.ObjectID};
		            objectToAdd.InvoiceName = serializedObject.InvoiceName;
		            objectToAdd.InvoiceID = serializedObject.InvoiceID;
		            objectToAdd.InvoicedAmount = serializedObject.InvoicedAmount;
		            objectToAdd.CreateDate = serializedObject.CreateDate;
		            objectToAdd.DueDate = serializedObject.DueDate;
		            objectToAdd.PaidAmount = serializedObject.PaidAmount;
		            objectToAdd.FeesAndInterestAmount = serializedObject.FeesAndInterestAmount;
		            objectToAdd.UnpaidAmount = serializedObject.UnpaidAmount;
		            objectToAdd.InvoiceDetails = serializedObject.InvoiceDetails;
		            objectToAdd.InvoiceUsers = serializedObject.InvoiceUsers;
					InvoiceTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceDetails")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InvoiceDetails.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceDetails {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.InvoiceUser.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceUser {ID = insertData.ObjectID};
		            objectToAdd.UserName = serializedObject.UserName;
		            objectToAdd.UserID = serializedObject.UserID;
		            objectToAdd.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            objectToAdd.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            objectToAdd.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
		            objectToAdd.InvoiceRowGroupCollection = serializedObject.InvoiceRowGroupCollection;
		            objectToAdd.InvoiceEventDetailGroupCollection = serializedObject.InvoiceEventDetailGroupCollection;
					InvoiceUserTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceRowGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InvoiceRowGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceRowGroup {ID = insertData.ObjectID};
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            objectToAdd.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            objectToAdd.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
		            objectToAdd.InvoiceRowCollection = serializedObject.InvoiceRowCollection;
					InvoiceRowGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceEventDetailGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InvoiceEventDetailGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetailGroup {ID = insertData.ObjectID};
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.InvoiceEventDetailCollection = serializedObject.InvoiceEventDetailCollection;
					InvoiceEventDetailGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InvoiceEventDetail")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InvoiceEventDetail.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetail {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.InvoiceRow.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceRow {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.Category.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProcessContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.ProcessContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessContainer {ID = insertData.ObjectID};
					if(serializedObject.ProcessIDs != null)
						serializedObject.ProcessIDs.ForEach(item => objectToAdd.ProcessIDs.Add(item));
					ProcessContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Process")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.Process.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Process {ID = insertData.ObjectID};
		            objectToAdd.ProcessDescription = serializedObject.ProcessDescription;
		            objectToAdd.ExecutingOperation = serializedObject.ExecutingOperation;
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
                        global::TheBall.Payments.ProcessItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessItem {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.SemanticInformationItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SemanticInformationItem {ID = insertData.ObjectID};
		            objectToAdd.ItemFullType = serializedObject.ItemFullType;
		            objectToAdd.ItemValue = serializedObject.ItemValue;
					SemanticInformationItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationOwnerInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.InformationOwnerInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationOwnerInfo {ID = insertData.ObjectID};
		            objectToAdd.OwnerType = serializedObject.OwnerType;
		            objectToAdd.OwnerIdentifier = serializedObject.OwnerIdentifier;
					InformationOwnerInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UsageSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.UsageSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UsageSummary {ID = insertData.ObjectID};
		            objectToAdd.SummaryName = serializedObject.SummaryName;
		            objectToAdd.SummaryMonitoringItem = serializedObject.SummaryMonitoringItem;
					UsageSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UsageMonitorItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.UsageMonitorItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UsageMonitorItem {ID = insertData.ObjectID};
		            objectToAdd.OwnerInfo = serializedObject.OwnerInfo;
		            objectToAdd.TimeRangeInclusiveStartExclusiveEnd = serializedObject.TimeRangeInclusiveStartExclusiveEnd;
		            objectToAdd.StepSizeInMinutes = serializedObject.StepSizeInMinutes;
		            objectToAdd.ProcessorUsages = serializedObject.ProcessorUsages;
		            objectToAdd.StorageTransactionUsages = serializedObject.StorageTransactionUsages;
		            objectToAdd.StorageUsages = serializedObject.StorageUsages;
		            objectToAdd.NetworkUsages = serializedObject.NetworkUsages;
					UsageMonitorItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RequestResourceUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.RequestResourceUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RequestResourceUsage {ID = insertData.ObjectID};
		            objectToAdd.OwnerInfo = serializedObject.OwnerInfo;
		            objectToAdd.ProcessorUsage = serializedObject.ProcessorUsage;
		            objectToAdd.StorageTransactionUsage = serializedObject.StorageTransactionUsage;
		            objectToAdd.NetworkUsage = serializedObject.NetworkUsage;
		            objectToAdd.RequestDetails = serializedObject.RequestDetails;
					RequestResourceUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProcessorUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.ProcessorUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessorUsage {ID = insertData.ObjectID};
		            objectToAdd.TimeRange = serializedObject.TimeRange;
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
                        global::TheBall.Payments.StorageTransactionUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StorageTransactionUsage {ID = insertData.ObjectID};
		            objectToAdd.TimeRange = serializedObject.TimeRange;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfTransactions = serializedObject.AmountOfTransactions;
					StorageTransactionUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "StorageUsage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.StorageUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StorageUsage {ID = insertData.ObjectID};
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
                        global::TheBall.Payments.NetworkUsage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NetworkUsage {ID = insertData.ObjectID};
		            objectToAdd.TimeRange = serializedObject.TimeRange;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfBytes = serializedObject.AmountOfBytes;
					NetworkUsageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TimeRange")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.TimeRange.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TimeRange {ID = insertData.ObjectID};
		            objectToAdd.StartTime = serializedObject.StartTime;
		            objectToAdd.EndTime = serializedObject.EndTime;
					TimeRangeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "HTTPActivityDetails")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::TheBall.Payments.HTTPActivityDetails.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new HTTPActivityDetails {ID = insertData.ObjectID};
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
                if (deleteData.SemanticDomain != "TheBall.Payments")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "ContentPackage")
		        {
                    ContentPackageTable.DeleteOnSubmit(new ContentPackage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InformationInput")
		        {
                    InformationInputTable.DeleteOnSubmit(new InformationInput { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InformationOutput")
		        {
                    InformationOutputTable.DeleteOnSubmit(new InformationOutput { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "AuthenticatedAsActiveDevice")
		        {
                    AuthenticatedAsActiveDeviceTable.DeleteOnSubmit(new AuthenticatedAsActiveDevice { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "DeviceMembership")
		        {
                    DeviceMembershipTable.DeleteOnSubmit(new DeviceMembership { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceFiscalExportSummary")
		        {
                    InvoiceFiscalExportSummaryTable.DeleteOnSubmit(new InvoiceFiscalExportSummary { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceSummaryContainer")
		        {
                    InvoiceSummaryContainerTable.DeleteOnSubmit(new InvoiceSummaryContainer { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Invoice")
		        {
                    InvoiceTable.DeleteOnSubmit(new Invoice { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceDetails")
		        {
                    InvoiceDetailsTable.DeleteOnSubmit(new InvoiceDetails { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceUser")
		        {
                    InvoiceUserTable.DeleteOnSubmit(new InvoiceUser { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceRowGroup")
		        {
                    InvoiceRowGroupTable.DeleteOnSubmit(new InvoiceRowGroup { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceEventDetailGroup")
		        {
                    InvoiceEventDetailGroupTable.DeleteOnSubmit(new InvoiceEventDetailGroup { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceEventDetail")
		        {
                    InvoiceEventDetailTable.DeleteOnSubmit(new InvoiceEventDetail { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InvoiceRow")
		        {
                    InvoiceRowTable.DeleteOnSubmit(new InvoiceRow { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Category")
		        {
                    CategoryTable.DeleteOnSubmit(new Category { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessContainer")
		        {
                    ProcessContainerTable.DeleteOnSubmit(new ProcessContainer { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "Process")
		        {
                    ProcessTable.DeleteOnSubmit(new Process { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessItem")
		        {
                    ProcessItemTable.DeleteOnSubmit(new ProcessItem { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "SemanticInformationItem")
		        {
                    SemanticInformationItemTable.DeleteOnSubmit(new SemanticInformationItem { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "InformationOwnerInfo")
		        {
                    InformationOwnerInfoTable.DeleteOnSubmit(new InformationOwnerInfo { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "UsageSummary")
		        {
                    UsageSummaryTable.DeleteOnSubmit(new UsageSummary { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "UsageMonitorItem")
		        {
                    UsageMonitorItemTable.DeleteOnSubmit(new UsageMonitorItem { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "RequestResourceUsage")
		        {
                    RequestResourceUsageTable.DeleteOnSubmit(new RequestResourceUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "ProcessorUsage")
		        {
                    ProcessorUsageTable.DeleteOnSubmit(new ProcessorUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "StorageTransactionUsage")
		        {
                    StorageTransactionUsageTable.DeleteOnSubmit(new StorageTransactionUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "StorageUsage")
		        {
                    StorageUsageTable.DeleteOnSubmit(new StorageUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "NetworkUsage")
		        {
                    NetworkUsageTable.DeleteOnSubmit(new NetworkUsage { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "TimeRange")
		        {
                    TimeRangeTable.DeleteOnSubmit(new TimeRange { ID = deleteData.ObjectID });
		            return;
		        }
		        if (deleteData.ObjectType == "HTTPActivityDetails")
		        {
                    HTTPActivityDetailsTable.DeleteOnSubmit(new HTTPActivityDetails { ID = deleteData.ObjectID });
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
	public class ContentPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ContentPackage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PackageType] TEXT NOT NULL, 
[PackageName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[PackageRootFolder] TEXT NOT NULL, 
[CreationTime] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PackageType { get; set; }
		// private string _unmodified_PackageType;

		[Column]
		public string PackageName { get; set; }
		// private string _unmodified_PackageName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string PackageRootFolder { get; set; }
		// private string _unmodified_PackageRootFolder;

		[Column]
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
	public class InformationInput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationInput(
[ID] TEXT NOT NULL PRIMARY KEY, 
[InputDescription] TEXT NOT NULL, 
[LocationURL] TEXT NOT NULL, 
[LocalContentName] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InputDescription { get; set; }
		// private string _unmodified_InputDescription;

		[Column]
		public string LocationURL { get; set; }
		// private string _unmodified_LocationURL;

		[Column]
		public string LocalContentName { get; set; }
		// private string _unmodified_LocalContentName;

		[Column]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
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
	public class InformationOutput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationOutput(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OutputDescription] TEXT NOT NULL, 
[DestinationURL] TEXT NOT NULL, 
[DestinationContentName] TEXT NOT NULL, 
[LocalContentURL] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OutputDescription { get; set; }
		// private string _unmodified_OutputDescription;

		[Column]
		public string DestinationURL { get; set; }
		// private string _unmodified_DestinationURL;

		[Column]
		public string DestinationContentName { get; set; }
		// private string _unmodified_DestinationContentName;

		[Column]
		public string LocalContentURL { get; set; }
		// private string _unmodified_LocalContentURL;

		[Column]
		public string AuthenticatedDeviceID { get; set; }
		// private string _unmodified_AuthenticatedDeviceID;

		[Column]
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
	public class AuthenticatedAsActiveDevice : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AuthenticatedAsActiveDevice(
[ID] TEXT NOT NULL PRIMARY KEY, 
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
		public string ID { get; set; }


		[Column]
		public string AuthenticationDescription { get; set; }
		// private string _unmodified_AuthenticationDescription;

		[Column]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
		public string EstablishedTrustID { get; set; }
		// private string _unmodified_EstablishedTrustID;

		[Column]
		public bool IsValidatedAndActive { get; set; }
		// private bool _unmodified_IsValidatedAndActive;

		[Column]
		public string NegotiationURL { get; set; }
		// private string _unmodified_NegotiationURL;

		[Column]
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
	public class DeviceMembership : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS DeviceMembership(
[ID] TEXT NOT NULL PRIMARY KEY, 
[DeviceDescription] TEXT NOT NULL, 
[SharedSecret] TEXT NOT NULL, 
[ActiveSymmetricAESKey] BLOB NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string DeviceDescription { get; set; }
		// private string _unmodified_DeviceDescription;

		[Column]
		public string SharedSecret { get; set; }
		// private string _unmodified_SharedSecret;

		[Column]
		public byte[] ActiveSymmetricAESKey { get; set; }
		// private byte[] _unmodified_ActiveSymmetricAESKey;

		[Column]
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
	public class InvoiceFiscalExportSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceFiscalExportSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[FiscalInclusiveStartDate] TEXT NOT NULL, 
[FiscalInclusiveEndDate] TEXT NOT NULL, 
[ExportedInvoices] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime FiscalInclusiveStartDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveStartDate;

		[Column]
		public DateTime FiscalInclusiveEndDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveEndDate;

		[Column]
		public InvoiceCollection ExportedInvoices { get; set; }
		// private InvoiceCollection _unmodified_ExportedInvoices;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "InvoiceSummaryContainer")]
	public class InvoiceSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceSummaryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OpenInvoices] TEXT NOT NULL, 
[PredictedInvoices] TEXT NOT NULL, 
[PaidInvoicesActiveYear] TEXT NOT NULL, 
[PaidInvoicesLast12Months] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InvoiceCollection OpenInvoices { get; set; }
		// private InvoiceCollection _unmodified_OpenInvoices;

		[Column]
		public InvoiceCollection PredictedInvoices { get; set; }
		// private InvoiceCollection _unmodified_PredictedInvoices;

		[Column]
		public InvoiceCollection PaidInvoicesActiveYear { get; set; }
		// private InvoiceCollection _unmodified_PaidInvoicesActiveYear;

		[Column]
		public InvoiceCollection PaidInvoicesLast12Months { get; set; }
		// private InvoiceCollection _unmodified_PaidInvoicesLast12Months;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Invoice")]
	public class Invoice : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Invoice(
[ID] TEXT NOT NULL PRIMARY KEY, 
[InvoiceName] TEXT NOT NULL, 
[InvoiceID] TEXT NOT NULL, 
[InvoicedAmount] TEXT NOT NULL, 
[CreateDate] TEXT NOT NULL, 
[DueDate] TEXT NOT NULL, 
[PaidAmount] TEXT NOT NULL, 
[FeesAndInterestAmount] TEXT NOT NULL, 
[UnpaidAmount] TEXT NOT NULL, 
[InvoiceDetails] TEXT NOT NULL, 
[InvoiceUsers] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InvoiceName { get; set; }
		// private string _unmodified_InvoiceName;

		[Column]
		public string InvoiceID { get; set; }
		// private string _unmodified_InvoiceID;

		[Column]
		public string InvoicedAmount { get; set; }
		// private string _unmodified_InvoicedAmount;

		[Column]
		public DateTime CreateDate { get; set; }
		// private DateTime _unmodified_CreateDate;

		[Column]
		public DateTime DueDate { get; set; }
		// private DateTime _unmodified_DueDate;

		[Column]
		public string PaidAmount { get; set; }
		// private string _unmodified_PaidAmount;

		[Column]
		public string FeesAndInterestAmount { get; set; }
		// private string _unmodified_FeesAndInterestAmount;

		[Column]
		public string UnpaidAmount { get; set; }
		// private string _unmodified_UnpaidAmount;

		[Column]
		public InvoiceDetails InvoiceDetails { get; set; }
		// private InvoiceDetails _unmodified_InvoiceDetails;

		[Column]
		public InvoiceUserCollection InvoiceUsers { get; set; }
		// private InvoiceUserCollection _unmodified_InvoiceUsers;
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
	public class InvoiceDetails : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceDetails(
[ID] TEXT NOT NULL PRIMARY KEY, 
[MonthlyFeesTotal] TEXT NOT NULL, 
[OneTimeFeesTotal] TEXT NOT NULL, 
[UsageFeesTotal] TEXT NOT NULL, 
[InterestFeesTotal] TEXT NOT NULL, 
[PenaltyFeesTotal] TEXT NOT NULL, 
[TotalFeesTotal] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string MonthlyFeesTotal { get; set; }
		// private string _unmodified_MonthlyFeesTotal;

		[Column]
		public string OneTimeFeesTotal { get; set; }
		// private string _unmodified_OneTimeFeesTotal;

		[Column]
		public string UsageFeesTotal { get; set; }
		// private string _unmodified_UsageFeesTotal;

		[Column]
		public string InterestFeesTotal { get; set; }
		// private string _unmodified_InterestFeesTotal;

		[Column]
		public string PenaltyFeesTotal { get; set; }
		// private string _unmodified_PenaltyFeesTotal;

		[Column]
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
	public class InvoiceUser : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceUser(
[ID] TEXT NOT NULL PRIMARY KEY, 
[UserName] TEXT NOT NULL, 
[UserID] TEXT NOT NULL, 
[UserPhoneNumber] TEXT NOT NULL, 
[UserSubscriptionNumber] TEXT NOT NULL, 
[UserInvoiceTotalAmount] TEXT NOT NULL, 
[InvoiceRowGroupCollection] TEXT NOT NULL, 
[InvoiceEventDetailGroupCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string UserName { get; set; }
		// private string _unmodified_UserName;

		[Column]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
		public string UserPhoneNumber { get; set; }
		// private string _unmodified_UserPhoneNumber;

		[Column]
		public string UserSubscriptionNumber { get; set; }
		// private string _unmodified_UserSubscriptionNumber;

		[Column]
		public string UserInvoiceTotalAmount { get; set; }
		// private string _unmodified_UserInvoiceTotalAmount;

		[Column]
		public InvoiceRowGroupCollection InvoiceRowGroupCollection { get; set; }
		// private InvoiceRowGroupCollection _unmodified_InvoiceRowGroupCollection;

		[Column]
		public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection { get; set; }
		// private InvoiceEventDetailGroupCollection _unmodified_InvoiceEventDetailGroupCollection;
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
	public class InvoiceRowGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceRowGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL, 
[GroupTotalPriceWithoutTaxes] TEXT NOT NULL, 
[GroupTotalTaxes] TEXT NOT NULL, 
[GroupTotalPriceWithTaxes] TEXT NOT NULL, 
[InvoiceRowCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public string GroupTotalPriceWithoutTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithoutTaxes;

		[Column]
		public string GroupTotalTaxes { get; set; }
		// private string _unmodified_GroupTotalTaxes;

		[Column]
		public string GroupTotalPriceWithTaxes { get; set; }
		// private string _unmodified_GroupTotalPriceWithTaxes;

		[Column]
		public InvoiceRowCollection InvoiceRowCollection { get; set; }
		// private InvoiceRowCollection _unmodified_InvoiceRowCollection;
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
	public class InvoiceEventDetailGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceEventDetailGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL, 
[InvoiceEventDetailCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public InvoiceEventDetailCollection InvoiceEventDetailCollection { get; set; }
		// private InvoiceEventDetailCollection _unmodified_InvoiceEventDetailCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupName == null)
				GroupName = string.Empty;
		}
	}
    [Table(Name = "InvoiceEventDetail")]
	public class InvoiceEventDetail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceEventDetail(
[ID] TEXT NOT NULL PRIMARY KEY, 
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
		public string ID { get; set; }


		[Column]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
		public DateTime EventStartDateTime { get; set; }
		// private DateTime _unmodified_EventStartDateTime;

		[Column]
		public DateTime EventEndDateTime { get; set; }
		// private DateTime _unmodified_EventEndDateTime;

		[Column]
		public string ReceivingParty { get; set; }
		// private string _unmodified_ReceivingParty;

		[Column]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
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
	public class InvoiceRow : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InvoiceRow(
[ID] TEXT NOT NULL PRIMARY KEY, 
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
		public string ID { get; set; }


		[Column]
		public string IndentMode { get; set; }
		// private string _unmodified_IndentMode;

		[Column]
		public string AmountOfUnits { get; set; }
		// private string _unmodified_AmountOfUnits;

		[Column]
		public string Duration { get; set; }
		// private string _unmodified_Duration;

		[Column]
		public string UnitPrice { get; set; }
		// private string _unmodified_UnitPrice;

		[Column]
		public string PriceWithoutTaxes { get; set; }
		// private string _unmodified_PriceWithoutTaxes;

		[Column]
		public string Taxes { get; set; }
		// private string _unmodified_Taxes;

		[Column]
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
	public class Category : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Category(
[ID] TEXT NOT NULL PRIMARY KEY, 
[CategoryName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CategoryName == null)
				CategoryName = string.Empty;
		}
	}
    [Table(Name = "ProcessContainer")]
	public class ProcessContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProcessContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProcessIDs] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "ProcessIDs")] public string ProcessIDsData;

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
	public class Process : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Process(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProcessDescription] TEXT NOT NULL, 
[ExecutingOperation] TEXT NOT NULL, 
[InitialArguments] TEXT NOT NULL, 
[ProcessItems] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ProcessDescription { get; set; }
		// private string _unmodified_ProcessDescription;

		[Column]
		public SemanticInformationItem ExecutingOperation { get; set; }
		// private SemanticInformationItem _unmodified_ExecutingOperation;
        [Column(Name = "InitialArguments")] public string InitialArgumentsData;

        private bool _IsInitialArgumentsRetrieved = false;
        private bool _IsInitialArgumentsChanged = false;
        private ObservableCollection<SemanticInformationItem> _InitialArguments = null;
        public ObservableCollection<SemanticInformationItem> InitialArguments
        {
            get
            {
                if (!_IsInitialArgumentsRetrieved)
                {
                    if (InitialArgumentsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(InitialArgumentsData);
                        _InitialArguments = new ObservableCollection<SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _InitialArguments = new ObservableCollection<SemanticInformationItem>();
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

        [Column(Name = "ProcessItems")] public string ProcessItemsData;

        private bool _IsProcessItemsRetrieved = false;
        private bool _IsProcessItemsChanged = false;
        private ObservableCollection<ProcessItem> _ProcessItems = null;
        public ObservableCollection<ProcessItem> ProcessItems
        {
            get
            {
                if (!_IsProcessItemsRetrieved)
                {
                    if (ProcessItemsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<ProcessItem[]>(ProcessItemsData);
                        _ProcessItems = new ObservableCollection<ProcessItem>(arrayData);
                    }
                    else
                    {
                        _ProcessItems = new ObservableCollection<ProcessItem>();
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
	public class ProcessItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProcessItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Outputs] TEXT NOT NULL, 
[Inputs] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }

        [Column(Name = "Outputs")] public string OutputsData;

        private bool _IsOutputsRetrieved = false;
        private bool _IsOutputsChanged = false;
        private ObservableCollection<SemanticInformationItem> _Outputs = null;
        public ObservableCollection<SemanticInformationItem> Outputs
        {
            get
            {
                if (!_IsOutputsRetrieved)
                {
                    if (OutputsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(OutputsData);
                        _Outputs = new ObservableCollection<SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _Outputs = new ObservableCollection<SemanticInformationItem>();
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

        [Column(Name = "Inputs")] public string InputsData;

        private bool _IsInputsRetrieved = false;
        private bool _IsInputsChanged = false;
        private ObservableCollection<SemanticInformationItem> _Inputs = null;
        public ObservableCollection<SemanticInformationItem> Inputs
        {
            get
            {
                if (!_IsInputsRetrieved)
                {
                    if (InputsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<SemanticInformationItem[]>(InputsData);
                        _Inputs = new ObservableCollection<SemanticInformationItem>(arrayData);
                    }
                    else
                    {
                        _Inputs = new ObservableCollection<SemanticInformationItem>();
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
	public class SemanticInformationItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SemanticInformationItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ItemFullType] TEXT NOT NULL, 
[ItemValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ItemFullType { get; set; }
		// private string _unmodified_ItemFullType;

		[Column]
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
	public class InformationOwnerInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationOwnerInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OwnerType] TEXT NOT NULL, 
[OwnerIdentifier] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OwnerType { get; set; }
		// private string _unmodified_OwnerType;

		[Column]
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
	public class UsageSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS UsageSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SummaryName] TEXT NOT NULL, 
[SummaryMonitoringItem] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SummaryName { get; set; }
		// private string _unmodified_SummaryName;

		[Column]
		public UsageMonitorItem SummaryMonitoringItem { get; set; }
		// private UsageMonitorItem _unmodified_SummaryMonitoringItem;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryName == null)
				SummaryName = string.Empty;
		}
	}
    [Table(Name = "UsageMonitorItem")]
	public class UsageMonitorItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS UsageMonitorItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OwnerInfo] TEXT NOT NULL, 
[TimeRangeInclusiveStartExclusiveEnd] TEXT NOT NULL, 
[StepSizeInMinutes] INTEGER NOT NULL, 
[ProcessorUsages] TEXT NOT NULL, 
[StorageTransactionUsages] TEXT NOT NULL, 
[StorageUsages] TEXT NOT NULL, 
[NetworkUsages] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InformationOwnerInfo OwnerInfo { get; set; }
		// private InformationOwnerInfo _unmodified_OwnerInfo;

		[Column]
		public TimeRange TimeRangeInclusiveStartExclusiveEnd { get; set; }
		// private TimeRange _unmodified_TimeRangeInclusiveStartExclusiveEnd;

		[Column]
		public long StepSizeInMinutes { get; set; }
		// private long _unmodified_StepSizeInMinutes;

		[Column]
		public ProcessorUsageCollection ProcessorUsages { get; set; }
		// private ProcessorUsageCollection _unmodified_ProcessorUsages;

		[Column]
		public StorageTransactionUsageCollection StorageTransactionUsages { get; set; }
		// private StorageTransactionUsageCollection _unmodified_StorageTransactionUsages;

		[Column]
		public StorageUsageCollection StorageUsages { get; set; }
		// private StorageUsageCollection _unmodified_StorageUsages;

		[Column]
		public NetworkUsageCollection NetworkUsages { get; set; }
		// private NetworkUsageCollection _unmodified_NetworkUsages;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RequestResourceUsage")]
	public class RequestResourceUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS RequestResourceUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OwnerInfo] TEXT NOT NULL, 
[ProcessorUsage] TEXT NOT NULL, 
[StorageTransactionUsage] TEXT NOT NULL, 
[NetworkUsage] TEXT NOT NULL, 
[RequestDetails] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public InformationOwnerInfo OwnerInfo { get; set; }
		// private InformationOwnerInfo _unmodified_OwnerInfo;

		[Column]
		public ProcessorUsage ProcessorUsage { get; set; }
		// private ProcessorUsage _unmodified_ProcessorUsage;

		[Column]
		public StorageTransactionUsage StorageTransactionUsage { get; set; }
		// private StorageTransactionUsage _unmodified_StorageTransactionUsage;

		[Column]
		public NetworkUsage NetworkUsage { get; set; }
		// private NetworkUsage _unmodified_NetworkUsage;

		[Column]
		public HTTPActivityDetails RequestDetails { get; set; }
		// private HTTPActivityDetails _unmodified_RequestDetails;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ProcessorUsage")]
	public class ProcessorUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProcessorUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TimeRange] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTicks] REAL NOT NULL, 
[FrequencyTicksPerSecond] REAL NOT NULL, 
[Milliseconds] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public double AmountOfTicks { get; set; }
		// private double _unmodified_AmountOfTicks;

		[Column]
		public double FrequencyTicksPerSecond { get; set; }
		// private double _unmodified_FrequencyTicksPerSecond;

		[Column]
		public long Milliseconds { get; set; }
		// private long _unmodified_Milliseconds;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "StorageTransactionUsage")]
	public class StorageTransactionUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS StorageTransactionUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TimeRange] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTransactions] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public long AmountOfTransactions { get; set; }
		// private long _unmodified_AmountOfTransactions;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "StorageUsage")]
	public class StorageUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS StorageUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SnapshotTime] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[UsageUnit] TEXT NOT NULL, 
[AmountOfUnits] REAL NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime SnapshotTime { get; set; }
		// private DateTime _unmodified_SnapshotTime;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public string UsageUnit { get; set; }
		// private string _unmodified_UsageUnit;

		[Column]
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
	public class NetworkUsage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS NetworkUsage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TimeRange] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfBytes] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TimeRange TimeRange { get; set; }
		// private TimeRange _unmodified_TimeRange;

		[Column]
		public string UsageType { get; set; }
		// private string _unmodified_UsageType;

		[Column]
		public long AmountOfBytes { get; set; }
		// private long _unmodified_AmountOfBytes;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(UsageType == null)
				UsageType = string.Empty;
		}
	}
    [Table(Name = "TimeRange")]
	public class TimeRange : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TimeRange(
[ID] TEXT NOT NULL PRIMARY KEY, 
[StartTime] TEXT NOT NULL, 
[EndTime] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime StartTime { get; set; }
		// private DateTime _unmodified_StartTime;

		[Column]
		public DateTime EndTime { get; set; }
		// private DateTime _unmodified_EndTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "HTTPActivityDetails")]
	public class HTTPActivityDetails : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS HTTPActivityDetails(
[ID] TEXT NOT NULL PRIMARY KEY, 
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
		public string ID { get; set; }


		[Column]
		public string RemoteIPAddress { get; set; }
		// private string _unmodified_RemoteIPAddress;

		[Column]
		public string RemoteEndpointUserName { get; set; }
		// private string _unmodified_RemoteEndpointUserName;

		[Column]
		public string UserID { get; set; }
		// private string _unmodified_UserID;

		[Column]
		public DateTime UTCDateTime { get; set; }
		// private DateTime _unmodified_UTCDateTime;

		[Column]
		public string RequestLine { get; set; }
		// private string _unmodified_RequestLine;

		[Column]
		public long HTTPStatusCode { get; set; }
		// private long _unmodified_HTTPStatusCode;

		[Column]
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
