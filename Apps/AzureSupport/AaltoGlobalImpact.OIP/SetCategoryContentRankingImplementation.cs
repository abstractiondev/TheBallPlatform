using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP.INT;
using AzureSupport;
using TheBall;
using TheBall.Core;
using TheBall.Core.Storage;

namespace AaltoGlobalImpact.OIP
{
    public class SetCategoryContentRankingImplementation
    {
        public static CategoryChildRanking GetTarget_RankingData()
        {
            var reqData = LogicalOperationContext.Current.HttpParameters.RequestContent;
            var result = JSONSupport.GetObjectFromData<CategoryChildRanking>(reqData);
            return result;
        }

        public static string GetTarget_CategoryID(CategoryChildRanking rankingData)
        {
            return rankingData.CategoryID;
        }

        public static async Task<ContentCategoryRankCollection> GetTarget_ContentRankingCollectionAsync()
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<ContentCategoryRankCollection>(InformationContext.CurrentOwner,
                "MasterCollection");
        }

        public static ContentCategoryRank[] GetTarget_CategoryRankingCollection(string categoryId, ContentCategoryRankCollection contentRankingCollection)
        {
            return contentRankingCollection.CollectionContent.Where(item => item.CategoryID == categoryId && item.RankName == "MANUAL").ToArray();
        }

        public static async Task ExecuteMethod_SyncRankingItemsToRankingDataAsync(CategoryChildRanking rankingData, ContentCategoryRank[] categoryRankingCollection)
        {
            var owner = InformationContext.CurrentOwner;
            var toBeDeleted = categoryRankingCollection.Where(
                candidate => rankingData.RankingItems.Any(
                    rItem =>
                        rItem.ContentID == candidate.ContentID) == false).ToArray();
            var toBeAdded = rankingData.RankingItems.Where(
                candidate => categoryRankingCollection.Any(
                    rItem =>
                        rItem.ContentID == candidate.ContentID) == false).ToArray();
            var toBeModified = categoryRankingCollection.Select(
                candidate =>
                {
                    var matchingItem = rankingData.RankingItems.FirstOrDefault(rItem =>
                        rItem.ContentID == candidate.ContentID &&
                        rItem.RankValue != candidate.RankValue);
                    return new {CurrentItem = candidate, ModifiedItem = matchingItem};
                }).Where(result => result.ModifiedItem != null).ToArray();
            foreach (var itemToDelete in toBeDeleted)
            {
                try
                {
                    var iObj = await ObjectStorage.RetrieveFromOwnerContentA<ContentCategoryRank>(owner, itemToDelete.ID);
                    await iObj.DeleteInformationObjectAsync();
                }
                catch 
                {
                }
            }
            foreach (var itemToAdd in toBeAdded)
            {
                try
                {
                    var iObj = new ContentCategoryRank();
                    iObj.SetLocationAsOwnerContent(owner, iObj.ID);
                    iObj.CategoryID = rankingData.CategoryID;
                    iObj.ContentID = itemToAdd.ContentID;
                    iObj.ContentSemanticType = itemToAdd.ContentSemanticType;
                    iObj.RankName = itemToAdd.RankName;
                    iObj.RankValue = itemToAdd.RankValue;
                    await iObj.StoreInformationAsync(owner);
                }
                catch
                {
                    
                }
            }
            foreach (var itemToModify in toBeModified)
            {
                try
                {
                    var iObj = await ObjectStorage.RetrieveFromOwnerContentA<ContentCategoryRank>(owner, itemToModify.CurrentItem.ID);
                    iObj.RankValue = itemToModify.ModifiedItem.RankValue;
                    await iObj.StoreInformationAsync(owner);
                }
                catch
                {

                }
                
            }
        }
    }
}