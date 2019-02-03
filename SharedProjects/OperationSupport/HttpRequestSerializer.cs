using System;
using System.IO;
using ProtoBuf;

namespace TheBall.CORE
{
    public static class HttpRequestSerializer
    {
        static HttpRequestSerializer()
        {
            Serializer.PrepareSerializer<HttpOperationData>();
        }

        public static void ToStream(this HttpOperationData package, Stream stream)
        {
            Serializer.Serialize(stream, package);
        }

        public static byte[] ToBytes(this HttpOperationData package)
        {
            using (var memStream = new MemoryStream())
            {
                package.ToStream(memStream);
                return memStream.ToArray();
            }
        }

        public static object DeserializeProtobuf(this Stream stream, Type type)
        {
            return Serializer.NonGeneric.Deserialize(type, stream);
        }

        public static void SerializeProtobuf(this Stream stream, object obj)
        {
            Serializer.NonGeneric.Serialize(stream, obj);
        }

        public static T DeserializeProtobuf<T>(this Stream stream)
        {
            return Serializer.Deserialize<T>(stream);
        }

    }
}