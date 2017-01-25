using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure;

namespace AzMTool
{
    class Program
    {
        static void Main(string[] args)
        {
            return;
            var token = GetAuthorizationHeader();
            var subscriptionId = ConfigurationManager.AppSettings["subscriptionId"];
            //var credential = new TokenCloudCredentials(
            //  ConfigurationManager.AppSettings["subscriptionId"], token);
            var thumbPrint = "‎7e 57 78 78 2a 77 0c 00 b5 05 38 dd c5 5f 8e 11 d5 a9 33 20";
            var credential = new CertificateCloudCredentials(subscriptionId,
                GetCertificate(StoreName.My, StoreLocation.CurrentUser, thumbPrint));
            CreateResources(credential);
            Console.ReadLine();

            DeleteResources(credential);
            Console.ReadLine();
        }

        private static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation storeLocation, string thumbPrint)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                StringBuilder builder = new StringBuilder(thumbPrint.Length);
                foreach (char c in thumbPrint)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        builder.Append(c);
                    }
                }

                string cleanThumbprint = builder.ToString();
                X509Certificate2Collection list = store.Certificates.Find(
                   X509FindType.FindByThumbprint, cleanThumbprint, false);

                X509Certificate2 cert;
                if (list == null || list.Count != 1)
                {
                    cert = null;
                }
                else
                {
                    cert = list[0];
                }

                return cert;
            }
            finally
            {
                store.Close();
            }
        }

        public async static void DeleteResources(
  CertificateCloudCredentials credential)
        {
            Console.WriteLine(
              "Deleting the Virtual Machine. This may take a few minutes...");
            var vmResult =
              await VirtualMachineCreator.DeleteVirtualMachine(credential);
            Console.WriteLine(vmResult);

            Console.WriteLine("Deleting the service...");
            var csResult =
              await CloudServiceCreator.DeleteCloudService(credential);
            Console.WriteLine(csResult);

            Console.WriteLine("Deleting the storage account...");
            var saResult =
              await StorageAccountCreator.DeleteStorageAccount(credential);
            Console.WriteLine(saResult);
        }

        public async static void CreateResources(
  CertificateCloudCredentials credential)
        {
            Console.WriteLine(
              "Creating the storage account. This may take a few minutes...");
            var stResult =
              await StorageAccountCreator.CreateStorageAccount(credential);
            Console.WriteLine(stResult);
            Console.WriteLine("Creating the cloud service...");
            var csResult =
              await CloudServiceCreator.CreateCloudService(credential);
            Console.WriteLine(csResult);

            Console.WriteLine(
  "Deploying the Virtual Machine. This may take a few minutes...");
            var vmResult =
              await VirtualMachineCreator.CreateVirtualMachine(credential);
            Console.WriteLine(vmResult);
            Console.WriteLine("Press Enter to remove the resources.");
        }

        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            var context = new AuthenticationContext(string.Format(
              ConfigurationManager.AppSettings["login"],
              ConfigurationManager.AppSettings["tenantId"]));

            var thread = new Thread(() =>
            {
                string apiEndPoint = ConfigurationManager.AppSettings["apiEndpoint"];
                string clientId = ConfigurationManager.AppSettings["clientId"];
                Uri redirectUri = new Uri(ConfigurationManager.AppSettings["redirectUri"]);
                var credential = new ClientCredential(clientId: clientId, clientSecret: "cRqXfD+XKak5hiyf5ZK8BKLNDpRbq/Gvg7+X8Ribi7U=");

                /*var task = context.AcquireTokenAsync(
                    apiEndPoint,
                    clientId,
                    redirectUri,
                    new PlatformParameters(PromptBehavior.Auto, null));*/
                var task = context.AcquireTokenAsync(apiEndPoint, clientCredential: credential);
                task.Wait();
                result = task.Result;
                /*
                result = await context.AcquireTokenAsync(
                    apiEndPoint,
                    clientId,
                    redirectUri, 
                    new PlatformParameters(PromptBehavior.Auto, null));
                 * */
                //result = await context.AcquireTokenByAuthorizationCodeAsync("", new Uri(ConfigurationManager.AppSettings["redirectUri"]), new ClientAssertion() )
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }
    }
}
