 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace TheBall.Infrastructure { 
		
		public class UpdateInfraDataInterfaceObjects 
		{
				public static async Task ExecuteAsync()
		{
						
					Microsoft.WindowsAzure.Storage.File.CloudFileShare MainConfigShare = UpdateInfraDataInterfaceObjectsImplementation.GetTarget_MainConfigShare();	
				INT.UpdateConfig UpdateConfig =  await UpdateInfraDataInterfaceObjectsImplementation.GetTarget_UpdateConfigAsync(MainConfigShare);	
				INT.WebConsoleConfig WebConsoleConfig =  await UpdateInfraDataInterfaceObjectsImplementation.GetTarget_WebConsoleConfigAsync(MainConfigShare);	
				Microsoft.WindowsAzure.Storage.File.CloudFileShare DeploymentShare = UpdateInfraDataInterfaceObjectsImplementation.GetTarget_DeploymentShare(UpdateConfig);	
				INT.DeploymentPackages DeploymentPackages =  await UpdateInfraDataInterfaceObjectsImplementation.GetTarget_DeploymentPackagesAsync(DeploymentShare);	
				 await UpdateInfraDataInterfaceObjectsImplementation.ExecuteMethod_StoreObjectsAsync(UpdateConfig, WebConsoleConfig, DeploymentPackages);		
				}
				}
				public class SetRuntimeVersionsParameters 
		{
				public INT.UpdateConfig RuntimeVersionData ;
				}
		
		public class SetRuntimeVersions 
		{
				private static void PrepareParameters(SetRuntimeVersionsParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SetRuntimeVersionsParameters parameters)
		{
						PrepareParameters(parameters);
					Microsoft.WindowsAzure.Storage.File.CloudFileShare MainConfigShare = SetRuntimeVersionsImplementation.GetTarget_MainConfigShare();	
				INT.UpdateConfig UpdateConfig =  await SetRuntimeVersionsImplementation.GetTarget_UpdateConfigAsync(MainConfigShare);	
				Microsoft.WindowsAzure.Storage.File.CloudFileShare DeploymentShare = SetRuntimeVersionsImplementation.GetTarget_DeploymentShare(UpdateConfig);	
				INT.DeploymentPackages DeploymentPackages =  await SetRuntimeVersionsImplementation.GetTarget_DeploymentPackagesAsync(DeploymentShare);	
				SetRuntimeVersionsImplementation.ExecuteMethod_ValidateRequestedVersionsAgainstDeploymentPackages(parameters.RuntimeVersionData, DeploymentPackages);		
				INT.WebConsoleConfig WebConsoleConfig =  await SetRuntimeVersionsImplementation.GetTarget_WebConsoleConfigAsync(MainConfigShare);	
				SetRuntimeVersionsImplementation.ExecuteMethod_UpdatePlatformConfigurations(parameters.RuntimeVersionData, UpdateConfig, WebConsoleConfig);		
				 await SetRuntimeVersionsImplementation.ExecuteMethod_SaveConfigurationAsync(MainConfigShare, UpdateConfig, WebConsoleConfig);		
				}
				}
		
		public class CreateCloudDrive 
		{
				}
		
		public class MountCloudDrive 
		{
				}
		 } 