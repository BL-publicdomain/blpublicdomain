using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace blProto.Controllers
{
    public class NamesTest2Controller : Controller
    {
        //
        // GET: /NamesTest2/
        public ActionResult Index()
        {
            return View();
        }



        // The first example of a GET method.
        public IEnumerable<tnames> Get()
        {
            return teamNames;
        }



        // This is a quickly made data-store to test using GET, POST etc.
        static List<tnames> teamNames = InitNames();
        private static List<tnames> InitNames()
        {
            var collection = new List<tnames>();
            collection.Add(new tnames { id = 0, name = "Boss Nektaria" });
            collection.Add(new tnames { id = 1, name = "Designer Wendy" });
            return collection;
        }



        // Below is a class for the name of each team member.
        public class tnames
        {
            public int id;
            public string name;
        }
	}
}