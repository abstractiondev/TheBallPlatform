using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Linq;
using System.Linq;
using System.Text;
using SQLite.TheBall.Payments;

namespace SQLiteSupport
{
    public static class MetaDataSync
    {
        /// <summary>
        /// Applies appropriate change actions to existing data
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="existingDatas">Existing datas</param>
        /// <returns>Datas to add (= datas in current data, that don't yet exist in existing data)</returns>
        public static InformationObjectMetaData[] UpdatePendingChangeActionsToExistingData(InformationObjectMetaData[] currentDatas,
            InformationObjectMetaData[] existingDatas)
        {
            // initially set all processing to delete, thus only visited objects get changeaction changed to something else
            Dictionary<string, InformationObjectMetaData> existingDict = existingDatas.ToDictionary(
                getMetaDataKey,
                item =>
                {
                    item.CurrentChangeAction = ChangeAction.Delete;
                    return item;
                });
            
            var objectsToAdd = new List<InformationObjectMetaData>();
            foreach (var currentData in currentDatas)
            {
                var objectKey = getMetaDataKey(currentData);
                if (!existingDict.ContainsKey(objectKey))
                {
                    currentData.CurrentChangeAction = ChangeAction.Insert;
                    objectsToAdd.Add(currentData);
                }
                else
                {
                    var existingObject = existingDict[objectKey];
                    bool isChanged = false;
                    if (!String.IsNullOrEmpty(currentData.MD5))
                        isChanged = currentData.MD5 != existingObject.MD5;
                    isChanged = isChanged || currentData.FileLength != existingObject.FileLength ||
                                currentData.LastWriteTime != existingObject.LastWriteTime;
                    if (isChanged)
                    {
                        existingObject.MD5 = currentData.MD5;
                        existingObject.LastWriteTime = currentData.LastWriteTime;
                        existingObject.FileLength = currentData.FileLength;
                        existingObject.CurrentChangeAction = ChangeAction.Update;
                        existingObject.CurrentStoragePath = currentData.CurrentStoragePath;
                    }
                    else 
                        existingObject.CurrentChangeAction = ChangeAction.None;
                }
            }

            return objectsToAdd.ToArray();
        }

        private static string getMetaDataKey(InformationObjectMetaData item)
        {
            return string.Format("{0}/{1}/{2}", item.SemanticDomain, item.ObjectType, item.ObjectID);
        }

        public static bool ApplyChangeActionsToExistingData(InformationObjectMetaData[] currentDatas, InformationObjectMetaData[] existingDatas, 
            Action<InformationObjectMetaData> insertAction, Action<InformationObjectMetaData> updateAction, Action<InformationObjectMetaData> deleteAction)
        {
            var pendingInserts = UpdatePendingChangeActionsToExistingData(currentDatas, existingDatas);
            var allChanges =
                existingDatas.Where(item => item.CurrentChangeAction != ChangeAction.None)
                    .Concat(pendingInserts)
                    .ToArray();
            bool anyChanges = allChanges.Length > 0;
            foreach (var change in allChanges)
            {
                switch (change.CurrentChangeAction)
                {
                    case ChangeAction.Insert:
                        insertAction(change);
                        break;
                    case ChangeAction.Update:
                        updateAction(change);
                        break;
                    case ChangeAction.Delete:
                        deleteAction(change);
                        break;
                }
                change.CurrentChangeAction = ChangeAction.None;
            }
            return anyChanges;
        }
    }
}
