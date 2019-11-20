using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP.INT;
using AzureSupport;
using TheBall;
using TheBall.Core;
using TheBall.Core.Storage;

namespace AaltoGlobalImpact.OIP
{
    public class SetCategoryHierarchyAndOrderInNodeSummaryImplementation
    {
        public static ParentToChildren[] GetTarget_Hierarchy()
        {
            var reqData = LogicalOperationContext.Current.HttpParameters.RequestContent;
            var result = JSONSupport.GetObjectFromData<ParentToChildren[]>(reqData);
            return result;
        }

        public static async Task ExecuteMethod_SetParentCategoriesAsync(ParentToChildren[] hierarchy)
        {
            foreach (var parentItem in hierarchy)
                await SetParentsRecursively(parentItem, null);
        }

        private static async Task SetParentsRecursively(ParentToChildren parentItem, string parentID)
        {
            var owner = InformationContext.Current.Owner;
            int retryCount = 3;
            while (retryCount-- > 0)
            {
                try
                {
                    string currID = parentItem.id;
                    Category cat = await ObjectStorage.RetrieveFromOwnerContentA<Category>(owner, currID);
                    if (cat != null && cat.ParentCategoryID != parentID)
                    {
                        cat.ParentCategoryID = parentID;
                        await cat.StoreInformationAsync();
                    }
                }
                catch
                {
                    if (retryCount == 0)
                        throw;
                }
            }
            if (parentItem.children == null)
                return;
            foreach(var childItem in parentItem.children)
                SetParentsRecursively(childItem, parentItem.id);
        }

        public static async Task<NodeSummaryContainer> GetTarget_NodeSummaryContainerAsync()
        {
            var owner = InformationContext.Current.Owner;
            return await ObjectStorage.RetrieveFromOwnerContentA<NodeSummaryContainer>(owner, "default");
        }

        public static void ExecuteMethod_SetCategoryOrder(ParentToChildren[] hierarchy, NodeSummaryContainer nodeSummaryContainer)
        {
            List<string> flattenedIDList = new List<string>();
            flattenHierarchyIDList(hierarchy, flattenedIDList);
            var flattenedArray = flattenedIDList.ToArray();
            string commaSeparatedIDs = String.Join(",", flattenedArray);
            nodeSummaryContainer.NodeSourceCategories.SelectedIDCommaSeparated = commaSeparatedIDs;
            var newList =
                nodeSummaryContainer.NodeSourceCategories.CollectionContent.OrderBy(
                    cat => flattenedIDList.IndexOf(cat.ID)).ToList();
            nodeSummaryContainer.NodeSourceCategories.CollectionContent = newList;
        }

        private static void flattenHierarchyIDList(ParentToChildren[] hierarchy, List<string> flattenedIdList)
        {
            foreach (var item in hierarchy)
            {
                flattenedIdList.Add(item.id);
                if(item.children != null)
                    flattenHierarchyIDList(item.children, flattenedIdList);
            }
        }

        public static async Task ExecuteMethod_StoreObjectAsync(NodeSummaryContainer nodeSummaryContainer)
        {
            await nodeSummaryContainer.StoreInformationAsync();
        }
    }
}