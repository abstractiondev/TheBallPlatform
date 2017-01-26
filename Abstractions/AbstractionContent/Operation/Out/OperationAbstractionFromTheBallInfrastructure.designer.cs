 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
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

		    public class CreateCloudDrive 
		{
				}
		
		public class MountCloudDrive 
		{
				}
		 } 