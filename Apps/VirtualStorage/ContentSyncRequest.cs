using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    [ProtoContract]
    public class ContentSyncRequest
    {
        [ProtoContract]
        public class ContentOwner
        {
            [ProtoMember(1)]
            public string OwnerPrefix;

            [ProtoMember(2)]
            public ContentFolder[] ContentFolders = new ContentFolder[0];
        }

        [ProtoContract]
        public class ContentFolder
        {
            [ProtoMember(1)]
            public string Name;

            [ProtoMember(2)]
            public string FullMD5Hash;
        }

        [ProtoMember(1)]
        public ContentOwner[] ContentOwners = new ContentOwner[0];

        [ProtoMember(2)] public string[]
        RequestedFolders = new string[0];

        [ProtoMember(3)]
        public string[] ContentMD5s = new string[0];

    }
}