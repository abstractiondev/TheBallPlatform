using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
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
using SQLiteSupport;
using Stripe;
using TheBall;
using TheBall.CORE;
using MetaModel = System.Web.DynamicData.MetaModel;
using MetaTable = System.Web.DynamicData.MetaTable;

namespace WebInterface
{
    public class Global : System.Web.HttpApplication
    {
        public class CustomRouteHandler : DynamicDataRouteHandler
        {
            protected override string GetCustomPageVirtualPath(MetaTable table, string viewName)
            {
                return base.GetCustomPageVirtualPath(table, viewName);
            }

            protected override string GetScaffoldPageVirtualPath(MetaTable table, string viewName)
            {
                return base.GetScaffoldPageVirtualPath(table, viewName);
            }

            public override IHttpHandler CreateHandler(DynamicDataRoute route, MetaTable table, string action)
            {
                var requestPath = HttpContext.Current.Request.Path;
                var currentOwner = VirtualOwner.FigureOwner(requestPath.Replace("/auth/", ""));
                InformationContext.Current.Owner = currentOwner;
                return base.CreateHandler(route, table, action);
            }
        }


        static Dictionary<string, MetaModel> ModelDict = new Dictionary<string, MetaModel>();
        static Dictionary<string, object> DataContextDictionary = new Dictionary<string, object>();

        public static void RegisterRoutes(RouteCollection routes, string[] dataContextDomains)
        {

            //                    IMPORTANT: DATA MODEL REGISTRATION 
            // Uncomment this line to register a LINQ to SQL model for ASP.NET Dynamic Data.
            // Set ScaffoldAllTables = true only if you are sure that you want all tables in the
            // data model to support a scaffold (i.e. templates) view. To control scaffolding for
            // individual tables, create a partial class for the table and apply the
            // [ScaffoldTable(true)] attribute to the partial class.
            // Note: Make sure that you change "YourDataContextType" to the name of the data context
            // class in your application.

            foreach (string dataContextDomain in dataContextDomains)
            {
                string dataContextTypeName = "SQLite." + dataContextDomain + ".TheBallDataContext";
                var dataContextType = SQLiteSupport.ReflectionSupport.GetSQLiteDataContextType(dataContextTypeName);
                var currentDataContext =
                    (IStorageSyncableDataContext)
                        dataContextType.InvokeMember("CreateOrAttachToExistingDB", BindingFlags.InvokeMethod, null, null,
                            new object[] { ":memory:" });
                Func<DbConnection> getConnectionFunc = () =>
                {
                    var inMemoryConnection = currentDataContext.Connection;
                    if (InformationContext.Current.IsOwnerDefined)
                    {
                        var owner = InformationContext.CurrentOwner;
                        var dbDirectory = UpdateOwnerDomainObjectsInSQLiteStorageImplementation.GetTarget_SQLiteDBLocationDirectory(owner);
                        var urlPathParts = HttpContext.Current.Request.Path.Split('/');
                        string semanticDomainName = urlPathParts[5];
                        var dbFileName =
                            UpdateOwnerDomainObjectsInSQLiteStorageImplementation.GetTarget_SQLiteDBLocationFileName(
                                semanticDomainName, dbDirectory);
                        return new SQLiteConnection(String.Format("Data Source={0}", dbFileName));
                    }
                    return inMemoryConnection;
                };
                dataContextType.GetProperty("GetCurrentConnectionFunc").SetValue(null, getConnectionFunc , null);
                var currentModel = new MetaModel();

                DataContextDictionary.Add(dataContextTypeName, currentDataContext);
                ModelDict.Add(dataContextTypeName, currentModel);

                currentModel.RegisterContext(dataContextType, new ContextConfiguration() { ScaffoldAllTables = true });

                //DefaultModel.RegisterContext(myDataModelProvider);
                // The following statement supports separate-page mode, where the List, Detail, Insert, and 
                // Update tasks are performed by using separate pages. To enable this mode, uncomment the following 
                // route definition, and comment out the route definitions in the combined-page mode section that follows.
                routes.Add(new DynamicDataRoute(String.Format("auth/grp/{{groupID}}/DDCRUD/{0}/{{table}}/{{action}}.aspx", dataContextDomain))
                {
                    Constraints = new RouteValueDictionary(new
                    {
                        action = "List|Details|Edit|Insert",
                        httpMethod = new HttpMethodConstraint("GET")
                        //groupID = "xyz"
                    }),
                    Model = currentModel,
                    RouteHandler = new CustomRouteHandler()
                });

            }

            // The following statements support combined-page mode, where the List, Detail, Insert, and
            // Update tasks are performed by using the same page. To enable this mode, uncomment the
            // following routes and comment out the route definition in the separate-page mode section above.
            /*
            routes.Add(new DynamicDataRoute("auth/{groupID}/DDR/{table}/ListDetails.aspx")
            {
                Action = PageAction.List,
                ViewName = "ListDetails",
                Model = DefaultModel
            });

            routes.Add(new DynamicDataRoute("auth/{groupID}/DDR/{table}/ListDetails.aspx")
            {
                Action = PageAction.Details,
                ViewName = "ListDetails",
                Model = DefaultModel
            });
             * */
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
            if (InstanceConfiguration.DynamicDataCRUDDomains.Length > 0 && InstanceConfiguration.UseSQLiteMasterDatabase)
            {
                RegisterRoutes(RouteTable.Routes, InstanceConfiguration.DynamicDataCRUDDomains);
                RegisterScripts();
            }

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