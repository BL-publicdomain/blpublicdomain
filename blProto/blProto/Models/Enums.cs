using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blProto.Models
{
    public class Enums
    {

        
        public enum SearchFields { title, author, publisher, location, fromDate, toDate, ocr, boolean, home }

        public enum AzureQueryFields { title, creators_and_contributors, publisher, place, date}


        
        public enum booleanOperators{and, or, not}
   

    }
}