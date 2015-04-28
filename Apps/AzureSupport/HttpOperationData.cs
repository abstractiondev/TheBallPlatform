using System;
using System.Collections.Generic;
using ProtoBuf;
using FileName=System.String;
using ParamName=System.String;
using String=System.String;

namespace AzureSupport
{
    [ProtoContract]
    public class HttpOperationData
    {
        [ProtoMember(1)]
        public string OperationRequestPath;

        [ProtoMember(2)]
        public Dictionary<string, string> QueryParameters;

        [ProtoMember(3)]
        public Dictionary<string, string> FormValues;

        //[ProtoMember(3)] public HttpFileCollection FileCollection;
        [ProtoMember(4)] public Dictionary<ParamName, Tuple<FileName, byte[]>> FileCollection;
            
        [ProtoMember(5)]
        public byte[] RequestContent;

        [ProtoMember(6)] public string ExecutorAccountID;

        [ProtoMember(7)] public string OwnerRoot;

    }
}