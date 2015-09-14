using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    [ProtoContract]
    public class ContentSyncRequest
    {
        [ProtoContract]
        public class ContentOwner
        {
            [ProtoMember(0)]
            public string OwnerPrefix;

            [ProtoMember(1)]
            public ContentFolder[] ContentFolders;
        }

        [ProtoContract]
        public class ContentFolder
        {
            [ProtoMember(0)]
            public string Name;

            [ProtoMember(1)]
            public string FullMD5Hash;
        }

        [ProtoMember(0)]
        public ContentOwner[] ContentOwners;

        [ProtoMember(1)] public string[] 
        RequestedFolders;

        [ProtoMember(2)]
        public string[] ContentMD5s;

    }
}