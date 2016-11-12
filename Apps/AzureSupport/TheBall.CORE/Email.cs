using System.Web;

namespace TheBall.CORE
{
    partial class Email
    {
        public static string GetIDFromEmailAddress(string emailAddress)
        {
            return HttpUtility.UrlEncode(emailAddress.ToLower());
        }
    }
}