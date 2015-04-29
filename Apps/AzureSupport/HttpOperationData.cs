using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;
using FileName=System.String;
using ParamName=System.String;
using String=System.String;

namespace AzureSupport
{
    [ProtoContract]
    public class HttpOperationData
    {
        [ProtoMember(1)] public string OperationName;
        [ProtoMember(2)] public string OperationRequestPath;
        [ProtoMember(3)] public Dictionary<string, string> QueryParameters;
        [ProtoMember(4)] public Dictionary<string, string> FormValues;
        [ProtoMember(5)] public Dictionary<ParamName, Tuple<FileName, byte[]>> FileCollection;
        [ProtoMember(6)] public byte[] RequestContent;
        [ProtoMember(7)] public string ExecutorAccountID;
        [ProtoMember(8)] public string OwnerRootLocation;
    }
}