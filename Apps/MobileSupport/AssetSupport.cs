using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace TheBall.CORE.MobileSupport
{
    class AssetSupport
    {
        public static async Task ExtractZip(Stream zipFileStream, string uiFolder)
        {
            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var memoryStream = new MemoryStream();
            await zipFileStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var targetFolder = Path.Combine(personalFolder, uiFolder);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(memoryStream, targetFolder, FastZip.Overwrite.Always, null, null, null, true, false);
            var verifyFile = Path.Combine(targetFolder, "index.html");
            bool exists = File.Exists(verifyFile);
        }
    }
}
