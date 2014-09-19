using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blProto.Controllers
{
    public class ResultController : Controller
    {
        //
        // GET: /Result/
        public ActionResult Index()
        {

            //string url = Request.Url.Query;


            //ViewBag.url = url;

            //int count=url.Length;

            ////ViewBag.count = count;


            return View();
        }
	}
}