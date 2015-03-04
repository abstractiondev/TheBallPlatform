 


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
				tableCreationCommands.Add(TBRegisterContainer.GetCreateTableSQL());
				tableCreationCommands.Add(LoginProvider.GetCreateTableSQL());
				tableCreationCommands.Add(ContactOipContainer.GetCreateTableSQL());
				tableCreationCommands.Add(TBPRegisterEmail.GetCreateTableSQL());
				tableCreationCommands.Add(JavaScriptContainer.GetCreateTableSQL());
				tableCreationCommands.Add(JavascriptContainer.GetCreateTableSQL());
				tableCreationCommands.Add(FooterContainer.GetCreateTableSQL());
				tableCreationCommands.Add(NavigationContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AccountSummary.GetCreateTableSQL());
				tableCreationCommands.Add(AccountContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AccountIndex.GetCreateTableSQL());
				tableCreationCommands.Add(AccountModule.GetCreateTableSQL());
				tableCreationCommands.Add(ImageGroupContainer.GetCreateTableSQL());
				tableCreationCommands.Add(LocationContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AddressAndLocation.GetCreateTableSQL());
				tableCreationCommands.Add(StreetAddress.GetCreateTableSQL());
				tableCreationCommands.Add(AccountContent.GetCreateTableSQL());
				tableCreationCommands.Add(AccountProfile.GetCreateTableSQL());
				tableCreationCommands.Add(AccountSecurity.GetCreateTableSQL());
				tableCreationCommands.Add(AccountRoles.GetCreateTableSQL());
				tableCreationCommands.Add(PersonalInfoVisibility.GetCreateTableSQL());
				tableCreationCommands.Add(GroupedInformation.GetCreateTableSQL());
				tableCreationCommands.Add(ReferenceToInformation.GetCreateTableSQL());
				tableCreationCommands.Add(BlogContainer.GetCreateTableSQL());
				tableCreationCommands.Add(RecentBlogSummary.GetCreateTableSQL());
				tableCreationCommands.Add(NodeSummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(RenderedNode.GetCreateTableSQL());
				tableCreationCommands.Add(ShortTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(LongTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(MapContainer.GetCreateTableSQL());
				tableCreationCommands.Add(MapMarker.GetCreateTableSQL());
				tableCreationCommands.Add(CalendarContainer.GetCreateTableSQL());
				tableCreationCommands.Add(AboutContainer.GetCreateTableSQL());
				tableCreationCommands.Add(OBSAccountContainer.GetCreateTableSQL());
				tableCreationCommands.Add(ProjectContainer.GetCreateTableSQL());
				tableCreationCommands.Add(CourseContainer.GetCreateTableSQL());
				tableCreationCommands.Add(ContainerHeader.GetCreateTableSQL());
				tableCreationCommands.Add(ActivitySummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(ActivityIndex.GetCreateTableSQL());
				tableCreationCommands.Add(ActivityContainer.GetCreateTableSQL());
				tableCreationCommands.Add(Activity.GetCreateTableSQL());
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
				tableCreationCommands.Add(Blog.GetCreateTableSQL());
				tableCreationCommands.Add(BlogIndexGroup.GetCreateTableSQL());
				tableCreationCommands.Add(CalendarIndex.GetCreateTableSQL());
				tableCreationCommands.Add(Filter.GetCreateTableSQL());
				tableCreationCommands.Add(Calendar.GetCreateTableSQL());
				tableCreationCommands.Add(Map.GetCreateTableSQL());
				tableCreationCommands.Add(MapIndexCollection.GetCreateTableSQL());
				tableCreationCommands.Add(MapResult.GetCreateTableSQL());
				tableCreationCommands.Add(MapResultsCollection.GetCreateTableSQL());
				tableCreationCommands.Add(Video.GetCreateTableSQL());
				tableCreationCommands.Add(Image.GetCreateTableSQL());
				tableCreationCommands.Add(BinaryFile.GetCreateTableSQL());
				tableCreationCommands.Add(ImageGroup.GetCreateTableSQL());
				tableCreationCommands.Add(VideoGroup.GetCreateTableSQL());
				tableCreationCommands.Add(Tooltip.GetCreateTableSQL());
				tableCreationCommands.Add(SocialPanel.GetCreateTableSQL());
				tableCreationCommands.Add(Longitude.GetCreateTableSQL());
				tableCreationCommands.Add(Latitude.GetCreateTableSQL());
				tableCreationCommands.Add(Location.GetCreateTableSQL());
				tableCreationCommands.Add(Date.GetCreateTableSQL());
				tableCreationCommands.Add(Sex.GetCreateTableSQL());
				tableCreationCommands.Add(OBSAddress.GetCreateTableSQL());
				tableCreationCommands.Add(Identity.GetCreateTableSQL());
				tableCreationCommands.Add(ImageVideoSoundVectorRaw.GetCreateTableSQL());
				tableCreationCommands.Add(CategoryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(Category.GetCreateTableSQL());
				tableCreationCommands.Add(Subscription.GetCreateTableSQL());
				tableCreationCommands.Add(QueueEnvelope.GetCreateTableSQL());
				tableCreationCommands.Add(OperationRequest.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionChainRequestMessage.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionChainRequestContent.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriptionTarget.GetCreateTableSQL());
				tableCreationCommands.Add(DeleteEntireOwnerOperation.GetCreateTableSQL());
				tableCreationCommands.Add(DeleteOwnerContentOperation.GetCreateTableSQL());
				tableCreationCommands.Add(SystemError.GetCreateTableSQL());
				tableCreationCommands.Add(SystemErrorItem.GetCreateTableSQL());
				tableCreationCommands.Add(InformationSource.GetCreateTableSQL());
				tableCreationCommands.Add(RefreshDefaultViewsOperation.GetCreateTableSQL());
				tableCreationCommands.Add(UpdateWebContentOperation.GetCreateTableSQL());
				tableCreationCommands.Add(UpdateWebContentHandlerItem.GetCreateTableSQL());
				tableCreationCommands.Add(PublishWebContentOperation.GetCreateTableSQL());
				tableCreationCommands.Add(SubscriberInput.GetCreateTableSQL());
				tableCreationCommands.Add(Monitor.GetCreateTableSQL());
				tableCreationCommands.Add(IconTitleDescription.GetCreateTableSQL());
				tableCreationCommands.Add(AboutAGIApplications.GetCreateTableSQL());
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
		        if (updateData.ObjectType == "TBSystem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBSystemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InstanceName = serializedObject.InstanceName;
		            existingObject.AdminGroupID = serializedObject.AdminGroupID;
		            return;
		        } 
		        if (updateData.ObjectType == "WebPublishInfo")
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
		            return;
		        } 
		        if (updateData.ObjectType == "PublicationPackage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PublicationPackageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.PackageName = serializedObject.PackageName;
		            existingObject.PublicationTime = serializedObject.PublicationTime;
		            return;
		        } 
		        if (updateData.ObjectType == "TBRLoginRoot")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBRAccountRoot")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBRGroupRoot")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBRLoginGroupRoot")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRLoginGroupRootTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Role = serializedObject.Role;
		            existingObject.GroupID = serializedObject.GroupID;
		            return;
		        } 
		        if (updateData.ObjectType == "TBREmailRoot")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBAccount")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBAccountTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "TBAccountCollaborationGroup")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBLoginInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBLoginInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OpenIDUrl = serializedObject.OpenIDUrl;
		            return;
		        } 
		        if (updateData.ObjectType == "TBEmail")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            existingObject.ValidatedAt = serializedObject.ValidatedAt;
		            return;
		        } 
		        if (updateData.ObjectType == "TBCollaboratorRole")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBCollaboratingGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBCollaboratingGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "TBEmailValidation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBMergeAccountConfirmation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBMergeAccountConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            existingObject.AccountToMergeToID = serializedObject.AccountToMergeToID;
		            return;
		        } 
		        if (updateData.ObjectType == "TBGroupJoinConfirmation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBGroupJoinConfirmationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupID = serializedObject.GroupID;
		            existingObject.InvitationMode = serializedObject.InvitationMode;
		            return;
		        } 
		        if (updateData.ObjectType == "TBDeviceJoinConfirmation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBInformationInputConfirmation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBInformationOutputConfirmation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TBRegisterContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBRegisterContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRegisterContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
		            existingObject.ReturnUrl = serializedObject.ReturnUrl;
		            return;
		        } 
		        if (updateData.ObjectType == "LoginProvider")
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
		            return;
		        } 
		        if (updateData.ObjectType == "ContactOipContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ContactOipContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContactOipContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OIPModeratorGroupID = serializedObject.OIPModeratorGroupID;
		            return;
		        } 
		        if (updateData.ObjectType == "TBPRegisterEmail")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBPRegisterEmailTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            return;
		        } 
		        if (updateData.ObjectType == "JavaScriptContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.JavaScriptContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = JavaScriptContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HtmlContent = serializedObject.HtmlContent;
		            return;
		        } 
		        if (updateData.ObjectType == "JavascriptContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.JavascriptContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = JavascriptContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HtmlContent = serializedObject.HtmlContent;
		            return;
		        } 
		        if (updateData.ObjectType == "FooterContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.FooterContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = FooterContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.HtmlContent = serializedObject.HtmlContent;
		            return;
		        } 
		        if (updateData.ObjectType == "NavigationContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.NavigationContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NavigationContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Dummy = serializedObject.Dummy;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
					if(serializedObject.ActivitySummary != null)
						existingObject.ActivitySummaryID = serializedObject.ActivitySummary.ID;
					else
						existingObject.ActivitySummaryID = null;
					if(serializedObject.GroupSummary != null)
						existingObject.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						existingObject.GroupSummaryID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
					if(serializedObject.AccountIndex != null)
						existingObject.AccountIndexID = serializedObject.AccountIndex.ID;
					else
						existingObject.AccountIndexID = null;
					if(serializedObject.AccountModule != null)
						existingObject.AccountModuleID = serializedObject.AccountModule.ID;
					else
						existingObject.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						existingObject.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						existingObject.AccountSummaryID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountIndexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountModule")
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
		            return;
		        } 
		        if (updateData.ObjectType == "ImageGroupContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ImageGroupContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ImageGroupContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "LocationContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LocationContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "AddressAndLocation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "StreetAddress")
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
		            return;
		        } 
		        if (updateData.ObjectType == "AccountContent")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountContent.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Dummy = serializedObject.Dummy;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountProfile")
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
		            return;
		        } 
		        if (updateData.ObjectType == "AccountSecurity")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountSecurityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountRoles")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountRolesTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
		            return;
		        } 
		        if (updateData.ObjectType == "PersonalInfoVisibility")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PersonalInfoVisibilityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.NoOne_Network_All = serializedObject.NoOne_Network_All;
		            return;
		        } 
		        if (updateData.ObjectType == "GroupedInformation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.GroupedInformation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupedInformationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            return;
		        } 
		        if (updateData.ObjectType == "ReferenceToInformation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ReferenceToInformationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.URL = serializedObject.URL;
		            return;
		        } 
		        if (updateData.ObjectType == "BlogContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.BlogContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = BlogContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
					if(serializedObject.FeaturedBlog != null)
						existingObject.FeaturedBlogID = serializedObject.FeaturedBlog.ID;
					else
						existingObject.FeaturedBlogID = null;
					if(serializedObject.RecentBlogSummary != null)
						existingObject.RecentBlogSummaryID = serializedObject.RecentBlogSummary.ID;
					else
						existingObject.RecentBlogSummaryID = null;
					if(serializedObject.BlogIndexGroup != null)
						existingObject.BlogIndexGroupID = serializedObject.BlogIndexGroup.ID;
					else
						existingObject.BlogIndexGroupID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "RecentBlogSummary")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.RecentBlogSummary.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RecentBlogSummaryTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "NodeSummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = NodeSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "RenderedNode")
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
		            existingObject.CategoryIDList = serializedObject.CategoryIDList;
		            return;
		        } 
		        if (updateData.ObjectType == "ShortTextObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ShortTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            return;
		        } 
		        if (updateData.ObjectType == "LongTextObject")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LongTextObjectTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Content = serializedObject.Content;
		            return;
		        } 
		        if (updateData.ObjectType == "MapContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.MapContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
					if(serializedObject.MapFeatured != null)
						existingObject.MapFeaturedID = serializedObject.MapFeatured.ID;
					else
						existingObject.MapFeaturedID = null;
					if(serializedObject.MapIndexCollection != null)
						existingObject.MapIndexCollectionID = serializedObject.MapIndexCollection.ID;
					else
						existingObject.MapIndexCollectionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "MapMarker")
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
		            return;
		        } 
		        if (updateData.ObjectType == "CalendarContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.CalendarContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CalendarContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.CalendarContainerHeader != null)
						existingObject.CalendarContainerHeaderID = serializedObject.CalendarContainerHeader.ID;
					else
						existingObject.CalendarContainerHeaderID = null;
					if(serializedObject.CalendarFeatured != null)
						existingObject.CalendarFeaturedID = serializedObject.CalendarFeatured.ID;
					else
						existingObject.CalendarFeaturedID = null;
					if(serializedObject.CalendarIndexCollection != null)
						existingObject.CalendarIndexCollectionID = serializedObject.CalendarIndexCollection.ID;
					else
						existingObject.CalendarIndexCollectionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "AboutContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AboutContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AboutContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.MainImage != null)
						existingObject.MainImageID = serializedObject.MainImage.ID;
					else
						existingObject.MainImageID = null;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Body = serializedObject.Body;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
					if(serializedObject.ImageGroup != null)
						existingObject.ImageGroupID = serializedObject.ImageGroup.ID;
					else
						existingObject.ImageGroupID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "OBSAccountContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.OBSAccountContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OBSAccountContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.AccountContainerHeader != null)
						existingObject.AccountContainerHeaderID = serializedObject.AccountContainerHeader.ID;
					else
						existingObject.AccountContainerHeaderID = null;
					if(serializedObject.AccountFeatured != null)
						existingObject.AccountFeaturedID = serializedObject.AccountFeatured.ID;
					else
						existingObject.AccountFeaturedID = null;
					if(serializedObject.AccountIndexCollection != null)
						existingObject.AccountIndexCollectionID = serializedObject.AccountIndexCollection.ID;
					else
						existingObject.AccountIndexCollectionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "ProjectContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ProjectContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ProjectContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ProjectContainerHeader != null)
						existingObject.ProjectContainerHeaderID = serializedObject.ProjectContainerHeader.ID;
					else
						existingObject.ProjectContainerHeaderID = null;
					if(serializedObject.ProjectFeatured != null)
						existingObject.ProjectFeaturedID = serializedObject.ProjectFeatured.ID;
					else
						existingObject.ProjectFeaturedID = null;
					if(serializedObject.ProjectIndexCollection != null)
						existingObject.ProjectIndexCollectionID = serializedObject.ProjectIndexCollection.ID;
					else
						existingObject.ProjectIndexCollectionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "CourseContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.CourseContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CourseContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.CourseContainerHeader != null)
						existingObject.CourseContainerHeaderID = serializedObject.CourseContainerHeader.ID;
					else
						existingObject.CourseContainerHeaderID = null;
					if(serializedObject.CourseFeatured != null)
						existingObject.CourseFeaturedID = serializedObject.CourseFeatured.ID;
					else
						existingObject.CourseFeaturedID = null;
					if(serializedObject.CourseIndexCollection != null)
						existingObject.CourseIndexCollectionID = serializedObject.CourseIndexCollection.ID;
					else
						existingObject.CourseIndexCollectionID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "ContainerHeader")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ContainerHeader.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContainerHeaderTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.SubTitle = serializedObject.SubTitle;
		            return;
		        } 
		        if (updateData.ObjectType == "ActivitySummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ActivitySummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivitySummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
		            existingObject.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
					if(serializedObject.ActivityIndex != null)
						existingObject.ActivityIndexID = serializedObject.ActivityIndex.ID;
					else
						existingObject.ActivityIndexID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "ActivityIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ActivityIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivityIndexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "ActivityContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ActivityContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivityContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
					if(serializedObject.ActivityIndex != null)
						existingObject.ActivityIndexID = serializedObject.ActivityIndex.ID;
					else
						existingObject.ActivityIndexID = null;
					if(serializedObject.ActivityModule != null)
						existingObject.ActivityModuleID = serializedObject.ActivityModule.ID;
					else
						existingObject.ActivityModuleID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "Activity")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Activity.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivityTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.ActivityName = serializedObject.ActivityName;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
		            existingObject.ContactPerson = serializedObject.ContactPerson;
		            existingObject.StartingTime = serializedObject.StartingTime;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Description = serializedObject.Description;
		            existingObject.IFrameSources = serializedObject.IFrameSources;
		            return;
		        } 
		        if (updateData.ObjectType == "Moderator")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ModeratorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ModeratorName = serializedObject.ModeratorName;
		            existingObject.ProfileUrl = serializedObject.ProfileUrl;
		            return;
		        } 
		        if (updateData.ObjectType == "Collaborator")
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
		            return;
		        } 
		        if (updateData.ObjectType == "GroupSummaryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupSummaryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
		            existingObject.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						existingObject.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						existingObject.GroupSummaryIndexID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "GroupContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Header != null)
						existingObject.HeaderID = serializedObject.Header.ID;
					else
						existingObject.HeaderID = null;
					if(serializedObject.GroupIndex != null)
						existingObject.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						existingObject.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						existingObject.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						existingObject.GroupProfileID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "GroupIndex")
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
		            return;
		        } 
		        if (updateData.ObjectType == "AddAddressAndLocationInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddAddressAndLocationInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.LocationName = serializedObject.LocationName;
		            return;
		        } 
		        if (updateData.ObjectType == "AddImageInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddImageInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageTitle = serializedObject.ImageTitle;
		            return;
		        } 
		        if (updateData.ObjectType == "AddImageGroupInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddImageGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ImageGroupTitle = serializedObject.ImageGroupTitle;
		            return;
		        } 
		        if (updateData.ObjectType == "AddEmailAddressInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddEmailAddressInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.EmailAddress = serializedObject.EmailAddress;
		            return;
		        } 
		        if (updateData.ObjectType == "CreateGroupInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CreateGroupInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.GroupName = serializedObject.GroupName;
		            return;
		        } 
		        if (updateData.ObjectType == "AddActivityInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddActivityInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ActivityName = serializedObject.ActivityName;
		            return;
		        } 
		        if (updateData.ObjectType == "AddBlogPostInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddBlogPostInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "AddCategoryInfo")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AddCategoryInfoTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            return;
		        } 
		        if (updateData.ObjectType == "Group")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Introduction")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = IntroductionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Body = serializedObject.Body;
		            return;
		        } 
		        if (updateData.ObjectType == "ContentCategoryRank")
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
		            return;
		        } 
		        if (updateData.ObjectType == "LinkToContent")
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
		            return;
		        } 
		        if (updateData.ObjectType == "EmbeddedContent")
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
		            return;
		        } 
		        if (updateData.ObjectType == "DynamicContentGroup")
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
		            return;
		        } 
		        if (updateData.ObjectType == "DynamicContent")
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
		            return;
		        } 
		        if (updateData.ObjectType == "AttachedToObject")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Comment")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Selection")
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
		            return;
		        } 
		        if (updateData.ObjectType == "TextContent")
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
		            existingObject.SortOrderNumber = serializedObject.SortOrderNumber;
		            existingObject.IFrameSources = serializedObject.IFrameSources;
		            existingObject.RawHtmlContent = serializedObject.RawHtmlContent;
		            return;
		        } 
		        if (updateData.ObjectType == "Blog")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Blog.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = BlogTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.Title = serializedObject.Title;
		            existingObject.SubTitle = serializedObject.SubTitle;
					if(serializedObject.Introduction != null)
						existingObject.IntroductionID = serializedObject.Introduction.ID;
					else
						existingObject.IntroductionID = null;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
					if(serializedObject.FeaturedImage != null)
						existingObject.FeaturedImageID = serializedObject.FeaturedImage.ID;
					else
						existingObject.FeaturedImageID = null;
					if(serializedObject.VideoGroup != null)
						existingObject.VideoGroupID = serializedObject.VideoGroup.ID;
					else
						existingObject.VideoGroupID = null;
		            existingObject.Body = serializedObject.Body;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.IFrameSources = serializedObject.IFrameSources;
		            return;
		        } 
		        if (updateData.ObjectType == "BlogIndexGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.BlogIndexGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = BlogIndexGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "CalendarIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.CalendarIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CalendarIndexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.Icon != null)
						existingObject.IconID = serializedObject.Icon.ID;
					else
						existingObject.IconID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "Filter")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Filter.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = FilterTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "Calendar")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Calendar.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CalendarTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "Map")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "MapIndexCollection")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.MapIndexCollection.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapIndexCollectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "MapResult")
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
		            return;
		        } 
		        if (updateData.ObjectType == "MapResultsCollection")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapResultsCollectionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "Video")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Image")
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
		            return;
		        } 
		        if (updateData.ObjectType == "BinaryFile")
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
		            return;
		        } 
		        if (updateData.ObjectType == "ImageGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ImageGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ImageGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.ReferenceToInformation != null)
						existingObject.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						existingObject.ReferenceToInformationID = null;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
					if(serializedObject.FeaturedImage != null)
						existingObject.FeaturedImageID = serializedObject.FeaturedImage.ID;
					else
						existingObject.FeaturedImageID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "VideoGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.VideoGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = VideoGroupTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "Tooltip")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Tooltip.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TooltipTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TooltipText = serializedObject.TooltipText;
		            return;
		        } 
		        if (updateData.ObjectType == "SocialPanel")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SocialPanel.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SocialPanelTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.SocialFilter != null)
						existingObject.SocialFilterID = serializedObject.SocialFilter.ID;
					else
						existingObject.SocialFilterID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "Longitude")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LongitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            return;
		        } 
		        if (updateData.ObjectType == "Latitude")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LatitudeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TextValue = serializedObject.TextValue;
		            return;
		        } 
		        if (updateData.ObjectType == "Location")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Date")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Sex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Sex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SexTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SexText = serializedObject.SexText;
		            return;
		        } 
		        if (updateData.ObjectType == "OBSAddress")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.OBSAddress.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OBSAddressTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.StreetName = serializedObject.StreetName;
		            existingObject.BuildingNumber = serializedObject.BuildingNumber;
		            existingObject.PostOfficeBox = serializedObject.PostOfficeBox;
		            existingObject.PostalCode = serializedObject.PostalCode;
		            existingObject.Municipality = serializedObject.Municipality;
		            existingObject.Region = serializedObject.Region;
		            existingObject.Province = serializedObject.Province;
		            existingObject.state = serializedObject.state;
		            existingObject.Country = serializedObject.Country;
		            existingObject.Continent = serializedObject.Continent;
		            return;
		        } 
		        if (updateData.ObjectType == "Identity")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Identity.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = IdentityTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
		            existingObject.Initials = serializedObject.Initials;
					if(serializedObject.Sex != null)
						existingObject.SexID = serializedObject.Sex.ID;
					else
						existingObject.SexID = null;
					if(serializedObject.Birthday != null)
						existingObject.BirthdayID = serializedObject.Birthday.ID;
					else
						existingObject.BirthdayID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "ImageVideoSoundVectorRaw")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ImageVideoSoundVectorRaw.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ImageVideoSoundVectorRawTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Image = serializedObject.Image;
		            existingObject.Video = serializedObject.Video;
		            existingObject.Sound = serializedObject.Sound;
		            existingObject.Vector = serializedObject.Vector;
		            existingObject.Raw = serializedObject.Raw;
		            return;
		        } 
		        if (updateData.ObjectType == "CategoryContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryContainerTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            return;
		        } 
		        if (updateData.ObjectType == "Category")
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
		            return;
		        } 
		        if (updateData.ObjectType == "Subscription")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Subscription.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Priority = serializedObject.Priority;
		            existingObject.TargetRelativeLocation = serializedObject.TargetRelativeLocation;
		            existingObject.TargetInformationObjectType = serializedObject.TargetInformationObjectType;
		            existingObject.SubscriberRelativeLocation = serializedObject.SubscriberRelativeLocation;
		            existingObject.SubscriberInformationObjectType = serializedObject.SubscriberInformationObjectType;
		            existingObject.SubscriptionType = serializedObject.SubscriptionType;
		            return;
		        } 
		        if (updateData.ObjectType == "QueueEnvelope")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.QueueEnvelope.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = QueueEnvelopeTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ActiveContainerName = serializedObject.ActiveContainerName;
		            existingObject.OwnerPrefix = serializedObject.OwnerPrefix;
		            existingObject.CurrentRetryCount = serializedObject.CurrentRetryCount;
					if(serializedObject.SingleOperation != null)
						existingObject.SingleOperationID = serializedObject.SingleOperation.ID;
					else
						existingObject.SingleOperationID = null;
					if(serializedObject.ErrorContent != null)
						existingObject.ErrorContentID = serializedObject.ErrorContent.ID;
					else
						existingObject.ErrorContentID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "OperationRequest")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.OperationRequest.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OperationRequestTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.SubscriberNotification != null)
						existingObject.SubscriberNotificationID = serializedObject.SubscriberNotification.ID;
					else
						existingObject.SubscriberNotificationID = null;
					if(serializedObject.SubscriptionChainRequest != null)
						existingObject.SubscriptionChainRequestID = serializedObject.SubscriptionChainRequest.ID;
					else
						existingObject.SubscriptionChainRequestID = null;
					if(serializedObject.UpdateWebContentOperation != null)
						existingObject.UpdateWebContentOperationID = serializedObject.UpdateWebContentOperation.ID;
					else
						existingObject.UpdateWebContentOperationID = null;
					if(serializedObject.RefreshDefaultViewsOperation != null)
						existingObject.RefreshDefaultViewsOperationID = serializedObject.RefreshDefaultViewsOperation.ID;
					else
						existingObject.RefreshDefaultViewsOperationID = null;
					if(serializedObject.DeleteEntireOwner != null)
						existingObject.DeleteEntireOwnerID = serializedObject.DeleteEntireOwner.ID;
					else
						existingObject.DeleteEntireOwnerID = null;
					if(serializedObject.DeleteOwnerContent != null)
						existingObject.DeleteOwnerContentID = serializedObject.DeleteOwnerContent.ID;
					else
						existingObject.DeleteOwnerContentID = null;
					if(serializedObject.PublishWebContent != null)
						existingObject.PublishWebContentID = serializedObject.PublishWebContent.ID;
					else
						existingObject.PublishWebContentID = null;
		            existingObject.ProcessIDToExecute = serializedObject.ProcessIDToExecute;
		            return;
		        } 
		        if (updateData.ObjectType == "SubscriptionChainRequestMessage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SubscriptionChainRequestMessage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionChainRequestMessageTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ContentItemID = serializedObject.ContentItemID;
		            return;
		        } 
		        if (updateData.ObjectType == "SubscriptionChainRequestContent")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SubscriptionChainRequestContent.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionChainRequestContentTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SubmitTime = serializedObject.SubmitTime;
		            existingObject.ProcessingStartTime = serializedObject.ProcessingStartTime;
		            existingObject.ProcessingEndTimeInformationObjects = serializedObject.ProcessingEndTimeInformationObjects;
		            existingObject.ProcessingEndTimeWebTemplatesRendering = serializedObject.ProcessingEndTimeWebTemplatesRendering;
		            existingObject.ProcessingEndTime = serializedObject.ProcessingEndTime;
		            return;
		        } 
		        if (updateData.ObjectType == "SubscriptionTarget")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SubscriptionTarget.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionTargetTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.BlobLocation = serializedObject.BlobLocation;
		            return;
		        } 
		        if (updateData.ObjectType == "DeleteEntireOwnerOperation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.DeleteEntireOwnerOperation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DeleteEntireOwnerOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ContainerName = serializedObject.ContainerName;
		            existingObject.LocationPrefix = serializedObject.LocationPrefix;
		            return;
		        } 
		        if (updateData.ObjectType == "DeleteOwnerContentOperation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.DeleteOwnerContentOperation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DeleteOwnerContentOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ContainerName = serializedObject.ContainerName;
		            existingObject.LocationPrefix = serializedObject.LocationPrefix;
		            return;
		        } 
		        if (updateData.ObjectType == "SystemError")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SystemError.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SystemErrorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ErrorTitle = serializedObject.ErrorTitle;
		            existingObject.OccurredAt = serializedObject.OccurredAt;
					if(serializedObject.MessageContent != null)
						existingObject.MessageContentID = serializedObject.MessageContent.ID;
					else
						existingObject.MessageContentID = null;
		            return;
		        } 
		        if (updateData.ObjectType == "SystemErrorItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SystemErrorItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SystemErrorItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ShortDescription = serializedObject.ShortDescription;
		            existingObject.LongDescription = serializedObject.LongDescription;
		            return;
		        } 
		        if (updateData.ObjectType == "InformationSource")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.InformationSource.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = InformationSourceTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceName = serializedObject.SourceName;
		            existingObject.SourceLocation = serializedObject.SourceLocation;
		            existingObject.SourceType = serializedObject.SourceType;
		            existingObject.IsDynamic = serializedObject.IsDynamic;
		            existingObject.SourceInformationObjectType = serializedObject.SourceInformationObjectType;
		            existingObject.SourceETag = serializedObject.SourceETag;
		            existingObject.SourceMD5 = serializedObject.SourceMD5;
		            existingObject.SourceLastModified = serializedObject.SourceLastModified;
		            return;
		        } 
		        if (updateData.ObjectType == "RefreshDefaultViewsOperation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.RefreshDefaultViewsOperation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RefreshDefaultViewsOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.ViewLocation = serializedObject.ViewLocation;
		            existingObject.TypeNameToRefresh = serializedObject.TypeNameToRefresh;
		            return;
		        } 
		        if (updateData.ObjectType == "UpdateWebContentOperation")
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
		            return;
		        } 
		        if (updateData.ObjectType == "UpdateWebContentHandlerItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = UpdateWebContentHandlerItemTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InformationTypeName = serializedObject.InformationTypeName;
		            existingObject.OptionName = serializedObject.OptionName;
		            return;
		        } 
		        if (updateData.ObjectType == "PublishWebContentOperation")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.PublishWebContentOperation.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PublishWebContentOperationTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.SourceContainerName = serializedObject.SourceContainerName;
		            existingObject.SourcePathRoot = serializedObject.SourcePathRoot;
		            existingObject.SourceOwner = serializedObject.SourceOwner;
		            existingObject.TargetContainerName = serializedObject.TargetContainerName;
		            return;
		        } 
		        if (updateData.ObjectType == "SubscriberInput")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SubscriberInput.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriberInputTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.InputRelativeLocation = serializedObject.InputRelativeLocation;
		            existingObject.InformationObjectName = serializedObject.InformationObjectName;
		            existingObject.InformationItemName = serializedObject.InformationItemName;
		            existingObject.SubscriberRelativeLocation = serializedObject.SubscriberRelativeLocation;
		            return;
		        } 
		        if (updateData.ObjectType == "Monitor")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Monitor.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MonitorTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.TargetObjectName = serializedObject.TargetObjectName;
		            existingObject.TargetItemName = serializedObject.TargetItemName;
		            existingObject.MonitoringUtcTimeStampToStart = serializedObject.MonitoringUtcTimeStampToStart;
		            existingObject.MonitoringCycleFrequencyUnit = serializedObject.MonitoringCycleFrequencyUnit;
		            existingObject.MonitoringCycleEveryXthOfUnit = serializedObject.MonitoringCycleEveryXthOfUnit;
		            existingObject.CustomMonitoringCycleOperationName = serializedObject.CustomMonitoringCycleOperationName;
		            existingObject.OperationActionName = serializedObject.OperationActionName;
		            return;
		        } 
		        if (updateData.ObjectType == "IconTitleDescription")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.IconTitleDescription.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = IconTitleDescriptionTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
		            existingObject.Icon = serializedObject.Icon;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "AboutAGIApplications")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AboutAGIApplications.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AboutAGIApplicationsTable.Single(item => item.ID == updateData.ObjectID);
					existingObject.ETag = updateData.ETag;
					if(serializedObject.BuiltForAnybody != null)
						existingObject.BuiltForAnybodyID = serializedObject.BuiltForAnybody.ID;
					else
						existingObject.BuiltForAnybodyID = null;
					if(serializedObject.ForAllPeople != null)
						existingObject.ForAllPeopleID = serializedObject.ForAllPeople.ID;
					else
						existingObject.ForAllPeopleID = null;
		            return;
		        } 
		    }

		    public void PerformInsert(string storageRootPath, InformationObjectMetaData insertData)
		    {
                if (insertData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
                InformationObjectMetaDataTable.InsertOnSubmit(insertData);
                if (insertData.ObjectType == "TBSystem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBSystem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBSystem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InstanceName = serializedObject.InstanceName;
		            objectToAdd.AdminGroupID = serializedObject.AdminGroupID;
					TBSystemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "WebPublishInfo")
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
					WebPublishInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "PublicationPackage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PublicationPackage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.PackageName = serializedObject.PackageName;
		            objectToAdd.PublicationTime = serializedObject.PublicationTime;
					PublicationPackageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBRLoginRoot")
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
                    return;
                }
                if (insertData.ObjectType == "TBRAccountRoot")
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
                    return;
                }
                if (insertData.ObjectType == "TBRGroupRoot")
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
                    return;
                }
                if (insertData.ObjectType == "TBRLoginGroupRoot")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRLoginGroupRoot {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.GroupID = serializedObject.GroupID;
					TBRLoginGroupRootTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBREmailRoot")
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
                    return;
                }
                if (insertData.ObjectType == "TBAccount")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccount.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBAccount {ID = insertData.ObjectID, ETag = insertData.ETag};
					TBAccountTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBAccountCollaborationGroup")
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
                    return;
                }
                if (insertData.ObjectType == "TBLoginInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBLoginInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBLoginInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OpenIDUrl = serializedObject.OpenIDUrl;
					TBLoginInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBEmail")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBEmail.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
		            objectToAdd.ValidatedAt = serializedObject.ValidatedAt;
					TBEmailTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBCollaboratorRole")
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
                    return;
                }
                if (insertData.ObjectType == "TBCollaboratingGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBCollaboratingGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBCollaboratingGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					TBCollaboratingGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBEmailValidation")
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
                    return;
                }
                if (insertData.ObjectType == "TBMergeAccountConfirmation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBMergeAccountConfirmation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBMergeAccountConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.AccountToBeMergedID = serializedObject.AccountToBeMergedID;
		            objectToAdd.AccountToMergeToID = serializedObject.AccountToMergeToID;
					TBMergeAccountConfirmationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBGroupJoinConfirmation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBGroupJoinConfirmation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBGroupJoinConfirmation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupID = serializedObject.GroupID;
		            objectToAdd.InvitationMode = serializedObject.InvitationMode;
					TBGroupJoinConfirmationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBDeviceJoinConfirmation")
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
                    return;
                }
                if (insertData.ObjectType == "TBInformationInputConfirmation")
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
                    return;
                }
                if (insertData.ObjectType == "TBInformationOutputConfirmation")
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
                    return;
                }
                if (insertData.ObjectType == "TBRegisterContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRegisterContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRegisterContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
		            objectToAdd.ReturnUrl = serializedObject.ReturnUrl;
					TBRegisterContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "LoginProvider")
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
                    return;
                }
                if (insertData.ObjectType == "ContactOipContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ContactOipContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContactOipContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OIPModeratorGroupID = serializedObject.OIPModeratorGroupID;
					ContactOipContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBPRegisterEmail")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBPRegisterEmail.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBPRegisterEmail {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					TBPRegisterEmailTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "JavaScriptContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.JavaScriptContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new JavaScriptContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HtmlContent = serializedObject.HtmlContent;
					JavaScriptContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "JavascriptContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.JavascriptContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new JavascriptContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HtmlContent = serializedObject.HtmlContent;
					JavascriptContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "FooterContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.FooterContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new FooterContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.HtmlContent = serializedObject.HtmlContent;
					FooterContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "NavigationContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.NavigationContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NavigationContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Dummy = serializedObject.Dummy;
					NavigationContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					if(serializedObject.ActivitySummary != null)
						objectToAdd.ActivitySummaryID = serializedObject.ActivitySummary.ID;
					else
						objectToAdd.ActivitySummaryID = null;
					if(serializedObject.GroupSummary != null)
						objectToAdd.GroupSummaryID = serializedObject.GroupSummary.ID;
					else
						objectToAdd.GroupSummaryID = null;
					AccountSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
					if(serializedObject.AccountIndex != null)
						objectToAdd.AccountIndexID = serializedObject.AccountIndex.ID;
					else
						objectToAdd.AccountIndexID = null;
					if(serializedObject.AccountModule != null)
						objectToAdd.AccountModuleID = serializedObject.AccountModule.ID;
					else
						objectToAdd.AccountModuleID = null;
					if(serializedObject.AccountSummary != null)
						objectToAdd.AccountSummaryID = serializedObject.AccountSummary.ID;
					else
						objectToAdd.AccountSummaryID = null;
					AccountContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountIndex {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					AccountIndexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountModule")
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
					AccountModuleTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ImageGroupContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ImageGroupContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ImageGroupContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					ImageGroupContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "LocationContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LocationContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LocationContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					LocationContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddressAndLocation")
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
                    return;
                }
                if (insertData.ObjectType == "StreetAddress")
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
                    return;
                }
                if (insertData.ObjectType == "AccountContent")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountContent.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Dummy = serializedObject.Dummy;
					AccountContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountProfile")
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
                    return;
                }
                if (insertData.ObjectType == "AccountSecurity")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountSecurity.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountSecurity {ID = insertData.ObjectID, ETag = insertData.ETag};
					AccountSecurityTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountRoles")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountRoles {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.OrganizationsImPartOf = serializedObject.OrganizationsImPartOf;
					AccountRolesTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "PersonalInfoVisibility")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PersonalInfoVisibility.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PersonalInfoVisibility {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.NoOne_Network_All = serializedObject.NoOne_Network_All;
					PersonalInfoVisibilityTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GroupedInformation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupedInformation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupedInformation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					GroupedInformationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ReferenceToInformation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ReferenceToInformation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ReferenceToInformation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.URL = serializedObject.URL;
					ReferenceToInformationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "BlogContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.BlogContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new BlogContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
					if(serializedObject.FeaturedBlog != null)
						objectToAdd.FeaturedBlogID = serializedObject.FeaturedBlog.ID;
					else
						objectToAdd.FeaturedBlogID = null;
					if(serializedObject.RecentBlogSummary != null)
						objectToAdd.RecentBlogSummaryID = serializedObject.RecentBlogSummary.ID;
					else
						objectToAdd.RecentBlogSummaryID = null;
					if(serializedObject.BlogIndexGroup != null)
						objectToAdd.BlogIndexGroupID = serializedObject.BlogIndexGroup.ID;
					else
						objectToAdd.BlogIndexGroupID = null;
					BlogContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RecentBlogSummary")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.RecentBlogSummary.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RecentBlogSummary {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					RecentBlogSummaryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "NodeSummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.NodeSummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new NodeSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					NodeSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RenderedNode")
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
		            objectToAdd.CategoryIDList = serializedObject.CategoryIDList;
					RenderedNodeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ShortTextObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ShortTextObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					ShortTextObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "LongTextObject")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.LongTextObject.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new LongTextObject {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Content = serializedObject.Content;
					LongTextObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "MapContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
					if(serializedObject.MapFeatured != null)
						objectToAdd.MapFeaturedID = serializedObject.MapFeatured.ID;
					else
						objectToAdd.MapFeaturedID = null;
					if(serializedObject.MapIndexCollection != null)
						objectToAdd.MapIndexCollectionID = serializedObject.MapIndexCollection.ID;
					else
						objectToAdd.MapIndexCollectionID = null;
					MapContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "MapMarker")
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
                    return;
                }
                if (insertData.ObjectType == "CalendarContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CalendarContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CalendarContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.CalendarContainerHeader != null)
						objectToAdd.CalendarContainerHeaderID = serializedObject.CalendarContainerHeader.ID;
					else
						objectToAdd.CalendarContainerHeaderID = null;
					if(serializedObject.CalendarFeatured != null)
						objectToAdd.CalendarFeaturedID = serializedObject.CalendarFeatured.ID;
					else
						objectToAdd.CalendarFeaturedID = null;
					if(serializedObject.CalendarIndexCollection != null)
						objectToAdd.CalendarIndexCollectionID = serializedObject.CalendarIndexCollection.ID;
					else
						objectToAdd.CalendarIndexCollectionID = null;
					CalendarContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AboutContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AboutContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AboutContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.MainImage != null)
						objectToAdd.MainImageID = serializedObject.MainImage.ID;
					else
						objectToAdd.MainImageID = null;
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Body = serializedObject.Body;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					if(serializedObject.ImageGroup != null)
						objectToAdd.ImageGroupID = serializedObject.ImageGroup.ID;
					else
						objectToAdd.ImageGroupID = null;
					AboutContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "OBSAccountContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.OBSAccountContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OBSAccountContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.AccountContainerHeader != null)
						objectToAdd.AccountContainerHeaderID = serializedObject.AccountContainerHeader.ID;
					else
						objectToAdd.AccountContainerHeaderID = null;
					if(serializedObject.AccountFeatured != null)
						objectToAdd.AccountFeaturedID = serializedObject.AccountFeatured.ID;
					else
						objectToAdd.AccountFeaturedID = null;
					if(serializedObject.AccountIndexCollection != null)
						objectToAdd.AccountIndexCollectionID = serializedObject.AccountIndexCollection.ID;
					else
						objectToAdd.AccountIndexCollectionID = null;
					OBSAccountContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ProjectContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ProjectContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ProjectContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ProjectContainerHeader != null)
						objectToAdd.ProjectContainerHeaderID = serializedObject.ProjectContainerHeader.ID;
					else
						objectToAdd.ProjectContainerHeaderID = null;
					if(serializedObject.ProjectFeatured != null)
						objectToAdd.ProjectFeaturedID = serializedObject.ProjectFeatured.ID;
					else
						objectToAdd.ProjectFeaturedID = null;
					if(serializedObject.ProjectIndexCollection != null)
						objectToAdd.ProjectIndexCollectionID = serializedObject.ProjectIndexCollection.ID;
					else
						objectToAdd.ProjectIndexCollectionID = null;
					ProjectContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CourseContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CourseContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CourseContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.CourseContainerHeader != null)
						objectToAdd.CourseContainerHeaderID = serializedObject.CourseContainerHeader.ID;
					else
						objectToAdd.CourseContainerHeaderID = null;
					if(serializedObject.CourseFeatured != null)
						objectToAdd.CourseFeaturedID = serializedObject.CourseFeatured.ID;
					else
						objectToAdd.CourseFeaturedID = null;
					if(serializedObject.CourseIndexCollection != null)
						objectToAdd.CourseIndexCollectionID = serializedObject.CourseIndexCollection.ID;
					else
						objectToAdd.CourseIndexCollectionID = null;
					CourseContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ContainerHeader")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ContainerHeader.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContainerHeader {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.SubTitle = serializedObject.SubTitle;
					ContainerHeaderTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ActivitySummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ActivitySummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ActivitySummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					if(serializedObject.ActivityIndex != null)
						objectToAdd.ActivityIndexID = serializedObject.ActivityIndex.ID;
					else
						objectToAdd.ActivityIndexID = null;
					ActivitySummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ActivityIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ActivityIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ActivityIndex {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					ActivityIndexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ActivityContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ActivityContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ActivityContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
					if(serializedObject.ActivityIndex != null)
						objectToAdd.ActivityIndexID = serializedObject.ActivityIndex.ID;
					else
						objectToAdd.ActivityIndexID = null;
					if(serializedObject.ActivityModule != null)
						objectToAdd.ActivityModuleID = serializedObject.ActivityModule.ID;
					else
						objectToAdd.ActivityModuleID = null;
					ActivityContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Activity")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Activity.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Activity {ID = insertData.ObjectID, ETag = insertData.ETag};
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
		            objectToAdd.ActivityName = serializedObject.ActivityName;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
		            objectToAdd.ContactPerson = serializedObject.ContactPerson;
		            objectToAdd.StartingTime = serializedObject.StartingTime;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.IFrameSources = serializedObject.IFrameSources;
					ActivityTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Moderator")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Moderator.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Moderator {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ModeratorName = serializedObject.ModeratorName;
		            objectToAdd.ProfileUrl = serializedObject.ProfileUrl;
					ModeratorTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Collaborator")
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
                    return;
                }
                if (insertData.ObjectType == "GroupSummaryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupSummaryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupSummaryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
					if(serializedObject.GroupSummaryIndex != null)
						objectToAdd.GroupSummaryIndexID = serializedObject.GroupSummaryIndex.ID;
					else
						objectToAdd.GroupSummaryIndexID = null;
					GroupSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GroupContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Header != null)
						objectToAdd.HeaderID = serializedObject.Header.ID;
					else
						objectToAdd.HeaderID = null;
					if(serializedObject.GroupIndex != null)
						objectToAdd.GroupIndexID = serializedObject.GroupIndex.ID;
					else
						objectToAdd.GroupIndexID = null;
					if(serializedObject.GroupProfile != null)
						objectToAdd.GroupProfileID = serializedObject.GroupProfile.ID;
					else
						objectToAdd.GroupProfileID = null;
					GroupContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GroupIndex")
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
                    return;
                }
                if (insertData.ObjectType == "AddAddressAndLocationInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddAddressAndLocationInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddAddressAndLocationInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.LocationName = serializedObject.LocationName;
					AddAddressAndLocationInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddImageInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddImageInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageTitle = serializedObject.ImageTitle;
					AddImageInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddImageGroupInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddImageGroupInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddImageGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ImageGroupTitle = serializedObject.ImageGroupTitle;
					AddImageGroupInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddEmailAddressInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddEmailAddressInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddEmailAddressInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.EmailAddress = serializedObject.EmailAddress;
					AddEmailAddressInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CreateGroupInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CreateGroupInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CreateGroupInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.GroupName = serializedObject.GroupName;
					CreateGroupInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddActivityInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddActivityInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddActivityInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ActivityName = serializedObject.ActivityName;
					AddActivityInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddBlogPostInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddBlogPostInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddBlogPostInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					AddBlogPostInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AddCategoryInfo")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AddCategoryInfo.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AddCategoryInfo {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
					AddCategoryInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Group")
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
					GroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Introduction")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Introduction.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Introduction {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Body = serializedObject.Body;
					IntroductionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ContentCategoryRank")
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
                    return;
                }
                if (insertData.ObjectType == "LinkToContent")
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
					LinkToContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "EmbeddedContent")
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
					EmbeddedContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "DynamicContentGroup")
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
                    return;
                }
                if (insertData.ObjectType == "DynamicContent")
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
                    return;
                }
                if (insertData.ObjectType == "AttachedToObject")
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
                    return;
                }
                if (insertData.ObjectType == "Comment")
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
                    return;
                }
                if (insertData.ObjectType == "Selection")
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
                    return;
                }
                if (insertData.ObjectType == "TextContent")
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
		            objectToAdd.SortOrderNumber = serializedObject.SortOrderNumber;
		            objectToAdd.IFrameSources = serializedObject.IFrameSources;
		            objectToAdd.RawHtmlContent = serializedObject.RawHtmlContent;
					TextContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Blog")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Blog.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Blog {ID = insertData.ObjectID, ETag = insertData.ETag};
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
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.SubTitle = serializedObject.SubTitle;
					if(serializedObject.Introduction != null)
						objectToAdd.IntroductionID = serializedObject.Introduction.ID;
					else
						objectToAdd.IntroductionID = null;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					if(serializedObject.FeaturedImage != null)
						objectToAdd.FeaturedImageID = serializedObject.FeaturedImage.ID;
					else
						objectToAdd.FeaturedImageID = null;
					if(serializedObject.VideoGroup != null)
						objectToAdd.VideoGroupID = serializedObject.VideoGroup.ID;
					else
						objectToAdd.VideoGroupID = null;
		            objectToAdd.Body = serializedObject.Body;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.IFrameSources = serializedObject.IFrameSources;
					BlogTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "BlogIndexGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.BlogIndexGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new BlogIndexGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					BlogIndexGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CalendarIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CalendarIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CalendarIndex {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.Icon != null)
						objectToAdd.IconID = serializedObject.Icon.ID;
					else
						objectToAdd.IconID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					CalendarIndexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Filter")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Filter.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Filter {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					FilterTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Calendar")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Calendar.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Calendar {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					CalendarTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Map")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Map.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Map {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
					MapTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "MapIndexCollection")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapIndexCollection.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapIndexCollection {ID = insertData.ObjectID, ETag = insertData.ETag};
					MapIndexCollectionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "MapResult")
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
                    return;
                }
                if (insertData.ObjectType == "MapResultsCollection")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapResultsCollection.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapResultsCollection {ID = insertData.ObjectID, ETag = insertData.ETag};
					MapResultsCollectionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Video")
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
                    return;
                }
                if (insertData.ObjectType == "Image")
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
					ImageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "BinaryFile")
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
					BinaryFileTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ImageGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ImageGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ImageGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.ReferenceToInformation != null)
						objectToAdd.ReferenceToInformationID = serializedObject.ReferenceToInformation.ID;
					else
						objectToAdd.ReferenceToInformationID = null;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					if(serializedObject.FeaturedImage != null)
						objectToAdd.FeaturedImageID = serializedObject.FeaturedImage.ID;
					else
						objectToAdd.FeaturedImageID = null;
					ImageGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "VideoGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.VideoGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new VideoGroup {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					VideoGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Tooltip")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Tooltip.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Tooltip {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TooltipText = serializedObject.TooltipText;
					TooltipTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SocialPanel")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SocialPanel.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SocialPanel {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.SocialFilter != null)
						objectToAdd.SocialFilterID = serializedObject.SocialFilter.ID;
					else
						objectToAdd.SocialFilterID = null;
					SocialPanelTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Longitude")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Longitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LongitudeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Latitude")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Latitude.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Latitude {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TextValue = serializedObject.TextValue;
					LatitudeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Location")
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
                    return;
                }
                if (insertData.ObjectType == "Date")
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
                    return;
                }
                if (insertData.ObjectType == "Sex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Sex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Sex {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SexText = serializedObject.SexText;
					SexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "OBSAddress")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.OBSAddress.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OBSAddress {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.StreetName = serializedObject.StreetName;
		            objectToAdd.BuildingNumber = serializedObject.BuildingNumber;
		            objectToAdd.PostOfficeBox = serializedObject.PostOfficeBox;
		            objectToAdd.PostalCode = serializedObject.PostalCode;
		            objectToAdd.Municipality = serializedObject.Municipality;
		            objectToAdd.Region = serializedObject.Region;
		            objectToAdd.Province = serializedObject.Province;
		            objectToAdd.state = serializedObject.state;
		            objectToAdd.Country = serializedObject.Country;
		            objectToAdd.Continent = serializedObject.Continent;
					OBSAddressTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Identity")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Identity.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Identity {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
		            objectToAdd.Initials = serializedObject.Initials;
					if(serializedObject.Sex != null)
						objectToAdd.SexID = serializedObject.Sex.ID;
					else
						objectToAdd.SexID = null;
					if(serializedObject.Birthday != null)
						objectToAdd.BirthdayID = serializedObject.Birthday.ID;
					else
						objectToAdd.BirthdayID = null;
					IdentityTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ImageVideoSoundVectorRaw")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ImageVideoSoundVectorRaw.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ImageVideoSoundVectorRaw {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Image = serializedObject.Image;
		            objectToAdd.Video = serializedObject.Video;
		            objectToAdd.Sound = serializedObject.Sound;
		            objectToAdd.Vector = serializedObject.Vector;
		            objectToAdd.Raw = serializedObject.Raw;
					ImageVideoSoundVectorRawTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "CategoryContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.CategoryContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new CategoryContainer {ID = insertData.ObjectID, ETag = insertData.ETag};
					CategoryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Category")
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
                    return;
                }
                if (insertData.ObjectType == "Subscription")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Subscription.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Subscription {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Priority = serializedObject.Priority;
		            objectToAdd.TargetRelativeLocation = serializedObject.TargetRelativeLocation;
		            objectToAdd.TargetInformationObjectType = serializedObject.TargetInformationObjectType;
		            objectToAdd.SubscriberRelativeLocation = serializedObject.SubscriberRelativeLocation;
		            objectToAdd.SubscriberInformationObjectType = serializedObject.SubscriberInformationObjectType;
		            objectToAdd.SubscriptionType = serializedObject.SubscriptionType;
					SubscriptionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "QueueEnvelope")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.QueueEnvelope.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new QueueEnvelope {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ActiveContainerName = serializedObject.ActiveContainerName;
		            objectToAdd.OwnerPrefix = serializedObject.OwnerPrefix;
		            objectToAdd.CurrentRetryCount = serializedObject.CurrentRetryCount;
					if(serializedObject.SingleOperation != null)
						objectToAdd.SingleOperationID = serializedObject.SingleOperation.ID;
					else
						objectToAdd.SingleOperationID = null;
					if(serializedObject.ErrorContent != null)
						objectToAdd.ErrorContentID = serializedObject.ErrorContent.ID;
					else
						objectToAdd.ErrorContentID = null;
					QueueEnvelopeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "OperationRequest")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.OperationRequest.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OperationRequest {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.SubscriberNotification != null)
						objectToAdd.SubscriberNotificationID = serializedObject.SubscriberNotification.ID;
					else
						objectToAdd.SubscriberNotificationID = null;
					if(serializedObject.SubscriptionChainRequest != null)
						objectToAdd.SubscriptionChainRequestID = serializedObject.SubscriptionChainRequest.ID;
					else
						objectToAdd.SubscriptionChainRequestID = null;
					if(serializedObject.UpdateWebContentOperation != null)
						objectToAdd.UpdateWebContentOperationID = serializedObject.UpdateWebContentOperation.ID;
					else
						objectToAdd.UpdateWebContentOperationID = null;
					if(serializedObject.RefreshDefaultViewsOperation != null)
						objectToAdd.RefreshDefaultViewsOperationID = serializedObject.RefreshDefaultViewsOperation.ID;
					else
						objectToAdd.RefreshDefaultViewsOperationID = null;
					if(serializedObject.DeleteEntireOwner != null)
						objectToAdd.DeleteEntireOwnerID = serializedObject.DeleteEntireOwner.ID;
					else
						objectToAdd.DeleteEntireOwnerID = null;
					if(serializedObject.DeleteOwnerContent != null)
						objectToAdd.DeleteOwnerContentID = serializedObject.DeleteOwnerContent.ID;
					else
						objectToAdd.DeleteOwnerContentID = null;
					if(serializedObject.PublishWebContent != null)
						objectToAdd.PublishWebContentID = serializedObject.PublishWebContent.ID;
					else
						objectToAdd.PublishWebContentID = null;
		            objectToAdd.ProcessIDToExecute = serializedObject.ProcessIDToExecute;
					OperationRequestTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SubscriptionChainRequestMessage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SubscriptionChainRequestMessage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SubscriptionChainRequestMessage {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ContentItemID = serializedObject.ContentItemID;
					SubscriptionChainRequestMessageTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SubscriptionChainRequestContent")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SubscriptionChainRequestContent.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SubscriptionChainRequestContent {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SubmitTime = serializedObject.SubmitTime;
		            objectToAdd.ProcessingStartTime = serializedObject.ProcessingStartTime;
		            objectToAdd.ProcessingEndTimeInformationObjects = serializedObject.ProcessingEndTimeInformationObjects;
		            objectToAdd.ProcessingEndTimeWebTemplatesRendering = serializedObject.ProcessingEndTimeWebTemplatesRendering;
		            objectToAdd.ProcessingEndTime = serializedObject.ProcessingEndTime;
					SubscriptionChainRequestContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SubscriptionTarget")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SubscriptionTarget.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SubscriptionTarget {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.BlobLocation = serializedObject.BlobLocation;
					SubscriptionTargetTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "DeleteEntireOwnerOperation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DeleteEntireOwnerOperation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DeleteEntireOwnerOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ContainerName = serializedObject.ContainerName;
		            objectToAdd.LocationPrefix = serializedObject.LocationPrefix;
					DeleteEntireOwnerOperationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "DeleteOwnerContentOperation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.DeleteOwnerContentOperation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new DeleteOwnerContentOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ContainerName = serializedObject.ContainerName;
		            objectToAdd.LocationPrefix = serializedObject.LocationPrefix;
					DeleteOwnerContentOperationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SystemError")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SystemError.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SystemError {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ErrorTitle = serializedObject.ErrorTitle;
		            objectToAdd.OccurredAt = serializedObject.OccurredAt;
					if(serializedObject.MessageContent != null)
						objectToAdd.MessageContentID = serializedObject.MessageContent.ID;
					else
						objectToAdd.MessageContentID = null;
					SystemErrorTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SystemErrorItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SystemErrorItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SystemErrorItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ShortDescription = serializedObject.ShortDescription;
		            objectToAdd.LongDescription = serializedObject.LongDescription;
					SystemErrorItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "InformationSource")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.InformationSource.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new InformationSource {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceName = serializedObject.SourceName;
		            objectToAdd.SourceLocation = serializedObject.SourceLocation;
		            objectToAdd.SourceType = serializedObject.SourceType;
		            objectToAdd.IsDynamic = serializedObject.IsDynamic;
		            objectToAdd.SourceInformationObjectType = serializedObject.SourceInformationObjectType;
		            objectToAdd.SourceETag = serializedObject.SourceETag;
		            objectToAdd.SourceMD5 = serializedObject.SourceMD5;
		            objectToAdd.SourceLastModified = serializedObject.SourceLastModified;
					InformationSourceTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RefreshDefaultViewsOperation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.RefreshDefaultViewsOperation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RefreshDefaultViewsOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.ViewLocation = serializedObject.ViewLocation;
		            objectToAdd.TypeNameToRefresh = serializedObject.TypeNameToRefresh;
					RefreshDefaultViewsOperationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UpdateWebContentOperation")
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
					UpdateWebContentOperationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "UpdateWebContentHandlerItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.UpdateWebContentHandlerItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new UpdateWebContentHandlerItem {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InformationTypeName = serializedObject.InformationTypeName;
		            objectToAdd.OptionName = serializedObject.OptionName;
					UpdateWebContentHandlerItemTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "PublishWebContentOperation")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PublishWebContentOperation.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PublishWebContentOperation {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.SourceContainerName = serializedObject.SourceContainerName;
		            objectToAdd.SourcePathRoot = serializedObject.SourcePathRoot;
		            objectToAdd.SourceOwner = serializedObject.SourceOwner;
		            objectToAdd.TargetContainerName = serializedObject.TargetContainerName;
					PublishWebContentOperationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SubscriberInput")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SubscriberInput.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SubscriberInput {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.InputRelativeLocation = serializedObject.InputRelativeLocation;
		            objectToAdd.InformationObjectName = serializedObject.InformationObjectName;
		            objectToAdd.InformationItemName = serializedObject.InformationItemName;
		            objectToAdd.SubscriberRelativeLocation = serializedObject.SubscriberRelativeLocation;
					SubscriberInputTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Monitor")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Monitor.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Monitor {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.TargetObjectName = serializedObject.TargetObjectName;
		            objectToAdd.TargetItemName = serializedObject.TargetItemName;
		            objectToAdd.MonitoringUtcTimeStampToStart = serializedObject.MonitoringUtcTimeStampToStart;
		            objectToAdd.MonitoringCycleFrequencyUnit = serializedObject.MonitoringCycleFrequencyUnit;
		            objectToAdd.MonitoringCycleEveryXthOfUnit = serializedObject.MonitoringCycleEveryXthOfUnit;
		            objectToAdd.CustomMonitoringCycleOperationName = serializedObject.CustomMonitoringCycleOperationName;
		            objectToAdd.OperationActionName = serializedObject.OperationActionName;
					MonitorTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "IconTitleDescription")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.IconTitleDescription.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new IconTitleDescription {ID = insertData.ObjectID, ETag = insertData.ETag};
		            objectToAdd.Icon = serializedObject.Icon;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					IconTitleDescriptionTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AboutAGIApplications")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AboutAGIApplications.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AboutAGIApplications {ID = insertData.ObjectID, ETag = insertData.ETag};
					if(serializedObject.BuiltForAnybody != null)
						objectToAdd.BuiltForAnybodyID = serializedObject.BuiltForAnybody.ID;
					else
						objectToAdd.BuiltForAnybodyID = null;
					if(serializedObject.ForAllPeople != null)
						objectToAdd.ForAllPeopleID = serializedObject.ForAllPeople.ID;
					else
						objectToAdd.ForAllPeopleID = null;
					AboutAGIApplicationsTable.InsertOnSubmit(objectToAdd);
                    return;
                }
            }

		    public void PerformDelete(string storageRootPath, InformationObjectMetaData deleteData)
		    {
                if (deleteData.SemanticDomain != "AaltoGlobalImpact.OIP")
                    throw new InvalidDataException("Mismatch on domain data");
				InformationObjectMetaDataTable.DeleteOnSubmit(deleteData);
		        if (deleteData.ObjectType == "TBSystem")
		        {
		            var objectToDelete = new TBSystem {ID = deleteData.ID};
                    TBSystemTable.Attach(objectToDelete);
                    TBSystemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "WebPublishInfo")
		        {
		            var objectToDelete = new WebPublishInfo {ID = deleteData.ID};
                    WebPublishInfoTable.Attach(objectToDelete);
                    WebPublishInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "PublicationPackage")
		        {
		            var objectToDelete = new PublicationPackage {ID = deleteData.ID};
                    PublicationPackageTable.Attach(objectToDelete);
                    PublicationPackageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBRLoginRoot")
		        {
		            var objectToDelete = new TBRLoginRoot {ID = deleteData.ID};
                    TBRLoginRootTable.Attach(objectToDelete);
                    TBRLoginRootTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBRAccountRoot")
		        {
		            var objectToDelete = new TBRAccountRoot {ID = deleteData.ID};
                    TBRAccountRootTable.Attach(objectToDelete);
                    TBRAccountRootTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBRGroupRoot")
		        {
		            var objectToDelete = new TBRGroupRoot {ID = deleteData.ID};
                    TBRGroupRootTable.Attach(objectToDelete);
                    TBRGroupRootTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBRLoginGroupRoot")
		        {
		            var objectToDelete = new TBRLoginGroupRoot {ID = deleteData.ID};
                    TBRLoginGroupRootTable.Attach(objectToDelete);
                    TBRLoginGroupRootTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBREmailRoot")
		        {
		            var objectToDelete = new TBREmailRoot {ID = deleteData.ID};
                    TBREmailRootTable.Attach(objectToDelete);
                    TBREmailRootTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBAccount")
		        {
		            var objectToDelete = new TBAccount {ID = deleteData.ID};
                    TBAccountTable.Attach(objectToDelete);
                    TBAccountTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBAccountCollaborationGroup")
		        {
		            var objectToDelete = new TBAccountCollaborationGroup {ID = deleteData.ID};
                    TBAccountCollaborationGroupTable.Attach(objectToDelete);
                    TBAccountCollaborationGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBLoginInfo")
		        {
		            var objectToDelete = new TBLoginInfo {ID = deleteData.ID};
                    TBLoginInfoTable.Attach(objectToDelete);
                    TBLoginInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBEmail")
		        {
		            var objectToDelete = new TBEmail {ID = deleteData.ID};
                    TBEmailTable.Attach(objectToDelete);
                    TBEmailTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBCollaboratorRole")
		        {
		            var objectToDelete = new TBCollaboratorRole {ID = deleteData.ID};
                    TBCollaboratorRoleTable.Attach(objectToDelete);
                    TBCollaboratorRoleTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBCollaboratingGroup")
		        {
		            var objectToDelete = new TBCollaboratingGroup {ID = deleteData.ID};
                    TBCollaboratingGroupTable.Attach(objectToDelete);
                    TBCollaboratingGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBEmailValidation")
		        {
		            var objectToDelete = new TBEmailValidation {ID = deleteData.ID};
                    TBEmailValidationTable.Attach(objectToDelete);
                    TBEmailValidationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBMergeAccountConfirmation")
		        {
		            var objectToDelete = new TBMergeAccountConfirmation {ID = deleteData.ID};
                    TBMergeAccountConfirmationTable.Attach(objectToDelete);
                    TBMergeAccountConfirmationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBGroupJoinConfirmation")
		        {
		            var objectToDelete = new TBGroupJoinConfirmation {ID = deleteData.ID};
                    TBGroupJoinConfirmationTable.Attach(objectToDelete);
                    TBGroupJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBDeviceJoinConfirmation")
		        {
		            var objectToDelete = new TBDeviceJoinConfirmation {ID = deleteData.ID};
                    TBDeviceJoinConfirmationTable.Attach(objectToDelete);
                    TBDeviceJoinConfirmationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBInformationInputConfirmation")
		        {
		            var objectToDelete = new TBInformationInputConfirmation {ID = deleteData.ID};
                    TBInformationInputConfirmationTable.Attach(objectToDelete);
                    TBInformationInputConfirmationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBInformationOutputConfirmation")
		        {
		            var objectToDelete = new TBInformationOutputConfirmation {ID = deleteData.ID};
                    TBInformationOutputConfirmationTable.Attach(objectToDelete);
                    TBInformationOutputConfirmationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBRegisterContainer")
		        {
		            var objectToDelete = new TBRegisterContainer {ID = deleteData.ID};
                    TBRegisterContainerTable.Attach(objectToDelete);
                    TBRegisterContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "LoginProvider")
		        {
		            var objectToDelete = new LoginProvider {ID = deleteData.ID};
                    LoginProviderTable.Attach(objectToDelete);
                    LoginProviderTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ContactOipContainer")
		        {
		            var objectToDelete = new ContactOipContainer {ID = deleteData.ID};
                    ContactOipContainerTable.Attach(objectToDelete);
                    ContactOipContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TBPRegisterEmail")
		        {
		            var objectToDelete = new TBPRegisterEmail {ID = deleteData.ID};
                    TBPRegisterEmailTable.Attach(objectToDelete);
                    TBPRegisterEmailTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "JavaScriptContainer")
		        {
		            var objectToDelete = new JavaScriptContainer {ID = deleteData.ID};
                    JavaScriptContainerTable.Attach(objectToDelete);
                    JavaScriptContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "JavascriptContainer")
		        {
		            var objectToDelete = new JavascriptContainer {ID = deleteData.ID};
                    JavascriptContainerTable.Attach(objectToDelete);
                    JavascriptContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "FooterContainer")
		        {
		            var objectToDelete = new FooterContainer {ID = deleteData.ID};
                    FooterContainerTable.Attach(objectToDelete);
                    FooterContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "NavigationContainer")
		        {
		            var objectToDelete = new NavigationContainer {ID = deleteData.ID};
                    NavigationContainerTable.Attach(objectToDelete);
                    NavigationContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountSummary")
		        {
		            var objectToDelete = new AccountSummary {ID = deleteData.ID};
                    AccountSummaryTable.Attach(objectToDelete);
                    AccountSummaryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountContainer")
		        {
		            var objectToDelete = new AccountContainer {ID = deleteData.ID};
                    AccountContainerTable.Attach(objectToDelete);
                    AccountContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountIndex")
		        {
		            var objectToDelete = new AccountIndex {ID = deleteData.ID};
                    AccountIndexTable.Attach(objectToDelete);
                    AccountIndexTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountModule")
		        {
		            var objectToDelete = new AccountModule {ID = deleteData.ID};
                    AccountModuleTable.Attach(objectToDelete);
                    AccountModuleTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ImageGroupContainer")
		        {
		            var objectToDelete = new ImageGroupContainer {ID = deleteData.ID};
                    ImageGroupContainerTable.Attach(objectToDelete);
                    ImageGroupContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "LocationContainer")
		        {
		            var objectToDelete = new LocationContainer {ID = deleteData.ID};
                    LocationContainerTable.Attach(objectToDelete);
                    LocationContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddressAndLocation")
		        {
		            var objectToDelete = new AddressAndLocation {ID = deleteData.ID};
                    AddressAndLocationTable.Attach(objectToDelete);
                    AddressAndLocationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "StreetAddress")
		        {
		            var objectToDelete = new StreetAddress {ID = deleteData.ID};
                    StreetAddressTable.Attach(objectToDelete);
                    StreetAddressTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountContent")
		        {
		            var objectToDelete = new AccountContent {ID = deleteData.ID};
                    AccountContentTable.Attach(objectToDelete);
                    AccountContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountProfile")
		        {
		            var objectToDelete = new AccountProfile {ID = deleteData.ID};
                    AccountProfileTable.Attach(objectToDelete);
                    AccountProfileTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountSecurity")
		        {
		            var objectToDelete = new AccountSecurity {ID = deleteData.ID};
                    AccountSecurityTable.Attach(objectToDelete);
                    AccountSecurityTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AccountRoles")
		        {
		            var objectToDelete = new AccountRoles {ID = deleteData.ID};
                    AccountRolesTable.Attach(objectToDelete);
                    AccountRolesTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "PersonalInfoVisibility")
		        {
		            var objectToDelete = new PersonalInfoVisibility {ID = deleteData.ID};
                    PersonalInfoVisibilityTable.Attach(objectToDelete);
                    PersonalInfoVisibilityTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GroupedInformation")
		        {
		            var objectToDelete = new GroupedInformation {ID = deleteData.ID};
                    GroupedInformationTable.Attach(objectToDelete);
                    GroupedInformationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ReferenceToInformation")
		        {
		            var objectToDelete = new ReferenceToInformation {ID = deleteData.ID};
                    ReferenceToInformationTable.Attach(objectToDelete);
                    ReferenceToInformationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "BlogContainer")
		        {
		            var objectToDelete = new BlogContainer {ID = deleteData.ID};
                    BlogContainerTable.Attach(objectToDelete);
                    BlogContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "RecentBlogSummary")
		        {
		            var objectToDelete = new RecentBlogSummary {ID = deleteData.ID};
                    RecentBlogSummaryTable.Attach(objectToDelete);
                    RecentBlogSummaryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "NodeSummaryContainer")
		        {
		            var objectToDelete = new NodeSummaryContainer {ID = deleteData.ID};
                    NodeSummaryContainerTable.Attach(objectToDelete);
                    NodeSummaryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "RenderedNode")
		        {
		            var objectToDelete = new RenderedNode {ID = deleteData.ID};
                    RenderedNodeTable.Attach(objectToDelete);
                    RenderedNodeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ShortTextObject")
		        {
		            var objectToDelete = new ShortTextObject {ID = deleteData.ID};
                    ShortTextObjectTable.Attach(objectToDelete);
                    ShortTextObjectTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "LongTextObject")
		        {
		            var objectToDelete = new LongTextObject {ID = deleteData.ID};
                    LongTextObjectTable.Attach(objectToDelete);
                    LongTextObjectTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "MapContainer")
		        {
		            var objectToDelete = new MapContainer {ID = deleteData.ID};
                    MapContainerTable.Attach(objectToDelete);
                    MapContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "MapMarker")
		        {
		            var objectToDelete = new MapMarker {ID = deleteData.ID};
                    MapMarkerTable.Attach(objectToDelete);
                    MapMarkerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CalendarContainer")
		        {
		            var objectToDelete = new CalendarContainer {ID = deleteData.ID};
                    CalendarContainerTable.Attach(objectToDelete);
                    CalendarContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AboutContainer")
		        {
		            var objectToDelete = new AboutContainer {ID = deleteData.ID};
                    AboutContainerTable.Attach(objectToDelete);
                    AboutContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "OBSAccountContainer")
		        {
		            var objectToDelete = new OBSAccountContainer {ID = deleteData.ID};
                    OBSAccountContainerTable.Attach(objectToDelete);
                    OBSAccountContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ProjectContainer")
		        {
		            var objectToDelete = new ProjectContainer {ID = deleteData.ID};
                    ProjectContainerTable.Attach(objectToDelete);
                    ProjectContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CourseContainer")
		        {
		            var objectToDelete = new CourseContainer {ID = deleteData.ID};
                    CourseContainerTable.Attach(objectToDelete);
                    CourseContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ContainerHeader")
		        {
		            var objectToDelete = new ContainerHeader {ID = deleteData.ID};
                    ContainerHeaderTable.Attach(objectToDelete);
                    ContainerHeaderTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ActivitySummaryContainer")
		        {
		            var objectToDelete = new ActivitySummaryContainer {ID = deleteData.ID};
                    ActivitySummaryContainerTable.Attach(objectToDelete);
                    ActivitySummaryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ActivityIndex")
		        {
		            var objectToDelete = new ActivityIndex {ID = deleteData.ID};
                    ActivityIndexTable.Attach(objectToDelete);
                    ActivityIndexTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ActivityContainer")
		        {
		            var objectToDelete = new ActivityContainer {ID = deleteData.ID};
                    ActivityContainerTable.Attach(objectToDelete);
                    ActivityContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Activity")
		        {
		            var objectToDelete = new Activity {ID = deleteData.ID};
                    ActivityTable.Attach(objectToDelete);
                    ActivityTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Moderator")
		        {
		            var objectToDelete = new Moderator {ID = deleteData.ID};
                    ModeratorTable.Attach(objectToDelete);
                    ModeratorTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Collaborator")
		        {
		            var objectToDelete = new Collaborator {ID = deleteData.ID};
                    CollaboratorTable.Attach(objectToDelete);
                    CollaboratorTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GroupSummaryContainer")
		        {
		            var objectToDelete = new GroupSummaryContainer {ID = deleteData.ID};
                    GroupSummaryContainerTable.Attach(objectToDelete);
                    GroupSummaryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GroupContainer")
		        {
		            var objectToDelete = new GroupContainer {ID = deleteData.ID};
                    GroupContainerTable.Attach(objectToDelete);
                    GroupContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "GroupIndex")
		        {
		            var objectToDelete = new GroupIndex {ID = deleteData.ID};
                    GroupIndexTable.Attach(objectToDelete);
                    GroupIndexTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddAddressAndLocationInfo")
		        {
		            var objectToDelete = new AddAddressAndLocationInfo {ID = deleteData.ID};
                    AddAddressAndLocationInfoTable.Attach(objectToDelete);
                    AddAddressAndLocationInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddImageInfo")
		        {
		            var objectToDelete = new AddImageInfo {ID = deleteData.ID};
                    AddImageInfoTable.Attach(objectToDelete);
                    AddImageInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddImageGroupInfo")
		        {
		            var objectToDelete = new AddImageGroupInfo {ID = deleteData.ID};
                    AddImageGroupInfoTable.Attach(objectToDelete);
                    AddImageGroupInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddEmailAddressInfo")
		        {
		            var objectToDelete = new AddEmailAddressInfo {ID = deleteData.ID};
                    AddEmailAddressInfoTable.Attach(objectToDelete);
                    AddEmailAddressInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CreateGroupInfo")
		        {
		            var objectToDelete = new CreateGroupInfo {ID = deleteData.ID};
                    CreateGroupInfoTable.Attach(objectToDelete);
                    CreateGroupInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddActivityInfo")
		        {
		            var objectToDelete = new AddActivityInfo {ID = deleteData.ID};
                    AddActivityInfoTable.Attach(objectToDelete);
                    AddActivityInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddBlogPostInfo")
		        {
		            var objectToDelete = new AddBlogPostInfo {ID = deleteData.ID};
                    AddBlogPostInfoTable.Attach(objectToDelete);
                    AddBlogPostInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AddCategoryInfo")
		        {
		            var objectToDelete = new AddCategoryInfo {ID = deleteData.ID};
                    AddCategoryInfoTable.Attach(objectToDelete);
                    AddCategoryInfoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Group")
		        {
		            var objectToDelete = new Group {ID = deleteData.ID};
                    GroupTable.Attach(objectToDelete);
                    GroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Introduction")
		        {
		            var objectToDelete = new Introduction {ID = deleteData.ID};
                    IntroductionTable.Attach(objectToDelete);
                    IntroductionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ContentCategoryRank")
		        {
		            var objectToDelete = new ContentCategoryRank {ID = deleteData.ID};
                    ContentCategoryRankTable.Attach(objectToDelete);
                    ContentCategoryRankTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "LinkToContent")
		        {
		            var objectToDelete = new LinkToContent {ID = deleteData.ID};
                    LinkToContentTable.Attach(objectToDelete);
                    LinkToContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "EmbeddedContent")
		        {
		            var objectToDelete = new EmbeddedContent {ID = deleteData.ID};
                    EmbeddedContentTable.Attach(objectToDelete);
                    EmbeddedContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "DynamicContentGroup")
		        {
		            var objectToDelete = new DynamicContentGroup {ID = deleteData.ID};
                    DynamicContentGroupTable.Attach(objectToDelete);
                    DynamicContentGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "DynamicContent")
		        {
		            var objectToDelete = new DynamicContent {ID = deleteData.ID};
                    DynamicContentTable.Attach(objectToDelete);
                    DynamicContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AttachedToObject")
		        {
		            var objectToDelete = new AttachedToObject {ID = deleteData.ID};
                    AttachedToObjectTable.Attach(objectToDelete);
                    AttachedToObjectTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Comment")
		        {
		            var objectToDelete = new Comment {ID = deleteData.ID};
                    CommentTable.Attach(objectToDelete);
                    CommentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Selection")
		        {
		            var objectToDelete = new Selection {ID = deleteData.ID};
                    SelectionTable.Attach(objectToDelete);
                    SelectionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "TextContent")
		        {
		            var objectToDelete = new TextContent {ID = deleteData.ID};
                    TextContentTable.Attach(objectToDelete);
                    TextContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Blog")
		        {
		            var objectToDelete = new Blog {ID = deleteData.ID};
                    BlogTable.Attach(objectToDelete);
                    BlogTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "BlogIndexGroup")
		        {
		            var objectToDelete = new BlogIndexGroup {ID = deleteData.ID};
                    BlogIndexGroupTable.Attach(objectToDelete);
                    BlogIndexGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CalendarIndex")
		        {
		            var objectToDelete = new CalendarIndex {ID = deleteData.ID};
                    CalendarIndexTable.Attach(objectToDelete);
                    CalendarIndexTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Filter")
		        {
		            var objectToDelete = new Filter {ID = deleteData.ID};
                    FilterTable.Attach(objectToDelete);
                    FilterTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Calendar")
		        {
		            var objectToDelete = new Calendar {ID = deleteData.ID};
                    CalendarTable.Attach(objectToDelete);
                    CalendarTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Map")
		        {
		            var objectToDelete = new Map {ID = deleteData.ID};
                    MapTable.Attach(objectToDelete);
                    MapTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "MapIndexCollection")
		        {
		            var objectToDelete = new MapIndexCollection {ID = deleteData.ID};
                    MapIndexCollectionTable.Attach(objectToDelete);
                    MapIndexCollectionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "MapResult")
		        {
		            var objectToDelete = new MapResult {ID = deleteData.ID};
                    MapResultTable.Attach(objectToDelete);
                    MapResultTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "MapResultsCollection")
		        {
		            var objectToDelete = new MapResultsCollection {ID = deleteData.ID};
                    MapResultsCollectionTable.Attach(objectToDelete);
                    MapResultsCollectionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Video")
		        {
		            var objectToDelete = new Video {ID = deleteData.ID};
                    VideoTable.Attach(objectToDelete);
                    VideoTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Image")
		        {
		            var objectToDelete = new Image {ID = deleteData.ID};
                    ImageTable.Attach(objectToDelete);
                    ImageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "BinaryFile")
		        {
		            var objectToDelete = new BinaryFile {ID = deleteData.ID};
                    BinaryFileTable.Attach(objectToDelete);
                    BinaryFileTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ImageGroup")
		        {
		            var objectToDelete = new ImageGroup {ID = deleteData.ID};
                    ImageGroupTable.Attach(objectToDelete);
                    ImageGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "VideoGroup")
		        {
		            var objectToDelete = new VideoGroup {ID = deleteData.ID};
                    VideoGroupTable.Attach(objectToDelete);
                    VideoGroupTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Tooltip")
		        {
		            var objectToDelete = new Tooltip {ID = deleteData.ID};
                    TooltipTable.Attach(objectToDelete);
                    TooltipTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SocialPanel")
		        {
		            var objectToDelete = new SocialPanel {ID = deleteData.ID};
                    SocialPanelTable.Attach(objectToDelete);
                    SocialPanelTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Longitude")
		        {
		            var objectToDelete = new Longitude {ID = deleteData.ID};
                    LongitudeTable.Attach(objectToDelete);
                    LongitudeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Latitude")
		        {
		            var objectToDelete = new Latitude {ID = deleteData.ID};
                    LatitudeTable.Attach(objectToDelete);
                    LatitudeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Location")
		        {
		            var objectToDelete = new Location {ID = deleteData.ID};
                    LocationTable.Attach(objectToDelete);
                    LocationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Date")
		        {
		            var objectToDelete = new Date {ID = deleteData.ID};
                    DateTable.Attach(objectToDelete);
                    DateTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Sex")
		        {
		            var objectToDelete = new Sex {ID = deleteData.ID};
                    SexTable.Attach(objectToDelete);
                    SexTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "OBSAddress")
		        {
		            var objectToDelete = new OBSAddress {ID = deleteData.ID};
                    OBSAddressTable.Attach(objectToDelete);
                    OBSAddressTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Identity")
		        {
		            var objectToDelete = new Identity {ID = deleteData.ID};
                    IdentityTable.Attach(objectToDelete);
                    IdentityTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "ImageVideoSoundVectorRaw")
		        {
		            var objectToDelete = new ImageVideoSoundVectorRaw {ID = deleteData.ID};
                    ImageVideoSoundVectorRawTable.Attach(objectToDelete);
                    ImageVideoSoundVectorRawTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "CategoryContainer")
		        {
		            var objectToDelete = new CategoryContainer {ID = deleteData.ID};
                    CategoryContainerTable.Attach(objectToDelete);
                    CategoryContainerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Category")
		        {
		            var objectToDelete = new Category {ID = deleteData.ID};
                    CategoryTable.Attach(objectToDelete);
                    CategoryTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Subscription")
		        {
		            var objectToDelete = new Subscription {ID = deleteData.ID};
                    SubscriptionTable.Attach(objectToDelete);
                    SubscriptionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "QueueEnvelope")
		        {
		            var objectToDelete = new QueueEnvelope {ID = deleteData.ID};
                    QueueEnvelopeTable.Attach(objectToDelete);
                    QueueEnvelopeTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "OperationRequest")
		        {
		            var objectToDelete = new OperationRequest {ID = deleteData.ID};
                    OperationRequestTable.Attach(objectToDelete);
                    OperationRequestTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SubscriptionChainRequestMessage")
		        {
		            var objectToDelete = new SubscriptionChainRequestMessage {ID = deleteData.ID};
                    SubscriptionChainRequestMessageTable.Attach(objectToDelete);
                    SubscriptionChainRequestMessageTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SubscriptionChainRequestContent")
		        {
		            var objectToDelete = new SubscriptionChainRequestContent {ID = deleteData.ID};
                    SubscriptionChainRequestContentTable.Attach(objectToDelete);
                    SubscriptionChainRequestContentTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SubscriptionTarget")
		        {
		            var objectToDelete = new SubscriptionTarget {ID = deleteData.ID};
                    SubscriptionTargetTable.Attach(objectToDelete);
                    SubscriptionTargetTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "DeleteEntireOwnerOperation")
		        {
		            var objectToDelete = new DeleteEntireOwnerOperation {ID = deleteData.ID};
                    DeleteEntireOwnerOperationTable.Attach(objectToDelete);
                    DeleteEntireOwnerOperationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "DeleteOwnerContentOperation")
		        {
		            var objectToDelete = new DeleteOwnerContentOperation {ID = deleteData.ID};
                    DeleteOwnerContentOperationTable.Attach(objectToDelete);
                    DeleteOwnerContentOperationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SystemError")
		        {
		            var objectToDelete = new SystemError {ID = deleteData.ID};
                    SystemErrorTable.Attach(objectToDelete);
                    SystemErrorTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SystemErrorItem")
		        {
		            var objectToDelete = new SystemErrorItem {ID = deleteData.ID};
                    SystemErrorItemTable.Attach(objectToDelete);
                    SystemErrorItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "InformationSource")
		        {
		            var objectToDelete = new InformationSource {ID = deleteData.ID};
                    InformationSourceTable.Attach(objectToDelete);
                    InformationSourceTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "RefreshDefaultViewsOperation")
		        {
		            var objectToDelete = new RefreshDefaultViewsOperation {ID = deleteData.ID};
                    RefreshDefaultViewsOperationTable.Attach(objectToDelete);
                    RefreshDefaultViewsOperationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "UpdateWebContentOperation")
		        {
		            var objectToDelete = new UpdateWebContentOperation {ID = deleteData.ID};
                    UpdateWebContentOperationTable.Attach(objectToDelete);
                    UpdateWebContentOperationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "UpdateWebContentHandlerItem")
		        {
		            var objectToDelete = new UpdateWebContentHandlerItem {ID = deleteData.ID};
                    UpdateWebContentHandlerItemTable.Attach(objectToDelete);
                    UpdateWebContentHandlerItemTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "PublishWebContentOperation")
		        {
		            var objectToDelete = new PublishWebContentOperation {ID = deleteData.ID};
                    PublishWebContentOperationTable.Attach(objectToDelete);
                    PublishWebContentOperationTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "SubscriberInput")
		        {
		            var objectToDelete = new SubscriberInput {ID = deleteData.ID};
                    SubscriberInputTable.Attach(objectToDelete);
                    SubscriberInputTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "Monitor")
		        {
		            var objectToDelete = new Monitor {ID = deleteData.ID};
                    MonitorTable.Attach(objectToDelete);
                    MonitorTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "IconTitleDescription")
		        {
		            var objectToDelete = new IconTitleDescription {ID = deleteData.ID};
                    IconTitleDescriptionTable.Attach(objectToDelete);
                    IconTitleDescriptionTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AboutAGIApplications")
		        {
		            var objectToDelete = new AboutAGIApplications {ID = deleteData.ID};
                    AboutAGIApplicationsTable.Attach(objectToDelete);
                    AboutAGIApplicationsTable.DeleteOnSubmit(objectToDelete);
		            return;
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
			public Table<TBRegisterContainer> TBRegisterContainerTable {
				get {
					return this.GetTable<TBRegisterContainer>();
				}
			}
			public Table<LoginProvider> LoginProviderTable {
				get {
					return this.GetTable<LoginProvider>();
				}
			}
			public Table<ContactOipContainer> ContactOipContainerTable {
				get {
					return this.GetTable<ContactOipContainer>();
				}
			}
			public Table<TBPRegisterEmail> TBPRegisterEmailTable {
				get {
					return this.GetTable<TBPRegisterEmail>();
				}
			}
			public Table<JavaScriptContainer> JavaScriptContainerTable {
				get {
					return this.GetTable<JavaScriptContainer>();
				}
			}
			public Table<JavascriptContainer> JavascriptContainerTable {
				get {
					return this.GetTable<JavascriptContainer>();
				}
			}
			public Table<FooterContainer> FooterContainerTable {
				get {
					return this.GetTable<FooterContainer>();
				}
			}
			public Table<NavigationContainer> NavigationContainerTable {
				get {
					return this.GetTable<NavigationContainer>();
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
			public Table<AccountIndex> AccountIndexTable {
				get {
					return this.GetTable<AccountIndex>();
				}
			}
			public Table<AccountModule> AccountModuleTable {
				get {
					return this.GetTable<AccountModule>();
				}
			}
			public Table<ImageGroupContainer> ImageGroupContainerTable {
				get {
					return this.GetTable<ImageGroupContainer>();
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
			public Table<AccountContent> AccountContentTable {
				get {
					return this.GetTable<AccountContent>();
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
			public Table<GroupedInformation> GroupedInformationTable {
				get {
					return this.GetTable<GroupedInformation>();
				}
			}
			public Table<ReferenceToInformation> ReferenceToInformationTable {
				get {
					return this.GetTable<ReferenceToInformation>();
				}
			}
			public Table<BlogContainer> BlogContainerTable {
				get {
					return this.GetTable<BlogContainer>();
				}
			}
			public Table<RecentBlogSummary> RecentBlogSummaryTable {
				get {
					return this.GetTable<RecentBlogSummary>();
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
			public Table<MapContainer> MapContainerTable {
				get {
					return this.GetTable<MapContainer>();
				}
			}
			public Table<MapMarker> MapMarkerTable {
				get {
					return this.GetTable<MapMarker>();
				}
			}
			public Table<CalendarContainer> CalendarContainerTable {
				get {
					return this.GetTable<CalendarContainer>();
				}
			}
			public Table<AboutContainer> AboutContainerTable {
				get {
					return this.GetTable<AboutContainer>();
				}
			}
			public Table<OBSAccountContainer> OBSAccountContainerTable {
				get {
					return this.GetTable<OBSAccountContainer>();
				}
			}
			public Table<ProjectContainer> ProjectContainerTable {
				get {
					return this.GetTable<ProjectContainer>();
				}
			}
			public Table<CourseContainer> CourseContainerTable {
				get {
					return this.GetTable<CourseContainer>();
				}
			}
			public Table<ContainerHeader> ContainerHeaderTable {
				get {
					return this.GetTable<ContainerHeader>();
				}
			}
			public Table<ActivitySummaryContainer> ActivitySummaryContainerTable {
				get {
					return this.GetTable<ActivitySummaryContainer>();
				}
			}
			public Table<ActivityIndex> ActivityIndexTable {
				get {
					return this.GetTable<ActivityIndex>();
				}
			}
			public Table<ActivityContainer> ActivityContainerTable {
				get {
					return this.GetTable<ActivityContainer>();
				}
			}
			public Table<Activity> ActivityTable {
				get {
					return this.GetTable<Activity>();
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
			public Table<Blog> BlogTable {
				get {
					return this.GetTable<Blog>();
				}
			}
			public Table<BlogIndexGroup> BlogIndexGroupTable {
				get {
					return this.GetTable<BlogIndexGroup>();
				}
			}
			public Table<CalendarIndex> CalendarIndexTable {
				get {
					return this.GetTable<CalendarIndex>();
				}
			}
			public Table<Filter> FilterTable {
				get {
					return this.GetTable<Filter>();
				}
			}
			public Table<Calendar> CalendarTable {
				get {
					return this.GetTable<Calendar>();
				}
			}
			public Table<Map> MapTable {
				get {
					return this.GetTable<Map>();
				}
			}
			public Table<MapIndexCollection> MapIndexCollectionTable {
				get {
					return this.GetTable<MapIndexCollection>();
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
			public Table<ImageGroup> ImageGroupTable {
				get {
					return this.GetTable<ImageGroup>();
				}
			}
			public Table<VideoGroup> VideoGroupTable {
				get {
					return this.GetTable<VideoGroup>();
				}
			}
			public Table<Tooltip> TooltipTable {
				get {
					return this.GetTable<Tooltip>();
				}
			}
			public Table<SocialPanel> SocialPanelTable {
				get {
					return this.GetTable<SocialPanel>();
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
			public Table<Sex> SexTable {
				get {
					return this.GetTable<Sex>();
				}
			}
			public Table<OBSAddress> OBSAddressTable {
				get {
					return this.GetTable<OBSAddress>();
				}
			}
			public Table<Identity> IdentityTable {
				get {
					return this.GetTable<Identity>();
				}
			}
			public Table<ImageVideoSoundVectorRaw> ImageVideoSoundVectorRawTable {
				get {
					return this.GetTable<ImageVideoSoundVectorRaw>();
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
			public Table<Subscription> SubscriptionTable {
				get {
					return this.GetTable<Subscription>();
				}
			}
			public Table<QueueEnvelope> QueueEnvelopeTable {
				get {
					return this.GetTable<QueueEnvelope>();
				}
			}
			public Table<OperationRequest> OperationRequestTable {
				get {
					return this.GetTable<OperationRequest>();
				}
			}
			public Table<SubscriptionChainRequestMessage> SubscriptionChainRequestMessageTable {
				get {
					return this.GetTable<SubscriptionChainRequestMessage>();
				}
			}
			public Table<SubscriptionChainRequestContent> SubscriptionChainRequestContentTable {
				get {
					return this.GetTable<SubscriptionChainRequestContent>();
				}
			}
			public Table<SubscriptionTarget> SubscriptionTargetTable {
				get {
					return this.GetTable<SubscriptionTarget>();
				}
			}
			public Table<DeleteEntireOwnerOperation> DeleteEntireOwnerOperationTable {
				get {
					return this.GetTable<DeleteEntireOwnerOperation>();
				}
			}
			public Table<DeleteOwnerContentOperation> DeleteOwnerContentOperationTable {
				get {
					return this.GetTable<DeleteOwnerContentOperation>();
				}
			}
			public Table<SystemError> SystemErrorTable {
				get {
					return this.GetTable<SystemError>();
				}
			}
			public Table<SystemErrorItem> SystemErrorItemTable {
				get {
					return this.GetTable<SystemErrorItem>();
				}
			}
			public Table<InformationSource> InformationSourceTable {
				get {
					return this.GetTable<InformationSource>();
				}
			}
			public Table<RefreshDefaultViewsOperation> RefreshDefaultViewsOperationTable {
				get {
					return this.GetTable<RefreshDefaultViewsOperation>();
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
			public Table<PublishWebContentOperation> PublishWebContentOperationTable {
				get {
					return this.GetTable<PublishWebContentOperation>();
				}
			}
			public Table<SubscriberInput> SubscriberInputTable {
				get {
					return this.GetTable<SubscriberInput>();
				}
			}
			public Table<Monitor> MonitorTable {
				get {
					return this.GetTable<Monitor>();
				}
			}
			public Table<IconTitleDescription> IconTitleDescriptionTable {
				get {
					return this.GetTable<IconTitleDescription>();
				}
			}
			public Table<AboutAGIApplications> AboutAGIApplicationsTable {
				get {
					return this.GetTable<AboutAGIApplications>();
				}
			}
        }

    [Table(Name = "TBSystem")]
	[ScaffoldTable(true)]
	public class TBSystem : ITheBallDataContextStorable
	{
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
	public class WebPublishInfo : ITheBallDataContextStorable
	{
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
[ActivePublicationID] TEXT NULL
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


		public WebPublishInfo() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class PublicationPackage : ITheBallDataContextStorable
	{
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
	public class TBRLoginRoot : ITheBallDataContextStorable
	{
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
	public class TBRAccountRoot : ITheBallDataContextStorable
	{
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
	public class TBRGroupRoot : ITheBallDataContextStorable
	{
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
	public class TBRLoginGroupRoot : ITheBallDataContextStorable
	{
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
	public class TBREmailRoot : ITheBallDataContextStorable
	{
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
	public class TBAccount : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBAccount](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public TBAccount() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBAccountCollaborationGroup")]
	[ScaffoldTable(true)]
	public class TBAccountCollaborationGroup : ITheBallDataContextStorable
	{
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
	public class TBLoginInfo : ITheBallDataContextStorable
	{
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
	public class TBEmail : ITheBallDataContextStorable
	{
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
	public class TBCollaboratorRole : ITheBallDataContextStorable
	{
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
	public class TBCollaboratingGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratingGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL
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


		public TBCollaboratingGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "TBEmailValidation")]
	[ScaffoldTable(true)]
	public class TBEmailValidation : ITheBallDataContextStorable
	{
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
	public class TBMergeAccountConfirmation : ITheBallDataContextStorable
	{
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
	public class TBGroupJoinConfirmation : ITheBallDataContextStorable
	{
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
	public class TBDeviceJoinConfirmation : ITheBallDataContextStorable
	{
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
	public class TBInformationInputConfirmation : ITheBallDataContextStorable
	{
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
	public class TBInformationOutputConfirmation : ITheBallDataContextStorable
	{
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
    [Table(Name = "TBRegisterContainer")]
	[ScaffoldTable(true)]
	public class TBRegisterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRegisterContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[ReturnUrl] TEXT NOT NULL
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


		public TBRegisterContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string ReturnUrl { get; set; }
		// private string _unmodified_ReturnUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ReturnUrl == null)
				ReturnUrl = string.Empty;
		}
	}
    [Table(Name = "LoginProvider")]
	[ScaffoldTable(true)]
	public class LoginProvider : ITheBallDataContextStorable
	{
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
    [Table(Name = "ContactOipContainer")]
	[ScaffoldTable(true)]
	public class ContactOipContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContactOipContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OIPModeratorGroupID] TEXT NOT NULL
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


		public ContactOipContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OIPModeratorGroupID { get; set; }
		// private string _unmodified_OIPModeratorGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(OIPModeratorGroupID == null)
				OIPModeratorGroupID = string.Empty;
		}
	}
    [Table(Name = "TBPRegisterEmail")]
	[ScaffoldTable(true)]
	public class TBPRegisterEmail : ITheBallDataContextStorable
	{
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
    [Table(Name = "JavaScriptContainer")]
	[ScaffoldTable(true)]
	public class JavaScriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [JavaScriptContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HtmlContent] TEXT NOT NULL
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


		public JavaScriptContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(HtmlContent == null)
				HtmlContent = string.Empty;
		}
	}
    [Table(Name = "JavascriptContainer")]
	[ScaffoldTable(true)]
	public class JavascriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [JavascriptContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HtmlContent] TEXT NOT NULL
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


		public JavascriptContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(HtmlContent == null)
				HtmlContent = string.Empty;
		}
	}
    [Table(Name = "FooterContainer")]
	[ScaffoldTable(true)]
	public class FooterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [FooterContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HtmlContent] TEXT NOT NULL
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


		public FooterContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(HtmlContent == null)
				HtmlContent = string.Empty;
		}
	}
    [Table(Name = "NavigationContainer")]
	[ScaffoldTable(true)]
	public class NavigationContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NavigationContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Dummy] TEXT NOT NULL
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


		public NavigationContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Dummy { get; set; }
		// private string _unmodified_Dummy;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Dummy == null)
				Dummy = string.Empty;
		}
	}
    [Table(Name = "AccountSummary")]
	[ScaffoldTable(true)]
	public class AccountSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IntroductionID] TEXT NULL, 
[ActivitySummaryID] TEXT NULL, 
[GroupSummaryID] TEXT NULL
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


		public AccountSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

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
			public string ActivitySummaryID { get; set; }
			private EntityRef< ActivitySummaryContainer > _ActivitySummary;
			[Association(Storage = "_ActivitySummary", ThisKey = "ActivitySummaryID")]
			public ActivitySummaryContainer ActivitySummary
			{
				get { return this._ActivitySummary.Entity; }
				set { this._ActivitySummary.Entity = value; }
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
	public class AccountContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[AccountIndexID] TEXT NULL, 
[AccountModuleID] TEXT NULL, 
[AccountSummaryID] TEXT NULL
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


		public AccountContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}

			[Column]
			public string AccountIndexID { get; set; }
			private EntityRef< AccountIndex > _AccountIndex;
			[Association(Storage = "_AccountIndex", ThisKey = "AccountIndexID")]
			public AccountIndex AccountIndex
			{
				get { return this._AccountIndex.Entity; }
				set { this._AccountIndex.Entity = value; }
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
    [Table(Name = "AccountIndex")]
	[ScaffoldTable(true)]
	public class AccountIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
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


		public AccountIndex() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "AccountModule")]
	[ScaffoldTable(true)]
	public class AccountModule : ITheBallDataContextStorable
	{
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
[RolesID] TEXT NULL
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


		public AccountModule() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ImageGroupContainer")]
	[ScaffoldTable(true)]
	public class ImageGroupContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageGroupContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public ImageGroupContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LocationContainer")]
	[ScaffoldTable(true)]
	public class LocationContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LocationContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public LocationContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddressAndLocation")]
	[ScaffoldTable(true)]
	public class AddressAndLocation : ITheBallDataContextStorable
	{
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
	public class StreetAddress : ITheBallDataContextStorable
	{
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
    [Table(Name = "AccountContent")]
	[ScaffoldTable(true)]
	public class AccountContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Dummy] TEXT NOT NULL
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


		public AccountContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Dummy { get; set; }
		// private string _unmodified_Dummy;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Dummy == null)
				Dummy = string.Empty;
		}
	}
    [Table(Name = "AccountProfile")]
	[ScaffoldTable(true)]
	public class AccountProfile : ITheBallDataContextStorable
	{
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
	public class AccountSecurity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountSecurity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public AccountSecurity() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountRoles")]
	[ScaffoldTable(true)]
	public class AccountRoles : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountRoles](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[OrganizationsImPartOf] TEXT NOT NULL
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


		public AccountRoles() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class PersonalInfoVisibility : ITheBallDataContextStorable
	{
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
    [Table(Name = "GroupedInformation")]
	[ScaffoldTable(true)]
	public class GroupedInformation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupedInformation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[GroupName] TEXT NOT NULL
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


		public GroupedInformation() 
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
    [Table(Name = "ReferenceToInformation")]
	[ScaffoldTable(true)]
	public class ReferenceToInformation : ITheBallDataContextStorable
	{
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
    [Table(Name = "BlogContainer")]
	[ScaffoldTable(true)]
	public class BlogContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BlogContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[FeaturedBlogID] TEXT NULL, 
[RecentBlogSummaryID] TEXT NULL, 
[BlogIndexGroupID] TEXT NULL
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


		public BlogContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}

			[Column]
			public string FeaturedBlogID { get; set; }
			private EntityRef< Blog > _FeaturedBlog;
			[Association(Storage = "_FeaturedBlog", ThisKey = "FeaturedBlogID")]
			public Blog FeaturedBlog
			{
				get { return this._FeaturedBlog.Entity; }
				set { this._FeaturedBlog.Entity = value; }
			}

			[Column]
			public string RecentBlogSummaryID { get; set; }
			private EntityRef< RecentBlogSummary > _RecentBlogSummary;
			[Association(Storage = "_RecentBlogSummary", ThisKey = "RecentBlogSummaryID")]
			public RecentBlogSummary RecentBlogSummary
			{
				get { return this._RecentBlogSummary.Entity; }
				set { this._RecentBlogSummary.Entity = value; }
			}

			[Column]
			public string BlogIndexGroupID { get; set; }
			private EntityRef< BlogIndexGroup > _BlogIndexGroup;
			[Association(Storage = "_BlogIndexGroup", ThisKey = "BlogIndexGroupID")]
			public BlogIndexGroup BlogIndexGroup
			{
				get { return this._BlogIndexGroup.Entity; }
				set { this._BlogIndexGroup.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RecentBlogSummary")]
	[ScaffoldTable(true)]
	public class RecentBlogSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RecentBlogSummary](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IntroductionID] TEXT NULL
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


		public RecentBlogSummary() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string IntroductionID { get; set; }
			private EntityRef< Introduction > _Introduction;
			[Association(Storage = "_Introduction", ThisKey = "IntroductionID")]
			public Introduction Introduction
			{
				get { return this._Introduction.Entity; }
				set { this._Introduction.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	[ScaffoldTable(true)]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NodeSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public NodeSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	[ScaffoldTable(true)]
	public class RenderedNode : ITheBallDataContextStorable
	{
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
[CategoryIDList] TEXT NOT NULL
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


		public RenderedNode() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
        [ScaffoldColumn(true)]
		public string CategoryIDList { get; set; }
		// private string _unmodified_CategoryIDList;
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
	public class ShortTextObject : ITheBallDataContextStorable
	{
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
	public class LongTextObject : ITheBallDataContextStorable
	{
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
    [Table(Name = "MapContainer")]
	[ScaffoldTable(true)]
	public class MapContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[MapFeaturedID] TEXT NULL, 
[MapIndexCollectionID] TEXT NULL
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


		public MapContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}

			[Column]
			public string MapFeaturedID { get; set; }
			private EntityRef< Map > _MapFeatured;
			[Association(Storage = "_MapFeatured", ThisKey = "MapFeaturedID")]
			public Map MapFeatured
			{
				get { return this._MapFeatured.Entity; }
				set { this._MapFeatured.Entity = value; }
			}

			[Column]
			public string MapIndexCollectionID { get; set; }
			private EntityRef< MapIndexCollection > _MapIndexCollection;
			[Association(Storage = "_MapIndexCollection", ThisKey = "MapIndexCollectionID")]
			public MapIndexCollection MapIndexCollection
			{
				get { return this._MapIndexCollection.Entity; }
				set { this._MapIndexCollection.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapMarker")]
	[ScaffoldTable(true)]
	public class MapMarker : ITheBallDataContextStorable
	{
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
    [Table(Name = "CalendarContainer")]
	[ScaffoldTable(true)]
	public class CalendarContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CalendarContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CalendarContainerHeaderID] TEXT NULL, 
[CalendarFeaturedID] TEXT NULL, 
[CalendarIndexCollectionID] TEXT NULL
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


		public CalendarContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string CalendarContainerHeaderID { get; set; }
			private EntityRef< ContainerHeader > _CalendarContainerHeader;
			[Association(Storage = "_CalendarContainerHeader", ThisKey = "CalendarContainerHeaderID")]
			public ContainerHeader CalendarContainerHeader
			{
				get { return this._CalendarContainerHeader.Entity; }
				set { this._CalendarContainerHeader.Entity = value; }
			}

			[Column]
			public string CalendarFeaturedID { get; set; }
			private EntityRef< Calendar > _CalendarFeatured;
			[Association(Storage = "_CalendarFeatured", ThisKey = "CalendarFeaturedID")]
			public Calendar CalendarFeatured
			{
				get { return this._CalendarFeatured.Entity; }
				set { this._CalendarFeatured.Entity = value; }
			}

			[Column]
			public string CalendarIndexCollectionID { get; set; }
			private EntityRef< CalendarIndex > _CalendarIndexCollection;
			[Association(Storage = "_CalendarIndexCollection", ThisKey = "CalendarIndexCollectionID")]
			public CalendarIndex CalendarIndexCollection
			{
				get { return this._CalendarIndexCollection.Entity; }
				set { this._CalendarIndexCollection.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AboutContainer")]
	[ScaffoldTable(true)]
	public class AboutContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AboutContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[MainImageID] TEXT NULL, 
[HeaderID] TEXT NULL, 
[Excerpt] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[ImageGroupID] TEXT NULL
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


		public AboutContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string MainImageID { get; set; }
			private EntityRef< Image > _MainImage;
			[Association(Storage = "_MainImage", ThisKey = "MainImageID")]
			public Image MainImage
			{
				get { return this._MainImage.Entity; }
				set { this._MainImage.Entity = value; }
			}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string Body { get; set; }
		// private string _unmodified_Body;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;
			[Column]
			public string ImageGroupID { get; set; }
			private EntityRef< ImageGroup > _ImageGroup;
			[Association(Storage = "_ImageGroup", ThisKey = "ImageGroupID")]
			public ImageGroup ImageGroup
			{
				get { return this._ImageGroup.Entity; }
				set { this._ImageGroup.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Body == null)
				Body = string.Empty;
			if(Author == null)
				Author = string.Empty;
		}
	}
    [Table(Name = "OBSAccountContainer")]
	[ScaffoldTable(true)]
	public class OBSAccountContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OBSAccountContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[AccountContainerHeaderID] TEXT NULL, 
[AccountFeaturedID] TEXT NULL, 
[AccountIndexCollectionID] TEXT NULL
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


		public OBSAccountContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string AccountContainerHeaderID { get; set; }
			private EntityRef< ContainerHeader > _AccountContainerHeader;
			[Association(Storage = "_AccountContainerHeader", ThisKey = "AccountContainerHeaderID")]
			public ContainerHeader AccountContainerHeader
			{
				get { return this._AccountContainerHeader.Entity; }
				set { this._AccountContainerHeader.Entity = value; }
			}

			[Column]
			public string AccountFeaturedID { get; set; }
			private EntityRef< Calendar > _AccountFeatured;
			[Association(Storage = "_AccountFeatured", ThisKey = "AccountFeaturedID")]
			public Calendar AccountFeatured
			{
				get { return this._AccountFeatured.Entity; }
				set { this._AccountFeatured.Entity = value; }
			}

			[Column]
			public string AccountIndexCollectionID { get; set; }
			private EntityRef< CalendarIndex > _AccountIndexCollection;
			[Association(Storage = "_AccountIndexCollection", ThisKey = "AccountIndexCollectionID")]
			public CalendarIndex AccountIndexCollection
			{
				get { return this._AccountIndexCollection.Entity; }
				set { this._AccountIndexCollection.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ProjectContainer")]
	[ScaffoldTable(true)]
	public class ProjectContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ProjectContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ProjectContainerHeaderID] TEXT NULL, 
[ProjectFeaturedID] TEXT NULL, 
[ProjectIndexCollectionID] TEXT NULL
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


		public ProjectContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string ProjectContainerHeaderID { get; set; }
			private EntityRef< ContainerHeader > _ProjectContainerHeader;
			[Association(Storage = "_ProjectContainerHeader", ThisKey = "ProjectContainerHeaderID")]
			public ContainerHeader ProjectContainerHeader
			{
				get { return this._ProjectContainerHeader.Entity; }
				set { this._ProjectContainerHeader.Entity = value; }
			}

			[Column]
			public string ProjectFeaturedID { get; set; }
			private EntityRef< Calendar > _ProjectFeatured;
			[Association(Storage = "_ProjectFeatured", ThisKey = "ProjectFeaturedID")]
			public Calendar ProjectFeatured
			{
				get { return this._ProjectFeatured.Entity; }
				set { this._ProjectFeatured.Entity = value; }
			}

			[Column]
			public string ProjectIndexCollectionID { get; set; }
			private EntityRef< CalendarIndex > _ProjectIndexCollection;
			[Association(Storage = "_ProjectIndexCollection", ThisKey = "ProjectIndexCollectionID")]
			public CalendarIndex ProjectIndexCollection
			{
				get { return this._ProjectIndexCollection.Entity; }
				set { this._ProjectIndexCollection.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CourseContainer")]
	[ScaffoldTable(true)]
	public class CourseContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CourseContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[CourseContainerHeaderID] TEXT NULL, 
[CourseFeaturedID] TEXT NULL, 
[CourseIndexCollectionID] TEXT NULL
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


		public CourseContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string CourseContainerHeaderID { get; set; }
			private EntityRef< ContainerHeader > _CourseContainerHeader;
			[Association(Storage = "_CourseContainerHeader", ThisKey = "CourseContainerHeaderID")]
			public ContainerHeader CourseContainerHeader
			{
				get { return this._CourseContainerHeader.Entity; }
				set { this._CourseContainerHeader.Entity = value; }
			}

			[Column]
			public string CourseFeaturedID { get; set; }
			private EntityRef< Calendar > _CourseFeatured;
			[Association(Storage = "_CourseFeatured", ThisKey = "CourseFeaturedID")]
			public Calendar CourseFeatured
			{
				get { return this._CourseFeatured.Entity; }
				set { this._CourseFeatured.Entity = value; }
			}

			[Column]
			public string CourseIndexCollectionID { get; set; }
			private EntityRef< CalendarIndex > _CourseIndexCollection;
			[Association(Storage = "_CourseIndexCollection", ThisKey = "CourseIndexCollectionID")]
			public CalendarIndex CourseIndexCollection
			{
				get { return this._CourseIndexCollection.Entity; }
				set { this._CourseIndexCollection.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ContainerHeader")]
	[ScaffoldTable(true)]
	public class ContainerHeader : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContainerHeader](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL
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


		public ContainerHeader() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(SubTitle == null)
				SubTitle = string.Empty;
		}
	}
    [Table(Name = "ActivitySummaryContainer")]
	[ScaffoldTable(true)]
	public class ActivitySummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ActivitySummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[SummaryBody] TEXT NOT NULL, 
[IntroductionID] TEXT NULL, 
[ActivityIndexID] TEXT NULL
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


		public ActivitySummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
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
			public string ActivityIndexID { get; set; }
			private EntityRef< ActivityIndex > _ActivityIndex;
			[Association(Storage = "_ActivityIndex", ThisKey = "ActivityIndexID")]
			public ActivityIndex ActivityIndex
			{
				get { return this._ActivityIndex.Entity; }
				set { this._ActivityIndex.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryBody == null)
				SummaryBody = string.Empty;
		}
	}
    [Table(Name = "ActivityIndex")]
	[ScaffoldTable(true)]
	public class ActivityIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ActivityIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
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


		public ActivityIndex() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "ActivityContainer")]
	[ScaffoldTable(true)]
	public class ActivityContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ActivityContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[ActivityIndexID] TEXT NULL, 
[ActivityModuleID] TEXT NULL
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


		public ActivityContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
			}

			[Column]
			public string ActivityIndexID { get; set; }
			private EntityRef< ActivityIndex > _ActivityIndex;
			[Association(Storage = "_ActivityIndex", ThisKey = "ActivityIndexID")]
			public ActivityIndex ActivityIndex
			{
				get { return this._ActivityIndex.Entity; }
				set { this._ActivityIndex.Entity = value; }
			}

			[Column]
			public string ActivityModuleID { get; set; }
			private EntityRef< Activity > _ActivityModule;
			[Association(Storage = "_ActivityModule", ThisKey = "ActivityModuleID")]
			public Activity ActivityModule
			{
				get { return this._ActivityModule.Entity; }
				set { this._ActivityModule.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Activity")]
	[ScaffoldTable(true)]
	public class Activity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Activity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ReferenceToInformationID] TEXT NULL, 
[ProfileImageID] TEXT NULL, 
[IconImageID] TEXT NULL, 
[ActivityName] TEXT NOT NULL, 
[IntroductionID] TEXT NULL, 
[ContactPerson] TEXT NOT NULL, 
[StartingTime] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[IFrameSources] TEXT NOT NULL
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


		public Activity() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
		public string ActivityName { get; set; }
		// private string _unmodified_ActivityName;
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
        [ScaffoldColumn(true)]
		public string ContactPerson { get; set; }
		// private string _unmodified_ContactPerson;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime StartingTime { get; set; }
		// private DateTime _unmodified_StartingTime;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
        [ScaffoldColumn(true)]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ActivityName == null)
				ActivityName = string.Empty;
			if(ContactPerson == null)
				ContactPerson = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(Description == null)
				Description = string.Empty;
			if(IFrameSources == null)
				IFrameSources = string.Empty;
		}
	}
    [Table(Name = "Moderator")]
	[ScaffoldTable(true)]
	public class Moderator : ITheBallDataContextStorable
	{
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
	public class Collaborator : ITheBallDataContextStorable
	{
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
	public class GroupSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[SummaryBody] TEXT NOT NULL, 
[IntroductionID] TEXT NULL, 
[GroupSummaryIndexID] TEXT NULL
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


		public GroupSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
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

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryBody == null)
				SummaryBody = string.Empty;
		}
	}
    [Table(Name = "GroupContainer")]
	[ScaffoldTable(true)]
	public class GroupContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[HeaderID] TEXT NULL, 
[GroupIndexID] TEXT NULL, 
[GroupProfileID] TEXT NULL
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


		public GroupContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string HeaderID { get; set; }
			private EntityRef< ContainerHeader > _Header;
			[Association(Storage = "_Header", ThisKey = "HeaderID")]
			public ContainerHeader Header
			{
				get { return this._Header.Entity; }
				set { this._Header.Entity = value; }
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

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupIndex")]
	[ScaffoldTable(true)]
	public class GroupIndex : ITheBallDataContextStorable
	{
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
	public class AddAddressAndLocationInfo : ITheBallDataContextStorable
	{
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
	public class AddImageInfo : ITheBallDataContextStorable
	{
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
	public class AddImageGroupInfo : ITheBallDataContextStorable
	{
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
	public class AddEmailAddressInfo : ITheBallDataContextStorable
	{
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
	public class CreateGroupInfo : ITheBallDataContextStorable
	{
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
	public class AddActivityInfo : ITheBallDataContextStorable
	{
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
	public class AddBlogPostInfo : ITheBallDataContextStorable
	{
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
	public class AddCategoryInfo : ITheBallDataContextStorable
	{
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
	public class Group : ITheBallDataContextStorable
	{
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
[WwwSiteToPublishTo] TEXT NOT NULL
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


		public Group() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class Introduction : ITheBallDataContextStorable
	{
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
	public class ContentCategoryRank : ITheBallDataContextStorable
	{
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
	public class LinkToContent : ITheBallDataContextStorable
	{
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
[ImageDataID] TEXT NULL
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


		public LinkToContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class EmbeddedContent : ITheBallDataContextStorable
	{
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
[Description] TEXT NOT NULL
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


		public EmbeddedContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class DynamicContentGroup : ITheBallDataContextStorable
	{
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
	public class DynamicContent : ITheBallDataContextStorable
	{
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
	public class AttachedToObject : ITheBallDataContextStorable
	{
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
	public class Comment : ITheBallDataContextStorable
	{
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
	public class Selection : ITheBallDataContextStorable
	{
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
	public class TextContent : ITheBallDataContextStorable
	{
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
[SortOrderNumber] REAL NOT NULL, 
[IFrameSources] TEXT NOT NULL, 
[RawHtmlContent] TEXT NOT NULL
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


		public TextContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "Blog")]
	[ScaffoldTable(true)]
	public class Blog : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Blog](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ReferenceToInformationID] TEXT NULL, 
[ProfileImageID] TEXT NULL, 
[IconImageID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL, 
[IntroductionID] TEXT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[FeaturedImageID] TEXT NULL, 
[VideoGroupID] TEXT NULL, 
[Body] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[IFrameSources] TEXT NOT NULL
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


		public Blog() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;
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
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;
			[Column]
			public string FeaturedImageID { get; set; }
			private EntityRef< Image > _FeaturedImage;
			[Association(Storage = "_FeaturedImage", ThisKey = "FeaturedImageID")]
			public Image FeaturedImage
			{
				get { return this._FeaturedImage.Entity; }
				set { this._FeaturedImage.Entity = value; }
			}

			[Column]
			public string VideoGroupID { get; set; }
			private EntityRef< VideoGroup > _VideoGroup;
			[Association(Storage = "_VideoGroup", ThisKey = "VideoGroupID")]
			public VideoGroup VideoGroup
			{
				get { return this._VideoGroup.Entity; }
				set { this._VideoGroup.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string Body { get; set; }
		// private string _unmodified_Body;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(SubTitle == null)
				SubTitle = string.Empty;
			if(Author == null)
				Author = string.Empty;
			if(Body == null)
				Body = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(IFrameSources == null)
				IFrameSources = string.Empty;
		}
	}
    [Table(Name = "BlogIndexGroup")]
	[ScaffoldTable(true)]
	public class BlogIndexGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BlogIndexGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
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


		public BlogIndexGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "CalendarIndex")]
	[ScaffoldTable(true)]
	public class CalendarIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CalendarIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[IconID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
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


		public CalendarIndex() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "Filter")]
	[ScaffoldTable(true)]
	public class Filter : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Filter](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL
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


		public Filter() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "Calendar")]
	[ScaffoldTable(true)]
	public class Calendar : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Calendar](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL
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


		public Calendar() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "Map")]
	[ScaffoldTable(true)]
	public class Map : ITheBallDataContextStorable
	{
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
    [Table(Name = "MapIndexCollection")]
	[ScaffoldTable(true)]
	public class MapIndexCollection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapIndexCollection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public MapIndexCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapResult")]
	[ScaffoldTable(true)]
	public class MapResult : ITheBallDataContextStorable
	{
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
	public class MapResultsCollection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapResultsCollection](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public MapResultsCollection() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Video")]
	[ScaffoldTable(true)]
	public class Video : ITheBallDataContextStorable
	{
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
	public class Image : ITheBallDataContextStorable
	{
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
[Description] TEXT NOT NULL
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


		public Image() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class BinaryFile : ITheBallDataContextStorable
	{
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
[Description] TEXT NOT NULL
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


		public BinaryFile() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
    [Table(Name = "ImageGroup")]
	[ScaffoldTable(true)]
	public class ImageGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ReferenceToInformationID] TEXT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[FeaturedImageID] TEXT NULL
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


		public ImageGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
			[Column]
			public string FeaturedImageID { get; set; }
			private EntityRef< Image > _FeaturedImage;
			[Association(Storage = "_FeaturedImage", ThisKey = "FeaturedImageID")]
			public Image FeaturedImage
			{
				get { return this._FeaturedImage.Entity; }
				set { this._FeaturedImage.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "VideoGroup")]
	[ScaffoldTable(true)]
	public class VideoGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [VideoGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
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


		public VideoGroup() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "Tooltip")]
	[ScaffoldTable(true)]
	public class Tooltip : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Tooltip](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TooltipText] TEXT NOT NULL
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


		public Tooltip() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string TooltipText { get; set; }
		// private string _unmodified_TooltipText;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TooltipText == null)
				TooltipText = string.Empty;
		}
	}
    [Table(Name = "SocialPanel")]
	[ScaffoldTable(true)]
	public class SocialPanel : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SocialPanel](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SocialFilterID] TEXT NULL
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


		public SocialPanel() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string SocialFilterID { get; set; }
			private EntityRef< Filter > _SocialFilter;
			[Association(Storage = "_SocialFilter", ThisKey = "SocialFilterID")]
			public Filter SocialFilter
			{
				get { return this._SocialFilter.Entity; }
				set { this._SocialFilter.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Longitude")]
	[ScaffoldTable(true)]
	public class Longitude : ITheBallDataContextStorable
	{
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
	public class Latitude : ITheBallDataContextStorable
	{
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
	public class Location : ITheBallDataContextStorable
	{
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
	public class Date : ITheBallDataContextStorable
	{
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
    [Table(Name = "Sex")]
	[ScaffoldTable(true)]
	public class Sex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Sex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SexText] TEXT NOT NULL
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


		public Sex() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SexText { get; set; }
		// private string _unmodified_SexText;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SexText == null)
				SexText = string.Empty;
		}
	}
    [Table(Name = "OBSAddress")]
	[ScaffoldTable(true)]
	public class OBSAddress : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OBSAddress](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[StreetName] TEXT NOT NULL, 
[BuildingNumber] TEXT NOT NULL, 
[PostOfficeBox] TEXT NOT NULL, 
[PostalCode] TEXT NOT NULL, 
[Municipality] TEXT NOT NULL, 
[Region] TEXT NOT NULL, 
[Province] TEXT NOT NULL, 
[state] TEXT NOT NULL, 
[Country] TEXT NOT NULL, 
[Continent] TEXT NOT NULL
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


		public OBSAddress() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string StreetName { get; set; }
		// private string _unmodified_StreetName;

		[Column]
        [ScaffoldColumn(true)]
		public string BuildingNumber { get; set; }
		// private string _unmodified_BuildingNumber;

		[Column]
        [ScaffoldColumn(true)]
		public string PostOfficeBox { get; set; }
		// private string _unmodified_PostOfficeBox;

		[Column]
        [ScaffoldColumn(true)]
		public string PostalCode { get; set; }
		// private string _unmodified_PostalCode;

		[Column]
        [ScaffoldColumn(true)]
		public string Municipality { get; set; }
		// private string _unmodified_Municipality;

		[Column]
        [ScaffoldColumn(true)]
		public string Region { get; set; }
		// private string _unmodified_Region;

		[Column]
        [ScaffoldColumn(true)]
		public string Province { get; set; }
		// private string _unmodified_Province;

		[Column]
        [ScaffoldColumn(true)]
		public string state { get; set; }
		// private string _unmodified_state;

		[Column]
        [ScaffoldColumn(true)]
		public string Country { get; set; }
		// private string _unmodified_Country;

		[Column]
        [ScaffoldColumn(true)]
		public string Continent { get; set; }
		// private string _unmodified_Continent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(StreetName == null)
				StreetName = string.Empty;
			if(BuildingNumber == null)
				BuildingNumber = string.Empty;
			if(PostOfficeBox == null)
				PostOfficeBox = string.Empty;
			if(PostalCode == null)
				PostalCode = string.Empty;
			if(Municipality == null)
				Municipality = string.Empty;
			if(Region == null)
				Region = string.Empty;
			if(Province == null)
				Province = string.Empty;
			if(state == null)
				state = string.Empty;
			if(Country == null)
				Country = string.Empty;
			if(Continent == null)
				Continent = string.Empty;
		}
	}
    [Table(Name = "Identity")]
	[ScaffoldTable(true)]
	public class Identity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Identity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[Initials] TEXT NOT NULL, 
[SexID] TEXT NULL, 
[BirthdayID] TEXT NULL
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


		public Identity() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
		public string Initials { get; set; }
		// private string _unmodified_Initials;
			[Column]
			public string SexID { get; set; }
			private EntityRef< Sex > _Sex;
			[Association(Storage = "_Sex", ThisKey = "SexID")]
			public Sex Sex
			{
				get { return this._Sex.Entity; }
				set { this._Sex.Entity = value; }
			}

			[Column]
			public string BirthdayID { get; set; }
			private EntityRef< Date > _Birthday;
			[Association(Storage = "_Birthday", ThisKey = "BirthdayID")]
			public Date Birthday
			{
				get { return this._Birthday.Entity; }
				set { this._Birthday.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(FirstName == null)
				FirstName = string.Empty;
			if(LastName == null)
				LastName = string.Empty;
			if(Initials == null)
				Initials = string.Empty;
		}
	}
    [Table(Name = "ImageVideoSoundVectorRaw")]
	[ScaffoldTable(true)]
	public class ImageVideoSoundVectorRaw : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageVideoSoundVectorRaw](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Image] BLOB NOT NULL, 
[Video] BLOB NOT NULL, 
[Sound] BLOB NOT NULL, 
[Vector] TEXT NOT NULL, 
[Raw] BLOB NOT NULL
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


		public ImageVideoSoundVectorRaw() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public byte[] Image { get; set; }
		// private byte[] _unmodified_Image;

		[Column]
        [ScaffoldColumn(true)]
		public byte[] Video { get; set; }
		// private byte[] _unmodified_Video;

		[Column]
        [ScaffoldColumn(true)]
		public byte[] Sound { get; set; }
		// private byte[] _unmodified_Sound;

		[Column]
        [ScaffoldColumn(true)]
		public string Vector { get; set; }
		// private string _unmodified_Vector;

		[Column]
        [ScaffoldColumn(true)]
		public byte[] Raw { get; set; }
		// private byte[] _unmodified_Raw;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Vector == null)
				Vector = string.Empty;
		}
	}
    [Table(Name = "CategoryContainer")]
	[ScaffoldTable(true)]
	public class CategoryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CategoryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL

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


		public CategoryContainer() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
    [Table(Name = "Subscription")]
	[ScaffoldTable(true)]
	public class Subscription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Subscription](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Priority] INTEGER NOT NULL, 
[TargetRelativeLocation] TEXT NOT NULL, 
[TargetInformationObjectType] TEXT NOT NULL, 
[SubscriberRelativeLocation] TEXT NOT NULL, 
[SubscriberInformationObjectType] TEXT NOT NULL, 
[SubscriptionType] TEXT NOT NULL
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


		public Subscription() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public long Priority { get; set; }
		// private long _unmodified_Priority;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetRelativeLocation { get; set; }
		// private string _unmodified_TargetRelativeLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetInformationObjectType { get; set; }
		// private string _unmodified_TargetInformationObjectType;

		[Column]
        [ScaffoldColumn(true)]
		public string SubscriberRelativeLocation { get; set; }
		// private string _unmodified_SubscriberRelativeLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string SubscriberInformationObjectType { get; set; }
		// private string _unmodified_SubscriberInformationObjectType;

		[Column]
        [ScaffoldColumn(true)]
		public string SubscriptionType { get; set; }
		// private string _unmodified_SubscriptionType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TargetRelativeLocation == null)
				TargetRelativeLocation = string.Empty;
			if(TargetInformationObjectType == null)
				TargetInformationObjectType = string.Empty;
			if(SubscriberRelativeLocation == null)
				SubscriberRelativeLocation = string.Empty;
			if(SubscriberInformationObjectType == null)
				SubscriberInformationObjectType = string.Empty;
			if(SubscriptionType == null)
				SubscriptionType = string.Empty;
		}
	}
    [Table(Name = "QueueEnvelope")]
	[ScaffoldTable(true)]
	public class QueueEnvelope : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueueEnvelope](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ActiveContainerName] TEXT NOT NULL, 
[OwnerPrefix] TEXT NOT NULL, 
[CurrentRetryCount] INTEGER NOT NULL, 
[SingleOperationID] TEXT NULL, 
[ErrorContentID] TEXT NULL
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


		public QueueEnvelope() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ActiveContainerName { get; set; }
		// private string _unmodified_ActiveContainerName;

		[Column]
        [ScaffoldColumn(true)]
		public string OwnerPrefix { get; set; }
		// private string _unmodified_OwnerPrefix;

		[Column]
        [ScaffoldColumn(true)]
		public long CurrentRetryCount { get; set; }
		// private long _unmodified_CurrentRetryCount;
			[Column]
			public string SingleOperationID { get; set; }
			private EntityRef< OperationRequest > _SingleOperation;
			[Association(Storage = "_SingleOperation", ThisKey = "SingleOperationID")]
			public OperationRequest SingleOperation
			{
				get { return this._SingleOperation.Entity; }
				set { this._SingleOperation.Entity = value; }
			}

			[Column]
			public string ErrorContentID { get; set; }
			private EntityRef< SystemError > _ErrorContent;
			[Association(Storage = "_ErrorContent", ThisKey = "ErrorContentID")]
			public SystemError ErrorContent
			{
				get { return this._ErrorContent.Entity; }
				set { this._ErrorContent.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ActiveContainerName == null)
				ActiveContainerName = string.Empty;
			if(OwnerPrefix == null)
				OwnerPrefix = string.Empty;
		}
	}
    [Table(Name = "OperationRequest")]
	[ScaffoldTable(true)]
	public class OperationRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OperationRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SubscriberNotificationID] TEXT NULL, 
[SubscriptionChainRequestID] TEXT NULL, 
[UpdateWebContentOperationID] TEXT NULL, 
[RefreshDefaultViewsOperationID] TEXT NULL, 
[DeleteEntireOwnerID] TEXT NULL, 
[DeleteOwnerContentID] TEXT NULL, 
[PublishWebContentID] TEXT NULL, 
[ProcessIDToExecute] TEXT NOT NULL
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


		public OperationRequest() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string SubscriberNotificationID { get; set; }
			private EntityRef< Subscription > _SubscriberNotification;
			[Association(Storage = "_SubscriberNotification", ThisKey = "SubscriberNotificationID")]
			public Subscription SubscriberNotification
			{
				get { return this._SubscriberNotification.Entity; }
				set { this._SubscriberNotification.Entity = value; }
			}

			[Column]
			public string SubscriptionChainRequestID { get; set; }
			private EntityRef< SubscriptionChainRequestMessage > _SubscriptionChainRequest;
			[Association(Storage = "_SubscriptionChainRequest", ThisKey = "SubscriptionChainRequestID")]
			public SubscriptionChainRequestMessage SubscriptionChainRequest
			{
				get { return this._SubscriptionChainRequest.Entity; }
				set { this._SubscriptionChainRequest.Entity = value; }
			}

			[Column]
			public string UpdateWebContentOperationID { get; set; }
			private EntityRef< UpdateWebContentOperation > _UpdateWebContentOperation;
			[Association(Storage = "_UpdateWebContentOperation", ThisKey = "UpdateWebContentOperationID")]
			public UpdateWebContentOperation UpdateWebContentOperation
			{
				get { return this._UpdateWebContentOperation.Entity; }
				set { this._UpdateWebContentOperation.Entity = value; }
			}

			[Column]
			public string RefreshDefaultViewsOperationID { get; set; }
			private EntityRef< RefreshDefaultViewsOperation > _RefreshDefaultViewsOperation;
			[Association(Storage = "_RefreshDefaultViewsOperation", ThisKey = "RefreshDefaultViewsOperationID")]
			public RefreshDefaultViewsOperation RefreshDefaultViewsOperation
			{
				get { return this._RefreshDefaultViewsOperation.Entity; }
				set { this._RefreshDefaultViewsOperation.Entity = value; }
			}

			[Column]
			public string DeleteEntireOwnerID { get; set; }
			private EntityRef< DeleteEntireOwnerOperation > _DeleteEntireOwner;
			[Association(Storage = "_DeleteEntireOwner", ThisKey = "DeleteEntireOwnerID")]
			public DeleteEntireOwnerOperation DeleteEntireOwner
			{
				get { return this._DeleteEntireOwner.Entity; }
				set { this._DeleteEntireOwner.Entity = value; }
			}

			[Column]
			public string DeleteOwnerContentID { get; set; }
			private EntityRef< DeleteOwnerContentOperation > _DeleteOwnerContent;
			[Association(Storage = "_DeleteOwnerContent", ThisKey = "DeleteOwnerContentID")]
			public DeleteOwnerContentOperation DeleteOwnerContent
			{
				get { return this._DeleteOwnerContent.Entity; }
				set { this._DeleteOwnerContent.Entity = value; }
			}

			[Column]
			public string PublishWebContentID { get; set; }
			private EntityRef< PublishWebContentOperation > _PublishWebContent;
			[Association(Storage = "_PublishWebContent", ThisKey = "PublishWebContentID")]
			public PublishWebContentOperation PublishWebContent
			{
				get { return this._PublishWebContent.Entity; }
				set { this._PublishWebContent.Entity = value; }
			}


		[Column]
        [ScaffoldColumn(true)]
		public string ProcessIDToExecute { get; set; }
		// private string _unmodified_ProcessIDToExecute;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ProcessIDToExecute == null)
				ProcessIDToExecute = string.Empty;
		}
	}
    [Table(Name = "SubscriptionChainRequestMessage")]
	[ScaffoldTable(true)]
	public class SubscriptionChainRequestMessage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionChainRequestMessage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ContentItemID] TEXT NOT NULL
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


		public SubscriptionChainRequestMessage() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ContentItemID { get; set; }
		// private string _unmodified_ContentItemID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ContentItemID == null)
				ContentItemID = string.Empty;
		}
	}
    [Table(Name = "SubscriptionChainRequestContent")]
	[ScaffoldTable(true)]
	public class SubscriptionChainRequestContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionChainRequestContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SubmitTime] TEXT NOT NULL, 
[ProcessingStartTime] TEXT NOT NULL, 
[ProcessingEndTimeInformationObjects] TEXT NOT NULL, 
[ProcessingEndTimeWebTemplatesRendering] TEXT NOT NULL, 
[ProcessingEndTime] TEXT NOT NULL
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


		public SubscriptionChainRequestContent() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public DateTime SubmitTime { get; set; }
		// private DateTime _unmodified_SubmitTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ProcessingStartTime { get; set; }
		// private DateTime _unmodified_ProcessingStartTime;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ProcessingEndTimeInformationObjects { get; set; }
		// private DateTime _unmodified_ProcessingEndTimeInformationObjects;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ProcessingEndTimeWebTemplatesRendering { get; set; }
		// private DateTime _unmodified_ProcessingEndTimeWebTemplatesRendering;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime ProcessingEndTime { get; set; }
		// private DateTime _unmodified_ProcessingEndTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SubscriptionTarget")]
	[ScaffoldTable(true)]
	public class SubscriptionTarget : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionTarget](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[BlobLocation] TEXT NOT NULL
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


		public SubscriptionTarget() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string BlobLocation { get; set; }
		// private string _unmodified_BlobLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(BlobLocation == null)
				BlobLocation = string.Empty;
		}
	}
    [Table(Name = "DeleteEntireOwnerOperation")]
	[ScaffoldTable(true)]
	public class DeleteEntireOwnerOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeleteEntireOwnerOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
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


		public DeleteEntireOwnerOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ContainerName { get; set; }
		// private string _unmodified_ContainerName;

		[Column]
        [ScaffoldColumn(true)]
		public string LocationPrefix { get; set; }
		// private string _unmodified_LocationPrefix;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ContainerName == null)
				ContainerName = string.Empty;
			if(LocationPrefix == null)
				LocationPrefix = string.Empty;
		}
	}
    [Table(Name = "DeleteOwnerContentOperation")]
	[ScaffoldTable(true)]
	public class DeleteOwnerContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeleteOwnerContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
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


		public DeleteOwnerContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ContainerName { get; set; }
		// private string _unmodified_ContainerName;

		[Column]
        [ScaffoldColumn(true)]
		public string LocationPrefix { get; set; }
		// private string _unmodified_LocationPrefix;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ContainerName == null)
				ContainerName = string.Empty;
			if(LocationPrefix == null)
				LocationPrefix = string.Empty;
		}
	}
    [Table(Name = "SystemError")]
	[ScaffoldTable(true)]
	public class SystemError : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SystemError](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ErrorTitle] TEXT NOT NULL, 
[OccurredAt] TEXT NOT NULL, 
[MessageContentID] TEXT NULL
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


		public SystemError() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ErrorTitle { get; set; }
		// private string _unmodified_ErrorTitle;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime OccurredAt { get; set; }
		// private DateTime _unmodified_OccurredAt;
			[Column]
			public string MessageContentID { get; set; }
			private EntityRef< QueueEnvelope > _MessageContent;
			[Association(Storage = "_MessageContent", ThisKey = "MessageContentID")]
			public QueueEnvelope MessageContent
			{
				get { return this._MessageContent.Entity; }
				set { this._MessageContent.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ErrorTitle == null)
				ErrorTitle = string.Empty;
		}
	}
    [Table(Name = "SystemErrorItem")]
	[ScaffoldTable(true)]
	public class SystemErrorItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SystemErrorItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ShortDescription] TEXT NOT NULL, 
[LongDescription] TEXT NOT NULL
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


		public SystemErrorItem() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ShortDescription { get; set; }
		// private string _unmodified_ShortDescription;

		[Column]
        [ScaffoldColumn(true)]
		public string LongDescription { get; set; }
		// private string _unmodified_LongDescription;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ShortDescription == null)
				ShortDescription = string.Empty;
			if(LongDescription == null)
				LongDescription = string.Empty;
		}
	}
    [Table(Name = "InformationSource")]
	[ScaffoldTable(true)]
	public class InformationSource : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationSource](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SourceName] TEXT NOT NULL, 
[SourceLocation] TEXT NOT NULL, 
[SourceType] TEXT NOT NULL, 
[IsDynamic] INTEGER NOT NULL, 
[SourceInformationObjectType] TEXT NOT NULL, 
[SourceETag] TEXT NOT NULL, 
[SourceMD5] TEXT NOT NULL, 
[SourceLastModified] TEXT NOT NULL
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


		public InformationSource() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SourceName { get; set; }
		// private string _unmodified_SourceName;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceLocation { get; set; }
		// private string _unmodified_SourceLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceType { get; set; }
		// private string _unmodified_SourceType;

		[Column]
        [ScaffoldColumn(true)]
		public bool IsDynamic { get; set; }
		// private bool _unmodified_IsDynamic;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceInformationObjectType { get; set; }
		// private string _unmodified_SourceInformationObjectType;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceETag { get; set; }
		// private string _unmodified_SourceETag;

		[Column]
        [ScaffoldColumn(true)]
		public string SourceMD5 { get; set; }
		// private string _unmodified_SourceMD5;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime SourceLastModified { get; set; }
		// private DateTime _unmodified_SourceLastModified;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SourceName == null)
				SourceName = string.Empty;
			if(SourceLocation == null)
				SourceLocation = string.Empty;
			if(SourceType == null)
				SourceType = string.Empty;
			if(SourceInformationObjectType == null)
				SourceInformationObjectType = string.Empty;
			if(SourceETag == null)
				SourceETag = string.Empty;
			if(SourceMD5 == null)
				SourceMD5 = string.Empty;
		}
	}
    [Table(Name = "RefreshDefaultViewsOperation")]
	[ScaffoldTable(true)]
	public class RefreshDefaultViewsOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RefreshDefaultViewsOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[ViewLocation] TEXT NOT NULL, 
[TypeNameToRefresh] TEXT NOT NULL
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


		public RefreshDefaultViewsOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ViewLocation { get; set; }
		// private string _unmodified_ViewLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string TypeNameToRefresh { get; set; }
		// private string _unmodified_TypeNameToRefresh;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ViewLocation == null)
				ViewLocation = string.Empty;
			if(TypeNameToRefresh == null)
				TypeNameToRefresh = string.Empty;
		}
	}
    [Table(Name = "UpdateWebContentOperation")]
	[ScaffoldTable(true)]
	public class UpdateWebContentOperation : ITheBallDataContextStorable
	{
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
[RenderWhileSync] INTEGER NOT NULL
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


		public UpdateWebContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
	public class UpdateWebContentHandlerItem : ITheBallDataContextStorable
	{
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
    [Table(Name = "PublishWebContentOperation")]
	[ScaffoldTable(true)]
	public class PublishWebContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PublishWebContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[SourceContainerName] TEXT NOT NULL, 
[SourcePathRoot] TEXT NOT NULL, 
[SourceOwner] TEXT NOT NULL, 
[TargetContainerName] TEXT NOT NULL
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


		public PublishWebContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
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
		public string SourceOwner { get; set; }
		// private string _unmodified_SourceOwner;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetContainerName { get; set; }
		// private string _unmodified_TargetContainerName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SourceContainerName == null)
				SourceContainerName = string.Empty;
			if(SourcePathRoot == null)
				SourcePathRoot = string.Empty;
			if(SourceOwner == null)
				SourceOwner = string.Empty;
			if(TargetContainerName == null)
				TargetContainerName = string.Empty;
		}
	}
    [Table(Name = "SubscriberInput")]
	[ScaffoldTable(true)]
	public class SubscriberInput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriberInput](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[InputRelativeLocation] TEXT NOT NULL, 
[InformationObjectName] TEXT NOT NULL, 
[InformationItemName] TEXT NOT NULL, 
[SubscriberRelativeLocation] TEXT NOT NULL
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


		public SubscriberInput() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string InputRelativeLocation { get; set; }
		// private string _unmodified_InputRelativeLocation;

		[Column]
        [ScaffoldColumn(true)]
		public string InformationObjectName { get; set; }
		// private string _unmodified_InformationObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string InformationItemName { get; set; }
		// private string _unmodified_InformationItemName;

		[Column]
        [ScaffoldColumn(true)]
		public string SubscriberRelativeLocation { get; set; }
		// private string _unmodified_SubscriberRelativeLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(InputRelativeLocation == null)
				InputRelativeLocation = string.Empty;
			if(InformationObjectName == null)
				InformationObjectName = string.Empty;
			if(InformationItemName == null)
				InformationItemName = string.Empty;
			if(SubscriberRelativeLocation == null)
				SubscriberRelativeLocation = string.Empty;
		}
	}
    [Table(Name = "Monitor")]
	[ScaffoldTable(true)]
	public class Monitor : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Monitor](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[TargetObjectName] TEXT NOT NULL, 
[TargetItemName] TEXT NOT NULL, 
[MonitoringUtcTimeStampToStart] TEXT NOT NULL, 
[MonitoringCycleFrequencyUnit] TEXT NOT NULL, 
[MonitoringCycleEveryXthOfUnit] INTEGER NOT NULL, 
[CustomMonitoringCycleOperationName] TEXT NOT NULL, 
[OperationActionName] TEXT NOT NULL
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


		public Monitor() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
        [ScaffoldColumn(true)]
		public string TargetItemName { get; set; }
		// private string _unmodified_TargetItemName;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime MonitoringUtcTimeStampToStart { get; set; }
		// private DateTime _unmodified_MonitoringUtcTimeStampToStart;

		[Column]
        [ScaffoldColumn(true)]
		public string MonitoringCycleFrequencyUnit { get; set; }
		// private string _unmodified_MonitoringCycleFrequencyUnit;

		[Column]
        [ScaffoldColumn(true)]
		public long MonitoringCycleEveryXthOfUnit { get; set; }
		// private long _unmodified_MonitoringCycleEveryXthOfUnit;

		[Column]
        [ScaffoldColumn(true)]
		public string CustomMonitoringCycleOperationName { get; set; }
		// private string _unmodified_CustomMonitoringCycleOperationName;

		[Column]
        [ScaffoldColumn(true)]
		public string OperationActionName { get; set; }
		// private string _unmodified_OperationActionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(TargetObjectName == null)
				TargetObjectName = string.Empty;
			if(TargetItemName == null)
				TargetItemName = string.Empty;
			if(MonitoringCycleFrequencyUnit == null)
				MonitoringCycleFrequencyUnit = string.Empty;
			if(CustomMonitoringCycleOperationName == null)
				CustomMonitoringCycleOperationName = string.Empty;
			if(OperationActionName == null)
				OperationActionName = string.Empty;
		}
	}
    [Table(Name = "IconTitleDescription")]
	[ScaffoldTable(true)]
	public class IconTitleDescription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [IconTitleDescription](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[Icon] BLOB NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
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


		public IconTitleDescription() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}


		[Column]
        [ScaffoldColumn(true)]
		public byte[] Icon { get; set; }
		// private byte[] _unmodified_Icon;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(Title == null)
				Title = string.Empty;
			if(Description == null)
				Description = string.Empty;
		}
	}
    [Table(Name = "AboutAGIApplications")]
	[ScaffoldTable(true)]
	public class AboutAGIApplications : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AboutAGIApplications](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ETag] TEXT NOT NULL
, 
[BuiltForAnybodyID] TEXT NULL, 
[ForAllPeopleID] TEXT NULL
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


		public AboutAGIApplications() 
		{
			ID = Guid.NewGuid().ToString();
			ETag = String.Empty;
		}

			[Column]
			public string BuiltForAnybodyID { get; set; }
			private EntityRef< IconTitleDescription > _BuiltForAnybody;
			[Association(Storage = "_BuiltForAnybody", ThisKey = "BuiltForAnybodyID")]
			public IconTitleDescription BuiltForAnybody
			{
				get { return this._BuiltForAnybody.Entity; }
				set { this._BuiltForAnybody.Entity = value; }
			}

			[Column]
			public string ForAllPeopleID { get; set; }
			private EntityRef< IconTitleDescription > _ForAllPeople;
			[Association(Storage = "_ForAllPeople", ThisKey = "ForAllPeopleID")]
			public IconTitleDescription ForAllPeople
			{
				get { return this._ForAllPeople.Entity; }
				set { this._ForAllPeople.Entity = value; }
			}

        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
 } 
