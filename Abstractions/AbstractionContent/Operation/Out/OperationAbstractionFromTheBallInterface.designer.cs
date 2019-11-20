 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace TheBall.Interface { 
				public class SendEmailParameters 
		{
				public INT.EmailPackage EmailInfo ;
				}
		
		public class SendEmail 
		{
				private static void PrepareParameters(SendEmailParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SendEmailParameters parameters)
		{
						PrepareParameters(parameters);
					SendEmailImplementation.ExecuteMethod_ValidateThatEmailSendingIsAllowed();		
				string[] EmailAddresses =  await SendEmailImplementation.GetTarget_EmailAddressesAsync(parameters.EmailInfo.RecipientAccountIDs);	
				TheBall.EmailSupport.FileAttachment[] Attachments =  await SendEmailImplementation.GetTarget_AttachmentsAsync(parameters.EmailInfo.Attachments);	
				 await SendEmailImplementation.ExecuteMethod_SendEmailsAsync(parameters.EmailInfo, EmailAddresses, Attachments);		
				}
				}
				public class ExecuteRemoteCalledConnectionOperationParameters 
		{
				public System.IO.Stream InputStream ;
				public System.IO.Stream OutputStream ;
				}
		
		public class ExecuteRemoteCalledConnectionOperation 
		{
				private static void PrepareParameters(ExecuteRemoteCalledConnectionOperationParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ExecuteRemoteCalledConnectionOperationParameters parameters)
		{
						PrepareParameters(parameters);
					INT.ConnectionCommunicationData ConnectionCommunicationData = ExecuteRemoteCalledConnectionOperationImplementation.GetTarget_ConnectionCommunicationData(parameters.InputStream);	
				 await ExecuteRemoteCalledConnectionOperationImplementation.ExecuteMethod_PerformOperationAsync(ConnectionCommunicationData);		
				ExecuteRemoteCalledConnectionOperationImplementation.ExecuteMethod_SerializeCommunicationDataToOutput(parameters.OutputStream, ConnectionCommunicationData);		
				}
				}
				public class PublishCollaborationContentOverConnectionParameters 
		{
				public string ConnectionID ;
				}
		
		public class PublishCollaborationContentOverConnection 
		{
				private static void PrepareParameters(PublishCollaborationContentOverConnectionParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PublishCollaborationContentOverConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await PublishCollaborationContentOverConnectionImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				
		{ // Local block to allow local naming
			SyncConnectionContentToDeviceToSendParameters operationParameters = PublishCollaborationContentOverConnectionImplementation.CallSyncConnectionContentToDeviceToSend_GetParameters(Connection);
			 await SyncConnectionContentToDeviceToSend.ExecuteAsync(operationParameters);
									
		} // Local block closing
				bool CallDeviceSyncToSendContentOutput =  await PublishCollaborationContentOverConnectionImplementation.ExecuteMethod_CallDeviceSyncToSendContentAsync(Connection);		
				 await PublishCollaborationContentOverConnectionImplementation.ExecuteMethod_CallOtherSideProcessingForCopiedContentAsync(Connection, CallDeviceSyncToSendContentOutput);		
				}
				}
		
		public class SetCategoryLinkingForConnection 
		{
				public static async Task ExecuteAsync()
		{
						
					INT.CategoryLinkParameters CategoryLinkingParameters = SetCategoryLinkingForConnectionImplementation.GetTarget_CategoryLinkingParameters();	
				Connection Connection =  await SetCategoryLinkingForConnectionImplementation.GetTarget_ConnectionAsync(CategoryLinkingParameters);	
				SetCategoryLinkingForConnectionImplementation.ExecuteMethod_SetConnectionLinkingData(Connection, CategoryLinkingParameters);		
				 await SetCategoryLinkingForConnectionImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				}
				}
				public class SyncConnectionContentToDeviceToSendParameters 
		{
				public Connection Connection ;
				}
		
		public class SyncConnectionContentToDeviceToSend 
		{
				private static void PrepareParameters(SyncConnectionContentToDeviceToSendParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SyncConnectionContentToDeviceToSendParameters parameters)
		{
						PrepareParameters(parameters);
					string PackageContentListingProcessID = SyncConnectionContentToDeviceToSendImplementation.GetTarget_PackageContentListingProcessID(parameters.Connection);	
				 await SyncConnectionContentToDeviceToSendImplementation.ExecuteMethod_ExecuteContentListingProcessAsync(PackageContentListingProcessID);		
				TheBall.Core.Process PackageContentListingProcess =  await SyncConnectionContentToDeviceToSendImplementation.GetTarget_PackageContentListingProcessAsync(PackageContentListingProcessID);	
				TheBall.Core.INT.ContentItemLocationWithMD5[] ContentListingResult = SyncConnectionContentToDeviceToSendImplementation.GetTarget_ContentListingResult(PackageContentListingProcess);	
				string SyncTargetRootFolder = SyncConnectionContentToDeviceToSendImplementation.GetTarget_SyncTargetRootFolder(parameters.Connection);	
				SyncConnectionContentToDeviceToSendImplementation.ExecuteMethod_CopyContentsToSyncRoot(ContentListingResult, SyncTargetRootFolder);		
				}
				}
				public class PackageAndPushCollaborationContentParameters 
		{
				public string ConnectionID ;
				}
		
		public class PackageAndPushCollaborationContent 
		{
				private static void PrepareParameters(PackageAndPushCollaborationContentParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PackageAndPushCollaborationContentParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await PackageAndPushCollaborationContentImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				string PackageContentListingOperationName = PackageAndPushCollaborationContentImplementation.GetTarget_PackageContentListingOperationName(Connection);	
				string[] DynamicPackageListingOperationOutput = PackageAndPushCollaborationContentImplementation.ExecuteMethod_DynamicPackageListingOperation(parameters.ConnectionID, PackageContentListingOperationName);		
				TransferPackage TransferPackage = PackageAndPushCollaborationContentImplementation.GetTarget_TransferPackage(parameters.ConnectionID);	
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_AddTransferPackageToConnection(Connection, TransferPackage);		
				 await PackageAndPushCollaborationContentImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				string[] PackageTransferPackageContentOutput = PackageAndPushCollaborationContentImplementation.ExecuteMethod_PackageTransferPackageContent(TransferPackage, DynamicPackageListingOperationOutput);		
				 await PackageAndPushCollaborationContentImplementation.ExecuteMethod_SendTransferPackageContentAsync(Connection, TransferPackage, PackageTransferPackageContentOutput);		
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_SetTransferPackageAsProcessed(TransferPackage);		
				 await PackageAndPushCollaborationContentImplementation.ExecuteMethod_StoreObjectCompleteAsync(Connection, TransferPackage);		
				}
				}
				public class CreateInterfaceOperationForExecutionParameters 
		{
				public string DataType ;
				public byte[] OperationData ;
				}
		
		public class CreateInterfaceOperationForExecution 
		{
				private static void PrepareParameters(CreateInterfaceOperationForExecutionParameters parameters)
		{
					}
				public static async Task<CreateInterfaceOperationForExecutionReturnValue> ExecuteAsync(CreateInterfaceOperationForExecutionParameters parameters)
		{
						PrepareParameters(parameters);
					InterfaceOperation Operation = CreateInterfaceOperationForExecutionImplementation.GetTarget_Operation(parameters.DataType);	
				string OperationDataLocation = CreateInterfaceOperationForExecutionImplementation.GetTarget_OperationDataLocation(Operation);	
				 await CreateInterfaceOperationForExecutionImplementation.ExecuteMethod_StoreOperationWithDataAsync(parameters.OperationData, Operation, OperationDataLocation);		
				CreateInterfaceOperationForExecutionReturnValue returnValue = CreateInterfaceOperationForExecutionImplementation.Get_ReturnValue(Operation);
		return returnValue;
				}
				}
				public class CreateInterfaceOperationForExecutionReturnValue 
		{
				public string OperationID ;
				}
		
		public class ExecuteLegacyHttpPostRequest 
		{
				public static void Execute()
		{
						
					AzureSupport.HttpOperationData RequestData = ExecuteLegacyHttpPostRequestImplementation.GetTarget_RequestData();	
				ExecuteLegacyHttpPostRequestImplementation.ExecuteMethod_ExecutePostRequest(RequestData);		
				}
				}
				public class ExecuteInterfaceOperationParameters 
		{
				public string OperationID ;
				public bool UseSQLiteDB ;
				}
		
		public class ExecuteInterfaceOperation 
		{
				private static void PrepareParameters(ExecuteInterfaceOperationParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ExecuteInterfaceOperationParameters parameters)
		{
						PrepareParameters(parameters);
					InterfaceOperation Operation =  await ExecuteInterfaceOperationImplementation.GetTarget_OperationAsync(parameters.OperationID);	
				string OperationDataLocation = ExecuteInterfaceOperationImplementation.GetTarget_OperationDataLocation(Operation);	
				 await ExecuteInterfaceOperationImplementation.ExecuteMethod_PreExecuteSyncSQLiteFromStorageAsync(parameters.UseSQLiteDB, Operation);		
				 await ExecuteInterfaceOperationImplementation.ExecuteMethod_ExecuteOperationAsync(Operation, OperationDataLocation);		
				 await ExecuteInterfaceOperationImplementation.ExecuteMethod_PostExecuteSyncStorageFromSQLiteAsync(parameters.UseSQLiteDB, Operation);		
				}
				}
				public class DeleteInterfaceOperationParameters 
		{
				public string OperationID ;
				}
		
		public class DeleteInterfaceOperation 
		{
				private static void PrepareParameters(DeleteInterfaceOperationParameters parameters)
		{
					}
				public static async Task ExecuteAsync(DeleteInterfaceOperationParameters parameters)
		{
						PrepareParameters(parameters);
					string OperationBlobLocation = DeleteInterfaceOperationImplementation.GetTarget_OperationBlobLocation(parameters.OperationID);	
				string OperationDataBlobLocation = DeleteInterfaceOperationImplementation.GetTarget_OperationDataBlobLocation(OperationBlobLocation);	
				 await DeleteInterfaceOperationImplementation.ExecuteMethod_DeleteOperationWithDataAsync(OperationBlobLocation, OperationDataBlobLocation);		
				}
				}
				public class PutInterfaceOperationToQueueParameters 
		{
				public string OperationID ;
				}
		
		public class PutInterfaceOperationToQueue 
		{
				private static void PrepareParameters(PutInterfaceOperationToQueueParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PutInterfaceOperationToQueueParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.IContainerOwner QueueOwner = PutInterfaceOperationToQueueImplementation.GetTarget_QueueOwner();	
				string QueueLocation = PutInterfaceOperationToQueueImplementation.GetTarget_QueueLocation();	
				TheBall.Core.IContainerOwner OperationOwner = PutInterfaceOperationToQueueImplementation.GetTarget_OperationOwner();	
				TheBall.Core.IAccountInfo InvokerAccount = PutInterfaceOperationToQueueImplementation.GetTarget_InvokerAccount();	
				string QueueItemFileNameFormat = PutInterfaceOperationToQueueImplementation.GetTarget_QueueItemFileNameFormat();	
				string QueueItemFullPath = PutInterfaceOperationToQueueImplementation.GetTarget_QueueItemFullPath(parameters.OperationID, QueueItemFileNameFormat, QueueOwner, QueueLocation, OperationOwner);	
				 await PutInterfaceOperationToQueueImplementation.ExecuteMethod_CreateQueueEntryAsync(parameters.OperationID, QueueItemFullPath, InvokerAccount);		
				}
				}
				public class LockInterfaceOperationsByOwnerParameters 
		{
				public TheBall.Core.IContainerOwner DedicatedToOwner ;
				}
		
		public class LockInterfaceOperationsByOwner 
		{
				private static void PrepareParameters(LockInterfaceOperationsByOwnerParameters parameters)
		{
					}
				public class AcquireFirstObtainableLockReturnValue 
		{
				public string LockedOwnerPrefix ;
				public string LockedOwnerID ;
				public string[] OperationIDs ;
				public string[] OperationQueueItems ;
				public string LockBlobFullPath ;
				}
				public static async Task<LockInterfaceOperationsByOwnerReturnValue> ExecuteAsync(LockInterfaceOperationsByOwnerParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.IContainerOwner QueueOwner = LockInterfaceOperationsByOwnerImplementation.GetTarget_QueueOwner();	
				string QueueLocation = LockInterfaceOperationsByOwnerImplementation.GetTarget_QueueLocation();	
				IEnumerable<System.Linq.IGrouping<string, string>> OwnerGroupedItems =  await LockInterfaceOperationsByOwnerImplementation.GetTarget_OwnerGroupedItemsAsync(QueueOwner, QueueLocation);	
				string LockFileNameFormat = LockInterfaceOperationsByOwnerImplementation.GetTarget_LockFileNameFormat();	
				AcquireFirstObtainableLockReturnValue AcquireFirstObtainableLockOutput =  await LockInterfaceOperationsByOwnerImplementation.ExecuteMethod_AcquireFirstObtainableLockAsync(parameters.DedicatedToOwner, OwnerGroupedItems, QueueOwner, QueueLocation, LockFileNameFormat);		
				LockInterfaceOperationsByOwnerReturnValue returnValue = LockInterfaceOperationsByOwnerImplementation.Get_ReturnValue(AcquireFirstObtainableLockOutput);
		return returnValue;
				}
				}
				public class LockInterfaceOperationsByOwnerReturnValue 
		{
				public string LockedOwnerPrefix ;
				public string LockedOwnerID ;
				public string[] OperationIDs ;
				public string[] OperationQueueItems ;
				public string LockBlobFullPath ;
				}
				public class ExecuteInterfaceOperationsByOwnerAndReleaseLockParameters 
		{
				public string InstanceName ;
				public string LockedOwnerPrefix ;
				public string LockedOwnerID ;
				public string[] OperationIDs ;
				public string[] OperationQueueItems ;
				public string LockBlobFullPath ;
				}
		
		public class ExecuteInterfaceOperationsByOwnerAndReleaseLock 
		{
				private static void PrepareParameters(ExecuteInterfaceOperationsByOwnerAndReleaseLockParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ExecuteInterfaceOperationsByOwnerAndReleaseLockParameters parameters)
		{
						PrepareParameters(parameters);
					 await ExecuteInterfaceOperationsByOwnerAndReleaseLockImplementation.ExecuteMethod_ExecuteOperationsAndReleaseLockAsync(parameters.InstanceName, parameters.LockedOwnerPrefix, parameters.LockedOwnerID, parameters.OperationIDs, parameters.OperationQueueItems, parameters.LockBlobFullPath);		
				}
				}
				public class UpdateStatusSummaryParameters 
		{
				public TheBall.Core.IContainerOwner Owner ;
				public DateTime UpdateTime ;
				public string[] ChangedIDList ;
				public int RemoveExpiredEntriesSeconds ;
				}
		
		public class UpdateStatusSummary 
		{
				private static void PrepareParameters(UpdateStatusSummaryParameters parameters)
		{
					}
				public static async Task ExecuteAsync(UpdateStatusSummaryParameters parameters)
		{
						PrepareParameters(parameters);
					 await UpdateStatusSummaryImplementation.ExecuteMethod_EnsureUpdateOnStatusSummaryAsync(parameters.Owner, parameters.UpdateTime, parameters.ChangedIDList, parameters.RemoveExpiredEntriesSeconds);		
				}
				}
				public class DeleteConnectionWithStructuresParameters 
		{
				public string ConnectionID ;
				public bool IsLaunchedByRemoteDelete ;
				}
		
		public class DeleteConnectionWithStructures 
		{
				private static void PrepareParameters(DeleteConnectionWithStructuresParameters parameters)
		{
					}
				public static async Task ExecuteAsync(DeleteConnectionWithStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await DeleteConnectionWithStructuresImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				 await DeleteConnectionWithStructuresImplementation.ExecuteMethod_CallDeleteOnOtherEndAndDeleteOtherEndDeviceAsync(parameters.IsLaunchedByRemoteDelete, Connection);		
				 await DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionIntermediateContentAsync(Connection);		
				 await DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionProcessesAsync(Connection);		
				 await DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionObjectAsync(Connection);		
				}
				}
				public class InitiateIntegrationConnectionParameters 
		{
				public string Description ;
				public string TargetBallHostName ;
				public string TargetGroupID ;
				}
		
		public class InitiateIntegrationConnection 
		{
				private static void PrepareParameters(InitiateIntegrationConnectionParameters parameters)
		{
					}
				public static async Task ExecuteAsync(InitiateIntegrationConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = InitiateIntegrationConnectionImplementation.GetTarget_Connection(parameters.Description);	
				TheBall.Core.AuthenticatedAsActiveDevice DeviceForConnection =  await InitiateIntegrationConnectionImplementation.GetTarget_DeviceForConnectionAsync(parameters.Description, parameters.TargetBallHostName, parameters.TargetGroupID, Connection);	
				 await InitiateIntegrationConnectionImplementation.ExecuteMethod_StoreConnectionAsync(Connection);		
				 await InitiateIntegrationConnectionImplementation.ExecuteMethod_NegotiateDeviceConnectionAsync(DeviceForConnection);		
				}
				}
				public class ExecuteConnectionProcessParameters 
		{
				public string ConnectionID ;
				public string ConnectionProcessToExecute ;
				}
		
		public class ExecuteConnectionProcess 
		{
				private static void PrepareParameters(ExecuteConnectionProcessParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ExecuteConnectionProcessParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await ExecuteConnectionProcessImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				 await ExecuteConnectionProcessImplementation.ExecuteMethod_PerformProcessExecutionAsync(parameters.ConnectionProcessToExecute, Connection);		
				}
				}
				public class FinalizeConnectionAfterGroupAuthorizationParameters 
		{
				public string ConnectionID ;
				}
		
		public class FinalizeConnectionAfterGroupAuthorization 
		{
				private static void PrepareParameters(FinalizeConnectionAfterGroupAuthorizationParameters parameters)
		{
					}
				public static async Task ExecuteAsync(FinalizeConnectionAfterGroupAuthorizationParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await FinalizeConnectionAfterGroupAuthorizationImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				INT.ConnectionCommunicationData ConnectionCommunicationData = FinalizeConnectionAfterGroupAuthorizationImplementation.GetTarget_ConnectionCommunicationData(Connection);	
				 await FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_CallDeviceServiceForFinalizingAsync(Connection, ConnectionCommunicationData);		
				FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_UpdateConnectionWithCommunicationData(Connection, ConnectionCommunicationData);		
				 await FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				
		{ // Local block to allow local naming
			CreateConnectionStructuresParameters operationParameters = FinalizeConnectionAfterGroupAuthorizationImplementation.CallCreateConnectionStructures_GetParameters(Connection);
			var operationReturnValue =  await CreateConnectionStructures.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
				public class CreateConnectionStructuresParameters 
		{
				public string ConnectionID ;
				}
		
		public class CreateConnectionStructures 
		{
				private static void PrepareParameters(CreateConnectionStructuresParameters parameters)
		{
					}
				public static async Task<CreateConnectionStructuresReturnValue> ExecuteAsync(CreateConnectionStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection =  await CreateConnectionStructuresImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				TheBall.Core.Process ProcessToListPackageContents =  await CreateConnectionStructuresImplementation.GetTarget_ProcessToListPackageContentsAsync(Connection);	
				TheBall.Core.Process ProcessToProcessReceivedData =  await CreateConnectionStructuresImplementation.GetTarget_ProcessToProcessReceivedDataAsync(Connection);	
				TheBall.Core.Process ProcessToUpdateThisSideCategories =  await CreateConnectionStructuresImplementation.GetTarget_ProcessToUpdateThisSideCategoriesAsync(Connection);	
				CreateConnectionStructuresImplementation.ExecuteMethod_SetConnectionProcesses(Connection, ProcessToListPackageContents, ProcessToProcessReceivedData, ProcessToUpdateThisSideCategories);		
				 await CreateConnectionStructuresImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				CreateConnectionStructuresReturnValue returnValue = CreateConnectionStructuresImplementation.Get_ReturnValue(Connection);
		return returnValue;
				}
				}
				public class CreateConnectionStructuresReturnValue 
		{
				public Connection UpdatedConnection ;
				}
				public class CreateReceivingConnectionStructuresParameters 
		{
				public INT.ConnectionCommunicationData ConnectionCommunicationData ;
				}
		
		public class CreateReceivingConnectionStructures 
		{
				private static void PrepareParameters(CreateReceivingConnectionStructuresParameters parameters)
		{
					}
				public static async Task ExecuteAsync(CreateReceivingConnectionStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection ThisSideConnection =  await CreateReceivingConnectionStructuresImplementation.GetTarget_ThisSideConnectionAsync(parameters.ConnectionCommunicationData);	
				 await CreateReceivingConnectionStructuresImplementation.ExecuteMethod_StoreObjectAsync(ThisSideConnection);		
				
		{ // Local block to allow local naming
			CreateConnectionStructuresParameters operationParameters = CreateReceivingConnectionStructuresImplementation.CallCreateConnectionStructures_GetParameters(ThisSideConnection);
			var operationReturnValue =  await CreateConnectionStructures.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
				public class CreateReceivingConnectionParameters 
		{
				public string Description ;
				public string OtherSideConnectionID ;
				}
		
		public class CreateReceivingConnection 
		{
				private static void PrepareParameters(CreateReceivingConnectionParameters parameters)
		{
					}
				public static async Task<CreateReceivingConnectionReturnValue> ExecuteAsync(CreateReceivingConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = CreateReceivingConnectionImplementation.GetTarget_Connection(parameters.Description, parameters.OtherSideConnectionID);	
				 await CreateReceivingConnectionImplementation.ExecuteMethod_StoreConnectionAsync(Connection);		
				CreateReceivingConnectionReturnValue returnValue = CreateReceivingConnectionImplementation.Get_ReturnValue(Connection);
		return returnValue;
				}
				}
				public class CreateReceivingConnectionReturnValue 
		{
				public string ConnectionID ;
				}
				public class SynchronizeConnectionCategoriesParameters 
		{
				public string ConnectionID ;
				}
		
		public class SynchronizeConnectionCategories 
		{
				private static void PrepareParameters(SynchronizeConnectionCategoriesParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SynchronizeConnectionCategoriesParameters parameters)
		{
						PrepareParameters(parameters);
					 await SynchronizeConnectionCategoriesImplementation.ExecuteMethod_ExecuteProcessToUpdateThisSideCategoriesAsync(parameters.ConnectionID);		
				TheBall.Interface.Connection Connection =  await SynchronizeConnectionCategoriesImplementation.GetTarget_ConnectionAsync(parameters.ConnectionID);	
				Category[] SyncCategoriesWithOtherSideCategoriesOutput =  await SynchronizeConnectionCategoriesImplementation.ExecuteMethod_SyncCategoriesWithOtherSideCategoriesAsync(Connection);		
				SynchronizeConnectionCategoriesImplementation.ExecuteMethod_UpdateOtherSideCategories(Connection, SyncCategoriesWithOtherSideCategoriesOutput);		
				 await SynchronizeConnectionCategoriesImplementation.ExecuteMethod_StoreObjectAsync(Connection);		
				}
				}
				public class ShareCollabInterfaceObjectParameters 
		{
				public INT.ShareCollabParams CollabParams ;
				}
		
		public class ShareCollabInterfaceObject 
		{
				private static void PrepareParameters(ShareCollabInterfaceObjectParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ShareCollabInterfaceObjectParameters parameters)
		{
						PrepareParameters(parameters);
					string FileName = ShareCollabInterfaceObjectImplementation.GetTarget_FileName(parameters.CollabParams);	
				ShareCollabInterfaceObjectImplementation.ExecuteMethod_ValidateFileName(FileName);		
				TheBall.Core.IContainerOwner CollaborationTarget = ShareCollabInterfaceObjectImplementation.GetTarget_CollaborationTarget(parameters.CollabParams);	
				string SourceFullPath = ShareCollabInterfaceObjectImplementation.GetTarget_SourceFullPath(FileName);	
				string MetadataFullPath = ShareCollabInterfaceObjectImplementation.GetTarget_MetadataFullPath(FileName, CollaborationTarget);	
				INT.ShareInfo MetadataObject =  await ShareCollabInterfaceObjectImplementation.GetTarget_MetadataObjectAsync(FileName, SourceFullPath);	
				 await ShareCollabInterfaceObjectImplementation.ExecuteMethod_StoreShareMetadataAsync(MetadataFullPath, MetadataObject);		
				
		{ // Local block to allow local naming
			PushSyncNotificationParameters operationParameters = ShareCollabInterfaceObjectImplementation.NotifyPartner_GetParameters(parameters.CollabParams);
			 await PushSyncNotification.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
				public class PushSyncNotificationParameters 
		{
				public INT.CollaborationPartner Partner ;
				}
		
		public class PushSyncNotification 
		{
				private static void PrepareParameters(PushSyncNotificationParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PushSyncNotificationParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.IContainerOwner CollaborationTarget = PushSyncNotificationImplementation.GetTarget_CollaborationTarget(parameters.Partner);	
				AzureSupport.HttpOperationData SyncOperationData = PushSyncNotificationImplementation.GetTarget_SyncOperationData(CollaborationTarget);	
				 await PushSyncNotificationImplementation.ExecuteMethod_QueueSyncOperationToTargetAsync(CollaborationTarget, SyncOperationData);		
				}
				}
				public class PullSyncDataParameters 
		{
				public INT.CollaborationPartner Partner ;
				}
		
		public class PullSyncData 
		{
				private static void PrepareParameters(PullSyncDataParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PullSyncDataParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.IContainerOwner CollaborationSource = PullSyncDataImplementation.GetTarget_CollaborationSource(parameters.Partner);	
				TheBall.Core.IContainerOwner CollaborationTarget = PullSyncDataImplementation.GetTarget_CollaborationTarget();	
				string SyncSourceRoot = PullSyncDataImplementation.GetTarget_SyncSourceRoot(CollaborationTarget);	
				string SyncTargetRoot = PullSyncDataImplementation.GetTarget_SyncTargetRoot(CollaborationSource);	
				TheBall.Core.Storage.BlobStorageItem[] ExistingSourceItems =  await PullSyncDataImplementation.GetTarget_ExistingSourceItemsAsync(CollaborationSource, SyncSourceRoot);	
				TheBall.Core.Storage.BlobStorageItem[] ExistingTargetItems =  await PullSyncDataImplementation.GetTarget_ExistingTargetItemsAsync(CollaborationTarget, SyncTargetRoot);	
				 await PullSyncDataImplementation.ExecuteMethod_SyncItemsAsync(CollaborationSource, SyncSourceRoot, ExistingSourceItems, CollaborationTarget, SyncTargetRoot, ExistingTargetItems);		
				
		{ // Local block to allow local naming
			UpdateSharedDataSummaryDataParameters operationParameters = PullSyncDataImplementation.UpdateSummaryData_GetParameters(parameters.Partner);
			 await UpdateSharedDataSummaryData.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
				public class UpdateSharedDataSummaryDataParameters 
		{
				public INT.CollaborationPartner Partner ;
				}
		
		public class UpdateSharedDataSummaryData 
		{
				private static void PrepareParameters(UpdateSharedDataSummaryDataParameters parameters)
		{
					}
				public static async Task ExecuteAsync(UpdateSharedDataSummaryDataParameters parameters)
		{
						PrepareParameters(parameters);
					bool IsCompleteUpdate = UpdateSharedDataSummaryDataImplementation.GetTarget_IsCompleteUpdate(parameters.Partner);	
				TheBall.Core.IContainerOwner[] CollaborationPartners =  await UpdateSharedDataSummaryDataImplementation.GetTarget_CollaborationPartnersAsync(parameters.Partner, IsCompleteUpdate);	
				Tuple<TheBall.Core.IContainerOwner, string>[] UpdatePartnerSummariesOutput =  await UpdateSharedDataSummaryDataImplementation.ExecuteMethod_UpdatePartnerSummariesAsync(CollaborationPartners, IsCompleteUpdate);		
				 await UpdateSharedDataSummaryDataImplementation.ExecuteMethod_UpdateCompleteShareSummaryAsync(UpdatePartnerSummariesOutput, IsCompleteUpdate);		
				}
				}
				public class DeleteInterfaceJSONParameters 
		{
				public INT.InterfaceJSONData SaveDataInfo ;
				}
		
		public class DeleteInterfaceJSON 
		{
				private static void PrepareParameters(DeleteInterfaceJSONParameters parameters)
		{
					}
				public static async Task ExecuteAsync(DeleteInterfaceJSONParameters parameters)
		{
						PrepareParameters(parameters);
					string DataName = DeleteInterfaceJSONImplementation.GetTarget_DataName(parameters.SaveDataInfo);	
				string JSONDataFileLocation = DeleteInterfaceJSONImplementation.GetTarget_JSONDataFileLocation(DataName);	
				 await DeleteInterfaceJSONImplementation.ExecuteMethod_DeleteJSONDataAsync(JSONDataFileLocation);		
				}
				}
				public class SaveInterfaceJSONParameters 
		{
				public INT.InterfaceJSONData SaveDataInfo ;
				}
		
		public class SaveInterfaceJSON 
		{
				private static void PrepareParameters(SaveInterfaceJSONParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SaveInterfaceJSONParameters parameters)
		{
						PrepareParameters(parameters);
					string DataName = SaveInterfaceJSONImplementation.GetTarget_DataName(parameters.SaveDataInfo);	
				System.Dynamic.ExpandoObject DataObject = SaveInterfaceJSONImplementation.GetTarget_DataObject(parameters.SaveDataInfo);	
				string JSONDataFileLocation = SaveInterfaceJSONImplementation.GetTarget_JSONDataFileLocation(DataName);	
				 await SaveInterfaceJSONImplementation.ExecuteMethod_StoreJSONDataAsync(JSONDataFileLocation, DataObject);		
				}
				}
				public class SaveGroupDetailsParameters 
		{
				public INT.GroupDetails GroupDetails ;
				}
		
		public class SaveGroupDetails 
		{
				private static void PrepareParameters(SaveGroupDetailsParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SaveGroupDetailsParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.Group Group =  await SaveGroupDetailsImplementation.GetTarget_GroupAsync();	
				 await SaveGroupDetailsImplementation.ExecuteMethod_SaveGroupDetailsAsync(parameters.GroupDetails, Group);		
				TheBall.Core.GroupMembership[] CurrentMemberships =  await SaveGroupDetailsImplementation.GetTarget_CurrentMembershipsAsync(Group);	
				 await SaveGroupDetailsImplementation.ExecuteMethod_UpdateDetailsChangeToMembersAsync(Group, CurrentMemberships);		
				}
				}
				public class FetchURLAsGroupContentParameters 
		{
				public string GroupID ;
				public string FileName ;
				public string DataURL ;
				}
		
		public class FetchURLAsGroupContent 
		{
				private static void PrepareParameters(FetchURLAsGroupContentParameters parameters)
		{
					}
				public static async Task ExecuteAsync(FetchURLAsGroupContentParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.Core.IContainerOwner Owner = FetchURLAsGroupContentImplementation.GetTarget_Owner(parameters.GroupID);	
				AaltoGlobalImpact.OIP.BinaryFile BinaryFile =  await FetchURLAsGroupContentImplementation.GetTarget_BinaryFileAsync(parameters.FileName, Owner);	
				 await FetchURLAsGroupContentImplementation.ExecuteMethod_FetchDataAndAttachToFileAsync(parameters.DataURL, BinaryFile);		
				}
				}
		 } 