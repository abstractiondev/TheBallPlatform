 


using DOM=PersonalWeb.Diosphere;
using System.Threading.Tasks;

namespace TheBall.CORE {
	public static partial class OwnerInitializer
	{
		private static async Task DOMAININIT_PersonalWeb_Diosphere(IContainerOwner owner)
		{
			await DOM.DomainInformationSupport.EnsureMasterCollections(owner);
			await DOM.DomainInformationSupport.RefreshMasterCollections(owner);
		}
	}
}


namespace PersonalWeb.Diosphere { 
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
using TheBall.CORE.Storage;

namespace INT { 
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