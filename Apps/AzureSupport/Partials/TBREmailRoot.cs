using static System.Net.WebUtility;

namespace AaltoGlobalImpact.OIP
{
    partial class TBREmailRoot
    {
        public static string GetIDFromEmailAddress(string emailAddress)
        {
            return UrlEncode(emailAddress);
        }
    }
}