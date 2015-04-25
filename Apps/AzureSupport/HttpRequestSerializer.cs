using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ProtoBuf;

namespace AzureSupport
{
    public static class HttpRequestSerializer
    {
        public class SyncingStream : Stream
        {
            private byte[] internalBuffer;
            private bool ended;
            private static ManualResetEvent dataAvailable = new ManualResetEvent(false);
            private static ManualResetEvent dataEmpty = new ManualResetEvent(true);

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override long Length
            {
                get { throw new NotImplementedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                dataAvailable.WaitOne();
                if (count >= internalBuffer.Length)
                {
                    var retVal = internalBuffer.Length;
                    Array.Copy(internalBuffer, buffer, retVal);
                    internalBuffer = null;
                    dataAvailable.Reset();
                    dataEmpty.Set();
                    return retVal;
                }
                else
                {
                    Array.Copy(internalBuffer, buffer, count);
                    internalBuffer = internalBuffer.Skip(count).ToArray(); // i know
                    return count;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                dataEmpty.WaitOne();
                dataEmpty.Reset();

                internalBuffer = new byte[count];
                Array.Copy(buffer, internalBuffer, count);

                Debug.WriteLine("Writing some data");

                dataAvailable.Set();
            }

            public void End()
            {
                dataEmpty.WaitOne();
                dataEmpty.Reset();

                internalBuffer = new byte[0];

                Debug.WriteLine("Ending writes");

                dataAvailable.Set();
            }

        }

        static HttpRequestSerializer()
        {
            Serializer.PrepareSerializer<HttpRequestPackage>();
        }

        public static void ToStream(this HttpRequestPackage package, Stream stream)
        {
            Serializer.Serialize(stream, package);
        }

        public static T DeserializeProtobuf<T>(this Stream stream)
        {
            return Serializer.Deserialize<T>(stream);
        }

        [ProtoContract]
        public class HttpRequestPackage
        {
            [ProtoMember(1)]
            public string ContentPath;

            [ProtoMember(2)]
            public Dictionary<string, string> Paramters;

            [ProtoMember(3)]
            public Dictionary<string, string> FormValues;

            //[ProtoMember(3)] public HttpFileCollection FileCollection;
            [ProtoMember(4)] public Dictionary<string, byte[]> FileCollection;
            
            [ProtoMember(5)]
            public byte[] RequestContent;

            [ProtoMember(6)] public string AccountId;

            [ProtoMember(7)] public string OwnerRoot;

        }
    }
}
