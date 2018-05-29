 

namespace SER.TheBall.Admin { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
//using ProtoBuf;
using System.Threading.Tasks;


namespace INT { 
		            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Admin.INT")]
			public partial class UsersData
			{
				[DataMember]
				public AccountInfo[] AccountInfos { get; set; }
			}

            [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TheBall.Admin.INT")]
			public partial class AccountInfo
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string EmailAddress { get; set; }
			}

 } 	#region Operation Calls
	public partial class Server 
	{

		// TODO: Implement in partial 
		//public static async Task ExecuteOperation(string operationName, object parameters) 

		// TODO: Implement in partial 

		// TODO: Implement in partial 


		public static async Task UpdateUsersData() 
		{
			await ExecuteOperation("TheBall.Admin.UpdateUsersData");
		}
		public static async Task<INT.UsersData> GetUsersData(string id = null)
		{
			var result = await GetInterfaceObject<INT.UsersData>(id);
			return result;
		}
		public static async Task<INT.AccountInfo> GetAccountInfo(string id = null)
		{
			var result = await GetInterfaceObject<INT.AccountInfo>(id);
			return result;
		}
	}
#endregion
 } 