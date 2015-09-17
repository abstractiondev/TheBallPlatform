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

        public static ISyncStreamHandler GetFileSystemSyncHandler(string syncRootFolder, string[] ownerSyncedFolders)
        {
            return new FileSystemSyncHandler(syncRootFolder, ownerSyncedFolders);
        }

    }
}
