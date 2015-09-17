using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBall.Support.VirtualStorage
{
    internal static class ExtStream
    {
        public static long CopyBytes(this Stream inStream, Stream outStream, long bytesRequired)
        {
            var readSoFar = 0L;
            var buffer = new byte[64 * 1024];
            do
            {
                var toRead = (int) Math.Min(bytesRequired - readSoFar, buffer.Length);
                var readNow = inStream.Read(buffer, 0, toRead);
                if (readNow == 0) // end of stream
                    break;
                outStream.Write(buffer, 0, readNow);
                readSoFar += readNow;
            } while (readSoFar < bytesRequired);
            return readSoFar;
        }

        public static async Task<long> CopyBytesAsync(this Stream inStream, Stream outStream, long bytesRequired)
        {
            var readSoFar = 0L;
            var buffer = new byte[64 * 1024];
            do
            {
                var toRead = (int)Math.Min(bytesRequired - readSoFar, buffer.Length);
                if (toRead == 0) // empty file
                    break;
                var readNow = await inStream.ReadAsync(buffer, 0, toRead);
                if (readNow == 0) // end of stream
                    break;
                await outStream.WriteAsync(buffer, 0, readNow);
                readSoFar += readNow;
            } while (readSoFar < bytesRequired);
            return readSoFar;
        }

    }
}
