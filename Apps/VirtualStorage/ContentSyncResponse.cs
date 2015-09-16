using System.ServiceModel.Channels;
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
            [ProtoMember(0)]
            public string ContentMD5;

            [ProtoMember(1)]
            public string[] FullNames;


            public ResponseContentType ResponseContentType => IsDeleted
                ? ResponseContentType.Deleted
                : ExistingContentNameData ? ResponseContentType.NameDataRefresh : ResponseContentType.IncludedInTransfer;


            [ProtoMember(2)] public long ContentLength;

            private bool IsDeleted => FullNames == null || FullNames.Length == 0;
            private bool ExistingContentNameData => ContentLength == -1;
            //private bool IncludedInTransfer => !IsDeleted && !ExistingContentNameData;
        }

        [ProtoMember(0)]
        public ContentData[] Contents;

        public bool IsUnchanged => Contents == null;
        public bool IsEmpty => Contents != null && Contents.Length == 0;
    }
}