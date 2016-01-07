using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;
using TheBall.CORE;

namespace TheBallTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connStr = //String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}",
                    args[0];
                                 //              );
                //connStr = "UseDevelopmentStorage=true";
                bool debugMode = false;


                StorageSupport.InitializeFixedStorageSettings(connStr);
                InformationContext.InitializeToLogicalContext(TBSystem.CurrSystem);
                InformationContext.Current.InitializeCloudStorageAccess(Properties.Settings.Default.CurrentActiveContainerName);
                QueueSupport.RegisterQueue("index-defaultindex-index");
                QueueSupport.RegisterQueue("index-defaultindex-query");

                if(DataPatcher.DoPatching())
                    return;

                throw new NotSupportedException("Functionality moved up-to-date to WebTemplateManager");

                //ProcessErrors(false);
                //return;

                string templateLocation = "livetemplate";
                string privateSiteLocation = "livesite";
                string publicSiteLocation = "livepubsite";
                const string accountNamePart = "oip-account\\";
                const string publicGroupNamePart = "oip-public\\";
                const string groupNamePart = "oip-group\\";
                const string wwwNamePart = "www-public\\";
                //DoMapData(webGroup);
                //return;
                string directory = Directory.GetCurrentDirectory();
                if (directory.EndsWith("\\") == false)
                    directory = directory + "\\";
                string[] allFiles =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(
                        str => str.Substring(directory.Length)).Where(str => str.StartsWith("theball-") == false).ToArray();
                string[] groupTemplates =
                    allFiles.Where(file => file.StartsWith(accountNamePart) == false && file.StartsWith(publicGroupNamePart) == false && file.StartsWith(wwwNamePart) == false).
                        ToArray();
                string[] publicGroupTemplates =
                    allFiles.Where(file => file.StartsWith(accountNamePart) == false && file.StartsWith(groupNamePart) == false && file.StartsWith(wwwNamePart) == false).
                        ToArray();
                string[] accountTemplates =
                    allFiles.Where(file => file.StartsWith(groupNamePart) == false && file.StartsWith(publicGroupNamePart) == false && file.StartsWith(wwwNamePart) == false).
                        ToArray();
                string[] wwwTemplates =
                    allFiles.Where(file => file.StartsWith(groupNamePart) == false && file.StartsWith(publicGroupNamePart) == false && file.StartsWith(accountNamePart) == false).
                        ToArray();
                //UploadAndMoveUnused(accountTemplates, groupTemplates, publicGroupTemplates, null);
                //UploadAndMoveUnused(null, null, null, wwwTemplates);
                //UploadAndMoveUnused(null, null, publicGroupTemplates, null);
                //UploadAndMoveUnused(accountTemplates, null, null);

                //DeleteAllAccountAndGroupContents(true);
                //RefreshAllAccounts();

                // TODO: The delete above needs to go through first before the refresh one below

                /*
                RenderWebSupport.RefreshAllAccountAndGroupTemplates(true, "AaltoGlobalImpact.OIP.Blog", "AaltoGlobalImpact.OIP.Activity", "AaltoGlobalImpact.OIP.AddressAndLocation",
                    "AaltoGlobalImpact.OIP.Image", "AaltoGlobalImpact.OIP.ImageGroup", "AaltoGlobalImpact.OIP.Category");
                */
                //RunTaskedQueueWorker();


                //FileSystemSupport.UploadTemplateContent(groupTemplates, webGroup, templateLocation, true);
                Console.WriteLine("Starting to sync...");
                //DoSyncs(templateLocation, privateSiteLocation, publicSiteLocation);
                //"grp/default/pub/", true);
                return;
                //doDataTest(connStr);
                //InitLandingPages();
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();
            } 
                catch(InvalidDataException ex)
            {
                Console.WriteLine("Error exit: " + ex.ToString());
            }
        }

        private static void TestEmail()
        {
            EmailSupport.SendEmail("no-reply-theball@msunit.citrus.fi", "kalle.launiala@citrus.fi", "The Ball - Says Hello!",
            "Text testing...");
        }

        private static void AddLoginToAccount(string loginUrlID, string accountID)
        {
            TBRAccountRoot accountRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRAccountRoot>(accountID);

            TBLoginInfo loginInfo = TBLoginInfo.CreateDefault();
            loginInfo.OpenIDUrl = loginUrlID;

            accountRoot.Account.Logins.CollectionContent.Add(loginInfo);
            accountRoot.Account.StoreAccountToRoot();
        }

        /*
        private static void TestDriveDynamicCreation()
        {
            object test = RenderWebSupport.GetOrInitiateContentObject(new List<RenderWebSupport.ContentItem>(),
                                                                      "AaltoGlobalImpact.OIP.InformationSource",
                                                                      "vilperi", false);
        }*/

        private const string FixedGroupID = "05DF28FD-58A7-46A7-9830-DA3F51AAF6AF";

        private static TBCollaboratingGroup InitializeDefaultOIPWebGroup()
        {
            TBRGroupRoot groupRoot = ObjectStorage.RetrieveFromDefaultLocation<TBRGroupRoot>(FixedGroupID);
            if(groupRoot == null)
            {
                groupRoot = TBRGroupRoot.CreateDefault();
                groupRoot.ID = FixedGroupID;
                groupRoot.UpdateRelativeLocationFromID();
                groupRoot.Group.JoinToGroup("kalle.launiala@citrus.fi", "moderator");
                groupRoot.Group.JoinToGroup("jeroen@caloom.com", "moderator");
                StorageSupport.StoreInformation(groupRoot);
            }
            return groupRoot.Group;
        }


        private static void ReportInfo(string text)
        {
            Console.WriteLine(text);
        }

    }
}
//AddLoginToAccount("https://www.google.com/accounts/o8/id?id=AItOawkXb-XQERsvhNkZVlEEiCSOuP1y82uHCQc", "fbbaaded-6615-4083-8ea8-92b2aa162861");
//TestDriveQueueWorker();
//TestDriveDynamicCreation();
//return;
//bool result = EmailSupport.SendEmail("kalle.launiala@gmail.com", "kalle.launiala@citrus.fi", "The Ball - Says Hello!",
//                       "Text testing...");
