using System;
using System.Collections.Generic;
using System.Text;

namespace TheBall.Core.InstanceSupport
{
    public static class RuntimeSupport
    {
        public delegate void ExceptionReportFunc(Exception exception, Dictionary<string, string> properties);

        public static ExceptionReportFunc ExceptionReportHandler;

        public static void ReportException(this Exception exception, Dictionary<string, string> properties = null) => ExceptionReportHandler?.Invoke(exception, properties);

        public static string FigureEnvironmentNameFromUrl(string pathAndQuery)
        {
            return "dev";
        }
    }
}
