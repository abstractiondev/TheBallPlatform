 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace PersonalWeb.Diosphere { 
				public class SaveRoomDataParameters 
		{
				public string RoomID ;
				public string JSONData ;
				}
		
		public class SaveRoomData 
		{
				private static void PrepareParameters(SaveRoomDataParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SaveRoomDataParameters parameters)
		{
						PrepareParameters(parameters);
					string OwnerRootRoomBlobName = SaveRoomDataImplementation.GetTarget_OwnerRootRoomBlobName();	
				TheBall.Core.IContainerOwner Owner = SaveRoomDataImplementation.GetTarget_Owner(parameters.RoomID);	
				 await SaveRoomDataImplementation.ExecuteMethod_SaveJSONContentToBlobAsync(parameters.JSONData, Owner, OwnerRootRoomBlobName);		
				}
				}
		 } 