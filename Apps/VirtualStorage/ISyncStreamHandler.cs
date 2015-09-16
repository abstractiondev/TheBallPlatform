using System;
using System.IO;

namespace TheBall.Support.VirtualStorage
{
    public interface ISyncStreamHandler
    {
        Action<Stream> RequestStreamHandler { get; set; }
        Action<Stream> ResponseStreamHandler { get; set; }
    }
}