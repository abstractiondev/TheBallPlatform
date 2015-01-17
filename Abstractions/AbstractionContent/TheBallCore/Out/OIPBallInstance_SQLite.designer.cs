 


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

namespace SQLite.AaltoGlobalImpact.OIP { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring(bool isInitialInsert);
	}

		[Flags]
		public enum SerializationType 
		{
			Undefined = 0,
			XML = 1,
			JSON = 2,
			XML_AND_JSON = XML | JSON
		}

		[Table]
		public class InformationObjectMetaData
		{
			[Column(IsPrimaryKey = true)]
			public string ID { get; set; }

			[Column]
			public string SemanticDomain { get; set; }
			[Column]
			public string ObjectType { get; set; }
			[Column]
			public string ObjectID { get; set; }
			[Column]
			public string MD5 { get; set; }
			[Column]
			public string LastWriteTime { get; set; }
			[Column]
			public long FileLength { get; set; }
			[Column]
			public SerializationType SerializationType { get; set; }

            public ChangeAction CurrentChangeAction { get; set; }
		}


		public class TheBallDataContext : DataContext
		{

		    public static string[] GetMetaDataTableCreateSQLs()
		    {
		        return new string[]
		        {
		            @"
CREATE TABLE IF NOT EXISTS InformationObjectMetaData(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SemanticDomain] TEXT NOT NULL, 
[ObjectType] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL,
[MD5] TEXT NOT NULL,
[LastWriteTime] TEXT NOT NULL,
[FileLength] INTEGER NOT NULL,
[SerializationType] INTEGER NOT NULL,
)",
		            @"
CREATE UNIQUE INDEX ObjectIX ON InformationObjectMetaData (
SemanticDomain, 
ObjectType, 
ObjectID
)"
		        };
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
                tableCreationCommands.AddRange(GetMetaDataTableCreateSQLs());
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
	public class TBSystem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBSystem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[InstanceName] TEXT NOT NULL, 
[AdminGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InstanceName { get; set; }
		// private string _unmodified_InstanceName;

		[Column]
		public string AdminGroupID { get; set; }
		// private string _unmodified_AdminGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "WebPublishInfo")]
	public class WebPublishInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS WebPublishInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PublishType] TEXT NOT NULL, 
[PublishContainer] TEXT NOT NULL, 
[ActivePublication] TEXT NOT NULL, 
[Publications] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PublishType { get; set; }
		// private string _unmodified_PublishType;

		[Column]
		public string PublishContainer { get; set; }
		// private string _unmodified_PublishContainer;

		[Column]
		public PublicationPackage ActivePublication { get; set; }
		// private PublicationPackage _unmodified_ActivePublication;

		[Column]
		public PublicationPackageCollection Publications { get; set; }
		// private PublicationPackageCollection _unmodified_Publications;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "PublicationPackage")]
	public class PublicationPackage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS PublicationPackage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[PackageName] TEXT NOT NULL, 
[PublicationTime] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PackageName { get; set; }
		// private string _unmodified_PackageName;

		[Column]
		public DateTime PublicationTime { get; set; }
		// private DateTime _unmodified_PublicationTime;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRLoginRoot")]
	public class TBRLoginRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBRLoginRoot(
[ID] TEXT NOT NULL PRIMARY KEY, 
[DomainName] TEXT NOT NULL, 
[Account] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string DomainName { get; set; }
		// private string _unmodified_DomainName;

		[Column]
		public TBAccount Account { get; set; }
		// private TBAccount _unmodified_Account;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRAccountRoot")]
	public class TBRAccountRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBRAccountRoot(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Account] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBAccount Account { get; set; }
		// private TBAccount _unmodified_Account;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRGroupRoot")]
	public class TBRGroupRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBRGroupRoot(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Group] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBCollaboratingGroup Group { get; set; }
		// private TBCollaboratingGroup _unmodified_Group;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRLoginGroupRoot")]
	public class TBRLoginGroupRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBRLoginGroupRoot(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Role] TEXT NOT NULL, 
[GroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBREmailRoot")]
	public class TBREmailRoot : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBREmailRoot(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Account] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBAccount Account { get; set; }
		// private TBAccount _unmodified_Account;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBAccount")]
	public class TBAccount : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBAccount(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Emails] TEXT NOT NULL, 
[Logins] TEXT NOT NULL, 
[GroupRoleCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBEmailCollection Emails { get; set; }
		// private TBEmailCollection _unmodified_Emails;

		[Column]
		public TBLoginInfoCollection Logins { get; set; }
		// private TBLoginInfoCollection _unmodified_Logins;

		[Column]
		public TBAccountCollaborationGroupCollection GroupRoleCollection { get; set; }
		// private TBAccountCollaborationGroupCollection _unmodified_GroupRoleCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBAccountCollaborationGroup")]
	public class TBAccountCollaborationGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBAccountCollaborationGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[GroupRole] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
		public string GroupRole { get; set; }
		// private string _unmodified_GroupRole;

		[Column]
		public string RoleStatus { get; set; }
		// private string _unmodified_RoleStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBLoginInfo")]
	public class TBLoginInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBLoginInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OpenIDUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OpenIDUrl { get; set; }
		// private string _unmodified_OpenIDUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBEmail")]
	public class TBEmail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBEmail(
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL, 
[ValidatedAt] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
		public DateTime ValidatedAt { get; set; }
		// private DateTime _unmodified_ValidatedAt;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBCollaboratorRole")]
	public class TBCollaboratorRole : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBCollaboratorRole(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Email] TEXT NOT NULL, 
[Role] TEXT NOT NULL, 
[RoleStatus] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBEmail Email { get; set; }
		// private TBEmail _unmodified_Email;

		[Column]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
		public string RoleStatus { get; set; }
		// private string _unmodified_RoleStatus;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBCollaboratingGroup")]
	public class TBCollaboratingGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBCollaboratingGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Roles] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public TBCollaboratorRoleCollection Roles { get; set; }
		// private TBCollaboratorRoleCollection _unmodified_Roles;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBEmailValidation")]
	public class TBEmailValidation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBEmailValidation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Email] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[ValidUntil] TEXT NOT NULL, 
[GroupJoinConfirmation] TEXT NOT NULL, 
[DeviceJoinConfirmation] TEXT NOT NULL, 
[InformationInputConfirmation] TEXT NOT NULL, 
[InformationOutputConfirmation] TEXT NOT NULL, 
[MergeAccountsConfirmation] TEXT NOT NULL, 
[RedirectUrlAfterValidation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Email { get; set; }
		// private string _unmodified_Email;

		[Column]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
		public DateTime ValidUntil { get; set; }
		// private DateTime _unmodified_ValidUntil;

		[Column]
		public TBGroupJoinConfirmation GroupJoinConfirmation { get; set; }
		// private TBGroupJoinConfirmation _unmodified_GroupJoinConfirmation;

		[Column]
		public TBDeviceJoinConfirmation DeviceJoinConfirmation { get; set; }
		// private TBDeviceJoinConfirmation _unmodified_DeviceJoinConfirmation;

		[Column]
		public TBInformationInputConfirmation InformationInputConfirmation { get; set; }
		// private TBInformationInputConfirmation _unmodified_InformationInputConfirmation;

		[Column]
		public TBInformationOutputConfirmation InformationOutputConfirmation { get; set; }
		// private TBInformationOutputConfirmation _unmodified_InformationOutputConfirmation;

		[Column]
		public TBMergeAccountConfirmation MergeAccountsConfirmation { get; set; }
		// private TBMergeAccountConfirmation _unmodified_MergeAccountsConfirmation;

		[Column]
		public string RedirectUrlAfterValidation { get; set; }
		// private string _unmodified_RedirectUrlAfterValidation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBMergeAccountConfirmation")]
	public class TBMergeAccountConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBMergeAccountConfirmation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[AccountToBeMergedID] TEXT NOT NULL, 
[AccountToMergeToID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string AccountToBeMergedID { get; set; }
		// private string _unmodified_AccountToBeMergedID;

		[Column]
		public string AccountToMergeToID { get; set; }
		// private string _unmodified_AccountToMergeToID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBGroupJoinConfirmation")]
	public class TBGroupJoinConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBGroupJoinConfirmation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[InvitationMode] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
		public string InvitationMode { get; set; }
		// private string _unmodified_InvitationMode;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBDeviceJoinConfirmation")]
	public class TBDeviceJoinConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBDeviceJoinConfirmation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[DeviceMembershipID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
		public string DeviceMembershipID { get; set; }
		// private string _unmodified_DeviceMembershipID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBInformationInputConfirmation")]
	public class TBInformationInputConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBInformationInputConfirmation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationInputID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
		public string InformationInputID { get; set; }
		// private string _unmodified_InformationInputID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBInformationOutputConfirmation")]
	public class TBInformationOutputConfirmation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBInformationOutputConfirmation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupID] TEXT NOT NULL, 
[AccountID] TEXT NOT NULL, 
[InformationOutputID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupID { get; set; }
		// private string _unmodified_GroupID;

		[Column]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
		public string InformationOutputID { get; set; }
		// private string _unmodified_InformationOutputID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBRegisterContainer")]
	public class TBRegisterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBRegisterContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[ReturnUrl] TEXT NOT NULL, 
[LoginProviderCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public string ReturnUrl { get; set; }
		// private string _unmodified_ReturnUrl;

		[Column]
		public LoginProviderCollection LoginProviderCollection { get; set; }
		// private LoginProviderCollection _unmodified_LoginProviderCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LoginProvider")]
	public class LoginProvider : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS LoginProvider(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProviderName] TEXT NOT NULL, 
[ProviderIconClass] TEXT NOT NULL, 
[ProviderType] TEXT NOT NULL, 
[ProviderUrl] TEXT NOT NULL, 
[ReturnUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ProviderName { get; set; }
		// private string _unmodified_ProviderName;

		[Column]
		public string ProviderIconClass { get; set; }
		// private string _unmodified_ProviderIconClass;

		[Column]
		public string ProviderType { get; set; }
		// private string _unmodified_ProviderType;

		[Column]
		public string ProviderUrl { get; set; }
		// private string _unmodified_ProviderUrl;

		[Column]
		public string ReturnUrl { get; set; }
		// private string _unmodified_ReturnUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ContactOipContainer")]
	public class ContactOipContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ContactOipContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OIPModeratorGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OIPModeratorGroupID { get; set; }
		// private string _unmodified_OIPModeratorGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TBPRegisterEmail")]
	public class TBPRegisterEmail : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TBPRegisterEmail(
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "JavaScriptContainer")]
	public class JavaScriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS JavaScriptContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "JavascriptContainer")]
	public class JavascriptContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS JavascriptContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "FooterContainer")]
	public class FooterContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS FooterContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[HtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string HtmlContent { get; set; }
		// private string _unmodified_HtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "NavigationContainer")]
	public class NavigationContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS NavigationContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Dummy] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Dummy { get; set; }
		// private string _unmodified_Dummy;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountSummary")]
	public class AccountSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Introduction] TEXT NOT NULL, 
[ActivitySummary] TEXT NOT NULL, 
[GroupSummary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public ActivitySummaryContainer ActivitySummary { get; set; }
		// private ActivitySummaryContainer _unmodified_ActivitySummary;

		[Column]
		public GroupSummaryContainer GroupSummary { get; set; }
		// private GroupSummaryContainer _unmodified_GroupSummary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountContainer")]
	public class AccountContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[AccountIndex] TEXT NOT NULL, 
[AccountModule] TEXT NOT NULL, 
[AccountSummary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public AccountIndex AccountIndex { get; set; }
		// private AccountIndex _unmodified_AccountIndex;

		[Column]
		public AccountModule AccountModule { get; set; }
		// private AccountModule _unmodified_AccountModule;

		[Column]
		public AccountSummary AccountSummary { get; set; }
		// private AccountSummary _unmodified_AccountSummary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountIndex")]
	public class AccountIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountIndex(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image Icon { get; set; }
		// private Image _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountModule")]
	public class AccountModule : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountModule(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Profile] TEXT NOT NULL, 
[Security] TEXT NOT NULL, 
[Roles] TEXT NOT NULL, 
[LocationCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public AccountProfile Profile { get; set; }
		// private AccountProfile _unmodified_Profile;

		[Column]
		public AccountSecurity Security { get; set; }
		// private AccountSecurity _unmodified_Security;

		[Column]
		public AccountRoles Roles { get; set; }
		// private AccountRoles _unmodified_Roles;

		[Column]
		public AddressAndLocationCollection LocationCollection { get; set; }
		// private AddressAndLocationCollection _unmodified_LocationCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ImageGroupContainer")]
	public class ImageGroupContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ImageGroupContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageGroups] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ImageGroupCollection ImageGroups { get; set; }
		// private ImageGroupCollection _unmodified_ImageGroups;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LocationContainer")]
	public class LocationContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS LocationContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Locations] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public AddressAndLocationCollection Locations { get; set; }
		// private AddressAndLocationCollection _unmodified_Locations;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddressAndLocation")]
	public class AddressAndLocation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddressAndLocation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[Address] TEXT NOT NULL, 
[Location] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public StreetAddress Address { get; set; }
		// private StreetAddress _unmodified_Address;

		[Column]
		public Location Location { get; set; }
		// private Location _unmodified_Location;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "StreetAddress")]
	public class StreetAddress : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS StreetAddress(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Street] TEXT NOT NULL, 
[ZipCode] TEXT NOT NULL, 
[Town] TEXT NOT NULL, 
[Country] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Street { get; set; }
		// private string _unmodified_Street;

		[Column]
		public string ZipCode { get; set; }
		// private string _unmodified_ZipCode;

		[Column]
		public string Town { get; set; }
		// private string _unmodified_Town;

		[Column]
		public string Country { get; set; }
		// private string _unmodified_Country;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountContent")]
	public class AccountContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Dummy] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Dummy { get; set; }
		// private string _unmodified_Dummy;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountProfile")]
	public class AccountProfile : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountProfile(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProfileImage] TEXT NOT NULL, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[Address] TEXT NOT NULL, 
[IsSimplifiedAccount] INTEGER NOT NULL, 
[SimplifiedAccountEmail] TEXT NOT NULL, 
[SimplifiedAccountGroupID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image ProfileImage { get; set; }
		// private Image _unmodified_ProfileImage;

		[Column]
		public string FirstName { get; set; }
		// private string _unmodified_FirstName;

		[Column]
		public string LastName { get; set; }
		// private string _unmodified_LastName;

		[Column]
		public StreetAddress Address { get; set; }
		// private StreetAddress _unmodified_Address;

		[Column]
		public bool IsSimplifiedAccount { get; set; }
		// private bool _unmodified_IsSimplifiedAccount;

		[Column]
		public string SimplifiedAccountEmail { get; set; }
		// private string _unmodified_SimplifiedAccountEmail;

		[Column]
		public string SimplifiedAccountGroupID { get; set; }
		// private string _unmodified_SimplifiedAccountGroupID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountSecurity")]
	public class AccountSecurity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountSecurity(
[ID] TEXT NOT NULL PRIMARY KEY, 
[LoginInfoCollection] TEXT NOT NULL, 
[EmailCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public TBLoginInfoCollection LoginInfoCollection { get; set; }
		// private TBLoginInfoCollection _unmodified_LoginInfoCollection;

		[Column]
		public TBEmailCollection EmailCollection { get; set; }
		// private TBEmailCollection _unmodified_EmailCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AccountRoles")]
	public class AccountRoles : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AccountRoles(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ModeratorInGroups] TEXT NOT NULL, 
[MemberInGroups] TEXT NOT NULL, 
[OrganizationsImPartOf] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceCollection ModeratorInGroups { get; set; }
		// private ReferenceCollection _unmodified_ModeratorInGroups;

		[Column]
		public ReferenceCollection MemberInGroups { get; set; }
		// private ReferenceCollection _unmodified_MemberInGroups;

		[Column]
		public string OrganizationsImPartOf { get; set; }
		// private string _unmodified_OrganizationsImPartOf;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "PersonalInfoVisibility")]
	public class PersonalInfoVisibility : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS PersonalInfoVisibility(
[ID] TEXT NOT NULL PRIMARY KEY, 
[NoOne_Network_All] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string NoOne_Network_All { get; set; }
		// private string _unmodified_NoOne_Network_All;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupedInformation")]
	public class GroupedInformation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GroupedInformation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL, 
[ReferenceCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public ReferenceCollection ReferenceCollection { get; set; }
		// private ReferenceCollection _unmodified_ReferenceCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ReferenceToInformation")]
	public class ReferenceToInformation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ReferenceToInformation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[URL] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string URL { get; set; }
		// private string _unmodified_URL;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "BlogContainer")]
	public class BlogContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS BlogContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[FeaturedBlog] TEXT NOT NULL, 
[RecentBlogSummary] TEXT NOT NULL, 
[BlogIndexGroup] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public Blog FeaturedBlog { get; set; }
		// private Blog _unmodified_FeaturedBlog;

		[Column]
		public RecentBlogSummary RecentBlogSummary { get; set; }
		// private RecentBlogSummary _unmodified_RecentBlogSummary;

		[Column]
		public BlogIndexGroup BlogIndexGroup { get; set; }
		// private BlogIndexGroup _unmodified_BlogIndexGroup;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RecentBlogSummary")]
	public class RecentBlogSummary : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS RecentBlogSummary(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Introduction] TEXT NOT NULL, 
[RecentBlogCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public BlogCollection RecentBlogCollection { get; set; }
		// private BlogCollection _unmodified_RecentBlogCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "NodeSummaryContainer")]
	public class NodeSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS NodeSummaryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Nodes] TEXT NOT NULL, 
[NodeSourceBlogs] TEXT NOT NULL, 
[NodeSourceActivities] TEXT NOT NULL, 
[NodeSourceTextContent] TEXT NOT NULL, 
[NodeSourceLinkToContent] TEXT NOT NULL, 
[NodeSourceEmbeddedContent] TEXT NOT NULL, 
[NodeSourceImages] TEXT NOT NULL, 
[NodeSourceBinaryFiles] TEXT NOT NULL, 
[NodeSourceCategories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public RenderedNodeCollection Nodes { get; set; }
		// private RenderedNodeCollection _unmodified_Nodes;

		[Column]
		public BlogCollection NodeSourceBlogs { get; set; }
		// private BlogCollection _unmodified_NodeSourceBlogs;

		[Column]
		public ActivityCollection NodeSourceActivities { get; set; }
		// private ActivityCollection _unmodified_NodeSourceActivities;

		[Column]
		public TextContentCollection NodeSourceTextContent { get; set; }
		// private TextContentCollection _unmodified_NodeSourceTextContent;

		[Column]
		public LinkToContentCollection NodeSourceLinkToContent { get; set; }
		// private LinkToContentCollection _unmodified_NodeSourceLinkToContent;

		[Column]
		public EmbeddedContentCollection NodeSourceEmbeddedContent { get; set; }
		// private EmbeddedContentCollection _unmodified_NodeSourceEmbeddedContent;

		[Column]
		public ImageCollection NodeSourceImages { get; set; }
		// private ImageCollection _unmodified_NodeSourceImages;

		[Column]
		public BinaryFileCollection NodeSourceBinaryFiles { get; set; }
		// private BinaryFileCollection _unmodified_NodeSourceBinaryFiles;

		[Column]
		public CategoryCollection NodeSourceCategories { get; set; }
		// private CategoryCollection _unmodified_NodeSourceCategories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RenderedNode")]
	public class RenderedNode : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS RenderedNode(
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
[CategoryFilters] TEXT NOT NULL, 
[CategoryNames] TEXT NOT NULL, 
[Categories] TEXT NOT NULL, 
[CategoryIDList] TEXT NOT NULL, 
[Authors] TEXT NOT NULL, 
[Locations] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OriginalContentID { get; set; }
		// private string _unmodified_OriginalContentID;

		[Column]
		public string TechnicalSource { get; set; }
		// private string _unmodified_TechnicalSource;

		[Column]
		public string ImageBaseUrl { get; set; }
		// private string _unmodified_ImageBaseUrl;

		[Column]
		public string ImageExt { get; set; }
		// private string _unmodified_ImageExt;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string ActualContentUrl { get; set; }
		// private string _unmodified_ActualContentUrl;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string TimestampText { get; set; }
		// private string _unmodified_TimestampText;

		[Column]
		public string MainSortableText { get; set; }
		// private string _unmodified_MainSortableText;

		[Column]
		public bool IsCategoryFilteringNode { get; set; }
		// private bool _unmodified_IsCategoryFilteringNode;

		[Column]
		public ShortTextCollection CategoryFilters { get; set; }
		// private ShortTextCollection _unmodified_CategoryFilters;

		[Column]
		public ShortTextCollection CategoryNames { get; set; }
		// private ShortTextCollection _unmodified_CategoryNames;

		[Column]
		public ShortTextCollection Categories { get; set; }
		// private ShortTextCollection _unmodified_Categories;

		[Column]
		public string CategoryIDList { get; set; }
		// private string _unmodified_CategoryIDList;

		[Column]
		public ShortTextCollection Authors { get; set; }
		// private ShortTextCollection _unmodified_Authors;

		[Column]
		public ShortTextCollection Locations { get; set; }
		// private ShortTextCollection _unmodified_Locations;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ShortTextObject")]
	public class ShortTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ShortTextObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LongTextObject")]
	public class LongTextObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS LongTextObject(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Content] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Content { get; set; }
		// private string _unmodified_Content;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapContainer")]
	public class MapContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS MapContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[MapFeatured] TEXT NOT NULL, 
[MapCollection] TEXT NOT NULL, 
[MapResultCollection] TEXT NOT NULL, 
[MapIndexCollection] TEXT NOT NULL, 
[MarkerSourceLocations] TEXT NOT NULL, 
[MarkerSourceBlogs] TEXT NOT NULL, 
[MarkerSourceActivities] TEXT NOT NULL, 
[MapMarkers] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public Map MapFeatured { get; set; }
		// private Map _unmodified_MapFeatured;

		[Column]
		public MapCollection MapCollection { get; set; }
		// private MapCollection _unmodified_MapCollection;

		[Column]
		public MapResultCollection MapResultCollection { get; set; }
		// private MapResultCollection _unmodified_MapResultCollection;

		[Column]
		public MapIndexCollection MapIndexCollection { get; set; }
		// private MapIndexCollection _unmodified_MapIndexCollection;

		[Column]
		public AddressAndLocationCollection MarkerSourceLocations { get; set; }
		// private AddressAndLocationCollection _unmodified_MarkerSourceLocations;

		[Column]
		public BlogCollection MarkerSourceBlogs { get; set; }
		// private BlogCollection _unmodified_MarkerSourceBlogs;

		[Column]
		public ActivityCollection MarkerSourceActivities { get; set; }
		// private ActivityCollection _unmodified_MarkerSourceActivities;

		[Column]
		public MapMarkerCollection MapMarkers { get; set; }
		// private MapMarkerCollection _unmodified_MapMarkers;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapMarker")]
	public class MapMarker : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS MapMarker(
[ID] TEXT NOT NULL PRIMARY KEY, 
[IconUrl] TEXT NOT NULL, 
[MarkerSource] TEXT NOT NULL, 
[CategoryName] TEXT NOT NULL, 
[LocationText] TEXT NOT NULL, 
[PopupTitle] TEXT NOT NULL, 
[PopupContent] TEXT NOT NULL, 
[Location] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string IconUrl { get; set; }
		// private string _unmodified_IconUrl;

		[Column]
		public string MarkerSource { get; set; }
		// private string _unmodified_MarkerSource;

		[Column]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;

		[Column]
		public string LocationText { get; set; }
		// private string _unmodified_LocationText;

		[Column]
		public string PopupTitle { get; set; }
		// private string _unmodified_PopupTitle;

		[Column]
		public string PopupContent { get; set; }
		// private string _unmodified_PopupContent;

		[Column]
		public Location Location { get; set; }
		// private Location _unmodified_Location;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CalendarContainer")]
	public class CalendarContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CalendarContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[CalendarContainerHeader] TEXT NOT NULL, 
[CalendarFeatured] TEXT NOT NULL, 
[CalendarCollection] TEXT NOT NULL, 
[CalendarIndexCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader CalendarContainerHeader { get; set; }
		// private ContainerHeader _unmodified_CalendarContainerHeader;

		[Column]
		public Calendar CalendarFeatured { get; set; }
		// private Calendar _unmodified_CalendarFeatured;

		[Column]
		public CalendarCollection CalendarCollection { get; set; }
		// private CalendarCollection _unmodified_CalendarCollection;

		[Column]
		public CalendarIndex CalendarIndexCollection { get; set; }
		// private CalendarIndex _unmodified_CalendarIndexCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AboutContainer")]
	public class AboutContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AboutContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[MainImage] TEXT NOT NULL, 
[Header] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[ImageGroup] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image MainImage { get; set; }
		// private Image _unmodified_MainImage;

		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Body { get; set; }
		// private string _unmodified_Body;

		[Column]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
		public ImageGroup ImageGroup { get; set; }
		// private ImageGroup _unmodified_ImageGroup;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "OBSAccountContainer")]
	public class OBSAccountContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS OBSAccountContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[AccountContainerHeader] TEXT NOT NULL, 
[AccountFeatured] TEXT NOT NULL, 
[AccountCollection] TEXT NOT NULL, 
[AccountIndexCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader AccountContainerHeader { get; set; }
		// private ContainerHeader _unmodified_AccountContainerHeader;

		[Column]
		public Calendar AccountFeatured { get; set; }
		// private Calendar _unmodified_AccountFeatured;

		[Column]
		public CalendarCollection AccountCollection { get; set; }
		// private CalendarCollection _unmodified_AccountCollection;

		[Column]
		public CalendarIndex AccountIndexCollection { get; set; }
		// private CalendarIndex _unmodified_AccountIndexCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ProjectContainer")]
	public class ProjectContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ProjectContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ProjectContainerHeader] TEXT NOT NULL, 
[ProjectFeatured] TEXT NOT NULL, 
[ProjectCollection] TEXT NOT NULL, 
[ProjectIndexCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader ProjectContainerHeader { get; set; }
		// private ContainerHeader _unmodified_ProjectContainerHeader;

		[Column]
		public Calendar ProjectFeatured { get; set; }
		// private Calendar _unmodified_ProjectFeatured;

		[Column]
		public CalendarCollection ProjectCollection { get; set; }
		// private CalendarCollection _unmodified_ProjectCollection;

		[Column]
		public CalendarIndex ProjectIndexCollection { get; set; }
		// private CalendarIndex _unmodified_ProjectIndexCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CourseContainer")]
	public class CourseContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CourseContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[CourseContainerHeader] TEXT NOT NULL, 
[CourseFeatured] TEXT NOT NULL, 
[CourseCollection] TEXT NOT NULL, 
[CourseIndexCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader CourseContainerHeader { get; set; }
		// private ContainerHeader _unmodified_CourseContainerHeader;

		[Column]
		public Calendar CourseFeatured { get; set; }
		// private Calendar _unmodified_CourseFeatured;

		[Column]
		public CalendarCollection CourseCollection { get; set; }
		// private CalendarCollection _unmodified_CourseCollection;

		[Column]
		public CalendarIndex CourseIndexCollection { get; set; }
		// private CalendarIndex _unmodified_CourseIndexCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ContainerHeader")]
	public class ContainerHeader : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ContainerHeader(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ActivitySummaryContainer")]
	public class ActivitySummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ActivitySummaryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[SummaryBody] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[ActivityIndex] TEXT NOT NULL, 
[ActivityCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public string SummaryBody { get; set; }
		// private string _unmodified_SummaryBody;

		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public ActivityIndex ActivityIndex { get; set; }
		// private ActivityIndex _unmodified_ActivityIndex;

		[Column]
		public ActivityCollection ActivityCollection { get; set; }
		// private ActivityCollection _unmodified_ActivityCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ActivityIndex")]
	public class ActivityIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ActivityIndex(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image Icon { get; set; }
		// private Image _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ActivityContainer")]
	public class ActivityContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ActivityContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[ActivityIndex] TEXT NOT NULL, 
[ActivityModule] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public ActivityIndex ActivityIndex { get; set; }
		// private ActivityIndex _unmodified_ActivityIndex;

		[Column]
		public Activity ActivityModule { get; set; }
		// private Activity _unmodified_ActivityModule;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Activity")]
	public class Activity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Activity(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[ProfileImage] TEXT NOT NULL, 
[IconImage] TEXT NOT NULL, 
[ActivityName] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[ContactPerson] TEXT NOT NULL, 
[StartingTime] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[IFrameSources] TEXT NOT NULL, 
[Collaborators] TEXT NOT NULL, 
[ImageGroupCollection] TEXT NOT NULL, 
[LocationCollection] TEXT NOT NULL, 
[CategoryCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public Image ProfileImage { get; set; }
		// private Image _unmodified_ProfileImage;

		[Column]
		public Image IconImage { get; set; }
		// private Image _unmodified_IconImage;

		[Column]
		public string ActivityName { get; set; }
		// private string _unmodified_ActivityName;

		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public string ContactPerson { get; set; }
		// private string _unmodified_ContactPerson;

		[Column]
		public DateTime StartingTime { get; set; }
		// private DateTime _unmodified_StartingTime;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;

		[Column]
		public CollaboratorCollection Collaborators { get; set; }
		// private CollaboratorCollection _unmodified_Collaborators;

		[Column]
		public ImageGroupCollection ImageGroupCollection { get; set; }
		// private ImageGroupCollection _unmodified_ImageGroupCollection;

		[Column]
		public AddressAndLocationCollection LocationCollection { get; set; }
		// private AddressAndLocationCollection _unmodified_LocationCollection;

		[Column]
		public CategoryCollection CategoryCollection { get; set; }
		// private CategoryCollection _unmodified_CategoryCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Moderator")]
	public class Moderator : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Moderator(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ModeratorName] TEXT NOT NULL, 
[ProfileUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ModeratorName { get; set; }
		// private string _unmodified_ModeratorName;

		[Column]
		public string ProfileUrl { get; set; }
		// private string _unmodified_ProfileUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Collaborator")]
	public class Collaborator : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Collaborator(
[ID] TEXT NOT NULL PRIMARY KEY, 
[AccountID] TEXT NOT NULL, 
[EmailAddress] TEXT NOT NULL, 
[CollaboratorName] TEXT NOT NULL, 
[Role] TEXT NOT NULL, 
[ProfileUrl] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string AccountID { get; set; }
		// private string _unmodified_AccountID;

		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
		public string CollaboratorName { get; set; }
		// private string _unmodified_CollaboratorName;

		[Column]
		public string Role { get; set; }
		// private string _unmodified_Role;

		[Column]
		public string ProfileUrl { get; set; }
		// private string _unmodified_ProfileUrl;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupSummaryContainer")]
	public class GroupSummaryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GroupSummaryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[SummaryBody] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[GroupSummaryIndex] TEXT NOT NULL, 
[GroupCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public string SummaryBody { get; set; }
		// private string _unmodified_SummaryBody;

		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public GroupIndex GroupSummaryIndex { get; set; }
		// private GroupIndex _unmodified_GroupSummaryIndex;

		[Column]
		public GroupCollection GroupCollection { get; set; }
		// private GroupCollection _unmodified_GroupCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupContainer")]
	public class GroupContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GroupContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Header] TEXT NOT NULL, 
[GroupIndex] TEXT NOT NULL, 
[GroupProfile] TEXT NOT NULL, 
[Collaborators] TEXT NOT NULL, 
[PendingCollaborators] TEXT NOT NULL, 
[Activities] TEXT NOT NULL, 
[ImageGroupCollection] TEXT NOT NULL, 
[LocationCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ContainerHeader Header { get; set; }
		// private ContainerHeader _unmodified_Header;

		[Column]
		public GroupIndex GroupIndex { get; set; }
		// private GroupIndex _unmodified_GroupIndex;

		[Column]
		public Group GroupProfile { get; set; }
		// private Group _unmodified_GroupProfile;

		[Column]
		public CollaboratorCollection Collaborators { get; set; }
		// private CollaboratorCollection _unmodified_Collaborators;

		[Column]
		public CollaboratorCollection PendingCollaborators { get; set; }
		// private CollaboratorCollection _unmodified_PendingCollaborators;

		[Column]
		public ActivityCollection Activities { get; set; }
		// private ActivityCollection _unmodified_Activities;

		[Column]
		public ImageGroupCollection ImageGroupCollection { get; set; }
		// private ImageGroupCollection _unmodified_ImageGroupCollection;

		[Column]
		public AddressAndLocationCollection LocationCollection { get; set; }
		// private AddressAndLocationCollection _unmodified_LocationCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "GroupIndex")]
	public class GroupIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS GroupIndex(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image Icon { get; set; }
		// private Image _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddAddressAndLocationInfo")]
	public class AddAddressAndLocationInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddAddressAndLocationInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[LocationName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string LocationName { get; set; }
		// private string _unmodified_LocationName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddImageInfo")]
	public class AddImageInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddImageInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ImageTitle { get; set; }
		// private string _unmodified_ImageTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddImageGroupInfo")]
	public class AddImageGroupInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddImageGroupInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageGroupTitle] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ImageGroupTitle { get; set; }
		// private string _unmodified_ImageGroupTitle;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddEmailAddressInfo")]
	public class AddEmailAddressInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddEmailAddressInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[EmailAddress] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CreateGroupInfo")]
	public class CreateGroupInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CreateGroupInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[GroupName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddActivityInfo")]
	public class AddActivityInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddActivityInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActivityName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ActivityName { get; set; }
		// private string _unmodified_ActivityName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddBlogPostInfo")]
	public class AddBlogPostInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddBlogPostInfo(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AddCategoryInfo")]
	public class AddCategoryInfo : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AddCategoryInfo(
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
		
		}
	}
    [Table(Name = "Group")]
	public class Group : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Group(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[ProfileImage] TEXT NOT NULL, 
[IconImage] TEXT NOT NULL, 
[GroupName] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[OrganizationsAndGroupsLinkedToUs] TEXT NOT NULL, 
[WwwSiteToPublishTo] TEXT NOT NULL, 
[CustomUICollection] TEXT NOT NULL, 
[Moderators] TEXT NOT NULL, 
[CategoryCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public Image ProfileImage { get; set; }
		// private Image _unmodified_ProfileImage;

		[Column]
		public Image IconImage { get; set; }
		// private Image _unmodified_IconImage;

		[Column]
		public string GroupName { get; set; }
		// private string _unmodified_GroupName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string OrganizationsAndGroupsLinkedToUs { get; set; }
		// private string _unmodified_OrganizationsAndGroupsLinkedToUs;

		[Column]
		public string WwwSiteToPublishTo { get; set; }
		// private string _unmodified_WwwSiteToPublishTo;

		[Column]
		public ShortTextCollection CustomUICollection { get; set; }
		// private ShortTextCollection _unmodified_CustomUICollection;

		[Column]
		public ModeratorCollection Moderators { get; set; }
		// private ModeratorCollection _unmodified_Moderators;

		[Column]
		public CategoryCollection CategoryCollection { get; set; }
		// private CategoryCollection _unmodified_CategoryCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Introduction")]
	public class Introduction : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Introduction(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Body] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Body { get; set; }
		// private string _unmodified_Body;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ContentCategoryRank")]
	public class ContentCategoryRank : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ContentCategoryRank(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContentID] TEXT NOT NULL, 
[ContentSemanticType] TEXT NOT NULL, 
[CategoryID] TEXT NOT NULL, 
[RankName] TEXT NOT NULL, 
[RankValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ContentID { get; set; }
		// private string _unmodified_ContentID;

		[Column]
		public string ContentSemanticType { get; set; }
		// private string _unmodified_ContentSemanticType;

		[Column]
		public string CategoryID { get; set; }
		// private string _unmodified_CategoryID;

		[Column]
		public string RankName { get; set; }
		// private string _unmodified_RankName;

		[Column]
		public string RankValue { get; set; }
		// private string _unmodified_RankValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "LinkToContent")]
	public class LinkToContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS LinkToContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[URL] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[ImageData] TEXT NOT NULL, 
[Locations] TEXT NOT NULL, 
[Categories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string URL { get; set; }
		// private string _unmodified_URL;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
		public MediaContent ImageData { get; set; }
		// private MediaContent _unmodified_ImageData;

		[Column]
		public AddressAndLocationCollection Locations { get; set; }
		// private AddressAndLocationCollection _unmodified_Locations;

		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "EmbeddedContent")]
	public class EmbeddedContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS EmbeddedContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[IFrameTagContents] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Locations] TEXT NOT NULL, 
[Categories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string IFrameTagContents { get; set; }
		// private string _unmodified_IFrameTagContents;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public AddressAndLocationCollection Locations { get; set; }
		// private AddressAndLocationCollection _unmodified_Locations;

		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "DynamicContentGroup")]
	public class DynamicContentGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS DynamicContentGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[HostName] TEXT NOT NULL, 
[GroupHeader] TEXT NOT NULL, 
[SortValue] TEXT NOT NULL, 
[PageLocation] TEXT NOT NULL, 
[ContentItemNames] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string HostName { get; set; }
		// private string _unmodified_HostName;

		[Column]
		public string GroupHeader { get; set; }
		// private string _unmodified_GroupHeader;

		[Column]
		public string SortValue { get; set; }
		// private string _unmodified_SortValue;

		[Column]
		public string PageLocation { get; set; }
		// private string _unmodified_PageLocation;

		[Column]
		public string ContentItemNames { get; set; }
		// private string _unmodified_ContentItemNames;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "DynamicContent")]
	public class DynamicContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS DynamicContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[HostName] TEXT NOT NULL, 
[ContentName] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[ElementQuery] TEXT NOT NULL, 
[Content] TEXT NOT NULL, 
[RawContent] TEXT NOT NULL, 
[ImageData] TEXT NOT NULL, 
[IsEnabled] INTEGER NOT NULL, 
[ApplyActively] INTEGER NOT NULL, 
[EditType] TEXT NOT NULL, 
[PageLocation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string HostName { get; set; }
		// private string _unmodified_HostName;

		[Column]
		public string ContentName { get; set; }
		// private string _unmodified_ContentName;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public string ElementQuery { get; set; }
		// private string _unmodified_ElementQuery;

		[Column]
		public string Content { get; set; }
		// private string _unmodified_Content;

		[Column]
		public string RawContent { get; set; }
		// private string _unmodified_RawContent;

		[Column]
		public MediaContent ImageData { get; set; }
		// private MediaContent _unmodified_ImageData;

		[Column]
		public bool IsEnabled { get; set; }
		// private bool _unmodified_IsEnabled;

		[Column]
		public bool ApplyActively { get; set; }
		// private bool _unmodified_ApplyActively;

		[Column]
		public string EditType { get; set; }
		// private string _unmodified_EditType;

		[Column]
		public string PageLocation { get; set; }
		// private string _unmodified_PageLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AttachedToObject")]
	public class AttachedToObject : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AttachedToObject(
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
		public string ID { get; set; }


		[Column]
		public string SourceObjectID { get; set; }
		// private string _unmodified_SourceObjectID;

		[Column]
		public string SourceObjectName { get; set; }
		// private string _unmodified_SourceObjectName;

		[Column]
		public string SourceObjectDomain { get; set; }
		// private string _unmodified_SourceObjectDomain;

		[Column]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Comment")]
	public class Comment : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Comment(
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
		public string ID { get; set; }


		[Column]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;

		[Column]
		public string CommentText { get; set; }
		// private string _unmodified_CommentText;

		[Column]
		public DateTime Created { get; set; }
		// private DateTime _unmodified_Created;

		[Column]
		public string OriginalAuthorName { get; set; }
		// private string _unmodified_OriginalAuthorName;

		[Column]
		public string OriginalAuthorEmail { get; set; }
		// private string _unmodified_OriginalAuthorEmail;

		[Column]
		public string OriginalAuthorAccountID { get; set; }
		// private string _unmodified_OriginalAuthorAccountID;

		[Column]
		public DateTime LastModified { get; set; }
		// private DateTime _unmodified_LastModified;

		[Column]
		public string LastAuthorName { get; set; }
		// private string _unmodified_LastAuthorName;

		[Column]
		public string LastAuthorEmail { get; set; }
		// private string _unmodified_LastAuthorEmail;

		[Column]
		public string LastAuthorAccountID { get; set; }
		// private string _unmodified_LastAuthorAccountID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Selection")]
	public class Selection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Selection(
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
		public string ID { get; set; }


		[Column]
		public string TargetObjectID { get; set; }
		// private string _unmodified_TargetObjectID;

		[Column]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
		public string TargetObjectDomain { get; set; }
		// private string _unmodified_TargetObjectDomain;

		[Column]
		public string SelectionCategory { get; set; }
		// private string _unmodified_SelectionCategory;

		[Column]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;

		[Column]
		public bool BooleanValue { get; set; }
		// private bool _unmodified_BooleanValue;

		[Column]
		public double DoubleValue { get; set; }
		// private double _unmodified_DoubleValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "TextContent")]
	public class TextContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS TextContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ImageData] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Locations] TEXT NOT NULL, 
[Categories] TEXT NOT NULL, 
[SortOrderNumber] REAL NOT NULL, 
[IFrameSources] TEXT NOT NULL, 
[RawHtmlContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public MediaContent ImageData { get; set; }
		// private MediaContent _unmodified_ImageData;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;

		[Column]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string Body { get; set; }
		// private string _unmodified_Body;

		[Column]
		public AddressAndLocationCollection Locations { get; set; }
		// private AddressAndLocationCollection _unmodified_Locations;

		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;

		[Column]
		public double SortOrderNumber { get; set; }
		// private double _unmodified_SortOrderNumber;

		[Column]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;

		[Column]
		public string RawHtmlContent { get; set; }
		// private string _unmodified_RawHtmlContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Blog")]
	public class Blog : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Blog(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[ProfileImage] TEXT NOT NULL, 
[IconImage] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[SubTitle] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Published] TEXT NOT NULL, 
[Author] TEXT NOT NULL, 
[FeaturedImage] TEXT NOT NULL, 
[ImageGroupCollection] TEXT NOT NULL, 
[VideoGroup] TEXT NOT NULL, 
[Body] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[IFrameSources] TEXT NOT NULL, 
[LocationCollection] TEXT NOT NULL, 
[CategoryCollection] TEXT NOT NULL, 
[SocialPanel] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public Image ProfileImage { get; set; }
		// private Image _unmodified_ProfileImage;

		[Column]
		public Image IconImage { get; set; }
		// private Image _unmodified_IconImage;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string SubTitle { get; set; }
		// private string _unmodified_SubTitle;

		[Column]
		public Introduction Introduction { get; set; }
		// private Introduction _unmodified_Introduction;

		[Column]
		public DateTime Published { get; set; }
		// private DateTime _unmodified_Published;

		[Column]
		public string Author { get; set; }
		// private string _unmodified_Author;

		[Column]
		public Image FeaturedImage { get; set; }
		// private Image _unmodified_FeaturedImage;

		[Column]
		public ImageGroupCollection ImageGroupCollection { get; set; }
		// private ImageGroupCollection _unmodified_ImageGroupCollection;

		[Column]
		public VideoGroup VideoGroup { get; set; }
		// private VideoGroup _unmodified_VideoGroup;

		[Column]
		public string Body { get; set; }
		// private string _unmodified_Body;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public string IFrameSources { get; set; }
		// private string _unmodified_IFrameSources;

		[Column]
		public AddressAndLocationCollection LocationCollection { get; set; }
		// private AddressAndLocationCollection _unmodified_LocationCollection;

		[Column]
		public CategoryCollection CategoryCollection { get; set; }
		// private CategoryCollection _unmodified_CategoryCollection;

		[Column]
		public SocialPanelCollection SocialPanel { get; set; }
		// private SocialPanelCollection _unmodified_SocialPanel;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "BlogIndexGroup")]
	public class BlogIndexGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS BlogIndexGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[GroupedByDate] TEXT NOT NULL, 
[GroupedByLocation] TEXT NOT NULL, 
[GroupedByAuthor] TEXT NOT NULL, 
[GroupedByCategory] TEXT NOT NULL, 
[FullBlogArchive] TEXT NOT NULL, 
[BlogSourceForSummary] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image Icon { get; set; }
		// private Image _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
		public GroupedInformationCollection GroupedByDate { get; set; }
		// private GroupedInformationCollection _unmodified_GroupedByDate;

		[Column]
		public GroupedInformationCollection GroupedByLocation { get; set; }
		// private GroupedInformationCollection _unmodified_GroupedByLocation;

		[Column]
		public GroupedInformationCollection GroupedByAuthor { get; set; }
		// private GroupedInformationCollection _unmodified_GroupedByAuthor;

		[Column]
		public GroupedInformationCollection GroupedByCategory { get; set; }
		// private GroupedInformationCollection _unmodified_GroupedByCategory;

		[Column]
		public ReferenceCollection FullBlogArchive { get; set; }
		// private ReferenceCollection _unmodified_FullBlogArchive;

		[Column]
		public BlogCollection BlogSourceForSummary { get; set; }
		// private BlogCollection _unmodified_BlogSourceForSummary;

		[Column]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CalendarIndex")]
	public class CalendarIndex : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CalendarIndex(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Introduction] TEXT NOT NULL, 
[Summary] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Image Icon { get; set; }
		// private Image _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Introduction { get; set; }
		// private string _unmodified_Introduction;

		[Column]
		public string Summary { get; set; }
		// private string _unmodified_Summary;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Filter")]
	public class Filter : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Filter(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Calendar")]
	public class Calendar : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Calendar(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Map")]
	public class Map : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Map(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapIndexCollection")]
	public class MapIndexCollection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS MapIndexCollection(
[ID] TEXT NOT NULL PRIMARY KEY, 
[MapByDate] TEXT NOT NULL, 
[MapByLocation] TEXT NOT NULL, 
[MapByAuthor] TEXT NOT NULL, 
[MapByCategory] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public MapCollection MapByDate { get; set; }
		// private MapCollection _unmodified_MapByDate;

		[Column]
		public MapCollection MapByLocation { get; set; }
		// private MapCollection _unmodified_MapByLocation;

		[Column]
		public MapCollection MapByAuthor { get; set; }
		// private MapCollection _unmodified_MapByAuthor;

		[Column]
		public MapCollection MapByCategory { get; set; }
		// private MapCollection _unmodified_MapByCategory;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapResult")]
	public class MapResult : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS MapResult(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Location] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Location Location { get; set; }
		// private Location _unmodified_Location;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "MapResultsCollection")]
	public class MapResultsCollection : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS MapResultsCollection(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ResultByDate] TEXT NOT NULL, 
[ResultByAuthor] TEXT NOT NULL, 
[ResultByProximity] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public MapResultCollection ResultByDate { get; set; }
		// private MapResultCollection _unmodified_ResultByDate;

		[Column]
		public MapResultCollection ResultByAuthor { get; set; }
		// private MapResultCollection _unmodified_ResultByAuthor;

		[Column]
		public MapResultCollection ResultByProximity { get; set; }
		// private MapResultCollection _unmodified_ResultByProximity;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Video")]
	public class Video : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Video(
[ID] TEXT NOT NULL PRIMARY KEY, 
[VideoData] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public MediaContent VideoData { get; set; }
		// private MediaContent _unmodified_VideoData;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Caption { get; set; }
		// private string _unmodified_Caption;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Image")]
	public class Image : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Image(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[ImageData] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Caption] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Locations] TEXT NOT NULL, 
[Categories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public MediaContent ImageData { get; set; }
		// private MediaContent _unmodified_ImageData;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Caption { get; set; }
		// private string _unmodified_Caption;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public AddressAndLocationCollection Locations { get; set; }
		// private AddressAndLocationCollection _unmodified_Locations;

		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "BinaryFile")]
	public class BinaryFile : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS BinaryFile(
[ID] TEXT NOT NULL PRIMARY KEY, 
[OriginalFileName] TEXT NOT NULL, 
[Data] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[Categories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string OriginalFileName { get; set; }
		// private string _unmodified_OriginalFileName;

		[Column]
		public MediaContent Data { get; set; }
		// private MediaContent _unmodified_Data;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ImageGroup")]
	public class ImageGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ImageGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ReferenceToInformation] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[FeaturedImage] TEXT NOT NULL, 
[ImagesCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public Image FeaturedImage { get; set; }
		// private Image _unmodified_FeaturedImage;

		[Column]
		public ImageCollection ImagesCollection { get; set; }
		// private ImageCollection _unmodified_ImagesCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "VideoGroup")]
	public class VideoGroup : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS VideoGroup(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL, 
[VideoCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;

		[Column]
		public VideoCollection VideoCollection { get; set; }
		// private VideoCollection _unmodified_VideoCollection;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Tooltip")]
	public class Tooltip : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Tooltip(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TooltipText] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TooltipText { get; set; }
		// private string _unmodified_TooltipText;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SocialPanel")]
	public class SocialPanel : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SocialPanel(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SocialFilter] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Filter SocialFilter { get; set; }
		// private Filter _unmodified_SocialFilter;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Longitude")]
	public class Longitude : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Longitude(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Latitude")]
	public class Latitude : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Latitude(
[ID] TEXT NOT NULL PRIMARY KEY, 
[TextValue] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string TextValue { get; set; }
		// private string _unmodified_TextValue;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Location")]
	public class Location : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Location(
[ID] TEXT NOT NULL PRIMARY KEY, 
[LocationName] TEXT NOT NULL, 
[Longitude] TEXT NOT NULL, 
[Latitude] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string LocationName { get; set; }
		// private string _unmodified_LocationName;

		[Column]
		public Longitude Longitude { get; set; }
		// private Longitude _unmodified_Longitude;

		[Column]
		public Latitude Latitude { get; set; }
		// private Latitude _unmodified_Latitude;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Date")]
	public class Date : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Date(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Day] TEXT NOT NULL, 
[Week] TEXT NOT NULL, 
[Month] TEXT NOT NULL, 
[Year] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime Day { get; set; }
		// private DateTime _unmodified_Day;

		[Column]
		public DateTime Week { get; set; }
		// private DateTime _unmodified_Week;

		[Column]
		public DateTime Month { get; set; }
		// private DateTime _unmodified_Month;

		[Column]
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
CREATE TABLE IF NOT EXISTS Sex(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SexText] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SexText { get; set; }
		// private string _unmodified_SexText;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "OBSAddress")]
	public class OBSAddress : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS OBSAddress(
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
		public string ID { get; set; }


		[Column]
		public string StreetName { get; set; }
		// private string _unmodified_StreetName;

		[Column]
		public string BuildingNumber { get; set; }
		// private string _unmodified_BuildingNumber;

		[Column]
		public string PostOfficeBox { get; set; }
		// private string _unmodified_PostOfficeBox;

		[Column]
		public string PostalCode { get; set; }
		// private string _unmodified_PostalCode;

		[Column]
		public string Municipality { get; set; }
		// private string _unmodified_Municipality;

		[Column]
		public string Region { get; set; }
		// private string _unmodified_Region;

		[Column]
		public string Province { get; set; }
		// private string _unmodified_Province;

		[Column]
		public string state { get; set; }
		// private string _unmodified_state;

		[Column]
		public string Country { get; set; }
		// private string _unmodified_Country;

		[Column]
		public string Continent { get; set; }
		// private string _unmodified_Continent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Identity")]
	public class Identity : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Identity(
[ID] TEXT NOT NULL PRIMARY KEY, 
[FirstName] TEXT NOT NULL, 
[LastName] TEXT NOT NULL, 
[Initials] TEXT NOT NULL, 
[Sex] TEXT NOT NULL, 
[Birthday] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string FirstName { get; set; }
		// private string _unmodified_FirstName;

		[Column]
		public string LastName { get; set; }
		// private string _unmodified_LastName;

		[Column]
		public string Initials { get; set; }
		// private string _unmodified_Initials;

		[Column]
		public Sex Sex { get; set; }
		// private Sex _unmodified_Sex;

		[Column]
		public Date Birthday { get; set; }
		// private Date _unmodified_Birthday;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "ImageVideoSoundVectorRaw")]
	public class ImageVideoSoundVectorRaw : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS ImageVideoSoundVectorRaw(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Image] BLOB NOT NULL, 
[Video] BLOB NOT NULL, 
[Sound] BLOB NOT NULL, 
[Vector] TEXT NOT NULL, 
[Raw] BLOB NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public byte[] Image { get; set; }
		// private byte[] _unmodified_Image;

		[Column]
		public byte[] Video { get; set; }
		// private byte[] _unmodified_Video;

		[Column]
		public byte[] Sound { get; set; }
		// private byte[] _unmodified_Sound;

		[Column]
		public string Vector { get; set; }
		// private string _unmodified_Vector;

		[Column]
		public byte[] Raw { get; set; }
		// private byte[] _unmodified_Raw;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "CategoryContainer")]
	public class CategoryContainer : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS CategoryContainer(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Categories] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public CategoryCollection Categories { get; set; }
		// private CategoryCollection _unmodified_Categories;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
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
[ReferenceToInformation] TEXT NOT NULL, 
[CategoryName] TEXT NOT NULL, 
[ImageData] TEXT NOT NULL, 
[Title] TEXT NOT NULL, 
[Excerpt] TEXT NOT NULL, 
[ParentCategory] TEXT NOT NULL, 
[ParentCategoryID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public ReferenceToInformation ReferenceToInformation { get; set; }
		// private ReferenceToInformation _unmodified_ReferenceToInformation;

		[Column]
		public string CategoryName { get; set; }
		// private string _unmodified_CategoryName;

		[Column]
		public MediaContent ImageData { get; set; }
		// private MediaContent _unmodified_ImageData;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Excerpt { get; set; }
		// private string _unmodified_Excerpt;

		[Column]
		public Category ParentCategory { get; set; }
		// private Category _unmodified_ParentCategory;

		[Column]
		public string ParentCategoryID { get; set; }
		// private string _unmodified_ParentCategoryID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Subscription")]
	public class Subscription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Subscription(
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
		public string ID { get; set; }


		[Column]
		public long Priority { get; set; }
		// private long _unmodified_Priority;

		[Column]
		public string TargetRelativeLocation { get; set; }
		// private string _unmodified_TargetRelativeLocation;

		[Column]
		public string TargetInformationObjectType { get; set; }
		// private string _unmodified_TargetInformationObjectType;

		[Column]
		public string SubscriberRelativeLocation { get; set; }
		// private string _unmodified_SubscriberRelativeLocation;

		[Column]
		public string SubscriberInformationObjectType { get; set; }
		// private string _unmodified_SubscriberInformationObjectType;

		[Column]
		public string SubscriptionType { get; set; }
		// private string _unmodified_SubscriptionType;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "QueueEnvelope")]
	public class QueueEnvelope : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS QueueEnvelope(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ActiveContainerName] TEXT NOT NULL, 
[OwnerPrefix] TEXT NOT NULL, 
[CurrentRetryCount] INTEGER NOT NULL, 
[SingleOperation] TEXT NOT NULL, 
[OrderDependentOperationSequence] TEXT NOT NULL, 
[ErrorContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ActiveContainerName { get; set; }
		// private string _unmodified_ActiveContainerName;

		[Column]
		public string OwnerPrefix { get; set; }
		// private string _unmodified_OwnerPrefix;

		[Column]
		public long CurrentRetryCount { get; set; }
		// private long _unmodified_CurrentRetryCount;

		[Column]
		public OperationRequest SingleOperation { get; set; }
		// private OperationRequest _unmodified_SingleOperation;

		[Column]
		public OperationRequestCollection OrderDependentOperationSequence { get; set; }
		// private OperationRequestCollection _unmodified_OrderDependentOperationSequence;

		[Column]
		public SystemError ErrorContent { get; set; }
		// private SystemError _unmodified_ErrorContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "OperationRequest")]
	public class OperationRequest : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS OperationRequest(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SubscriberNotification] TEXT NOT NULL, 
[SubscriptionChainRequest] TEXT NOT NULL, 
[UpdateWebContentOperation] TEXT NOT NULL, 
[RefreshDefaultViewsOperation] TEXT NOT NULL, 
[DeleteEntireOwner] TEXT NOT NULL, 
[DeleteOwnerContent] TEXT NOT NULL, 
[PublishWebContent] TEXT NOT NULL, 
[ProcessIDToExecute] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public Subscription SubscriberNotification { get; set; }
		// private Subscription _unmodified_SubscriberNotification;

		[Column]
		public SubscriptionChainRequestMessage SubscriptionChainRequest { get; set; }
		// private SubscriptionChainRequestMessage _unmodified_SubscriptionChainRequest;

		[Column]
		public UpdateWebContentOperation UpdateWebContentOperation { get; set; }
		// private UpdateWebContentOperation _unmodified_UpdateWebContentOperation;

		[Column]
		public RefreshDefaultViewsOperation RefreshDefaultViewsOperation { get; set; }
		// private RefreshDefaultViewsOperation _unmodified_RefreshDefaultViewsOperation;

		[Column]
		public DeleteEntireOwnerOperation DeleteEntireOwner { get; set; }
		// private DeleteEntireOwnerOperation _unmodified_DeleteEntireOwner;

		[Column]
		public DeleteOwnerContentOperation DeleteOwnerContent { get; set; }
		// private DeleteOwnerContentOperation _unmodified_DeleteOwnerContent;

		[Column]
		public PublishWebContentOperation PublishWebContent { get; set; }
		// private PublishWebContentOperation _unmodified_PublishWebContent;

		[Column]
		public string ProcessIDToExecute { get; set; }
		// private string _unmodified_ProcessIDToExecute;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SubscriptionChainRequestMessage")]
	public class SubscriptionChainRequestMessage : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SubscriptionChainRequestMessage(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContentItemID] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ContentItemID { get; set; }
		// private string _unmodified_ContentItemID;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SubscriptionChainRequestContent")]
	public class SubscriptionChainRequestContent : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SubscriptionChainRequestContent(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SubmitTime] TEXT NOT NULL, 
[ProcessingStartTime] TEXT NOT NULL, 
[ProcessingEndTimeInformationObjects] TEXT NOT NULL, 
[ProcessingEndTimeWebTemplatesRendering] TEXT NOT NULL, 
[ProcessingEndTime] TEXT NOT NULL, 
[SubscriptionTargetCollection] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public DateTime SubmitTime { get; set; }
		// private DateTime _unmodified_SubmitTime;

		[Column]
		public DateTime ProcessingStartTime { get; set; }
		// private DateTime _unmodified_ProcessingStartTime;

		[Column]
		public DateTime ProcessingEndTimeInformationObjects { get; set; }
		// private DateTime _unmodified_ProcessingEndTimeInformationObjects;

		[Column]
		public DateTime ProcessingEndTimeWebTemplatesRendering { get; set; }
		// private DateTime _unmodified_ProcessingEndTimeWebTemplatesRendering;

		[Column]
		public DateTime ProcessingEndTime { get; set; }
		// private DateTime _unmodified_ProcessingEndTime;

		[Column]
		public SubscriptionTargetCollection SubscriptionTargetCollection { get; set; }
		// private SubscriptionTargetCollection _unmodified_SubscriptionTargetCollection;
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
CREATE TABLE IF NOT EXISTS SubscriptionTarget(
[ID] TEXT NOT NULL PRIMARY KEY, 
[BlobLocation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string BlobLocation { get; set; }
		// private string _unmodified_BlobLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "DeleteEntireOwnerOperation")]
	public class DeleteEntireOwnerOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS DeleteEntireOwnerOperation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ContainerName { get; set; }
		// private string _unmodified_ContainerName;

		[Column]
		public string LocationPrefix { get; set; }
		// private string _unmodified_LocationPrefix;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "DeleteOwnerContentOperation")]
	public class DeleteOwnerContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS DeleteOwnerContentOperation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ContainerName] TEXT NOT NULL, 
[LocationPrefix] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ContainerName { get; set; }
		// private string _unmodified_ContainerName;

		[Column]
		public string LocationPrefix { get; set; }
		// private string _unmodified_LocationPrefix;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SystemError")]
	public class SystemError : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SystemError(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ErrorTitle] TEXT NOT NULL, 
[OccurredAt] TEXT NOT NULL, 
[SystemErrorItems] TEXT NOT NULL, 
[MessageContent] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ErrorTitle { get; set; }
		// private string _unmodified_ErrorTitle;

		[Column]
		public DateTime OccurredAt { get; set; }
		// private DateTime _unmodified_OccurredAt;

		[Column]
		public SystemErrorItemCollection SystemErrorItems { get; set; }
		// private SystemErrorItemCollection _unmodified_SystemErrorItems;

		[Column]
		public QueueEnvelope MessageContent { get; set; }
		// private QueueEnvelope _unmodified_MessageContent;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SystemErrorItem")]
	public class SystemErrorItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SystemErrorItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ShortDescription] TEXT NOT NULL, 
[LongDescription] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ShortDescription { get; set; }
		// private string _unmodified_ShortDescription;

		[Column]
		public string LongDescription { get; set; }
		// private string _unmodified_LongDescription;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "InformationSource")]
	public class InformationSource : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS InformationSource(
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
		public string ID { get; set; }


		[Column]
		public string SourceName { get; set; }
		// private string _unmodified_SourceName;

		[Column]
		public string SourceLocation { get; set; }
		// private string _unmodified_SourceLocation;

		[Column]
		public string SourceType { get; set; }
		// private string _unmodified_SourceType;

		[Column]
		public bool IsDynamic { get; set; }
		// private bool _unmodified_IsDynamic;

		[Column]
		public string SourceInformationObjectType { get; set; }
		// private string _unmodified_SourceInformationObjectType;

		[Column]
		public string SourceETag { get; set; }
		// private string _unmodified_SourceETag;

		[Column]
		public string SourceMD5 { get; set; }
		// private string _unmodified_SourceMD5;

		[Column]
		public DateTime SourceLastModified { get; set; }
		// private DateTime _unmodified_SourceLastModified;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "RefreshDefaultViewsOperation")]
	public class RefreshDefaultViewsOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS RefreshDefaultViewsOperation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[ViewLocation] TEXT NOT NULL, 
[TypeNameToRefresh] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string ViewLocation { get; set; }
		// private string _unmodified_ViewLocation;

		[Column]
		public string TypeNameToRefresh { get; set; }
		// private string _unmodified_TypeNameToRefresh;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "UpdateWebContentOperation")]
	public class UpdateWebContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS UpdateWebContentOperation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SourceContainerName] TEXT NOT NULL, 
[SourcePathRoot] TEXT NOT NULL, 
[TargetContainerName] TEXT NOT NULL, 
[TargetPathRoot] TEXT NOT NULL, 
[RenderWhileSync] INTEGER NOT NULL, 
[Handlers] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SourceContainerName { get; set; }
		// private string _unmodified_SourceContainerName;

		[Column]
		public string SourcePathRoot { get; set; }
		// private string _unmodified_SourcePathRoot;

		[Column]
		public string TargetContainerName { get; set; }
		// private string _unmodified_TargetContainerName;

		[Column]
		public string TargetPathRoot { get; set; }
		// private string _unmodified_TargetPathRoot;

		[Column]
		public bool RenderWhileSync { get; set; }
		// private bool _unmodified_RenderWhileSync;

		[Column]
		public UpdateWebContentHandlerCollection Handlers { get; set; }
		// private UpdateWebContentHandlerCollection _unmodified_Handlers;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "UpdateWebContentHandlerItem")]
	public class UpdateWebContentHandlerItem : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS UpdateWebContentHandlerItem(
[ID] TEXT NOT NULL PRIMARY KEY, 
[InformationTypeName] TEXT NOT NULL, 
[OptionName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InformationTypeName { get; set; }
		// private string _unmodified_InformationTypeName;

		[Column]
		public string OptionName { get; set; }
		// private string _unmodified_OptionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "PublishWebContentOperation")]
	public class PublishWebContentOperation : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS PublishWebContentOperation(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SourceContainerName] TEXT NOT NULL, 
[SourcePathRoot] TEXT NOT NULL, 
[SourceOwner] TEXT NOT NULL, 
[TargetContainerName] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string SourceContainerName { get; set; }
		// private string _unmodified_SourceContainerName;

		[Column]
		public string SourcePathRoot { get; set; }
		// private string _unmodified_SourcePathRoot;

		[Column]
		public string SourceOwner { get; set; }
		// private string _unmodified_SourceOwner;

		[Column]
		public string TargetContainerName { get; set; }
		// private string _unmodified_TargetContainerName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "SubscriberInput")]
	public class SubscriberInput : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS SubscriberInput(
[ID] TEXT NOT NULL PRIMARY KEY, 
[InputRelativeLocation] TEXT NOT NULL, 
[InformationObjectName] TEXT NOT NULL, 
[InformationItemName] TEXT NOT NULL, 
[SubscriberRelativeLocation] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string InputRelativeLocation { get; set; }
		// private string _unmodified_InputRelativeLocation;

		[Column]
		public string InformationObjectName { get; set; }
		// private string _unmodified_InformationObjectName;

		[Column]
		public string InformationItemName { get; set; }
		// private string _unmodified_InformationItemName;

		[Column]
		public string SubscriberRelativeLocation { get; set; }
		// private string _unmodified_SubscriberRelativeLocation;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "Monitor")]
	public class Monitor : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS Monitor(
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
		public string ID { get; set; }


		[Column]
		public string TargetObjectName { get; set; }
		// private string _unmodified_TargetObjectName;

		[Column]
		public string TargetItemName { get; set; }
		// private string _unmodified_TargetItemName;

		[Column]
		public DateTime MonitoringUtcTimeStampToStart { get; set; }
		// private DateTime _unmodified_MonitoringUtcTimeStampToStart;

		[Column]
		public string MonitoringCycleFrequencyUnit { get; set; }
		// private string _unmodified_MonitoringCycleFrequencyUnit;

		[Column]
		public long MonitoringCycleEveryXthOfUnit { get; set; }
		// private long _unmodified_MonitoringCycleEveryXthOfUnit;

		[Column]
		public string CustomMonitoringCycleOperationName { get; set; }
		// private string _unmodified_CustomMonitoringCycleOperationName;

		[Column]
		public string OperationActionName { get; set; }
		// private string _unmodified_OperationActionName;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "IconTitleDescription")]
	public class IconTitleDescription : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS IconTitleDescription(
[ID] TEXT NOT NULL PRIMARY KEY, 
[Icon] BLOB NOT NULL, 
[Title] TEXT NOT NULL, 
[Description] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public byte[] Icon { get; set; }
		// private byte[] _unmodified_Icon;

		[Column]
		public string Title { get; set; }
		// private string _unmodified_Title;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
    [Table(Name = "AboutAGIApplications")]
	public class AboutAGIApplications : ITheBallDataContextStorable
	{
        public static string GetCreateTableSQL()
        {
            return
                @"
CREATE TABLE IF NOT EXISTS AboutAGIApplications(
[ID] TEXT NOT NULL PRIMARY KEY, 
[BuiltForAnybody] TEXT NOT NULL, 
[ForAllPeople] TEXT NOT NULL
)";
        }


		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public IconTitleDescription BuiltForAnybody { get; set; }
		// private IconTitleDescription _unmodified_BuiltForAnybody;

		[Column]
		public IconTitleDescription ForAllPeople { get; set; }
		// private IconTitleDescription _unmodified_ForAllPeople;
        public void PrepareForStoring(bool isInitialInsert)
        {
		
		}
	}
 } 
