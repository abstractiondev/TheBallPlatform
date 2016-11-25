 


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
				tableCreationCommands.Add(Login.GetCreateTableSQL());
				tableCreationCommands.Add(LoginAccount.GetCreateTableSQL());
				tableCreationCommands.Add(Email.GetCreateTableSQL());
				tableCreationCommands.Add(EmailAccount.GetCreateTableSQL());
				tableCreationCommands.Add(Account.GetCreateTableSQL());
				tableCreationCommands.Add(AccountEmails.GetCreateTableSQL());
				tableCreationCommands.Add(AccountLogins.GetCreateTableSQL());
				tableCreationCommands.Add(AccountGroupMemberships.GetCreateTableSQL());
				tableCreationCommands.Add(Group.GetCreateTableSQL());
				tableCreationCommands.Add(GroupGroupMemberships.GetCreateTableSQL());
				tableCreationCommands.Add(GroupMembership.GetCreateTableSQL());
				tableCreationCommands.Add(GroupMembershipAccount.GetCreateTableSQL());
				tableCreationCommands.Add(GroupMembershipGroup.GetCreateTableSQL());
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
				tableCreationCommands.Add(ContentPackageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InformationInputCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InformationOutputCollection.GetCreateTableSQL());
				tableCreationCommands.Add(AuthenticatedAsActiveDeviceCollection.GetCreateTableSQL());
				tableCreationCommands.Add(DeviceMembershipCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceUserCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceRowGroupCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceEventDetailGroupCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceEventDetailCollection.GetCreateTableSQL());
				tableCreationCommands.Add(InvoiceRowCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryCollection.GetCreateTableSQL());
				tableCreationCommands.Add(RequestResourceUsageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ProcessorUsageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(StorageTransactionUsageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(StorageUsageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(NetworkUsageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(HTTPActivityDetailsCollection.GetCreateTableSQL());
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

				switch(updateData.ObjectType)
				{
		        case "Login":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Login.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LoginTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LoginName = serializedObject.LoginName;
		            existingObject.PasswordHash = serializedObject.PasswordHash;
		            existingObject.PasswordSalt = serializedObject.PasswordSalt;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new LoginAccount
                            {
                                LoginID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            LoginAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

		            break;
		        } 
		        case "Email":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Email.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = EmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new EmailAccount
                            {
                                EmailID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            EmailAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

		            existingObject.PendingValidation = serializedObject.PendingValidation;
		            existingObject.ValidationKey = serializedObject.ValidationKey;
		            existingObject.ValidationProcessExpiration = serializedObject.ValidationProcessExpiration;
		            break;
		        } 
		        case "Account":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Account.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Emails != null)
                    {
						existingObject.Emails.Clear();
                        serializedObject.Emails.ForEach(
                            item =>
                            {
                                var relationObject = new AccountEmails
                                {
                                    AccountID = existingObject.ID,
                                    EmailID = item
                                };
                                AccountEmailsTable.InsertOnSubmit(relationObject);
                                existingObject.Emails.Add(relationObject);

                            });
                    }

                    if (serializedObject.Logins != null)
                    {
						existingObject.Logins.Clear();
                        serializedObject.Logins.ForEach(
                            item =>
                            {
                                var relationObject = new AccountLogins
                                {
                                    AccountID = existingObject.ID,
                                    LoginID = item
                                };
                                AccountLoginsTable.InsertOnSubmit(relationObject);
                                existingObject.Logins.Add(relationObject);

                            });
                    }

                    if (serializedObject.GroupMemberships != null)
                    {
						existingObject.GroupMemberships.Clear();
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new AccountGroupMemberships
                                {
                                    AccountID = existingObject.ID,
                                    GroupMembershipID = item
                                };
                                AccountGroupMembershipsTable.InsertOnSubmit(relationObject);
                                existingObject.GroupMemberships.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "Group":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Group.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.GroupMemberships != null)
                    {
						existingObject.GroupMemberships.Clear();
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new GroupGroupMemberships
                                {
                                    GroupID = existingObject.ID,
                                    GroupMembershipID = item
                                };
                                GroupGroupMembershipsTable.InsertOnSubmit(relationObject);
                                existingObject.GroupMemberships.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "GroupMembership":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.GroupMembership.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupMembershipTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new GroupMembershipAccount
                            {
                                GroupMembershipID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            GroupMembershipAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

                    if (serializedObject.Group != null)
                    {
                            var relationObject = new GroupMembershipGroup
                            {
                                GroupMembershipID = existingObject.ID,
                                GroupID = serializedObject.Group
                            };
                            GroupMembershipGroupTable.InsertOnSubmit(relationObject);
							existingObject.Group = relationObject;
                    }

		            existingObject.Role = serializedObject.Role;
		            break;
		        } 
		        case "ContentPackage":
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
		            break;
		        } 
		        case "InformationInput":
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
		            break;
		        } 
		        case "InformationOutput":
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
		            break;
		        } 
		        case "AuthenticatedAsActiveDevice":
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
		            break;
		        } 
		        case "DeviceMembership":
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
		            break;
		        } 
		        case "InvoiceFiscalExportSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceFiscalExportSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            existingObject.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
					if(serializedObject.ExportedInvoices != null)
						existingObject.ExportedInvoicesID = serializedObject.ExportedInvoices.ID;
					else
						existingObject.ExportedInvoicesID = null;
		            break;
		        } 
		        case "InvoiceSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.OpenInvoices != null)
						existingObject.OpenInvoicesID = serializedObject.OpenInvoices.ID;
					else
						existingObject.OpenInvoicesID = null;
					if(serializedObject.PredictedInvoices != null)
						existingObject.PredictedInvoicesID = serializedObject.PredictedInvoices.ID;
					else
						existingObject.PredictedInvoicesID = null;
					if(serializedObject.PaidInvoicesActiveYear != null)
						existingObject.PaidInvoicesActiveYearID = serializedObject.PaidInvoicesActiveYear.ID;
					else
						existingObject.PaidInvoicesActiveYearID = null;
					if(serializedObject.PaidInvoicesLast12Months != null)
						existingObject.PaidInvoicesLast12MonthsID = serializedObject.PaidInvoicesLast12Months.ID;
					else
						existingObject.PaidInvoicesLast12MonthsID = null;
		            break;
		        } 
		        case "Invoice":
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
					if(serializedObject.InvoiceUsers != null)
						existingObject.InvoiceUsersID = serializedObject.InvoiceUsers.ID;
					else
						existingObject.InvoiceUsersID = null;
		            break;
		        } 
		        case "InvoiceDetails":
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
		            break;
		        } 
		        case "InvoiceUser":
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
					if(serializedObject.InvoiceRowGroupCollection != null)
						existingObject.InvoiceRowGroupCollectionID = serializedObject.InvoiceRowGroupCollection.ID;
					else
						existingObject.InvoiceRowGroupCollectionID = null;
					if(serializedObject.InvoiceEventDetailGroupCollection != null)
						existingObject.InvoiceEventDetailGroupCollectionID = serializedObject.InvoiceEventDetailGroupCollection.ID;
					else
						existingObject.InvoiceEventDetailGroupCollectionID = null;
		            break;
		        } 
		        case "InvoiceRowGroup":
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
					if(serializedObject.InvoiceRowCollection != null)
						existingObject.InvoiceRowCollectionID = serializedObject.InvoiceRowCollection.ID;
					else
						existingObject.InvoiceRowCollectionID = null;
		            break;
		        } 
		        case "InvoiceEventDetailGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
					if(serializedObject.InvoiceEventDetailCollection != null)
						existingObject.InvoiceEventDetailCollectionID = serializedObject.InvoiceEventDetailCollection.ID;
					else
						existingObject.InvoiceEventDetailCollectionID = null;
		            break;
		        } 
		        case "InvoiceEventDetail":
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
		            break;
		        } 
		        case "InvoiceRow":
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
		            break;
		        } 
		        case "Category":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Category.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            break;
		        } 
		        case "ProcessContainer":
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
					
		            break;
		        } 
		        case "Process":
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
					
		            break;
		        } 
		        case "ProcessItem":
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
					
		            break;
		        } 
		        case "SemanticInformationItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SemanticInformationItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ItemFullType = serializedObject.ItemFullType;
		            existingObject.ItemValue = serializedObject.ItemValue;
		            break;
		        } 
		        case "InformationOwnerInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationOwnerInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OwnerType = serializedObject.OwnerType;
		            existingObject.OwnerIdentifier = serializedObject.OwnerIdentifier;
		            break;
		        } 
		        case "UsageSummary":
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
		            break;
		        } 
		        case "UsageMonitorItem":
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
					if(serializedObject.ProcessorUsages != null)
						existingObject.ProcessorUsagesID = serializedObject.ProcessorUsages.ID;
					else
						existingObject.ProcessorUsagesID = null;
					if(serializedObject.StorageTransactionUsages != null)
						existingObject.StorageTransactionUsagesID = serializedObject.StorageTransactionUsages.ID;
					else
						existingObject.StorageTransactionUsagesID = null;
					if(serializedObject.StorageUsages != null)
						existingObject.StorageUsagesID = serializedObject.StorageUsages.ID;
					else
						existingObject.StorageUsagesID = null;
					if(serializedObject.NetworkUsages != null)
						existingObject.NetworkUsagesID = serializedObject.NetworkUsages.ID;
					else
						existingObject.NetworkUsagesID = null;
		            break;
		        } 
		        case "RequestResourceUsage":
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
		            break;
		        } 
		        case "ProcessorUsage":
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
		            break;
		        } 
		        case "StorageTransactionUsage":
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
		            break;
		        } 
		        case "StorageUsage":
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
		            break;
		        } 
		        case "NetworkUsage":
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
		            break;
		        } 
		        case "TimeRange":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TimeRangeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StartTime = serializedObject.StartTime;
		            existingObject.EndTime = serializedObject.EndTime;
		            break;
		        } 
		        case "HTTPActivityDetails":
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
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "Login":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Login.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LoginTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LoginName = serializedObject.LoginName;
		            existingObject.PasswordHash = serializedObject.PasswordHash;
		            existingObject.PasswordSalt = serializedObject.PasswordSalt;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new LoginAccount
                            {
                                LoginID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            LoginAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

		            break;
		        } 
		        case "Email":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Email.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = EmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new EmailAccount
                            {
                                EmailID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            EmailAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

		            existingObject.PendingValidation = serializedObject.PendingValidation;
		            existingObject.ValidationKey = serializedObject.ValidationKey;
		            existingObject.ValidationProcessExpiration = serializedObject.ValidationProcessExpiration;
		            break;
		        } 
		        case "Account":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Account.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Emails != null)
                    {
						existingObject.Emails.Clear();
                        serializedObject.Emails.ForEach(
                            item =>
                            {
                                var relationObject = new AccountEmails
                                {
                                    AccountID = existingObject.ID,
                                    EmailID = item
                                };
                                AccountEmailsTable.InsertOnSubmit(relationObject);
                                existingObject.Emails.Add(relationObject);

                            });
                    }

                    if (serializedObject.Logins != null)
                    {
						existingObject.Logins.Clear();
                        serializedObject.Logins.ForEach(
                            item =>
                            {
                                var relationObject = new AccountLogins
                                {
                                    AccountID = existingObject.ID,
                                    LoginID = item
                                };
                                AccountLoginsTable.InsertOnSubmit(relationObject);
                                existingObject.Logins.Add(relationObject);

                            });
                    }

                    if (serializedObject.GroupMemberships != null)
                    {
						existingObject.GroupMemberships.Clear();
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new AccountGroupMemberships
                                {
                                    AccountID = existingObject.ID,
                                    GroupMembershipID = item
                                };
                                AccountGroupMembershipsTable.InsertOnSubmit(relationObject);
                                existingObject.GroupMemberships.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "Group":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Group.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.GroupMemberships != null)
                    {
						existingObject.GroupMemberships.Clear();
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new GroupGroupMemberships
                                {
                                    GroupID = existingObject.ID,
                                    GroupMembershipID = item
                                };
                                GroupGroupMembershipsTable.InsertOnSubmit(relationObject);
                                existingObject.GroupMemberships.Add(relationObject);

                            });
                    }

		            break;
		        } 
		        case "GroupMembership":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.GroupMembership.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupMembershipTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new GroupMembershipAccount
                            {
                                GroupMembershipID = existingObject.ID,
                                AccountID = serializedObject.Account
                            };
                            GroupMembershipAccountTable.InsertOnSubmit(relationObject);
							existingObject.Account = relationObject;
                    }

                    if (serializedObject.Group != null)
                    {
                            var relationObject = new GroupMembershipGroup
                            {
                                GroupMembershipID = existingObject.ID,
                                GroupID = serializedObject.Group
                            };
                            GroupMembershipGroupTable.InsertOnSubmit(relationObject);
							existingObject.Group = relationObject;
                    }

		            existingObject.Role = serializedObject.Role;
		            break;
		        } 
		        case "ContentPackage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.ContentPackage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ContentPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PackageType = serializedObject.PackageType;
		            existingObject.PackageName = serializedObject.PackageName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.PackageRootFolder = serializedObject.PackageRootFolder;
		            existingObject.CreationTime = serializedObject.CreationTime;
		            break;
		        } 
		        case "InformationInput":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InformationInput.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InformationInputTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InputDescription = serializedObject.InputDescription;
		            existingObject.LocationURL = serializedObject.LocationURL;
		            existingObject.LocalContentName = serializedObject.LocalContentName;
		            existingObject.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            break;
		        } 
		        case "InformationOutput":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InformationOutput.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InformationOutputTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OutputDescription = serializedObject.OutputDescription;
		            existingObject.DestinationURL = serializedObject.DestinationURL;
		            existingObject.DestinationContentName = serializedObject.DestinationContentName;
		            existingObject.LocalContentURL = serializedObject.LocalContentURL;
		            existingObject.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            break;
		        } 
		        case "AuthenticatedAsActiveDevice":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.AuthenticatedAsActiveDevice.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AuthenticatedAsActiveDeviceTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AuthenticationDescription = serializedObject.AuthenticationDescription;
		            existingObject.SharedSecret = serializedObject.SharedSecret;
		            existingObject.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            existingObject.EstablishedTrustID = serializedObject.EstablishedTrustID;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            existingObject.NegotiationURL = serializedObject.NegotiationURL;
		            existingObject.ConnectionURL = serializedObject.ConnectionURL;
		            break;
		        } 
		        case "DeviceMembership":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.DeviceMembership.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = DeviceMembershipTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.DeviceDescription = serializedObject.DeviceDescription;
		            existingObject.SharedSecret = serializedObject.SharedSecret;
		            existingObject.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            existingObject.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            break;
		        } 
		        case "InvoiceFiscalExportSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceFiscalExportSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            existingObject.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
					if(serializedObject.ExportedInvoices != null)
						existingObject.ExportedInvoicesID = serializedObject.ExportedInvoices.ID;
					else
						existingObject.ExportedInvoicesID = null;
		            break;
		        } 
		        case "InvoiceSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.OpenInvoices != null)
						existingObject.OpenInvoicesID = serializedObject.OpenInvoices.ID;
					else
						existingObject.OpenInvoicesID = null;
					if(serializedObject.PredictedInvoices != null)
						existingObject.PredictedInvoicesID = serializedObject.PredictedInvoices.ID;
					else
						existingObject.PredictedInvoicesID = null;
					if(serializedObject.PaidInvoicesActiveYear != null)
						existingObject.PaidInvoicesActiveYearID = serializedObject.PaidInvoicesActiveYear.ID;
					else
						existingObject.PaidInvoicesActiveYearID = null;
					if(serializedObject.PaidInvoicesLast12Months != null)
						existingObject.PaidInvoicesLast12MonthsID = serializedObject.PaidInvoicesLast12Months.ID;
					else
						existingObject.PaidInvoicesLast12MonthsID = null;
		            break;
		        } 
		        case "Invoice":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Invoice.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					if(serializedObject.InvoiceUsers != null)
						existingObject.InvoiceUsersID = serializedObject.InvoiceUsers.ID;
					else
						existingObject.InvoiceUsersID = null;
		            break;
		        } 
		        case "InvoiceDetails":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceDetails.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceDetailsTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.MonthlyFeesTotal = serializedObject.MonthlyFeesTotal;
		            existingObject.OneTimeFeesTotal = serializedObject.OneTimeFeesTotal;
		            existingObject.UsageFeesTotal = serializedObject.UsageFeesTotal;
		            existingObject.InterestFeesTotal = serializedObject.InterestFeesTotal;
		            existingObject.PenaltyFeesTotal = serializedObject.PenaltyFeesTotal;
		            existingObject.TotalFeesTotal = serializedObject.TotalFeesTotal;
		            break;
		        } 
		        case "InvoiceUser":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceUser.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceUserTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.UserName = serializedObject.UserName;
		            existingObject.UserID = serializedObject.UserID;
		            existingObject.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            existingObject.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            existingObject.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
					if(serializedObject.InvoiceRowGroupCollection != null)
						existingObject.InvoiceRowGroupCollectionID = serializedObject.InvoiceRowGroupCollection.ID;
					else
						existingObject.InvoiceRowGroupCollectionID = null;
					if(serializedObject.InvoiceEventDetailGroupCollection != null)
						existingObject.InvoiceEventDetailGroupCollectionID = serializedObject.InvoiceEventDetailGroupCollection.ID;
					else
						existingObject.InvoiceEventDetailGroupCollectionID = null;
		            break;
		        } 
		        case "InvoiceRowGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceRowGroup.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceRowGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            existingObject.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            existingObject.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
					if(serializedObject.InvoiceRowCollection != null)
						existingObject.InvoiceRowCollectionID = serializedObject.InvoiceRowCollection.ID;
					else
						existingObject.InvoiceRowCollectionID = null;
		            break;
		        } 
		        case "InvoiceEventDetailGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceEventDetailGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
					if(serializedObject.InvoiceEventDetailCollection != null)
						existingObject.InvoiceEventDetailCollectionID = serializedObject.InvoiceEventDetailCollection.ID;
					else
						existingObject.InvoiceEventDetailCollectionID = null;
		            break;
		        } 
		        case "InvoiceEventDetail":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceEventDetail.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
		            break;
		        } 
		        case "InvoiceRow":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InvoiceRow.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InvoiceRowTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IndentMode = serializedObject.IndentMode;
		            existingObject.AmountOfUnits = serializedObject.AmountOfUnits;
		            existingObject.Duration = serializedObject.Duration;
		            existingObject.UnitPrice = serializedObject.UnitPrice;
		            existingObject.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            existingObject.Taxes = serializedObject.Taxes;
		            existingObject.PriceWithTaxes = serializedObject.PriceWithTaxes;
		            break;
		        } 
		        case "Category":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Category.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            break;
		        } 
		        case "ProcessContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.ProcessContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ProcessContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.ProcessIDs.Clear();
					if(serializedObject.ProcessIDs != null)
	                    serializedObject.ProcessIDs.ForEach(item => existingObject.ProcessIDs.Add(item));
					
		            break;
		        } 
		        case "Process":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.Process.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					
		            break;
		        } 
		        case "ProcessItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.ProcessItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ProcessItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
                    existingObject.Outputs.Clear();
					if(serializedObject.Outputs != null)
	                    serializedObject.Outputs.ForEach(item => existingObject.Outputs.Add(item));
					
                    existingObject.Inputs.Clear();
					if(serializedObject.Inputs != null)
	                    serializedObject.Inputs.ForEach(item => existingObject.Inputs.Add(item));
					
		            break;
		        } 
		        case "SemanticInformationItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = SemanticInformationItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ItemFullType = serializedObject.ItemFullType;
		            existingObject.ItemValue = serializedObject.ItemValue;
		            break;
		        } 
		        case "InformationOwnerInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = InformationOwnerInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OwnerType = serializedObject.OwnerType;
		            existingObject.OwnerIdentifier = serializedObject.OwnerIdentifier;
		            break;
		        } 
		        case "UsageSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.UsageSummary.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = UsageSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SummaryName = serializedObject.SummaryName;
					if(serializedObject.SummaryMonitoringItem != null)
						existingObject.SummaryMonitoringItemID = serializedObject.SummaryMonitoringItem.ID;
					else
						existingObject.SummaryMonitoringItemID = null;
		            break;
		        } 
		        case "UsageMonitorItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.UsageMonitorItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					if(serializedObject.ProcessorUsages != null)
						existingObject.ProcessorUsagesID = serializedObject.ProcessorUsages.ID;
					else
						existingObject.ProcessorUsagesID = null;
					if(serializedObject.StorageTransactionUsages != null)
						existingObject.StorageTransactionUsagesID = serializedObject.StorageTransactionUsages.ID;
					else
						existingObject.StorageTransactionUsagesID = null;
					if(serializedObject.StorageUsages != null)
						existingObject.StorageUsagesID = serializedObject.StorageUsages.ID;
					else
						existingObject.StorageUsagesID = null;
					if(serializedObject.NetworkUsages != null)
						existingObject.NetworkUsagesID = serializedObject.NetworkUsages.ID;
					else
						existingObject.NetworkUsagesID = null;
		            break;
		        } 
		        case "RequestResourceUsage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.RequestResourceUsage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
		            break;
		        } 
		        case "ProcessorUsage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.ProcessorUsage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
		            break;
		        } 
		        case "StorageTransactionUsage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.StorageTransactionUsage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = StorageTransactionUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.TimeRange != null)
						existingObject.TimeRangeID = serializedObject.TimeRange.ID;
					else
						existingObject.TimeRangeID = null;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfTransactions = serializedObject.AmountOfTransactions;
		            break;
		        } 
		        case "StorageUsage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.StorageUsage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = StorageUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SnapshotTime = serializedObject.SnapshotTime;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.UsageUnit = serializedObject.UsageUnit;
		            existingObject.AmountOfUnits = serializedObject.AmountOfUnits;
		            break;
		        } 
		        case "NetworkUsage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.NetworkUsage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = NetworkUsageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.TimeRange != null)
						existingObject.TimeRangeID = serializedObject.TimeRange.ID;
					else
						existingObject.TimeRangeID = null;
		            existingObject.UsageType = serializedObject.UsageType;
		            existingObject.AmountOfBytes = serializedObject.AmountOfBytes;
		            break;
		        } 
		        case "TimeRange":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TimeRangeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StartTime = serializedObject.StartTime;
		            existingObject.EndTime = serializedObject.EndTime;
		            break;
		        } 
		        case "HTTPActivityDetails":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.TheBall.CORE.HTTPActivityDetails.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = HTTPActivityDetailsTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.RemoteIPAddress = serializedObject.RemoteIPAddress;
		            existingObject.RemoteEndpointUserName = serializedObject.RemoteEndpointUserName;
		            existingObject.UserID = serializedObject.UserID;
		            existingObject.UTCDateTime = serializedObject.UTCDateTime;
		            existingObject.RequestLine = serializedObject.RequestLine;
		            existingObject.HTTPStatusCode = serializedObject.HTTPStatusCode;
		            existingObject.ReturnedContentLength = serializedObject.ReturnedContentLength;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

				switch(insertData.ObjectType)
				{
                case "Login":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Login.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Login {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LoginName = serializedObject.LoginName;
		            objectToAdd.PasswordHash = serializedObject.PasswordHash;
		            objectToAdd.PasswordSalt = serializedObject.PasswordSalt;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new LoginAccount
                            {
                                LoginID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            LoginAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

					LoginTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Email":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Email.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Email {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new EmailAccount
                            {
                                EmailID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            EmailAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

		            objectToAdd.PendingValidation = serializedObject.PendingValidation;
		            objectToAdd.ValidationKey = serializedObject.ValidationKey;
		            objectToAdd.ValidationProcessExpiration = serializedObject.ValidationProcessExpiration;
					EmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Account":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Account.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Account {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Emails != null)
                    {
                        serializedObject.Emails.ForEach(
                            item =>
                            {
                                var relationObject = new AccountEmails
                                {
                                    AccountID = objectToAdd.ID,
                                    EmailID = item
                                };
                                AccountEmailsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Emails.Add(relationObject);

                            });
                    }

                    if (serializedObject.Logins != null)
                    {
                        serializedObject.Logins.ForEach(
                            item =>
                            {
                                var relationObject = new AccountLogins
                                {
                                    AccountID = objectToAdd.ID,
                                    LoginID = item
                                };
                                AccountLoginsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Logins.Add(relationObject);

                            });
                    }

                    if (serializedObject.GroupMemberships != null)
                    {
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new AccountGroupMemberships
                                {
                                    AccountID = objectToAdd.ID,
                                    GroupMembershipID = item
                                };
                                AccountGroupMembershipsTable.InsertOnSubmit(relationObject);
                                objectToAdd.GroupMemberships.Add(relationObject);

                            });
                    }

					AccountTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Group":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Group.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Group {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.GroupMemberships != null)
                    {
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new GroupGroupMemberships
                                {
                                    GroupID = objectToAdd.ID,
                                    GroupMembershipID = item
                                };
                                GroupGroupMembershipsTable.InsertOnSubmit(relationObject);
                                objectToAdd.GroupMemberships.Add(relationObject);

                            });
                    }

					GroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupMembership":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.GroupMembership.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupMembership {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new GroupMembershipAccount
                            {
                                GroupMembershipID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            GroupMembershipAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

                    if (serializedObject.Group != null)
                    {
                            var relationObject = new GroupMembershipGroup
                            {
                                GroupMembershipID = objectToAdd.ID,
                                GroupID = serializedObject.Group
                            };
                            GroupMembershipGroupTable.InsertOnSubmit(relationObject);
                            objectToAdd.Group = relationObject;
                    }

		            objectToAdd.Role = serializedObject.Role;
					GroupMembershipTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ContentPackage":
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
                    break;
                }
                case "InformationInput":
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
                    break;
                }
                case "InformationOutput":
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
                    break;
                }
                case "AuthenticatedAsActiveDevice":
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
                    break;
                }
                case "DeviceMembership":
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
                    break;
                }
                case "InvoiceFiscalExportSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceFiscalExportSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            objectToAdd.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
					if(serializedObject.ExportedInvoices != null)
						objectToAdd.ExportedInvoicesID = serializedObject.ExportedInvoices.ID;
					else
						objectToAdd.ExportedInvoicesID = null;
					InvoiceFiscalExportSummaryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.OpenInvoices != null)
						objectToAdd.OpenInvoicesID = serializedObject.OpenInvoices.ID;
					else
						objectToAdd.OpenInvoicesID = null;
					if(serializedObject.PredictedInvoices != null)
						objectToAdd.PredictedInvoicesID = serializedObject.PredictedInvoices.ID;
					else
						objectToAdd.PredictedInvoicesID = null;
					if(serializedObject.PaidInvoicesActiveYear != null)
						objectToAdd.PaidInvoicesActiveYearID = serializedObject.PaidInvoicesActiveYear.ID;
					else
						objectToAdd.PaidInvoicesActiveYearID = null;
					if(serializedObject.PaidInvoicesLast12Months != null)
						objectToAdd.PaidInvoicesLast12MonthsID = serializedObject.PaidInvoicesLast12Months.ID;
					else
						objectToAdd.PaidInvoicesLast12MonthsID = null;
					InvoiceSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Invoice":
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
					if(serializedObject.InvoiceUsers != null)
						objectToAdd.InvoiceUsersID = serializedObject.InvoiceUsers.ID;
					else
						objectToAdd.InvoiceUsersID = null;
					InvoiceTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceDetails":
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
                    break;
                }
                case "InvoiceUser":
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
					if(serializedObject.InvoiceRowGroupCollection != null)
						objectToAdd.InvoiceRowGroupCollectionID = serializedObject.InvoiceRowGroupCollection.ID;
					else
						objectToAdd.InvoiceRowGroupCollectionID = null;
					if(serializedObject.InvoiceEventDetailGroupCollection != null)
						objectToAdd.InvoiceEventDetailGroupCollectionID = serializedObject.InvoiceEventDetailGroupCollection.ID;
					else
						objectToAdd.InvoiceEventDetailGroupCollectionID = null;
					InvoiceUserTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceRowGroup":
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
					if(serializedObject.InvoiceRowCollection != null)
						objectToAdd.InvoiceRowCollectionID = serializedObject.InvoiceRowCollection.ID;
					else
						objectToAdd.InvoiceRowCollectionID = null;
					InvoiceRowGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceEventDetailGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetailGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					if(serializedObject.InvoiceEventDetailCollection != null)
						objectToAdd.InvoiceEventDetailCollectionID = serializedObject.InvoiceEventDetailCollection.ID;
					else
						objectToAdd.InvoiceEventDetailCollectionID = null;
					InvoiceEventDetailGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceEventDetail":
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
                    break;
                }
                case "InvoiceRow":
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
                    break;
                }
                case "Category":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Category.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ProcessContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProcessContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProcessIDs != null)
						serializedObject.ProcessIDs.ForEach(item => objectToAdd.ProcessIDs.Add(item));
					ProcessContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Process":
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
                    break;
                }
                case "ProcessItem":
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
                    break;
                }
                case "SemanticInformationItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SemanticInformationItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ItemFullType = serializedObject.ItemFullType;
		            objectToAdd.ItemValue = serializedObject.ItemValue;
					SemanticInformationItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InformationOwnerInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationOwnerInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OwnerType = serializedObject.OwnerType;
		            objectToAdd.OwnerIdentifier = serializedObject.OwnerIdentifier;
					InformationOwnerInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UsageSummary":
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
                    break;
                }
                case "UsageMonitorItem":
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
					if(serializedObject.ProcessorUsages != null)
						objectToAdd.ProcessorUsagesID = serializedObject.ProcessorUsages.ID;
					else
						objectToAdd.ProcessorUsagesID = null;
					if(serializedObject.StorageTransactionUsages != null)
						objectToAdd.StorageTransactionUsagesID = serializedObject.StorageTransactionUsages.ID;
					else
						objectToAdd.StorageTransactionUsagesID = null;
					if(serializedObject.StorageUsages != null)
						objectToAdd.StorageUsagesID = serializedObject.StorageUsages.ID;
					else
						objectToAdd.StorageUsagesID = null;
					if(serializedObject.NetworkUsages != null)
						objectToAdd.NetworkUsagesID = serializedObject.NetworkUsages.ID;
					else
						objectToAdd.NetworkUsagesID = null;
					UsageMonitorItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "RequestResourceUsage":
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
                    break;
                }
                case "ProcessorUsage":
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
                    break;
                }
                case "StorageTransactionUsage":
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
                    break;
                }
                case "StorageUsage":
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
                    break;
                }
                case "NetworkUsage":
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
                    break;
                }
                case "TimeRange":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TimeRange {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StartTime = serializedObject.StartTime;
		            objectToAdd.EndTime = serializedObject.EndTime;
					TimeRangeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "HTTPActivityDetails":
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
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

				switch(insertData.ObjectType)
				{
                case "Login":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Login.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Login {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LoginName = serializedObject.LoginName;
		            objectToAdd.PasswordHash = serializedObject.PasswordHash;
		            objectToAdd.PasswordSalt = serializedObject.PasswordSalt;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new LoginAccount
                            {
                                LoginID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            LoginAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

					LoginTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Email":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Email.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Email {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new EmailAccount
                            {
                                EmailID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            EmailAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

		            objectToAdd.PendingValidation = serializedObject.PendingValidation;
		            objectToAdd.ValidationKey = serializedObject.ValidationKey;
		            objectToAdd.ValidationProcessExpiration = serializedObject.ValidationProcessExpiration;
					EmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Account":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Account.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Account {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Emails != null)
                    {
                        serializedObject.Emails.ForEach(
                            item =>
                            {
                                var relationObject = new AccountEmails
                                {
                                    AccountID = objectToAdd.ID,
                                    EmailID = item
                                };
                                AccountEmailsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Emails.Add(relationObject);

                            });
                    }

                    if (serializedObject.Logins != null)
                    {
                        serializedObject.Logins.ForEach(
                            item =>
                            {
                                var relationObject = new AccountLogins
                                {
                                    AccountID = objectToAdd.ID,
                                    LoginID = item
                                };
                                AccountLoginsTable.InsertOnSubmit(relationObject);
                                objectToAdd.Logins.Add(relationObject);

                            });
                    }

                    if (serializedObject.GroupMemberships != null)
                    {
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new AccountGroupMemberships
                                {
                                    AccountID = objectToAdd.ID,
                                    GroupMembershipID = item
                                };
                                AccountGroupMembershipsTable.InsertOnSubmit(relationObject);
                                objectToAdd.GroupMemberships.Add(relationObject);

                            });
                    }

					AccountTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Group":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Group.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Group {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.GroupMemberships != null)
                    {
                        serializedObject.GroupMemberships.ForEach(
                            item =>
                            {
                                var relationObject = new GroupGroupMemberships
                                {
                                    GroupID = objectToAdd.ID,
                                    GroupMembershipID = item
                                };
                                GroupGroupMembershipsTable.InsertOnSubmit(relationObject);
                                objectToAdd.GroupMemberships.Add(relationObject);

                            });
                    }

					GroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupMembership":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.GroupMembership.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GroupMembership {ID = insertData.ObjectID, ETag = insertData.ETag};
                    if (serializedObject.Account != null)
                    {
                            var relationObject = new GroupMembershipAccount
                            {
                                GroupMembershipID = objectToAdd.ID,
                                AccountID = serializedObject.Account
                            };
                            GroupMembershipAccountTable.InsertOnSubmit(relationObject);
                            objectToAdd.Account = relationObject;
                    }

                    if (serializedObject.Group != null)
                    {
                            var relationObject = new GroupMembershipGroup
                            {
                                GroupMembershipID = objectToAdd.ID,
                                GroupID = serializedObject.Group
                            };
                            GroupMembershipGroupTable.InsertOnSubmit(relationObject);
                            objectToAdd.Group = relationObject;
                    }

		            objectToAdd.Role = serializedObject.Role;
					GroupMembershipTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ContentPackage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ContentPackage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ContentPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PackageType = serializedObject.PackageType;
		            objectToAdd.PackageName = serializedObject.PackageName;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.PackageRootFolder = serializedObject.PackageRootFolder;
		            objectToAdd.CreationTime = serializedObject.CreationTime;
					ContentPackageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InformationInput":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationInput.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InformationInput {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InputDescription = serializedObject.InputDescription;
		            objectToAdd.LocationURL = serializedObject.LocationURL;
		            objectToAdd.LocalContentName = serializedObject.LocalContentName;
		            objectToAdd.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					InformationInputTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InformationOutput":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationOutput.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InformationOutput {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OutputDescription = serializedObject.OutputDescription;
		            objectToAdd.DestinationURL = serializedObject.DestinationURL;
		            objectToAdd.DestinationContentName = serializedObject.DestinationContentName;
		            objectToAdd.LocalContentURL = serializedObject.LocalContentURL;
		            objectToAdd.AuthenticatedDeviceID = serializedObject.AuthenticatedDeviceID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					InformationOutputTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AuthenticatedAsActiveDevice":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.AuthenticatedAsActiveDevice.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AuthenticatedAsActiveDevice {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AuthenticationDescription = serializedObject.AuthenticationDescription;
		            objectToAdd.SharedSecret = serializedObject.SharedSecret;
		            objectToAdd.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            objectToAdd.EstablishedTrustID = serializedObject.EstablishedTrustID;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
		            objectToAdd.NegotiationURL = serializedObject.NegotiationURL;
		            objectToAdd.ConnectionURL = serializedObject.ConnectionURL;
					AuthenticatedAsActiveDeviceTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "DeviceMembership":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.DeviceMembership.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new DeviceMembership {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.DeviceDescription = serializedObject.DeviceDescription;
		            objectToAdd.SharedSecret = serializedObject.SharedSecret;
		            objectToAdd.ActiveSymmetricAESKey = serializedObject.ActiveSymmetricAESKey;
		            objectToAdd.IsValidatedAndActive = serializedObject.IsValidatedAndActive;
					DeviceMembershipTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceFiscalExportSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceFiscalExportSummary.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceFiscalExportSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FiscalInclusiveStartDate = serializedObject.FiscalInclusiveStartDate;
		            objectToAdd.FiscalInclusiveEndDate = serializedObject.FiscalInclusiveEndDate;
					if(serializedObject.ExportedInvoices != null)
						objectToAdd.ExportedInvoicesID = serializedObject.ExportedInvoices.ID;
					else
						objectToAdd.ExportedInvoicesID = null;
					InvoiceFiscalExportSummaryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceSummaryContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.OpenInvoices != null)
						objectToAdd.OpenInvoicesID = serializedObject.OpenInvoices.ID;
					else
						objectToAdd.OpenInvoicesID = null;
					if(serializedObject.PredictedInvoices != null)
						objectToAdd.PredictedInvoicesID = serializedObject.PredictedInvoices.ID;
					else
						objectToAdd.PredictedInvoicesID = null;
					if(serializedObject.PaidInvoicesActiveYear != null)
						objectToAdd.PaidInvoicesActiveYearID = serializedObject.PaidInvoicesActiveYear.ID;
					else
						objectToAdd.PaidInvoicesActiveYearID = null;
					if(serializedObject.PaidInvoicesLast12Months != null)
						objectToAdd.PaidInvoicesLast12MonthsID = serializedObject.PaidInvoicesLast12Months.ID;
					else
						objectToAdd.PaidInvoicesLast12MonthsID = null;
					InvoiceSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Invoice":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Invoice.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					if(serializedObject.InvoiceUsers != null)
						objectToAdd.InvoiceUsersID = serializedObject.InvoiceUsers.ID;
					else
						objectToAdd.InvoiceUsersID = null;
					InvoiceTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceDetails":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceDetails.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceDetails {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.MonthlyFeesTotal = serializedObject.MonthlyFeesTotal;
		            objectToAdd.OneTimeFeesTotal = serializedObject.OneTimeFeesTotal;
		            objectToAdd.UsageFeesTotal = serializedObject.UsageFeesTotal;
		            objectToAdd.InterestFeesTotal = serializedObject.InterestFeesTotal;
		            objectToAdd.PenaltyFeesTotal = serializedObject.PenaltyFeesTotal;
		            objectToAdd.TotalFeesTotal = serializedObject.TotalFeesTotal;
					InvoiceDetailsTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceUser":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceUser.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceUser {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.UserName = serializedObject.UserName;
		            objectToAdd.UserID = serializedObject.UserID;
		            objectToAdd.UserPhoneNumber = serializedObject.UserPhoneNumber;
		            objectToAdd.UserSubscriptionNumber = serializedObject.UserSubscriptionNumber;
		            objectToAdd.UserInvoiceTotalAmount = serializedObject.UserInvoiceTotalAmount;
					if(serializedObject.InvoiceRowGroupCollection != null)
						objectToAdd.InvoiceRowGroupCollectionID = serializedObject.InvoiceRowGroupCollection.ID;
					else
						objectToAdd.InvoiceRowGroupCollectionID = null;
					if(serializedObject.InvoiceEventDetailGroupCollection != null)
						objectToAdd.InvoiceEventDetailGroupCollectionID = serializedObject.InvoiceEventDetailGroupCollection.ID;
					else
						objectToAdd.InvoiceEventDetailGroupCollectionID = null;
					InvoiceUserTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceRowGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceRowGroup.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceRowGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.GroupTotalPriceWithoutTaxes = serializedObject.GroupTotalPriceWithoutTaxes;
		            objectToAdd.GroupTotalTaxes = serializedObject.GroupTotalTaxes;
		            objectToAdd.GroupTotalPriceWithTaxes = serializedObject.GroupTotalPriceWithTaxes;
					if(serializedObject.InvoiceRowCollection != null)
						objectToAdd.InvoiceRowCollectionID = serializedObject.InvoiceRowCollection.ID;
					else
						objectToAdd.InvoiceRowCollectionID = null;
					InvoiceRowGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceEventDetailGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceEventDetailGroup.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceEventDetailGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					if(serializedObject.InvoiceEventDetailCollection != null)
						objectToAdd.InvoiceEventDetailCollectionID = serializedObject.InvoiceEventDetailCollection.ID;
					else
						objectToAdd.InvoiceEventDetailCollectionID = null;
					InvoiceEventDetailGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InvoiceEventDetail":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceEventDetail.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
                    break;
                }
                case "InvoiceRow":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InvoiceRow.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InvoiceRow {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IndentMode = serializedObject.IndentMode;
		            objectToAdd.AmountOfUnits = serializedObject.AmountOfUnits;
		            objectToAdd.Duration = serializedObject.Duration;
		            objectToAdd.UnitPrice = serializedObject.UnitPrice;
		            objectToAdd.PriceWithoutTaxes = serializedObject.PriceWithoutTaxes;
		            objectToAdd.Taxes = serializedObject.Taxes;
		            objectToAdd.PriceWithTaxes = serializedObject.PriceWithTaxes;
					InvoiceRowTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Category":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Category.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ProcessContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ProcessContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProcessIDs != null)
						serializedObject.ProcessIDs.ForEach(item => objectToAdd.ProcessIDs.Add(item));
					ProcessContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Process":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.Process.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
                    break;
                }
                case "ProcessItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ProcessItem {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Outputs != null)
						serializedObject.Outputs.ForEach(item => objectToAdd.Outputs.Add(item));
					if(serializedObject.Inputs != null)
						serializedObject.Inputs.ForEach(item => objectToAdd.Inputs.Add(item));
					ProcessItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "SemanticInformationItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.SemanticInformationItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new SemanticInformationItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ItemFullType = serializedObject.ItemFullType;
		            objectToAdd.ItemValue = serializedObject.ItemValue;
					SemanticInformationItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "InformationOwnerInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.InformationOwnerInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new InformationOwnerInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OwnerType = serializedObject.OwnerType;
		            objectToAdd.OwnerIdentifier = serializedObject.OwnerIdentifier;
					InformationOwnerInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UsageSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.UsageSummary.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new UsageSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SummaryName = serializedObject.SummaryName;
					if(serializedObject.SummaryMonitoringItem != null)
						objectToAdd.SummaryMonitoringItemID = serializedObject.SummaryMonitoringItem.ID;
					else
						objectToAdd.SummaryMonitoringItemID = null;
					UsageSummaryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UsageMonitorItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.UsageMonitorItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
					if(serializedObject.ProcessorUsages != null)
						objectToAdd.ProcessorUsagesID = serializedObject.ProcessorUsages.ID;
					else
						objectToAdd.ProcessorUsagesID = null;
					if(serializedObject.StorageTransactionUsages != null)
						objectToAdd.StorageTransactionUsagesID = serializedObject.StorageTransactionUsages.ID;
					else
						objectToAdd.StorageTransactionUsagesID = null;
					if(serializedObject.StorageUsages != null)
						objectToAdd.StorageUsagesID = serializedObject.StorageUsages.ID;
					else
						objectToAdd.StorageUsagesID = null;
					if(serializedObject.NetworkUsages != null)
						objectToAdd.NetworkUsagesID = serializedObject.NetworkUsages.ID;
					else
						objectToAdd.NetworkUsagesID = null;
					UsageMonitorItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "RequestResourceUsage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.RequestResourceUsage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
                    break;
                }
                case "ProcessorUsage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.ProcessorUsage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
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
                    break;
                }
                case "StorageTransactionUsage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.StorageTransactionUsage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new StorageTransactionUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.TimeRange != null)
						objectToAdd.TimeRangeID = serializedObject.TimeRange.ID;
					else
						objectToAdd.TimeRangeID = null;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfTransactions = serializedObject.AmountOfTransactions;
					StorageTransactionUsageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "StorageUsage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.StorageUsage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new StorageUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SnapshotTime = serializedObject.SnapshotTime;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.UsageUnit = serializedObject.UsageUnit;
		            objectToAdd.AmountOfUnits = serializedObject.AmountOfUnits;
					StorageUsageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "NetworkUsage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.NetworkUsage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new NetworkUsage {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.TimeRange != null)
						objectToAdd.TimeRangeID = serializedObject.TimeRange.ID;
					else
						objectToAdd.TimeRangeID = null;
		            objectToAdd.UsageType = serializedObject.UsageType;
		            objectToAdd.AmountOfBytes = serializedObject.AmountOfBytes;
					NetworkUsageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TimeRange":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.TimeRange.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TimeRange {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StartTime = serializedObject.StartTime;
		            objectToAdd.EndTime = serializedObject.EndTime;
					TimeRangeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "HTTPActivityDetails":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.TheBall.CORE.HTTPActivityDetails.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new HTTPActivityDetails {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.RemoteIPAddress = serializedObject.RemoteIPAddress;
		            objectToAdd.RemoteEndpointUserName = serializedObject.RemoteEndpointUserName;
		            objectToAdd.UserID = serializedObject.UserID;
		            objectToAdd.UTCDateTime = serializedObject.UTCDateTime;
		            objectToAdd.RequestLine = serializedObject.RequestLine;
		            objectToAdd.HTTPStatusCode = serializedObject.HTTPStatusCode;
		            objectToAdd.ReturnedContentLength = serializedObject.ReturnedContentLength;
					HTTPActivityDetailsTable.InsertOnSubmit(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Login":
					{
						//var objectToDelete = new Login {ID = deleteData.ObjectID};
						//LoginTable.Attach(objectToDelete);
						var objectToDelete = LoginTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Email":
					{
						//var objectToDelete = new Email {ID = deleteData.ObjectID};
						//EmailTable.Attach(objectToDelete);
						var objectToDelete = EmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Account":
					{
						//var objectToDelete = new Account {ID = deleteData.ObjectID};
						//AccountTable.Attach(objectToDelete);
						var objectToDelete = AccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Group":
					{
						//var objectToDelete = new Group {ID = deleteData.ObjectID};
						//GroupTable.Attach(objectToDelete);
						var objectToDelete = GroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupMembership":
					{
						//var objectToDelete = new GroupMembership {ID = deleteData.ObjectID};
						//GroupMembershipTable.Attach(objectToDelete);
						var objectToDelete = GroupMembershipTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupMembershipTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentPackage":
					{
						//var objectToDelete = new ContentPackage {ID = deleteData.ObjectID};
						//ContentPackageTable.Attach(objectToDelete);
						var objectToDelete = ContentPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentPackageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationInput":
					{
						//var objectToDelete = new InformationInput {ID = deleteData.ObjectID};
						//InformationInputTable.Attach(objectToDelete);
						var objectToDelete = InformationInputTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationInputTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOutput":
					{
						//var objectToDelete = new InformationOutput {ID = deleteData.ObjectID};
						//InformationOutputTable.Attach(objectToDelete);
						var objectToDelete = InformationOutputTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOutputTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AuthenticatedAsActiveDevice":
					{
						//var objectToDelete = new AuthenticatedAsActiveDevice {ID = deleteData.ObjectID};
						//AuthenticatedAsActiveDeviceTable.Attach(objectToDelete);
						var objectToDelete = AuthenticatedAsActiveDeviceTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AuthenticatedAsActiveDeviceTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DeviceMembership":
					{
						//var objectToDelete = new DeviceMembership {ID = deleteData.ObjectID};
						//DeviceMembershipTable.Attach(objectToDelete);
						var objectToDelete = DeviceMembershipTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DeviceMembershipTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceFiscalExportSummary":
					{
						//var objectToDelete = new InvoiceFiscalExportSummary {ID = deleteData.ObjectID};
						//InvoiceFiscalExportSummaryTable.Attach(objectToDelete);
						var objectToDelete = InvoiceFiscalExportSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceFiscalExportSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceSummaryContainer":
					{
						//var objectToDelete = new InvoiceSummaryContainer {ID = deleteData.ObjectID};
						//InvoiceSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = InvoiceSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Invoice":
					{
						//var objectToDelete = new Invoice {ID = deleteData.ObjectID};
						//InvoiceTable.Attach(objectToDelete);
						var objectToDelete = InvoiceTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceDetails":
					{
						//var objectToDelete = new InvoiceDetails {ID = deleteData.ObjectID};
						//InvoiceDetailsTable.Attach(objectToDelete);
						var objectToDelete = InvoiceDetailsTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceDetailsTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceUser":
					{
						//var objectToDelete = new InvoiceUser {ID = deleteData.ObjectID};
						//InvoiceUserTable.Attach(objectToDelete);
						var objectToDelete = InvoiceUserTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceUserTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowGroup":
					{
						//var objectToDelete = new InvoiceRowGroup {ID = deleteData.ObjectID};
						//InvoiceRowGroupTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailGroup":
					{
						//var objectToDelete = new InvoiceEventDetailGroup {ID = deleteData.ObjectID};
						//InvoiceEventDetailGroupTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetail":
					{
						//var objectToDelete = new InvoiceEventDetail {ID = deleteData.ObjectID};
						//InvoiceEventDetailTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRow":
					{
						//var objectToDelete = new InvoiceRow {ID = deleteData.ObjectID};
						//InvoiceRowTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Category":
					{
						//var objectToDelete = new Category {ID = deleteData.ObjectID};
						//CategoryTable.Attach(objectToDelete);
						var objectToDelete = CategoryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessContainer":
					{
						//var objectToDelete = new ProcessContainer {ID = deleteData.ObjectID};
						//ProcessContainerTable.Attach(objectToDelete);
						var objectToDelete = ProcessContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Process":
					{
						//var objectToDelete = new Process {ID = deleteData.ObjectID};
						//ProcessTable.Attach(objectToDelete);
						var objectToDelete = ProcessTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessItem":
					{
						//var objectToDelete = new ProcessItem {ID = deleteData.ObjectID};
						//ProcessItemTable.Attach(objectToDelete);
						var objectToDelete = ProcessItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SemanticInformationItem":
					{
						//var objectToDelete = new SemanticInformationItem {ID = deleteData.ObjectID};
						//SemanticInformationItemTable.Attach(objectToDelete);
						var objectToDelete = SemanticInformationItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SemanticInformationItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOwnerInfo":
					{
						//var objectToDelete = new InformationOwnerInfo {ID = deleteData.ObjectID};
						//InformationOwnerInfoTable.Attach(objectToDelete);
						var objectToDelete = InformationOwnerInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOwnerInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UsageSummary":
					{
						//var objectToDelete = new UsageSummary {ID = deleteData.ObjectID};
						//UsageSummaryTable.Attach(objectToDelete);
						var objectToDelete = UsageSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UsageSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UsageMonitorItem":
					{
						//var objectToDelete = new UsageMonitorItem {ID = deleteData.ObjectID};
						//UsageMonitorItemTable.Attach(objectToDelete);
						var objectToDelete = UsageMonitorItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UsageMonitorItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RequestResourceUsage":
					{
						//var objectToDelete = new RequestResourceUsage {ID = deleteData.ObjectID};
						//RequestResourceUsageTable.Attach(objectToDelete);
						var objectToDelete = RequestResourceUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RequestResourceUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessorUsage":
					{
						//var objectToDelete = new ProcessorUsage {ID = deleteData.ObjectID};
						//ProcessorUsageTable.Attach(objectToDelete);
						var objectToDelete = ProcessorUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessorUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageTransactionUsage":
					{
						//var objectToDelete = new StorageTransactionUsage {ID = deleteData.ObjectID};
						//StorageTransactionUsageTable.Attach(objectToDelete);
						var objectToDelete = StorageTransactionUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageTransactionUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageUsage":
					{
						//var objectToDelete = new StorageUsage {ID = deleteData.ObjectID};
						//StorageUsageTable.Attach(objectToDelete);
						var objectToDelete = StorageUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NetworkUsage":
					{
						//var objectToDelete = new NetworkUsage {ID = deleteData.ObjectID};
						//NetworkUsageTable.Attach(objectToDelete);
						var objectToDelete = NetworkUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NetworkUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TimeRange":
					{
						//var objectToDelete = new TimeRange {ID = deleteData.ObjectID};
						//TimeRangeTable.Attach(objectToDelete);
						var objectToDelete = TimeRangeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TimeRangeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "HTTPActivityDetails":
					{
						//var objectToDelete = new HTTPActivityDetails {ID = deleteData.ObjectID};
						//HTTPActivityDetailsTable.Attach(objectToDelete);
						var objectToDelete = HTTPActivityDetailsTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							HTTPActivityDetailsTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentPackageCollection":
					{
						//var objectToDelete = new ContentPackageCollection {ID = deleteData.ObjectID};
						//ContentPackageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ContentPackageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentPackageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationInputCollection":
					{
						//var objectToDelete = new InformationInputCollection {ID = deleteData.ObjectID};
						//InformationInputCollectionTable.Attach(objectToDelete);
						var objectToDelete = InformationInputCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationInputCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOutputCollection":
					{
						//var objectToDelete = new InformationOutputCollection {ID = deleteData.ObjectID};
						//InformationOutputCollectionTable.Attach(objectToDelete);
						var objectToDelete = InformationOutputCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOutputCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AuthenticatedAsActiveDeviceCollection":
					{
						//var objectToDelete = new AuthenticatedAsActiveDeviceCollection {ID = deleteData.ObjectID};
						//AuthenticatedAsActiveDeviceCollectionTable.Attach(objectToDelete);
						var objectToDelete = AuthenticatedAsActiveDeviceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AuthenticatedAsActiveDeviceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DeviceMembershipCollection":
					{
						//var objectToDelete = new DeviceMembershipCollection {ID = deleteData.ObjectID};
						//DeviceMembershipCollectionTable.Attach(objectToDelete);
						var objectToDelete = DeviceMembershipCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DeviceMembershipCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceCollection":
					{
						//var objectToDelete = new InvoiceCollection {ID = deleteData.ObjectID};
						//InvoiceCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceUserCollection":
					{
						//var objectToDelete = new InvoiceUserCollection {ID = deleteData.ObjectID};
						//InvoiceUserCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceUserCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceUserCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowGroupCollection":
					{
						//var objectToDelete = new InvoiceRowGroupCollection {ID = deleteData.ObjectID};
						//InvoiceRowGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailGroupCollection":
					{
						//var objectToDelete = new InvoiceEventDetailGroupCollection {ID = deleteData.ObjectID};
						//InvoiceEventDetailGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailCollection":
					{
						//var objectToDelete = new InvoiceEventDetailCollection {ID = deleteData.ObjectID};
						//InvoiceEventDetailCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowCollection":
					{
						//var objectToDelete = new InvoiceRowCollection {ID = deleteData.ObjectID};
						//InvoiceRowCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CategoryCollection":
					{
						//var objectToDelete = new CategoryCollection {ID = deleteData.ObjectID};
						//CategoryCollectionTable.Attach(objectToDelete);
						var objectToDelete = CategoryCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RequestResourceUsageCollection":
					{
						//var objectToDelete = new RequestResourceUsageCollection {ID = deleteData.ObjectID};
						//RequestResourceUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = RequestResourceUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RequestResourceUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessorUsageCollection":
					{
						//var objectToDelete = new ProcessorUsageCollection {ID = deleteData.ObjectID};
						//ProcessorUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ProcessorUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessorUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageTransactionUsageCollection":
					{
						//var objectToDelete = new StorageTransactionUsageCollection {ID = deleteData.ObjectID};
						//StorageTransactionUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = StorageTransactionUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageTransactionUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageUsageCollection":
					{
						//var objectToDelete = new StorageUsageCollection {ID = deleteData.ObjectID};
						//StorageUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = StorageUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NetworkUsageCollection":
					{
						//var objectToDelete = new NetworkUsageCollection {ID = deleteData.ObjectID};
						//NetworkUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = NetworkUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NetworkUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "HTTPActivityDetailsCollection":
					{
						//var objectToDelete = new HTTPActivityDetailsCollection {ID = deleteData.ObjectID};
						//HTTPActivityDetailsCollectionTable.Attach(objectToDelete);
						var objectToDelete = HTTPActivityDetailsCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							HTTPActivityDetailsCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "TheBall.CORE")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "Login":
					{
						//var objectToDelete = new Login {ID = deleteData.ObjectID};
						//LoginTable.Attach(objectToDelete);
						var objectToDelete = LoginTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Email":
					{
						//var objectToDelete = new Email {ID = deleteData.ObjectID};
						//EmailTable.Attach(objectToDelete);
						var objectToDelete = EmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Account":
					{
						//var objectToDelete = new Account {ID = deleteData.ObjectID};
						//AccountTable.Attach(objectToDelete);
						var objectToDelete = AccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Group":
					{
						//var objectToDelete = new Group {ID = deleteData.ObjectID};
						//GroupTable.Attach(objectToDelete);
						var objectToDelete = GroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupMembership":
					{
						//var objectToDelete = new GroupMembership {ID = deleteData.ObjectID};
						//GroupMembershipTable.Attach(objectToDelete);
						var objectToDelete = GroupMembershipTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupMembershipTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentPackage":
					{
						//var objectToDelete = new ContentPackage {ID = deleteData.ObjectID};
						//ContentPackageTable.Attach(objectToDelete);
						var objectToDelete = ContentPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentPackageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationInput":
					{
						//var objectToDelete = new InformationInput {ID = deleteData.ObjectID};
						//InformationInputTable.Attach(objectToDelete);
						var objectToDelete = InformationInputTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationInputTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOutput":
					{
						//var objectToDelete = new InformationOutput {ID = deleteData.ObjectID};
						//InformationOutputTable.Attach(objectToDelete);
						var objectToDelete = InformationOutputTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOutputTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AuthenticatedAsActiveDevice":
					{
						//var objectToDelete = new AuthenticatedAsActiveDevice {ID = deleteData.ObjectID};
						//AuthenticatedAsActiveDeviceTable.Attach(objectToDelete);
						var objectToDelete = AuthenticatedAsActiveDeviceTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AuthenticatedAsActiveDeviceTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DeviceMembership":
					{
						//var objectToDelete = new DeviceMembership {ID = deleteData.ObjectID};
						//DeviceMembershipTable.Attach(objectToDelete);
						var objectToDelete = DeviceMembershipTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DeviceMembershipTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceFiscalExportSummary":
					{
						//var objectToDelete = new InvoiceFiscalExportSummary {ID = deleteData.ObjectID};
						//InvoiceFiscalExportSummaryTable.Attach(objectToDelete);
						var objectToDelete = InvoiceFiscalExportSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceFiscalExportSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceSummaryContainer":
					{
						//var objectToDelete = new InvoiceSummaryContainer {ID = deleteData.ObjectID};
						//InvoiceSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = InvoiceSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Invoice":
					{
						//var objectToDelete = new Invoice {ID = deleteData.ObjectID};
						//InvoiceTable.Attach(objectToDelete);
						var objectToDelete = InvoiceTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceDetails":
					{
						//var objectToDelete = new InvoiceDetails {ID = deleteData.ObjectID};
						//InvoiceDetailsTable.Attach(objectToDelete);
						var objectToDelete = InvoiceDetailsTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceDetailsTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceUser":
					{
						//var objectToDelete = new InvoiceUser {ID = deleteData.ObjectID};
						//InvoiceUserTable.Attach(objectToDelete);
						var objectToDelete = InvoiceUserTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceUserTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowGroup":
					{
						//var objectToDelete = new InvoiceRowGroup {ID = deleteData.ObjectID};
						//InvoiceRowGroupTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailGroup":
					{
						//var objectToDelete = new InvoiceEventDetailGroup {ID = deleteData.ObjectID};
						//InvoiceEventDetailGroupTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetail":
					{
						//var objectToDelete = new InvoiceEventDetail {ID = deleteData.ObjectID};
						//InvoiceEventDetailTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRow":
					{
						//var objectToDelete = new InvoiceRow {ID = deleteData.ObjectID};
						//InvoiceRowTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Category":
					{
						//var objectToDelete = new Category {ID = deleteData.ObjectID};
						//CategoryTable.Attach(objectToDelete);
						var objectToDelete = CategoryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessContainer":
					{
						//var objectToDelete = new ProcessContainer {ID = deleteData.ObjectID};
						//ProcessContainerTable.Attach(objectToDelete);
						var objectToDelete = ProcessContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Process":
					{
						//var objectToDelete = new Process {ID = deleteData.ObjectID};
						//ProcessTable.Attach(objectToDelete);
						var objectToDelete = ProcessTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessItem":
					{
						//var objectToDelete = new ProcessItem {ID = deleteData.ObjectID};
						//ProcessItemTable.Attach(objectToDelete);
						var objectToDelete = ProcessItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SemanticInformationItem":
					{
						//var objectToDelete = new SemanticInformationItem {ID = deleteData.ObjectID};
						//SemanticInformationItemTable.Attach(objectToDelete);
						var objectToDelete = SemanticInformationItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SemanticInformationItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOwnerInfo":
					{
						//var objectToDelete = new InformationOwnerInfo {ID = deleteData.ObjectID};
						//InformationOwnerInfoTable.Attach(objectToDelete);
						var objectToDelete = InformationOwnerInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOwnerInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UsageSummary":
					{
						//var objectToDelete = new UsageSummary {ID = deleteData.ObjectID};
						//UsageSummaryTable.Attach(objectToDelete);
						var objectToDelete = UsageSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UsageSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UsageMonitorItem":
					{
						//var objectToDelete = new UsageMonitorItem {ID = deleteData.ObjectID};
						//UsageMonitorItemTable.Attach(objectToDelete);
						var objectToDelete = UsageMonitorItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UsageMonitorItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RequestResourceUsage":
					{
						//var objectToDelete = new RequestResourceUsage {ID = deleteData.ObjectID};
						//RequestResourceUsageTable.Attach(objectToDelete);
						var objectToDelete = RequestResourceUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RequestResourceUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessorUsage":
					{
						//var objectToDelete = new ProcessorUsage {ID = deleteData.ObjectID};
						//ProcessorUsageTable.Attach(objectToDelete);
						var objectToDelete = ProcessorUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessorUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageTransactionUsage":
					{
						//var objectToDelete = new StorageTransactionUsage {ID = deleteData.ObjectID};
						//StorageTransactionUsageTable.Attach(objectToDelete);
						var objectToDelete = StorageTransactionUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageTransactionUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageUsage":
					{
						//var objectToDelete = new StorageUsage {ID = deleteData.ObjectID};
						//StorageUsageTable.Attach(objectToDelete);
						var objectToDelete = StorageUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NetworkUsage":
					{
						//var objectToDelete = new NetworkUsage {ID = deleteData.ObjectID};
						//NetworkUsageTable.Attach(objectToDelete);
						var objectToDelete = NetworkUsageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NetworkUsageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TimeRange":
					{
						//var objectToDelete = new TimeRange {ID = deleteData.ObjectID};
						//TimeRangeTable.Attach(objectToDelete);
						var objectToDelete = TimeRangeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TimeRangeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "HTTPActivityDetails":
					{
						//var objectToDelete = new HTTPActivityDetails {ID = deleteData.ObjectID};
						//HTTPActivityDetailsTable.Attach(objectToDelete);
						var objectToDelete = HTTPActivityDetailsTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							HTTPActivityDetailsTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentPackageCollection":
					{
						//var objectToDelete = new ContentPackageCollection {ID = deleteData.ObjectID};
						//ContentPackageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ContentPackageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentPackageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationInputCollection":
					{
						//var objectToDelete = new InformationInputCollection {ID = deleteData.ObjectID};
						//InformationInputCollectionTable.Attach(objectToDelete);
						var objectToDelete = InformationInputCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationInputCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InformationOutputCollection":
					{
						//var objectToDelete = new InformationOutputCollection {ID = deleteData.ObjectID};
						//InformationOutputCollectionTable.Attach(objectToDelete);
						var objectToDelete = InformationOutputCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InformationOutputCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AuthenticatedAsActiveDeviceCollection":
					{
						//var objectToDelete = new AuthenticatedAsActiveDeviceCollection {ID = deleteData.ObjectID};
						//AuthenticatedAsActiveDeviceCollectionTable.Attach(objectToDelete);
						var objectToDelete = AuthenticatedAsActiveDeviceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AuthenticatedAsActiveDeviceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DeviceMembershipCollection":
					{
						//var objectToDelete = new DeviceMembershipCollection {ID = deleteData.ObjectID};
						//DeviceMembershipCollectionTable.Attach(objectToDelete);
						var objectToDelete = DeviceMembershipCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DeviceMembershipCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceCollection":
					{
						//var objectToDelete = new InvoiceCollection {ID = deleteData.ObjectID};
						//InvoiceCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceUserCollection":
					{
						//var objectToDelete = new InvoiceUserCollection {ID = deleteData.ObjectID};
						//InvoiceUserCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceUserCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceUserCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowGroupCollection":
					{
						//var objectToDelete = new InvoiceRowGroupCollection {ID = deleteData.ObjectID};
						//InvoiceRowGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailGroupCollection":
					{
						//var objectToDelete = new InvoiceEventDetailGroupCollection {ID = deleteData.ObjectID};
						//InvoiceEventDetailGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceEventDetailCollection":
					{
						//var objectToDelete = new InvoiceEventDetailCollection {ID = deleteData.ObjectID};
						//InvoiceEventDetailCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceEventDetailCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceEventDetailCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "InvoiceRowCollection":
					{
						//var objectToDelete = new InvoiceRowCollection {ID = deleteData.ObjectID};
						//InvoiceRowCollectionTable.Attach(objectToDelete);
						var objectToDelete = InvoiceRowCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							InvoiceRowCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CategoryCollection":
					{
						//var objectToDelete = new CategoryCollection {ID = deleteData.ObjectID};
						//CategoryCollectionTable.Attach(objectToDelete);
						var objectToDelete = CategoryCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RequestResourceUsageCollection":
					{
						//var objectToDelete = new RequestResourceUsageCollection {ID = deleteData.ObjectID};
						//RequestResourceUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = RequestResourceUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RequestResourceUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ProcessorUsageCollection":
					{
						//var objectToDelete = new ProcessorUsageCollection {ID = deleteData.ObjectID};
						//ProcessorUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ProcessorUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ProcessorUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageTransactionUsageCollection":
					{
						//var objectToDelete = new StorageTransactionUsageCollection {ID = deleteData.ObjectID};
						//StorageTransactionUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = StorageTransactionUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageTransactionUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StorageUsageCollection":
					{
						//var objectToDelete = new StorageUsageCollection {ID = deleteData.ObjectID};
						//StorageUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = StorageUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StorageUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NetworkUsageCollection":
					{
						//var objectToDelete = new NetworkUsageCollection {ID = deleteData.ObjectID};
						//NetworkUsageCollectionTable.Attach(objectToDelete);
						var objectToDelete = NetworkUsageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NetworkUsageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "HTTPActivityDetailsCollection":
					{
						//var objectToDelete = new HTTPActivityDetailsCollection {ID = deleteData.ObjectID};
						//HTTPActivityDetailsCollectionTable.Attach(objectToDelete);
						var objectToDelete = HTTPActivityDetailsCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							HTTPActivityDetailsCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



			public Table<Login> LoginTable {
				get {
					return this.GetTable<Login>();
				}
			}
			public Table<LoginAccount> LoginAccountTable {
				get {
					return this.GetTable<LoginAccount>();
				}
			}

			public Table<Email> EmailTable {
				get {
					return this.GetTable<Email>();
				}
			}
			public Table<EmailAccount> EmailAccountTable {
				get {
					return this.GetTable<EmailAccount>();
				}
			}

			public Table<Account> AccountTable {
				get {
					return this.GetTable<Account>();
				}
			}
			public Table<AccountEmails> AccountEmailsTable {
				get {
					return this.GetTable<AccountEmails>();
				}
			}

			public Table<AccountLogins> AccountLoginsTable {
				get {
					return this.GetTable<AccountLogins>();
				}
			}

			public Table<AccountGroupMemberships> AccountGroupMembershipsTable {
				get {
					return this.GetTable<AccountGroupMemberships>();
				}
			}

			public Table<Group> GroupTable {
				get {
					return this.GetTable<Group>();
				}
			}
			public Table<GroupGroupMemberships> GroupGroupMembershipsTable {
				get {
					return this.GetTable<GroupGroupMemberships>();
				}
			}

			public Table<GroupMembership> GroupMembershipTable {
				get {
					return this.GetTable<GroupMembership>();
				}
			}
			public Table<GroupMembershipAccount> GroupMembershipAccountTable {
				get {
					return this.GetTable<GroupMembershipAccount>();
				}
			}

			public Table<GroupMembershipGroup> GroupMembershipGroupTable {
				get {
					return this.GetTable<GroupMembershipGroup>();
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
			public Table<ContentPackageCollection> ContentPackageCollectionTable {
				get {
					return this.GetTable<ContentPackageCollection>();
				}
			}
			public Table<InformationInputCollection> InformationInputCollectionTable {
				get {
					return this.GetTable<InformationInputCollection>();
				}
			}
			public Table<InformationOutputCollection> InformationOutputCollectionTable {
				get {
					return this.GetTable<InformationOutputCollection>();
				}
			}
			public Table<AuthenticatedAsActiveDeviceCollection> AuthenticatedAsActiveDeviceCollectionTable {
				get {
					return this.GetTable<AuthenticatedAsActiveDeviceCollection>();
				}
			}
			public Table<DeviceMembershipCollection> DeviceMembershipCollectionTable {
				get {
					return this.GetTable<DeviceMembershipCollection>();
				}
			}
			public Table<InvoiceCollection> InvoiceCollectionTable {
				get {
					return this.GetTable<InvoiceCollection>();
				}
			}
			public Table<InvoiceUserCollection> InvoiceUserCollectionTable {
				get {
					return this.GetTable<InvoiceUserCollection>();
				}
			}
			public Table<InvoiceRowGroupCollection> InvoiceRowGroupCollectionTable {
				get {
					return this.GetTable<InvoiceRowGroupCollection>();
				}
			}
			public Table<InvoiceEventDetailGroupCollection> InvoiceEventDetailGroupCollectionTable {
				get {
					return this.GetTable<InvoiceEventDetailGroupCollection>();
				}
			}
			public Table<InvoiceEventDetailCollection> InvoiceEventDetailCollectionTable {
				get {
					return this.GetTable<InvoiceEventDetailCollection>();
				}
			}
			public Table<InvoiceRowCollection> InvoiceRowCollectionTable {
				get {
					return this.GetTable<InvoiceRowCollection>();
				}
			}
			public Table<CategoryCollection> CategoryCollectionTable {
				get {
					return this.GetTable<CategoryCollection>();
				}
			}
			public Table<RequestResourceUsageCollection> RequestResourceUsageCollectionTable {
				get {
					return this.GetTable<RequestResourceUsageCollection>();
				}
			}
			public Table<ProcessorUsageCollection> ProcessorUsageCollectionTable {
				get {
					return this.GetTable<ProcessorUsageCollection>();
				}
			}
			public Table<StorageTransactionUsageCollection> StorageTransactionUsageCollectionTable {
				get {
					return this.GetTable<StorageTransactionUsageCollection>();
				}
			}
			public Table<StorageUsageCollection> StorageUsageCollectionTable {
				get {
					return this.GetTable<StorageUsageCollection>();
				}
			}
			public Table<NetworkUsageCollection> NetworkUsageCollectionTable {
				get {
					return this.GetTable<NetworkUsageCollection>();
				}
			}
			public Table<HTTPActivityDetailsCollection> HTTPActivityDetailsCollectionTable {
				get {
					return this.GetTable<HTTPActivityDetailsCollection>();
				}
			}
        }

    [Table(Name = "Login")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Login: {ID}")]
	public class Login : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Login() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Login](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LoginName] TEXT NOT NULL, 
[PasswordHash] TEXT NOT NULL, 
[PasswordSalt] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string LoginName { get; set; }
		// private string _unmodified_LoginName;

		[Column]
        [ScaffoldColumn(true)]
		public string PasswordHash { get; set; }
		// private string _unmodified_PasswordHash;

		[Column]
        [ScaffoldColumn(true)]
		public string PasswordSalt { get; set; }
		// private string _unmodified_PasswordSalt;
		private EntityRef<LoginAccount> _Account = new EntityRef<LoginAccount>();
        [Association(ThisKey = "ID", OtherKey = "LoginID", Storage="_Account")]
        public LoginAccount Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(LoginName == null)
				LoginName = string.Empty;
			if(PasswordHash == null)
				PasswordHash = string.Empty;
			if(PasswordSalt == null)
				PasswordSalt = string.Empty;
		}
	}
    [Table(Name = "Email")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Email: {ID}")]
	public class Email : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Email() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Email](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailAddress] TEXT NOT NULL, 
[PendingValidation] INTEGER NOT NULL, 
[ValidationKey] TEXT NOT NULL, 
[ValidationProcessExpiration] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;
		private EntityRef<EmailAccount> _Account = new EntityRef<EmailAccount>();
        [Association(ThisKey = "ID", OtherKey = "EmailID", Storage="_Account")]
        public EmailAccount Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}


		[Column]
        [ScaffoldColumn(true)]
		public bool PendingValidation { get; set; }
		// private bool _unmodified_PendingValidation;

		[Column]
        [ScaffoldColumn(true)]
		public string ValidationKey { get; set; }
		// private string _unmodified_ValidationKey;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ValidationProcessExpiration { get; set; }
		// private DateTime _unmodified_ValidationProcessExpiration;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(EmailAddress == null)
				EmailAddress = string.Empty;
			if(ValidationKey == null)
				ValidationKey = string.Empty;
		}
	}
    [Table(Name = "Account")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Account: {ID}")]
	public class Account : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Account() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Account](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

)";
        }

		private EntitySet<AccountEmails> _Emails = new EntitySet<AccountEmails>();
        [Association(ThisKey = "ID", OtherKey = "AccountID", Storage="_Emails")]
        public EntitySet<AccountEmails> Emails { 
			get { return _Emails; }
			set { _Emails.Assign(value); }
		}

		private EntitySet<AccountLogins> _Logins = new EntitySet<AccountLogins>();
        [Association(ThisKey = "ID", OtherKey = "AccountID", Storage="_Logins")]
        public EntitySet<AccountLogins> Logins { 
			get { return _Logins; }
			set { _Logins.Assign(value); }
		}

		private EntitySet<AccountGroupMemberships> _GroupMemberships = new EntitySet<AccountGroupMemberships>();
        [Association(ThisKey = "ID", OtherKey = "AccountID", Storage="_GroupMemberships")]
        public EntitySet<AccountGroupMemberships> GroupMemberships { 
			get { return _GroupMemberships; }
			set { _GroupMemberships.Assign(value); }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Group")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Group: {ID}")]
	public class Group : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Group() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Group](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

)";
        }

		private EntitySet<GroupGroupMemberships> _GroupMemberships = new EntitySet<GroupGroupMemberships>();
        [Association(ThisKey = "ID", OtherKey = "GroupID", Storage="_GroupMemberships")]
        public EntitySet<GroupGroupMemberships> GroupMemberships { 
			get { return _GroupMemberships; }
			set { _GroupMemberships.Assign(value); }
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupMembership")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupMembership: {ID}")]
	public class GroupMembership : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupMembership() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupMembership](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Role] TEXT NOT NULL
)";
        }

		private EntityRef<GroupMembershipAccount> _Account = new EntityRef<GroupMembershipAccount>();
        [Association(ThisKey = "ID", OtherKey = "GroupMembershipID", Storage="_Account")]
        public GroupMembershipAccount Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

		private EntityRef<GroupMembershipGroup> _Group = new EntityRef<GroupMembershipGroup>();
        [Association(ThisKey = "ID", OtherKey = "GroupMembershipID", Storage="_Group")]
        public GroupMembershipGroup Group 
		{ 
			get { return _Group.Entity; }
			set { _Group.Entity = value; }
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Role { get; set; }
		// private string _unmodified_Role;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Role == null)
				Role = string.Empty;
		}
	}
    [Table(Name = "ContentPackage")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ContentPackage: {ID}")]
	public class ContentPackage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PackageType] TEXT NOT NULL, 
[PackageName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[PackageRootFolder] TEXT NOT NULL, 
[CreationTime] TEXT NOT NULL
)";
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
	[DebuggerDisplay("InformationInput: {ID}")]
	public class InformationInput : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationInput](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[InputDescription] TEXT NOT NULL, 
[LocationURL] TEXT NOT NULL, 
[LocalContentName] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("InformationOutput: {ID}")]
	public class InformationOutput : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationOutput](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OutputDescription] TEXT NOT NULL, 
[DestinationURL] TEXT NOT NULL, 
[DestinationContentName] TEXT NOT NULL, 
[LocalContentURL] TEXT NOT NULL, 
[AuthenticatedDeviceID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("AuthenticatedAsActiveDevice: {ID}")]
	public class AuthenticatedAsActiveDevice : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AuthenticatedAsActiveDevice](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AuthenticationDescription] TEXT NOT NULL, 
[SharedSecret] TEXT NOT NULL, 
[ActiveSymmetricAESKey] BLOB NOT NULL, 
[EstablishedTrustID] TEXT NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL, 
[NegotiationURL] TEXT NOT NULL, 
[ConnectionURL] TEXT NOT NULL
)";
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
	[DebuggerDisplay("DeviceMembership: {ID}")]
	public class DeviceMembership : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeviceMembership](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[DeviceDescription] TEXT NOT NULL, 
[SharedSecret] TEXT NOT NULL, 
[ActiveSymmetricAESKey] BLOB NOT NULL, 
[IsValidatedAndActive] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("InvoiceFiscalExportSummary: {ID}")]
	public class InvoiceFiscalExportSummary : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceFiscalExportSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[FiscalInclusiveStartDate] TEXT NOT NULL, 
[FiscalInclusiveEndDate] TEXT NOT NULL, 
[ExportedInvoicesID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public DateTime FiscalInclusiveStartDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveStartDate;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime FiscalInclusiveEndDate { get; set; }
		// private DateTime _unmodified_FiscalInclusiveEndDate;
			[Column]
			public string ExportedInvoicesID { get; set; }
			private EntityRef< InvoiceCollection > _ExportedInvoices;
			[Association(Storage = "_ExportedInvoices", ThisKey = "ExportedInvoicesID")]
			public InvoiceCollection ExportedInvoices
			{
				get { return this._ExportedInvoices.Entity; }
				set { this._ExportedInvoices.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "InvoiceSummaryContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceSummaryContainer: {ID}")]
	public class InvoiceSummaryContainer : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OpenInvoicesID] TEXT NULL, 
[PredictedInvoicesID] TEXT NULL, 
[PaidInvoicesActiveYearID] TEXT NULL, 
[PaidInvoicesLast12MonthsID] TEXT NULL
)";
        }

			[Column]
			public string OpenInvoicesID { get; set; }
			private EntityRef< InvoiceCollection > _OpenInvoices;
			[Association(Storage = "_OpenInvoices", ThisKey = "OpenInvoicesID")]
			public InvoiceCollection OpenInvoices
			{
				get { return this._OpenInvoices.Entity; }
				set { this._OpenInvoices.Entity = value; }
			}
			[Column]
			public string PredictedInvoicesID { get; set; }
			private EntityRef< InvoiceCollection > _PredictedInvoices;
			[Association(Storage = "_PredictedInvoices", ThisKey = "PredictedInvoicesID")]
			public InvoiceCollection PredictedInvoices
			{
				get { return this._PredictedInvoices.Entity; }
				set { this._PredictedInvoices.Entity = value; }
			}
			[Column]
			public string PaidInvoicesActiveYearID { get; set; }
			private EntityRef< InvoiceCollection > _PaidInvoicesActiveYear;
			[Association(Storage = "_PaidInvoicesActiveYear", ThisKey = "PaidInvoicesActiveYearID")]
			public InvoiceCollection PaidInvoicesActiveYear
			{
				get { return this._PaidInvoicesActiveYear.Entity; }
				set { this._PaidInvoicesActiveYear.Entity = value; }
			}
			[Column]
			public string PaidInvoicesLast12MonthsID { get; set; }
			private EntityRef< InvoiceCollection > _PaidInvoicesLast12Months;
			[Association(Storage = "_PaidInvoicesLast12Months", ThisKey = "PaidInvoicesLast12MonthsID")]
			public InvoiceCollection PaidInvoicesLast12Months
			{
				get { return this._PaidInvoicesLast12Months.Entity; }
				set { this._PaidInvoicesLast12Months.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Invoice")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Invoice: {ID}")]
	public class Invoice : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Invoice](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
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

			[Column]
			public string InvoiceUsersID { get; set; }
			private EntityRef< InvoiceUserCollection > _InvoiceUsers;
			[Association(Storage = "_InvoiceUsers", ThisKey = "InvoiceUsersID")]
			public InvoiceUserCollection InvoiceUsers
			{
				get { return this._InvoiceUsers.Entity; }
				set { this._InvoiceUsers.Entity = value; }
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
	[DebuggerDisplay("InvoiceDetails: {ID}")]
	public class InvoiceDetails : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceDetails](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[MonthlyFeesTotal] TEXT NOT NULL, 
[OneTimeFeesTotal] TEXT NOT NULL, 
[UsageFeesTotal] TEXT NOT NULL, 
[InterestFeesTotal] TEXT NOT NULL, 
[PenaltyFeesTotal] TEXT NOT NULL, 
[TotalFeesTotal] TEXT NOT NULL
)";
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
	[DebuggerDisplay("InvoiceUser: {ID}")]
	public class InvoiceUser : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceUser](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[UserName] TEXT NOT NULL, 
[UserID] TEXT NOT NULL, 
[UserPhoneNumber] TEXT NOT NULL, 
[UserSubscriptionNumber] TEXT NOT NULL, 
[UserInvoiceTotalAmount] TEXT NOT NULL, 
[InvoiceRowGroupCollectionID] TEXT NULL, 
[InvoiceEventDetailGroupCollectionID] TEXT NULL
)";
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
			[Column]
			public string InvoiceRowGroupCollectionID { get; set; }
			private EntityRef< InvoiceRowGroupCollection > _InvoiceRowGroupCollection;
			[Association(Storage = "_InvoiceRowGroupCollection", ThisKey = "InvoiceRowGroupCollectionID")]
			public InvoiceRowGroupCollection InvoiceRowGroupCollection
			{
				get { return this._InvoiceRowGroupCollection.Entity; }
				set { this._InvoiceRowGroupCollection.Entity = value; }
			}
			[Column]
			public string InvoiceEventDetailGroupCollectionID { get; set; }
			private EntityRef< InvoiceEventDetailGroupCollection > _InvoiceEventDetailGroupCollection;
			[Association(Storage = "_InvoiceEventDetailGroupCollection", ThisKey = "InvoiceEventDetailGroupCollectionID")]
			public InvoiceEventDetailGroupCollection InvoiceEventDetailGroupCollection
			{
				get { return this._InvoiceEventDetailGroupCollection.Entity; }
				set { this._InvoiceEventDetailGroupCollection.Entity = value; }
			}
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
	[DebuggerDisplay("InvoiceRowGroup: {ID}")]
	public class InvoiceRowGroup : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRowGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupName] TEXT NOT NULL, 
[GroupTotalPriceWithoutTaxes] TEXT NOT NULL, 
[GroupTotalTaxes] TEXT NOT NULL, 
[GroupTotalPriceWithTaxes] TEXT NOT NULL, 
[InvoiceRowCollectionID] TEXT NULL
)";
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
			[Column]
			public string InvoiceRowCollectionID { get; set; }
			private EntityRef< InvoiceRowCollection > _InvoiceRowCollection;
			[Association(Storage = "_InvoiceRowCollection", ThisKey = "InvoiceRowCollectionID")]
			public InvoiceRowCollection InvoiceRowCollection
			{
				get { return this._InvoiceRowCollection.Entity; }
				set { this._InvoiceRowCollection.Entity = value; }
			}
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
	[DebuggerDisplay("InvoiceEventDetailGroup: {ID}")]
	public class InvoiceEventDetailGroup : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetailGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupName] TEXT NOT NULL, 
[InvoiceEventDetailCollectionID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;
			[Column]
			public string InvoiceEventDetailCollectionID { get; set; }
			private EntityRef< InvoiceEventDetailCollection > _InvoiceEventDetailCollection;
			[Association(Storage = "_InvoiceEventDetailCollection", ThisKey = "InvoiceEventDetailCollectionID")]
			public InvoiceEventDetailCollection InvoiceEventDetailCollection
			{
				get { return this._InvoiceEventDetailCollection.Entity; }
				set { this._InvoiceEventDetailCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupName == null)
				GroupName = string.Empty;
		}
	}
    [Table(Name = "InvoiceEventDetail")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceEventDetail: {ID}")]
	public class InvoiceEventDetail : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
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
	[DebuggerDisplay("InvoiceRow: {ID}")]
	public class InvoiceRow : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRow](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IndentMode] TEXT NOT NULL, 
[AmountOfUnits] TEXT NOT NULL, 
[Duration] TEXT NOT NULL, 
[UnitPrice] TEXT NOT NULL, 
[PriceWithoutTaxes] TEXT NOT NULL, 
[Taxes] TEXT NOT NULL, 
[PriceWithTaxes] TEXT NOT NULL
)";
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
	[DebuggerDisplay("Category: {ID}")]
	public class Category : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Category](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CategoryName] TEXT NOT NULL
)";
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
	[DebuggerDisplay("ProcessContainer: {ID}")]
	public class ProcessContainer : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProcessIDs] TEXT NOT NULL
)";
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
	[DebuggerDisplay("Process: {ID}")]
	public class Process : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Process](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProcessDescription] TEXT NOT NULL, 
[ExecutingOperationID] TEXT NULL, 
[InitialArgumentsID] TEXT NULL, 
[ProcessItemsID] TEXT NULL
)";
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
	[DebuggerDisplay("ProcessItem: {ID}")]
	public class ProcessItem : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OutputsID] TEXT NULL, 
[InputsID] TEXT NULL
)";
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
	[DebuggerDisplay("SemanticInformationItem: {ID}")]
	public class SemanticInformationItem : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SemanticInformationItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ItemFullType] TEXT NOT NULL, 
[ItemValue] TEXT NOT NULL
)";
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
	[DebuggerDisplay("InformationOwnerInfo: {ID}")]
	public class InformationOwnerInfo : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationOwnerInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OwnerType] TEXT NOT NULL, 
[OwnerIdentifier] TEXT NOT NULL
)";
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
	[DebuggerDisplay("UsageSummary: {ID}")]
	public class UsageSummary : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UsageSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SummaryName] TEXT NOT NULL, 
[SummaryMonitoringItemID] TEXT NULL
)";
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
	[DebuggerDisplay("UsageMonitorItem: {ID}")]
	public class UsageMonitorItem : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UsageMonitorItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OwnerInfoID] TEXT NULL, 
[TimeRangeInclusiveStartExclusiveEndID] TEXT NULL, 
[StepSizeInMinutes] INTEGER NOT NULL, 
[ProcessorUsagesID] TEXT NULL, 
[StorageTransactionUsagesID] TEXT NULL, 
[StorageUsagesID] TEXT NULL, 
[NetworkUsagesID] TEXT NULL
)";
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
			[Column]
			public string ProcessorUsagesID { get; set; }
			private EntityRef< ProcessorUsageCollection > _ProcessorUsages;
			[Association(Storage = "_ProcessorUsages", ThisKey = "ProcessorUsagesID")]
			public ProcessorUsageCollection ProcessorUsages
			{
				get { return this._ProcessorUsages.Entity; }
				set { this._ProcessorUsages.Entity = value; }
			}
			[Column]
			public string StorageTransactionUsagesID { get; set; }
			private EntityRef< StorageTransactionUsageCollection > _StorageTransactionUsages;
			[Association(Storage = "_StorageTransactionUsages", ThisKey = "StorageTransactionUsagesID")]
			public StorageTransactionUsageCollection StorageTransactionUsages
			{
				get { return this._StorageTransactionUsages.Entity; }
				set { this._StorageTransactionUsages.Entity = value; }
			}
			[Column]
			public string StorageUsagesID { get; set; }
			private EntityRef< StorageUsageCollection > _StorageUsages;
			[Association(Storage = "_StorageUsages", ThisKey = "StorageUsagesID")]
			public StorageUsageCollection StorageUsages
			{
				get { return this._StorageUsages.Entity; }
				set { this._StorageUsages.Entity = value; }
			}
			[Column]
			public string NetworkUsagesID { get; set; }
			private EntityRef< NetworkUsageCollection > _NetworkUsages;
			[Association(Storage = "_NetworkUsages", ThisKey = "NetworkUsagesID")]
			public NetworkUsageCollection NetworkUsages
			{
				get { return this._NetworkUsages.Entity; }
				set { this._NetworkUsages.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RequestResourceUsage")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("RequestResourceUsage: {ID}")]
	public class RequestResourceUsage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RequestResourceUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OwnerInfoID] TEXT NULL, 
[ProcessorUsageID] TEXT NULL, 
[StorageTransactionUsageID] TEXT NULL, 
[NetworkUsageID] TEXT NULL, 
[RequestDetailsID] TEXT NULL
)";
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
	[DebuggerDisplay("ProcessorUsage: {ID}")]
	public class ProcessorUsage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessorUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTicks] REAL NOT NULL, 
[FrequencyTicksPerSecond] REAL NOT NULL, 
[Milliseconds] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("StorageTransactionUsage: {ID}")]
	public class StorageTransactionUsage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageTransactionUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfTransactions] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("StorageUsage: {ID}")]
	public class StorageUsage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SnapshotTime] TEXT NOT NULL, 
[UsageType] TEXT NOT NULL, 
[UsageUnit] TEXT NOT NULL, 
[AmountOfUnits] REAL NOT NULL
)";
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
	[DebuggerDisplay("NetworkUsage: {ID}")]
	public class NetworkUsage : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NetworkUsage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TimeRangeID] TEXT NULL, 
[UsageType] TEXT NOT NULL, 
[AmountOfBytes] INTEGER NOT NULL
)";
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
	[DebuggerDisplay("TimeRange: {ID}")]
	public class TimeRange : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TimeRange](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[StartTime] TEXT NOT NULL, 
[EndTime] TEXT NOT NULL
)";
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
	[DebuggerDisplay("HTTPActivityDetails: {ID}")]
	public class HTTPActivityDetails : ITheBallDataContextStorable
	{

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

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [HTTPActivityDetails](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[RemoteIPAddress] TEXT NOT NULL, 
[RemoteEndpointUserName] TEXT NOT NULL, 
[UserID] TEXT NOT NULL, 
[UTCDateTime] TEXT NOT NULL, 
[RequestLine] TEXT NOT NULL, 
[HTTPStatusCode] INTEGER NOT NULL, 
[ReturnedContentLength] INTEGER NOT NULL
)";
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
    [Table(Name = "ContentPackageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ContentPackageCollection: {ID}")]
	public class ContentPackageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ContentPackageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentPackageCollection](
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
    [Table(Name = "InformationInputCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InformationInputCollection: {ID}")]
	public class InformationInputCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InformationInputCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationInputCollection](
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
    [Table(Name = "InformationOutputCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InformationOutputCollection: {ID}")]
	public class InformationOutputCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InformationOutputCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationOutputCollection](
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
    [Table(Name = "AuthenticatedAsActiveDeviceCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AuthenticatedAsActiveDeviceCollection: {ID}")]
	public class AuthenticatedAsActiveDeviceCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AuthenticatedAsActiveDeviceCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AuthenticatedAsActiveDeviceCollection](
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
    [Table(Name = "DeviceMembershipCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("DeviceMembershipCollection: {ID}")]
	public class DeviceMembershipCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public DeviceMembershipCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeviceMembershipCollection](
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
    [Table(Name = "InvoiceCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceCollection: {ID}")]
	public class InvoiceCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceCollection](
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
    [Table(Name = "InvoiceUserCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceUserCollection: {ID}")]
	public class InvoiceUserCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceUserCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceUserCollection](
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
    [Table(Name = "InvoiceRowGroupCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceRowGroupCollection: {ID}")]
	public class InvoiceRowGroupCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceRowGroupCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRowGroupCollection](
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
    [Table(Name = "InvoiceEventDetailGroupCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceEventDetailGroupCollection: {ID}")]
	public class InvoiceEventDetailGroupCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceEventDetailGroupCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetailGroupCollection](
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
    [Table(Name = "InvoiceEventDetailCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceEventDetailCollection: {ID}")]
	public class InvoiceEventDetailCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceEventDetailCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceEventDetailCollection](
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
    [Table(Name = "InvoiceRowCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("InvoiceRowCollection: {ID}")]
	public class InvoiceRowCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public InvoiceRowCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InvoiceRowCollection](
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
    [Table(Name = "CategoryCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CategoryCollection: {ID}")]
	public class CategoryCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CategoryCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CategoryCollection](
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
    [Table(Name = "RequestResourceUsageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("RequestResourceUsageCollection: {ID}")]
	public class RequestResourceUsageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public RequestResourceUsageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RequestResourceUsageCollection](
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
    [Table(Name = "ProcessorUsageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ProcessorUsageCollection: {ID}")]
	public class ProcessorUsageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ProcessorUsageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProcessorUsageCollection](
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
    [Table(Name = "StorageTransactionUsageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("StorageTransactionUsageCollection: {ID}")]
	public class StorageTransactionUsageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public StorageTransactionUsageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageTransactionUsageCollection](
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
    [Table(Name = "StorageUsageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("StorageUsageCollection: {ID}")]
	public class StorageUsageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public StorageUsageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StorageUsageCollection](
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
    [Table(Name = "NetworkUsageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("NetworkUsageCollection: {ID}")]
	public class NetworkUsageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public NetworkUsageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NetworkUsageCollection](
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
    [Table(Name = "HTTPActivityDetailsCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("HTTPActivityDetailsCollection: {ID}")]
	public class HTTPActivityDetailsCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public HTTPActivityDetailsCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [HTTPActivityDetailsCollection](
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
    [Table(Name = "LoginAccount")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LoginAccount: {LoginID} - {AccountID}")]
	public class LoginAccount // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LoginAccount](
[LoginID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL,
PRIMARY KEY (LoginID, AccountID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string LoginID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }


        private EntityRef<Login> _Login = new EntityRef<Login>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "LoginID", OtherKey = "ID", 
			Storage = "_Login", IsUnique = true)]
        public Login Login 
		{ 
			get { return _Login.Entity; }
			set { _Login.Entity = value; }
		}

        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account")]
		public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

    }

    [Table(Name = "EmailAccount")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("EmailAccount: {EmailID} - {AccountID}")]
	public class EmailAccount // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [EmailAccount](
[EmailID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL,
PRIMARY KEY (EmailID, AccountID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string EmailID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }


        private EntityRef<Email> _Email = new EntityRef<Email>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "EmailID", OtherKey = "ID", 
			Storage = "_Email", IsUnique = true)]
        public Email Email 
		{ 
			get { return _Email.Entity; }
			set { _Email.Entity = value; }
		}

        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account")]
		public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

    }

    [Table(Name = "AccountEmails")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountEmails: {AccountID} - {EmailID}")]
	public class AccountEmails // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountEmails](
[AccountID] TEXT NOT NULL, 
[EmailID] TEXT NOT NULL,
PRIMARY KEY (AccountID, EmailID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string EmailID { get; set; }


        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account", IsUnique = false)]
        public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

        private EntityRef<Email> _Email = new EntityRef<Email>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "EmailID", OtherKey = "ID", 
			Storage = "_Email")]
		public Email Email 
		{ 
			get { return _Email.Entity; }
			set { _Email.Entity = value; }
		}

    }

    [Table(Name = "AccountLogins")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountLogins: {AccountID} - {LoginID}")]
	public class AccountLogins // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountLogins](
[AccountID] TEXT NOT NULL, 
[LoginID] TEXT NOT NULL,
PRIMARY KEY (AccountID, LoginID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string LoginID { get; set; }


        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account", IsUnique = false)]
        public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

        private EntityRef<Login> _Login = new EntityRef<Login>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "LoginID", OtherKey = "ID", 
			Storage = "_Login")]
		public Login Login 
		{ 
			get { return _Login.Entity; }
			set { _Login.Entity = value; }
		}

    }

    [Table(Name = "AccountGroupMemberships")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountGroupMemberships: {AccountID} - {GroupMembershipID}")]
	public class AccountGroupMemberships // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountGroupMemberships](
[AccountID] TEXT NOT NULL, 
[GroupMembershipID] TEXT NOT NULL,
PRIMARY KEY (AccountID, GroupMembershipID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupMembershipID { get; set; }


        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account", IsUnique = false)]
        public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

        private EntityRef<GroupMembership> _GroupMembership = new EntityRef<GroupMembership>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupMembershipID", OtherKey = "ID", 
			Storage = "_GroupMembership")]
		public GroupMembership GroupMembership 
		{ 
			get { return _GroupMembership.Entity; }
			set { _GroupMembership.Entity = value; }
		}

    }

    [Table(Name = "GroupGroupMemberships")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupGroupMemberships: {GroupID} - {GroupMembershipID}")]
	public class GroupGroupMemberships // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupGroupMemberships](
[GroupID] TEXT NOT NULL, 
[GroupMembershipID] TEXT NOT NULL,
PRIMARY KEY (GroupID, GroupMembershipID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupMembershipID { get; set; }


        private EntityRef<Group> _Group = new EntityRef<Group>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupID", OtherKey = "ID", 
			Storage = "_Group", IsUnique = false)]
        public Group Group 
		{ 
			get { return _Group.Entity; }
			set { _Group.Entity = value; }
		}

        private EntityRef<GroupMembership> _GroupMembership = new EntityRef<GroupMembership>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupMembershipID", OtherKey = "ID", 
			Storage = "_GroupMembership")]
		public GroupMembership GroupMembership 
		{ 
			get { return _GroupMembership.Entity; }
			set { _GroupMembership.Entity = value; }
		}

    }

    [Table(Name = "GroupMembershipAccount")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupMembershipAccount: {GroupMembershipID} - {AccountID}")]
	public class GroupMembershipAccount // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupMembershipAccount](
[GroupMembershipID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL,
PRIMARY KEY (GroupMembershipID, AccountID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupMembershipID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string AccountID { get; set; }


        private EntityRef<GroupMembership> _GroupMembership = new EntityRef<GroupMembership>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupMembershipID", OtherKey = "ID", 
			Storage = "_GroupMembership", IsUnique = true)]
        public GroupMembership GroupMembership 
		{ 
			get { return _GroupMembership.Entity; }
			set { _GroupMembership.Entity = value; }
		}

        private EntityRef<Account> _Account = new EntityRef<Account>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "AccountID", OtherKey = "ID", 
			Storage = "_Account")]
		public Account Account 
		{ 
			get { return _Account.Entity; }
			set { _Account.Entity = value; }
		}

    }

    [Table(Name = "GroupMembershipGroup")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupMembershipGroup: {GroupMembershipID} - {GroupID}")]
	public class GroupMembershipGroup // : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupMembershipGroup](
[GroupMembershipID] TEXT NOT NULL, 
[GroupID] TEXT NOT NULL,
PRIMARY KEY (GroupMembershipID, GroupID)
)";
        }


        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupMembershipID { get; set; }
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string GroupID { get; set; }


        private EntityRef<GroupMembership> _GroupMembership = new EntityRef<GroupMembership>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupMembershipID", OtherKey = "ID", 
			Storage = "_GroupMembership", IsUnique = true)]
        public GroupMembership GroupMembership 
		{ 
			get { return _GroupMembership.Entity; }
			set { _GroupMembership.Entity = value; }
		}

        private EntityRef<Group> _Group = new EntityRef<Group>();
        [Association(DeleteOnNull = true, IsForeignKey = true, ThisKey = "GroupID", OtherKey = "ID", 
			Storage = "_Group")]
		public Group Group 
		{ 
			get { return _Group.Entity; }
			set { _Group.Entity = value; }
		}

    }

 } 
