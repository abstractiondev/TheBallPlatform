using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    [ProtoContract]
    public class ContentSyncRequest
    {
        [ProtoContract]
        public class ContentFolder
        {
            [ProtoMember(0)]
            public string Name;

            [ProtoMember(1)]
            public string FullMD5Hash;
        }

        [ProtoMember(0)]
        public ContentFolder[] ContentFolders;

        [ProtoMember(1)]
        public string[] ContentMD5s;
    }
}