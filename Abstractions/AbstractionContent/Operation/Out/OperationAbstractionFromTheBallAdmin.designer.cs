 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

		namespace TheBall.Admin { 
				public class FixGroupMastersAndCollectionsParameters 
		{
				public string GroupID ;
				}
		
		public class FixGroupMastersAndCollections 
		{
				private static void PrepareParameters(FixGroupMastersAndCollectionsParameters parameters)
		{
					}
				public static void Execute(FixGroupMastersAndCollectionsParameters parameters)
		{
						PrepareParameters(parameters);
					FixGroupMastersAndCollectionsImplementation.ExecuteMethod_FixMastersAndCollections(parameters.GroupID);		
				}
				}
		 } 