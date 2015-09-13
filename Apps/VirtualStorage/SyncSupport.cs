using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Support.VirtualStorage
{
    public static class SyncSupport
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
            return ProtoBuf.Serializer.Deserialize<ContentSyncResponse>(stream);
        }

        public static void PutSyncResponseToStream(Stream stream, ContentSyncResponse syncResponse)
        {
            ProtoBuf.Serializer.Serialize(stream, syncResponse);
        }

    }
}
