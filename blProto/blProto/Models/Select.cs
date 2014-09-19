using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RazorTesting.Models
{
    public class Select
    {
        public SelectList List { get; set; }
        public string and { get; set; }
        public string or { get; set; }
        public string not { get; set; }
    }
}