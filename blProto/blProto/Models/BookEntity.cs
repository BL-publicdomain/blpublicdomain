using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace RazorTesting.Models
{
    public class BookEntity : TableEntity
    {

        public BookEntity(string category, string sku)
            : base(category, sku) { }

        public BookEntity() { }

        public string partitionKey { get; set; }
        public string rowKey { get; set; }
        public string azureUrl { get; set; }
        public string biblioasJson { get; set; }
        public string date { get; set; }
        public string electonicSysNum { get; set; }
        public string flickr_original_jpeg { get; set; }
        public string flickr_url { get; set; }
        public string fromShelfMark { get; set; }
        public string height { get; set; }
        public string idx { get; set; }
        public string ocrtext { get; set; }
        public string pdfs { get; set; }
        public string place { get; set; }
        public string printSysNum { get; set; }
        public string publisher { get; set; }
        public string scanNumber { get; set; }
        public string sizeBracket { get; set; }
        public string tags { get; set; }
        public string title { get; set; }
        public string vol { get; set; }
        public string width { get; set; }
    }
}