using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IO;
using Bookstore.Models.Enums;
namespace Bookstore.Models
{
    public class ContactFormModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? FileId { get; set; }
        public Guid SubmitterId { get; set; }
        public StateEnum StateEnum { get; set; }
    }
}
