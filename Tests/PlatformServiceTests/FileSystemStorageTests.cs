using System;
using System.Threading.Tasks;
using TheBall.Core.StorageCore;
using Xunit;

namespace PlatformServiceTests.StorageService
{
    public class FileSystemStorageTests
    {
        [Fact]
        public async Task RequireTBRootAsPartOfFolderName()
        {
            Assert.Throws<ArgumentException>(() => new FileSystemStorageService(@"X:\NotValidPath"));
        }

    }
}
