using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    [ProtoContract]
    public class ContentSyncResponse
    {
        [ProtoContract]
        public class ContentData
        {
            [ProtoMember(0)]
            public string ContentMD5;

            [ProtoMember(1)]
            public string[] FullNames;

            [ProtoMember(2)] public long ContentLength;
        }

        [ProtoMember(0)]
        public ContentData[] Contents;
    }
}