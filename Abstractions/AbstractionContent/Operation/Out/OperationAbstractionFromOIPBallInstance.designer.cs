 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace AaltoGlobalImpact.OIP { 
				public class ListConnectionPackageContentsParameters 
		{
				public TheBall.CORE.Process Process ;
				}
		
		public class ListConnectionPackageContents 
		{
				private static void PrepareParameters(ListConnectionPackageContentsParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ListConnectionPackageContentsParameters parameters)
		{
						PrepareParameters(parameters);
					string ConnectionID = ListConnectionPackageContentsImplementation.GetTarget_ConnectionID(parameters.Process);	
				string[] CallPickCategorizedContentConnectionOutput;
		{ // Local block to allow local naming
			PickCategorizedContentToConnectionParameters operationParameters = ListConnectionPackageContentsImplementation.CallPickCategorizedContentConnection_GetParameters(ConnectionID);
			var operationReturnValue =  await PickCategorizedContentToConnection.ExecuteAsync(operationParameters);
			CallPickCategorizedContentConnectionOutput = ListConnectionPackageContentsImplementation.CallPickCategorizedContentConnection_GetOutput(operationReturnValue, ConnectionID);						
		} // Local block closing
				 await ListConnectionPackageContentsImplementation.ExecuteMethod_SetContentsAsProcessOutputAsync(parameters.Process, CallPickCategorizedContentConnectionOutput);		
				}
				}
				public class PickCategorizedContentToConnectionParameters 
		{
				public string ConnectionID ;
				}
		
		public class PickCategorizedContentToConnection 
		{
				private static void PrepareParameters(PickCategorizedContentToConnectionParameters parameters)
		{
					}
				public static async Task<PickCategorizedContentToConnectionReturnValue> ExecuteAsync(PickCategorizedContentToConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Interface.Connection Connection =  await PickCategorizedContentToConnectionImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				Dictionary<string, Category> CategoriesToTransfer =  await PickCategorizedContentToConnectionImplementation.GetTarget_CategoriesToTransferAsync(Connection);	
				string[] ContentToTransferLocations =  await PickCategorizedContentToConnectionImplementation.GetTarget_ContentToTransferLocationsAsync(CategoriesToTransfer);	
				PickCategorizedContentToConnectionReturnValue returnValue = PickCategorizedContentToConnectionImplementation.Get_ReturnValue(ContentToTransferLocations);
		return returnValue;
				}
				}
				public class PickCategorizedContentToConnectionReturnValue 
		{
				public string[] ContentLocations ;
				}
				public class PublishGroupContentToWwwParameters 
		{
				public string GroupID ;
				public bool UseWorker ;
				}
		
		public class PublishGroupContentToWww 
		{
				private static void PrepareParameters(PublishGroupContentToWwwParameters parameters)
		{
					}
				public static void Execute(PublishGroupContentToWwwParameters parameters)
		{
						PrepareParameters(parameters);
					string CurrentContainerName = PublishGroupContentToWwwImplementation.GetTarget_CurrentContainerName(parameters.GroupID);	
				string WwwContainerName = PublishGroupContentToWwwImplementation.GetTarget_WwwContainerName(parameters.GroupID);	
				PublishGroupContentToWwwImplementation.ExecuteMethod_PublishGroupContentToWww(parameters.GroupID, parameters.UseWorker, CurrentContainerName, WwwContainerName);		
				}
				}
				public class CreateAdditionalMediaFormatsParameters 
		{
				public string MasterRelativeLocation ;
				}
		
		public class CreateAdditionalMediaFormats 
		{
				private static void PrepareParameters(CreateAdditionalMediaFormatsParameters parameters)
		{
					}
				public static async Task ExecuteAsync(CreateAdditionalMediaFormatsParameters parameters)
		{
						PrepareParameters(parameters);
					System.Drawing.Bitmap BitmapData =  await CreateAdditionalMediaFormatsImplementation.GetTarget_BitmapDataAsync(parameters.MasterRelativeLocation);	
				object VideoData = CreateAdditionalMediaFormatsImplementation.GetTarget_VideoData(parameters.MasterRelativeLocation);	
				CreateAdditionalMediaFormatsImplementation.ExecuteMethod_CreateImageMediaFormats(parameters.MasterRelativeLocation, BitmapData);		
				CreateAdditionalMediaFormatsImplementation.ExecuteMethod_CreateVideoMediaFormats(parameters.MasterRelativeLocation, VideoData);		
				}
				}
				public class ClearAdditionalMediaFormatsParameters 
		{
				public string MasterRelativeLocation ;
				}
		
		public class ClearAdditionalMediaFormats 
		{
				private static void PrepareParameters(ClearAdditionalMediaFormatsParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ClearAdditionalMediaFormatsParameters parameters)
		{
						PrepareParameters(parameters);
					 await ClearAdditionalMediaFormatsImplementation.ExecuteMethod_ClearImageMediaFormatsAsync(parameters.MasterRelativeLocation);		
				}
				}
				public class UpdatePublicationInfoParameters 
		{
				public TheBall.CORE.IContainerOwner Owner ;
				public string ContainerName ;
				}
		
		public class UpdatePublicationInfo 
		{
				private static void PrepareParameters(UpdatePublicationInfoParameters parameters)
		{
					}
				public static void Execute(UpdatePublicationInfoParameters parameters)
		{
						PrepareParameters(parameters);
					WebPublishInfo PublishInfo = UpdatePublicationInfoImplementation.GetTarget_PublishInfo(parameters.Owner, parameters.ContainerName);	
				}
				}
				public class CleanOldPublicationsParameters 
		{
				public TheBall.CORE.IContainerOwner Owner ;
				}
		
		public class CleanOldPublications 
		{
				private static void PrepareParameters(CleanOldPublicationsParameters parameters)
		{
					}
				public static void Execute(CleanOldPublicationsParameters parameters)
		{
						PrepareParameters(parameters);
					WebPublishInfo PublishInfo = CleanOldPublicationsImplementation.GetTarget_PublishInfo(parameters.Owner);	
				CleanOldPublicationsImplementation.ExecuteMethod_ClearPublications(PublishInfo);		
				}
				}
		
		public class SetCategoryContentRanking 
		{
				public static async Task ExecuteAsync()
		{
						
					INT.CategoryChildRanking RankingData = SetCategoryContentRankingImplementation.GetTarget_RankingData();	
				string CategoryID = SetCategoryContentRankingImplementation.GetTarget_CategoryID(RankingData);	
				ContentCategoryRankCollection ContentRankingCollection =  await SetCategoryContentRankingImplementation.GetTarget_ContentRankingCollectionAsync();	
				ContentCategoryRank[] CategoryRankingCollection = SetCategoryContentRankingImplementation.GetTarget_CategoryRankingCollection(CategoryID, ContentRankingCollection);	
				 await SetCategoryContentRankingImplementation.ExecuteMethod_SyncRankingItemsToRankingDataAsync(RankingData, CategoryRankingCollection);		
				}
				}
		
		public class SetCategoryHierarchyAndOrderInNodeSummary 
		{
				public static async Task ExecuteAsync()
		{
						
					INT.ParentToChildren[] Hierarchy = SetCategoryHierarchyAndOrderInNodeSummaryImplementation.GetTarget_Hierarchy();	
				 await SetCategoryHierarchyAndOrderInNodeSummaryImplementation.ExecuteMethod_SetParentCategoriesAsync(Hierarchy);		
				NodeSummaryContainer NodeSummaryContainer =  await SetCategoryHierarchyAndOrderInNodeSummaryImplementation.GetTarget_NodeSummaryContainerAsync();	
				SetCategoryHierarchyAndOrderInNodeSummaryImplementation.ExecuteMethod_SetCategoryOrder(Hierarchy, NodeSummaryContainer);		
				 await SetCategoryHierarchyAndOrderInNodeSummaryImplementation.ExecuteMethod_StoreObjectAsync(NodeSummaryContainer);		
				}
				}
				public class ProcessConnectionReceivedDataParameters 
		{
				public TheBall.CORE.Process Process ;
				}
		
		public class ProcessConnectionReceivedData 
		{
				private static void PrepareParameters(ProcessConnectionReceivedDataParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ProcessConnectionReceivedDataParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Interface.Connection Connection =  await ProcessConnectionReceivedDataImplementation.GetTarget_ConnectionAsync(parameters.Process);	
				string SourceContentRoot = ProcessConnectionReceivedDataImplementation.GetTarget_SourceContentRoot(Connection);	
				string TargetContentRoot = ProcessConnectionReceivedDataImplementation.GetTarget_TargetContentRoot();	
				Dictionary<string, string> CategoryMap = ProcessConnectionReceivedDataImplementation.GetTarget_CategoryMap(Connection);	
				ProcessConnectionReceivedDataImplementation.ExecuteMethod_CallMigrationSupport(parameters.Process, SourceContentRoot, TargetContentRoot, CategoryMap);		
				}
				}
				public class UpdateConnectionThisSideCategoriesParameters 
		{
				public TheBall.CORE.Process Process ;
				}
		
		public class UpdateConnectionThisSideCategories 
		{
				private static void PrepareParameters(UpdateConnectionThisSideCategoriesParameters parameters)
		{
					}
				public static async Task ExecuteAsync(UpdateConnectionThisSideCategoriesParameters parameters)
		{
						PrepareParameters(parameters);
					NodeSummaryContainer CurrentCategoryContainer =  await UpdateConnectionThisSideCategoriesImplementation.GetTarget_CurrentCategoryContainerAsync();	
				Category[] ActiveCategories = UpdateConnectionThisSideCategoriesImplementation.GetTarget_ActiveCategories(CurrentCategoryContainer);	
				TheBall.Interface.Connection Connection =  await UpdateConnectionThisSideCategoriesImplementation.GetTarget_ConnectionAsync(parameters.Process);	
				UpdateConnectionThisSideCategoriesImplementation.ExecuteMethod_UpdateThisSideCategories(Connection, ActiveCategories);		
				 await UpdateConnectionThisSideCategoriesImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				}
				}
				public class SetGroupAsDefaultForAccountParameters 
		{
				public string GroupID ;
				}
		
		public class SetGroupAsDefaultForAccount 
		{
				private static void PrepareParameters(SetGroupAsDefaultForAccountParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SetGroupAsDefaultForAccountParameters parameters)
		{
						PrepareParameters(parameters);
					AccountContainer AccountContainer =  await SetGroupAsDefaultForAccountImplementation.GetTarget_AccountContainerAsync();	
				string RedirectFromFolderBlobName = SetGroupAsDefaultForAccountImplementation.GetTarget_RedirectFromFolderBlobName();	
				SetGroupAsDefaultForAccountImplementation.ExecuteMethod_SetDefaultGroupValue(parameters.GroupID, AccountContainer);		
				 await SetGroupAsDefaultForAccountImplementation.ExecuteMethod_StoreObjectAsync(AccountContainer);		
				 await SetGroupAsDefaultForAccountImplementation.ExecuteMethod_SetAccountRedirectFileToGroupAsync(parameters.GroupID, RedirectFromFolderBlobName);		
				}
				}
		
		public class ClearDefaultGroupFromAccount 
		{
				public static async Task ExecuteAsync()
		{
						
					AccountContainer AccountContainer =  await ClearDefaultGroupFromAccountImplementation.GetTarget_AccountContainerAsync();	
				string RedirectFromFolderBlobName = ClearDefaultGroupFromAccountImplementation.GetTarget_RedirectFromFolderBlobName();	
				ClearDefaultGroupFromAccountImplementation.ExecuteMethod_ClearDefaultGroupValue(AccountContainer);		
				 await ClearDefaultGroupFromAccountImplementation.ExecuteMethod_StoreObjectAsync(AccountContainer);		
				 await ClearDefaultGroupFromAccountImplementation.ExecuteMethod_RemoveAccountRedirectFileAsync(RedirectFromFolderBlobName);		
				}
				}
		 } 