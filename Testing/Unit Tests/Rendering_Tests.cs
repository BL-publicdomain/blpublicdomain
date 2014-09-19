using System;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Web;
using blProto.Controllers;
using System.Web.Mvc;
using Moq;


namespace blProto
{      
    //Simple test cases that test if the Controllers return the appropriate Views
    [TestClass]
    public class TestViews
    {
        [TestMethod]
        public void AboutView()
        {
            AboutController test = new AboutController();
            var results = test.AboutPage() as ViewResult;
            Assert.AreEqual("", results.ViewName);
        }
        [TestMethod]
        public void AdvSearchView()
        {
            AdvSearchController AdvSearchView = new AdvSearchController();
            var results = AdvSearchView.Index() as ViewResult;
            Assert.AreEqual("", results.ViewName);
            
        }
        [TestMethod]
        public void HomePageView()
        {
            HomeController HomeView = new HomeController();
            var results = HomeView.Index() as ViewResult;
            Assert.AreEqual("", results.ViewName);
        }
        [TestMethod]
        public void TheCollectionView()
        {
            CollectionPageController CollectionPage = new CollectionPageController();
            var results = CollectionPage.TheCollection() as ViewResult;
            Assert.AreEqual("", results.ViewName);
        }

        [TestMethod]
        public void FaqPageView()
        {
            FAQPageController FaqPage = new FAQPageController();
            var results = FaqPage.FAQ() as ViewResult;
            Assert.AreEqual("", results.ViewName);
        }
        
        [TestMethod]
        public void ContactPageView()
        {
            ContactPageController ContactPage = new ContactPageController();
            var results = ContactPage.ContactUs() as ViewResult;
            Assert.AreEqual("", results.ViewName);
        }

        //[TestMethod]
        //public void StopWordsView()
        //{
        //    StopWordListController StopWordsPage = new StopWordListController();
        //    var results = StopWordsPage.StopWordList() as ViewResult;
        //    Assert.AreEqual("", results.ViewName);
        //}

        [TestMethod]
        public void ResultsView()
        {
            ResultController ResultsPage = new ResultController();
            var results = ResultsPage.Index() as ViewResult;
            Assert.AreEqual("", results.ViewName);

        }

        [TestMethod]
        public void ItemPageView()
        {
            ResultController ItemPage = new ResultController();
            var results = ItemPage.Index() as ViewResult;
            Assert.AreEqual("", results.ViewName);

        }


        [TestMethod]
        public void ErrorPageView()
        {
            ErrorPageController ErrorPage = new ErrorPageController();
            var results = ErrorPage.ErrorPage() as ViewResult;
            Assert.AreEqual("", results.ViewName);

        }
        [TestMethod]
        public void ItemImagePageView()
        {
            ItemPageImageController ItemImagePage = new ItemPageImageController();
            //Initialise sample Title and sample Image to test the ActionResults
            //In order to pass it accepts two parameters Title and Image
            //Then returns the correct index
            //This test should pass
            string image = "testString";
            string title = "testString";
            var results = ItemImagePage.ItemImage(title, image) as ViewResult;
            Assert.AreEqual("", results.ViewName);

        }

        //[TestMethod]
        //public void StatisticalAnalysisPageView()
        //{
        //    StatisticalAnalysisController StatisticalAnalysisPage = new StatisticalAnalysisController();
        //    //Initialise sample URI and sample Title to test the ActionResults
        //    //In order to pass it accepts two parameters Uri and BookTitle
        //    //Then returns the correct index
        //    //This test should pass
        //    Uri testUri = new Uri("http://blmc.blob.core.windows.net/ocrplaintext/002547752_0.txt");
        //    string testTitle = "A good story";

           
        //    var results = StatisticalAnalysisPage.Index(testUri,testTitle) as ViewResult;
        //    ControllerContext keyword = new ControllerContext();
            
        //    Assert.AreEqual("", results.ViewName);

        //} 

    }
}