using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    public static class RemoteSyncSupport
    {

        public static ContentSyncRequest GetSyncRequestFromStream(Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<ContentSyncRequest>(stream);
        }

        public static void PutSyncRequestToStream(Stream stream, ContentSyncRequest syncRequest)
        {
            ProtoBuf.Serializer.Serialize(stream, syncRequest);
        }

        public static ContentSyncResponse GetSyncResponseFromStream(Stream stream)
        {
            return ProtoBuf.Serializer.DeserializeWithLengthPrefix<ContentSyncResponse>(stream, PrefixStyle.Base128);
            //return ProtoBuf.Serializer.Deserialize<ContentSyncResponse>(stream);
        }

        public static void PutSyncResponseToStream(Stream stream, ContentSyncResponse syncResponse)
        {
            ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, syncResponse, PrefixStyle.Base128);
            //ProtoBuf.Serializer.Serialize(stream, syncResponse);
        }

        public static async Task<ISyncStreamHandler> GetFileSystemSyncHandler(string syncRootFolder, string[] ownerSyncedFolders, Func<byte[], byte[]> md5HashComputer)
        {
            return await FileSystemSyncHandler.CreateFileSystemSyncHandler(syncRootFolder, ownerSyncedFolders, md5HashComputer);
        }

        public static string GetFolderMD5Hash(IEnumerable<FolderContent> folderContent, Func<byte[], byte[]> md5HashComputer)
        {
            string fullMd5Hash;
            using (var memStream = new MemoryStream())
            {
                foreach (var contentItem in folderContent.OrderBy(item => item.ContentMD5).ThenBy(item => item.RelativeName))
                {
                    var md5ItemData = Convert.FromBase64String(contentItem.ContentMD5);
                    var nameData = Encoding.UTF8.GetBytes(contentItem.RelativeName);
                    memStream.Write(md5ItemData, 0, md5ItemData.Length);
                    memStream.Write(nameData, 0, nameData.Length);
                }
                var dataToHash = memStream.ToArray();
                byte[] hash = md5HashComputer(dataToHash);
                fullMd5Hash = Convert.ToBase64String(hash);
            }
            return fullMd5Hash;

        }

        public class FolderContent
        {
            public string ContentMD5;
            public string RelativeName;
        }
    }
}
