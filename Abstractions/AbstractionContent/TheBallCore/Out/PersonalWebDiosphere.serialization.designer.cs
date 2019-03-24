 

namespace SER.PersonalWeb.Diosphere { 
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
		 } 	
	#region Operation Calls
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

	}
#endregion
 } 