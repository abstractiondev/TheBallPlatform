using System.Collections.Generic;
using ProtoBuf;

namespace AzureSupport
{
    [ProtoContract]
    public class HttpRequestData
    {
        [ProtoMember(1)]
        public string ContentPath;

        [ProtoMember(2)]
        public Dictionary<string, string> QueryParameters;

        [ProtoMember(3)]
        public Dictionary<string, string> FormValues;

        //[ProtoMember(3)] public HttpFileCollection FileCollection;
        [ProtoMember(4)] public Dictionary<string, byte[]> FileCollection;
            
        [ProtoMember(5)]
        public byte[] RequestContent;

        [ProtoMember(6)] public string ExecutorAccountID;

        [ProtoMember(7)] public string OwnerRoot;

    }
}