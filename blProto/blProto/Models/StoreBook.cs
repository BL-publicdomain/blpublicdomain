using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RazorTesting.Models
{
    public class StoreBook
    {


        public int key { get; set; }

        public string title { get; set; }
        public string flickr_original_jpeg { get; set; }
        public string ocrtext { get; set; }
        public Int32 idx { get; set; }
        public Int32 printsysnum { get; set; }
        public Int32 vol { get; set; }
        public Int32 scannumber { get; set; }
        public Int32 height { get; set; }
        public Int32 width { get; set; }
        public string fromshelfmark { get; set; }
        public string place { get; set; }
        public string sizebracket { get; set; }
        public Int32 electronicsysnum { get; set; }
        public Int32 date { get; set; }
        public string flickr_url { get; set; }
        public string azure_url { get; set; }
        public List<string> tags { get; set; }

    }
}