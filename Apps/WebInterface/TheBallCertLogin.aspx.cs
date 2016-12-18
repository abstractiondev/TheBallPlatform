using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBall;

namespace WebInterface
{
    public partial class TheBallCertLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userName = getUserNameFromCertificate(this.Request);
            AuthenticationSupport.SetAuthenticationCookie(Response, userName, null, null);
            //FormsAuthentication.RedirectFromLoginPage(response.ClaimedIdentifier, false);
            //string redirectUrl = FormsAuthentication.GetRedirectUrl(userName, true);
            string redirectUrl = Request.Params["ReturnUrl"];
            if (redirectUrl == null)
                redirectUrl = FormsAuthentication.DefaultUrl;
            Response.Redirect(redirectUrl, true);
        }

        private string getUserNameFromCertificate(HttpRequest request)
        {
            var x509 = new X509Certificate2(request.ClientCertificate.Certificate);

            // create the certificate chain by using the machine store
            var chain = new X509Chain(true);
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
            chain.Build(x509);

            // at this point chain.ChainElements[0] will contain the original
            // certificate, the higher indexes are the issuers.
            // note that if the certificate is self-signed, there will be just one entry.
            var issuer = chain.ChainElements[1].Certificate.Thumbprint;
            return "https://x509_" + x509.Thumbprint;
        }
    }
}