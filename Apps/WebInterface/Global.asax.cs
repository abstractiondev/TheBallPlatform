using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Routing;
using System.Web.DynamicData;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using AzureSupport;
using Microsoft.WindowsAzure;
using SQLite.TheBall.Payments;
using Stripe;
using TheBall;
using MetaModel = System.Web.DynamicData.MetaModel;

namespace WebInterface
{
    public class Global : System.Web.HttpApplication
    {
        private static MetaModel defaultModel = new MetaModel();
        private static SQLite.TheBall.Payments.TheBallDataContext CurrentDataContext;

        public static MetaModel DefaultModel
        {
            get { return defaultModel; }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            CurrentDataContext = TheBallDataContext.CreateOrAttachToExistingDB(@"x:\tbcore\grp\e370760e-fde1-47e6-81ce-bc5ec12f6eff\TheBall.Payments.sqlite");
            TheBallDataContext.CurrentConnection = (SQLiteConnection) CurrentDataContext.Connection;
            DefaultModel.RegisterContext(typeof (SQLite.TheBall.Payments.TheBallDataContext),
                new ContextConfiguration() {ScaffoldAllTables = true});
        }

        private static void RegisterScripts()
        {
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                //Path = "~/Scripts/jquery-1.7.1.min.js",
                //DebugPath = "~/Scripts/jquery-1.7.1.js",
                Path = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js",
                DebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.js",
                CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js",
                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.js",
                CdnSupportsSecureConnection = true
            });
        }


        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            RegisterScripts();

            string connStr;
            connStr = InstanceConfiguration.AzureStorageConnectionString;
            StorageSupport.InitializeWithConnectionString(connStr);
            QueueSupport.RegisterQueue("index-defaultindex-index");
            QueueSupport.RegisterQueue("index-defaultindex-query");
            NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("SecureKeysConfig");
            string stripeApiKey = settings.Get("StripeSecretKey");
            StripeConfiguration.SetApiKey(stripeApiKey);
            var xmlBOMString = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            SQLiteSupport.ContentStorage.GetContentAsStringFunc = blobName =>
            {
                var blob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, blobName);
                var text = blob.DownloadText();
                if (text.StartsWith(xmlBOMString))
                    text = text.Substring(xmlBOMString.Length);
                return text;
            };
            if(!InstanceConfiguration.IsDeveloperMachine)
                FileShareSupport.MountCoreShare();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //if(Request.Path == "default.htm")
            //    Response.RedirectPermanent("anon/default/oip-anon-landing-page.phtml", true);
            //if(Request.Path.ToLower().StartsWith("/theball") == false)
            //    Response.Redirect("/theballanon/oip-layouts/oip-edit-default-layout-jeroen.html", true);
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                bool isWebSocket = Request.Path.StartsWith("/websocket/");
                bool isIndexAspx = Request.Path.StartsWith("index.aspx");
                // TODO: Line below is a hack, that's assuming www.prefix
                bool isWww = Request.Url.DnsSafeHost.StartsWith("www.") ||
                             Request.Url.DnsSafeHost.StartsWith("teaching.") ||
                             Request.Url.DnsSafeHost.StartsWith("ptt.") ||
                             Request.Url.DnsSafeHost.StartsWith("7lk.") ||
                             Request.Url.DnsSafeHost.StartsWith("globalimpact.") ||
                             Request.Url.DnsSafeHost.StartsWith("apps.") ||
                             Request.Url.DnsSafeHost.StartsWith("newglobal.") ||
                             Request.Url.DnsSafeHost.StartsWith("ptt-") ||
                             Request.Url.DnsSafeHost.StartsWith("izenzei.probroz.info");
                if (isWebSocket == false && isIndexAspx == false && isWww == false)
                {
                    string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                    Response.Redirect(redirectUrl, true);
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var request = ctx.Request;
            var authorization = request.Headers["Authorization"];
            if (authorization != null && authorization.StartsWith("DeviceAES:"))
            {
                string[] parts = authorization.Split(':');
                string trustID = parts[2];
                ctx.User = new GenericPrincipal(new GenericIdentity(trustID), new string[] { "DeviceAES"});
            } else
                AuthenticationSupport.SetUserFromCookieIfExists(HttpContext.Current);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}