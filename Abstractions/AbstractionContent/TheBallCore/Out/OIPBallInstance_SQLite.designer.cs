 


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
using System.ComponentModel.DataAnnotations;


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

		    public static SQLiteConnection CurrentConnection { get; set; }

		    public TheBallDataContext() : base(CurrentConnection)
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
				tableCreationCommands.Add(TBSystem.GetCreateTableSQL());
				tableCreationCommands.Add(WebPublishInfo.GetCreateTableSQL());
				tableCreationCommands.Add(PublicationPackage.GetCreateTableSQL());
				tableCreationCommands.Add(TBRLoginRoot.GetCreateTableSQL());
				tableCreationCommands.Add(TBRLoginGroupRoot.GetCreateTableSQL());
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
				tableCreationCommands.Add(AccountIndex.GetCreateTableSQL());
				tableCreationCommands.Add(StreetAddress.GetCreateTableSQL());
				tableCreationCommands.Add(AccountContent.GetCreateTableSQL());
				tableCreationCommands.Add(AccountProfile.GetCreateTableSQL());
				tableCreationCommands.Add(AccountRoles.GetCreateTableSQL());
				tableCreationCommands.Add(PersonalInfoVisibility.GetCreateTableSQL());
				tableCreationCommands.Add(GroupedInformation.GetCreateTableSQL());
				tableCreationCommands.Add(ReferenceToInformation.GetCreateTableSQL());
				tableCreationCommands.Add(RenderedNode.GetCreateTableSQL());
				tableCreationCommands.Add(ShortTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(LongTextObject.GetCreateTableSQL());
				tableCreationCommands.Add(MapMarker.GetCreateTableSQL());
				tableCreationCommands.Add(AboutContainer.GetCreateTableSQL());
				tableCreationCommands.Add(ContainerHeader.GetCreateTableSQL());
				tableCreationCommands.Add(ActivitySummaryContainer.GetCreateTableSQL());
				tableCreationCommands.Add(ActivityIndex.GetCreateTableSQL());
				tableCreationCommands.Add(Activity.GetCreateTableSQL());
				tableCreationCommands.Add(Moderator.GetCreateTableSQL());
				tableCreationCommands.Add(Collaborator.GetCreateTableSQL());
				tableCreationCommands.Add(GroupSummaryContainer.GetCreateTableSQL());
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
				tableCreationCommands.Add(Video.GetCreateTableSQL());
				tableCreationCommands.Add(Image.GetCreateTableSQL());
				tableCreationCommands.Add(BinaryFile.GetCreateTableSQL());
				tableCreationCommands.Add(ImageGroup.GetCreateTableSQL());
				tableCreationCommands.Add(VideoGroup.GetCreateTableSQL());
				tableCreationCommands.Add(Tooltip.GetCreateTableSQL());
				tableCreationCommands.Add(Longitude.GetCreateTableSQL());
				tableCreationCommands.Add(Latitude.GetCreateTableSQL());
				tableCreationCommands.Add(Location.GetCreateTableSQL());
				tableCreationCommands.Add(Date.GetCreateTableSQL());
				tableCreationCommands.Add(Sex.GetCreateTableSQL());
				tableCreationCommands.Add(OBSAddress.GetCreateTableSQL());
				tableCreationCommands.Add(Identity.GetCreateTableSQL());
				tableCreationCommands.Add(ImageVideoSoundVectorRaw.GetCreateTableSQL());
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
		            existingObject.PublishType = serializedObject.PublishType;
		            existingObject.PublishContainer = serializedObject.PublishContainer;
		            return;
		        } 
		        if (updateData.ObjectType == "PublicationPackage")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = PublicationPackageTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.DomainName = serializedObject.DomainName;
		            return;
		        } 
		        if (updateData.ObjectType == "TBRLoginGroupRoot")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBRLoginGroupRootTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Role = serializedObject.Role;
		            existingObject.GroupID = serializedObject.GroupID;
		            return;
		        } 
		        if (updateData.ObjectType == "TBAccountCollaborationGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = TBAccountCollaborationGroupTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.Email = serializedObject.Email;
		            existingObject.AccountID = serializedObject.AccountID;
		            existingObject.ValidUntil = serializedObject.ValidUntil;
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
		            existingObject.Dummy = serializedObject.Dummy;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountIndexTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "StreetAddress")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = StreetAddressTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
		            existingObject.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            existingObject.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            existingObject.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
		            return;
		        } 
		        if (updateData.ObjectType == "AccountRoles")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AccountRolesTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.Title = serializedObject.Title;
		            existingObject.URL = serializedObject.URL;
		            return;
		        } 
		        if (updateData.ObjectType == "RenderedNode")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = RenderedNodeTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.Content = serializedObject.Content;
		            return;
		        } 
		        if (updateData.ObjectType == "MapMarker")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = MapMarkerTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.IconUrl = serializedObject.IconUrl;
		            existingObject.MarkerSource = serializedObject.MarkerSource;
		            existingObject.CategoryName = serializedObject.CategoryName;
		            existingObject.LocationText = serializedObject.LocationText;
		            existingObject.PopupTitle = serializedObject.PopupTitle;
		            existingObject.PopupContent = serializedObject.PopupContent;
		            return;
		        } 
		        if (updateData.ObjectType == "AboutContainer")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.AboutContainer.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = AboutContainerTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.Body = serializedObject.Body;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            return;
		        } 
		        if (updateData.ObjectType == "ContainerHeader")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ContainerHeader.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ContainerHeaderTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.SummaryBody = serializedObject.SummaryBody;
		            return;
		        } 
		        if (updateData.ObjectType == "ActivityIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ActivityIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivityIndexTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Title = serializedObject.Title;
		            existingObject.Introduction = serializedObject.Introduction;
		            existingObject.Summary = serializedObject.Summary;
		            return;
		        } 
		        if (updateData.ObjectType == "Activity")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Activity.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ActivityTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.ActivityName = serializedObject.ActivityName;
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
		            existingObject.SummaryBody = serializedObject.SummaryBody;
		            return;
		        } 
		        if (updateData.ObjectType == "GroupIndex")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = GroupIndexTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.URL = serializedObject.URL;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
		            return;
		        } 
		        if (updateData.ObjectType == "EmbeddedContent")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = EmbeddedContentTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.HostName = serializedObject.HostName;
		            existingObject.ContentName = serializedObject.ContentName;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            existingObject.ElementQuery = serializedObject.ElementQuery;
		            existingObject.Content = serializedObject.Content;
		            existingObject.RawContent = serializedObject.RawContent;
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
		            existingObject.Title = serializedObject.Title;
		            existingObject.SubTitle = serializedObject.SubTitle;
		            existingObject.Published = serializedObject.Published;
		            existingObject.Author = serializedObject.Author;
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
		            existingObject.Title = serializedObject.Title;
		            return;
		        } 
		        if (updateData.ObjectType == "Video")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = VideoTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.OriginalFileName = serializedObject.OriginalFileName;
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
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
		            return;
		        } 
		        if (updateData.ObjectType == "VideoGroup")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.VideoGroup.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = VideoGroupTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.TooltipText = serializedObject.TooltipText;
		            return;
		        } 
		        if (updateData.ObjectType == "Longitude")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = LongitudeTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.LocationName = serializedObject.LocationName;
		            return;
		        } 
		        if (updateData.ObjectType == "Date")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = DateTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.FirstName = serializedObject.FirstName;
		            existingObject.LastName = serializedObject.LastName;
		            existingObject.Initials = serializedObject.Initials;
		            return;
		        } 
		        if (updateData.ObjectType == "ImageVideoSoundVectorRaw")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.ImageVideoSoundVectorRaw.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = ImageVideoSoundVectorRawTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.Image = serializedObject.Image;
		            existingObject.Video = serializedObject.Video;
		            existingObject.Sound = serializedObject.Sound;
		            existingObject.Vector = serializedObject.Vector;
		            existingObject.Raw = serializedObject.Raw;
		            return;
		        } 
		        if (updateData.ObjectType == "Category")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = CategoryTable.Single(item => item.ID == updateData.ObjectID);
		            existingObject.CategoryName = serializedObject.CategoryName;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Excerpt = serializedObject.Excerpt;
		            existingObject.ParentCategoryID = serializedObject.ParentCategoryID;
		            return;
		        } 
		        if (updateData.ObjectType == "Subscription")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.Subscription.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SubscriptionTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.ActiveContainerName = serializedObject.ActiveContainerName;
		            existingObject.OwnerPrefix = serializedObject.OwnerPrefix;
		            existingObject.CurrentRetryCount = serializedObject.CurrentRetryCount;
		            return;
		        } 
		        if (updateData.ObjectType == "OperationRequest")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.OperationRequest.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = OperationRequestTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.ErrorTitle = serializedObject.ErrorTitle;
		            existingObject.OccurredAt = serializedObject.OccurredAt;
		            return;
		        } 
		        if (updateData.ObjectType == "SystemErrorItem")
		        {
		            string currentFullStoragePath = Path.Combine(storageRootPath, updateData.CurrentStoragePath);
		            var serializedObject =
		                global::SER.AaltoGlobalImpact.OIP.SystemErrorItem.DeserializeFromXml(
		                    ContentStorage.GetContentAsString(currentFullStoragePath));
		            var existingObject = SystemErrorItemTable.Single(item => item.ID == updateData.ObjectID);
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
		            existingObject.Icon = serializedObject.Icon;
		            existingObject.Title = serializedObject.Title;
		            existingObject.Description = serializedObject.Description;
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
                    var objectToAdd = new TBSystem {ID = insertData.ObjectID};
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
                    var objectToAdd = new WebPublishInfo {ID = insertData.ObjectID};
		            objectToAdd.PublishType = serializedObject.PublishType;
		            objectToAdd.PublishContainer = serializedObject.PublishContainer;
					WebPublishInfoTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "PublicationPackage")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.PublicationPackage.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new PublicationPackage {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBRLoginRoot {ID = insertData.ObjectID};
		            objectToAdd.DomainName = serializedObject.DomainName;
					TBRLoginRootTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBRLoginGroupRoot")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBRLoginGroupRoot.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBRLoginGroupRoot {ID = insertData.ObjectID};
		            objectToAdd.Role = serializedObject.Role;
		            objectToAdd.GroupID = serializedObject.GroupID;
					TBRLoginGroupRootTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "TBAccountCollaborationGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.TBAccountCollaborationGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new TBAccountCollaborationGroup {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBLoginInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBEmail {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBCollaboratorRole {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBCollaboratingGroup {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBEmailValidation {ID = insertData.ObjectID};
		            objectToAdd.Email = serializedObject.Email;
		            objectToAdd.AccountID = serializedObject.AccountID;
		            objectToAdd.ValidUntil = serializedObject.ValidUntil;
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
                    var objectToAdd = new TBMergeAccountConfirmation {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBGroupJoinConfirmation {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBDeviceJoinConfirmation {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBInformationInputConfirmation {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBInformationOutputConfirmation {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBRegisterContainer {ID = insertData.ObjectID};
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
                    var objectToAdd = new LoginProvider {ID = insertData.ObjectID};
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
                    var objectToAdd = new ContactOipContainer {ID = insertData.ObjectID};
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
                    var objectToAdd = new TBPRegisterEmail {ID = insertData.ObjectID};
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
                    var objectToAdd = new JavaScriptContainer {ID = insertData.ObjectID};
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
                    var objectToAdd = new JavascriptContainer {ID = insertData.ObjectID};
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
                    var objectToAdd = new FooterContainer {ID = insertData.ObjectID};
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
                    var objectToAdd = new NavigationContainer {ID = insertData.ObjectID};
		            objectToAdd.Dummy = serializedObject.Dummy;
					NavigationContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountIndex {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					AccountIndexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "StreetAddress")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.StreetAddress.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new StreetAddress {ID = insertData.ObjectID};
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
                    var objectToAdd = new AccountContent {ID = insertData.ObjectID};
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
                    var objectToAdd = new AccountProfile {ID = insertData.ObjectID};
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
		            objectToAdd.IsSimplifiedAccount = serializedObject.IsSimplifiedAccount;
		            objectToAdd.SimplifiedAccountEmail = serializedObject.SimplifiedAccountEmail;
		            objectToAdd.SimplifiedAccountGroupID = serializedObject.SimplifiedAccountGroupID;
					AccountProfileTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AccountRoles")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AccountRoles.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AccountRoles {ID = insertData.ObjectID};
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
                    var objectToAdd = new PersonalInfoVisibility {ID = insertData.ObjectID};
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
                    var objectToAdd = new GroupedInformation {ID = insertData.ObjectID};
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
                    var objectToAdd = new ReferenceToInformation {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.URL = serializedObject.URL;
					ReferenceToInformationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "RenderedNode")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.RenderedNode.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new RenderedNode {ID = insertData.ObjectID};
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
                    var objectToAdd = new ShortTextObject {ID = insertData.ObjectID};
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
                    var objectToAdd = new LongTextObject {ID = insertData.ObjectID};
		            objectToAdd.Content = serializedObject.Content;
					LongTextObjectTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "MapMarker")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.MapMarker.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new MapMarker {ID = insertData.ObjectID};
		            objectToAdd.IconUrl = serializedObject.IconUrl;
		            objectToAdd.MarkerSource = serializedObject.MarkerSource;
		            objectToAdd.CategoryName = serializedObject.CategoryName;
		            objectToAdd.LocationText = serializedObject.LocationText;
		            objectToAdd.PopupTitle = serializedObject.PopupTitle;
		            objectToAdd.PopupContent = serializedObject.PopupContent;
					MapMarkerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "AboutContainer")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.AboutContainer.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new AboutContainer {ID = insertData.ObjectID};
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.Body = serializedObject.Body;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					AboutContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ContainerHeader")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ContainerHeader.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ContainerHeader {ID = insertData.ObjectID};
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
                    var objectToAdd = new ActivitySummaryContainer {ID = insertData.ObjectID};
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					ActivitySummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ActivityIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ActivityIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ActivityIndex {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Introduction = serializedObject.Introduction;
		            objectToAdd.Summary = serializedObject.Summary;
					ActivityIndexTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Activity")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Activity.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Activity {ID = insertData.ObjectID};
		            objectToAdd.ActivityName = serializedObject.ActivityName;
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
                    var objectToAdd = new Moderator {ID = insertData.ObjectID};
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
                    var objectToAdd = new Collaborator {ID = insertData.ObjectID};
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
                    var objectToAdd = new GroupSummaryContainer {ID = insertData.ObjectID};
		            objectToAdd.SummaryBody = serializedObject.SummaryBody;
					GroupSummaryContainerTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "GroupIndex")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.GroupIndex.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new GroupIndex {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddAddressAndLocationInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddImageInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddImageGroupInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddEmailAddressInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new CreateGroupInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddActivityInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddBlogPostInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new AddCategoryInfo {ID = insertData.ObjectID};
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
                    var objectToAdd = new Group {ID = insertData.ObjectID};
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
                    var objectToAdd = new Introduction {ID = insertData.ObjectID};
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
                    var objectToAdd = new ContentCategoryRank {ID = insertData.ObjectID};
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
                    var objectToAdd = new LinkToContent {ID = insertData.ObjectID};
		            objectToAdd.URL = serializedObject.URL;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
					LinkToContentTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "EmbeddedContent")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.EmbeddedContent.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new EmbeddedContent {ID = insertData.ObjectID};
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
                    var objectToAdd = new DynamicContentGroup {ID = insertData.ObjectID};
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
                    var objectToAdd = new DynamicContent {ID = insertData.ObjectID};
		            objectToAdd.HostName = serializedObject.HostName;
		            objectToAdd.ContentName = serializedObject.ContentName;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
		            objectToAdd.ElementQuery = serializedObject.ElementQuery;
		            objectToAdd.Content = serializedObject.Content;
		            objectToAdd.RawContent = serializedObject.RawContent;
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
                    var objectToAdd = new AttachedToObject {ID = insertData.ObjectID};
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
                    var objectToAdd = new Comment {ID = insertData.ObjectID};
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
                    var objectToAdd = new Selection {ID = insertData.ObjectID};
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
                    var objectToAdd = new TextContent {ID = insertData.ObjectID};
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
                    var objectToAdd = new Blog {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.SubTitle = serializedObject.SubTitle;
		            objectToAdd.Published = serializedObject.Published;
		            objectToAdd.Author = serializedObject.Author;
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
                    var objectToAdd = new BlogIndexGroup {ID = insertData.ObjectID};
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
                    var objectToAdd = new CalendarIndex {ID = insertData.ObjectID};
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
                    var objectToAdd = new Filter {ID = insertData.ObjectID};
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
                    var objectToAdd = new Calendar {ID = insertData.ObjectID};
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
                    var objectToAdd = new Map {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
					MapTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Video")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Video.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Video {ID = insertData.ObjectID};
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
                    var objectToAdd = new Image {ID = insertData.ObjectID};
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
                    var objectToAdd = new BinaryFile {ID = insertData.ObjectID};
		            objectToAdd.OriginalFileName = serializedObject.OriginalFileName;
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
                    var objectToAdd = new ImageGroup {ID = insertData.ObjectID};
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					ImageGroupTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "VideoGroup")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.VideoGroup.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new VideoGroup {ID = insertData.ObjectID};
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
                    var objectToAdd = new Tooltip {ID = insertData.ObjectID};
		            objectToAdd.TooltipText = serializedObject.TooltipText;
					TooltipTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Longitude")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Longitude.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Longitude {ID = insertData.ObjectID};
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
                    var objectToAdd = new Latitude {ID = insertData.ObjectID};
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
                    var objectToAdd = new Location {ID = insertData.ObjectID};
		            objectToAdd.LocationName = serializedObject.LocationName;
					LocationTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Date")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Date.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Date {ID = insertData.ObjectID};
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
                    var objectToAdd = new Sex {ID = insertData.ObjectID};
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
                    var objectToAdd = new OBSAddress {ID = insertData.ObjectID};
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
                    var objectToAdd = new Identity {ID = insertData.ObjectID};
		            objectToAdd.FirstName = serializedObject.FirstName;
		            objectToAdd.LastName = serializedObject.LastName;
		            objectToAdd.Initials = serializedObject.Initials;
					IdentityTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "ImageVideoSoundVectorRaw")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.ImageVideoSoundVectorRaw.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new ImageVideoSoundVectorRaw {ID = insertData.ObjectID};
		            objectToAdd.Image = serializedObject.Image;
		            objectToAdd.Video = serializedObject.Video;
		            objectToAdd.Sound = serializedObject.Sound;
		            objectToAdd.Vector = serializedObject.Vector;
		            objectToAdd.Raw = serializedObject.Raw;
					ImageVideoSoundVectorRawTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Category")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Category.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Category {ID = insertData.ObjectID};
		            objectToAdd.CategoryName = serializedObject.CategoryName;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Excerpt = serializedObject.Excerpt;
		            objectToAdd.ParentCategoryID = serializedObject.ParentCategoryID;
					CategoryTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "Subscription")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.Subscription.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new Subscription {ID = insertData.ObjectID};
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
                    var objectToAdd = new QueueEnvelope {ID = insertData.ObjectID};
		            objectToAdd.ActiveContainerName = serializedObject.ActiveContainerName;
		            objectToAdd.OwnerPrefix = serializedObject.OwnerPrefix;
		            objectToAdd.CurrentRetryCount = serializedObject.CurrentRetryCount;
					QueueEnvelopeTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "OperationRequest")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.OperationRequest.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new OperationRequest {ID = insertData.ObjectID};
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
                    var objectToAdd = new SubscriptionChainRequestMessage {ID = insertData.ObjectID};
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
                    var objectToAdd = new SubscriptionChainRequestContent {ID = insertData.ObjectID};
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
                    var objectToAdd = new SubscriptionTarget {ID = insertData.ObjectID};
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
                    var objectToAdd = new DeleteEntireOwnerOperation {ID = insertData.ObjectID};
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
                    var objectToAdd = new DeleteOwnerContentOperation {ID = insertData.ObjectID};
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
                    var objectToAdd = new SystemError {ID = insertData.ObjectID};
		            objectToAdd.ErrorTitle = serializedObject.ErrorTitle;
		            objectToAdd.OccurredAt = serializedObject.OccurredAt;
					SystemErrorTable.InsertOnSubmit(objectToAdd);
                    return;
                }
                if (insertData.ObjectType == "SystemErrorItem")
                {
                    string currentFullStoragePath = Path.Combine(storageRootPath, insertData.CurrentStoragePath);
                    var serializedObject =
                        global::SER.AaltoGlobalImpact.OIP.SystemErrorItem.DeserializeFromXml(
                            ContentStorage.GetContentAsString(currentFullStoragePath));
                    var objectToAdd = new SystemErrorItem {ID = insertData.ObjectID};
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
                    var objectToAdd = new InformationSource {ID = insertData.ObjectID};
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
                    var objectToAdd = new RefreshDefaultViewsOperation {ID = insertData.ObjectID};
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
                    var objectToAdd = new UpdateWebContentOperation {ID = insertData.ObjectID};
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
                    var objectToAdd = new UpdateWebContentHandlerItem {ID = insertData.ObjectID};
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
                    var objectToAdd = new PublishWebContentOperation {ID = insertData.ObjectID};
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
                    var objectToAdd = new SubscriberInput {ID = insertData.ObjectID};
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
                    var objectToAdd = new Monitor {ID = insertData.ObjectID};
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
                    var objectToAdd = new IconTitleDescription {ID = insertData.ObjectID};
		            objectToAdd.Icon = serializedObject.Icon;
		            objectToAdd.Title = serializedObject.Title;
		            objectToAdd.Description = serializedObject.Description;
					IconTitleDescriptionTable.InsertOnSubmit(objectToAdd);
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
		        if (deleteData.ObjectType == "TBRLoginGroupRoot")
		        {
		            var objectToDelete = new TBRLoginGroupRoot {ID = deleteData.ID};
                    TBRLoginGroupRootTable.Attach(objectToDelete);
                    TBRLoginGroupRootTable.DeleteOnSubmit(objectToDelete);
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
		        if (deleteData.ObjectType == "AccountIndex")
		        {
		            var objectToDelete = new AccountIndex {ID = deleteData.ID};
                    AccountIndexTable.Attach(objectToDelete);
                    AccountIndexTable.DeleteOnSubmit(objectToDelete);
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
		        if (deleteData.ObjectType == "MapMarker")
		        {
		            var objectToDelete = new MapMarker {ID = deleteData.ID};
                    MapMarkerTable.Attach(objectToDelete);
                    MapMarkerTable.DeleteOnSubmit(objectToDelete);
		            return;
		        }
		        if (deleteData.ObjectType == "AboutContainer")
		        {
		            var objectToDelete = new AboutContainer {ID = deleteData.ID};
                    AboutContainerTable.Attach(objectToDelete);
                    AboutContainerTable.DeleteOnSubmit(objectToDelete);
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
			public Table<TBRLoginGroupRoot> TBRLoginGroupRootTable {
				get {
					return this.GetTable<TBRLoginGroupRoot>();
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
			public Table<AccountIndex> AccountIndexTable {
				get {
					return this.GetTable<AccountIndex>();
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
			public Table<AboutContainer> AboutContainerTable {
				get {
					return this.GetTable<AboutContainer>();
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
        }

    [Table(Name = "TBSystem")]
	public class TBSystem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBSystem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[InstanceName] TEXT NOT NULL, 
[AdminGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBSystem() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class WebPublishInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [WebPublishInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[PublishType] TEXT NOT NULL, 
[PublishContainer] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public WebPublishInfo() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string PublishType { get; set; }
		// private string _unmodified_PublishType;

		[Column]
        [ScaffoldColumn(true)]
		public string PublishContainer { get; set; }
		// private string _unmodified_PublishContainer;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(PublishType == null)
				PublishType = string.Empty;
			if(PublishContainer == null)
				PublishContainer = string.Empty;
		}
	}
    [Table(Name = "PublicationPackage")]
	public class PublicationPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PublicationPackage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[PackageName] TEXT NOT NULL, 
[PublicationTime] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public PublicationPackage() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBRLoginRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRLoginRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[DomainName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBRLoginRoot() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string DomainName { get; set; }
		// private string _unmodified_DomainName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(DomainName == null)
				DomainName = string.Empty;
		}
	}
    [Table(Name = "TBRLoginGroupRoot")]
	public class TBRLoginGroupRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRLoginGroupRoot](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Role] TEXT NOT NULL, 
[GroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBRLoginGroupRoot() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "TBAccountCollaborationGroup")]
	public class TBAccountCollaborationGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBAccountCollaborationGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[GroupRole] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBAccountCollaborationGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBLoginInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBLoginInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[OpenIDUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBLoginInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBEmail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBEmail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL, 
[ValidatedAt] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBEmail() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBCollaboratorRole : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratorRole](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Role] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBCollaboratorRole() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBCollaboratingGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBCollaboratingGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBCollaboratingGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBEmailValidation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBEmailValidation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Email] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[ValidUntil] TEXT NOT NULL, 
[RedirectUrlAfterValidation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBEmailValidation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBMergeAccountConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBMergeAccountConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[AccountToBeMergedID] TEXT NOT NULL, 
[AccountToMergeToID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBMergeAccountConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBGroupJoinConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBGroupJoinConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[InvitationMode] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBGroupJoinConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBDeviceJoinConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBDeviceJoinConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[DeviceMembershipID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBDeviceJoinConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBInformationInputConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBInformationInputConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationInputID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBInformationInputConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBInformationOutputConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBInformationOutputConfirmation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationOutputID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBInformationOutputConfirmation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBRegisterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBRegisterContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReturnUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBRegisterContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class LoginProvider : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LoginProvider](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public LoginProvider() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ContactOipContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContactOipContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[OIPModeratorGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ContactOipContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TBPRegisterEmail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TBPRegisterEmail](
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public TBPRegisterEmail() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class JavaScriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [JavaScriptContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public JavaScriptContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class JavascriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [JavascriptContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public JavascriptContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class FooterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [FooterContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public FooterContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class NavigationContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [NavigationContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Dummy] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public NavigationContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "AccountIndex")]
	public class AccountIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AccountIndex() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "StreetAddress")]
	public class StreetAddress : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [StreetAddress](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public StreetAddress() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AccountContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Dummy] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AccountContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AccountProfile : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountProfile](
[ID] TEXT NOT NULL PRIMARY KEY, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[IsSimplifiedAccount] INTEGER NOT NULL, 
[SimplifiedAccountEmail] TEXT NOT NULL, 
[SimplifiedAccountGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AccountProfile() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "AccountRoles")]
	public class AccountRoles : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AccountRoles](
[ID] TEXT NOT NULL PRIMARY KEY, 
[OrganizationsImPartOf] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AccountRoles() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class PersonalInfoVisibility : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PersonalInfoVisibility](
[ID] TEXT NOT NULL PRIMARY KEY, 
[NoOne_Network_All] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public PersonalInfoVisibility() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class GroupedInformation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupedInformation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public GroupedInformation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ReferenceToInformation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ReferenceToInformation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[URL] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ReferenceToInformation() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "RenderedNode")]
	public class RenderedNode : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RenderedNode](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public RenderedNode() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ShortTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ShortTextObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ShortTextObject() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class LongTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LongTextObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public LongTextObject() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class MapMarker : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [MapMarker](
[ID] TEXT NOT NULL PRIMARY KEY, 
[IconUrl] TEXT NOT NULL, 
[MarkerSource] TEXT NOT NULL, 
[CategoryName] TEXT NOT NULL, 
[LocationText] TEXT NOT NULL, 
[PopupTitle] TEXT NOT NULL, 
[PopupContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public MapMarker() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "AboutContainer")]
	public class AboutContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AboutContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Excerpt] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AboutContainer() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "ContainerHeader")]
	public class ContainerHeader : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContainerHeader](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ContainerHeader() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ActivitySummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ActivitySummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[SummaryBody] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ActivitySummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SummaryBody { get; set; }
		// private string _unmodified_SummaryBody;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryBody == null)
				SummaryBody = string.Empty;
		}
	}
    [Table(Name = "ActivityIndex")]
	public class ActivityIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ActivityIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ActivityIndex() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "Activity")]
	public class Activity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Activity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActivityName] TEXT NOT NULL, 
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

		public Activity() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ActivityName { get; set; }
		// private string _unmodified_ActivityName;

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
	public class Moderator : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Moderator](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ModeratorName] TEXT NOT NULL, 
[ProfileUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Moderator() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Collaborator : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Collaborator](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Collaborator() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class GroupSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupSummaryContainer](
[ID] TEXT NOT NULL PRIMARY KEY, 
[SummaryBody] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public GroupSummaryContainer() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string SummaryBody { get; set; }
		// private string _unmodified_SummaryBody;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(SummaryBody == null)
				SummaryBody = string.Empty;
		}
	}
    [Table(Name = "GroupIndex")]
	public class GroupIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [GroupIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public GroupIndex() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddAddressAndLocationInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddAddressAndLocationInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[LocationName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddAddressAndLocationInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddImageInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddImageInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddImageInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddImageGroupInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddImageGroupInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageGroupTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddImageGroupInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddEmailAddressInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddEmailAddressInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddEmailAddressInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class CreateGroupInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CreateGroupInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public CreateGroupInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddActivityInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddActivityInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActivityName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddActivityInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddBlogPostInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddBlogPostInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddBlogPostInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AddCategoryInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AddCategoryInfo](
[ID] TEXT NOT NULL PRIMARY KEY, 
[CategoryName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public AddCategoryInfo() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Group : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Group](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Group() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Introduction : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Introduction](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Body] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Introduction() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ContentCategoryRank : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ContentCategoryRank](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public ContentCategoryRank() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class LinkToContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [LinkToContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[URL] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public LinkToContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class EmbeddedContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [EmbeddedContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public EmbeddedContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class DynamicContentGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContentGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public DynamicContentGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class DynamicContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DynamicContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
[HostName] TEXT NOT NULL, 
[ContentName] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ElementQuery] TEXT NOT NULL, 
[Content] TEXT NOT NULL, 
[RawContent] TEXT NOT NULL, 
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

		public DynamicContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class AttachedToObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [AttachedToObject](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public AttachedToObject() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Comment : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Comment](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Comment() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Selection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Selection](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Selection() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class TextContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [TextContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public TextContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Blog : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Blog](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[IFrameSources] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Blog() 
		{
			ID = Guid.NewGuid().ToString();
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
        [ScaffoldColumn(true)]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
        [ScaffoldColumn(true)]
		public string Author { get; set; }
		// private string _unmodified_Author;

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
	public class BlogIndexGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BlogIndexGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public BlogIndexGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class CalendarIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [CalendarIndex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public CalendarIndex() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Filter : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Filter](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Filter() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Calendar : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Calendar](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Calendar() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Map : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Map](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Map() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "Video")]
	public class Video : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Video](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Video() 
		{
			ID = Guid.NewGuid().ToString();
		}


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
	public class Image : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Image](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Image() 
		{
			ID = Guid.NewGuid().ToString();
		}


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
	public class BinaryFile : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [BinaryFile](
[ID] TEXT NOT NULL PRIMARY KEY, 
[OriginalFileName] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public BinaryFile() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string OriginalFileName { get; set; }
		// private string _unmodified_OriginalFileName;

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
	public class ImageGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public ImageGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "VideoGroup")]
	public class VideoGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [VideoGroup](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public VideoGroup() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Tooltip : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Tooltip](
[ID] TEXT NOT NULL PRIMARY KEY, 
[TooltipText] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Tooltip() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "Longitude")]
	public class Longitude : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Longitude](
[ID] TEXT NOT NULL PRIMARY KEY, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Longitude() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Latitude : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Latitude](
[ID] TEXT NOT NULL PRIMARY KEY, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Latitude() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Location : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Location](
[ID] TEXT NOT NULL PRIMARY KEY, 
[LocationName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Location() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "Date")]
	public class Date : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Date](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Date() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Sex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Sex](
[ID] TEXT NOT NULL PRIMARY KEY, 
[SexText] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Sex() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class OBSAddress : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OBSAddress](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public OBSAddress() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Identity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Identity](
[ID] TEXT NOT NULL PRIMARY KEY, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[Initials] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Identity() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class ImageVideoSoundVectorRaw : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [ImageVideoSoundVectorRaw](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public ImageVideoSoundVectorRaw() 
		{
			ID = Guid.NewGuid().ToString();
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
    [Table(Name = "Category")]
	public class Category : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Category](
[ID] TEXT NOT NULL PRIMARY KEY, 
[CategoryName] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[ParentCategoryID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public Category() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;

		[Column]
        [ScaffoldColumn(true)]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
        [ScaffoldColumn(true)]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
        [ScaffoldColumn(true)]
		public string ParentCategoryID { get; set; }
		// private string _unmodified_ParentCategoryID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(CategoryName == null)
				CategoryName = string.Empty;
			if(Title == null)
				Title = string.Empty;
			if(Excerpt == null)
				Excerpt = string.Empty;
			if(ParentCategoryID == null)
				ParentCategoryID = string.Empty;
		}
	}
    [Table(Name = "Subscription")]
	public class Subscription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Subscription](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Subscription() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class QueueEnvelope : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [QueueEnvelope](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActiveContainerName] TEXT NOT NULL, 
[OwnerPrefix] TEXT NOT NULL, 
[CurrentRetryCount] INTEGER NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public QueueEnvelope() 
		{
			ID = Guid.NewGuid().ToString();
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
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ActiveContainerName == null)
				ActiveContainerName = string.Empty;
			if(OwnerPrefix == null)
				OwnerPrefix = string.Empty;
		}
	}
    [Table(Name = "OperationRequest")]
	public class OperationRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [OperationRequest](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProcessIDToExecute] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public OperationRequest() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class SubscriptionChainRequestMessage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionChainRequestMessage](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContentItemID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public SubscriptionChainRequestMessage() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class SubscriptionChainRequestContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionChainRequestContent](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public SubscriptionChainRequestContent() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class SubscriptionTarget : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriptionTarget](
[ID] TEXT NOT NULL PRIMARY KEY, 
[BlobLocation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public SubscriptionTarget() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class DeleteEntireOwnerOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeleteEntireOwnerOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public DeleteEntireOwnerOperation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class DeleteOwnerContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [DeleteOwnerContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public DeleteOwnerContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class SystemError : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SystemError](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ErrorTitle] TEXT NOT NULL, 
[OccurredAt] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public SystemError() 
		{
			ID = Guid.NewGuid().ToString();
		}


		[Column]
        [ScaffoldColumn(true)]
		public string ErrorTitle { get; set; }
		// private string _unmodified_ErrorTitle;

		[Column]
        [ScaffoldColumn(true)]
		public DateTime OccurredAt { get; set; }
		// private DateTime _unmodified_OccurredAt;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
			if(ErrorTitle == null)
				ErrorTitle = string.Empty;
		}
	}
    [Table(Name = "SystemErrorItem")]
	public class SystemErrorItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SystemErrorItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ShortDescription] TEXT NOT NULL, 
[LongDescription] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public SystemErrorItem() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class InformationSource : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [InformationSource](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public InformationSource() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class RefreshDefaultViewsOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [RefreshDefaultViewsOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
[ViewLocation] TEXT NOT NULL, 
[TypeNameToRefresh] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public RefreshDefaultViewsOperation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class UpdateWebContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UpdateWebContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public UpdateWebContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class UpdateWebContentHandlerItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [UpdateWebContentHandlerItem](
[ID] TEXT NOT NULL PRIMARY KEY, 
[InformationTypeName] TEXT NOT NULL, 
[OptionName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public UpdateWebContentHandlerItem() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class PublishWebContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [PublishWebContentOperation](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public PublishWebContentOperation() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class SubscriberInput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [SubscriberInput](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public SubscriberInput() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class Monitor : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [Monitor](
[ID] TEXT NOT NULL PRIMARY KEY, 
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

		public Monitor() 
		{
			ID = Guid.NewGuid().ToString();
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
	public class IconTitleDescription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS [IconTitleDescription](
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] BLOB NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
        [ScaffoldColumn(true)]
        [Editable(false)]
		public string ID { get; set; }

		public IconTitleDescription() 
		{
			ID = Guid.NewGuid().ToString();
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
 } 
