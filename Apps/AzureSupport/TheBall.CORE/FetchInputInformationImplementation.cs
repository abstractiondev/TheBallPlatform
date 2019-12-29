using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using SecuritySupport;
using TheBall.Core.StorageCore;

namespace TheBall.Core
{
    public class FetchInputInformationImplementation
    {
        public static async Task<InformationInput> GetTarget_InformationInputAsync(IContainerOwner owner, string informationInputId)
        {
            return await ObjectStorage.RetrieveFromDefaultLocationA<InformationInput>(informationInputId, owner);
        }

        public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
        {
            if(informationInput.IsValidatedAndActive == false)
                throw new SecurityException("InformationInput is not active");
        }

        public static string GetTarget_InputFetchLocation(InformationInput informationInput)
        {
            return informationInput.RelativeLocation + "_Input";
        }

        public static string GetTarget_InputFetchName(InformationInput informationInput)
        {
            // TODO: timestamped, incremental and other options supported, now just bulk
            string localContentName = informationInput.LocalContentName;
            if (string.IsNullOrEmpty(localContentName))
                return "bulkdump.all";
            return informationInput.LocalContentName;
        }

        public static async Task ExecuteMethod_FetchInputToStorageAsync(IContainerOwner owner, string queryParameters, InformationInput informationInput, string inputFetchLocation, string inputFetchName, AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
                string url = string.IsNullOrEmpty(queryParameters)
                                 ? informationInput.LocationURL
                                 : informationInput.LocationURL + queryParameters;
            if (authenticatedAsActiveDevice == null)
            {
                WebRequest getRequest = WebRequest.Create(url);
                var response = getRequest.GetResponse();
                var stream = response.GetResponseStream();
                var inputFullName = storageService.CombinePathForOwner(owner, inputFetchLocation, inputFetchName);
                await storageService.UploadBlobStreamA(owner, inputFullName, stream);
            }
            else
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Authorization", "DeviceAES::" + authenticatedAsActiveDevice.EstablishedTrustID + ":");
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new InvalidOperationException("Authroized fetch failed with non-OK status code");
                string ivStr = response.Headers["IV"];
                string contentRoot = inputFetchLocation;
                string blobName = contentRoot + "/" + inputFetchName;
                var blob = StorageSupport.GetOwnerBlobReference(owner, blobName);
                if (blob.Name != blobName)
                    throw new InvalidDataException("Invalid content name");
                var respStream = response.GetResponseStream();
                AesManaged aes = new AesManaged();
                aes.KeySize = SymmetricSupport.AES_KEYSIZE;
                aes.BlockSize = SymmetricSupport.AES_BLOCKSIZE;
                aes.IV = Convert.FromBase64String(ivStr);
                aes.Key = authenticatedAsActiveDevice.ActiveSymmetricAESKey;
                aes.Padding = SymmetricSupport.PADDING_MODE;
                aes.Mode = SymmetricSupport.AES_MODE;
                aes.FeedbackSize = SymmetricSupport.AES_FEEDBACK_SIZE;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                CryptoStream cryptoStream = new CryptoStream(respStream, decryptor, CryptoStreamMode.Read);
                await blob.UploadFromStreamAsync(cryptoStream);
            }
        }

        public static async Task<AuthenticatedAsActiveDevice> GetTarget_AuthenticatedAsActiveDeviceAsync(InformationInput informationInput)
        {
            var authenticationID = informationInput.AuthenticatedDeviceID;
            if (string.IsNullOrEmpty(authenticationID))
                return null;
            var owner = VirtualOwner.FigureOwner(informationInput.RelativeLocation);
            return await ObjectStorage.RetrieveFromOwnerContentA<AuthenticatedAsActiveDevice>(owner, authenticationID);
        }
    }
}