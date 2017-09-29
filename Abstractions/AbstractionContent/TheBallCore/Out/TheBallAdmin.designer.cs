 


using DOM=TheBall.Admin;
using System.Threading.Tasks;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static async Task DOMAININIT_TheBall_Admin(IContainerOwner owner)
		{
			await DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			await DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace TheBall.Admin { 
		using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;
using ProtoBuf;
using TheBall;
using TheBall.CORE;



namespace INT { 
					[DataContract]
			public partial class UsersData
			{
				[DataMember]
				public AccountInfo[] AccountInfos { get; set; }
			}

			[DataContract]
			public partial class AccountInfo
			{
				[DataMember]
				public string AccountID { get; set; }
				[DataMember]
				public string EmailAddress { get; set; }
			}

 } 		public static class DomainInformationSupport
		{
            public static async Task EnsureMasterCollections(IContainerOwner owner)
            {
            }

            public static async Task RefreshMasterCollections(IContainerOwner owner)
            {
            }
		}
 } 