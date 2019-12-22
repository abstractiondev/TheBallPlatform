using System;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall
{
    public static class ErrorSupport
    {
        public static void ReportError(string error)
        {
            //QueueSupport.PutToErrorQueue(error);
        }

        [Obsolete("not implemented", false)]
        public static void ReportException(Exception exception)
        {
            // Under NO circumstances the exception reporting shall cause another exception to be thrown unhandled
            try
            {
                string error = GetErrorFromExcetion(exception);
                ReportError(error);
            } catch
            {
                
            }
        }

        public static string GetErrorFromExcetion(Exception exception)
        {
            return string.Format("Error: {1}{0}Occurred: {2}{0}Description: {3}{0}", Environment.NewLine,
                exception.GetType().Name,
                DateTime.UtcNow.ToLongDateString(), exception.ToString())
            ;
        }

    }
}