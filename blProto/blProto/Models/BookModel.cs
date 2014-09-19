using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RazorTesting.Models
{
    public class BookModel
    {
        
       
        public int key { get; set; }
        public Double score { get; set; }
        //the azure_url of an item
        public string azure_url { get; set; }
        //the field creators_and_contributors contains the creator and the contributor of each item
        public List<string> creators_and_contributors { get; set; }
        //the date field which provide information about the date which the book was published
        public Int32 date { get; set; }
        //the electronic version for a specific book
        public Int32 electronicsysnum { get; set; }
        // flickr_original_jpeg, mentioned the image sourse on Flickr
        public string flickr_original_jpeg { get; set; }
        //flickr_url, mentioned the image source in Flickr
        public string flickr_url { get; set; }
        public string fromshelfmark { get; set; }
        public Int32 height { get; set; }
        //idx- describes the index of the image on that page
        public Int32 idx { get; set; }
        //ocrtext mentioned the entire text of a volume 
        public string ocrtext { get; set; }
        //place, represent the place where a specific book was publishing or manufacture
        public string place { get; set; }
        //printsysnum- a book system number ffor the printer version
        public Int32 printsysnum { get; set; }
        //publisher field that mentioned the publisher of the item.
        public string publisher { get; set; }
        public Int32 scannumber { get; set; }
        public string sizebracket { get; set; }
        //the tags field that includes keywords for search optimisation
        public List<string> tags { get; set; }
        public string title { get; set; }
        //vol that described the volume  the image was taken
        public Int32 vol { get; set; }
        //width of the scan image
        public Int32 width { get; set; }
        


        //The fucntions GetHashCode and Equals(Object obj) overriden 
        //in order to be able to able to compare two BookModel objects effectively
        //The key is unique and therefore this property was used to compare objects.
        public override int GetHashCode()
        {
            return key;
        }

        //The Equals (Object obj) overriden to compare two BookModel objects effectively
        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is BookModel))
                return false;
            else
                return key == ((BookModel)obj).key;
        }

    }

}