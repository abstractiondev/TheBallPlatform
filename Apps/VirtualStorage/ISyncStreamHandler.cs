using System;
using System.IO;
using System.Threading.Tasks;

namespace TheBall.Support.VirtualStorage
{
    public interface ISyncStreamHandler
    {
        Func<Stream, Task> RequestStreamHandler { get; set; }
        Func<Stream, Task> ResponseStreamHandler { get; set; }
    }
}