 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace PersonalWeb.Diosphere { 
				public class SaveRoomDataParameters 
		{
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
				SaveRoomDataImplementation.ExecuteMethod_SaveJSONContentToBlob(parameters.JSONData, OwnerRootRoomBlobName);		
				}
				}
		 } 