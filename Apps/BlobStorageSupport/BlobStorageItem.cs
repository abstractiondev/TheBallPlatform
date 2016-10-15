using System;
using System.Collections.Generic;
using System.Text;

namespace TheBall.CORE.Storage
{
    public class BlobStorageItem
    {
        public string ContentMD5 { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
    }
}
