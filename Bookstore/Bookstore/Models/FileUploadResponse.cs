using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models
{
    public class FileUploadResponse
    {
        public string Message { get; set; }
        public FileModel File { get; set; }
    }
}
