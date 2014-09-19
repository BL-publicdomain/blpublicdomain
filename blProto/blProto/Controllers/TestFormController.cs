
using RazorTesting.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace blProto.Controllers
{
    public class TestFormController : Controller
    {
        // GET: /TestForm/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult RetrieveWithFormCollection()
        {
            return View();
        }



        [HttpPost]
        public ActionResult RetrieveWithFormCollection(FormCollection formCollection)
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
                //list(ref querys,value);

            }

            ViewBag.gold = querys[0];
            ViewBag.list = querys;

            //string newKeyword = ReturnKeyword(querys);

            return RedirectToAction("Index", "Class2", new RouteValueDictionary(new { id = querys[0] }));

            //return RedirectToAction("Index", "Class2");
            
            //return View();

        }



        public void list(ref List<string> querys,string x)
        {
            string value = x;
            querys.Add(value);
        
        }



        // This method retrieves the keyword query from the list.
        //public string ReturnKeyword( List<string> querys)
        //{


        //    string kokos = querys[0];
        //    //string kokos = "Manchester";
        //    return kokos;
        //}

	}

}

