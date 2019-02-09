using ProtoBuf;

namespace TheBall.Support.VirtualStorage
{
    public enum ResponseContentType
    {
        Undefined = 0,
        Deleted,
        NameDataRefresh,
        IncludedInTransfer
    }

    [ProtoContract]
    public class ContentSyncResponse
    {
        [ProtoContract]
        public class ContentData
        {
            [ProtoMember(1)]
            public string ContentMD5;

            [ProtoMember(2)]
            public string[] FullNames = new string[0];


            public ResponseContentType ResponseContentType => IsDeleted
                ? ResponseContentType.Deleted
                : ExistingContentNameData ? ResponseContentType.NameDataRefresh : ResponseContentType.IncludedInTransfer;


            [ProtoMember(3)] public long ContentLength;

            private bool IsDeleted => FullNames == null || FullNames.Length == 0;
            private bool ExistingContentNameData => ContentLength == -1;
            //private bool IncludedInTransfer => !IsDeleted && !ExistingContentNameData;
        }

        [ProtoMember(1)]
        public ContentData[] Contents = new ContentData[0];

        [ProtoMember(2)] public bool IsUnchanged = false;

        public bool IsEmpty => IsUnchanged == false && Contents.Length == 0;
    }
}