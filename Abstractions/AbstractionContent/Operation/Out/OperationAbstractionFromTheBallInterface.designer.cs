 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

		namespace TheBall.Interface { 
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
				public static void Execute(ExecuteRemoteCalledConnectionOperationParameters parameters)
		{
						PrepareParameters(parameters);
					INT.ConnectionCommunicationData ConnectionCommunicationData = ExecuteRemoteCalledConnectionOperationImplementation.GetTarget_ConnectionCommunicationData(parameters.InputStream);	
				ExecuteRemoteCalledConnectionOperationImplementation.ExecuteMethod_PerformOperation(ConnectionCommunicationData);		
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
				public static void Execute(PublishCollaborationContentOverConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = PublishCollaborationContentOverConnectionImplementation.GetTarget_Connection(parameters.ConnectionID);	
				
		{ // Local block to allow local naming
			SyncConnectionContentToDeviceToSendParameters operationParameters = PublishCollaborationContentOverConnectionImplementation.CallSyncConnectionContentToDeviceToSend_GetParameters(Connection);
			SyncConnectionContentToDeviceToSend.Execute(operationParameters);
									
		} // Local block closing
				bool CallDeviceSyncToSendContentOutput = PublishCollaborationContentOverConnectionImplementation.ExecuteMethod_CallDeviceSyncToSendContent(Connection);		
				PublishCollaborationContentOverConnectionImplementation.ExecuteMethod_CallOtherSideProcessingForCopiedContent(Connection, CallDeviceSyncToSendContentOutput);		
				}
				}
		
		public class SetCategoryLinkingForConnection 
		{
				public static void Execute()
		{
						
					INT.CategoryLinkParameters CategoryLinkingParameters = SetCategoryLinkingForConnectionImplementation.GetTarget_CategoryLinkingParameters();	
				Connection Connection = SetCategoryLinkingForConnectionImplementation.GetTarget_Connection(CategoryLinkingParameters);	
				SetCategoryLinkingForConnectionImplementation.ExecuteMethod_SetConnectionLinkingData(Connection, CategoryLinkingParameters);		
				SetCategoryLinkingForConnectionImplementation.ExecuteMethod_StoreObject(Connection);		
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
				public static void Execute(SyncConnectionContentToDeviceToSendParameters parameters)
		{
						PrepareParameters(parameters);
					string PackageContentListingProcessID = SyncConnectionContentToDeviceToSendImplementation.GetTarget_PackageContentListingProcessID(parameters.Connection);	
				SyncConnectionContentToDeviceToSendImplementation.ExecuteMethod_ExecuteContentListingProcess(PackageContentListingProcessID);		
				TheBall.CORE.Process PackageContentListingProcess = SyncConnectionContentToDeviceToSendImplementation.GetTarget_PackageContentListingProcess(PackageContentListingProcessID);	
				TheBall.CORE.INT.ContentItemLocationWithMD5[] ContentListingResult = SyncConnectionContentToDeviceToSendImplementation.GetTarget_ContentListingResult(PackageContentListingProcess);	
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
				public static void Execute(PackageAndPushCollaborationContentParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = PackageAndPushCollaborationContentImplementation.GetTarget_Connection(parameters.ConnectionID);	
				string PackageContentListingOperationName = PackageAndPushCollaborationContentImplementation.GetTarget_PackageContentListingOperationName(Connection);	
				string[] DynamicPackageListingOperationOutput = PackageAndPushCollaborationContentImplementation.ExecuteMethod_DynamicPackageListingOperation(parameters.ConnectionID, PackageContentListingOperationName);		
				TransferPackage TransferPackage = PackageAndPushCollaborationContentImplementation.GetTarget_TransferPackage(parameters.ConnectionID);	
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_AddTransferPackageToConnection(Connection, TransferPackage);		
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_StoreObject(Connection);		
				string[] PackageTransferPackageContentOutput = PackageAndPushCollaborationContentImplementation.ExecuteMethod_PackageTransferPackageContent(TransferPackage, DynamicPackageListingOperationOutput);		
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_SendTransferPackageContent(Connection, TransferPackage, PackageTransferPackageContentOutput);		
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_SetTransferPackageAsProcessed(TransferPackage);		
				PackageAndPushCollaborationContentImplementation.ExecuteMethod_StoreObjectComplete(Connection, TransferPackage);		
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
					TheBall.CORE.IContainerOwner QueueOwner = PutInterfaceOperationToQueueImplementation.GetTarget_QueueOwner();	
				string QueueLocation = PutInterfaceOperationToQueueImplementation.GetTarget_QueueLocation();	
				TheBall.CORE.IContainerOwner OperationOwner = PutInterfaceOperationToQueueImplementation.GetTarget_OperationOwner();	
				TheBall.CORE.IAccountInfo InvokerAccount = PutInterfaceOperationToQueueImplementation.GetTarget_InvokerAccount();	
				string QueueItemFileNameFormat = PutInterfaceOperationToQueueImplementation.GetTarget_QueueItemFileNameFormat();	
				string QueueItemFullPath = PutInterfaceOperationToQueueImplementation.GetTarget_QueueItemFullPath(parameters.OperationID, QueueItemFileNameFormat, QueueOwner, QueueLocation, OperationOwner);	
				 await PutInterfaceOperationToQueueImplementation.ExecuteMethod_CreateQueueEntryAsync(parameters.OperationID, QueueItemFullPath, InvokerAccount);		
				}
				}
				public class LockInterfaceOperationsByOwnerParameters 
		{
				public TheBall.CORE.IContainerOwner DedicatedToOwner ;
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
					TheBall.CORE.IContainerOwner QueueOwner = LockInterfaceOperationsByOwnerImplementation.GetTarget_QueueOwner();	
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
				public TheBall.CORE.IContainerOwner Owner ;
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
				public static void Execute(DeleteConnectionWithStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = DeleteConnectionWithStructuresImplementation.GetTarget_Connection(parameters.ConnectionID);	
				DeleteConnectionWithStructuresImplementation.ExecuteMethod_CallDeleteOnOtherEndAndDeleteOtherEndDevice(parameters.IsLaunchedByRemoteDelete, Connection);		
				DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionIntermediateContent(Connection);		
				DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionProcesses(Connection);		
				DeleteConnectionWithStructuresImplementation.ExecuteMethod_DeleteConnectionObject(Connection);		
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
				public static void Execute(InitiateIntegrationConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = InitiateIntegrationConnectionImplementation.GetTarget_Connection(parameters.Description);	
				TheBall.CORE.AuthenticatedAsActiveDevice DeviceForConnection = InitiateIntegrationConnectionImplementation.GetTarget_DeviceForConnection(parameters.Description, parameters.TargetBallHostName, parameters.TargetGroupID, Connection);	
				InitiateIntegrationConnectionImplementation.ExecuteMethod_StoreConnection(Connection);		
				InitiateIntegrationConnectionImplementation.ExecuteMethod_NegotiateDeviceConnection(DeviceForConnection);		
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
				public static void Execute(ExecuteConnectionProcessParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = ExecuteConnectionProcessImplementation.GetTarget_Connection(parameters.ConnectionID);	
				ExecuteConnectionProcessImplementation.ExecuteMethod_PerformProcessExecution(parameters.ConnectionProcessToExecute, Connection);		
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
				public static void Execute(FinalizeConnectionAfterGroupAuthorizationParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = FinalizeConnectionAfterGroupAuthorizationImplementation.GetTarget_Connection(parameters.ConnectionID);	
				INT.ConnectionCommunicationData ConnectionCommunicationData = FinalizeConnectionAfterGroupAuthorizationImplementation.GetTarget_ConnectionCommunicationData(Connection);	
				FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_CallDeviceServiceForFinalizing(Connection, ConnectionCommunicationData);		
				FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_UpdateConnectionWithCommunicationData(Connection, ConnectionCommunicationData);		
				FinalizeConnectionAfterGroupAuthorizationImplementation.ExecuteMethod_StoreObject(Connection);		
				
		{ // Local block to allow local naming
			CreateConnectionStructuresParameters operationParameters = FinalizeConnectionAfterGroupAuthorizationImplementation.CallCreateConnectionStructures_GetParameters(Connection);
			var operationReturnValue = CreateConnectionStructures.Execute(operationParameters);
									
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
				public static CreateConnectionStructuresReturnValue Execute(CreateConnectionStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = CreateConnectionStructuresImplementation.GetTarget_Connection(parameters.ConnectionID);	
				TheBall.CORE.Process ProcessToListPackageContents = CreateConnectionStructuresImplementation.GetTarget_ProcessToListPackageContents(Connection);	
				TheBall.CORE.Process ProcessToProcessReceivedData = CreateConnectionStructuresImplementation.GetTarget_ProcessToProcessReceivedData(Connection);	
				TheBall.CORE.Process ProcessToUpdateThisSideCategories = CreateConnectionStructuresImplementation.GetTarget_ProcessToUpdateThisSideCategories(Connection);	
				CreateConnectionStructuresImplementation.ExecuteMethod_SetConnectionProcesses(Connection, ProcessToListPackageContents, ProcessToProcessReceivedData, ProcessToUpdateThisSideCategories);		
				CreateConnectionStructuresImplementation.ExecuteMethod_StoreObject(Connection);		
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
				public static void Execute(CreateReceivingConnectionStructuresParameters parameters)
		{
						PrepareParameters(parameters);
					Connection ThisSideConnection = CreateReceivingConnectionStructuresImplementation.GetTarget_ThisSideConnection(parameters.ConnectionCommunicationData);	
				CreateReceivingConnectionStructuresImplementation.ExecuteMethod_StoreObject(ThisSideConnection);		
				
		{ // Local block to allow local naming
			CreateConnectionStructuresParameters operationParameters = CreateReceivingConnectionStructuresImplementation.CallCreateConnectionStructures_GetParameters(ThisSideConnection);
			var operationReturnValue = CreateConnectionStructures.Execute(operationParameters);
									
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
				public static CreateReceivingConnectionReturnValue Execute(CreateReceivingConnectionParameters parameters)
		{
						PrepareParameters(parameters);
					Connection Connection = CreateReceivingConnectionImplementation.GetTarget_Connection(parameters.Description, parameters.OtherSideConnectionID);	
				CreateReceivingConnectionImplementation.ExecuteMethod_StoreConnection(Connection);		
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
				public static void Execute(SynchronizeConnectionCategoriesParameters parameters)
		{
						PrepareParameters(parameters);
					SynchronizeConnectionCategoriesImplementation.ExecuteMethod_ExecuteProcessToUpdateThisSideCategories(parameters.ConnectionID);		
				TheBall.Interface.Connection Connection = SynchronizeConnectionCategoriesImplementation.GetTarget_Connection(parameters.ConnectionID);	
				Category[] SyncCategoriesWithOtherSideCategoriesOutput = SynchronizeConnectionCategoriesImplementation.ExecuteMethod_SyncCategoriesWithOtherSideCategories(Connection);		
				SynchronizeConnectionCategoriesImplementation.ExecuteMethod_UpdateOtherSideCategories(Connection, SyncCategoriesWithOtherSideCategoriesOutput);		
				SynchronizeConnectionCategoriesImplementation.ExecuteMethod_StoreObject(Connection);		
				}
				}
				public class ShareCollabInterfaceObjectParameters 
		{
				public string ColTargetType ;
				public string ColTargetID ;
				public string FileName ;
				}
		
		public class ShareCollabInterfaceObject 
		{
				private static void PrepareParameters(ShareCollabInterfaceObjectParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ShareCollabInterfaceObjectParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.CORE.IContainerOwner CollaborationTarget = ShareCollabInterfaceObjectImplementation.GetTarget_CollaborationTarget(parameters.ColTargetType, parameters.ColTargetID);	
				string SourceFullPath = ShareCollabInterfaceObjectImplementation.GetTarget_SourceFullPath(parameters.FileName);	
				string MetadataFullPath = ShareCollabInterfaceObjectImplementation.GetTarget_MetadataFullPath(parameters.FileName, CollaborationTarget);	
				INT.ShareInfo MetadataObject =  await ShareCollabInterfaceObjectImplementation.GetTarget_MetadataObjectAsync(parameters.FileName, SourceFullPath);	
				 await ShareCollabInterfaceObjectImplementation.ExecuteMethod_StoreShareMetadataAsync(MetadataFullPath, MetadataObject);		
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
				public static void Execute(FetchURLAsGroupContentParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.CORE.IContainerOwner Owner = FetchURLAsGroupContentImplementation.GetTarget_Owner(parameters.GroupID);	
				AaltoGlobalImpact.OIP.BinaryFile BinaryFile = FetchURLAsGroupContentImplementation.GetTarget_BinaryFile(parameters.FileName, Owner);	
				FetchURLAsGroupContentImplementation.ExecuteMethod_FetchDataAndAttachToFile(parameters.DataURL, BinaryFile);		
				}
				}
		 } 