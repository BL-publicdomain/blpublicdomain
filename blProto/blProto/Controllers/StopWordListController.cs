using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blProto.Controllers
{
    public class StopWordListController : Controller
    {
        //
        // GET: /StopWordList/
        public ActionResult StopWordList()
        {
            string[] fileContents = (System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/stoplist.txt"))).Split('\n');

            ViewBag.stopList = fileContents;

            return View();
        }
	}
}