using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBall.Core;
using TheBall.Core.StorageCore;
//using Trinet.Core.IO.Ntfs;
using Xunit;

namespace PlatformServiceTests.StorageService
{
    public class FileSystemStorageTests
    {
        private const string DefaultTestPath = @"T:/tmp/tbtest/TheBallData/";
        IContainerOwner testOwner = new VirtualOwner("tst", Guid.Empty.ToString());
        public async Task<FileSystemStorageService> GetFSS()
        {
            var fss = new FileSystemStorageService(DefaultTestPath);
            await fss.InitializeService();
            return fss;
        }

        [Fact]
        public async Task RequireTBRootAsPartOfFolderNameFail()
        {
            Assert.Throws<ArgumentException>(() => new FileSystemStorageService(@"X:\NotValidPath"));
        }

        [Fact]
        public async Task RequireTBRootAsPartOfFolderNameOK()
        {
            var fsStorage = new FileSystemStorageService(DefaultTestPath);
        }

        [Fact]
        public async Task PutBlobItem()
        {
            var fss = await GetFSS();
            var testData = "test";
            var result = await fss.UploadBlobTextA(testOwner, "testblob.txt", testData);
            Assert.Equal(4, result.Length);
            //Assert.Equal("xxxDeadBeef", result.ContentMD5);
        }

#if never
        [Fact]
        public async Task GetADSData()
        {
            const string ADSTestFileName = @"D:\UserData\Kalle\Dropbox (Personal)\tbtesting.txt";
            var fileInfo = new FileInfo(ADSTestFileName);
            var adsList = fileInfo.ListAlternateDataStreams();
            var dropBoxAds = adsList.FirstOrDefault(item => item.Name == "com.dropbox.attrs");
            if (dropBoxAds != null)
            {
                byte[] dataContent = null;
                using (var adsStream = dropBoxAds.OpenRead())
                {
                    dataContent = new byte[adsStream.Length];
                    await adsStream.ReadAsync(dataContent, 0, dataContent.Length);
                }

                var textContent = UTF8Encoding.UTF8.GetString(dataContent);
            }
        }
#endif
    }
}
