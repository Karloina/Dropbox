using System.IO;
using Microsoft.AspNetCore.Http;

namespace Dropbox
{
    public class UploadTask : TransferTask
    {
        public IFormFile FormFile { get; set; }

        public override void DelayedJob()
        {
            using (var stream = File.Create($@"{FileLocation}\{Login}\{FileName}"))
            {
                FormFile.CopyTo(stream);
            }
        }
    }
}
