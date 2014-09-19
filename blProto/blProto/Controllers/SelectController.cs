using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace blProto.Controllers
{
    public class SelectController : Controller
    {
        //
        // GET: /Select/

     
        public ActionResult Index()
        {

            List<SelectListItem> dropdownItems = new List<SelectListItem>();
            List<SelectListItem> dropdownItems2 = new List<SelectListItem>();



            dropdownItems.AddRange(new[]{
                            new SelectListItem() { Text = "Option One", Value = "1" },
                            new SelectListItem() { Text = "Option Two", Value = "2" },
                            new SelectListItem() { Text = "Option Three", Value = "3" }});
            ViewData.Add("DropDownItems", dropdownItems);






            dropdownItems2.AddRange(new[]{
                            new SelectListItem() { Text = "Option One", Value = "1" },
                            new SelectListItem() { Text = "Option Two", Value = "2" },
                            new SelectListItem() { Text = "Option Three", Value = "3" }});
            ViewData.Add("DropDownItems2", dropdownItems);
        

            //return RedirectToAction("Index", "Result", new RouteValueDictionary(new { queryID = x }));

            return View();
        }










	}
}