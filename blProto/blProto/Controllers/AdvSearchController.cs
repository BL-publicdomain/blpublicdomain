
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace blProto.Controllers
{
    public class AdvSearchController : Controller
    {
        //
        // GET: /AdvSearch/
        public ActionResult Index()
        {
            return View();
        }





        // Retrieve the advanced keyword search.
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



            string x = "";


            //the AdvancedSearchPage based on the textboxes that contain a search term and select boxes that contain the prefered boolean operator choice of the user,
            //we track the exact input of the user.
            //We store not only the keywords but also the boolean selections of the user per each non empty textbox. 
            //The possible boolean operators are "not", "and" and "or". 
            for (int i = 0; i < (querys.Count) - 1; i++)
            {
                int found = 0;
                if (querys[i] != "")
                {
                    //we pass to the URI parameter the word "title" with the selected search keyword
                    if (i == 0) { x += "title"; }
                    //we pass to the URI parameter the word "author" with the selected search keyword
                    else if (i == 2) { x += "author"; }
                    //we pass to the URI parameter the word "publisher" with the selected search keyword
                    else if (i == 4) { x += "publisher"; }
                    //we pass to the URI parameter the word "location" with the selected search keyword
                    else if (i == 6) { x += "location"; }
                    //we pass to the URI parameter the word "fromDate" with the selected search keyword
                    else if (i == 8) { x += "fromDate"; }
                    //we pass to the URI parameter the word "toDate" with the selected search keyword
                    else if (i == 9) { x += "toDate"; }
                    //we pass to the URI parameter the word "ocr" with the selected search keyword
                    else if (i == 11) { x += "ocr"; }
                    //we pass to the URI parameter the word "boolean" with the chosen operator
                    else if (i == 1) { if (querys[2] != "") { x += "boolean"; } else { found = 1; } }
                    //we pass to the URI parameter the word "boolean" with the chosen operator
                    else if (i == 3) { if (querys[4] != "") { x += "boolean"; } else { found = 1; } }
                    //we pass to the URI parameter the word "boolean" with the chosen operator,
                    else if (i == 5) { if (querys[6] != "") { x += "boolean"; } else { found = 1; } }
                    //we pass to the URI parameter the word "boolean" with the chosen operator,
                    else if (i == 7) { if ((querys[8] != "") || (querys[9] != "")) { x += "boolean"; } else { found = 1; } }
                    //we pass to the URI parameter the word "boolean" with the chosen operator,
                    else if (i == 10) { if (querys[11] != "") { x += "boolean"; } else { found = 1; } }


                    if (found != 1)
                    {
                        x += "¬";
                        x = string.Concat(x, querys[i]);
                        x += " ";
                        x += "¬";

                    }

                }

            }


            // The return statement redirects to the Index method of the
            // AzureSearchService controller, sending the keyword query as a URI parameter.
            return RedirectToAction("Index", "AzureSearchService", new RouteValueDictionary(new { queryID = x }));

        }
    }
}