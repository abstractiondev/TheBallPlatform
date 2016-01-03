using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexLucene.Analysis;
using FlexLucene.Analysis.Core;
using FlexLucene.Analysis.Standard;
using FlexLucene.Document;
using FlexLucene.Index;
using FlexLucene.Queryparser.Classic;
using FlexLucene.Search;
using FlexLucene.Store;
using FlexLucene.Util;
using java.nio.file;
using Version = System.Version;

namespace LuceneSupport
{
    public class ResultDoc
    {
        public Document Doc;
        public float Score;
    }

    public class FieldIndexSupport
    {

        private static void doWithWriter(string indexRoot, Action<IndexWriter> actionWithWriter, Analyzer analyzer, bool recreateIndex = false)
        {
            FSDirectory indexDirectory = FSDirectory.Open(Paths.get(indexRoot));
            if(analyzer == null)
                analyzer = new StandardAnalyzer();
            //var writer = new IndexWriter(indexDirectory, analyzer, recreateIndex, IndexWriter.MaxFieldLength.UNLIMITED);
            var writer = new IndexWriter(indexDirectory, new IndexWriterConfig(analyzer));
            actionWithWriter(writer);
            //writer.Commit();
            //writer.Commit();
            writer.Flush();
        }

        public static void AddAndRemoveDocuments(string indexRoot, Document[] docsToAdd, string[] docsToRemove, Analyzer analyzer = null)
        {
            doWithWriter(indexRoot, writer =>
                {
                    Term[] terms = docsToRemove.Select(docId => new Term("ID", docId)).ToArray();
                    writer.DeleteDocuments(terms);
                    foreach (var doc in docsToAdd)
                    {
                        string id = doc.GetField("ID").StringValue();
                        writer.UpdateDocument(new Term("ID", id), doc);
                    }
                }, analyzer);            
        }

        public static void AddDocuments(string indexRoot, Document[] docs, Analyzer analyzer = null, bool recreateIndex = false)
        {
            doWithWriter(indexRoot, writer => {
                    foreach (var doc in docs)
                    {
                        string id = doc.GetField("ID").StringValue();
                        writer.UpdateDocument(new Term("ID", id), doc);
                    }
                }, analyzer, recreateIndex);
        }

        public static ResultDoc[] PerformQuery(string indexPath, string queryText, string defaultFieldName, Analyzer analyzer, int resultAmount = 100)
        {
            Directory searchDirectory = FSDirectory.Open(Paths.get(indexPath));
            DirectoryReader directoryReader = DirectoryReader.Open(searchDirectory);
            IndexSearcher searcher = new IndexSearcher(directoryReader);
            QueryParser parser = new QueryParser(defaultFieldName, analyzer);
            Query query = parser.Parse(queryText);
            TopDocs topDocs = searcher.Search(query, resultAmount);
            return topDocs.ScoreDocs.Select(sDoc =>
                                            new ResultDoc {Doc = searcher.Doc(sDoc.Doc), Score = sDoc.Score}).ToArray();
        }

        public static void RemoveDocuments(string indexRoot, params string[] documentIds)
        {
            doWithWriter(indexRoot, writer =>
                {
                    Term[] terms = documentIds.Select(docId => new Term("ID", docId)).ToArray();
                    writer.DeleteDocuments(terms);
                }, new KeywordAnalyzer());
        }

        public static void CreateIndex(string indexRoot)
        {
            doWithWriter(indexRoot, writer => {}, null, true);
        }

        public static Field GetField(string name, string value, bool analyzed = false)
        {
            var field = new Field(name, value, FieldStore.YES, analyzed ? FieldIndex.ANALYZED : FieldIndex.NOT_ANALYZED);
            return field;
        }
    }
}
