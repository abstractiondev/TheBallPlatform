 

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
	    public delegate Task ExecuteOperationFunc(string operationName, object parameters = null);

	    public static ExecuteOperationFunc ExecuteOperation;

	    public delegate Task<object> GetObjectFunc(Type type, string id);

	    public static GetObjectFunc GetInformationObjectImplementation;
	    public static GetObjectFunc GetInterfaceObjectImplementation;


        private static async Task<T> GetInformationObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInformationObjectImplementation(type, id);
	        return (T) objResult;
	    }

	    private static async Task<T> GetInterfaceObject<T>(string id)
	    {
	        Type type = typeof(T);
	        var objResult = await GetInterfaceObjectImplementation(type, id);
	        return (T)objResult;
	    }


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