using System.Web;
using Microsoft.AspNetCore.Http;

namespace TheBall
{
    public class MediaFileData
    {
        public string FileName;
        public byte[] FileContent;
        public IFormFile HttpFile;
    }
}