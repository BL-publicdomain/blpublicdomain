using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blProto.Controllers
{
    public class ItemPageImageController : Controller
    {
        //
        // GET: /ItemPageImage/
        public ActionResult ItemImage(String title, String image)
        {
            ViewBag.itemTitle = title;
            ViewBag.image = image;
            return View();
        }
	}
}