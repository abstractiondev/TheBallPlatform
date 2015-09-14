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

            public bool IsDeleted => FullNames == null ||  FullNames.Length == 0;

            [ProtoMember(2)] public long ContentLength;

            public bool IsUnchanged => ContentLength == -1;
        }

        [ProtoMember(0)]
        public ContentData[] Contents;

        public bool IsUnchanged => Contents == null;
        public bool IsEmpty => Contents != null && Contents.Length == 0;
    }
}