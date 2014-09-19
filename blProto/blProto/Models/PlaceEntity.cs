using System.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Data.Services.Client;


namespace RazorTesting.Models
{
    public class PlaceEntity: TableEntity
    {
        public PlaceEntity(string category, string sku)
            : base(category, sku) { }

        public PlaceEntity() { }

        public string newPartitionKey { get; set; }

    }
}