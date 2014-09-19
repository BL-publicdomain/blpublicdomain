using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace blProto.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "British Library Big Data Experiment";
            return View();
        }





        // Retrieve the keyword search.
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            // Instantiate a list to store the keyword query as a string.
            List<string> querys = new List<string>();



            foreach (string formData in formCollection)
            {
                ViewData[formData] = formCollection[formData];
            }



            // This loop is similar to the one above.
            // It adds the keyword query to the list for further access.
            foreach (string key in formCollection)
            {
                string value = formCollection[key];
                querys.Add(value);
            }

            //string x is responsible to stores the word home as well as the input keyword fo the user
            string x = "";
            // stores in the string x the word home in order to pass to the URI parameter
            x += "home";
            x += "¬";
            x += querys[0];
            // The return statement redirects to the Index method of the
            // AzureSearchService controller, sending the keyword query as a URI parameter.
            return RedirectToAction("Index", "AzureSearchService", new RouteValueDictionary(new { queryID = x }));
        }

    }

}
