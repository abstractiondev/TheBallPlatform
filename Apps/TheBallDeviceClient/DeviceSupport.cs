using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PCLCrypto;
using CryptoStream = System.Security.Cryptography.CryptoStream;
using CryptoStreamMode = System.Security.Cryptography.CryptoStreamMode;
using SymmetricAlgorithm = PCLCrypto.SymmetricAlgorithm;

namespace TheBall.Support.DeviceClient
{
    public static class DeviceSupport
    {
        public static DeviceOperationData ExecuteDeviceOperation(this Device device,
            DeviceOperationData operationParameters, string ownerPrefix = null)
        {
            var dod = DeviceSupport.ExecuteRemoteOperation<DeviceOperationData>(device,
                "TheBall.CORE.RemoteDeviceCoreOperation", operationParameters, ownerPrefix);
            if (!dod.OperationResult)
                throw new OperationCanceledException("Remote device operation failed");
            return dod;
        }


        public const string OperationPrefixStr = "op/";

        public static TReturnType ExecuteRemoteOperation<TReturnType>(Device device, string operationName,
            object operationParameters, string ownerPrefix = null)
        {
            return
                (TReturnType)
                    executeRemoteOperation<TReturnType>(device, operationName, operationParameters, ownerPrefix);
        }

        public static void ExecuteRemoteOperationVoid(Device device, string operationName, object operationParameters,
            string ownerPrefix = null)
        {
            executeRemoteOperation<object>(device, operationName, operationParameters, ownerPrefix);
        }

        private static object executeRemoteOperation<TReturnType>(Device device, string operationName,
            object operationParameters, string ownerPrefix)
        {
            string operationUrl = String.Format("{0}{1}", OperationPrefixStr, operationName);
            string url = device.ConnectionURL.Replace("/DEV", "/" + operationUrl);
            if (ownerPrefix != null)
                url = replaceUrlAccountOwner(url, ownerPrefix);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            var aes = createAES(device.AESKey);
            var ivBase64 = Convert.ToBase64String(aes.IV);
            request.Headers.Add("Authorization", "DeviceAES:" + ivBase64
                                                 + ":" + device.EstablishedTrustID
                                                 + ":" + device.AccountEmail);
            var requestStream = request.GetRequestStream();
            var encryptor = aes.CreateEncryptor();
            using (var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write))
            {
                JSONSupport.SerializeToJSONStream(operationParameters, cryptoStream);
            }
            var response = (HttpWebResponse) request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("PushToInformationOutput failed with Http status: " +
                                                    response.StatusCode.ToString());
            if (typeof (TReturnType) == typeof (object))
                return null;
            return getObjectFromResponseStream<TReturnType>(response, device.AESKey);
        }

        private static async Task executeRemoteOperationAsync(Device device, string operationName, string ownerPrefix, Func<Stream, Task> requestStreamHandler,
            Func<Stream, Task> responseStreamHandler)
        {
            string operationUrl = String.Format("{0}{1}", OperationPrefixStr, operationName);
            string url = device.ConnectionURL.Replace("/DEV", "/" + operationUrl);
            if (ownerPrefix != null)
                url = replaceUrlAccountOwner(url, ownerPrefix);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            var encAes = createAES(device.AESKey);
            var ivBase64 = Convert.ToBase64String(encAes.IV);
            request.Headers.Add("Authorization", "DeviceAES:" + ivBase64
                                                 + ":" + device.EstablishedTrustID
                                                 + ":" + device.AccountEmail);
            var requestStream = request.GetRequestStream();
            var pclAESKey = createPCLAES(device.AESKey);
            var encryptor = WinRTCrypto.CryptographicEngine.CreateEncryptor(pclAESKey, encAes.IV);
            using (var reqCryptoStream = new PCLCrypto.CryptoStream(requestStream, encryptor, PCLCrypto.CryptoStreamMode.Write))
                try
                {
                    await requestStreamHandler(reqCryptoStream);
                }
                finally
                {
                    //reqCryptoStream.Dispose();
                }
                /*
                var encryptor = encAes.CreateEncryptor();
                using (var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write))
                {
                    await requestStreamHandler(cryptoStream);
                }*/
                var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("ExecuteRemoteOperation failed with Http status: " +
                                                    response.StatusCode.ToString());
            string ivStr = response.Headers["IV"];
            var decAes = createAES(device.AESKey);
            decAes.IV = Convert.FromBase64String(ivStr);
            var responseStream = response.GetResponseStream();

            var decryptor = WinRTCrypto.CryptographicEngine.CreateDecryptor(pclAESKey, decAes.IV);
            using (var respCryptoStream = new PCLCrypto.CryptoStream(responseStream, decryptor, PCLCrypto.CryptoStreamMode.Read))
                try
                {
                    await responseStreamHandler(respCryptoStream);
                }
                finally
                {
                    //respCryptoStream.Dispose();
                }

            /*
            var decryptor = decAes.CreateDecryptor();
            using (CryptoStream cryptoStream = new CryptoStream(responseStream, decryptor, CryptoStreamMode.Read))
            {
                await responseStreamHandler(cryptoStream);
            }*/

        }

        private static ICryptographicKey createPCLAES(byte[] aesKey)
        {
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            return provider.CreateSymmetricKey(aesKey);
        }

        private static AesManaged createAES(byte[] aesKey, byte[] iv = null)
        {
            const PaddingMode PADDING_MODE = PaddingMode.ISO10126;
            const CipherMode AES_MODE = CipherMode.CBC;
            const int AES_FEEDBACK_SIZE = 8;
            const int AES_KEYSIZE = 256;
            const int AES_BLOCKSIZE = 128;

            AesManaged aes = new AesManaged();
            aes.KeySize = AES_KEYSIZE;
            aes.BlockSize = AES_BLOCKSIZE;
            if (iv == null)
                aes.GenerateIV();
            else
                aes.IV = iv;
            aes.Key = aesKey;
            aes.Padding = PADDING_MODE;
            aes.Mode = AES_MODE;
            aes.FeedbackSize = AES_FEEDBACK_SIZE;
            return aes;
        }

        private static string replaceUrlAccountOwner(string url, string ownerPrefix)
        {
            return url.Replace("/auth/account/", "/auth/" + ownerPrefix + "/");
        }

        private static TReturnType getObjectFromResponseStream<TReturnType>(HttpWebResponse response, byte[] aesKey)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                string ivStr = response.Headers["IV"];
                var respStream = response.GetResponseStream();
                respStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var aes = createAES(aesKey, Convert.FromBase64String(ivStr));
                var decryptor = aes.CreateDecryptor();

                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var contentObject = JSONSupport.GetObjectFromStream<TReturnType>(cryptoStream);
                    return contentObject;
                }
            }
        }

        public static void PushContentToDevice(Device device, Stream localContentStream, string destinationContentName)
        {
            string url = device.ConnectionURL.Replace("/DEV", "/" + destinationContentName);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            var aes = createAES(device.AESKey);
            var ivBase64 = Convert.ToBase64String(aes.IV);
            request.Headers.Add("Authorization",
                "DeviceAES:" + ivBase64 + ":" + device.EstablishedTrustID + ":" + device.AccountEmail);
            using (var requestStream = request.GetRequestStream())
            {
                var encryptor = aes.CreateEncryptor();
                var cryptoStream = new CryptoStream(requestStream, encryptor, CryptoStreamMode.Write);
                localContentStream.CopyTo(cryptoStream);
                cryptoStream.Close();
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new InvalidOperationException("PushToInformationOutput failed with Http status: " +
                                                        response.StatusCode.ToString());
            }

        }

        public static void PushContentToDevice(Device device, string localContentFileName, string destinationContentName)
        {
            using (var fileStream = File.OpenRead(localContentFileName))
            {
                PushContentToDevice(device, fileStream, destinationContentName);
            }
        }

        public static void FetchContentFromDevice(Device device, string remoteContentFileName, Stream localContentStream,
            string ownerPrefix)
        {
            string url = device.ConnectionURL.Replace("/DEV", "/" + remoteContentFileName);
            if (ownerPrefix != null)
                url = replaceUrlAccountOwner(url, ownerPrefix);
            string establishedTrustID = device.EstablishedTrustID;

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", "DeviceAES::" + establishedTrustID + ":" + device.AccountEmail);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("Authroized fetch failed with non-OK status code");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                string ivStr = response.Headers["IV"];
                var respStream = response.GetResponseStream();
                respStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                AesManaged aes = createAES(device.AESKey, Convert.FromBase64String(ivStr));
                var decryptor = aes.CreateDecryptor();
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(localContentStream);
                }
            }
        }

        public static void FetchContentFromDevice(Device device, string remoteContentFileName, string localContentFileName, string ownerPrefix)
        {
            using (FileStream fileStream = File.Create(localContentFileName))
            {
                FetchContentFromDevice(device, remoteContentFileName, fileStream, ownerPrefix);
                fileStream.Close();
            }

        }

        public static async Task ExecuteRemoteOperationAsync(Device device, string operationName, Func<Stream, Task> requestStreamHandler, Func<Stream, Task> responseStreamHandler)
        {
            await executeRemoteOperationAsync(device, operationName, null, requestStreamHandler, responseStreamHandler);
        }
    }
}