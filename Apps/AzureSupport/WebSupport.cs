using System.Security.Principal;
using System.Text;
using System.Web;
using TheBall;
using TheBall.CORE.InstanceSupport;

namespace AzureSupport
{
    public static class WebSupport
    {
        public static string GetLoginUrl(IPrincipal identityPrincipal)
        {
            return identityPrincipal.Identity.Name;
        }

        public static string GetLoginUrl(HttpContext context)
        {
            return GetLoginUrl(context.User);
        }

        static string GetContainerName(HttpRequest request)
        {
            string hostName = request.Url.DnsSafeHost;
            if (hostName == "localhost" || hostName == "localdev" || hostName.StartsWith("192.168."))
                return InstanceConfig.Current.WorkerActiveContainerName;
            string containerName = hostName.Replace('.', '-').ToLower();
            if (InstanceConfig.Current.ContainerRedirectsDict.ContainsKey(containerName))
                return InstanceConfig.Current.ContainerRedirectsDict[containerName];
            return containerName;
        }

        public static void InitializeContextStorage(HttpRequest request)
        {
            string containerName = GetContainerName(request);
            InformationContext.Current.InitializeCloudStorageAccess(containerName);
        }


        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJsString(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }
    }
}