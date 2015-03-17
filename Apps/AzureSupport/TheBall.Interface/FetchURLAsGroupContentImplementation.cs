using System.Data.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class FetchURLAsGroupContentImplementation
    {
        public static IContainerOwner GetTarget_Owner(string groupId)
        {
            return new VirtualOwner("grp", groupId);
        }

        public static BinaryFile GetTarget_BinaryFile(string fileName, IContainerOwner owner)
        {
            BinaryFile binaryFile = new BinaryFile();
            binaryFile.SetLocationAsOwnerContent(owner, binaryFile.ID);
            binaryFile.OriginalFileName = fileName;
            binaryFile.StoreInformation(owner);
            return binaryFile;
        }

        public static void ExecuteMethod_FetchDataAndAttachToFile(string dataUrl, BinaryFile binaryFile)
        {
            var owner = VirtualOwner.FigureOwner(binaryFile);
            var mediaContent = new MediaContent();
            mediaContent.SetLocationAsOwnerContent(owner, mediaContent.ID);
            mediaContent.FileExt = Path.GetExtension(binaryFile.OriginalFileName);
            mediaContent.OriginalFileName = binaryFile.OriginalFileName;
            binaryFile.Data = mediaContent;
            HttpWebRequest request = WebRequest.CreateHttp(dataUrl);
            var response = request.GetResponse();
            var readStream = response.GetResponseStream();
            string blobName = mediaContent.RelativeLocation + mediaContent.FileExt;
            var storageBlob = StorageSupport.CurrActiveContainer.GetBlob(blobName, owner);
            var writeStream = storageBlob.OpenWrite();
            int bufferSize = 1024*1024;
            byte[] dataBuffer = new byte[bufferSize];

            int totalLength = 0;
            int actualReadLength = 0;
            Task writeTask = null;
            do
            {
                Task<int> readTask = readStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                if (writeTask != null)
                    Task.WaitAll(readTask, writeTask);
                else 
                    readTask.Wait();
                actualReadLength = readTask.Result;
                writeTask = writeStream.WriteAsync(dataBuffer, 0, actualReadLength);
                if(actualReadLength < bufferSize)
                    writeTask.Wait();
                totalLength += actualReadLength;
            } while (actualReadLength < bufferSize);
            binaryFile.Data.ContentLength = totalLength;
            binaryFile.StoreInformation();
        }


    }
}