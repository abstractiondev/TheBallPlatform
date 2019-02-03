using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DiagnosticsUtils;
//using LuceneSupport;
using TheBall.CORE;

namespace TheBall.Index
{
    public class QueryIndexedInformationImplementation
    {
        public static async Task<QueryRequest> GetTarget_QueryRequestAsync(IContainerOwner owner, string queryRequestId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<QueryRequest>(owner, queryRequestId);
        }

        public static string GetTarget_LuceneIndexFolder(IContainerOwner owner, string indexName, string indexStorageRootPath)
        {
            string fullPath = Path.Combine(indexStorageRootPath, owner.ToFolderName(), indexName);
            return fullPath;
        }

        public static void ExecuteMethod_PerformQueryRequest(QueryRequest queryRequest, string luceneIndexFolder)
        {
            throw new NotImplementedException();
            /*
            var queryString = queryRequest.QueryString;
            var defaultFieldName = queryRequest.DefaultFieldName;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultDocuments = FieldIndexSupport.PerformQuery(luceneIndexFolder, queryString, defaultFieldName, new StandardAnalyzer(Version.LUCENE_30));
            queryRequest.QueryResultObjects.Clear();
            foreach (var resultDoc in resultDocuments)
            {
                var doc = resultDoc.Doc;
                string objectDomainName = doc.Get("ObjectDomainName");
                string objectName = doc.Get("ObjectName");
                string objectID = doc.Get("ObjectID");
                QueryResultItem item = new QueryResultItem
                    {
                        ObjectDomainName = objectDomainName,
                        ObjectName = objectName,
                        ObjectID = objectID,
                        Rank = resultDoc.Score
                    };
                queryRequest.QueryResultObjects.Add(item);
            }
            stopwatch.Stop();
            queryRequest.LastCompletionDurationMs = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);
            queryRequest.LastCompletionTime = DateTime.UtcNow;
            queryRequest.IsQueryCompleted = true;*/
        }

        public static async Task ExecuteMethod_SaveQueryRequestAsync(QueryRequest queryRequest)
        {
            await queryRequest.StoreInformationAsync();
        }
    }
}