using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace blProto.Controllers
{
    public class NamesTestController : ApiController
    {
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
    }
    


    // Below is a class for the name of each team member.
    public class tnames
    {
        public int id;
        public string name;
    }

}
