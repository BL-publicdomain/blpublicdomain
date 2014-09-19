using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace blProto.Controllers
{
    public class ItemPageController : Controller
    {
        //
        // GET: /ItemPage/
        public ActionResult ItemPage(Uri ocr_url, string azure_url, List<string> creators_and_contributors, string date, string electronicsysnum,
         string flickr_original_jpeg, string flickr_url, string fromshelfmark, string height, string idx, string place, string printsysnum,
            string publisher, string scannumber, string sizebracket, string tags, string title, string vol, string width)
        {
        
            ViewBag.title = title;
            ViewBag.azure_url = azure_url;
            ViewBag.electronicsysnum = electronicsysnum;
            ViewBag.flickr_url = flickr_url;
            ViewBag.fromshelfmark = fromshelfmark;
            ViewBag.height = height;
            ViewBag.idx = idx;
            ViewBag.printsysnum = printsysnum;
            ViewBag.scannumber = scannumber;
            ViewBag.sizebracket = sizebracket;
            ViewBag.tags = tags;
            ViewBag.width = width;
            ViewBag.creators_and_contributors = creators_and_contributors;
            ViewBag.image = flickr_original_jpeg;
            ViewBag.publisher = publisher;
            ViewBag.year = date;
            ViewBag.place = place;
            ViewBag.ocr = ocr_url;            
            
            ViewBag.ocrPreview = readOCR(ocr_url);
            
            return View();


        }

        public static string readOCR (Uri ocr_url) {
            AzureSearchServiceController checkUrl = new AzureSearchServiceController();
            string ocrPrevText = "";
            if (checkUrl.RemoteFileExists(ocr_url.ToString()))
            {
                

                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] raw = wc.DownloadData(ocr_url);
                string webData = System.Text.Encoding.UTF8.GetString(raw);

                string[] ocrSplit = webData.Split(' ');

                for (int i = 0; i < 300; i++)
                {
                    ocrPrevText += ocrSplit[i];
                    ocrPrevText += " ";

                }

                return ocrPrevText;

            }
            else
            {

                ocrPrevText ="Unfortunately OCR is not available for this document";

            } return ocrPrevText;
           
        }
        public ActionResult Download(Uri ocr_url, string azure_url, List<string> creators_and_contributors, string date, string electronicsysnum,
        string flickr_original_jpeg, string flickr_url, string fromshelfmark, string height, string idx, string place, string printsysnum,
           string publisher, string scannumber, string sizebracket, string tags, string title, string vol, string width)
        {



            var outputStream = new MemoryStream();
            XDocument doc = new XDocument(new XElement("body",
                                         new XElement("metadata",
                                             new XElement("azure_url", azure_url),
                                             new XElement("date", date),
                                             new XElement("electronicsysnum", creators_and_contributors[0]),
                                             new XElement("electronicsysnum", electronicsysnum),
                                             new XElement("flickr_original_jpeg", flickr_original_jpeg),
                                             new XElement("flickr_url", flickr_url),
                                             new XElement("fromshelfmark", fromshelfmark),
                                             new XElement("height", height),
                                             new XElement("idx", idx),
                                             new XElement("ocrtext", ocr_url.ToString()),
                                             new XElement("place", place),
                                             new XElement("printsysnum", printsysnum),
                                             new XElement("publisher", publisher),
                                             new XElement("scannumber", scannumber),
                                             new XElement("sizebracket", sizebracket),
                                             new XElement("tags", tags),
                                             new XElement("title", title),
                                             new XElement("vol", vol),
                                             new XElement("width", width))));






            MemoryStream xmlStream = new MemoryStream();
            doc.Save(xmlStream);

            xmlStream.Flush();//Adjust this if you want read your data 
            xmlStream.Position = 0;


            using (var zip = new ZipFile())
            {

                System.Net.WebClient wc = new System.Net.WebClient();
                Stream s = wc.OpenRead(ocr_url);

                zip.AddEntry(date + "_" + printsysnum + ".txt", s);
                zip.AddEntry(date + "_" + printsysnum + ".xml", xmlStream);


                zip.Save(outputStream);
            }

            outputStream.Position = 0;
            return File(outputStream, "application/zip", date + "_" + printsysnum + ".zip");

        }



        public ActionResult InternetShortcut(string url, string printsysnum , string date)
        {
            
            
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            tw.WriteLine("[InternetShortcut]");
            tw.WriteLine("URL=" + url);
           
            tw.Flush();
            byte[] bytes = ms.ToArray();
            ms.Close();

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment;    filename=" + printsysnum + "_" + date + ".url");
            Response.BinaryWrite(bytes);
            Response.End();










            return File(ms, "application/text", "file.url");
        }

    

	}
}