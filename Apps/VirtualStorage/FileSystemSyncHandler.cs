using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace TheBall.Support.VirtualStorage
{
    public class FileSystemSyncHandler : ISyncStreamHandler
    {
        public ContentSyncRequest SyncRequest { get; private set; }
        public ContentSyncResponse SyncResponse { get; private set; }
        public string SyncRootFolder { get; private set; }
        public FileSystemSyncHandler(string syncRootFolder, string[] ownerSyncedFolders, Func<byte[], byte[]> md5HashComputer)
        {
            SyncRootFolder = syncRootFolder;
            SyncRequest = VirtualFS.Current.CreateFullSyncRequest(syncRootFolder,
                ownerSyncedFolders, md5HashComputer);
            RequestStreamHandler = handleRequestStream;
            ResponseStreamHandler = stream => handleResponseStream(stream).Wait();
        }

        private void handleRequestStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionLevel.Fastest))
            {
                RemoteSyncSupport.PutSyncRequestToStream(compressedStream, SyncRequest);
            }
        }

        private async Task handleResponseStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionMode.Decompress, true))
            {
                var syncResponse = RemoteSyncSupport.GetSyncResponseFromStream(compressedStream);
                SyncResponse = syncResponse;
                var contentToExpect = syncResponse.Contents.Where(content => content.ResponseContentType == ResponseContentType.IncludedInTransfer).ToArray();
                foreach (var content in contentToExpect)
                    await streamToFile(content, compressedStream, SyncRootFolder);
                var contentToDelete = syncResponse.Contents.Where(content => content.ResponseContentType == ResponseContentType.Deleted).ToArray();
                foreach (var content in contentToDelete)
                    deleteContent(content);
                var contentToRefresh =
                    syncResponse.Contents.Where(
                        content => content.ResponseContentType == ResponseContentType.NameDataRefresh).ToArray();
                foreach (var content in contentToRefresh)
                    refreshContentNameData(content, SyncRootFolder);
            }
        }

        private static void refreshContentNameData(ContentSyncResponse.ContentData content, string syncRootFolder)
        {
            var fullNames = content.FullNames.Select(name => Path.Combine(syncRootFolder, name)).ToArray();
            VirtualFS.Current.UpdateContentNameData(content.ContentMD5, fullNames);
        }

        private void deleteContent(ContentSyncResponse.ContentData content)
        {
            VirtualFS.Current.RemoveLocalContentByMD5(content.ContentMD5);
            Debug.WriteLine("Deleted content: {0}", content.ContentMD5);
        }

        private static async Task streamToFile(ContentSyncResponse.ContentData content, GZipStream compressedStream, string syncRootFolder)
        {
            var contentMd5 = content.ContentMD5;
            using (var outStream = await VirtualFS.Current.GetLocalTargetStreamForWrite(contentMd5))
            {
                await compressedStream.CopyBytesAsync(outStream, content.ContentLength);
                var fullNames = content.FullNames.Select(name => Path.Combine(syncRootFolder, name)).ToArray();
                VirtualFS.Current.UpdateContentNameData(contentMd5, fullNames, true, content.ContentLength);
                Debug.WriteLine("Wrote {0} bytes of file(s): {1}", content.ContentLength, String.Join(", ", fullNames));
            }
        }


        public Action<Stream> RequestStreamHandler { get; set; }
        public Action<Stream> ResponseStreamHandler { get; set; }
    }
}