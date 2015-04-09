 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
				public static void Execute(SaveRoomDataParameters parameters)
		{
						PrepareParameters(parameters);
					string OwnerRootRoomBlobName = SaveRoomDataImplementation.GetTarget_OwnerRootRoomBlobName();	
				TheBall.CORE.IContainerOwner Owner = SaveRoomDataImplementation.GetTarget_Owner(parameters.RoomID);	
				SaveRoomDataImplementation.ExecuteMethod_SaveJSONContentToBlob(parameters.JSONData, Owner, OwnerRootRoomBlobName);		
				}
				}
		 } 