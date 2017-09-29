using System.Linq;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SynchronizeConnectionCategoriesImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static async Task<Connection> GetTarget_ConnectionAsync(string connectionId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, connectionId);
        }

        public static async Task<Category[]> ExecuteMethod_SyncCategoriesWithOtherSideCategoriesAsync(Connection connection)
        {
            ConnectionCommunicationData connectionData = new ConnectionCommunicationData
                {
                    ActiveSideConnectionID = connection.ID,
                    ReceivingSideConnectionID = connection.OtherSideConnectionID,
                    ProcessRequest = "SYNCCATEGORIES",
                    CategoryCollectionData = connection.ThisSideCategories.Select(CategoryInfo.FromCategory).ToArray(),
                    LinkItems = connection.CategoryLinks.Select(catLink => new CategoryLinkItem
                        {
                            SourceCategoryID = catLink.SourceCategoryID,
                            TargetCategoryID = catLink.TargetCategoryID,
                            LinkingType = catLink.LinkingType
                        }).ToArray()
                };
            var result = await DeviceSupport.ExecuteRemoteOperation<ConnectionCommunicationData>(connection.DeviceID,
                                                                                           "TheBall.Interface.ExecuteRemoteCalledConnectionOperation", connectionData);
            return result.CategoryCollectionData.Select(catInfo => catInfo.ToCategory()).ToArray();
        }


        /*
         * 
         *             <InterfaceItem name="NativeCategoryID" logicalDataType="Text_Short" />
                    <InterfaceItem name="NativeCategoryDomainName" logicalDataType="Text_Short"/>
                    <InterfaceItem name="NativeCategoryObjectName" logicalDataType="Text_Short"/>
                    <InterfaceItem name="NativeCategoryTitle" logicalDataType="Text_Short"/>
                    <InterfaceItem name="IdentifyingCategoryName" logicalDataType="Text_Short" />
                    <InterfaceItem name="ParentCategoryID" logicalDataType="Text_Short"/>

         * 
         * */

        public static void ExecuteMethod_UpdateOtherSideCategories(Connection connection, Category[] getOtherSideCategoriesOutput)
        {
            connection.OtherSideCategories.Clear();
            connection.OtherSideCategories.AddRange(getOtherSideCategoriesOutput);
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }

        public static async Task ExecuteMethod_ExecuteProcessToUpdateThisSideCategoriesAsync(string connectionID)
        {
            await ExecuteConnectionProcess.ExecuteAsync(new ExecuteConnectionProcessParameters
                {
                    ConnectionID = connectionID,
                    ConnectionProcessToExecute = "UpdateConnectionThisSideCategories"
                });
        }
    }
}