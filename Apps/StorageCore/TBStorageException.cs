using System;
using System.Net;
using System.Runtime.Serialization;

namespace TheBall.Core.Storage
{
    public class TBStorageException : Exception
    {
        public TBStorageException()
        {

        }

        public TBStorageException(HttpStatusCode statusCode, Exception innerException = null, string message = null) : base(message ?? statusCode.ToString(), innerException)
        {
            StatusCode = statusCode;
        }
        public TBStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        public TBStorageException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public HttpStatusCode StatusCode { get; }
    }
}