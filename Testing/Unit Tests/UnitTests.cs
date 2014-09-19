using blProto.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blProto
{
    
    [TestClass]
    public class UnitTests
    {
        //Test if the return type is int
        [TestMethod]
        public void StatisticalAnalysisReturnType()
        {
            //Create Mock Data
            List<string>    tagsList = new List<string>();
	                        tagsList.Add("lewis");
	                        tagsList.Add("Shetlander");
	                        tagsList.Add("shetlander");
	                        tagsList.Add("Lewis");

            List<string>    creators_and_contributorsList = new List<string>();
	                        creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
                            BookModel model = new BookModel
                            {
                                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                                creators_and_contributors = creators_and_contributorsList,
                                date = 1894,
                                electronicsysnum = 014812388,
                                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                                fromshelfmark = "British Library HMNTS 012630.e.11.",
                                height = 1896,
                                idx = 1,
                                // This OCR Text is valid
                                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                                place = "London",
                                printsysnum = 001218010,
                                publisher = "T. Nelson & Sons",
                                scannumber = 000191,
                                sizebracket = "plates",
                                tags = tagsList,
                                title = "Sons of the Vikings. An Orkney story",
                                vol = 0,
                                width = 1256
                            }; 
            
            
            StatisticalAnalysisController StatisticalAnalysisPage = new StatisticalAnalysisController();
            Uri testUri = new Uri(model.ocrtext);
            var results = StatisticalAnalysisPage.frequentWordInOCR(testUri, "king");          
            int testType = 4;
            Assert.AreEqual(testType.GetType(), results.GetType());

        }

        //Test when the ocr text url is invalid
        //This test should redirect the users to the error page
        [TestMethod]
        public void StatisticalAnalysis_INVALID_OCR()
        {
            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text URL is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/002784427_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };


            StatisticalAnalysisController StatisticalAnalysisPage = new StatisticalAnalysisController();
            Uri testUri = new Uri(model.ocrtext);
            
            
           var actionResult = (RedirectToRouteResult)StatisticalAnalysisPage.Index(testUri, model.title);

                actionResult.RouteValues["action"].Equals("ErrorPage"); 
                actionResult.RouteValues["controller"].Equals("ErrorPage");        
        }
        [TestMethod]
        public void Download_Test_Invalid_URL()
        {

            //Create Mock Data
            List<string>    tagsList = new List<string>();
	                        tagsList.Add("lewis");
	                        tagsList.Add("Shetlander");
	                        tagsList.Add("shetlander");
	                        tagsList.Add("Lewis");

            List<string>    creators_and_contributorsList = new List<string>();
	                        creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
                            BookModel model = new BookModel
                            {
                                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                                creators_and_contributors = creators_and_contributorsList,
                                date = 1894,
                                electronicsysnum = 014812388,
                                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                                fromshelfmark = "British Library HMNTS 012630.e.11.",
                                height = 1896,
                                idx = 1,
                                // This OCR Text is INVALID
                                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/002784427_0.txt",
                                place = "London",
                                printsysnum = 001218010,
                                publisher = "T. Nelson & Sons",
                                scannumber = 000191,
                                sizebracket = "plates",
                                tags = tagsList,
                                title = "Sons of the Vikings. An Orkney story",
                                vol = 0,
                                width = 1256
                            }; 
            AzureSearchServiceController DownloadTest = new AzureSearchServiceController();
            Uri testUri = new Uri(model.ocrtext);
            var results = (RedirectToRouteResult)DownloadTest.Download(testUri, model.azure_url, model.creators_and_contributors, model.date.ToString(), model.electronicsysnum.ToString(), model.flickr_original_jpeg
                , model.flickr_url, model.fromshelfmark, model.height.ToString(), model.idx.ToString(), model.place, model.printsysnum.ToString(), model.publisher, model.scannumber.ToString(), model.sizebracket, model.tags.ToString()
                , model.title, model.vol.ToString(), model.width.ToString());

            results.RouteValues["action"].Equals("ErrorPage");
            results.RouteValues["controller"].Equals("ErrorPage"); 

        }
        [TestMethod]
        public void Download_Test_VALID_URL()
        {

            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };
            AzureSearchServiceController DownloadTest = new AzureSearchServiceController();
            Uri testUri = new Uri(model.ocrtext);
            var results = DownloadTest.Download(testUri, model.azure_url, model.creators_and_contributors, model.date.ToString(), model.electronicsysnum.ToString(), model.flickr_original_jpeg
                , model.flickr_url, model.fromshelfmark, model.height.ToString(), model.idx.ToString(), model.place, model.printsysnum.ToString(), model.publisher, model.scannumber.ToString(), model.sizebracket, model.tags.ToString()
                , model.title, model.vol.ToString(), model.width.ToString());
            
           Assert.AreEqual(typeof(FileStreamResult), results.GetType());

        }
        [TestMethod]
        public void Download_InternetShortcut()
        {

            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };
            ItemPageController DownloadTest = new ItemPageController();
            
            var results = DownloadTest.InternetShortcut(model.ocrtext, model.printsysnum.ToString(), model.date.ToString()); 

            Assert.AreEqual(typeof(FileStreamResult), results.GetType());

        }

        [TestMethod]
        public void Search_By_Year()
        {

            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };
            AzureSearchServiceController SearchByYear = new AzureSearchServiceController();

            
            var results = (RedirectToRouteResult)SearchByYear.SearchByYear(model.date.ToString());
            
            results.RouteValues["action"].Equals("Index");
            results.RouteValues["controller"].Equals("Home");

            Assert.AreEqual("Index", results.RouteValues["action"]);
            Assert.AreEqual("AzureSearchService", results.RouteValues["controller"]);
            

        }
        [TestMethod]
        public void Search_By_Location()
        {

            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };
            AzureSearchServiceController SearchByLocation = new AzureSearchServiceController();


            var results = (RedirectToRouteResult)SearchByLocation.SearchByLocation(model.place);

            results.RouteValues["action"].Equals("Index");
            results.RouteValues["controller"].Equals("Home");

            Assert.AreEqual("Index", results.RouteValues["action"]);
            Assert.AreEqual("AzureSearchService", results.RouteValues["controller"]);


        }
        [TestMethod]
        public void Search_By_Publisher()
        {

            //Create Mock Data
            List<string> tagsList = new List<string>();
            tagsList.Add("lewis");
            tagsList.Add("Shetlander");
            tagsList.Add("shetlander");
            tagsList.Add("Lewis");

            List<string> creators_and_contributorsList = new List<string>();
            creators_and_contributorsList.Add("GUNN, John - M.A., D.Sc");


            //Create a mock model to pass as a parameter
            BookModel model = new BookModel
            {
                azure_url = "http://blmc.blob.core.windows.net/1894/001544024_0_000191_1_1894_plates.jpg",
                creators_and_contributors = creators_and_contributorsList,
                date = 1894,
                electronicsysnum = 014812388,
                flickr_original_jpeg = "http://farm4.staticflickr.com/3811/11230026636_0021861570_o.jpg",
                flickr_url = "https://flickr.com/photos/britishlibrary/11230026636",
                fromshelfmark = "British Library HMNTS 012630.e.11.",
                height = 1896,
                idx = 1,
                // This OCR Text is INVALID
                ocrtext = "http://blmc.blob.core.windows.net/ocrplaintext/001544024_0.txt",
                place = "London",
                printsysnum = 001218010,
                publisher = "T. Nelson & Sons",
                scannumber = 000191,
                sizebracket = "plates",
                tags = tagsList,
                title = "Sons of the Vikings. An Orkney story",
                vol = 0,
                width = 1256
            };
            AzureSearchServiceController SearchByPublisher = new AzureSearchServiceController();


            var results = (RedirectToRouteResult)SearchByPublisher.SearchByPublisher(model.publisher);

            results.RouteValues["action"].Equals("Index");
            results.RouteValues["controller"].Equals("Home");

            Assert.AreEqual("Index", results.RouteValues["action"]);
            Assert.AreEqual("AzureSearchService", results.RouteValues["controller"]);


        }
        [TestMethod]
        public void AzureSearch()
        {

           
            AzureSearchServiceController SearchByPublisher = new AzureSearchServiceController();


            var results = SearchByPublisher.insertANDElemensToFinalList()

            results.RouteValues["action"].Equals("Index");
            results.RouteValues["controller"].Equals("Home");

            Assert.AreEqual("Index", results.RouteValues["action"]);
            Assert.AreEqual("AzureSearchService", results.RouteValues["controller"]);


        }
    }
}