 


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


namespace SQLite.AaltoGlobalImpact.OIP { 
		
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
				tableCreationCommands.Add(TBSystem.GetCreateTableSQL());
				tableCreationCommands.Add(WebPublishInfo.GetCreateTableSQL());
				tableCreationCommands.Add(PublicationPackage.GetCreateTableSQL());
				tableCreationCommands.Add(TBRLoginRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBRAccountRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBRGroupRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBRLoginGroupRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBREmailRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBAccount.GetCreateTableSQL());
				tableCreationCommands.Add(TBAccountCollaborationGroup.GetCreateTableSQL());
				tableCreationCommands.Add(TBLoginInfo.GetCreateTableSQL());
				tableCreationCommands.Add(TBEmail.GetCreateTableSQL());
				tableCreationCommands.Add(TBCollaboratorRole.GetCreateTableSQL());
				tableCreationCommands.Add(TBCollaboratingGroup.GetCreateTableSQL());
				tableCreationCommands.Add(TBEmailValidation.GetCreateTableSQL());
				tableCreationCommands.Add(TBMergeAccountConfirmation.GetCreateTableSQL());
				tableCreationCommands.Add(TBGroupJoinConfirmation.GetCreateTableSQL());
				tableCreationCommands.Add(TBDeviceJoinConfirmation.GetCreateTableSQL());
				tableCreationCommands.Add(TBInformationInputConfirmation.GetCreateTableSQL());
				tableCreationCommands.Add(TBInformationOutputConfirmation.GetCreateTableSQL());
				tableCreationCommands.Add(LoginProvider.GetCreateTableSQL());
				tableCreationCommands.Add(TBPRegisterEmail.GetCreateTableSQL());
				tableCreationCommands.Add(AccountSummary.GetCreateTableSQL());
				tableCreationCommands.Add(AccountContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AccountModule.GetCreateTableSQL());
				tableCreationCommands.Add(LocationContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AddressAndLocation.GetCreateTableSQL());
				tableCreationCommands.Add(StreetAddress.GetCreateTableSQL());
				tableCreationCommands.Add(AccountProfile.GetCreateTableSQL());
				tableCreationCommands.Add(AccountSecurity.GetCreateTableSQL());
				tableCreationCommands.Add(AccountRoles.GetCreateTableSQL());
				tableCreationCommands.Add(PersonalInfoVisibility.GetCreateTableSQL());
				tableCreationCommands.Add(ReferenceToInformation.GetCreateTableSQL());
				tableCreationCommands.Add(NodeSummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(RenderedNode.GetCreateTableSQL());
				tableCreationCommands.Add(ShortTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(LongTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(MapMarker.GetCreateTableSQL());
				tableCreationCommands.Add(Moderator.GetCreateTableSQL());
				tableCreationCommands.Add(Collaborator.GetCreateTableSQL());
				tableCreationCommands.Add(GroupSummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(GroupContainer.GetCreateTableSQL());
				tableCreationCommands.Add(GroupIndex.GetCreateTableSQL());
				tableCreationCommands.Add(AddAddressAndLocationInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddImageInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddImageGroupInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddEmailAddressInfo.GetCreateTableSQL());
				tableCreationCommands.Add(CreateGroupInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddActivityInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddBlogPostInfo.GetCreateTableSQL());
				tableCreationCommands.Add(AddCategoryInfo.GetCreateTableSQL());
				tableCreationCommands.Add(Group.GetCreateTableSQL());
				tableCreationCommands.Add(Introduction.GetCreateTableSQL());
				tableCreationCommands.Add(ContentCategoryRank.GetCreateTableSQL());
				tableCreationCommands.Add(LinkToContent.GetCreateTableSQL());
				tableCreationCommands.Add(EmbeddedContent.GetCreateTableSQL());
				tableCreationCommands.Add(DynamicContentGroup.GetCreateTableSQL());
				tableCreationCommands.Add(DynamicContent.GetCreateTableSQL());
				tableCreationCommands.Add(AttachedToObject.GetCreateTableSQL());
				tableCreationCommands.Add(Comment.GetCreateTableSQL());
				tableCreationCommands.Add(Selection.GetCreateTableSQL());
				tableCreationCommands.Add(TextContent.GetCreateTableSQL());
				tableCreationCommands.Add(Map.GetCreateTableSQL());
				tableCreationCommands.Add(MapResult.GetCreateTableSQL());
				tableCreationCommands.Add(MapResultsCollection.GetCreateTableSQL());
				tableCreationCommands.Add(Video.GetCreateTableSQL());
				tableCreationCommands.Add(Image.GetCreateTableSQL());
				tableCreationCommands.Add(BinaryFile.GetCreateTableSQL());
				tableCreationCommands.Add(Longitude.GetCreateTableSQL());
				tableCreationCommands.Add(Latitude.GetCreateTableSQL());
				tableCreationCommands.Add(Location.GetCreateTableSQL());
				tableCreationCommands.Add(Date.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(UpdateWebContentOperation.GetCreateTableSQL());
				tableCreationCommands.Add(UpdateWebContentHandlerItem.GetCreateTableSQL());
				tableCreationCommands.Add(PublicationPackageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TBAccountCollaborationGroupCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TBLoginInfoCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TBEmailCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TBCollaboratorRoleCollection.GetCreateTableSQL());
				tableCreationCommands.Add(LoginProviderCollection.GetCreateTableSQL());
				tableCreationCommands.Add(AddressAndLocationCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ReferenceCollection.GetCreateTableSQL());
				tableCreationCommands.Add(RenderedNodeCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ShortTextCollection.GetCreateTableSQL());
				tableCreationCommands.Add(LongTextCollection.GetCreateTableSQL());
				tableCreationCommands.Add(MapMarkerCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ModeratorCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CollaboratorCollection.GetCreateTableSQL());
				tableCreationCommands.Add(GroupCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ContentCategoryRankCollection.GetCreateTableSQL());
				tableCreationCommands.Add(LinkToContentCollection.GetCreateTableSQL());
				tableCreationCommands.Add(EmbeddedContentCollection.GetCreateTableSQL());
				tableCreationCommands.Add(DynamicContentGroupCollection.GetCreateTableSQL());
				tableCreationCommands.Add(DynamicContentCollection.GetCreateTableSQL());
				tableCreationCommands.Add(AttachedToObjectCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CommentCollection.GetCreateTableSQL());
				tableCreationCommands.Add(SelectionCollection.GetCreateTableSQL());
				tableCreationCommands.Add(TextContentCollection.GetCreateTableSQL());
				tableCreationCommands.Add(MapCollection.GetCreateTableSQL());
				tableCreationCommands.Add(MapResultCollection.GetCreateTableSQL());
				tableCreationCommands.Add(ImageCollection.GetCreateTableSQL());
				tableCreationCommands.Add(BinaryFileCollection.GetCreateTableSQL());
				tableCreationCommands.Add(LocationCollection.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryCollection.GetCreateTableSQL());
				tableCreationCommands.Add(UpdateWebContentHandlerCollection.GetCreateTableSQL());
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
                if(updateData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "TBSystem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBSystemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InstanceName = serializedObject.InstanceName;
		            existingObject.AdminGroupID = serializedObject.AdminGroupID;
		            break;
		        } 
		        case "WebPublishInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.WebPublishInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = WebPublishInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PublishType = serializedObject.PublishType;
		            existingObject.PublishContainer = serializedObject.PublishContainer;
					if(serializedObject.ActivePublication != null)
						existingObject.ActivePublicationID = serializedObject.ActivePublication.ID;
					else
						existingObject.ActivePublicationID = null;
					if(serializedObject.Publications != null)
						existingObject.PublicationsID = serializedObject.Publications.ID;
					else
						existingObject.PublicationsID = null;
		            break;
		        } 
		        case "PublicationPackage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PublicationPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PackageName = serializedObject.PackageName;
		            existingObject.PublicationTime = serializedObject.PublicationTime;
		            break;
		        } 
		        case "TBRLoginRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginRoot.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRLoginRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.DomainName = serializedObject.DomainName;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBRAccountRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRAccountRoot.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRAccountRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBRGroupRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRGroupRoot.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRGroupRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Group != null)
						existingObject.GroupID = serializedObject.Group.ID;
					else
						existingObject.GroupID = null;
		            break;
		        } 
		        case "TBRLoginGroupRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRLoginGroupRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Role = serializedObject.Role;
		            existingObject.GroupID = serializedObject.GroupID;
		            break;
		        } 
		        case "TBREmailRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBREmailRoot.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBREmailRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBAccount":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Emails != null)
						existingObject.EmailsID = serializedObject.Emails.ID;
					else
						existingObject.EmailsID = null;
					if(serializedObject.Logins != null)
						existingObject.LoginsID = serializedObject.Logins.ID;
					else
						existingObject.LoginsID = null;
					if(serializedObject.GroupRoleCollection != null)
						existingObject.GroupRoleCollectionID = serializedObject.GroupRoleCollection.ID;
					else
						existingObject.GroupRoleCollectionID = null;
		            break;
		        } 
		        case "TBAccountCollaborationGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBAccountCollaborationGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.GroupRole = serializedObject.GroupRole;
		            existingObject.RoleStatus = serializedObject.RoleStatus;
		            break;
		        } 
		        case "TBLoginInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBLoginInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OpenIDUrl = serializedObject.OpenIDUrl;
		            break;
		        } 
		        case "TBEmail":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.ValidatedAt = serializedObject.ValidatedAt;
		            break;
		        } 
		        case "TBCollaboratorRole":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBCollaboratorRole.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBCollaboratorRoleTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Email != null)
						existingObject.EmailID = serializedObject.Email.ID;
					else
						existingObject.EmailID = null;
		            existingObject.Role = serializedObject.Role;
		            existingObject.RoleStatus = serializedObject.RoleStatus;
		            break;
		        } 
		        case "TBCollaboratingGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBCollaboratingGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
					if(serializedObject.Roles != null)
						existingObject.RolesID = serializedObject.Roles.ID;
					else
						existingObject.RolesID = null;
		            break;
		        } 
		        case "TBEmailValidation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBEmailValidation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBEmailValidationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Email = serializedObject.Email;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.ValidUntil = serializedObject.ValidUntil;
					if(serializedObject.GroupJoinConfirmation != null)
						existingObject.GroupJoinConfirmationID = serializedObject.GroupJoinConfirmation.ID;
					else
						existingObject.GroupJoinConfirmationID = null;
					if(serializedObject.DeviceJoinConfirmation != null)
						existingObject.DeviceJoinConfirmationID = serializedObject.DeviceJoinConfirmation.ID;
					else
						existingObject.DeviceJoinConfirmationID = null;
					if(serializedObject.InformationInputConfirmation != null)
						existingObject.InformationInputConfirmationID = serializedObject.InformationInputConfirmation.ID;
					else
						existingObject.InformationInputConfirmationID = null;
					if(serializedObject.InformationOutputConfirmation != null)
						existingObject.InformationOutputConfirmationID = serializedObject.InformationOutputConfirmation.ID;
					else
						existingObject.InformationOutputConfirmationID = null;
					if(serializedObject.MergeAccountsConfirmation != null)
						existingObject.MergeAccountsConfirmationID = serializedObject.MergeAccountsConfirmation.ID;
					else
						existingObject.MergeAccountsConfirmationID = null;
		            existingObject.RedirectUrlAfterValidation = serializedObject.RedirectUrlAfterValidation;
		            break;
		        } 
		        case "TBMergeAccountConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBMergeAccountConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            existingObject.AccountToMergeToID = serializedObject.AccountToMergeToID;
		            break;
		        } 
		        case "TBGroupJoinConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBGroupJoinConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.InvitationMode = serializedObject.InvitationMode;
		            break;
		        } 
		        case "TBDeviceJoinConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBDeviceJoinConfirmation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBDeviceJoinConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.DeviceMembershipID = serializedObject.DeviceMembershipID;
		            break;
		        } 
		        case "TBInformationInputConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBInformationInputConfirmation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBInformationInputConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.InformationInputID = serializedObject.InformationInputID;
		            break;
		        } 
		        case "TBInformationOutputConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBInformationOutputConfirmation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBInformationOutputConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.InformationOutputID = serializedObject.InformationOutputID;
		            break;
		        } 
		        case "LoginProvider":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LoginProvider.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LoginProviderTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ProviderName = serializedObject.ProviderName;
		            existingObject.ProviderIconClass = serializedObject.ProviderIconClass;
		            existingObject.ProviderType = serializedObject.ProviderType;
		            existingObject.ProviderUrl = serializedObject.ProviderUrl;
		            existingObject.ReturnUrl = serializedObject.ReturnUrl;
		            break;
		        } 
		        case "TBPRegisterEmail":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBPRegisterEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            break;
		        } 
		        case "AccountSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.GroupSummary != null)
						existingObject.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						existingObject.GroupSummaryID = null;
		            break;
		        } 
		        case "AccountContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.AccountModule != null)
						existingObject.AccountModuleID = serializedObject.AccountModule.ID;
					else
						existingObject.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						existingObject.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						existingObject.AccountSummaryID = null;
		            break;
		        } 
		        case "AccountModule":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountModule.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountModuleTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Profile != null)
						existingObject.ProfileID = serializedObject.Profile.ID;
					else
						existingObject.ProfileID = null;
					if(serializedObject.Security != null)
						existingObject.SecurityID = serializedObject.Security.ID;
					else
						existingObject.SecurityID = null;
					if(serializedObject.Roles != null)
						existingObject.RolesID = serializedObject.Roles.ID;
					else
						existingObject.RolesID = null;
					if(serializedObject.LocationCollection != null)
						existingObject.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						existingObject.LocationCollectionID = null;
		            break;
		        } 
		        case "LocationContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LocationContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
		            break;
		        } 
		        case "AddressAndLocation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddressAndLocation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddressAndLocationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.Address != null)
						existingObject.AddressID = serializedObject.Address.ID;
					else
						existingObject.AddressID = null;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "StreetAddress":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StreetAddressTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Street = serializedObject.Street;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.Town = serializedObject.Town;
		            existingObject.Country = serializedObject.Country;
		            break;
		        } 
		        case "AccountProfile":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountProfile.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountProfileTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ProfileImage != null)
						existingObject.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						existingObject.ProfileImageID = null;
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
					if(serializedObject.Address != null)
						existingObject.AddressID = serializedObject.Address.ID;
					else
						existingObject.AddressID = null;
		            existingObject.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            existingObject.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            existingObject.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
		            break;
		        } 
		        case "AccountSecurity":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountSecurityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.LoginInfoCollection != null)
						existingObject.LoginInfoCollectionID = serializedObject.LoginInfoCollection.ID;
					else
						existingObject.LoginInfoCollectionID = null;
					if(serializedObject.EmailCollection != null)
						existingObject.EmailCollectionID = serializedObject.EmailCollection.ID;
					else
						existingObject.EmailCollectionID = null;
		            break;
		        } 
		        case "AccountRoles":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountRolesTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ModeratorInGroups != null)
						existingObject.ModeratorInGroupsID = serializedObject.ModeratorInGroups.ID;
					else
						existingObject.ModeratorInGroupsID = null;
					if(serializedObject.MemberInGroups != null)
						existingObject.MemberInGroupsID = serializedObject.MemberInGroups.ID;
					else
						existingObject.MemberInGroupsID = null;
		            existingObject.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
		            break;
		        } 
		        case "PersonalInfoVisibility":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PersonalInfoVisibilityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.NoOne_Network_All = serializedObject.NoOne_Network_All;
		            break;
		        } 
		        case "ReferenceToInformation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ReferenceToInformationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.URL = serializedObject.URL;
		            break;
		        } 
		        case "NodeSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NodeSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Nodes != null)
						existingObject.NodesID = serializedObject.Nodes.ID;
					else
						existingObject.NodesID = null;
					if(serializedObject.NodeSourceTextContent != null)
						existingObject.NodeSourceTextContentID = serializedObject.NodeSourceTextContent.ID;
					else
						existingObject.NodeSourceTextContentID = null;
					if(serializedObject.NodeSourceLinkToContent != null)
						existingObject.NodeSourceLinkToContentID = serializedObject.NodeSourceLinkToContent.ID;
					else
						existingObject.NodeSourceLinkToContentID = null;
					if(serializedObject.NodeSourceEmbeddedContent != null)
						existingObject.NodeSourceEmbeddedContentID = serializedObject.NodeSourceEmbeddedContent.ID;
					else
						existingObject.NodeSourceEmbeddedContentID = null;
					if(serializedObject.NodeSourceImages != null)
						existingObject.NodeSourceImagesID = serializedObject.NodeSourceImages.ID;
					else
						existingObject.NodeSourceImagesID = null;
					if(serializedObject.NodeSourceBinaryFiles != null)
						existingObject.NodeSourceBinaryFilesID = serializedObject.NodeSourceBinaryFiles.ID;
					else
						existingObject.NodeSourceBinaryFilesID = null;
					if(serializedObject.NodeSourceCategories != null)
						existingObject.NodeSourceCategoriesID = serializedObject.NodeSourceCategories.ID;
					else
						existingObject.NodeSourceCategoriesID = null;
		            break;
		        } 
		        case "RenderedNode":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RenderedNodeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OriginalContentID = serializedObject.OriginalContentID;
		            existingObject.TechnicalSource = serializedObject.TechnicalSource;
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.ImageExt = serializedObject.ImageExt;
		            existingObject.Title = serializedObject.Title;
		            existingObject.ActualContentUrl = serializedObject.ActualContentUrl;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.TimestampText = serializedObject.TimestampText;
		            existingObject.MainSortableText = serializedObject.MainSortableText;
		            existingObject.IsCategoryFilteringNode = serializedObject.IsCategoryFilteringNode;
					if(serializedObject.CategoryFilters != null)
						existingObject.CategoryFiltersID = serializedObject.CategoryFilters.ID;
					else
						existingObject.CategoryFiltersID = null;
					if(serializedObject.CategoryNames != null)
						existingObject.CategoryNamesID = serializedObject.CategoryNames.ID;
					else
						existingObject.CategoryNamesID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            existingObject.CategoryIDList = serializedObject.CategoryIDList;
					if(serializedObject.Authors != null)
						existingObject.AuthorsID = serializedObject.Authors.ID;
					else
						existingObject.AuthorsID = null;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
		            break;
		        } 
		        case "ShortTextObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ShortTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            break;
		        } 
		        case "LongTextObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LongTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            break;
		        } 
		        case "MapMarker":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapMarkerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IconUrl = serializedObject.IconUrl;
		            existingObject.MarkerSource = serializedObject.MarkerSource;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            existingObject.LocationText = serializedObject.LocationText;
		            existingObject.PopupTitle = serializedObject.PopupTitle;
		            existingObject.PopupContent = serializedObject.PopupContent;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "Moderator":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ModeratorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ModeratorName = serializedObject.ModeratorName;
		            existingObject.ProfileUrl = serializedObject.ProfileUrl;
		            break;
		        } 
		        case "Collaborator":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Collaborator.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CollaboratorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.CollaboratorName = serializedObject.CollaboratorName;
		            existingObject.Role = serializedObject.Role;
		            existingObject.ProfileUrl = serializedObject.ProfileUrl;
		            break;
		        } 
		        case "GroupSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						existingObject.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						existingObject.GroupSummaryIndexID = null;
					if(serializedObject.GroupCollection != null)
						existingObject.GroupCollectionID = serializedObject.GroupCollection.ID;
					else
						existingObject.GroupCollectionID = null;
		            break;
		        } 
		        case "GroupContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.GroupIndex != null)
						existingObject.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						existingObject.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						existingObject.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						existingObject.GroupProfileID = null;
					if(serializedObject.Collaborators != null)
						existingObject.CollaboratorsID = serializedObject.Collaborators.ID;
					else
						existingObject.CollaboratorsID = null;
					if(serializedObject.PendingCollaborators != null)
						existingObject.PendingCollaboratorsID = serializedObject.PendingCollaborators.ID;
					else
						existingObject.PendingCollaboratorsID = null;
					if(serializedObject.LocationCollection != null)
						existingObject.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						existingObject.LocationCollectionID = null;
		            break;
		        } 
		        case "GroupIndex":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupIndexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            break;
		        } 
		        case "AddAddressAndLocationInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddAddressAndLocationInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LocationName = serializedObject.LocationName;
		            break;
		        } 
		        case "AddImageInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddImageInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageTitle = serializedObject.ImageTitle;
		            break;
		        } 
		        case "AddImageGroupInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddImageGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageGroupTitle = serializedObject.ImageGroupTitle;
		            break;
		        } 
		        case "AddEmailAddressInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddEmailAddressInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            break;
		        } 
		        case "CreateGroupInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CreateGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            break;
		        } 
		        case "AddActivityInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddActivityInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ActivityName = serializedObject.ActivityName;
		            break;
		        } 
		        case "AddBlogPostInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddBlogPostInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            break;
		        } 
		        case "AddCategoryInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddCategoryInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            break;
		        } 
		        case "Group":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Group.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.ProfileImage != null)
						existingObject.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						existingObject.ProfileImageID = null;
					if(serializedObject.IconImage != null)
						existingObject.IconImageID = serializedObject.IconImage.ID;
					else
						existingObject.IconImageID = null;
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.OrganizationsAndGroupsLinkedToUs = serializedObject.OrganizationsAndGroupsLinkedToUs;
		            existingObject.WwwSiteToPublishTo = serializedObject.WwwSiteToPublishTo;
					if(serializedObject.CustomUICollection != null)
						existingObject.CustomUICollectionID = serializedObject.CustomUICollection.ID;
					else
						existingObject.CustomUICollectionID = null;
					if(serializedObject.Moderators != null)
						existingObject.ModeratorsID = serializedObject.Moderators.ID;
					else
						existingObject.ModeratorsID = null;
					if(serializedObject.CategoryCollection != null)
						existingObject.CategoryCollectionID = serializedObject.CategoryCollection.ID;
					else
						existingObject.CategoryCollectionID = null;
		            break;
		        } 
		        case "Introduction":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = IntroductionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Body = serializedObject.Body;
		            break;
		        } 
		        case "ContentCategoryRank":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ContentCategoryRank.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContentCategoryRankTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ContentID = serializedObject.ContentID;
		            existingObject.ContentSemanticType = serializedObject.ContentSemanticType;
		            existingObject.CategoryID = serializedObject.CategoryID;
		            existingObject.RankName = serializedObject.RankName;
		            existingObject.RankValue = serializedObject.RankValue;
		            break;
		        } 
		        case "LinkToContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LinkToContent.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LinkToContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.URL = serializedObject.URL;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "EmbeddedContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = EmbeddedContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IFrameTagContents = serializedObject.IFrameTagContents;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "DynamicContentGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.DynamicContentGroup.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DynamicContentGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HostName = serializedObject.HostName;
		            existingObject.GroupHeader = serializedObject.GroupHeader;
		            existingObject.SortValue = serializedObject.SortValue;
		            existingObject.PageLocation = serializedObject.PageLocation;
		            existingObject.ContentItemNames = serializedObject.ContentItemNames;
		            break;
		        } 
		        case "DynamicContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.DynamicContent.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DynamicContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HostName = serializedObject.HostName;
		            existingObject.ContentName = serializedObject.ContentName;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.ElementQuery = serializedObject.ElementQuery;
		            existingObject.Content = serializedObject.Content;
		            existingObject.RawContent = serializedObject.RawContent;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.IsEnabled = serializedObject.IsEnabled;
		            existingObject.ApplyActively = serializedObject.ApplyActively;
		            existingObject.EditType = serializedObject.EditType;
		            existingObject.PageLocation = serializedObject.PageLocation;
		            break;
		        } 
		        case "AttachedToObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AttachedToObject.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AttachedToObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceObjectID = serializedObject.SourceObjectID;
		            existingObject.SourceObjectName = serializedObject.SourceObjectName;
		            existingObject.SourceObjectDomain = serializedObject.SourceObjectDomain;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            break;
		        } 
		        case "Comment":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Comment.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CommentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            existingObject.CommentText = serializedObject.CommentText;
		            existingObject.Created = serializedObject.Created;
		            existingObject.OriginalAuthorName = serializedObject.OriginalAuthorName;
		            existingObject.OriginalAuthorEmail = serializedObject.OriginalAuthorEmail;
		            existingObject.OriginalAuthorAccountID = serializedObject.OriginalAuthorAccountID;
		            existingObject.LastModified = serializedObject.LastModified;
		            existingObject.LastAuthorName = serializedObject.LastAuthorName;
		            existingObject.LastAuthorEmail = serializedObject.LastAuthorEmail;
		            existingObject.LastAuthorAccountID = serializedObject.LastAuthorAccountID;
		            break;
		        } 
		        case "Selection":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Selection.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SelectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            existingObject.SelectionCategory = serializedObject.SelectionCategory;
		            existingObject.TextValue = serializedObject.TextValue;
		            existingObject.BooleanValue = serializedObject.BooleanValue;
		            existingObject.DoubleValue = serializedObject.DoubleValue;
		            break;
		        } 
		        case "TextContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TextContent.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TextContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.SubTitle = serializedObject.SubTitle;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Body = serializedObject.Body;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            existingObject.SortOrderNumber = serializedObject.SortOrderNumber;
		            existingObject.IFrameSources = serializedObject.IFrameSources;
		            existingObject.RawHtmlContent = serializedObject.RawHtmlContent;
		            break;
		        } 
		        case "Map":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            break;
		        } 
		        case "MapResult":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapResult.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapResultTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "MapResultsCollection":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapResultsCollectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ResultByDate != null)
						existingObject.ResultByDateID = serializedObject.ResultByDate.ID;
					else
						existingObject.ResultByDateID = null;
					if(serializedObject.ResultByAuthor != null)
						existingObject.ResultByAuthorID = serializedObject.ResultByAuthor.ID;
					else
						existingObject.ResultByAuthorID = null;
					if(serializedObject.ResultByProximity != null)
						existingObject.ResultByProximityID = serializedObject.ResultByProximity.ID;
					else
						existingObject.ResultByProximityID = null;
		            break;
		        } 
		        case "Video":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = VideoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.VideoData != null)
						existingObject.VideoDataID = serializedObject.VideoData.ID;
					else
						existingObject.VideoDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Caption = serializedObject.Caption;
		            break;
		        } 
		        case "Image":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Image.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ImageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Caption = serializedObject.Caption;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "BinaryFile":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.BinaryFile.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = BinaryFileTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OriginalFileName = serializedObject.OriginalFileName;
					if(serializedObject.Data != null)
						existingObject.DataID = serializedObject.Data.ID;
					else
						existingObject.DataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "Longitude":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LongitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            break;
		        } 
		        case "Latitude":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LatitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            break;
		        } 
		        case "Location":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Location.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LocationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LocationName = serializedObject.LocationName;
					if(serializedObject.Longitude != null)
						existingObject.LongitudeID = serializedObject.Longitude.ID;
					else
						existingObject.LongitudeID = null;
					if(serializedObject.Latitude != null)
						existingObject.LatitudeID = serializedObject.Latitude.ID;
					else
						existingObject.LatitudeID = null;
		            break;
		        } 
		        case "Date":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DateTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Day = serializedObject.Day;
		            existingObject.Week = serializedObject.Week;
		            existingObject.Month = serializedObject.Month;
		            existingObject.Year = serializedObject.Year;
		            break;
		        } 
		        case "CategoryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "Category":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
		            existingObject.CategoryName = serializedObject.CategoryName;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
					if(serializedObject.ParentCategory != null)
						existingObject.ParentCategoryID = serializedObject.ParentCategory.ID;
					else
						existingObject.ParentCategoryID = null;
		            break;
		        } 
		        case "UpdateWebContentOperation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.UpdateWebContentOperation.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UpdateWebContentOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceContainerName = serializedObject.SourceContainerName;
		            existingObject.SourcePathRoot = serializedObject.SourcePathRoot;
		            existingObject.TargetContainerName = serializedObject.TargetContainerName;
		            existingObject.TargetPathRoot = serializedObject.TargetPathRoot;
		            existingObject.RenderWhileSync = serializedObject.RenderWhileSync;
					if(serializedObject.Handlers != null)
						existingObject.HandlersID = serializedObject.Handlers.ID;
					else
						existingObject.HandlersID = null;
		            break;
		        } 
		        case "UpdateWebContentHandlerItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
		                     ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UpdateWebContentHandlerItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InformationTypeName = serializedObject.InformationTypeName;
		            existingObject.OptionName = serializedObject.OptionName;
		            break;
		        } 
				}
		    }


			public async Task PerformUpdateAsync(string storageRootPath, InformationObjectMetaData updateData)
		    {
                if(updateData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");

				switch(updateData.ObjectType)
				{
		        case "TBSystem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBSystemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InstanceName = serializedObject.InstanceName;
		            existingObject.AdminGroupID = serializedObject.AdminGroupID;
		            break;
		        } 
		        case "WebPublishInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.WebPublishInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = WebPublishInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PublishType = serializedObject.PublishType;
		            existingObject.PublishContainer = serializedObject.PublishContainer;
					if(serializedObject.ActivePublication != null)
						existingObject.ActivePublicationID = serializedObject.ActivePublication.ID;
					else
						existingObject.ActivePublicationID = null;
					if(serializedObject.Publications != null)
						existingObject.PublicationsID = serializedObject.Publications.ID;
					else
						existingObject.PublicationsID = null;
		            break;
		        } 
		        case "PublicationPackage":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = PublicationPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PackageName = serializedObject.PackageName;
		            existingObject.PublicationTime = serializedObject.PublicationTime;
		            break;
		        } 
		        case "TBRLoginRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginRoot.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBRLoginRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.DomainName = serializedObject.DomainName;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBRAccountRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRAccountRoot.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBRAccountRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBRGroupRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRGroupRoot.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBRGroupRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Group != null)
						existingObject.GroupID = serializedObject.Group.ID;
					else
						existingObject.GroupID = null;
		            break;
		        } 
		        case "TBRLoginGroupRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBRLoginGroupRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Role = serializedObject.Role;
		            existingObject.GroupID = serializedObject.GroupID;
		            break;
		        } 
		        case "TBREmailRoot":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBREmailRoot.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBREmailRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Account != null)
						existingObject.AccountID = serializedObject.Account.ID;
					else
						existingObject.AccountID = null;
		            break;
		        } 
		        case "TBAccount":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Emails != null)
						existingObject.EmailsID = serializedObject.Emails.ID;
					else
						existingObject.EmailsID = null;
					if(serializedObject.Logins != null)
						existingObject.LoginsID = serializedObject.Logins.ID;
					else
						existingObject.LoginsID = null;
					if(serializedObject.GroupRoleCollection != null)
						existingObject.GroupRoleCollectionID = serializedObject.GroupRoleCollection.ID;
					else
						existingObject.GroupRoleCollectionID = null;
		            break;
		        } 
		        case "TBAccountCollaborationGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBAccountCollaborationGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.GroupRole = serializedObject.GroupRole;
		            existingObject.RoleStatus = serializedObject.RoleStatus;
		            break;
		        } 
		        case "TBLoginInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBLoginInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OpenIDUrl = serializedObject.OpenIDUrl;
		            break;
		        } 
		        case "TBEmail":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.ValidatedAt = serializedObject.ValidatedAt;
		            break;
		        } 
		        case "TBCollaboratorRole":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBCollaboratorRole.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBCollaboratorRoleTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Email != null)
						existingObject.EmailID = serializedObject.Email.ID;
					else
						existingObject.EmailID = null;
		            existingObject.Role = serializedObject.Role;
		            existingObject.RoleStatus = serializedObject.RoleStatus;
		            break;
		        } 
		        case "TBCollaboratingGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBCollaboratingGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
					if(serializedObject.Roles != null)
						existingObject.RolesID = serializedObject.Roles.ID;
					else
						existingObject.RolesID = null;
		            break;
		        } 
		        case "TBEmailValidation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBEmailValidation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBEmailValidationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Email = serializedObject.Email;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.ValidUntil = serializedObject.ValidUntil;
					if(serializedObject.GroupJoinConfirmation != null)
						existingObject.GroupJoinConfirmationID = serializedObject.GroupJoinConfirmation.ID;
					else
						existingObject.GroupJoinConfirmationID = null;
					if(serializedObject.DeviceJoinConfirmation != null)
						existingObject.DeviceJoinConfirmationID = serializedObject.DeviceJoinConfirmation.ID;
					else
						existingObject.DeviceJoinConfirmationID = null;
					if(serializedObject.InformationInputConfirmation != null)
						existingObject.InformationInputConfirmationID = serializedObject.InformationInputConfirmation.ID;
					else
						existingObject.InformationInputConfirmationID = null;
					if(serializedObject.InformationOutputConfirmation != null)
						existingObject.InformationOutputConfirmationID = serializedObject.InformationOutputConfirmation.ID;
					else
						existingObject.InformationOutputConfirmationID = null;
					if(serializedObject.MergeAccountsConfirmation != null)
						existingObject.MergeAccountsConfirmationID = serializedObject.MergeAccountsConfirmation.ID;
					else
						existingObject.MergeAccountsConfirmationID = null;
		            existingObject.RedirectUrlAfterValidation = serializedObject.RedirectUrlAfterValidation;
		            break;
		        } 
		        case "TBMergeAccountConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBMergeAccountConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            existingObject.AccountToMergeToID = serializedObject.AccountToMergeToID;
		            break;
		        } 
		        case "TBGroupJoinConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBGroupJoinConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.InvitationMode = serializedObject.InvitationMode;
		            break;
		        } 
		        case "TBDeviceJoinConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBDeviceJoinConfirmation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBDeviceJoinConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.DeviceMembershipID = serializedObject.DeviceMembershipID;
		            break;
		        } 
		        case "TBInformationInputConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBInformationInputConfirmation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBInformationInputConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.InformationInputID = serializedObject.InformationInputID;
		            break;
		        } 
		        case "TBInformationOutputConfirmation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBInformationOutputConfirmation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBInformationOutputConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.InformationOutputID = serializedObject.InformationOutputID;
		            break;
		        } 
		        case "LoginProvider":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LoginProvider.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LoginProviderTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ProviderName = serializedObject.ProviderName;
		            existingObject.ProviderIconClass = serializedObject.ProviderIconClass;
		            existingObject.ProviderType = serializedObject.ProviderType;
		            existingObject.ProviderUrl = serializedObject.ProviderUrl;
		            existingObject.ReturnUrl = serializedObject.ReturnUrl;
		            break;
		        } 
		        case "TBPRegisterEmail":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TBPRegisterEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            break;
		        } 
		        case "AccountSummary":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.GroupSummary != null)
						existingObject.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						existingObject.GroupSummaryID = null;
		            break;
		        } 
		        case "AccountContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.AccountModule != null)
						existingObject.AccountModuleID = serializedObject.AccountModule.ID;
					else
						existingObject.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						existingObject.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						existingObject.AccountSummaryID = null;
		            break;
		        } 
		        case "AccountModule":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountModule.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountModuleTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Profile != null)
						existingObject.ProfileID = serializedObject.Profile.ID;
					else
						existingObject.ProfileID = null;
					if(serializedObject.Security != null)
						existingObject.SecurityID = serializedObject.Security.ID;
					else
						existingObject.SecurityID = null;
					if(serializedObject.Roles != null)
						existingObject.RolesID = serializedObject.Roles.ID;
					else
						existingObject.RolesID = null;
					if(serializedObject.LocationCollection != null)
						existingObject.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						existingObject.LocationCollectionID = null;
		            break;
		        } 
		        case "LocationContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LocationContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
		            break;
		        } 
		        case "AddressAndLocation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddressAndLocation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddressAndLocationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.Address != null)
						existingObject.AddressID = serializedObject.Address.ID;
					else
						existingObject.AddressID = null;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "StreetAddress":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = StreetAddressTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Street = serializedObject.Street;
		            existingObject.ZipCode = serializedObject.ZipCode;
		            existingObject.Town = serializedObject.Town;
		            existingObject.Country = serializedObject.Country;
		            break;
		        } 
		        case "AccountProfile":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountProfile.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountProfileTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ProfileImage != null)
						existingObject.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						existingObject.ProfileImageID = null;
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
					if(serializedObject.Address != null)
						existingObject.AddressID = serializedObject.Address.ID;
					else
						existingObject.AddressID = null;
		            existingObject.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            existingObject.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            existingObject.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
		            break;
		        } 
		        case "AccountSecurity":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountSecurityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.LoginInfoCollection != null)
						existingObject.LoginInfoCollectionID = serializedObject.LoginInfoCollection.ID;
					else
						existingObject.LoginInfoCollectionID = null;
					if(serializedObject.EmailCollection != null)
						existingObject.EmailCollectionID = serializedObject.EmailCollection.ID;
					else
						existingObject.EmailCollectionID = null;
		            break;
		        } 
		        case "AccountRoles":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AccountRolesTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ModeratorInGroups != null)
						existingObject.ModeratorInGroupsID = serializedObject.ModeratorInGroups.ID;
					else
						existingObject.ModeratorInGroupsID = null;
					if(serializedObject.MemberInGroups != null)
						existingObject.MemberInGroupsID = serializedObject.MemberInGroups.ID;
					else
						existingObject.MemberInGroupsID = null;
		            existingObject.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
		            break;
		        } 
		        case "PersonalInfoVisibility":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = PersonalInfoVisibilityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.NoOne_Network_All = serializedObject.NoOne_Network_All;
		            break;
		        } 
		        case "ReferenceToInformation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ReferenceToInformationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.URL = serializedObject.URL;
		            break;
		        } 
		        case "NodeSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = NodeSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Nodes != null)
						existingObject.NodesID = serializedObject.Nodes.ID;
					else
						existingObject.NodesID = null;
					if(serializedObject.NodeSourceTextContent != null)
						existingObject.NodeSourceTextContentID = serializedObject.NodeSourceTextContent.ID;
					else
						existingObject.NodeSourceTextContentID = null;
					if(serializedObject.NodeSourceLinkToContent != null)
						existingObject.NodeSourceLinkToContentID = serializedObject.NodeSourceLinkToContent.ID;
					else
						existingObject.NodeSourceLinkToContentID = null;
					if(serializedObject.NodeSourceEmbeddedContent != null)
						existingObject.NodeSourceEmbeddedContentID = serializedObject.NodeSourceEmbeddedContent.ID;
					else
						existingObject.NodeSourceEmbeddedContentID = null;
					if(serializedObject.NodeSourceImages != null)
						existingObject.NodeSourceImagesID = serializedObject.NodeSourceImages.ID;
					else
						existingObject.NodeSourceImagesID = null;
					if(serializedObject.NodeSourceBinaryFiles != null)
						existingObject.NodeSourceBinaryFilesID = serializedObject.NodeSourceBinaryFiles.ID;
					else
						existingObject.NodeSourceBinaryFilesID = null;
					if(serializedObject.NodeSourceCategories != null)
						existingObject.NodeSourceCategoriesID = serializedObject.NodeSourceCategories.ID;
					else
						existingObject.NodeSourceCategoriesID = null;
		            break;
		        } 
		        case "RenderedNode":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = RenderedNodeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OriginalContentID = serializedObject.OriginalContentID;
		            existingObject.TechnicalSource = serializedObject.TechnicalSource;
		            existingObject.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            existingObject.ImageExt = serializedObject.ImageExt;
		            existingObject.Title = serializedObject.Title;
		            existingObject.ActualContentUrl = serializedObject.ActualContentUrl;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.TimestampText = serializedObject.TimestampText;
		            existingObject.MainSortableText = serializedObject.MainSortableText;
		            existingObject.IsCategoryFilteringNode = serializedObject.IsCategoryFilteringNode;
					if(serializedObject.CategoryFilters != null)
						existingObject.CategoryFiltersID = serializedObject.CategoryFilters.ID;
					else
						existingObject.CategoryFiltersID = null;
					if(serializedObject.CategoryNames != null)
						existingObject.CategoryNamesID = serializedObject.CategoryNames.ID;
					else
						existingObject.CategoryNamesID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            existingObject.CategoryIDList = serializedObject.CategoryIDList;
					if(serializedObject.Authors != null)
						existingObject.AuthorsID = serializedObject.Authors.ID;
					else
						existingObject.AuthorsID = null;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
		            break;
		        } 
		        case "ShortTextObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ShortTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            break;
		        } 
		        case "LongTextObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LongTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            break;
		        } 
		        case "MapMarker":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MapMarkerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IconUrl = serializedObject.IconUrl;
		            existingObject.MarkerSource = serializedObject.MarkerSource;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            existingObject.LocationText = serializedObject.LocationText;
		            existingObject.PopupTitle = serializedObject.PopupTitle;
		            existingObject.PopupContent = serializedObject.PopupContent;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "Moderator":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ModeratorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ModeratorName = serializedObject.ModeratorName;
		            existingObject.ProfileUrl = serializedObject.ProfileUrl;
		            break;
		        } 
		        case "Collaborator":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Collaborator.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CollaboratorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.CollaboratorName = serializedObject.CollaboratorName;
		            existingObject.Role = serializedObject.Role;
		            existingObject.ProfileUrl = serializedObject.ProfileUrl;
		            break;
		        } 
		        case "GroupSummaryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						existingObject.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						existingObject.GroupSummaryIndexID = null;
					if(serializedObject.GroupCollection != null)
						existingObject.GroupCollectionID = serializedObject.GroupCollection.ID;
					else
						existingObject.GroupCollectionID = null;
		            break;
		        } 
		        case "GroupContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.GroupIndex != null)
						existingObject.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						existingObject.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						existingObject.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						existingObject.GroupProfileID = null;
					if(serializedObject.Collaborators != null)
						existingObject.CollaboratorsID = serializedObject.Collaborators.ID;
					else
						existingObject.CollaboratorsID = null;
					if(serializedObject.PendingCollaborators != null)
						existingObject.PendingCollaboratorsID = serializedObject.PendingCollaborators.ID;
					else
						existingObject.PendingCollaboratorsID = null;
					if(serializedObject.LocationCollection != null)
						existingObject.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						existingObject.LocationCollectionID = null;
		            break;
		        } 
		        case "GroupIndex":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupIndexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            break;
		        } 
		        case "AddAddressAndLocationInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddAddressAndLocationInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LocationName = serializedObject.LocationName;
		            break;
		        } 
		        case "AddImageInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddImageInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageTitle = serializedObject.ImageTitle;
		            break;
		        } 
		        case "AddImageGroupInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddImageGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageGroupTitle = serializedObject.ImageGroupTitle;
		            break;
		        } 
		        case "AddEmailAddressInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddEmailAddressInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            break;
		        } 
		        case "CreateGroupInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CreateGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            break;
		        } 
		        case "AddActivityInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddActivityInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ActivityName = serializedObject.ActivityName;
		            break;
		        } 
		        case "AddBlogPostInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddBlogPostInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            break;
		        } 
		        case "AddCategoryInfo":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AddCategoryInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            break;
		        } 
		        case "Group":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Group.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = GroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.ProfileImage != null)
						existingObject.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						existingObject.ProfileImageID = null;
					if(serializedObject.IconImage != null)
						existingObject.IconImageID = serializedObject.IconImage.ID;
					else
						existingObject.IconImageID = null;
		            existingObject.GroupName = serializedObject.GroupName;
		            existingObject.Description = serializedObject.Description;
		            existingObject.OrganizationsAndGroupsLinkedToUs = serializedObject.OrganizationsAndGroupsLinkedToUs;
		            existingObject.WwwSiteToPublishTo = serializedObject.WwwSiteToPublishTo;
					if(serializedObject.CustomUICollection != null)
						existingObject.CustomUICollectionID = serializedObject.CustomUICollection.ID;
					else
						existingObject.CustomUICollectionID = null;
					if(serializedObject.Moderators != null)
						existingObject.ModeratorsID = serializedObject.Moderators.ID;
					else
						existingObject.ModeratorsID = null;
					if(serializedObject.CategoryCollection != null)
						existingObject.CategoryCollectionID = serializedObject.CategoryCollection.ID;
					else
						existingObject.CategoryCollectionID = null;
		            break;
		        } 
		        case "Introduction":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = IntroductionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Body = serializedObject.Body;
		            break;
		        } 
		        case "ContentCategoryRank":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.ContentCategoryRank.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ContentCategoryRankTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ContentID = serializedObject.ContentID;
		            existingObject.ContentSemanticType = serializedObject.ContentSemanticType;
		            existingObject.CategoryID = serializedObject.CategoryID;
		            existingObject.RankName = serializedObject.RankName;
		            existingObject.RankValue = serializedObject.RankValue;
		            break;
		        } 
		        case "LinkToContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.LinkToContent.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LinkToContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.URL = serializedObject.URL;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "EmbeddedContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = EmbeddedContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.IFrameTagContents = serializedObject.IFrameTagContents;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "DynamicContentGroup":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.DynamicContentGroup.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = DynamicContentGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HostName = serializedObject.HostName;
		            existingObject.GroupHeader = serializedObject.GroupHeader;
		            existingObject.SortValue = serializedObject.SortValue;
		            existingObject.PageLocation = serializedObject.PageLocation;
		            existingObject.ContentItemNames = serializedObject.ContentItemNames;
		            break;
		        } 
		        case "DynamicContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.DynamicContent.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = DynamicContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HostName = serializedObject.HostName;
		            existingObject.ContentName = serializedObject.ContentName;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.ElementQuery = serializedObject.ElementQuery;
		            existingObject.Content = serializedObject.Content;
		            existingObject.RawContent = serializedObject.RawContent;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.IsEnabled = serializedObject.IsEnabled;
		            existingObject.ApplyActively = serializedObject.ApplyActively;
		            existingObject.EditType = serializedObject.EditType;
		            existingObject.PageLocation = serializedObject.PageLocation;
		            break;
		        } 
		        case "AttachedToObject":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.AttachedToObject.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = AttachedToObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceObjectID = serializedObject.SourceObjectID;
		            existingObject.SourceObjectName = serializedObject.SourceObjectName;
		            existingObject.SourceObjectDomain = serializedObject.SourceObjectDomain;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            break;
		        } 
		        case "Comment":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Comment.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CommentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            existingObject.CommentText = serializedObject.CommentText;
		            existingObject.Created = serializedObject.Created;
		            existingObject.OriginalAuthorName = serializedObject.OriginalAuthorName;
		            existingObject.OriginalAuthorEmail = serializedObject.OriginalAuthorEmail;
		            existingObject.OriginalAuthorAccountID = serializedObject.OriginalAuthorAccountID;
		            existingObject.LastModified = serializedObject.LastModified;
		            existingObject.LastAuthorName = serializedObject.LastAuthorName;
		            existingObject.LastAuthorEmail = serializedObject.LastAuthorEmail;
		            existingObject.LastAuthorAccountID = serializedObject.LastAuthorAccountID;
		            break;
		        } 
		        case "Selection":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Selection.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = SelectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TargetObjectID = serializedObject.TargetObjectID;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            existingObject.SelectionCategory = serializedObject.SelectionCategory;
		            existingObject.TextValue = serializedObject.TextValue;
		            existingObject.BooleanValue = serializedObject.BooleanValue;
		            existingObject.DoubleValue = serializedObject.DoubleValue;
		            break;
		        } 
		        case "TextContent":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.TextContent.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = TextContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.SubTitle = serializedObject.SubTitle;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Body = serializedObject.Body;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            existingObject.SortOrderNumber = serializedObject.SortOrderNumber;
		            existingObject.IFrameSources = serializedObject.IFrameSources;
		            existingObject.RawHtmlContent = serializedObject.RawHtmlContent;
		            break;
		        } 
		        case "Map":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MapTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            break;
		        } 
		        case "MapResult":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapResult.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MapResultTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Location != null)
						existingObject.LocationID = serializedObject.Location.ID;
					else
						existingObject.LocationID = null;
		            break;
		        } 
		        case "MapResultsCollection":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = MapResultsCollectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ResultByDate != null)
						existingObject.ResultByDateID = serializedObject.ResultByDate.ID;
					else
						existingObject.ResultByDateID = null;
					if(serializedObject.ResultByAuthor != null)
						existingObject.ResultByAuthorID = serializedObject.ResultByAuthor.ID;
					else
						existingObject.ResultByAuthorID = null;
					if(serializedObject.ResultByProximity != null)
						existingObject.ResultByProximityID = serializedObject.ResultByProximity.ID;
					else
						existingObject.ResultByProximityID = null;
		            break;
		        } 
		        case "Video":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = VideoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.VideoData != null)
						existingObject.VideoDataID = serializedObject.VideoData.ID;
					else
						existingObject.VideoDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Caption = serializedObject.Caption;
		            break;
		        } 
		        case "Image":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Image.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = ImageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Caption = serializedObject.Caption;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						existingObject.LocationsID = serializedObject.Locations.ID;
					else
						existingObject.LocationsID = null;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "BinaryFile":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.BinaryFile.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = BinaryFileTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OriginalFileName = serializedObject.OriginalFileName;
					if(serializedObject.Data != null)
						existingObject.DataID = serializedObject.Data.ID;
					else
						existingObject.DataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "Longitude":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LongitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            break;
		        } 
		        case "Latitude":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LatitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            break;
		        } 
		        case "Location":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Location.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = LocationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LocationName = serializedObject.LocationName;
					if(serializedObject.Longitude != null)
						existingObject.LongitudeID = serializedObject.Longitude.ID;
					else
						existingObject.LongitudeID = null;
					if(serializedObject.Latitude != null)
						existingObject.LatitudeID = serializedObject.Latitude.ID;
					else
						existingObject.LatitudeID = null;
		            break;
		        } 
		        case "Date":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = DateTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Day = serializedObject.Day;
		            existingObject.Week = serializedObject.Week;
		            existingObject.Month = serializedObject.Month;
		            existingObject.Year = serializedObject.Year;
		            break;
		        } 
		        case "CategoryContainer":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CategoryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Categories != null)
						existingObject.CategoriesID = serializedObject.Categories.ID;
					else
						existingObject.CategoriesID = null;
		            break;
		        } 
		        case "Category":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
		            existingObject.CategoryName = serializedObject.CategoryName;
					if(serializedObject.ImageData != null)
						existingObject.ImageDataID = serializedObject.ImageData.ID;
					else
						existingObject.ImageDataID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
					if(serializedObject.ParentCategory != null)
						existingObject.ParentCategoryID = serializedObject.ParentCategory.ID;
					else
						existingObject.ParentCategoryID = null;
		            break;
		        } 
		        case "UpdateWebContentOperation":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.UpdateWebContentOperation.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = UpdateWebContentOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceContainerName = serializedObject.SourceContainerName;
		            existingObject.SourcePathRoot = serializedObject.SourcePathRoot;
		            existingObject.TargetContainerName = serializedObject.TargetContainerName;
		            existingObject.TargetPathRoot = serializedObject.TargetPathRoot;
		            existingObject.RenderWhileSync = serializedObject.RenderWhileSync;
					if(serializedObject.Handlers != null)
						existingObject.HandlersID = serializedObject.Handlers.ID;
					else
						existingObject.HandlersID = null;
		            break;
		        } 
		        case "UpdateWebContentHandlerItem":
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject = 
		                global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
		                    await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
		            var existingObject = UpdateWebContentHandlerItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InformationTypeName = serializedObject.InformationTypeName;
		            existingObject.OptionName = serializedObject.OptionName;
		            break;
		        } 
				}
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

				switch(insertData.ObjectType)
				{
                case "TBSystem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBSystem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InstanceName = serializedObject.InstanceName;
		            objectToAdd.AdminGroupID = serializedObject.AdminGroupID;
					TBSystemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "WebPublishInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.WebPublishInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new WebPublishInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PublishType = serializedObject.PublishType;
		            objectToAdd.PublishContainer = serializedObject.PublishContainer;
					if(serializedObject.ActivePublication != null)
						objectToAdd.ActivePublicationID = serializedObject.ActivePublication.ID;
					else
						objectToAdd.ActivePublicationID = null;
					if(serializedObject.Publications != null)
						objectToAdd.PublicationsID = serializedObject.Publications.ID;
					else
						objectToAdd.PublicationsID = null;
					WebPublishInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "PublicationPackage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PublicationPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PackageName = serializedObject.PackageName;
		            objectToAdd.PublicationTime = serializedObject.PublicationTime;
					PublicationPackageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRLoginRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginRoot.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRLoginRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.DomainName = serializedObject.DomainName;
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBRLoginRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRAccountRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRAccountRoot.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRAccountRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBRAccountRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRGroupRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRGroupRoot.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRGroupRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Group != null)
						objectToAdd.GroupID = serializedObject.Group.ID;
					else
						objectToAdd.GroupID = null;
					TBRGroupRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRLoginGroupRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRLoginGroupRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.GroupID = serializedObject.GroupID;
					TBRLoginGroupRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBREmailRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBREmailRoot.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBREmailRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBREmailRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBAccount":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Emails != null)
						objectToAdd.EmailsID = serializedObject.Emails.ID;
					else
						objectToAdd.EmailsID = null;
					if(serializedObject.Logins != null)
						objectToAdd.LoginsID = serializedObject.Logins.ID;
					else
						objectToAdd.LoginsID = null;
					if(serializedObject.GroupRoleCollection != null)
						objectToAdd.GroupRoleCollectionID = serializedObject.GroupRoleCollection.ID;
					else
						objectToAdd.GroupRoleCollectionID = null;
					TBAccountTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBAccountCollaborationGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBAccountCollaborationGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.GroupRole = serializedObject.GroupRole;
		            objectToAdd.RoleStatus = serializedObject.RoleStatus;
					TBAccountCollaborationGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBLoginInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBLoginInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OpenIDUrl = serializedObject.OpenIDUrl;
					TBLoginInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBEmail":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.ValidatedAt = serializedObject.ValidatedAt;
					TBEmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBCollaboratorRole":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBCollaboratorRole.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBCollaboratorRole {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Email != null)
						objectToAdd.EmailID = serializedObject.Email.ID;
					else
						objectToAdd.EmailID = null;
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.RoleStatus = serializedObject.RoleStatus;
					TBCollaboratorRoleTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBCollaboratingGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBCollaboratingGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					if(serializedObject.Roles != null)
						objectToAdd.RolesID = serializedObject.Roles.ID;
					else
						objectToAdd.RolesID = null;
					TBCollaboratingGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBEmailValidation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBEmailValidation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBEmailValidation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.ValidUntil = serializedObject.ValidUntil;
					if(serializedObject.GroupJoinConfirmation != null)
						objectToAdd.GroupJoinConfirmationID = serializedObject.GroupJoinConfirmation.ID;
					else
						objectToAdd.GroupJoinConfirmationID = null;
					if(serializedObject.DeviceJoinConfirmation != null)
						objectToAdd.DeviceJoinConfirmationID = serializedObject.DeviceJoinConfirmation.ID;
					else
						objectToAdd.DeviceJoinConfirmationID = null;
					if(serializedObject.InformationInputConfirmation != null)
						objectToAdd.InformationInputConfirmationID = serializedObject.InformationInputConfirmation.ID;
					else
						objectToAdd.InformationInputConfirmationID = null;
					if(serializedObject.InformationOutputConfirmation != null)
						objectToAdd.InformationOutputConfirmationID = serializedObject.InformationOutputConfirmation.ID;
					else
						objectToAdd.InformationOutputConfirmationID = null;
					if(serializedObject.MergeAccountsConfirmation != null)
						objectToAdd.MergeAccountsConfirmationID = serializedObject.MergeAccountsConfirmation.ID;
					else
						objectToAdd.MergeAccountsConfirmationID = null;
		            objectToAdd.RedirectUrlAfterValidation = serializedObject.RedirectUrlAfterValidation;
					TBEmailValidationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBMergeAccountConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBMergeAccountConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            objectToAdd.AccountToMergeToID = serializedObject.AccountToMergeToID;
					TBMergeAccountConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBGroupJoinConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBGroupJoinConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.InvitationMode = serializedObject.InvitationMode;
					TBGroupJoinConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBDeviceJoinConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBDeviceJoinConfirmation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBDeviceJoinConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.DeviceMembershipID = serializedObject.DeviceMembershipID;
					TBDeviceJoinConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBInformationInputConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBInformationInputConfirmation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBInformationInputConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.InformationInputID = serializedObject.InformationInputID;
					TBInformationInputConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBInformationOutputConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBInformationOutputConfirmation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBInformationOutputConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.InformationOutputID = serializedObject.InformationOutputID;
					TBInformationOutputConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LoginProvider":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LoginProvider.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LoginProvider {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ProviderName = serializedObject.ProviderName;
		            objectToAdd.ProviderIconClass = serializedObject.ProviderIconClass;
		            objectToAdd.ProviderType = serializedObject.ProviderType;
		            objectToAdd.ProviderUrl = serializedObject.ProviderUrl;
		            objectToAdd.ReturnUrl = serializedObject.ReturnUrl;
					LoginProviderTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBPRegisterEmail":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBPRegisterEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					TBPRegisterEmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.GroupSummary != null)
						objectToAdd.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						objectToAdd.GroupSummaryID = null;
					AccountSummaryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.AccountModule != null)
						objectToAdd.AccountModuleID = serializedObject.AccountModule.ID;
					else
						objectToAdd.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						objectToAdd.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						objectToAdd.AccountSummaryID = null;
					AccountContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountModule":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountModule.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountModule {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Profile != null)
						objectToAdd.ProfileID = serializedObject.Profile.ID;
					else
						objectToAdd.ProfileID = null;
					if(serializedObject.Security != null)
						objectToAdd.SecurityID = serializedObject.Security.ID;
					else
						objectToAdd.SecurityID = null;
					if(serializedObject.Roles != null)
						objectToAdd.RolesID = serializedObject.Roles.ID;
					else
						objectToAdd.RolesID = null;
					if(serializedObject.LocationCollection != null)
						objectToAdd.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						objectToAdd.LocationCollectionID = null;
					AccountModuleTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LocationContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LocationContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					LocationContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddressAndLocation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddressAndLocation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddressAndLocation {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.Address != null)
						objectToAdd.AddressID = serializedObject.Address.ID;
					else
						objectToAdd.AddressID = null;
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					AddressAndLocationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "StreetAddress":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StreetAddress {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Street = serializedObject.Street;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.Town = serializedObject.Town;
		            objectToAdd.Country = serializedObject.Country;
					StreetAddressTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountProfile":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountProfile.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountProfile {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProfileImage != null)
						objectToAdd.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						objectToAdd.ProfileImageID = null;
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
					if(serializedObject.Address != null)
						objectToAdd.AddressID = serializedObject.Address.ID;
					else
						objectToAdd.AddressID = null;
		            objectToAdd.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            objectToAdd.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            objectToAdd.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
					AccountProfileTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountSecurity":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountSecurity {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.LoginInfoCollection != null)
						objectToAdd.LoginInfoCollectionID = serializedObject.LoginInfoCollection.ID;
					else
						objectToAdd.LoginInfoCollectionID = null;
					if(serializedObject.EmailCollection != null)
						objectToAdd.EmailCollectionID = serializedObject.EmailCollection.ID;
					else
						objectToAdd.EmailCollectionID = null;
					AccountSecurityTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountRoles":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountRoles {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ModeratorInGroups != null)
						objectToAdd.ModeratorInGroupsID = serializedObject.ModeratorInGroups.ID;
					else
						objectToAdd.ModeratorInGroupsID = null;
					if(serializedObject.MemberInGroups != null)
						objectToAdd.MemberInGroupsID = serializedObject.MemberInGroups.ID;
					else
						objectToAdd.MemberInGroupsID = null;
		            objectToAdd.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
					AccountRolesTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "PersonalInfoVisibility":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PersonalInfoVisibility {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.NoOne_Network_All = serializedObject.NoOne_Network_All;
					PersonalInfoVisibilityTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ReferenceToInformation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ReferenceToInformation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.URL = serializedObject.URL;
					ReferenceToInformationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "NodeSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NodeSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Nodes != null)
						objectToAdd.NodesID = serializedObject.Nodes.ID;
					else
						objectToAdd.NodesID = null;
					if(serializedObject.NodeSourceTextContent != null)
						objectToAdd.NodeSourceTextContentID = serializedObject.NodeSourceTextContent.ID;
					else
						objectToAdd.NodeSourceTextContentID = null;
					if(serializedObject.NodeSourceLinkToContent != null)
						objectToAdd.NodeSourceLinkToContentID = serializedObject.NodeSourceLinkToContent.ID;
					else
						objectToAdd.NodeSourceLinkToContentID = null;
					if(serializedObject.NodeSourceEmbeddedContent != null)
						objectToAdd.NodeSourceEmbeddedContentID = serializedObject.NodeSourceEmbeddedContent.ID;
					else
						objectToAdd.NodeSourceEmbeddedContentID = null;
					if(serializedObject.NodeSourceImages != null)
						objectToAdd.NodeSourceImagesID = serializedObject.NodeSourceImages.ID;
					else
						objectToAdd.NodeSourceImagesID = null;
					if(serializedObject.NodeSourceBinaryFiles != null)
						objectToAdd.NodeSourceBinaryFilesID = serializedObject.NodeSourceBinaryFiles.ID;
					else
						objectToAdd.NodeSourceBinaryFilesID = null;
					if(serializedObject.NodeSourceCategories != null)
						objectToAdd.NodeSourceCategoriesID = serializedObject.NodeSourceCategories.ID;
					else
						objectToAdd.NodeSourceCategoriesID = null;
					NodeSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "RenderedNode":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RenderedNode {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OriginalContentID = serializedObject.OriginalContentID;
		            objectToAdd.TechnicalSource = serializedObject.TechnicalSource;
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.ImageExt = serializedObject.ImageExt;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.ActualContentUrl = serializedObject.ActualContentUrl;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.TimestampText = serializedObject.TimestampText;
		            objectToAdd.MainSortableText = serializedObject.MainSortableText;
		            objectToAdd.IsCategoryFilteringNode = serializedObject.IsCategoryFilteringNode;
					if(serializedObject.CategoryFilters != null)
						objectToAdd.CategoryFiltersID = serializedObject.CategoryFilters.ID;
					else
						objectToAdd.CategoryFiltersID = null;
					if(serializedObject.CategoryNames != null)
						objectToAdd.CategoryNamesID = serializedObject.CategoryNames.ID;
					else
						objectToAdd.CategoryNamesID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
		            objectToAdd.CategoryIDList = serializedObject.CategoryIDList;
					if(serializedObject.Authors != null)
						objectToAdd.AuthorsID = serializedObject.Authors.ID;
					else
						objectToAdd.AuthorsID = null;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					RenderedNodeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ShortTextObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					ShortTextObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LongTextObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LongTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					LongTextObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapMarker":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapMarker {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IconUrl = serializedObject.IconUrl;
		            objectToAdd.MarkerSource = serializedObject.MarkerSource;
		            objectToAdd.CategoryName = serializedObject.CategoryName;
		            objectToAdd.LocationText = serializedObject.LocationText;
		            objectToAdd.PopupTitle = serializedObject.PopupTitle;
		            objectToAdd.PopupContent = serializedObject.PopupContent;
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					MapMarkerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Moderator":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Moderator {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ModeratorName = serializedObject.ModeratorName;
		            objectToAdd.ProfileUrl = serializedObject.ProfileUrl;
					ModeratorTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Collaborator":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Collaborator.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Collaborator {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.CollaboratorName = serializedObject.CollaboratorName;
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.ProfileUrl = serializedObject.ProfileUrl;
					CollaboratorTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						objectToAdd.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						objectToAdd.GroupSummaryIndexID = null;
					if(serializedObject.GroupCollection != null)
						objectToAdd.GroupCollectionID = serializedObject.GroupCollection.ID;
					else
						objectToAdd.GroupCollectionID = null;
					GroupSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.GroupIndex != null)
						objectToAdd.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						objectToAdd.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						objectToAdd.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						objectToAdd.GroupProfileID = null;
					if(serializedObject.Collaborators != null)
						objectToAdd.CollaboratorsID = serializedObject.Collaborators.ID;
					else
						objectToAdd.CollaboratorsID = null;
					if(serializedObject.PendingCollaborators != null)
						objectToAdd.PendingCollaboratorsID = serializedObject.PendingCollaborators.ID;
					else
						objectToAdd.PendingCollaboratorsID = null;
					if(serializedObject.LocationCollection != null)
						objectToAdd.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						objectToAdd.LocationCollectionID = null;
					GroupContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupIndex":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupIndex {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					GroupIndexTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddAddressAndLocationInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddAddressAndLocationInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LocationName = serializedObject.LocationName;
					AddAddressAndLocationInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddImageInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddImageInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageTitle = serializedObject.ImageTitle;
					AddImageInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddImageGroupInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddImageGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageGroupTitle = serializedObject.ImageGroupTitle;
					AddImageGroupInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddEmailAddressInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddEmailAddressInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					AddEmailAddressInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "CreateGroupInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CreateGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					CreateGroupInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddActivityInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddActivityInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ActivityName = serializedObject.ActivityName;
					AddActivityInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddBlogPostInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddBlogPostInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					AddBlogPostInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddCategoryInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddCategoryInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					AddCategoryInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Group":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Group.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Group {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.ProfileImage != null)
						objectToAdd.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						objectToAdd.ProfileImageID = null;
					if(serializedObject.IconImage != null)
						objectToAdd.IconImageID = serializedObject.IconImage.ID;
					else
						objectToAdd.IconImageID = null;
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.OrganizationsAndGroupsLinkedToUs = serializedObject.OrganizationsAndGroupsLinkedToUs;
		            objectToAdd.WwwSiteToPublishTo = serializedObject.WwwSiteToPublishTo;
					if(serializedObject.CustomUICollection != null)
						objectToAdd.CustomUICollectionID = serializedObject.CustomUICollection.ID;
					else
						objectToAdd.CustomUICollectionID = null;
					if(serializedObject.Moderators != null)
						objectToAdd.ModeratorsID = serializedObject.Moderators.ID;
					else
						objectToAdd.ModeratorsID = null;
					if(serializedObject.CategoryCollection != null)
						objectToAdd.CategoryCollectionID = serializedObject.CategoryCollection.ID;
					else
						objectToAdd.CategoryCollectionID = null;
					GroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Introduction":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Introduction {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Body = serializedObject.Body;
					IntroductionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ContentCategoryRank":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ContentCategoryRank.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContentCategoryRank {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ContentID = serializedObject.ContentID;
		            objectToAdd.ContentSemanticType = serializedObject.ContentSemanticType;
		            objectToAdd.CategoryID = serializedObject.CategoryID;
		            objectToAdd.RankName = serializedObject.RankName;
		            objectToAdd.RankValue = serializedObject.RankValue;
					ContentCategoryRankTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LinkToContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LinkToContent.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LinkToContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.URL = serializedObject.URL;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					LinkToContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "EmbeddedContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new EmbeddedContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IFrameTagContents = serializedObject.IFrameTagContents;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					EmbeddedContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "DynamicContentGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DynamicContentGroup.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DynamicContentGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HostName = serializedObject.HostName;
		            objectToAdd.GroupHeader = serializedObject.GroupHeader;
		            objectToAdd.SortValue = serializedObject.SortValue;
		            objectToAdd.PageLocation = serializedObject.PageLocation;
		            objectToAdd.ContentItemNames = serializedObject.ContentItemNames;
					DynamicContentGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "DynamicContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DynamicContent.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DynamicContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HostName = serializedObject.HostName;
		            objectToAdd.ContentName = serializedObject.ContentName;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.ElementQuery = serializedObject.ElementQuery;
		            objectToAdd.Content = serializedObject.Content;
		            objectToAdd.RawContent = serializedObject.RawContent;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.IsEnabled = serializedObject.IsEnabled;
		            objectToAdd.ApplyActively = serializedObject.ApplyActively;
		            objectToAdd.EditType = serializedObject.EditType;
		            objectToAdd.PageLocation = serializedObject.PageLocation;
					DynamicContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AttachedToObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AttachedToObject.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AttachedToObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceObjectID = serializedObject.SourceObjectID;
		            objectToAdd.SourceObjectName = serializedObject.SourceObjectName;
		            objectToAdd.SourceObjectDomain = serializedObject.SourceObjectDomain;
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
					AttachedToObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Comment":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Comment.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Comment {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            objectToAdd.CommentText = serializedObject.CommentText;
		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.OriginalAuthorName = serializedObject.OriginalAuthorName;
		            objectToAdd.OriginalAuthorEmail = serializedObject.OriginalAuthorEmail;
		            objectToAdd.OriginalAuthorAccountID = serializedObject.OriginalAuthorAccountID;
		            objectToAdd.LastModified = serializedObject.LastModified;
		            objectToAdd.LastAuthorName = serializedObject.LastAuthorName;
		            objectToAdd.LastAuthorEmail = serializedObject.LastAuthorEmail;
		            objectToAdd.LastAuthorAccountID = serializedObject.LastAuthorAccountID;
					CommentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Selection":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Selection.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Selection {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            objectToAdd.SelectionCategory = serializedObject.SelectionCategory;
		            objectToAdd.TextValue = serializedObject.TextValue;
		            objectToAdd.BooleanValue = serializedObject.BooleanValue;
		            objectToAdd.DoubleValue = serializedObject.DoubleValue;
					SelectionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TextContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TextContent.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TextContent {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.SubTitle = serializedObject.SubTitle;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Body = serializedObject.Body;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
		            objectToAdd.SortOrderNumber = serializedObject.SortOrderNumber;
		            objectToAdd.IFrameSources = serializedObject.IFrameSources;
		            objectToAdd.RawHtmlContent = serializedObject.RawHtmlContent;
					TextContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Map":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Map {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					MapTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapResult":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapResult.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapResult {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					MapResultTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapResultsCollection":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapResultsCollection {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ResultByDate != null)
						objectToAdd.ResultByDateID = serializedObject.ResultByDate.ID;
					else
						objectToAdd.ResultByDateID = null;
					if(serializedObject.ResultByAuthor != null)
						objectToAdd.ResultByAuthorID = serializedObject.ResultByAuthor.ID;
					else
						objectToAdd.ResultByAuthorID = null;
					if(serializedObject.ResultByProximity != null)
						objectToAdd.ResultByProximityID = serializedObject.ResultByProximity.ID;
					else
						objectToAdd.ResultByProximityID = null;
					MapResultsCollectionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Video":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Video {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.VideoData != null)
						objectToAdd.VideoDataID = serializedObject.VideoData.ID;
					else
						objectToAdd.VideoDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Caption = serializedObject.Caption;
					VideoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Image":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Image.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Image {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Caption = serializedObject.Caption;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					ImageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "BinaryFile":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.BinaryFile.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new BinaryFile {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OriginalFileName = serializedObject.OriginalFileName;
					if(serializedObject.Data != null)
						objectToAdd.DataID = serializedObject.Data.ID;
					else
						objectToAdd.DataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					BinaryFileTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Longitude":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Longitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LongitudeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Latitude":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Latitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LatitudeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Location":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Location.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Location {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LocationName = serializedObject.LocationName;
					if(serializedObject.Longitude != null)
						objectToAdd.LongitudeID = serializedObject.Longitude.ID;
					else
						objectToAdd.LongitudeID = null;
					if(serializedObject.Latitude != null)
						objectToAdd.LatitudeID = serializedObject.Latitude.ID;
					else
						objectToAdd.LatitudeID = null;
					LocationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Date":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Date {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Day = serializedObject.Day;
		            objectToAdd.Week = serializedObject.Week;
		            objectToAdd.Month = serializedObject.Month;
		            objectToAdd.Year = serializedObject.Year;
					DateTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "CategoryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CategoryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					CategoryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Category":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
					if(serializedObject.ParentCategory != null)
						objectToAdd.ParentCategoryID = serializedObject.ParentCategory.ID;
					else
						objectToAdd.ParentCategoryID = null;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UpdateWebContentOperation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.UpdateWebContentOperation.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UpdateWebContentOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceContainerName = serializedObject.SourceContainerName;
		            objectToAdd.SourcePathRoot = serializedObject.SourcePathRoot;
		            objectToAdd.TargetContainerName = serializedObject.TargetContainerName;
		            objectToAdd.TargetPathRoot = serializedObject.TargetPathRoot;
		            objectToAdd.RenderWhileSync = serializedObject.RenderWhileSync;
					if(serializedObject.Handlers != null)
						objectToAdd.HandlersID = serializedObject.Handlers.ID;
					else
						objectToAdd.HandlersID = null;
					UpdateWebContentOperationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UpdateWebContentHandlerItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
                             ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UpdateWebContentHandlerItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InformationTypeName = serializedObject.InformationTypeName;
		            objectToAdd.OptionName = serializedObject.OptionName;
					UpdateWebContentHandlerItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
				}
            }


		    public async Task PerformInsertAsync(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);

				switch(insertData.ObjectType)
				{
                case "TBSystem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBSystem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InstanceName = serializedObject.InstanceName;
		            objectToAdd.AdminGroupID = serializedObject.AdminGroupID;
					TBSystemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "WebPublishInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.WebPublishInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new WebPublishInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PublishType = serializedObject.PublishType;
		            objectToAdd.PublishContainer = serializedObject.PublishContainer;
					if(serializedObject.ActivePublication != null)
						objectToAdd.ActivePublicationID = serializedObject.ActivePublication.ID;
					else
						objectToAdd.ActivePublicationID = null;
					if(serializedObject.Publications != null)
						objectToAdd.PublicationsID = serializedObject.Publications.ID;
					else
						objectToAdd.PublicationsID = null;
					WebPublishInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "PublicationPackage":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new PublicationPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PackageName = serializedObject.PackageName;
		            objectToAdd.PublicationTime = serializedObject.PublicationTime;
					PublicationPackageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRLoginRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginRoot.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBRLoginRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.DomainName = serializedObject.DomainName;
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBRLoginRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRAccountRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRAccountRoot.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBRAccountRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBRAccountRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRGroupRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRGroupRoot.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBRGroupRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Group != null)
						objectToAdd.GroupID = serializedObject.Group.ID;
					else
						objectToAdd.GroupID = null;
					TBRGroupRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBRLoginGroupRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBRLoginGroupRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.GroupID = serializedObject.GroupID;
					TBRLoginGroupRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBREmailRoot":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBREmailRoot.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBREmailRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Account != null)
						objectToAdd.AccountID = serializedObject.Account.ID;
					else
						objectToAdd.AccountID = null;
					TBREmailRootTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBAccount":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Emails != null)
						objectToAdd.EmailsID = serializedObject.Emails.ID;
					else
						objectToAdd.EmailsID = null;
					if(serializedObject.Logins != null)
						objectToAdd.LoginsID = serializedObject.Logins.ID;
					else
						objectToAdd.LoginsID = null;
					if(serializedObject.GroupRoleCollection != null)
						objectToAdd.GroupRoleCollectionID = serializedObject.GroupRoleCollection.ID;
					else
						objectToAdd.GroupRoleCollectionID = null;
					TBAccountTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBAccountCollaborationGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBAccountCollaborationGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.GroupRole = serializedObject.GroupRole;
		            objectToAdd.RoleStatus = serializedObject.RoleStatus;
					TBAccountCollaborationGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBLoginInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBLoginInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OpenIDUrl = serializedObject.OpenIDUrl;
					TBLoginInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBEmail":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.ValidatedAt = serializedObject.ValidatedAt;
					TBEmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBCollaboratorRole":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBCollaboratorRole.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBCollaboratorRole {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Email != null)
						objectToAdd.EmailID = serializedObject.Email.ID;
					else
						objectToAdd.EmailID = null;
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.RoleStatus = serializedObject.RoleStatus;
					TBCollaboratorRoleTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBCollaboratingGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBCollaboratingGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					if(serializedObject.Roles != null)
						objectToAdd.RolesID = serializedObject.Roles.ID;
					else
						objectToAdd.RolesID = null;
					TBCollaboratingGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBEmailValidation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBEmailValidation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBEmailValidation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.ValidUntil = serializedObject.ValidUntil;
					if(serializedObject.GroupJoinConfirmation != null)
						objectToAdd.GroupJoinConfirmationID = serializedObject.GroupJoinConfirmation.ID;
					else
						objectToAdd.GroupJoinConfirmationID = null;
					if(serializedObject.DeviceJoinConfirmation != null)
						objectToAdd.DeviceJoinConfirmationID = serializedObject.DeviceJoinConfirmation.ID;
					else
						objectToAdd.DeviceJoinConfirmationID = null;
					if(serializedObject.InformationInputConfirmation != null)
						objectToAdd.InformationInputConfirmationID = serializedObject.InformationInputConfirmation.ID;
					else
						objectToAdd.InformationInputConfirmationID = null;
					if(serializedObject.InformationOutputConfirmation != null)
						objectToAdd.InformationOutputConfirmationID = serializedObject.InformationOutputConfirmation.ID;
					else
						objectToAdd.InformationOutputConfirmationID = null;
					if(serializedObject.MergeAccountsConfirmation != null)
						objectToAdd.MergeAccountsConfirmationID = serializedObject.MergeAccountsConfirmation.ID;
					else
						objectToAdd.MergeAccountsConfirmationID = null;
		            objectToAdd.RedirectUrlAfterValidation = serializedObject.RedirectUrlAfterValidation;
					TBEmailValidationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBMergeAccountConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBMergeAccountConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            objectToAdd.AccountToMergeToID = serializedObject.AccountToMergeToID;
					TBMergeAccountConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBGroupJoinConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBGroupJoinConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.InvitationMode = serializedObject.InvitationMode;
					TBGroupJoinConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBDeviceJoinConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBDeviceJoinConfirmation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBDeviceJoinConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.DeviceMembershipID = serializedObject.DeviceMembershipID;
					TBDeviceJoinConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBInformationInputConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBInformationInputConfirmation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBInformationInputConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.InformationInputID = serializedObject.InformationInputID;
					TBInformationInputConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBInformationOutputConfirmation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBInformationOutputConfirmation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBInformationOutputConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.InformationOutputID = serializedObject.InformationOutputID;
					TBInformationOutputConfirmationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LoginProvider":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LoginProvider.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new LoginProvider {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ProviderName = serializedObject.ProviderName;
		            objectToAdd.ProviderIconClass = serializedObject.ProviderIconClass;
		            objectToAdd.ProviderType = serializedObject.ProviderType;
		            objectToAdd.ProviderUrl = serializedObject.ProviderUrl;
		            objectToAdd.ReturnUrl = serializedObject.ReturnUrl;
					LoginProviderTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TBPRegisterEmail":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TBPRegisterEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					TBPRegisterEmailTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountSummary":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.GroupSummary != null)
						objectToAdd.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						objectToAdd.GroupSummaryID = null;
					AccountSummaryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.AccountModule != null)
						objectToAdd.AccountModuleID = serializedObject.AccountModule.ID;
					else
						objectToAdd.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						objectToAdd.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						objectToAdd.AccountSummaryID = null;
					AccountContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountModule":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountModule.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountModule {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Profile != null)
						objectToAdd.ProfileID = serializedObject.Profile.ID;
					else
						objectToAdd.ProfileID = null;
					if(serializedObject.Security != null)
						objectToAdd.SecurityID = serializedObject.Security.ID;
					else
						objectToAdd.SecurityID = null;
					if(serializedObject.Roles != null)
						objectToAdd.RolesID = serializedObject.Roles.ID;
					else
						objectToAdd.RolesID = null;
					if(serializedObject.LocationCollection != null)
						objectToAdd.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						objectToAdd.LocationCollectionID = null;
					AccountModuleTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LocationContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new LocationContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					LocationContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddressAndLocation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddressAndLocation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddressAndLocation {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.Address != null)
						objectToAdd.AddressID = serializedObject.Address.ID;
					else
						objectToAdd.AddressID = null;
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					AddressAndLocationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "StreetAddress":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new StreetAddress {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Street = serializedObject.Street;
		            objectToAdd.ZipCode = serializedObject.ZipCode;
		            objectToAdd.Town = serializedObject.Town;
		            objectToAdd.Country = serializedObject.Country;
					StreetAddressTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountProfile":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountProfile.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountProfile {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProfileImage != null)
						objectToAdd.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						objectToAdd.ProfileImageID = null;
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
					if(serializedObject.Address != null)
						objectToAdd.AddressID = serializedObject.Address.ID;
					else
						objectToAdd.AddressID = null;
		            objectToAdd.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            objectToAdd.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            objectToAdd.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
					AccountProfileTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountSecurity":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountSecurity {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.LoginInfoCollection != null)
						objectToAdd.LoginInfoCollectionID = serializedObject.LoginInfoCollection.ID;
					else
						objectToAdd.LoginInfoCollectionID = null;
					if(serializedObject.EmailCollection != null)
						objectToAdd.EmailCollectionID = serializedObject.EmailCollection.ID;
					else
						objectToAdd.EmailCollectionID = null;
					AccountSecurityTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AccountRoles":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AccountRoles {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ModeratorInGroups != null)
						objectToAdd.ModeratorInGroupsID = serializedObject.ModeratorInGroups.ID;
					else
						objectToAdd.ModeratorInGroupsID = null;
					if(serializedObject.MemberInGroups != null)
						objectToAdd.MemberInGroupsID = serializedObject.MemberInGroups.ID;
					else
						objectToAdd.MemberInGroupsID = null;
		            objectToAdd.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
					AccountRolesTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "PersonalInfoVisibility":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new PersonalInfoVisibility {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.NoOne_Network_All = serializedObject.NoOne_Network_All;
					PersonalInfoVisibilityTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ReferenceToInformation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ReferenceToInformation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.URL = serializedObject.URL;
					ReferenceToInformationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "NodeSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new NodeSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Nodes != null)
						objectToAdd.NodesID = serializedObject.Nodes.ID;
					else
						objectToAdd.NodesID = null;
					if(serializedObject.NodeSourceTextContent != null)
						objectToAdd.NodeSourceTextContentID = serializedObject.NodeSourceTextContent.ID;
					else
						objectToAdd.NodeSourceTextContentID = null;
					if(serializedObject.NodeSourceLinkToContent != null)
						objectToAdd.NodeSourceLinkToContentID = serializedObject.NodeSourceLinkToContent.ID;
					else
						objectToAdd.NodeSourceLinkToContentID = null;
					if(serializedObject.NodeSourceEmbeddedContent != null)
						objectToAdd.NodeSourceEmbeddedContentID = serializedObject.NodeSourceEmbeddedContent.ID;
					else
						objectToAdd.NodeSourceEmbeddedContentID = null;
					if(serializedObject.NodeSourceImages != null)
						objectToAdd.NodeSourceImagesID = serializedObject.NodeSourceImages.ID;
					else
						objectToAdd.NodeSourceImagesID = null;
					if(serializedObject.NodeSourceBinaryFiles != null)
						objectToAdd.NodeSourceBinaryFilesID = serializedObject.NodeSourceBinaryFiles.ID;
					else
						objectToAdd.NodeSourceBinaryFilesID = null;
					if(serializedObject.NodeSourceCategories != null)
						objectToAdd.NodeSourceCategoriesID = serializedObject.NodeSourceCategories.ID;
					else
						objectToAdd.NodeSourceCategoriesID = null;
					NodeSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "RenderedNode":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new RenderedNode {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OriginalContentID = serializedObject.OriginalContentID;
		            objectToAdd.TechnicalSource = serializedObject.TechnicalSource;
		            objectToAdd.ImageBaseUrl = serializedObject.ImageBaseUrl;
		            objectToAdd.ImageExt = serializedObject.ImageExt;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.ActualContentUrl = serializedObject.ActualContentUrl;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.TimestampText = serializedObject.TimestampText;
		            objectToAdd.MainSortableText = serializedObject.MainSortableText;
		            objectToAdd.IsCategoryFilteringNode = serializedObject.IsCategoryFilteringNode;
					if(serializedObject.CategoryFilters != null)
						objectToAdd.CategoryFiltersID = serializedObject.CategoryFilters.ID;
					else
						objectToAdd.CategoryFiltersID = null;
					if(serializedObject.CategoryNames != null)
						objectToAdd.CategoryNamesID = serializedObject.CategoryNames.ID;
					else
						objectToAdd.CategoryNamesID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
		            objectToAdd.CategoryIDList = serializedObject.CategoryIDList;
					if(serializedObject.Authors != null)
						objectToAdd.AuthorsID = serializedObject.Authors.ID;
					else
						objectToAdd.AuthorsID = null;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					RenderedNodeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ShortTextObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					ShortTextObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LongTextObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new LongTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					LongTextObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapMarker":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new MapMarker {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IconUrl = serializedObject.IconUrl;
		            objectToAdd.MarkerSource = serializedObject.MarkerSource;
		            objectToAdd.CategoryName = serializedObject.CategoryName;
		            objectToAdd.LocationText = serializedObject.LocationText;
		            objectToAdd.PopupTitle = serializedObject.PopupTitle;
		            objectToAdd.PopupContent = serializedObject.PopupContent;
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					MapMarkerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Moderator":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Moderator {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ModeratorName = serializedObject.ModeratorName;
		            objectToAdd.ProfileUrl = serializedObject.ProfileUrl;
					ModeratorTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Collaborator":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Collaborator.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Collaborator {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.CollaboratorName = serializedObject.CollaboratorName;
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.ProfileUrl = serializedObject.ProfileUrl;
					CollaboratorTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupSummaryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GroupSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						objectToAdd.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						objectToAdd.GroupSummaryIndexID = null;
					if(serializedObject.GroupCollection != null)
						objectToAdd.GroupCollectionID = serializedObject.GroupCollection.ID;
					else
						objectToAdd.GroupCollectionID = null;
					GroupSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GroupContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.GroupIndex != null)
						objectToAdd.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						objectToAdd.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						objectToAdd.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						objectToAdd.GroupProfileID = null;
					if(serializedObject.Collaborators != null)
						objectToAdd.CollaboratorsID = serializedObject.Collaborators.ID;
					else
						objectToAdd.CollaboratorsID = null;
					if(serializedObject.PendingCollaborators != null)
						objectToAdd.PendingCollaboratorsID = serializedObject.PendingCollaborators.ID;
					else
						objectToAdd.PendingCollaboratorsID = null;
					if(serializedObject.LocationCollection != null)
						objectToAdd.LocationCollectionID = serializedObject.LocationCollection.ID;
					else
						objectToAdd.LocationCollectionID = null;
					GroupContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "GroupIndex":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new GroupIndex {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					GroupIndexTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddAddressAndLocationInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddAddressAndLocationInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LocationName = serializedObject.LocationName;
					AddAddressAndLocationInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddImageInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddImageInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageTitle = serializedObject.ImageTitle;
					AddImageInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddImageGroupInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddImageGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageGroupTitle = serializedObject.ImageGroupTitle;
					AddImageGroupInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddEmailAddressInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddEmailAddressInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					AddEmailAddressInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "CreateGroupInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new CreateGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					CreateGroupInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddActivityInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddActivityInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ActivityName = serializedObject.ActivityName;
					AddActivityInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddBlogPostInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddBlogPostInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					AddBlogPostInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AddCategoryInfo":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AddCategoryInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					AddCategoryInfoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Group":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Group.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Group {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.ProfileImage != null)
						objectToAdd.ProfileImageID = serializedObject.ProfileImage.ID;
					else
						objectToAdd.ProfileImageID = null;
					if(serializedObject.IconImage != null)
						objectToAdd.IconImageID = serializedObject.IconImage.ID;
					else
						objectToAdd.IconImageID = null;
		            objectToAdd.GroupName = serializedObject.GroupName;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.OrganizationsAndGroupsLinkedToUs = serializedObject.OrganizationsAndGroupsLinkedToUs;
		            objectToAdd.WwwSiteToPublishTo = serializedObject.WwwSiteToPublishTo;
					if(serializedObject.CustomUICollection != null)
						objectToAdd.CustomUICollectionID = serializedObject.CustomUICollection.ID;
					else
						objectToAdd.CustomUICollectionID = null;
					if(serializedObject.Moderators != null)
						objectToAdd.ModeratorsID = serializedObject.Moderators.ID;
					else
						objectToAdd.ModeratorsID = null;
					if(serializedObject.CategoryCollection != null)
						objectToAdd.CategoryCollectionID = serializedObject.CategoryCollection.ID;
					else
						objectToAdd.CategoryCollectionID = null;
					GroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Introduction":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Introduction {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Body = serializedObject.Body;
					IntroductionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "ContentCategoryRank":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ContentCategoryRank.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new ContentCategoryRank {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ContentID = serializedObject.ContentID;
		            objectToAdd.ContentSemanticType = serializedObject.ContentSemanticType;
		            objectToAdd.CategoryID = serializedObject.CategoryID;
		            objectToAdd.RankName = serializedObject.RankName;
		            objectToAdd.RankValue = serializedObject.RankValue;
					ContentCategoryRankTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "LinkToContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LinkToContent.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new LinkToContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.URL = serializedObject.URL;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					LinkToContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "EmbeddedContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new EmbeddedContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.IFrameTagContents = serializedObject.IFrameTagContents;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					EmbeddedContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "DynamicContentGroup":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DynamicContentGroup.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new DynamicContentGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HostName = serializedObject.HostName;
		            objectToAdd.GroupHeader = serializedObject.GroupHeader;
		            objectToAdd.SortValue = serializedObject.SortValue;
		            objectToAdd.PageLocation = serializedObject.PageLocation;
		            objectToAdd.ContentItemNames = serializedObject.ContentItemNames;
					DynamicContentGroupTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "DynamicContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DynamicContent.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new DynamicContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HostName = serializedObject.HostName;
		            objectToAdd.ContentName = serializedObject.ContentName;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.ElementQuery = serializedObject.ElementQuery;
		            objectToAdd.Content = serializedObject.Content;
		            objectToAdd.RawContent = serializedObject.RawContent;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.IsEnabled = serializedObject.IsEnabled;
		            objectToAdd.ApplyActively = serializedObject.ApplyActively;
		            objectToAdd.EditType = serializedObject.EditType;
		            objectToAdd.PageLocation = serializedObject.PageLocation;
					DynamicContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "AttachedToObject":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AttachedToObject.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new AttachedToObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceObjectID = serializedObject.SourceObjectID;
		            objectToAdd.SourceObjectName = serializedObject.SourceObjectName;
		            objectToAdd.SourceObjectDomain = serializedObject.SourceObjectDomain;
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
					AttachedToObjectTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Comment":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Comment.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Comment {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            objectToAdd.CommentText = serializedObject.CommentText;
		            objectToAdd.Created = serializedObject.Created;
		            objectToAdd.OriginalAuthorName = serializedObject.OriginalAuthorName;
		            objectToAdd.OriginalAuthorEmail = serializedObject.OriginalAuthorEmail;
		            objectToAdd.OriginalAuthorAccountID = serializedObject.OriginalAuthorAccountID;
		            objectToAdd.LastModified = serializedObject.LastModified;
		            objectToAdd.LastAuthorName = serializedObject.LastAuthorName;
		            objectToAdd.LastAuthorEmail = serializedObject.LastAuthorEmail;
		            objectToAdd.LastAuthorAccountID = serializedObject.LastAuthorAccountID;
					CommentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Selection":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Selection.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Selection {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TargetObjectID = serializedObject.TargetObjectID;
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetObjectDomain = serializedObject.TargetObjectDomain;
		            objectToAdd.SelectionCategory = serializedObject.SelectionCategory;
		            objectToAdd.TextValue = serializedObject.TextValue;
		            objectToAdd.BooleanValue = serializedObject.BooleanValue;
		            objectToAdd.DoubleValue = serializedObject.DoubleValue;
					SelectionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "TextContent":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TextContent.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new TextContent {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.SubTitle = serializedObject.SubTitle;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Body = serializedObject.Body;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
		            objectToAdd.SortOrderNumber = serializedObject.SortOrderNumber;
		            objectToAdd.IFrameSources = serializedObject.IFrameSources;
		            objectToAdd.RawHtmlContent = serializedObject.RawHtmlContent;
					TextContentTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Map":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Map {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					MapTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapResult":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapResult.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new MapResult {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Location != null)
						objectToAdd.LocationID = serializedObject.Location.ID;
					else
						objectToAdd.LocationID = null;
					MapResultTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "MapResultsCollection":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new MapResultsCollection {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ResultByDate != null)
						objectToAdd.ResultByDateID = serializedObject.ResultByDate.ID;
					else
						objectToAdd.ResultByDateID = null;
					if(serializedObject.ResultByAuthor != null)
						objectToAdd.ResultByAuthorID = serializedObject.ResultByAuthor.ID;
					else
						objectToAdd.ResultByAuthorID = null;
					if(serializedObject.ResultByProximity != null)
						objectToAdd.ResultByProximityID = serializedObject.ResultByProximity.ID;
					else
						objectToAdd.ResultByProximityID = null;
					MapResultsCollectionTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Video":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Video {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.VideoData != null)
						objectToAdd.VideoDataID = serializedObject.VideoData.ID;
					else
						objectToAdd.VideoDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Caption = serializedObject.Caption;
					VideoTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Image":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Image.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Image {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Caption = serializedObject.Caption;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Locations != null)
						objectToAdd.LocationsID = serializedObject.Locations.ID;
					else
						objectToAdd.LocationsID = null;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					ImageTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "BinaryFile":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.BinaryFile.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new BinaryFile {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OriginalFileName = serializedObject.OriginalFileName;
					if(serializedObject.Data != null)
						objectToAdd.DataID = serializedObject.Data.ID;
					else
						objectToAdd.DataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					BinaryFileTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Longitude":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Longitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LongitudeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Latitude":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Latitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LatitudeTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Location":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Location.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Location {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LocationName = serializedObject.LocationName;
					if(serializedObject.Longitude != null)
						objectToAdd.LongitudeID = serializedObject.Longitude.ID;
					else
						objectToAdd.LongitudeID = null;
					if(serializedObject.Latitude != null)
						objectToAdd.LatitudeID = serializedObject.Latitude.ID;
					else
						objectToAdd.LatitudeID = null;
					LocationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Date":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Date {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Day = serializedObject.Day;
		            objectToAdd.Week = serializedObject.Week;
		            objectToAdd.Month = serializedObject.Month;
		            objectToAdd.Year = serializedObject.Year;
					DateTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "CategoryContainer":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new CategoryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Categories != null)
						objectToAdd.CategoriesID = serializedObject.Categories.ID;
					else
						objectToAdd.CategoriesID = null;
					CategoryContainerTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "Category":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					if(serializedObject.ImageData != null)
						objectToAdd.ImageDataID = serializedObject.ImageData.ID;
					else
						objectToAdd.ImageDataID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
					if(serializedObject.ParentCategory != null)
						objectToAdd.ParentCategoryID = serializedObject.ParentCategory.ID;
					else
						objectToAdd.ParentCategoryID = null;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UpdateWebContentOperation":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.UpdateWebContentOperation.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new UpdateWebContentOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceContainerName = serializedObject.SourceContainerName;
		            objectToAdd.SourcePathRoot = serializedObject.SourcePathRoot;
		            objectToAdd.TargetContainerName = serializedObject.TargetContainerName;
		            objectToAdd.TargetPathRoot = serializedObject.TargetPathRoot;
		            objectToAdd.RenderWhileSync = serializedObject.RenderWhileSync;
					if(serializedObject.Handlers != null)
						objectToAdd.HandlersID = serializedObject.Handlers.ID;
					else
						objectToAdd.HandlersID = null;
					UpdateWebContentOperationTable.InsertOnSubmit(objectToAdd);
                    break;
                }
                case "UpdateWebContentHandlerItem":
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
                            await ContentStorage.GetContentAsStringAsync(currentFullStoragePath));
                    var objectToAdd = new UpdateWebContentHandlerItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InformationTypeName = serializedObject.InformationTypeName;
		            objectToAdd.OptionName = serializedObject.OptionName;
					UpdateWebContentHandlerItemTable.InsertOnSubmit(objectToAdd);
                    break;
                }
				}
            }


		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "TBSystem":
					{
						//var objectToDelete = new TBSystem {ID = deleteData.ObjectID};
						//TBSystemTable.Attach(objectToDelete);
						var objectToDelete = TBSystemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBSystemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "WebPublishInfo":
					{
						//var objectToDelete = new WebPublishInfo {ID = deleteData.ObjectID};
						//WebPublishInfoTable.Attach(objectToDelete);
						var objectToDelete = WebPublishInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							WebPublishInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PublicationPackage":
					{
						//var objectToDelete = new PublicationPackage {ID = deleteData.ObjectID};
						//PublicationPackageTable.Attach(objectToDelete);
						var objectToDelete = PublicationPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PublicationPackageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRLoginRoot":
					{
						//var objectToDelete = new TBRLoginRoot {ID = deleteData.ObjectID};
						//TBRLoginRootTable.Attach(objectToDelete);
						var objectToDelete = TBRLoginRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRLoginRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRAccountRoot":
					{
						//var objectToDelete = new TBRAccountRoot {ID = deleteData.ObjectID};
						//TBRAccountRootTable.Attach(objectToDelete);
						var objectToDelete = TBRAccountRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRAccountRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRGroupRoot":
					{
						//var objectToDelete = new TBRGroupRoot {ID = deleteData.ObjectID};
						//TBRGroupRootTable.Attach(objectToDelete);
						var objectToDelete = TBRGroupRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRGroupRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRLoginGroupRoot":
					{
						//var objectToDelete = new TBRLoginGroupRoot {ID = deleteData.ObjectID};
						//TBRLoginGroupRootTable.Attach(objectToDelete);
						var objectToDelete = TBRLoginGroupRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRLoginGroupRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBREmailRoot":
					{
						//var objectToDelete = new TBREmailRoot {ID = deleteData.ObjectID};
						//TBREmailRootTable.Attach(objectToDelete);
						var objectToDelete = TBREmailRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBREmailRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccount":
					{
						//var objectToDelete = new TBAccount {ID = deleteData.ObjectID};
						//TBAccountTable.Attach(objectToDelete);
						var objectToDelete = TBAccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccountCollaborationGroup":
					{
						//var objectToDelete = new TBAccountCollaborationGroup {ID = deleteData.ObjectID};
						//TBAccountCollaborationGroupTable.Attach(objectToDelete);
						var objectToDelete = TBAccountCollaborationGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountCollaborationGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBLoginInfo":
					{
						//var objectToDelete = new TBLoginInfo {ID = deleteData.ObjectID};
						//TBLoginInfoTable.Attach(objectToDelete);
						var objectToDelete = TBLoginInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBLoginInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmail":
					{
						//var objectToDelete = new TBEmail {ID = deleteData.ObjectID};
						//TBEmailTable.Attach(objectToDelete);
						var objectToDelete = TBEmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratorRole":
					{
						//var objectToDelete = new TBCollaboratorRole {ID = deleteData.ObjectID};
						//TBCollaboratorRoleTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratorRoleTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratorRoleTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratingGroup":
					{
						//var objectToDelete = new TBCollaboratingGroup {ID = deleteData.ObjectID};
						//TBCollaboratingGroupTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratingGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratingGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmailValidation":
					{
						//var objectToDelete = new TBEmailValidation {ID = deleteData.ObjectID};
						//TBEmailValidationTable.Attach(objectToDelete);
						var objectToDelete = TBEmailValidationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailValidationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBMergeAccountConfirmation":
					{
						//var objectToDelete = new TBMergeAccountConfirmation {ID = deleteData.ObjectID};
						//TBMergeAccountConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBMergeAccountConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBMergeAccountConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBGroupJoinConfirmation":
					{
						//var objectToDelete = new TBGroupJoinConfirmation {ID = deleteData.ObjectID};
						//TBGroupJoinConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBGroupJoinConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBGroupJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBDeviceJoinConfirmation":
					{
						//var objectToDelete = new TBDeviceJoinConfirmation {ID = deleteData.ObjectID};
						//TBDeviceJoinConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBDeviceJoinConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBDeviceJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBInformationInputConfirmation":
					{
						//var objectToDelete = new TBInformationInputConfirmation {ID = deleteData.ObjectID};
						//TBInformationInputConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBInformationInputConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBInformationInputConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBInformationOutputConfirmation":
					{
						//var objectToDelete = new TBInformationOutputConfirmation {ID = deleteData.ObjectID};
						//TBInformationOutputConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBInformationOutputConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBInformationOutputConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LoginProvider":
					{
						//var objectToDelete = new LoginProvider {ID = deleteData.ObjectID};
						//LoginProviderTable.Attach(objectToDelete);
						var objectToDelete = LoginProviderTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginProviderTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBPRegisterEmail":
					{
						//var objectToDelete = new TBPRegisterEmail {ID = deleteData.ObjectID};
						//TBPRegisterEmailTable.Attach(objectToDelete);
						var objectToDelete = TBPRegisterEmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBPRegisterEmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountSummary":
					{
						//var objectToDelete = new AccountSummary {ID = deleteData.ObjectID};
						//AccountSummaryTable.Attach(objectToDelete);
						var objectToDelete = AccountSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountContainer":
					{
						//var objectToDelete = new AccountContainer {ID = deleteData.ObjectID};
						//AccountContainerTable.Attach(objectToDelete);
						var objectToDelete = AccountContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountModule":
					{
						//var objectToDelete = new AccountModule {ID = deleteData.ObjectID};
						//AccountModuleTable.Attach(objectToDelete);
						var objectToDelete = AccountModuleTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountModuleTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LocationContainer":
					{
						//var objectToDelete = new LocationContainer {ID = deleteData.ObjectID};
						//LocationContainerTable.Attach(objectToDelete);
						var objectToDelete = LocationContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddressAndLocation":
					{
						//var objectToDelete = new AddressAndLocation {ID = deleteData.ObjectID};
						//AddressAndLocationTable.Attach(objectToDelete);
						var objectToDelete = AddressAndLocationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddressAndLocationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StreetAddress":
					{
						//var objectToDelete = new StreetAddress {ID = deleteData.ObjectID};
						//StreetAddressTable.Attach(objectToDelete);
						var objectToDelete = StreetAddressTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StreetAddressTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountProfile":
					{
						//var objectToDelete = new AccountProfile {ID = deleteData.ObjectID};
						//AccountProfileTable.Attach(objectToDelete);
						var objectToDelete = AccountProfileTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountProfileTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountSecurity":
					{
						//var objectToDelete = new AccountSecurity {ID = deleteData.ObjectID};
						//AccountSecurityTable.Attach(objectToDelete);
						var objectToDelete = AccountSecurityTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountSecurityTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountRoles":
					{
						//var objectToDelete = new AccountRoles {ID = deleteData.ObjectID};
						//AccountRolesTable.Attach(objectToDelete);
						var objectToDelete = AccountRolesTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountRolesTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PersonalInfoVisibility":
					{
						//var objectToDelete = new PersonalInfoVisibility {ID = deleteData.ObjectID};
						//PersonalInfoVisibilityTable.Attach(objectToDelete);
						var objectToDelete = PersonalInfoVisibilityTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PersonalInfoVisibilityTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ReferenceToInformation":
					{
						//var objectToDelete = new ReferenceToInformation {ID = deleteData.ObjectID};
						//ReferenceToInformationTable.Attach(objectToDelete);
						var objectToDelete = ReferenceToInformationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ReferenceToInformationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NodeSummaryContainer":
					{
						//var objectToDelete = new NodeSummaryContainer {ID = deleteData.ObjectID};
						//NodeSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = NodeSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NodeSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RenderedNode":
					{
						//var objectToDelete = new RenderedNode {ID = deleteData.ObjectID};
						//RenderedNodeTable.Attach(objectToDelete);
						var objectToDelete = RenderedNodeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RenderedNodeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ShortTextObject":
					{
						//var objectToDelete = new ShortTextObject {ID = deleteData.ObjectID};
						//ShortTextObjectTable.Attach(objectToDelete);
						var objectToDelete = ShortTextObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ShortTextObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LongTextObject":
					{
						//var objectToDelete = new LongTextObject {ID = deleteData.ObjectID};
						//LongTextObjectTable.Attach(objectToDelete);
						var objectToDelete = LongTextObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongTextObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapMarker":
					{
						//var objectToDelete = new MapMarker {ID = deleteData.ObjectID};
						//MapMarkerTable.Attach(objectToDelete);
						var objectToDelete = MapMarkerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapMarkerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Moderator":
					{
						//var objectToDelete = new Moderator {ID = deleteData.ObjectID};
						//ModeratorTable.Attach(objectToDelete);
						var objectToDelete = ModeratorTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ModeratorTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Collaborator":
					{
						//var objectToDelete = new Collaborator {ID = deleteData.ObjectID};
						//CollaboratorTable.Attach(objectToDelete);
						var objectToDelete = CollaboratorTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CollaboratorTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupSummaryContainer":
					{
						//var objectToDelete = new GroupSummaryContainer {ID = deleteData.ObjectID};
						//GroupSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = GroupSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupContainer":
					{
						//var objectToDelete = new GroupContainer {ID = deleteData.ObjectID};
						//GroupContainerTable.Attach(objectToDelete);
						var objectToDelete = GroupContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupIndex":
					{
						//var objectToDelete = new GroupIndex {ID = deleteData.ObjectID};
						//GroupIndexTable.Attach(objectToDelete);
						var objectToDelete = GroupIndexTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupIndexTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddAddressAndLocationInfo":
					{
						//var objectToDelete = new AddAddressAndLocationInfo {ID = deleteData.ObjectID};
						//AddAddressAndLocationInfoTable.Attach(objectToDelete);
						var objectToDelete = AddAddressAndLocationInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddAddressAndLocationInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddImageInfo":
					{
						//var objectToDelete = new AddImageInfo {ID = deleteData.ObjectID};
						//AddImageInfoTable.Attach(objectToDelete);
						var objectToDelete = AddImageInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddImageInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddImageGroupInfo":
					{
						//var objectToDelete = new AddImageGroupInfo {ID = deleteData.ObjectID};
						//AddImageGroupInfoTable.Attach(objectToDelete);
						var objectToDelete = AddImageGroupInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddImageGroupInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddEmailAddressInfo":
					{
						//var objectToDelete = new AddEmailAddressInfo {ID = deleteData.ObjectID};
						//AddEmailAddressInfoTable.Attach(objectToDelete);
						var objectToDelete = AddEmailAddressInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddEmailAddressInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CreateGroupInfo":
					{
						//var objectToDelete = new CreateGroupInfo {ID = deleteData.ObjectID};
						//CreateGroupInfoTable.Attach(objectToDelete);
						var objectToDelete = CreateGroupInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CreateGroupInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddActivityInfo":
					{
						//var objectToDelete = new AddActivityInfo {ID = deleteData.ObjectID};
						//AddActivityInfoTable.Attach(objectToDelete);
						var objectToDelete = AddActivityInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddActivityInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddBlogPostInfo":
					{
						//var objectToDelete = new AddBlogPostInfo {ID = deleteData.ObjectID};
						//AddBlogPostInfoTable.Attach(objectToDelete);
						var objectToDelete = AddBlogPostInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddBlogPostInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddCategoryInfo":
					{
						//var objectToDelete = new AddCategoryInfo {ID = deleteData.ObjectID};
						//AddCategoryInfoTable.Attach(objectToDelete);
						var objectToDelete = AddCategoryInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddCategoryInfoTable.DeleteOnSubmit(objectToDelete);
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
					case "Introduction":
					{
						//var objectToDelete = new Introduction {ID = deleteData.ObjectID};
						//IntroductionTable.Attach(objectToDelete);
						var objectToDelete = IntroductionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							IntroductionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentCategoryRank":
					{
						//var objectToDelete = new ContentCategoryRank {ID = deleteData.ObjectID};
						//ContentCategoryRankTable.Attach(objectToDelete);
						var objectToDelete = ContentCategoryRankTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentCategoryRankTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LinkToContent":
					{
						//var objectToDelete = new LinkToContent {ID = deleteData.ObjectID};
						//LinkToContentTable.Attach(objectToDelete);
						var objectToDelete = LinkToContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LinkToContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "EmbeddedContent":
					{
						//var objectToDelete = new EmbeddedContent {ID = deleteData.ObjectID};
						//EmbeddedContentTable.Attach(objectToDelete);
						var objectToDelete = EmbeddedContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmbeddedContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentGroup":
					{
						//var objectToDelete = new DynamicContentGroup {ID = deleteData.ObjectID};
						//DynamicContentGroupTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContent":
					{
						//var objectToDelete = new DynamicContent {ID = deleteData.ObjectID};
						//DynamicContentTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AttachedToObject":
					{
						//var objectToDelete = new AttachedToObject {ID = deleteData.ObjectID};
						//AttachedToObjectTable.Attach(objectToDelete);
						var objectToDelete = AttachedToObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AttachedToObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Comment":
					{
						//var objectToDelete = new Comment {ID = deleteData.ObjectID};
						//CommentTable.Attach(objectToDelete);
						var objectToDelete = CommentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CommentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Selection":
					{
						//var objectToDelete = new Selection {ID = deleteData.ObjectID};
						//SelectionTable.Attach(objectToDelete);
						var objectToDelete = SelectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SelectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TextContent":
					{
						//var objectToDelete = new TextContent {ID = deleteData.ObjectID};
						//TextContentTable.Attach(objectToDelete);
						var objectToDelete = TextContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TextContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Map":
					{
						//var objectToDelete = new Map {ID = deleteData.ObjectID};
						//MapTable.Attach(objectToDelete);
						var objectToDelete = MapTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResult":
					{
						//var objectToDelete = new MapResult {ID = deleteData.ObjectID};
						//MapResultTable.Attach(objectToDelete);
						var objectToDelete = MapResultTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResultsCollection":
					{
						//var objectToDelete = new MapResultsCollection {ID = deleteData.ObjectID};
						//MapResultsCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapResultsCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultsCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Video":
					{
						//var objectToDelete = new Video {ID = deleteData.ObjectID};
						//VideoTable.Attach(objectToDelete);
						var objectToDelete = VideoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							VideoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Image":
					{
						//var objectToDelete = new Image {ID = deleteData.ObjectID};
						//ImageTable.Attach(objectToDelete);
						var objectToDelete = ImageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ImageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "BinaryFile":
					{
						//var objectToDelete = new BinaryFile {ID = deleteData.ObjectID};
						//BinaryFileTable.Attach(objectToDelete);
						var objectToDelete = BinaryFileTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							BinaryFileTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Longitude":
					{
						//var objectToDelete = new Longitude {ID = deleteData.ObjectID};
						//LongitudeTable.Attach(objectToDelete);
						var objectToDelete = LongitudeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongitudeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Latitude":
					{
						//var objectToDelete = new Latitude {ID = deleteData.ObjectID};
						//LatitudeTable.Attach(objectToDelete);
						var objectToDelete = LatitudeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LatitudeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Location":
					{
						//var objectToDelete = new Location {ID = deleteData.ObjectID};
						//LocationTable.Attach(objectToDelete);
						var objectToDelete = LocationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Date":
					{
						//var objectToDelete = new Date {ID = deleteData.ObjectID};
						//DateTable.Attach(objectToDelete);
						var objectToDelete = DateTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DateTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CategoryContainer":
					{
						//var objectToDelete = new CategoryContainer {ID = deleteData.ObjectID};
						//CategoryContainerTable.Attach(objectToDelete);
						var objectToDelete = CategoryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryContainerTable.DeleteOnSubmit(objectToDelete);
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
					case "UpdateWebContentOperation":
					{
						//var objectToDelete = new UpdateWebContentOperation {ID = deleteData.ObjectID};
						//UpdateWebContentOperationTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentOperationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentOperationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UpdateWebContentHandlerItem":
					{
						//var objectToDelete = new UpdateWebContentHandlerItem {ID = deleteData.ObjectID};
						//UpdateWebContentHandlerItemTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentHandlerItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentHandlerItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PublicationPackageCollection":
					{
						//var objectToDelete = new PublicationPackageCollection {ID = deleteData.ObjectID};
						//PublicationPackageCollectionTable.Attach(objectToDelete);
						var objectToDelete = PublicationPackageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PublicationPackageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccountCollaborationGroupCollection":
					{
						//var objectToDelete = new TBAccountCollaborationGroupCollection {ID = deleteData.ObjectID};
						//TBAccountCollaborationGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBAccountCollaborationGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountCollaborationGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBLoginInfoCollection":
					{
						//var objectToDelete = new TBLoginInfoCollection {ID = deleteData.ObjectID};
						//TBLoginInfoCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBLoginInfoCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBLoginInfoCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmailCollection":
					{
						//var objectToDelete = new TBEmailCollection {ID = deleteData.ObjectID};
						//TBEmailCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBEmailCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratorRoleCollection":
					{
						//var objectToDelete = new TBCollaboratorRoleCollection {ID = deleteData.ObjectID};
						//TBCollaboratorRoleCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratorRoleCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratorRoleCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LoginProviderCollection":
					{
						//var objectToDelete = new LoginProviderCollection {ID = deleteData.ObjectID};
						//LoginProviderCollectionTable.Attach(objectToDelete);
						var objectToDelete = LoginProviderCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginProviderCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddressAndLocationCollection":
					{
						//var objectToDelete = new AddressAndLocationCollection {ID = deleteData.ObjectID};
						//AddressAndLocationCollectionTable.Attach(objectToDelete);
						var objectToDelete = AddressAndLocationCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddressAndLocationCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ReferenceCollection":
					{
						//var objectToDelete = new ReferenceCollection {ID = deleteData.ObjectID};
						//ReferenceCollectionTable.Attach(objectToDelete);
						var objectToDelete = ReferenceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ReferenceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RenderedNodeCollection":
					{
						//var objectToDelete = new RenderedNodeCollection {ID = deleteData.ObjectID};
						//RenderedNodeCollectionTable.Attach(objectToDelete);
						var objectToDelete = RenderedNodeCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RenderedNodeCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ShortTextCollection":
					{
						//var objectToDelete = new ShortTextCollection {ID = deleteData.ObjectID};
						//ShortTextCollectionTable.Attach(objectToDelete);
						var objectToDelete = ShortTextCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ShortTextCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LongTextCollection":
					{
						//var objectToDelete = new LongTextCollection {ID = deleteData.ObjectID};
						//LongTextCollectionTable.Attach(objectToDelete);
						var objectToDelete = LongTextCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongTextCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapMarkerCollection":
					{
						//var objectToDelete = new MapMarkerCollection {ID = deleteData.ObjectID};
						//MapMarkerCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapMarkerCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapMarkerCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ModeratorCollection":
					{
						//var objectToDelete = new ModeratorCollection {ID = deleteData.ObjectID};
						//ModeratorCollectionTable.Attach(objectToDelete);
						var objectToDelete = ModeratorCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ModeratorCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CollaboratorCollection":
					{
						//var objectToDelete = new CollaboratorCollection {ID = deleteData.ObjectID};
						//CollaboratorCollectionTable.Attach(objectToDelete);
						var objectToDelete = CollaboratorCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CollaboratorCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupCollection":
					{
						//var objectToDelete = new GroupCollection {ID = deleteData.ObjectID};
						//GroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = GroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentCategoryRankCollection":
					{
						//var objectToDelete = new ContentCategoryRankCollection {ID = deleteData.ObjectID};
						//ContentCategoryRankCollectionTable.Attach(objectToDelete);
						var objectToDelete = ContentCategoryRankCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentCategoryRankCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LinkToContentCollection":
					{
						//var objectToDelete = new LinkToContentCollection {ID = deleteData.ObjectID};
						//LinkToContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = LinkToContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LinkToContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "EmbeddedContentCollection":
					{
						//var objectToDelete = new EmbeddedContentCollection {ID = deleteData.ObjectID};
						//EmbeddedContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = EmbeddedContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmbeddedContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentGroupCollection":
					{
						//var objectToDelete = new DynamicContentGroupCollection {ID = deleteData.ObjectID};
						//DynamicContentGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentCollection":
					{
						//var objectToDelete = new DynamicContentCollection {ID = deleteData.ObjectID};
						//DynamicContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AttachedToObjectCollection":
					{
						//var objectToDelete = new AttachedToObjectCollection {ID = deleteData.ObjectID};
						//AttachedToObjectCollectionTable.Attach(objectToDelete);
						var objectToDelete = AttachedToObjectCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AttachedToObjectCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CommentCollection":
					{
						//var objectToDelete = new CommentCollection {ID = deleteData.ObjectID};
						//CommentCollectionTable.Attach(objectToDelete);
						var objectToDelete = CommentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CommentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SelectionCollection":
					{
						//var objectToDelete = new SelectionCollection {ID = deleteData.ObjectID};
						//SelectionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SelectionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SelectionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TextContentCollection":
					{
						//var objectToDelete = new TextContentCollection {ID = deleteData.ObjectID};
						//TextContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = TextContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TextContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapCollection":
					{
						//var objectToDelete = new MapCollection {ID = deleteData.ObjectID};
						//MapCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResultCollection":
					{
						//var objectToDelete = new MapResultCollection {ID = deleteData.ObjectID};
						//MapResultCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapResultCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ImageCollection":
					{
						//var objectToDelete = new ImageCollection {ID = deleteData.ObjectID};
						//ImageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ImageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ImageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "BinaryFileCollection":
					{
						//var objectToDelete = new BinaryFileCollection {ID = deleteData.ObjectID};
						//BinaryFileCollectionTable.Attach(objectToDelete);
						var objectToDelete = BinaryFileCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							BinaryFileCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LocationCollection":
					{
						//var objectToDelete = new LocationCollection {ID = deleteData.ObjectID};
						//LocationCollectionTable.Attach(objectToDelete);
						var objectToDelete = LocationCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationCollectionTable.DeleteOnSubmit(objectToDelete);
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
					case "UpdateWebContentHandlerCollection":
					{
						//var objectToDelete = new UpdateWebContentHandlerCollection {ID = deleteData.ObjectID};
						//UpdateWebContentHandlerCollectionTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentHandlerCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentHandlerCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



		    public async Task PerformDeleteAsync(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);

				switch(deleteData.ObjectType)
				{
					case "TBSystem":
					{
						//var objectToDelete = new TBSystem {ID = deleteData.ObjectID};
						//TBSystemTable.Attach(objectToDelete);
						var objectToDelete = TBSystemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBSystemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "WebPublishInfo":
					{
						//var objectToDelete = new WebPublishInfo {ID = deleteData.ObjectID};
						//WebPublishInfoTable.Attach(objectToDelete);
						var objectToDelete = WebPublishInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							WebPublishInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PublicationPackage":
					{
						//var objectToDelete = new PublicationPackage {ID = deleteData.ObjectID};
						//PublicationPackageTable.Attach(objectToDelete);
						var objectToDelete = PublicationPackageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PublicationPackageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRLoginRoot":
					{
						//var objectToDelete = new TBRLoginRoot {ID = deleteData.ObjectID};
						//TBRLoginRootTable.Attach(objectToDelete);
						var objectToDelete = TBRLoginRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRLoginRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRAccountRoot":
					{
						//var objectToDelete = new TBRAccountRoot {ID = deleteData.ObjectID};
						//TBRAccountRootTable.Attach(objectToDelete);
						var objectToDelete = TBRAccountRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRAccountRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRGroupRoot":
					{
						//var objectToDelete = new TBRGroupRoot {ID = deleteData.ObjectID};
						//TBRGroupRootTable.Attach(objectToDelete);
						var objectToDelete = TBRGroupRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRGroupRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBRLoginGroupRoot":
					{
						//var objectToDelete = new TBRLoginGroupRoot {ID = deleteData.ObjectID};
						//TBRLoginGroupRootTable.Attach(objectToDelete);
						var objectToDelete = TBRLoginGroupRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBRLoginGroupRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBREmailRoot":
					{
						//var objectToDelete = new TBREmailRoot {ID = deleteData.ObjectID};
						//TBREmailRootTable.Attach(objectToDelete);
						var objectToDelete = TBREmailRootTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBREmailRootTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccount":
					{
						//var objectToDelete = new TBAccount {ID = deleteData.ObjectID};
						//TBAccountTable.Attach(objectToDelete);
						var objectToDelete = TBAccountTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccountCollaborationGroup":
					{
						//var objectToDelete = new TBAccountCollaborationGroup {ID = deleteData.ObjectID};
						//TBAccountCollaborationGroupTable.Attach(objectToDelete);
						var objectToDelete = TBAccountCollaborationGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountCollaborationGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBLoginInfo":
					{
						//var objectToDelete = new TBLoginInfo {ID = deleteData.ObjectID};
						//TBLoginInfoTable.Attach(objectToDelete);
						var objectToDelete = TBLoginInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBLoginInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmail":
					{
						//var objectToDelete = new TBEmail {ID = deleteData.ObjectID};
						//TBEmailTable.Attach(objectToDelete);
						var objectToDelete = TBEmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratorRole":
					{
						//var objectToDelete = new TBCollaboratorRole {ID = deleteData.ObjectID};
						//TBCollaboratorRoleTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratorRoleTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratorRoleTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratingGroup":
					{
						//var objectToDelete = new TBCollaboratingGroup {ID = deleteData.ObjectID};
						//TBCollaboratingGroupTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratingGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratingGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmailValidation":
					{
						//var objectToDelete = new TBEmailValidation {ID = deleteData.ObjectID};
						//TBEmailValidationTable.Attach(objectToDelete);
						var objectToDelete = TBEmailValidationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailValidationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBMergeAccountConfirmation":
					{
						//var objectToDelete = new TBMergeAccountConfirmation {ID = deleteData.ObjectID};
						//TBMergeAccountConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBMergeAccountConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBMergeAccountConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBGroupJoinConfirmation":
					{
						//var objectToDelete = new TBGroupJoinConfirmation {ID = deleteData.ObjectID};
						//TBGroupJoinConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBGroupJoinConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBGroupJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBDeviceJoinConfirmation":
					{
						//var objectToDelete = new TBDeviceJoinConfirmation {ID = deleteData.ObjectID};
						//TBDeviceJoinConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBDeviceJoinConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBDeviceJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBInformationInputConfirmation":
					{
						//var objectToDelete = new TBInformationInputConfirmation {ID = deleteData.ObjectID};
						//TBInformationInputConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBInformationInputConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBInformationInputConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBInformationOutputConfirmation":
					{
						//var objectToDelete = new TBInformationOutputConfirmation {ID = deleteData.ObjectID};
						//TBInformationOutputConfirmationTable.Attach(objectToDelete);
						var objectToDelete = TBInformationOutputConfirmationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBInformationOutputConfirmationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LoginProvider":
					{
						//var objectToDelete = new LoginProvider {ID = deleteData.ObjectID};
						//LoginProviderTable.Attach(objectToDelete);
						var objectToDelete = LoginProviderTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginProviderTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBPRegisterEmail":
					{
						//var objectToDelete = new TBPRegisterEmail {ID = deleteData.ObjectID};
						//TBPRegisterEmailTable.Attach(objectToDelete);
						var objectToDelete = TBPRegisterEmailTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBPRegisterEmailTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountSummary":
					{
						//var objectToDelete = new AccountSummary {ID = deleteData.ObjectID};
						//AccountSummaryTable.Attach(objectToDelete);
						var objectToDelete = AccountSummaryTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountSummaryTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountContainer":
					{
						//var objectToDelete = new AccountContainer {ID = deleteData.ObjectID};
						//AccountContainerTable.Attach(objectToDelete);
						var objectToDelete = AccountContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountModule":
					{
						//var objectToDelete = new AccountModule {ID = deleteData.ObjectID};
						//AccountModuleTable.Attach(objectToDelete);
						var objectToDelete = AccountModuleTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountModuleTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LocationContainer":
					{
						//var objectToDelete = new LocationContainer {ID = deleteData.ObjectID};
						//LocationContainerTable.Attach(objectToDelete);
						var objectToDelete = LocationContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddressAndLocation":
					{
						//var objectToDelete = new AddressAndLocation {ID = deleteData.ObjectID};
						//AddressAndLocationTable.Attach(objectToDelete);
						var objectToDelete = AddressAndLocationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddressAndLocationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "StreetAddress":
					{
						//var objectToDelete = new StreetAddress {ID = deleteData.ObjectID};
						//StreetAddressTable.Attach(objectToDelete);
						var objectToDelete = StreetAddressTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							StreetAddressTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountProfile":
					{
						//var objectToDelete = new AccountProfile {ID = deleteData.ObjectID};
						//AccountProfileTable.Attach(objectToDelete);
						var objectToDelete = AccountProfileTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountProfileTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountSecurity":
					{
						//var objectToDelete = new AccountSecurity {ID = deleteData.ObjectID};
						//AccountSecurityTable.Attach(objectToDelete);
						var objectToDelete = AccountSecurityTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountSecurityTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AccountRoles":
					{
						//var objectToDelete = new AccountRoles {ID = deleteData.ObjectID};
						//AccountRolesTable.Attach(objectToDelete);
						var objectToDelete = AccountRolesTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AccountRolesTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PersonalInfoVisibility":
					{
						//var objectToDelete = new PersonalInfoVisibility {ID = deleteData.ObjectID};
						//PersonalInfoVisibilityTable.Attach(objectToDelete);
						var objectToDelete = PersonalInfoVisibilityTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PersonalInfoVisibilityTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ReferenceToInformation":
					{
						//var objectToDelete = new ReferenceToInformation {ID = deleteData.ObjectID};
						//ReferenceToInformationTable.Attach(objectToDelete);
						var objectToDelete = ReferenceToInformationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ReferenceToInformationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "NodeSummaryContainer":
					{
						//var objectToDelete = new NodeSummaryContainer {ID = deleteData.ObjectID};
						//NodeSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = NodeSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							NodeSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RenderedNode":
					{
						//var objectToDelete = new RenderedNode {ID = deleteData.ObjectID};
						//RenderedNodeTable.Attach(objectToDelete);
						var objectToDelete = RenderedNodeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RenderedNodeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ShortTextObject":
					{
						//var objectToDelete = new ShortTextObject {ID = deleteData.ObjectID};
						//ShortTextObjectTable.Attach(objectToDelete);
						var objectToDelete = ShortTextObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ShortTextObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LongTextObject":
					{
						//var objectToDelete = new LongTextObject {ID = deleteData.ObjectID};
						//LongTextObjectTable.Attach(objectToDelete);
						var objectToDelete = LongTextObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongTextObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapMarker":
					{
						//var objectToDelete = new MapMarker {ID = deleteData.ObjectID};
						//MapMarkerTable.Attach(objectToDelete);
						var objectToDelete = MapMarkerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapMarkerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Moderator":
					{
						//var objectToDelete = new Moderator {ID = deleteData.ObjectID};
						//ModeratorTable.Attach(objectToDelete);
						var objectToDelete = ModeratorTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ModeratorTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Collaborator":
					{
						//var objectToDelete = new Collaborator {ID = deleteData.ObjectID};
						//CollaboratorTable.Attach(objectToDelete);
						var objectToDelete = CollaboratorTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CollaboratorTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupSummaryContainer":
					{
						//var objectToDelete = new GroupSummaryContainer {ID = deleteData.ObjectID};
						//GroupSummaryContainerTable.Attach(objectToDelete);
						var objectToDelete = GroupSummaryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupSummaryContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupContainer":
					{
						//var objectToDelete = new GroupContainer {ID = deleteData.ObjectID};
						//GroupContainerTable.Attach(objectToDelete);
						var objectToDelete = GroupContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupContainerTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupIndex":
					{
						//var objectToDelete = new GroupIndex {ID = deleteData.ObjectID};
						//GroupIndexTable.Attach(objectToDelete);
						var objectToDelete = GroupIndexTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupIndexTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddAddressAndLocationInfo":
					{
						//var objectToDelete = new AddAddressAndLocationInfo {ID = deleteData.ObjectID};
						//AddAddressAndLocationInfoTable.Attach(objectToDelete);
						var objectToDelete = AddAddressAndLocationInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddAddressAndLocationInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddImageInfo":
					{
						//var objectToDelete = new AddImageInfo {ID = deleteData.ObjectID};
						//AddImageInfoTable.Attach(objectToDelete);
						var objectToDelete = AddImageInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddImageInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddImageGroupInfo":
					{
						//var objectToDelete = new AddImageGroupInfo {ID = deleteData.ObjectID};
						//AddImageGroupInfoTable.Attach(objectToDelete);
						var objectToDelete = AddImageGroupInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddImageGroupInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddEmailAddressInfo":
					{
						//var objectToDelete = new AddEmailAddressInfo {ID = deleteData.ObjectID};
						//AddEmailAddressInfoTable.Attach(objectToDelete);
						var objectToDelete = AddEmailAddressInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddEmailAddressInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CreateGroupInfo":
					{
						//var objectToDelete = new CreateGroupInfo {ID = deleteData.ObjectID};
						//CreateGroupInfoTable.Attach(objectToDelete);
						var objectToDelete = CreateGroupInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CreateGroupInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddActivityInfo":
					{
						//var objectToDelete = new AddActivityInfo {ID = deleteData.ObjectID};
						//AddActivityInfoTable.Attach(objectToDelete);
						var objectToDelete = AddActivityInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddActivityInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddBlogPostInfo":
					{
						//var objectToDelete = new AddBlogPostInfo {ID = deleteData.ObjectID};
						//AddBlogPostInfoTable.Attach(objectToDelete);
						var objectToDelete = AddBlogPostInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddBlogPostInfoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddCategoryInfo":
					{
						//var objectToDelete = new AddCategoryInfo {ID = deleteData.ObjectID};
						//AddCategoryInfoTable.Attach(objectToDelete);
						var objectToDelete = AddCategoryInfoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddCategoryInfoTable.DeleteOnSubmit(objectToDelete);
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
					case "Introduction":
					{
						//var objectToDelete = new Introduction {ID = deleteData.ObjectID};
						//IntroductionTable.Attach(objectToDelete);
						var objectToDelete = IntroductionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							IntroductionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentCategoryRank":
					{
						//var objectToDelete = new ContentCategoryRank {ID = deleteData.ObjectID};
						//ContentCategoryRankTable.Attach(objectToDelete);
						var objectToDelete = ContentCategoryRankTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentCategoryRankTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LinkToContent":
					{
						//var objectToDelete = new LinkToContent {ID = deleteData.ObjectID};
						//LinkToContentTable.Attach(objectToDelete);
						var objectToDelete = LinkToContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LinkToContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "EmbeddedContent":
					{
						//var objectToDelete = new EmbeddedContent {ID = deleteData.ObjectID};
						//EmbeddedContentTable.Attach(objectToDelete);
						var objectToDelete = EmbeddedContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmbeddedContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentGroup":
					{
						//var objectToDelete = new DynamicContentGroup {ID = deleteData.ObjectID};
						//DynamicContentGroupTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentGroupTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentGroupTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContent":
					{
						//var objectToDelete = new DynamicContent {ID = deleteData.ObjectID};
						//DynamicContentTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AttachedToObject":
					{
						//var objectToDelete = new AttachedToObject {ID = deleteData.ObjectID};
						//AttachedToObjectTable.Attach(objectToDelete);
						var objectToDelete = AttachedToObjectTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AttachedToObjectTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Comment":
					{
						//var objectToDelete = new Comment {ID = deleteData.ObjectID};
						//CommentTable.Attach(objectToDelete);
						var objectToDelete = CommentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CommentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Selection":
					{
						//var objectToDelete = new Selection {ID = deleteData.ObjectID};
						//SelectionTable.Attach(objectToDelete);
						var objectToDelete = SelectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SelectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TextContent":
					{
						//var objectToDelete = new TextContent {ID = deleteData.ObjectID};
						//TextContentTable.Attach(objectToDelete);
						var objectToDelete = TextContentTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TextContentTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Map":
					{
						//var objectToDelete = new Map {ID = deleteData.ObjectID};
						//MapTable.Attach(objectToDelete);
						var objectToDelete = MapTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResult":
					{
						//var objectToDelete = new MapResult {ID = deleteData.ObjectID};
						//MapResultTable.Attach(objectToDelete);
						var objectToDelete = MapResultTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResultsCollection":
					{
						//var objectToDelete = new MapResultsCollection {ID = deleteData.ObjectID};
						//MapResultsCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapResultsCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultsCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Video":
					{
						//var objectToDelete = new Video {ID = deleteData.ObjectID};
						//VideoTable.Attach(objectToDelete);
						var objectToDelete = VideoTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							VideoTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Image":
					{
						//var objectToDelete = new Image {ID = deleteData.ObjectID};
						//ImageTable.Attach(objectToDelete);
						var objectToDelete = ImageTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ImageTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "BinaryFile":
					{
						//var objectToDelete = new BinaryFile {ID = deleteData.ObjectID};
						//BinaryFileTable.Attach(objectToDelete);
						var objectToDelete = BinaryFileTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							BinaryFileTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Longitude":
					{
						//var objectToDelete = new Longitude {ID = deleteData.ObjectID};
						//LongitudeTable.Attach(objectToDelete);
						var objectToDelete = LongitudeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongitudeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Latitude":
					{
						//var objectToDelete = new Latitude {ID = deleteData.ObjectID};
						//LatitudeTable.Attach(objectToDelete);
						var objectToDelete = LatitudeTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LatitudeTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Location":
					{
						//var objectToDelete = new Location {ID = deleteData.ObjectID};
						//LocationTable.Attach(objectToDelete);
						var objectToDelete = LocationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "Date":
					{
						//var objectToDelete = new Date {ID = deleteData.ObjectID};
						//DateTable.Attach(objectToDelete);
						var objectToDelete = DateTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DateTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CategoryContainer":
					{
						//var objectToDelete = new CategoryContainer {ID = deleteData.ObjectID};
						//CategoryContainerTable.Attach(objectToDelete);
						var objectToDelete = CategoryContainerTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CategoryContainerTable.DeleteOnSubmit(objectToDelete);
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
					case "UpdateWebContentOperation":
					{
						//var objectToDelete = new UpdateWebContentOperation {ID = deleteData.ObjectID};
						//UpdateWebContentOperationTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentOperationTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentOperationTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "UpdateWebContentHandlerItem":
					{
						//var objectToDelete = new UpdateWebContentHandlerItem {ID = deleteData.ObjectID};
						//UpdateWebContentHandlerItemTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentHandlerItemTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentHandlerItemTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "PublicationPackageCollection":
					{
						//var objectToDelete = new PublicationPackageCollection {ID = deleteData.ObjectID};
						//PublicationPackageCollectionTable.Attach(objectToDelete);
						var objectToDelete = PublicationPackageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							PublicationPackageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBAccountCollaborationGroupCollection":
					{
						//var objectToDelete = new TBAccountCollaborationGroupCollection {ID = deleteData.ObjectID};
						//TBAccountCollaborationGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBAccountCollaborationGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBAccountCollaborationGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBLoginInfoCollection":
					{
						//var objectToDelete = new TBLoginInfoCollection {ID = deleteData.ObjectID};
						//TBLoginInfoCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBLoginInfoCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBLoginInfoCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBEmailCollection":
					{
						//var objectToDelete = new TBEmailCollection {ID = deleteData.ObjectID};
						//TBEmailCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBEmailCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBEmailCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TBCollaboratorRoleCollection":
					{
						//var objectToDelete = new TBCollaboratorRoleCollection {ID = deleteData.ObjectID};
						//TBCollaboratorRoleCollectionTable.Attach(objectToDelete);
						var objectToDelete = TBCollaboratorRoleCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TBCollaboratorRoleCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LoginProviderCollection":
					{
						//var objectToDelete = new LoginProviderCollection {ID = deleteData.ObjectID};
						//LoginProviderCollectionTable.Attach(objectToDelete);
						var objectToDelete = LoginProviderCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LoginProviderCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AddressAndLocationCollection":
					{
						//var objectToDelete = new AddressAndLocationCollection {ID = deleteData.ObjectID};
						//AddressAndLocationCollectionTable.Attach(objectToDelete);
						var objectToDelete = AddressAndLocationCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AddressAndLocationCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ReferenceCollection":
					{
						//var objectToDelete = new ReferenceCollection {ID = deleteData.ObjectID};
						//ReferenceCollectionTable.Attach(objectToDelete);
						var objectToDelete = ReferenceCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ReferenceCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "RenderedNodeCollection":
					{
						//var objectToDelete = new RenderedNodeCollection {ID = deleteData.ObjectID};
						//RenderedNodeCollectionTable.Attach(objectToDelete);
						var objectToDelete = RenderedNodeCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							RenderedNodeCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ShortTextCollection":
					{
						//var objectToDelete = new ShortTextCollection {ID = deleteData.ObjectID};
						//ShortTextCollectionTable.Attach(objectToDelete);
						var objectToDelete = ShortTextCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ShortTextCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LongTextCollection":
					{
						//var objectToDelete = new LongTextCollection {ID = deleteData.ObjectID};
						//LongTextCollectionTable.Attach(objectToDelete);
						var objectToDelete = LongTextCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LongTextCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapMarkerCollection":
					{
						//var objectToDelete = new MapMarkerCollection {ID = deleteData.ObjectID};
						//MapMarkerCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapMarkerCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapMarkerCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ModeratorCollection":
					{
						//var objectToDelete = new ModeratorCollection {ID = deleteData.ObjectID};
						//ModeratorCollectionTable.Attach(objectToDelete);
						var objectToDelete = ModeratorCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ModeratorCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CollaboratorCollection":
					{
						//var objectToDelete = new CollaboratorCollection {ID = deleteData.ObjectID};
						//CollaboratorCollectionTable.Attach(objectToDelete);
						var objectToDelete = CollaboratorCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CollaboratorCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "GroupCollection":
					{
						//var objectToDelete = new GroupCollection {ID = deleteData.ObjectID};
						//GroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = GroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							GroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ContentCategoryRankCollection":
					{
						//var objectToDelete = new ContentCategoryRankCollection {ID = deleteData.ObjectID};
						//ContentCategoryRankCollectionTable.Attach(objectToDelete);
						var objectToDelete = ContentCategoryRankCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ContentCategoryRankCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LinkToContentCollection":
					{
						//var objectToDelete = new LinkToContentCollection {ID = deleteData.ObjectID};
						//LinkToContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = LinkToContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LinkToContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "EmbeddedContentCollection":
					{
						//var objectToDelete = new EmbeddedContentCollection {ID = deleteData.ObjectID};
						//EmbeddedContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = EmbeddedContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							EmbeddedContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentGroupCollection":
					{
						//var objectToDelete = new DynamicContentGroupCollection {ID = deleteData.ObjectID};
						//DynamicContentGroupCollectionTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentGroupCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentGroupCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "DynamicContentCollection":
					{
						//var objectToDelete = new DynamicContentCollection {ID = deleteData.ObjectID};
						//DynamicContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = DynamicContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							DynamicContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "AttachedToObjectCollection":
					{
						//var objectToDelete = new AttachedToObjectCollection {ID = deleteData.ObjectID};
						//AttachedToObjectCollectionTable.Attach(objectToDelete);
						var objectToDelete = AttachedToObjectCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							AttachedToObjectCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "CommentCollection":
					{
						//var objectToDelete = new CommentCollection {ID = deleteData.ObjectID};
						//CommentCollectionTable.Attach(objectToDelete);
						var objectToDelete = CommentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							CommentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "SelectionCollection":
					{
						//var objectToDelete = new SelectionCollection {ID = deleteData.ObjectID};
						//SelectionCollectionTable.Attach(objectToDelete);
						var objectToDelete = SelectionCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							SelectionCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "TextContentCollection":
					{
						//var objectToDelete = new TextContentCollection {ID = deleteData.ObjectID};
						//TextContentCollectionTable.Attach(objectToDelete);
						var objectToDelete = TextContentCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							TextContentCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapCollection":
					{
						//var objectToDelete = new MapCollection {ID = deleteData.ObjectID};
						//MapCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "MapResultCollection":
					{
						//var objectToDelete = new MapResultCollection {ID = deleteData.ObjectID};
						//MapResultCollectionTable.Attach(objectToDelete);
						var objectToDelete = MapResultCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							MapResultCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "ImageCollection":
					{
						//var objectToDelete = new ImageCollection {ID = deleteData.ObjectID};
						//ImageCollectionTable.Attach(objectToDelete);
						var objectToDelete = ImageCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							ImageCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "BinaryFileCollection":
					{
						//var objectToDelete = new BinaryFileCollection {ID = deleteData.ObjectID};
						//BinaryFileCollectionTable.Attach(objectToDelete);
						var objectToDelete = BinaryFileCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							BinaryFileCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
					case "LocationCollection":
					{
						//var objectToDelete = new LocationCollection {ID = deleteData.ObjectID};
						//LocationCollectionTable.Attach(objectToDelete);
						var objectToDelete = LocationCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							LocationCollectionTable.DeleteOnSubmit(objectToDelete);
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
					case "UpdateWebContentHandlerCollection":
					{
						//var objectToDelete = new UpdateWebContentHandlerCollection {ID = deleteData.ObjectID};
						//UpdateWebContentHandlerCollectionTable.Attach(objectToDelete);
						var objectToDelete = UpdateWebContentHandlerCollectionTable.SingleOrDefault(item => item.ID == deleteData.ObjectID);
						if(objectToDelete != null)
							UpdateWebContentHandlerCollectionTable.DeleteOnSubmit(objectToDelete);
						break;
					}
				}
			}



			public Table<TBSystem> TBSystemTable {
				get {
					return this.GetTable<TBSystem>();
				}
			}
			public Table<WebPublishInfo> WebPublishInfoTable {
				get {
					return this.GetTable<WebPublishInfo>();
				}
			}
			public Table<PublicationPackage> PublicationPackageTable {
				get {
					return this.GetTable<PublicationPackage>();
				}
			}
			public Table<TBRLoginRoot> TBRLoginRootTable {
				get {
					return this.GetTable<TBRLoginRoot>();
				}
			}
			public Table<TBRAccountRoot> TBRAccountRootTable {
				get {
					return this.GetTable<TBRAccountRoot>();
				}
			}
			public Table<TBRGroupRoot> TBRGroupRootTable {
				get {
					return this.GetTable<TBRGroupRoot>();
				}
			}
			public Table<TBRLoginGroupRoot> TBRLoginGroupRootTable {
				get {
					return this.GetTable<TBRLoginGroupRoot>();
				}
			}
			public Table<TBREmailRoot> TBREmailRootTable {
				get {
					return this.GetTable<TBREmailRoot>();
				}
			}
			public Table<TBAccount> TBAccountTable {
				get {
					return this.GetTable<TBAccount>();
				}
			}
			public Table<TBAccountCollaborationGroup> TBAccountCollaborationGroupTable {
				get {
					return this.GetTable<TBAccountCollaborationGroup>();
				}
			}
			public Table<TBLoginInfo> TBLoginInfoTable {
				get {
					return this.GetTable<TBLoginInfo>();
				}
			}
			public Table<TBEmail> TBEmailTable {
				get {
					return this.GetTable<TBEmail>();
				}
			}
			public Table<TBCollaboratorRole> TBCollaboratorRoleTable {
				get {
					return this.GetTable<TBCollaboratorRole>();
				}
			}
			public Table<TBCollaboratingGroup> TBCollaboratingGroupTable {
				get {
					return this.GetTable<TBCollaboratingGroup>();
				}
			}
			public Table<TBEmailValidation> TBEmailValidationTable {
				get {
					return this.GetTable<TBEmailValidation>();
				}
			}
			public Table<TBMergeAccountConfirmation> TBMergeAccountConfirmationTable {
				get {
					return this.GetTable<TBMergeAccountConfirmation>();
				}
			}
			public Table<TBGroupJoinConfirmation> TBGroupJoinConfirmationTable {
				get {
					return this.GetTable<TBGroupJoinConfirmation>();
				}
			}
			public Table<TBDeviceJoinConfirmation> TBDeviceJoinConfirmationTable {
				get {
					return this.GetTable<TBDeviceJoinConfirmation>();
				}
			}
			public Table<TBInformationInputConfirmation> TBInformationInputConfirmationTable {
				get {
					return this.GetTable<TBInformationInputConfirmation>();
				}
			}
			public Table<TBInformationOutputConfirmation> TBInformationOutputConfirmationTable {
				get {
					return this.GetTable<TBInformationOutputConfirmation>();
				}
			}
			public Table<LoginProvider> LoginProviderTable {
				get {
					return this.GetTable<LoginProvider>();
				}
			}
			public Table<TBPRegisterEmail> TBPRegisterEmailTable {
				get {
					return this.GetTable<TBPRegisterEmail>();
				}
			}
			public Table<AccountSummary> AccountSummaryTable {
				get {
					return this.GetTable<AccountSummary>();
				}
			}
			public Table<AccountContainer> AccountContainerTable {
				get {
					return this.GetTable<AccountContainer>();
				}
			}
			public Table<AccountModule> AccountModuleTable {
				get {
					return this.GetTable<AccountModule>();
				}
			}
			public Table<LocationContainer> LocationContainerTable {
				get {
					return this.GetTable<LocationContainer>();
				}
			}
			public Table<AddressAndLocation> AddressAndLocationTable {
				get {
					return this.GetTable<AddressAndLocation>();
				}
			}
			public Table<StreetAddress> StreetAddressTable {
				get {
					return this.GetTable<StreetAddress>();
				}
			}
			public Table<AccountProfile> AccountProfileTable {
				get {
					return this.GetTable<AccountProfile>();
				}
			}
			public Table<AccountSecurity> AccountSecurityTable {
				get {
					return this.GetTable<AccountSecurity>();
				}
			}
			public Table<AccountRoles> AccountRolesTable {
				get {
					return this.GetTable<AccountRoles>();
				}
			}
			public Table<PersonalInfoVisibility> PersonalInfoVisibilityTable {
				get {
					return this.GetTable<PersonalInfoVisibility>();
				}
			}
			public Table<ReferenceToInformation> ReferenceToInformationTable {
				get {
					return this.GetTable<ReferenceToInformation>();
				}
			}
			public Table<NodeSummaryContainer> NodeSummaryContainerTable {
				get {
					return this.GetTable<NodeSummaryContainer>();
				}
			}
			public Table<RenderedNode> RenderedNodeTable {
				get {
					return this.GetTable<RenderedNode>();
				}
			}
			public Table<ShortTextObject> ShortTextObjectTable {
				get {
					return this.GetTable<ShortTextObject>();
				}
			}
			public Table<LongTextObject> LongTextObjectTable {
				get {
					return this.GetTable<LongTextObject>();
				}
			}
			public Table<MapMarker> MapMarkerTable {
				get {
					return this.GetTable<MapMarker>();
				}
			}
			public Table<Moderator> ModeratorTable {
				get {
					return this.GetTable<Moderator>();
				}
			}
			public Table<Collaborator> CollaboratorTable {
				get {
					return this.GetTable<Collaborator>();
				}
			}
			public Table<GroupSummaryContainer> GroupSummaryContainerTable {
				get {
					return this.GetTable<GroupSummaryContainer>();
				}
			}
			public Table<GroupContainer> GroupContainerTable {
				get {
					return this.GetTable<GroupContainer>();
				}
			}
			public Table<GroupIndex> GroupIndexTable {
				get {
					return this.GetTable<GroupIndex>();
				}
			}
			public Table<AddAddressAndLocationInfo> AddAddressAndLocationInfoTable {
				get {
					return this.GetTable<AddAddressAndLocationInfo>();
				}
			}
			public Table<AddImageInfo> AddImageInfoTable {
				get {
					return this.GetTable<AddImageInfo>();
				}
			}
			public Table<AddImageGroupInfo> AddImageGroupInfoTable {
				get {
					return this.GetTable<AddImageGroupInfo>();
				}
			}
			public Table<AddEmailAddressInfo> AddEmailAddressInfoTable {
				get {
					return this.GetTable<AddEmailAddressInfo>();
				}
			}
			public Table<CreateGroupInfo> CreateGroupInfoTable {
				get {
					return this.GetTable<CreateGroupInfo>();
				}
			}
			public Table<AddActivityInfo> AddActivityInfoTable {
				get {
					return this.GetTable<AddActivityInfo>();
				}
			}
			public Table<AddBlogPostInfo> AddBlogPostInfoTable {
				get {
					return this.GetTable<AddBlogPostInfo>();
				}
			}
			public Table<AddCategoryInfo> AddCategoryInfoTable {
				get {
					return this.GetTable<AddCategoryInfo>();
				}
			}
			public Table<Group> GroupTable {
				get {
					return this.GetTable<Group>();
				}
			}
			public Table<Introduction> IntroductionTable {
				get {
					return this.GetTable<Introduction>();
				}
			}
			public Table<ContentCategoryRank> ContentCategoryRankTable {
				get {
					return this.GetTable<ContentCategoryRank>();
				}
			}
			public Table<LinkToContent> LinkToContentTable {
				get {
					return this.GetTable<LinkToContent>();
				}
			}
			public Table<EmbeddedContent> EmbeddedContentTable {
				get {
					return this.GetTable<EmbeddedContent>();
				}
			}
			public Table<DynamicContentGroup> DynamicContentGroupTable {
				get {
					return this.GetTable<DynamicContentGroup>();
				}
			}
			public Table<DynamicContent> DynamicContentTable {
				get {
					return this.GetTable<DynamicContent>();
				}
			}
			public Table<AttachedToObject> AttachedToObjectTable {
				get {
					return this.GetTable<AttachedToObject>();
				}
			}
			public Table<Comment> CommentTable {
				get {
					return this.GetTable<Comment>();
				}
			}
			public Table<Selection> SelectionTable {
				get {
					return this.GetTable<Selection>();
				}
			}
			public Table<TextContent> TextContentTable {
				get {
					return this.GetTable<TextContent>();
				}
			}
			public Table<Map> MapTable {
				get {
					return this.GetTable<Map>();
				}
			}
			public Table<MapResult> MapResultTable {
				get {
					return this.GetTable<MapResult>();
				}
			}
			public Table<MapResultsCollection> MapResultsCollectionTable {
				get {
					return this.GetTable<MapResultsCollection>();
				}
			}
			public Table<Video> VideoTable {
				get {
					return this.GetTable<Video>();
				}
			}
			public Table<Image> ImageTable {
				get {
					return this.GetTable<Image>();
				}
			}
			public Table<BinaryFile> BinaryFileTable {
				get {
					return this.GetTable<BinaryFile>();
				}
			}
			public Table<Longitude> LongitudeTable {
				get {
					return this.GetTable<Longitude>();
				}
			}
			public Table<Latitude> LatitudeTable {
				get {
					return this.GetTable<Latitude>();
				}
			}
			public Table<Location> LocationTable {
				get {
					return this.GetTable<Location>();
				}
			}
			public Table<Date> DateTable {
				get {
					return this.GetTable<Date>();
				}
			}
			public Table<CategoryContainer> CategoryContainerTable {
				get {
					return this.GetTable<CategoryContainer>();
				}
			}
			public Table<Category> CategoryTable {
				get {
					return this.GetTable<Category>();
				}
			}
			public Table<UpdateWebContentOperation> UpdateWebContentOperationTable {
				get {
					return this.GetTable<UpdateWebContentOperation>();
				}
			}
			public Table<UpdateWebContentHandlerItem> UpdateWebContentHandlerItemTable {
				get {
					return this.GetTable<UpdateWebContentHandlerItem>();
				}
			}
			public Table<PublicationPackageCollection> PublicationPackageCollectionTable {
				get {
					return this.GetTable<PublicationPackageCollection>();
				}
			}
			public Table<TBAccountCollaborationGroupCollection> TBAccountCollaborationGroupCollectionTable {
				get {
					return this.GetTable<TBAccountCollaborationGroupCollection>();
				}
			}
			public Table<TBLoginInfoCollection> TBLoginInfoCollectionTable {
				get {
					return this.GetTable<TBLoginInfoCollection>();
				}
			}
			public Table<TBEmailCollection> TBEmailCollectionTable {
				get {
					return this.GetTable<TBEmailCollection>();
				}
			}
			public Table<TBCollaboratorRoleCollection> TBCollaboratorRoleCollectionTable {
				get {
					return this.GetTable<TBCollaboratorRoleCollection>();
				}
			}
			public Table<LoginProviderCollection> LoginProviderCollectionTable {
				get {
					return this.GetTable<LoginProviderCollection>();
				}
			}
			public Table<AddressAndLocationCollection> AddressAndLocationCollectionTable {
				get {
					return this.GetTable<AddressAndLocationCollection>();
				}
			}
			public Table<ReferenceCollection> ReferenceCollectionTable {
				get {
					return this.GetTable<ReferenceCollection>();
				}
			}
			public Table<RenderedNodeCollection> RenderedNodeCollectionTable {
				get {
					return this.GetTable<RenderedNodeCollection>();
				}
			}
			public Table<ShortTextCollection> ShortTextCollectionTable {
				get {
					return this.GetTable<ShortTextCollection>();
				}
			}
			public Table<LongTextCollection> LongTextCollectionTable {
				get {
					return this.GetTable<LongTextCollection>();
				}
			}
			public Table<MapMarkerCollection> MapMarkerCollectionTable {
				get {
					return this.GetTable<MapMarkerCollection>();
				}
			}
			public Table<ModeratorCollection> ModeratorCollectionTable {
				get {
					return this.GetTable<ModeratorCollection>();
				}
			}
			public Table<CollaboratorCollection> CollaboratorCollectionTable {
				get {
					return this.GetTable<CollaboratorCollection>();
				}
			}
			public Table<GroupCollection> GroupCollectionTable {
				get {
					return this.GetTable<GroupCollection>();
				}
			}
			public Table<ContentCategoryRankCollection> ContentCategoryRankCollectionTable {
				get {
					return this.GetTable<ContentCategoryRankCollection>();
				}
			}
			public Table<LinkToContentCollection> LinkToContentCollectionTable {
				get {
					return this.GetTable<LinkToContentCollection>();
				}
			}
			public Table<EmbeddedContentCollection> EmbeddedContentCollectionTable {
				get {
					return this.GetTable<EmbeddedContentCollection>();
				}
			}
			public Table<DynamicContentGroupCollection> DynamicContentGroupCollectionTable {
				get {
					return this.GetTable<DynamicContentGroupCollection>();
				}
			}
			public Table<DynamicContentCollection> DynamicContentCollectionTable {
				get {
					return this.GetTable<DynamicContentCollection>();
				}
			}
			public Table<AttachedToObjectCollection> AttachedToObjectCollectionTable {
				get {
					return this.GetTable<AttachedToObjectCollection>();
				}
			}
			public Table<CommentCollection> CommentCollectionTable {
				get {
					return this.GetTable<CommentCollection>();
				}
			}
			public Table<SelectionCollection> SelectionCollectionTable {
				get {
					return this.GetTable<SelectionCollection>();
				}
			}
			public Table<TextContentCollection> TextContentCollectionTable {
				get {
					return this.GetTable<TextContentCollection>();
				}
			}
			public Table<MapCollection> MapCollectionTable {
				get {
					return this.GetTable<MapCollection>();
				}
			}
			public Table<MapResultCollection> MapResultCollectionTable {
				get {
					return this.GetTable<MapResultCollection>();
				}
			}
			public Table<ImageCollection> ImageCollectionTable {
				get {
					return this.GetTable<ImageCollection>();
				}
			}
			public Table<BinaryFileCollection> BinaryFileCollectionTable {
				get {
					return this.GetTable<BinaryFileCollection>();
				}
			}
			public Table<LocationCollection> LocationCollectionTable {
				get {
					return this.GetTable<LocationCollection>();
				}
			}
			public Table<CategoryCollection> CategoryCollectionTable {
				get {
					return this.GetTable<CategoryCollection>();
				}
			}
			public Table<UpdateWebContentHandlerCollection> UpdateWebContentHandlerCollectionTable {
				get {
					return this.GetTable<UpdateWebContentHandlerCollection>();
				}
			}
        }

    [Table(Name = "TBSystem")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBSystem: {ID}")]
	public class TBSystem : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBSystem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBSystem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[InstanceName] TEXT NOT NULL, 
[AdminGroupID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string InstanceName { get; set; }
		// private string _unmodified_InstanceName;

		[Column]
        [ScaffoldColumn(true)]
		public string AdminGroupID { get; set; }
		// private string _unmodified_AdminGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(InstanceName == null)
				InstanceName = string.Empty;
			if(AdminGroupID == null)
				AdminGroupID = string.Empty;
		}
	}
    [Table(Name = "WebPublishInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("WebPublishInfo: {ID}")]
	public class WebPublishInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public WebPublishInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [WebPublishInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PublishType] TEXT NOT NULL, 
[PublishContainer] TEXT NOT NULL, 
[ActivePublicationID] TEXT NULL, 
[PublicationsID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string PublishType { get; set; }
		// private string _unmodified_PublishType;

		[Column]
        [ScaffoldColumn(true)]
		public string PublishContainer { get; set; }
		// private string _unmodified_PublishContainer;
			[Column]
			public string ActivePublicationID { get; set; }
			private EntityRef< PublicationPackage > _ActivePublication;
			[Association(Storage = "_ActivePublication", ThisKey = "ActivePublicationID")]
			public PublicationPackage ActivePublication
			{
				get { return this._ActivePublication.Entity; }
				set { this._ActivePublication.Entity = value; }
			}

			[Column]
			public string PublicationsID { get; set; }
			private EntityRef< PublicationPackageCollection > _Publications;
			[Association(Storage = "_Publications", ThisKey = "PublicationsID")]
			public PublicationPackageCollection Publications
			{
				get { return this._Publications.Entity; }
				set { this._Publications.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PublishType == null)
				PublishType = string.Empty;
			if(PublishContainer == null)
				PublishContainer = string.Empty;
		}
	}
    [Table(Name = "PublicationPackage")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("PublicationPackage: {ID}")]
	public class PublicationPackage : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public PublicationPackage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PublicationPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[PackageName] TEXT NOT NULL, 
[PublicationTime] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string PackageName { get; set; }
		// private string _unmodified_PackageName;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime PublicationTime { get; set; }
		// private DateTime _unmodified_PublicationTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PackageName == null)
				PackageName = string.Empty;
		}
	}
    [Table(Name = "TBRLoginRoot")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBRLoginRoot: {ID}")]
	public class TBRLoginRoot : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBRLoginRoot() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRLoginRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[DomainName] TEXT NOT NULL, 
[AccountID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string DomainName { get; set; }
		// private string _unmodified_DomainName;
			[Column]
			public string AccountID { get; set; }
			private EntityRef< TBAccount > _Account;
			[Association(Storage = "_Account", ThisKey = "AccountID")]
			public TBAccount Account
			{
				get { return this._Account.Entity; }
				set { this._Account.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(DomainName == null)
				DomainName = string.Empty;
		}
	}
    [Table(Name = "TBRAccountRoot")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBRAccountRoot: {ID}")]
	public class TBRAccountRoot : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBRAccountRoot() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRAccountRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountID] TEXT NULL
)";
        }

			[Column]
			public string AccountID { get; set; }
			private EntityRef< TBAccount > _Account;
			[Association(Storage = "_Account", ThisKey = "AccountID")]
			public TBAccount Account
			{
				get { return this._Account.Entity; }
				set { this._Account.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRGroupRoot")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBRGroupRoot: {ID}")]
	public class TBRGroupRoot : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBRGroupRoot() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRGroupRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NULL
)";
        }

			[Column]
			public string GroupID { get; set; }
			private EntityRef< TBCollaboratingGroup > _Group;
			[Association(Storage = "_Group", ThisKey = "GroupID")]
			public TBCollaboratingGroup Group
			{
				get { return this._Group.Entity; }
				set { this._Group.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRLoginGroupRoot")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBRLoginGroupRoot: {ID}")]
	public class TBRLoginGroupRoot : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBRLoginGroupRoot() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRLoginGroupRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Role] TEXT NOT NULL, 
[GroupID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Role == null)
				Role = string.Empty;
			if(GroupID == null)
				GroupID = string.Empty;
		}
	}
    [Table(Name = "TBREmailRoot")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBREmailRoot: {ID}")]
	public class TBREmailRoot : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBREmailRoot() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBREmailRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountID] TEXT NULL
)";
        }

			[Column]
			public string AccountID { get; set; }
			private EntityRef< TBAccount > _Account;
			[Association(Storage = "_Account", ThisKey = "AccountID")]
			public TBAccount Account
			{
				get { return this._Account.Entity; }
				set { this._Account.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBAccount")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBAccount: {ID}")]
	public class TBAccount : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBAccount() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBAccount](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailsID] TEXT NULL, 
[LoginsID] TEXT NULL, 
[GroupRoleCollectionID] TEXT NULL
)";
        }

			[Column]
			public string EmailsID { get; set; }
			private EntityRef< TBEmailCollection > _Emails;
			[Association(Storage = "_Emails", ThisKey = "EmailsID")]
			public TBEmailCollection Emails
			{
				get { return this._Emails.Entity; }
				set { this._Emails.Entity = value; }
			}
			[Column]
			public string LoginsID { get; set; }
			private EntityRef< TBLoginInfoCollection > _Logins;
			[Association(Storage = "_Logins", ThisKey = "LoginsID")]
			public TBLoginInfoCollection Logins
			{
				get { return this._Logins.Entity; }
				set { this._Logins.Entity = value; }
			}
			[Column]
			public string GroupRoleCollectionID { get; set; }
			private EntityRef< TBAccountCollaborationGroupCollection > _GroupRoleCollection;
			[Association(Storage = "_GroupRoleCollection", ThisKey = "GroupRoleCollectionID")]
			public TBAccountCollaborationGroupCollection GroupRoleCollection
			{
				get { return this._GroupRoleCollection.Entity; }
				set { this._GroupRoleCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBAccountCollaborationGroup")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBAccountCollaborationGroup: {ID}")]
	public class TBAccountCollaborationGroup : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBAccountCollaborationGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBAccountCollaborationGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NOT NULL, 
[GroupRole] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupRole { get; set; }
		// private string _unmodified_GroupRole;

		[Column]
        [ScaffoldColumn(true)]
		public string RoleStatus { get; set; }
		// private string _unmodified_RoleStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupID == null)
				GroupID = string.Empty;
			if(GroupRole == null)
				GroupRole = string.Empty;
			if(RoleStatus == null)
				RoleStatus = string.Empty;
		}
	}
    [Table(Name = "TBLoginInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBLoginInfo: {ID}")]
	public class TBLoginInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBLoginInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBLoginInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OpenIDUrl] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string OpenIDUrl { get; set; }
		// private string _unmodified_OpenIDUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OpenIDUrl == null)
				OpenIDUrl = string.Empty;
		}
	}
    [Table(Name = "TBEmail")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBEmail: {ID}")]
	public class TBEmail : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBEmail() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBEmail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailAddress] TEXT NOT NULL, 
[ValidatedAt] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ValidatedAt { get; set; }
		// private DateTime _unmodified_ValidatedAt;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(EmailAddress == null)
				EmailAddress = string.Empty;
		}
	}
    [Table(Name = "TBCollaboratorRole")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBCollaboratorRole: {ID}")]
	public class TBCollaboratorRole : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBCollaboratorRole() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratorRole](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailID] TEXT NULL, 
[Role] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }

			[Column]
			public string EmailID { get; set; }
			private EntityRef< TBEmail > _Email;
			[Association(Storage = "_Email", ThisKey = "EmailID")]
			public TBEmail Email
			{
				get { return this._Email.Entity; }
				set { this._Email.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
        [ScaffoldColumn(true)]
		public string RoleStatus { get; set; }
		// private string _unmodified_RoleStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Role == null)
				Role = string.Empty;
			if(RoleStatus == null)
				RoleStatus = string.Empty;
		}
	}
    [Table(Name = "TBCollaboratingGroup")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBCollaboratingGroup: {ID}")]
	public class TBCollaboratingGroup : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBCollaboratingGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratingGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL, 
[RolesID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;
			[Column]
			public string RolesID { get; set; }
			private EntityRef< TBCollaboratorRoleCollection > _Roles;
			[Association(Storage = "_Roles", ThisKey = "RolesID")]
			public TBCollaboratorRoleCollection Roles
			{
				get { return this._Roles.Entity; }
				set { this._Roles.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
		}
	}
    [Table(Name = "TBEmailValidation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBEmailValidation: {ID}")]
	public class TBEmailValidation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBEmailValidation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBEmailValidation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Email] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[ValidUntil] TEXT NOT NULL, 
[GroupJoinConfirmationID] TEXT NULL, 
[DeviceJoinConfirmationID] TEXT NULL, 
[InformationInputConfirmationID] TEXT NULL, 
[InformationOutputConfirmationID] TEXT NULL, 
[MergeAccountsConfirmationID] TEXT NULL, 
[RedirectUrlAfterValidation] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Email { get; set; }
		// private string _unmodified_Email;

		[Column]
        [ScaffoldColumn(true)]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ValidUntil { get; set; }
		// private DateTime _unmodified_ValidUntil;
			[Column]
			public string GroupJoinConfirmationID { get; set; }
			private EntityRef< TBGroupJoinConfirmation > _GroupJoinConfirmation;
			[Association(Storage = "_GroupJoinConfirmation", ThisKey = "GroupJoinConfirmationID")]
			public TBGroupJoinConfirmation GroupJoinConfirmation
			{
				get { return this._GroupJoinConfirmation.Entity; }
				set { this._GroupJoinConfirmation.Entity = value; }
			}

			[Column]
			public string DeviceJoinConfirmationID { get; set; }
			private EntityRef< TBDeviceJoinConfirmation > _DeviceJoinConfirmation;
			[Association(Storage = "_DeviceJoinConfirmation", ThisKey = "DeviceJoinConfirmationID")]
			public TBDeviceJoinConfirmation DeviceJoinConfirmation
			{
				get { return this._DeviceJoinConfirmation.Entity; }
				set { this._DeviceJoinConfirmation.Entity = value; }
			}

			[Column]
			public string InformationInputConfirmationID { get; set; }
			private EntityRef< TBInformationInputConfirmation > _InformationInputConfirmation;
			[Association(Storage = "_InformationInputConfirmation", ThisKey = "InformationInputConfirmationID")]
			public TBInformationInputConfirmation InformationInputConfirmation
			{
				get { return this._InformationInputConfirmation.Entity; }
				set { this._InformationInputConfirmation.Entity = value; }
			}

			[Column]
			public string InformationOutputConfirmationID { get; set; }
			private EntityRef< TBInformationOutputConfirmation > _InformationOutputConfirmation;
			[Association(Storage = "_InformationOutputConfirmation", ThisKey = "InformationOutputConfirmationID")]
			public TBInformationOutputConfirmation InformationOutputConfirmation
			{
				get { return this._InformationOutputConfirmation.Entity; }
				set { this._InformationOutputConfirmation.Entity = value; }
			}

			[Column]
			public string MergeAccountsConfirmationID { get; set; }
			private EntityRef< TBMergeAccountConfirmation > _MergeAccountsConfirmation;
			[Association(Storage = "_MergeAccountsConfirmation", ThisKey = "MergeAccountsConfirmationID")]
			public TBMergeAccountConfirmation MergeAccountsConfirmation
			{
				get { return this._MergeAccountsConfirmation.Entity; }
				set { this._MergeAccountsConfirmation.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string RedirectUrlAfterValidation { get; set; }
		// private string _unmodified_RedirectUrlAfterValidation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Email == null)
				Email = string.Empty;
			if(AccountID == null)
				AccountID = string.Empty;
			if(RedirectUrlAfterValidation == null)
				RedirectUrlAfterValidation = string.Empty;
		}
	}
    [Table(Name = "TBMergeAccountConfirmation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBMergeAccountConfirmation: {ID}")]
	public class TBMergeAccountConfirmation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBMergeAccountConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBMergeAccountConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountToBeMergedID] TEXT NOT NULL, 
[AccountToMergeToID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string AccountToBeMergedID { get; set; }
		// private string _unmodified_AccountToBeMergedID;

		[Column]
        [ScaffoldColumn(true)]
		public string AccountToMergeToID { get; set; }
		// private string _unmodified_AccountToMergeToID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(AccountToBeMergedID == null)
				AccountToBeMergedID = string.Empty;
			if(AccountToMergeToID == null)
				AccountToMergeToID = string.Empty;
		}
	}
    [Table(Name = "TBGroupJoinConfirmation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBGroupJoinConfirmation: {ID}")]
	public class TBGroupJoinConfirmation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBGroupJoinConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBGroupJoinConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NOT NULL, 
[InvitationMode] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
        [ScaffoldColumn(true)]
		public string InvitationMode { get; set; }
		// private string _unmodified_InvitationMode;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupID == null)
				GroupID = string.Empty;
			if(InvitationMode == null)
				InvitationMode = string.Empty;
		}
	}
    [Table(Name = "TBDeviceJoinConfirmation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBDeviceJoinConfirmation: {ID}")]
	public class TBDeviceJoinConfirmation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBDeviceJoinConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBDeviceJoinConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[DeviceMembershipID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
        [ScaffoldColumn(true)]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
        [ScaffoldColumn(true)]
		public string DeviceMembershipID { get; set; }
		// private string _unmodified_DeviceMembershipID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupID == null)
				GroupID = string.Empty;
			if(AccountID == null)
				AccountID = string.Empty;
			if(DeviceMembershipID == null)
				DeviceMembershipID = string.Empty;
		}
	}
    [Table(Name = "TBInformationInputConfirmation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBInformationInputConfirmation: {ID}")]
	public class TBInformationInputConfirmation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBInformationInputConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBInformationInputConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationInputID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
        [ScaffoldColumn(true)]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
        [ScaffoldColumn(true)]
		public string InformationInputID { get; set; }
		// private string _unmodified_InformationInputID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupID == null)
				GroupID = string.Empty;
			if(AccountID == null)
				AccountID = string.Empty;
			if(InformationInputID == null)
				InformationInputID = string.Empty;
		}
	}
    [Table(Name = "TBInformationOutputConfirmation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBInformationOutputConfirmation: {ID}")]
	public class TBInformationOutputConfirmation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBInformationOutputConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBInformationOutputConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationOutputID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
        [ScaffoldColumn(true)]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
        [ScaffoldColumn(true)]
		public string InformationOutputID { get; set; }
		// private string _unmodified_InformationOutputID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupID == null)
				GroupID = string.Empty;
			if(AccountID == null)
				AccountID = string.Empty;
			if(InformationOutputID == null)
				InformationOutputID = string.Empty;
		}
	}
    [Table(Name = "LoginProvider")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LoginProvider: {ID}")]
	public class LoginProvider : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LoginProvider() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LoginProvider](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProviderName] TEXT NOT NULL, 
[ProviderIconClass] TEXT NOT NULL, 
[ProviderType] TEXT NOT NULL, 
[ProviderUrl] TEXT NOT NULL, 
[ReturnUrl] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ProviderName { get; set; }
		// private string _unmodified_ProviderName;

		[Column]
        [ScaffoldColumn(true)]
		public string ProviderIconClass { get; set; }
		// private string _unmodified_ProviderIconClass;

		[Column]
        [ScaffoldColumn(true)]
		public string ProviderType { get; set; }
		// private string _unmodified_ProviderType;

		[Column]
        [ScaffoldColumn(true)]
		public string ProviderUrl { get; set; }
		// private string _unmodified_ProviderUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string ReturnUrl { get; set; }
		// private string _unmodified_ReturnUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ProviderName == null)
				ProviderName = string.Empty;
			if(ProviderIconClass == null)
				ProviderIconClass = string.Empty;
			if(ProviderType == null)
				ProviderType = string.Empty;
			if(ProviderUrl == null)
				ProviderUrl = string.Empty;
			if(ReturnUrl == null)
				ReturnUrl = string.Empty;
		}
	}
    [Table(Name = "TBPRegisterEmail")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBPRegisterEmail: {ID}")]
	public class TBPRegisterEmail : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBPRegisterEmail() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBPRegisterEmail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(EmailAddress == null)
				EmailAddress = string.Empty;
		}
	}
    [Table(Name = "AccountSummary")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountSummary: {ID}")]
	public class AccountSummary : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupSummaryID] TEXT NULL
)";
        }

			[Column]
			public string GroupSummaryID { get; set; }
			private EntityRef< GroupSummaryContainer > _GroupSummary;
			[Association(Storage = "_GroupSummary", ThisKey = "GroupSummaryID")]
			public GroupSummaryContainer GroupSummary
			{
				get { return this._GroupSummary.Entity; }
				set { this._GroupSummary.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountContainer: {ID}")]
	public class AccountContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountModuleID] TEXT NULL, 
[AccountSummaryID] TEXT NULL
)";
        }

			[Column]
			public string AccountModuleID { get; set; }
			private EntityRef< AccountModule > _AccountModule;
			[Association(Storage = "_AccountModule", ThisKey = "AccountModuleID")]
			public AccountModule AccountModule
			{
				get { return this._AccountModule.Entity; }
				set { this._AccountModule.Entity = value; }
			}

			[Column]
			public string AccountSummaryID { get; set; }
			private EntityRef< AccountSummary > _AccountSummary;
			[Association(Storage = "_AccountSummary", ThisKey = "AccountSummaryID")]
			public AccountSummary AccountSummary
			{
				get { return this._AccountSummary.Entity; }
				set { this._AccountSummary.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountModule")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountModule: {ID}")]
	public class AccountModule : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountModule() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountModule](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProfileID] TEXT NULL, 
[SecurityID] TEXT NULL, 
[RolesID] TEXT NULL, 
[LocationCollectionID] TEXT NULL
)";
        }

			[Column]
			public string ProfileID { get; set; }
			private EntityRef< AccountProfile > _Profile;
			[Association(Storage = "_Profile", ThisKey = "ProfileID")]
			public AccountProfile Profile
			{
				get { return this._Profile.Entity; }
				set { this._Profile.Entity = value; }
			}

			[Column]
			public string SecurityID { get; set; }
			private EntityRef< AccountSecurity > _Security;
			[Association(Storage = "_Security", ThisKey = "SecurityID")]
			public AccountSecurity Security
			{
				get { return this._Security.Entity; }
				set { this._Security.Entity = value; }
			}

			[Column]
			public string RolesID { get; set; }
			private EntityRef< AccountRoles > _Roles;
			[Association(Storage = "_Roles", ThisKey = "RolesID")]
			public AccountRoles Roles
			{
				get { return this._Roles.Entity; }
				set { this._Roles.Entity = value; }
			}

			[Column]
			public string LocationCollectionID { get; set; }
			private EntityRef< AddressAndLocationCollection > _LocationCollection;
			[Association(Storage = "_LocationCollection", ThisKey = "LocationCollectionID")]
			public AddressAndLocationCollection LocationCollection
			{
				get { return this._LocationCollection.Entity; }
				set { this._LocationCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LocationContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LocationContainer: {ID}")]
	public class LocationContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LocationContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LocationContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LocationsID] TEXT NULL
)";
        }

			[Column]
			public string LocationsID { get; set; }
			private EntityRef< AddressAndLocationCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public AddressAndLocationCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddressAndLocation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddressAndLocation: {ID}")]
	public class AddressAndLocation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddressAndLocation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddressAndLocation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ReferenceToInformationID] TEXT NULL, 
[AddressID] TEXT NULL, 
[LocationID] TEXT NULL
)";
        }

			[Column]
			public string ReferenceToInformationID { get; set; }
			private EntityRef< ReferenceToInformation > _ReferenceToInformation;
			[Association(Storage = "_ReferenceToInformation", ThisKey = "ReferenceToInformationID")]
			public ReferenceToInformation ReferenceToInformation
			{
				get { return this._ReferenceToInformation.Entity; }
				set { this._ReferenceToInformation.Entity = value; }
			}

			[Column]
			public string AddressID { get; set; }
			private EntityRef< StreetAddress > _Address;
			[Association(Storage = "_Address", ThisKey = "AddressID")]
			public StreetAddress Address
			{
				get { return this._Address.Entity; }
				set { this._Address.Entity = value; }
			}

			[Column]
			public string LocationID { get; set; }
			private EntityRef< Location > _Location;
			[Association(Storage = "_Location", ThisKey = "LocationID")]
			public Location Location
			{
				get { return this._Location.Entity; }
				set { this._Location.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "StreetAddress")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("StreetAddress: {ID}")]
	public class StreetAddress : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public StreetAddress() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StreetAddress](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Street] TEXT NOT NULL, 
[ZipCode] TEXT NOT NULL, 
[Town] TEXT NOT NULL, 
[Country] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Street { get; set; }
		// private string _unmodified_Street;

		[Column]
        [ScaffoldColumn(true)]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		[Column]
        [ScaffoldColumn(true)]
		public string Town { get; set; }
		// private string _unmodified_Town;

		[Column]
        [ScaffoldColumn(true)]
		public string Country { get; set; }
		// private string _unmodified_Country;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Street == null)
				Street = string.Empty;
			if(ZipCode == null)
				ZipCode = string.Empty;
			if(Town == null)
				Town = string.Empty;
			if(Country == null)
				Country = string.Empty;
		}
	}
    [Table(Name = "AccountProfile")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountProfile: {ID}")]
	public class AccountProfile : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountProfile() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountProfile](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProfileImageID] TEXT NULL, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[AddressID] TEXT NULL, 
[IsSimplifiedAccount] INTEGER NOT NULL, 
[SimplifiedAccountEmail] TEXT NOT NULL, 
[SimplifiedAccountGroupID] TEXT NOT NULL
)";
        }

			[Column]
			public string ProfileImageID { get; set; }
			private EntityRef< Image > _ProfileImage;
			[Association(Storage = "_ProfileImage", ThisKey = "ProfileImageID")]
			public Image ProfileImage
			{
				get { return this._ProfileImage.Entity; }
				set { this._ProfileImage.Entity = value; }
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
			public string AddressID { get; set; }
			private EntityRef< StreetAddress > _Address;
			[Association(Storage = "_Address", ThisKey = "AddressID")]
			public StreetAddress Address
			{
				get { return this._Address.Entity; }
				set { this._Address.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public bool IsSimplifiedAccount { get; set; }
		// private bool _unmodified_IsSimplifiedAccount;

		[Column]
        [ScaffoldColumn(true)]
		public string SimplifiedAccountEmail { get; set; }
		// private string _unmodified_SimplifiedAccountEmail;

		[Column]
        [ScaffoldColumn(true)]
		public string SimplifiedAccountGroupID { get; set; }
		// private string _unmodified_SimplifiedAccountGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(FirstName == null)
				FirstName = string.Empty;
			if(LastName == null)
				LastName = string.Empty;
			if(SimplifiedAccountEmail == null)
				SimplifiedAccountEmail = string.Empty;
			if(SimplifiedAccountGroupID == null)
				SimplifiedAccountGroupID = string.Empty;
		}
	}
    [Table(Name = "AccountSecurity")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountSecurity: {ID}")]
	public class AccountSecurity : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountSecurity() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountSecurity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LoginInfoCollectionID] TEXT NULL, 
[EmailCollectionID] TEXT NULL
)";
        }

			[Column]
			public string LoginInfoCollectionID { get; set; }
			private EntityRef< TBLoginInfoCollection > _LoginInfoCollection;
			[Association(Storage = "_LoginInfoCollection", ThisKey = "LoginInfoCollectionID")]
			public TBLoginInfoCollection LoginInfoCollection
			{
				get { return this._LoginInfoCollection.Entity; }
				set { this._LoginInfoCollection.Entity = value; }
			}
			[Column]
			public string EmailCollectionID { get; set; }
			private EntityRef< TBEmailCollection > _EmailCollection;
			[Association(Storage = "_EmailCollection", ThisKey = "EmailCollectionID")]
			public TBEmailCollection EmailCollection
			{
				get { return this._EmailCollection.Entity; }
				set { this._EmailCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountRoles")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AccountRoles: {ID}")]
	public class AccountRoles : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AccountRoles() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountRoles](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ModeratorInGroupsID] TEXT NULL, 
[MemberInGroupsID] TEXT NULL, 
[OrganizationsImPartOf] TEXT NOT NULL
)";
        }

			[Column]
			public string ModeratorInGroupsID { get; set; }
			private EntityRef< ReferenceCollection > _ModeratorInGroups;
			[Association(Storage = "_ModeratorInGroups", ThisKey = "ModeratorInGroupsID")]
			public ReferenceCollection ModeratorInGroups
			{
				get { return this._ModeratorInGroups.Entity; }
				set { this._ModeratorInGroups.Entity = value; }
			}
			[Column]
			public string MemberInGroupsID { get; set; }
			private EntityRef< ReferenceCollection > _MemberInGroups;
			[Association(Storage = "_MemberInGroups", ThisKey = "MemberInGroupsID")]
			public ReferenceCollection MemberInGroups
			{
				get { return this._MemberInGroups.Entity; }
				set { this._MemberInGroups.Entity = value; }
			}

		[Column]
        [ScaffoldColumn(true)]
		public string OrganizationsImPartOf { get; set; }
		// private string _unmodified_OrganizationsImPartOf;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OrganizationsImPartOf == null)
				OrganizationsImPartOf = string.Empty;
		}
	}
    [Table(Name = "PersonalInfoVisibility")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("PersonalInfoVisibility: {ID}")]
	public class PersonalInfoVisibility : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public PersonalInfoVisibility() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PersonalInfoVisibility](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[NoOne_Network_All] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string NoOne_Network_All { get; set; }
		// private string _unmodified_NoOne_Network_All;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(NoOne_Network_All == null)
				NoOne_Network_All = string.Empty;
		}
	}
    [Table(Name = "ReferenceToInformation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ReferenceToInformation: {ID}")]
	public class ReferenceToInformation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ReferenceToInformation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ReferenceToInformation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL, 
[URL] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string URL { get; set; }
		// private string _unmodified_URL;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(URL == null)
				URL = string.Empty;
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("NodeSummaryContainer: {ID}")]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public NodeSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NodeSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[NodesID] TEXT NULL, 
[NodeSourceTextContentID] TEXT NULL, 
[NodeSourceLinkToContentID] TEXT NULL, 
[NodeSourceEmbeddedContentID] TEXT NULL, 
[NodeSourceImagesID] TEXT NULL, 
[NodeSourceBinaryFilesID] TEXT NULL, 
[NodeSourceCategoriesID] TEXT NULL
)";
        }

			[Column]
			public string NodesID { get; set; }
			private EntityRef< RenderedNodeCollection > _Nodes;
			[Association(Storage = "_Nodes", ThisKey = "NodesID")]
			public RenderedNodeCollection Nodes
			{
				get { return this._Nodes.Entity; }
				set { this._Nodes.Entity = value; }
			}
			[Column]
			public string NodeSourceTextContentID { get; set; }
			private EntityRef< TextContentCollection > _NodeSourceTextContent;
			[Association(Storage = "_NodeSourceTextContent", ThisKey = "NodeSourceTextContentID")]
			public TextContentCollection NodeSourceTextContent
			{
				get { return this._NodeSourceTextContent.Entity; }
				set { this._NodeSourceTextContent.Entity = value; }
			}
			[Column]
			public string NodeSourceLinkToContentID { get; set; }
			private EntityRef< LinkToContentCollection > _NodeSourceLinkToContent;
			[Association(Storage = "_NodeSourceLinkToContent", ThisKey = "NodeSourceLinkToContentID")]
			public LinkToContentCollection NodeSourceLinkToContent
			{
				get { return this._NodeSourceLinkToContent.Entity; }
				set { this._NodeSourceLinkToContent.Entity = value; }
			}
			[Column]
			public string NodeSourceEmbeddedContentID { get; set; }
			private EntityRef< EmbeddedContentCollection > _NodeSourceEmbeddedContent;
			[Association(Storage = "_NodeSourceEmbeddedContent", ThisKey = "NodeSourceEmbeddedContentID")]
			public EmbeddedContentCollection NodeSourceEmbeddedContent
			{
				get { return this._NodeSourceEmbeddedContent.Entity; }
				set { this._NodeSourceEmbeddedContent.Entity = value; }
			}
			[Column]
			public string NodeSourceImagesID { get; set; }
			private EntityRef< ImageCollection > _NodeSourceImages;
			[Association(Storage = "_NodeSourceImages", ThisKey = "NodeSourceImagesID")]
			public ImageCollection NodeSourceImages
			{
				get { return this._NodeSourceImages.Entity; }
				set { this._NodeSourceImages.Entity = value; }
			}
			[Column]
			public string NodeSourceBinaryFilesID { get; set; }
			private EntityRef< BinaryFileCollection > _NodeSourceBinaryFiles;
			[Association(Storage = "_NodeSourceBinaryFiles", ThisKey = "NodeSourceBinaryFilesID")]
			public BinaryFileCollection NodeSourceBinaryFiles
			{
				get { return this._NodeSourceBinaryFiles.Entity; }
				set { this._NodeSourceBinaryFiles.Entity = value; }
			}
			[Column]
			public string NodeSourceCategoriesID { get; set; }
			private EntityRef< CategoryCollection > _NodeSourceCategories;
			[Association(Storage = "_NodeSourceCategories", ThisKey = "NodeSourceCategoriesID")]
			public CategoryCollection NodeSourceCategories
			{
				get { return this._NodeSourceCategories.Entity; }
				set { this._NodeSourceCategories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("RenderedNode: {ID}")]
	public class RenderedNode : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public RenderedNode() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RenderedNode](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OriginalContentID] TEXT NOT NULL, 
[TechnicalSource] TEXT NOT NULL, 
[ImageBaseUrl] TEXT NOT NULL, 
[ImageExt] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[ActualContentUrl] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[TimestampText] TEXT NOT NULL, 
[MainSortableText] TEXT NOT NULL, 
[IsCategoryFilteringNode] INTEGER NOT NULL, 
[CategoryFiltersID] TEXT NULL, 
[CategoryNamesID] TEXT NULL, 
[CategoriesID] TEXT NULL, 
[CategoryIDList] TEXT NOT NULL, 
[AuthorsID] TEXT NULL, 
[LocationsID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string OriginalContentID { get; set; }
		// private string _unmodified_OriginalContentID;

		[Column]
        [ScaffoldColumn(true)]
		public string TechnicalSource { get; set; }
		// private string _unmodified_TechnicalSource;

		[Column]
        [ScaffoldColumn(true)]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string ImageExt { get; set; }
		// private string _unmodified_ImageExt;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string ActualContentUrl { get; set; }
		// private string _unmodified_ActualContentUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string TimestampText { get; set; }
		// private string _unmodified_TimestampText;

		[Column]
        [ScaffoldColumn(true)]
		public string MainSortableText { get; set; }
		// private string _unmodified_MainSortableText;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsCategoryFilteringNode { get; set; }
		// private bool _unmodified_IsCategoryFilteringNode;
			[Column]
			public string CategoryFiltersID { get; set; }
			private EntityRef< ShortTextCollection > _CategoryFilters;
			[Association(Storage = "_CategoryFilters", ThisKey = "CategoryFiltersID")]
			public ShortTextCollection CategoryFilters
			{
				get { return this._CategoryFilters.Entity; }
				set { this._CategoryFilters.Entity = value; }
			}
			[Column]
			public string CategoryNamesID { get; set; }
			private EntityRef< ShortTextCollection > _CategoryNames;
			[Association(Storage = "_CategoryNames", ThisKey = "CategoryNamesID")]
			public ShortTextCollection CategoryNames
			{
				get { return this._CategoryNames.Entity; }
				set { this._CategoryNames.Entity = value; }
			}
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< ShortTextCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public ShortTextCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}

		[Column]
        [ScaffoldColumn(true)]
		public string CategoryIDList { get; set; }
		// private string _unmodified_CategoryIDList;
			[Column]
			public string AuthorsID { get; set; }
			private EntityRef< ShortTextCollection > _Authors;
			[Association(Storage = "_Authors", ThisKey = "AuthorsID")]
			public ShortTextCollection Authors
			{
				get { return this._Authors.Entity; }
				set { this._Authors.Entity = value; }
			}
			[Column]
			public string LocationsID { get; set; }
			private EntityRef< ShortTextCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public ShortTextCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OriginalContentID == null)
				OriginalContentID = string.Empty;
			if(TechnicalSource == null)
				TechnicalSource = string.Empty;
			if(ImageBaseUrl == null)
				ImageBaseUrl = string.Empty;
			if(ImageExt == null)
				ImageExt = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(ActualContentUrl == null)
				ActualContentUrl = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(TimestampText == null)
				TimestampText = string.Empty;
			if(MainSortableText == null)
				MainSortableText = string.Empty;
			if(CategoryIDList == null)
				CategoryIDList = string.Empty;
		}
	}
    [Table(Name = "ShortTextObject")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ShortTextObject: {ID}")]
	public class ShortTextObject : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ShortTextObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ShortTextObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Content] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Content == null)
				Content = string.Empty;
		}
	}
    [Table(Name = "LongTextObject")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LongTextObject: {ID}")]
	public class LongTextObject : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LongTextObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LongTextObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Content] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Content == null)
				Content = string.Empty;
		}
	}
    [Table(Name = "MapMarker")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapMarker: {ID}")]
	public class MapMarker : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapMarker() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapMarker](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconUrl] TEXT NOT NULL, 
[MarkerSource] TEXT NOT NULL, 
[CategoryName] TEXT NOT NULL, 
[LocationText] TEXT NOT NULL, 
[PopupTitle] TEXT NOT NULL, 
[PopupContent] TEXT NOT NULL, 
[LocationID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string IconUrl { get; set; }
		// private string _unmodified_IconUrl;

		[Column]
        [ScaffoldColumn(true)]
		public string MarkerSource { get; set; }
		// private string _unmodified_MarkerSource;

		[Column]
        [ScaffoldColumn(true)]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;

		[Column]
        [ScaffoldColumn(true)]
		public string LocationText { get; set; }
		// private string _unmodified_LocationText;

		[Column]
        [ScaffoldColumn(true)]
		public string PopupTitle { get; set; }
		// private string _unmodified_PopupTitle;

		[Column]
        [ScaffoldColumn(true)]
		public string PopupContent { get; set; }
		// private string _unmodified_PopupContent;
			[Column]
			public string LocationID { get; set; }
			private EntityRef< Location > _Location;
			[Association(Storage = "_Location", ThisKey = "LocationID")]
			public Location Location
			{
				get { return this._Location.Entity; }
				set { this._Location.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(IconUrl == null)
				IconUrl = string.Empty;
			if(MarkerSource == null)
				MarkerSource = string.Empty;
			if(CategoryName == null)
				CategoryName = string.Empty;
			if(LocationText == null)
				LocationText = string.Empty;
			if(PopupTitle == null)
				PopupTitle = string.Empty;
			if(PopupContent == null)
				PopupContent = string.Empty;
		}
	}
    [Table(Name = "Moderator")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Moderator: {ID}")]
	public class Moderator : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Moderator() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Moderator](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ModeratorName] TEXT NOT NULL, 
[ProfileUrl] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ModeratorName { get; set; }
		// private string _unmodified_ModeratorName;

		[Column]
        [ScaffoldColumn(true)]
		public string ProfileUrl { get; set; }
		// private string _unmodified_ProfileUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ModeratorName == null)
				ModeratorName = string.Empty;
			if(ProfileUrl == null)
				ProfileUrl = string.Empty;
		}
	}
    [Table(Name = "Collaborator")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Collaborator: {ID}")]
	public class Collaborator : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Collaborator() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Collaborator](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountID] TEXT NOT NULL, 
[EmailAddress] TEXT NOT NULL, 
[CollaboratorName] TEXT NOT NULL, 
[Role] TEXT NOT NULL, 
[ProfileUrl] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
        [ScaffoldColumn(true)]
		public string CollaboratorName { get; set; }
		// private string _unmodified_CollaboratorName;

		[Column]
        [ScaffoldColumn(true)]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
        [ScaffoldColumn(true)]
		public string ProfileUrl { get; set; }
		// private string _unmodified_ProfileUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(AccountID == null)
				AccountID = string.Empty;
			if(EmailAddress == null)
				EmailAddress = string.Empty;
			if(CollaboratorName == null)
				CollaboratorName = string.Empty;
			if(Role == null)
				Role = string.Empty;
			if(ProfileUrl == null)
				ProfileUrl = string.Empty;
		}
	}
    [Table(Name = "GroupSummaryContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupSummaryContainer: {ID}")]
	public class GroupSummaryContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SummaryBody] TEXT NOT NULL, 
[IntroductionID] TEXT NULL, 
[GroupSummaryIndexID] TEXT NULL, 
[GroupCollectionID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string SummaryBody { get; set; }
		// private string _unmodified_SummaryBody;
			[Column]
			public string IntroductionID { get; set; }
			private EntityRef< Introduction > _Introduction;
			[Association(Storage = "_Introduction", ThisKey = "IntroductionID")]
			public Introduction Introduction
			{
				get { return this._Introduction.Entity; }
				set { this._Introduction.Entity = value; }
			}

			[Column]
			public string GroupSummaryIndexID { get; set; }
			private EntityRef< GroupIndex > _GroupSummaryIndex;
			[Association(Storage = "_GroupSummaryIndex", ThisKey = "GroupSummaryIndexID")]
			public GroupIndex GroupSummaryIndex
			{
				get { return this._GroupSummaryIndex.Entity; }
				set { this._GroupSummaryIndex.Entity = value; }
			}

			[Column]
			public string GroupCollectionID { get; set; }
			private EntityRef< GroupCollection > _GroupCollection;
			[Association(Storage = "_GroupCollection", ThisKey = "GroupCollectionID")]
			public GroupCollection GroupCollection
			{
				get { return this._GroupCollection.Entity; }
				set { this._GroupCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryBody == null)
				SummaryBody = string.Empty;
		}
	}
    [Table(Name = "GroupContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupContainer: {ID}")]
	public class GroupContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupIndexID] TEXT NULL, 
[GroupProfileID] TEXT NULL, 
[CollaboratorsID] TEXT NULL, 
[PendingCollaboratorsID] TEXT NULL, 
[LocationCollectionID] TEXT NULL
)";
        }

			[Column]
			public string GroupIndexID { get; set; }
			private EntityRef< GroupIndex > _GroupIndex;
			[Association(Storage = "_GroupIndex", ThisKey = "GroupIndexID")]
			public GroupIndex GroupIndex
			{
				get { return this._GroupIndex.Entity; }
				set { this._GroupIndex.Entity = value; }
			}

			[Column]
			public string GroupProfileID { get; set; }
			private EntityRef< Group > _GroupProfile;
			[Association(Storage = "_GroupProfile", ThisKey = "GroupProfileID")]
			public Group GroupProfile
			{
				get { return this._GroupProfile.Entity; }
				set { this._GroupProfile.Entity = value; }
			}

			[Column]
			public string CollaboratorsID { get; set; }
			private EntityRef< CollaboratorCollection > _Collaborators;
			[Association(Storage = "_Collaborators", ThisKey = "CollaboratorsID")]
			public CollaboratorCollection Collaborators
			{
				get { return this._Collaborators.Entity; }
				set { this._Collaborators.Entity = value; }
			}
			[Column]
			public string PendingCollaboratorsID { get; set; }
			private EntityRef< CollaboratorCollection > _PendingCollaborators;
			[Association(Storage = "_PendingCollaborators", ThisKey = "PendingCollaboratorsID")]
			public CollaboratorCollection PendingCollaborators
			{
				get { return this._PendingCollaborators.Entity; }
				set { this._PendingCollaborators.Entity = value; }
			}
			[Column]
			public string LocationCollectionID { get; set; }
			private EntityRef< AddressAndLocationCollection > _LocationCollection;
			[Association(Storage = "_LocationCollection", ThisKey = "LocationCollectionID")]
			public AddressAndLocationCollection LocationCollection
			{
				get { return this._LocationCollection.Entity; }
				set { this._LocationCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupIndex")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupIndex: {ID}")]
	public class GroupIndex : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupIndex() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }

			[Column]
			public string IconID { get; set; }
			private EntityRef< Image > _Icon;
			[Association(Storage = "_Icon", ThisKey = "IconID")]
			public Image Icon
			{
				get { return this._Icon.Entity; }
				set { this._Icon.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
        [ScaffoldColumn(true)]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Introduction == null)
				Introduction = string.Empty;
			if(Summary == null)
				Summary = string.Empty;
		}
	}
    [Table(Name = "AddAddressAndLocationInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddAddressAndLocationInfo: {ID}")]
	public class AddAddressAndLocationInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddAddressAndLocationInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddAddressAndLocationInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LocationName] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string LocationName { get; set; }
		// private string _unmodified_LocationName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(LocationName == null)
				LocationName = string.Empty;
		}
	}
    [Table(Name = "AddImageInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddImageInfo: {ID}")]
	public class AddImageInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddImageInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddImageInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ImageTitle] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ImageTitle { get; set; }
		// private string _unmodified_ImageTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageTitle == null)
				ImageTitle = string.Empty;
		}
	}
    [Table(Name = "AddImageGroupInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddImageGroupInfo: {ID}")]
	public class AddImageGroupInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddImageGroupInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddImageGroupInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ImageGroupTitle] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ImageGroupTitle { get; set; }
		// private string _unmodified_ImageGroupTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ImageGroupTitle == null)
				ImageGroupTitle = string.Empty;
		}
	}
    [Table(Name = "AddEmailAddressInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddEmailAddressInfo: {ID}")]
	public class AddEmailAddressInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddEmailAddressInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddEmailAddressInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(EmailAddress == null)
				EmailAddress = string.Empty;
		}
	}
    [Table(Name = "CreateGroupInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CreateGroupInfo: {ID}")]
	public class CreateGroupInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CreateGroupInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CreateGroupInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupName] TEXT NOT NULL
)";
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
    [Table(Name = "AddActivityInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddActivityInfo: {ID}")]
	public class AddActivityInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddActivityInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddActivityInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ActivityName] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ActivityName { get; set; }
		// private string _unmodified_ActivityName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ActivityName == null)
				ActivityName = string.Empty;
		}
	}
    [Table(Name = "AddBlogPostInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddBlogPostInfo: {ID}")]
	public class AddBlogPostInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddBlogPostInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddBlogPostInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
		}
	}
    [Table(Name = "AddCategoryInfo")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddCategoryInfo: {ID}")]
	public class AddCategoryInfo : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddCategoryInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddCategoryInfo](
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
, 
[ReferenceToInformationID] TEXT NULL, 
[ProfileImageID] TEXT NULL, 
[IconImageID] TEXT NULL, 
[GroupName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[OrganizationsAndGroupsLinkedToUs] TEXT NOT NULL, 
[WwwSiteToPublishTo] TEXT NOT NULL, 
[CustomUICollectionID] TEXT NULL, 
[ModeratorsID] TEXT NULL, 
[CategoryCollectionID] TEXT NULL
)";
        }

			[Column]
			public string ReferenceToInformationID { get; set; }
			private EntityRef< ReferenceToInformation > _ReferenceToInformation;
			[Association(Storage = "_ReferenceToInformation", ThisKey = "ReferenceToInformationID")]
			public ReferenceToInformation ReferenceToInformation
			{
				get { return this._ReferenceToInformation.Entity; }
				set { this._ReferenceToInformation.Entity = value; }
			}

			[Column]
			public string ProfileImageID { get; set; }
			private EntityRef< Image > _ProfileImage;
			[Association(Storage = "_ProfileImage", ThisKey = "ProfileImageID")]
			public Image ProfileImage
			{
				get { return this._ProfileImage.Entity; }
				set { this._ProfileImage.Entity = value; }
			}

			[Column]
			public string IconImageID { get; set; }
			private EntityRef< Image > _IconImage;
			[Association(Storage = "_IconImage", ThisKey = "IconImageID")]
			public Image IconImage
			{
				get { return this._IconImage.Entity; }
				set { this._IconImage.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string OrganizationsAndGroupsLinkedToUs { get; set; }
		// private string _unmodified_OrganizationsAndGroupsLinkedToUs;

		[Column]
        [ScaffoldColumn(true)]
		public string WwwSiteToPublishTo { get; set; }
		// private string _unmodified_WwwSiteToPublishTo;
			[Column]
			public string CustomUICollectionID { get; set; }
			private EntityRef< ShortTextCollection > _CustomUICollection;
			[Association(Storage = "_CustomUICollection", ThisKey = "CustomUICollectionID")]
			public ShortTextCollection CustomUICollection
			{
				get { return this._CustomUICollection.Entity; }
				set { this._CustomUICollection.Entity = value; }
			}
			[Column]
			public string ModeratorsID { get; set; }
			private EntityRef< ModeratorCollection > _Moderators;
			[Association(Storage = "_Moderators", ThisKey = "ModeratorsID")]
			public ModeratorCollection Moderators
			{
				get { return this._Moderators.Entity; }
				set { this._Moderators.Entity = value; }
			}
			[Column]
			public string CategoryCollectionID { get; set; }
			private EntityRef< CategoryCollection > _CategoryCollection;
			[Association(Storage = "_CategoryCollection", ThisKey = "CategoryCollectionID")]
			public CategoryCollection CategoryCollection
			{
				get { return this._CategoryCollection.Entity; }
				set { this._CategoryCollection.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(GroupName == null)
				GroupName = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(OrganizationsAndGroupsLinkedToUs == null)
				OrganizationsAndGroupsLinkedToUs = string.Empty;
			if(WwwSiteToPublishTo == null)
				WwwSiteToPublishTo = string.Empty;
		}
	}
    [Table(Name = "Introduction")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Introduction: {ID}")]
	public class Introduction : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Introduction() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Introduction](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL, 
[Body] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Body { get; set; }
		// private string _unmodified_Body;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Body == null)
				Body = string.Empty;
		}
	}
    [Table(Name = "ContentCategoryRank")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ContentCategoryRank: {ID}")]
	public class ContentCategoryRank : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ContentCategoryRank() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentCategoryRank](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ContentID] TEXT NOT NULL, 
[ContentSemanticType] TEXT NOT NULL, 
[CategoryID] TEXT NOT NULL, 
[RankName] TEXT NOT NULL, 
[RankValue] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string ContentID { get; set; }
		// private string _unmodified_ContentID;

		[Column]
        [ScaffoldColumn(true)]
		public string ContentSemanticType { get; set; }
		// private string _unmodified_ContentSemanticType;

		[Column]
        [ScaffoldColumn(true)]
		public string CategoryID { get; set; }
		// private string _unmodified_CategoryID;

		[Column]
        [ScaffoldColumn(true)]
		public string RankName { get; set; }
		// private string _unmodified_RankName;

		[Column]
        [ScaffoldColumn(true)]
		public string RankValue { get; set; }
		// private string _unmodified_RankValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ContentID == null)
				ContentID = string.Empty;
			if(ContentSemanticType == null)
				ContentSemanticType = string.Empty;
			if(CategoryID == null)
				CategoryID = string.Empty;
			if(RankName == null)
				RankName = string.Empty;
			if(RankValue == null)
				RankValue = string.Empty;
		}
	}
    [Table(Name = "LinkToContent")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LinkToContent: {ID}")]
	public class LinkToContent : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LinkToContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LinkToContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[URL] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[ImageDataID] TEXT NULL, 
[LocationsID] TEXT NULL, 
[CategoriesID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string URL { get; set; }
		// private string _unmodified_URL;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;
			[Column]
			public string ImageDataID { get; set; }
			[Column]
			public string LocationsID { get; set; }
			private EntityRef< AddressAndLocationCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public AddressAndLocationCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(URL == null)
				URL = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(Author == null)
				Author = string.Empty;
		}
	}
    [Table(Name = "EmbeddedContent")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("EmbeddedContent: {ID}")]
	public class EmbeddedContent : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public EmbeddedContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [EmbeddedContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IFrameTagContents] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[LocationsID] TEXT NULL, 
[CategoriesID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string IFrameTagContents { get; set; }
		// private string _unmodified_IFrameTagContents;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
			[Column]
			public string LocationsID { get; set; }
			private EntityRef< AddressAndLocationCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public AddressAndLocationCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(IFrameTagContents == null)
				IFrameTagContents = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Author == null)
				Author = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "DynamicContentGroup")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("DynamicContentGroup: {ID}")]
	public class DynamicContentGroup : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public DynamicContentGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContentGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HostName] TEXT NOT NULL, 
[GroupHeader] TEXT NOT NULL, 
[SortValue] TEXT NOT NULL, 
[PageLocation] TEXT NOT NULL, 
[ContentItemNames] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string HostName { get; set; }
		// private string _unmodified_HostName;

		[Column]
        [ScaffoldColumn(true)]
		public string GroupHeader { get; set; }
		// private string _unmodified_GroupHeader;

		[Column]
        [ScaffoldColumn(true)]
		public string SortValue { get; set; }
		// private string _unmodified_SortValue;

		[Column]
        [ScaffoldColumn(true)]
		public string PageLocation { get; set; }
		// private string _unmodified_PageLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string ContentItemNames { get; set; }
		// private string _unmodified_ContentItemNames;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(HostName == null)
				HostName = string.Empty;
			if(GroupHeader == null)
				GroupHeader = string.Empty;
			if(SortValue == null)
				SortValue = string.Empty;
			if(PageLocation == null)
				PageLocation = string.Empty;
			if(ContentItemNames == null)
				ContentItemNames = string.Empty;
		}
	}
    [Table(Name = "DynamicContent")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("DynamicContent: {ID}")]
	public class DynamicContent : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public DynamicContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HostName] TEXT NOT NULL, 
[ContentName] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ElementQuery] TEXT NOT NULL, 
[Content] TEXT NOT NULL, 
[RawContent] TEXT NOT NULL, 
[ImageDataID] TEXT NULL, 
[IsEnabled] INTEGER NOT NULL, 
[ApplyActively] INTEGER NOT NULL, 
[EditType] TEXT NOT NULL, 
[PageLocation] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string HostName { get; set; }
		// private string _unmodified_HostName;

		[Column]
        [ScaffoldColumn(true)]
		public string ContentName { get; set; }
		// private string _unmodified_ContentName;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string ElementQuery { get; set; }
		// private string _unmodified_ElementQuery;

		[Column]
        [ScaffoldColumn(true)]
		public string Content { get; set; }
		// private string _unmodified_Content;

		[Column]
        [ScaffoldColumn(true)]
		public string RawContent { get; set; }
		// private string _unmodified_RawContent;
			[Column]
			public string ImageDataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public bool IsEnabled { get; set; }
		// private bool _unmodified_IsEnabled;

		[Column]
        [ScaffoldColumn(true)]
		public bool ApplyActively { get; set; }
		// private bool _unmodified_ApplyActively;

		[Column]
        [ScaffoldColumn(true)]
		public string EditType { get; set; }
		// private string _unmodified_EditType;

		[Column]
        [ScaffoldColumn(true)]
		public string PageLocation { get; set; }
		// private string _unmodified_PageLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(HostName == null)
				HostName = string.Empty;
			if(ContentName == null)
				ContentName = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(ElementQuery == null)
				ElementQuery = string.Empty;
			if(Content == null)
				Content = string.Empty;
			if(RawContent == null)
				RawContent = string.Empty;
			if(EditType == null)
				EditType = string.Empty;
			if(PageLocation == null)
				PageLocation = string.Empty;
		}
	}
    [Table(Name = "AttachedToObject")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AttachedToObject: {ID}")]
	public class AttachedToObject : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AttachedToObject() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AttachedToObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SourceObjectID] TEXT NOT NULL, 
[SourceObjectName] TEXT NOT NULL, 
[SourceObjectDomain] TEXT NOT NULL, 
[TargetObjectID] TEXT NOT NULL, 
[TargetObjectName] TEXT NOT NULL, 
[TargetObjectDomain] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string SourceObjectID { get; set; }
		// private string _unmodified_SourceObjectID;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceObjectName { get; set; }
		// private string _unmodified_SourceObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceObjectDomain { get; set; }
		// private string _unmodified_SourceObjectDomain;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SourceObjectID == null)
				SourceObjectID = string.Empty;
			if(SourceObjectName == null)
				SourceObjectName = string.Empty;
			if(SourceObjectDomain == null)
				SourceObjectDomain = string.Empty;
			if(TargetObjectID == null)
				TargetObjectID = string.Empty;
			if(TargetObjectName == null)
				TargetObjectName = string.Empty;
			if(TargetObjectDomain == null)
				TargetObjectDomain = string.Empty;
		}
	}
    [Table(Name = "Comment")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Comment: {ID}")]
	public class Comment : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Comment() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Comment](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TargetObjectID] TEXT NOT NULL, 
[TargetObjectName] TEXT NOT NULL, 
[TargetObjectDomain] TEXT NOT NULL, 
[CommentText] TEXT NOT NULL, 
[Created] TEXT NOT NULL, 
[OriginalAuthorName] TEXT NOT NULL, 
[OriginalAuthorEmail] TEXT NOT NULL, 
[OriginalAuthorAccountID] TEXT NOT NULL, 
[LastModified] TEXT NOT NULL, 
[LastAuthorName] TEXT NOT NULL, 
[LastAuthorEmail] TEXT NOT NULL, 
[LastAuthorAccountID] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;

		[Column]
        [ScaffoldColumn(true)]
		public string CommentText { get; set; }
		// private string _unmodified_CommentText;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Created { get; set; }
		// private DateTime _unmodified_Created;

		[Column]
        [ScaffoldColumn(true)]
		public string OriginalAuthorName { get; set; }
		// private string _unmodified_OriginalAuthorName;

		[Column]
        [ScaffoldColumn(true)]
		public string OriginalAuthorEmail { get; set; }
		// private string _unmodified_OriginalAuthorEmail;

		[Column]
        [ScaffoldColumn(true)]
		public string OriginalAuthorAccountID { get; set; }
		// private string _unmodified_OriginalAuthorAccountID;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime LastModified { get; set; }
		// private DateTime _unmodified_LastModified;

		[Column]
        [ScaffoldColumn(true)]
		public string LastAuthorName { get; set; }
		// private string _unmodified_LastAuthorName;

		[Column]
        [ScaffoldColumn(true)]
		public string LastAuthorEmail { get; set; }
		// private string _unmodified_LastAuthorEmail;

		[Column]
        [ScaffoldColumn(true)]
		public string LastAuthorAccountID { get; set; }
		// private string _unmodified_LastAuthorAccountID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TargetObjectID == null)
				TargetObjectID = string.Empty;
			if(TargetObjectName == null)
				TargetObjectName = string.Empty;
			if(TargetObjectDomain == null)
				TargetObjectDomain = string.Empty;
			if(CommentText == null)
				CommentText = string.Empty;
			if(OriginalAuthorName == null)
				OriginalAuthorName = string.Empty;
			if(OriginalAuthorEmail == null)
				OriginalAuthorEmail = string.Empty;
			if(OriginalAuthorAccountID == null)
				OriginalAuthorAccountID = string.Empty;
			if(LastAuthorName == null)
				LastAuthorName = string.Empty;
			if(LastAuthorEmail == null)
				LastAuthorEmail = string.Empty;
			if(LastAuthorAccountID == null)
				LastAuthorAccountID = string.Empty;
		}
	}
    [Table(Name = "Selection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Selection: {ID}")]
	public class Selection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Selection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Selection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TargetObjectID] TEXT NOT NULL, 
[TargetObjectName] TEXT NOT NULL, 
[TargetObjectDomain] TEXT NOT NULL, 
[SelectionCategory] TEXT NOT NULL, 
[TextValue] TEXT NOT NULL, 
[BooleanValue] INTEGER NOT NULL, 
[DoubleValue] REAL NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;

		[Column]
        [ScaffoldColumn(true)]
		public string SelectionCategory { get; set; }
		// private string _unmodified_SelectionCategory;

		[Column]
        [ScaffoldColumn(true)]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;

		[Column]
        [ScaffoldColumn(true)]
		public bool BooleanValue { get; set; }
		// private bool _unmodified_BooleanValue;

		[Column]
        [ScaffoldColumn(true)]
		public double DoubleValue { get; set; }
		// private double _unmodified_DoubleValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TargetObjectID == null)
				TargetObjectID = string.Empty;
			if(TargetObjectName == null)
				TargetObjectName = string.Empty;
			if(TargetObjectDomain == null)
				TargetObjectDomain = string.Empty;
			if(SelectionCategory == null)
				SelectionCategory = string.Empty;
			if(TextValue == null)
				TextValue = string.Empty;
		}
	}
    [Table(Name = "TextContent")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TextContent: {ID}")]
	public class TextContent : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TextContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TextContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ImageDataID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[LocationsID] TEXT NULL, 
[CategoriesID] TEXT NULL, 
[SortOrderNumber] REAL NOT NULL, 
[IFrameSources] TEXT NOT NULL, 
[RawHtmlContent] TEXT NOT NULL
)";
        }

			[Column]
			public string ImageDataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string Body { get; set; }
		// private string _unmodified_Body;
			[Column]
			public string LocationsID { get; set; }
			private EntityRef< AddressAndLocationCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public AddressAndLocationCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}

		[Column]
        [ScaffoldColumn(true)]
		public double SortOrderNumber { get; set; }
		// private double _unmodified_SortOrderNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;

		[Column]
        [ScaffoldColumn(true)]
		public string RawHtmlContent { get; set; }
		// private string _unmodified_RawHtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(SubTitle == null)
				SubTitle = string.Empty;
			if(Author == null)
				Author = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Body == null)
				Body = string.Empty;
			if(IFrameSources == null)
				IFrameSources = string.Empty;
			if(RawHtmlContent == null)
				RawHtmlContent = string.Empty;
		}
	}
    [Table(Name = "Map")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Map: {ID}")]
	public class Map : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Map() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Map](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
		}
	}
    [Table(Name = "MapResult")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapResult: {ID}")]
	public class MapResult : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapResult() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapResult](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LocationID] TEXT NULL
)";
        }

			[Column]
			public string LocationID { get; set; }
			private EntityRef< Location > _Location;
			[Association(Storage = "_Location", ThisKey = "LocationID")]
			public Location Location
			{
				get { return this._Location.Entity; }
				set { this._Location.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapResultsCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapResultsCollection: {ID}")]
	public class MapResultsCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapResultsCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapResultsCollection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ResultByDateID] TEXT NULL, 
[ResultByAuthorID] TEXT NULL, 
[ResultByProximityID] TEXT NULL
)";
        }

			[Column]
			public string ResultByDateID { get; set; }
			private EntityRef< MapResultCollection > _ResultByDate;
			[Association(Storage = "_ResultByDate", ThisKey = "ResultByDateID")]
			public MapResultCollection ResultByDate
			{
				get { return this._ResultByDate.Entity; }
				set { this._ResultByDate.Entity = value; }
			}
			[Column]
			public string ResultByAuthorID { get; set; }
			private EntityRef< MapResultCollection > _ResultByAuthor;
			[Association(Storage = "_ResultByAuthor", ThisKey = "ResultByAuthorID")]
			public MapResultCollection ResultByAuthor
			{
				get { return this._ResultByAuthor.Entity; }
				set { this._ResultByAuthor.Entity = value; }
			}
			[Column]
			public string ResultByProximityID { get; set; }
			private EntityRef< MapResultCollection > _ResultByProximity;
			[Association(Storage = "_ResultByProximity", ThisKey = "ResultByProximityID")]
			public MapResultCollection ResultByProximity
			{
				get { return this._ResultByProximity.Entity; }
				set { this._ResultByProximity.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Video")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Video: {ID}")]
	public class Video : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Video() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Video](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[VideoDataID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL
)";
        }

			[Column]
			public string VideoDataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Caption { get; set; }
		// private string _unmodified_Caption;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Caption == null)
				Caption = string.Empty;
		}
	}
    [Table(Name = "Image")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Image: {ID}")]
	public class Image : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Image() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Image](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ReferenceToInformationID] TEXT NULL, 
[ImageDataID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[LocationsID] TEXT NULL, 
[CategoriesID] TEXT NULL
)";
        }

			[Column]
			public string ReferenceToInformationID { get; set; }
			private EntityRef< ReferenceToInformation > _ReferenceToInformation;
			[Association(Storage = "_ReferenceToInformation", ThisKey = "ReferenceToInformationID")]
			public ReferenceToInformation ReferenceToInformation
			{
				get { return this._ReferenceToInformation.Entity; }
				set { this._ReferenceToInformation.Entity = value; }
			}

			[Column]
			public string ImageDataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Caption { get; set; }
		// private string _unmodified_Caption;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
			[Column]
			public string LocationsID { get; set; }
			private EntityRef< AddressAndLocationCollection > _Locations;
			[Association(Storage = "_Locations", ThisKey = "LocationsID")]
			public AddressAndLocationCollection Locations
			{
				get { return this._Locations.Entity; }
				set { this._Locations.Entity = value; }
			}
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Caption == null)
				Caption = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "BinaryFile")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("BinaryFile: {ID}")]
	public class BinaryFile : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public BinaryFile() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BinaryFile](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OriginalFileName] TEXT NOT NULL, 
[DataID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[CategoriesID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string OriginalFileName { get; set; }
		// private string _unmodified_OriginalFileName;
			[Column]
			public string DataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OriginalFileName == null)
				OriginalFileName = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "Longitude")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Longitude: {ID}")]
	public class Longitude : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Longitude() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Longitude](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TextValue == null)
				TextValue = string.Empty;
		}
	}
    [Table(Name = "Latitude")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Latitude: {ID}")]
	public class Latitude : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Latitude() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Latitude](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TextValue == null)
				TextValue = string.Empty;
		}
	}
    [Table(Name = "Location")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Location: {ID}")]
	public class Location : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Location() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Location](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[LocationName] TEXT NOT NULL, 
[LongitudeID] TEXT NULL, 
[LatitudeID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string LocationName { get; set; }
		// private string _unmodified_LocationName;
			[Column]
			public string LongitudeID { get; set; }
			private EntityRef< Longitude > _Longitude;
			[Association(Storage = "_Longitude", ThisKey = "LongitudeID")]
			public Longitude Longitude
			{
				get { return this._Longitude.Entity; }
				set { this._Longitude.Entity = value; }
			}

			[Column]
			public string LatitudeID { get; set; }
			private EntityRef< Latitude > _Latitude;
			[Association(Storage = "_Latitude", ThisKey = "LatitudeID")]
			public Latitude Latitude
			{
				get { return this._Latitude.Entity; }
				set { this._Latitude.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(LocationName == null)
				LocationName = string.Empty;
		}
	}
    [Table(Name = "Date")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("Date: {ID}")]
	public class Date : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public Date() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Date](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Day] TEXT NOT NULL, 
[Week] TEXT NOT NULL, 
[Month] TEXT NOT NULL, 
[Year] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public DateTime Day { get; set; }
		// private DateTime _unmodified_Day;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Week { get; set; }
		// private DateTime _unmodified_Week;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Month { get; set; }
		// private DateTime _unmodified_Month;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Year { get; set; }
		// private DateTime _unmodified_Year;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CategoryContainer")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CategoryContainer: {ID}")]
	public class CategoryContainer : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CategoryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CategoryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CategoriesID] TEXT NULL
)";
        }

			[Column]
			public string CategoriesID { get; set; }
			private EntityRef< CategoryCollection > _Categories;
			[Association(Storage = "_Categories", ThisKey = "CategoriesID")]
			public CategoryCollection Categories
			{
				get { return this._Categories.Entity; }
				set { this._Categories.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
[ReferenceToInformationID] TEXT NULL, 
[CategoryName] TEXT NOT NULL, 
[ImageDataID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[ParentCategoryID] TEXT NULL
)";
        }

			[Column]
			public string ReferenceToInformationID { get; set; }
			private EntityRef< ReferenceToInformation > _ReferenceToInformation;
			[Association(Storage = "_ReferenceToInformation", ThisKey = "ReferenceToInformationID")]
			public ReferenceToInformation ReferenceToInformation
			{
				get { return this._ReferenceToInformation.Entity; }
				set { this._ReferenceToInformation.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;
			[Column]
			public string ImageDataID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;
			[Column]
			public string ParentCategoryID { get; set; }
			private EntityRef< Category > _ParentCategory;
			[Association(Storage = "_ParentCategory", ThisKey = "ParentCategoryID")]
			public Category ParentCategory
			{
				get { return this._ParentCategory.Entity; }
				set { this._ParentCategory.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CategoryName == null)
				CategoryName = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
		}
	}
    [Table(Name = "UpdateWebContentOperation")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("UpdateWebContentOperation: {ID}")]
	public class UpdateWebContentOperation : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public UpdateWebContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UpdateWebContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SourceContainerName] TEXT NOT NULL, 
[SourcePathRoot] TEXT NOT NULL, 
[TargetContainerName] TEXT NOT NULL, 
[TargetPathRoot] TEXT NOT NULL, 
[RenderWhileSync] INTEGER NOT NULL, 
[HandlersID] TEXT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string SourceContainerName { get; set; }
		// private string _unmodified_SourceContainerName;

		[Column]
        [ScaffoldColumn(true)]
		public string SourcePathRoot { get; set; }
		// private string _unmodified_SourcePathRoot;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetContainerName { get; set; }
		// private string _unmodified_TargetContainerName;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetPathRoot { get; set; }
		// private string _unmodified_TargetPathRoot;

		[Column]
        [ScaffoldColumn(true)]
		public bool RenderWhileSync { get; set; }
		// private bool _unmodified_RenderWhileSync;
			[Column]
			public string HandlersID { get; set; }
			private EntityRef< UpdateWebContentHandlerCollection > _Handlers;
			[Association(Storage = "_Handlers", ThisKey = "HandlersID")]
			public UpdateWebContentHandlerCollection Handlers
			{
				get { return this._Handlers.Entity; }
				set { this._Handlers.Entity = value; }
			}
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SourceContainerName == null)
				SourceContainerName = string.Empty;
			if(SourcePathRoot == null)
				SourcePathRoot = string.Empty;
			if(TargetContainerName == null)
				TargetContainerName = string.Empty;
			if(TargetPathRoot == null)
				TargetPathRoot = string.Empty;
		}
	}
    [Table(Name = "UpdateWebContentHandlerItem")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("UpdateWebContentHandlerItem: {ID}")]
	public class UpdateWebContentHandlerItem : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public UpdateWebContentHandlerItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UpdateWebContentHandlerItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[InformationTypeName] TEXT NOT NULL, 
[OptionName] TEXT NOT NULL
)";
        }


		[Column]
        [ScaffoldColumn(true)]
		public string InformationTypeName { get; set; }
		// private string _unmodified_InformationTypeName;

		[Column]
        [ScaffoldColumn(true)]
		public string OptionName { get; set; }
		// private string _unmodified_OptionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(InformationTypeName == null)
				InformationTypeName = string.Empty;
			if(OptionName == null)
				OptionName = string.Empty;
		}
	}
    [Table(Name = "PublicationPackageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("PublicationPackageCollection: {ID}")]
	public class PublicationPackageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public PublicationPackageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PublicationPackageCollection](
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
    [Table(Name = "TBAccountCollaborationGroupCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBAccountCollaborationGroupCollection: {ID}")]
	public class TBAccountCollaborationGroupCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBAccountCollaborationGroupCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBAccountCollaborationGroupCollection](
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
    [Table(Name = "TBLoginInfoCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBLoginInfoCollection: {ID}")]
	public class TBLoginInfoCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBLoginInfoCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBLoginInfoCollection](
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
    [Table(Name = "TBEmailCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBEmailCollection: {ID}")]
	public class TBEmailCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBEmailCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBEmailCollection](
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
    [Table(Name = "TBCollaboratorRoleCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TBCollaboratorRoleCollection: {ID}")]
	public class TBCollaboratorRoleCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TBCollaboratorRoleCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratorRoleCollection](
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
    [Table(Name = "LoginProviderCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LoginProviderCollection: {ID}")]
	public class LoginProviderCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LoginProviderCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LoginProviderCollection](
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
    [Table(Name = "AddressAndLocationCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AddressAndLocationCollection: {ID}")]
	public class AddressAndLocationCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AddressAndLocationCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddressAndLocationCollection](
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
    [Table(Name = "ReferenceCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ReferenceCollection: {ID}")]
	public class ReferenceCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ReferenceCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ReferenceCollection](
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
    [Table(Name = "RenderedNodeCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("RenderedNodeCollection: {ID}")]
	public class RenderedNodeCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public RenderedNodeCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RenderedNodeCollection](
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
    [Table(Name = "ShortTextCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ShortTextCollection: {ID}")]
	public class ShortTextCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ShortTextCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ShortTextCollection](
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
    [Table(Name = "LongTextCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LongTextCollection: {ID}")]
	public class LongTextCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LongTextCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LongTextCollection](
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
    [Table(Name = "MapMarkerCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapMarkerCollection: {ID}")]
	public class MapMarkerCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapMarkerCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapMarkerCollection](
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
    [Table(Name = "ModeratorCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ModeratorCollection: {ID}")]
	public class ModeratorCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ModeratorCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ModeratorCollection](
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
    [Table(Name = "CollaboratorCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CollaboratorCollection: {ID}")]
	public class CollaboratorCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CollaboratorCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CollaboratorCollection](
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
    [Table(Name = "GroupCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("GroupCollection: {ID}")]
	public class GroupCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public GroupCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupCollection](
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
    [Table(Name = "ContentCategoryRankCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ContentCategoryRankCollection: {ID}")]
	public class ContentCategoryRankCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ContentCategoryRankCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentCategoryRankCollection](
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
    [Table(Name = "LinkToContentCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LinkToContentCollection: {ID}")]
	public class LinkToContentCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LinkToContentCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LinkToContentCollection](
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
    [Table(Name = "EmbeddedContentCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("EmbeddedContentCollection: {ID}")]
	public class EmbeddedContentCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public EmbeddedContentCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [EmbeddedContentCollection](
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
    [Table(Name = "DynamicContentGroupCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("DynamicContentGroupCollection: {ID}")]
	public class DynamicContentGroupCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public DynamicContentGroupCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContentGroupCollection](
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
    [Table(Name = "DynamicContentCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("DynamicContentCollection: {ID}")]
	public class DynamicContentCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public DynamicContentCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContentCollection](
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
    [Table(Name = "AttachedToObjectCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("AttachedToObjectCollection: {ID}")]
	public class AttachedToObjectCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public AttachedToObjectCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AttachedToObjectCollection](
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
    [Table(Name = "CommentCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("CommentCollection: {ID}")]
	public class CommentCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public CommentCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CommentCollection](
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
    [Table(Name = "SelectionCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("SelectionCollection: {ID}")]
	public class SelectionCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public SelectionCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SelectionCollection](
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
    [Table(Name = "TextContentCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("TextContentCollection: {ID}")]
	public class TextContentCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public TextContentCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TextContentCollection](
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
    [Table(Name = "MapCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapCollection: {ID}")]
	public class MapCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapCollection](
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
    [Table(Name = "MapResultCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("MapResultCollection: {ID}")]
	public class MapResultCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public MapResultCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapResultCollection](
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
    [Table(Name = "ImageCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("ImageCollection: {ID}")]
	public class ImageCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public ImageCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageCollection](
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
    [Table(Name = "BinaryFileCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("BinaryFileCollection: {ID}")]
	public class BinaryFileCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public BinaryFileCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BinaryFileCollection](
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
    [Table(Name = "LocationCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("LocationCollection: {ID}")]
	public class LocationCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public LocationCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LocationCollection](
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
    [Table(Name = "UpdateWebContentHandlerCollection")]
	[ScaffoldTable(true)]
	[DebuggerDisplay("UpdateWebContentHandlerCollection: {ID}")]
	public class UpdateWebContentHandlerCollection : ITheBallDataContextStorable
	{

		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		[Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ETag { get; set; }


		public UpdateWebContentHandlerCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UpdateWebContentHandlerCollection](
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
 } 
