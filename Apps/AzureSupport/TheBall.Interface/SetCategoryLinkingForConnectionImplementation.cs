using System.Web;
using AzureSupport;
using TheBall.CORE;
using System.Linq;
using System.Threading.Tasks;
using TheBall.CORE.Storage;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SetCategoryLinkingForConnectionImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }

        public static CategoryLinkParameters GetTarget_CategoryLinkingParameters()
        {
            var reqData = LogicalOperationContext.Current.HttpParameters.RequestContent;
            var result = JSONSupport.GetObjectFromData<CategoryLinkParameters>(reqData);
            return result;
        }

        public static async Task<Connection> GetTarget_ConnectionAsync(CategoryLinkParameters categoryLinkingParameters)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<Connection>(Owner, categoryLinkingParameters.ConnectionID);
        }

        public static void ExecuteMethod_SetConnectionLinkingData(Connection connection, CategoryLinkParameters categoryLinkingParameters)
        {
            connection.CategoryLinks.Clear();
            connection.CategoryLinks.AddRange(categoryLinkingParameters.LinkItems.Select(getCategoryLinkFromInterfaceLink));
        }

        private static CategoryLink getCategoryLinkFromInterfaceLink(CategoryLinkItem categoryLinkItem)
        {
            return new CategoryLink
                {
                    SourceCategoryID = categoryLinkItem.SourceCategoryID,
                    TargetCategoryID = categoryLinkItem.TargetCategoryID,
                    LinkingType = categoryLinkItem.LinkingType
                };
        }

        public static async Task ExecuteMethod_StoreObjectAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }
    }
}