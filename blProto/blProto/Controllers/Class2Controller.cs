using blProto.Controllers;
using Microsoft.WindowsAzure.Storage.Table;
using RazorTesting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RazorTesting.Controllers
{
    public class Class2Controller : Controller
    {
        //
        // GET: /Class2/
        public ActionResult Index(string id)
        {


           

            char[] chars = id.ToCharArray();
            char firstChar = chars[0];
            int choice =  (int)char.GetNumericValue(firstChar);

            string keyword = "";

            for (int i = 1; i < chars.Count(); i++)
            {
                keyword += chars[i];
            }


            ViewBag.s = choice;
            //string keyword = id;

            ViewBag.id = keyword;
           

            CloudTable mainbl;
            CloudTable placebl;
            CloudTable publisherbl;
            CloudTable titlebl;




            //choice equals with zero publisher, choice equals wiht one title, 
            //choice equals with two publication place, 



            List<TableKeys> results = new List<TableKeys>();
            List<PrintEntity> newResults = new List<PrintEntity>();
            var classController = new AzureStorageController();
            classController.connectionDatabase(out mainbl, out placebl, out publisherbl, out titlebl);
            //String keyword = "Griffith";

            //String keyword = "Twilight";

            //String keyword = "Manchester United Liverpool";
            //String keyword =  HttpContext.Request.Url.AbsoluteUri;
            //ViewBag.keyword=keyword;

            //String keyword = "Farran";

            //string newKeyword = "Manchester";
            string newKeyword = (keyword.ToString().ToLower());

            char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}', '"' };
            string[] words = newKeyword.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);


            int itemExistResult = 0;
            //string[] words = (keyword.ToString().ToLower()).Split();
            int counterOfKeyword = words.Count();
            int counter = 0;
            ViewBag.counterOfKeyword = counterOfKeyword;
            int itemExistSorted = 0;

            TableQuery<PlaceEntity> queryPlaces = new TableQuery<PlaceEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, " "));

           // int choice = queryID;
            //int choice = 5;
            var listOfPlaces = titlebl.ExecuteQuery(queryPlaces).ToList();

            if (choice == 4) { listOfPlaces = placebl.ExecuteQuery(queryPlaces).ToList(); }
            else if (choice == 0) { listOfPlaces = titlebl.ExecuteQuery(queryPlaces).ToList(); }
            else if (choice == 1) { listOfPlaces = publisherbl.ExecuteQuery(queryPlaces).ToList(); }





            ///////////////////////////////////////////////////////////////////////////////////////////////
            //
            //      Choice 1-Publisher
            //
            ////////////////////////////////////////////////////////////////////////////////////////////




            if (choice == 1)
            {




                foreach (var v in listOfPlaces)
                {

                    int itemExist = 0;

                    int counterAppearanceWord = 0;
                    foreach (string word in words)
                    {

                        string tempPartionkey = ((v.PartitionKey.ToString()).ToLower());

                        string[] temorarylist = tempPartionkey.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);



                        foreach (string tem in temorarylist)
                        {

                            if (tem.Equals(word))
                            {
                                itemExist = 1;
                                counterAppearanceWord++;

                            }
                        }


                    }


                    if (itemExist == 1)
                    {

                        results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterAppearanceWord });

                    }



                }


                ViewBag.results = results;





                List<TableKeys> results2 = new List<TableKeys>();




                for (int i = 0; i < results.Count(); i++)
                {


                    if (results2.Count() == 0)
                    {
                        results2.Add(new TableKeys()
                        {
                            PartitionKey = results[i].PartitionKey,
                            RowKey = results[i].RowKey,
                            Counter = results[i].Counter
                        });
                    }

                    else
                    {

                        for (int ii = 0; ii < results2.Count(); ii++)
                        {



                            if (results[i].PartitionKey.Contains(results2[ii].PartitionKey)) { }
                            else
                            {
                                results2.Add(new TableKeys() { PartitionKey = results[i].PartitionKey, RowKey = results[i].RowKey, Counter = results[i].Counter });

                            }
                        }
                    }

                }











                var sortedList = from p in results2

                                 orderby p.Counter descending
                                 select p;



                var sortedList2 = from p in results2
                                  where p.Counter == words.Count()
                                  orderby p.Counter descending
                                  select p;



                foreach (var v in sortedList2)
                {

                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                        string title = vvv.title;
                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;

                        newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = v.RowKey, FlickrUrl = flickrUrl, PartitionKey= v.PartitionKey });
                    }


                }






                foreach (var v in sortedList)
                {
                    itemExistSorted = 0;
                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                        string title = vvv.title;
                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;



                        for (int i = 0; i < newResults.Count(); i++)
                        {
                            if (newResults[i].RowKey.Equals(v.RowKey))
                            {
                                itemExistSorted = 1;
                            }
                        }




                        if (itemExistSorted == 0)
                        {
                            newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = v.RowKey, FlickrUrl = flickrUrl, PartitionKey=v.PartitionKey });
                        }
                    }


                }





                ViewBag.resultsChoice0 = newResults;


            }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            //
            //      Choice0-title
            //
            ////////////////////////////////////////////////////////////////////////////////////////////







            if (choice == 0)
            {

                foreach (var v in listOfPlaces)
                {

                    int itemExist = 0;

                    int counterAppearanceWord = 0;
                    foreach (string word in words)
                    {

                        string tempPartionkey = ((v.PartitionKey.ToString()).ToLower());

                        string[] temorarylist = tempPartionkey.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);



                        foreach (string tem in temorarylist)
                        {

                            if (tem.Equals(word))
                            {
                                itemExist = 1;
                                counterAppearanceWord++;

                            }
                        }


                    }


                    if (itemExist == 1)
                    {

                        results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterAppearanceWord });

                    }



                }




                ViewBag.nektaria = results;


                List<TableKeys> results2 = new List<TableKeys>();




                for (int i = 0; i < results.Count(); i++)
                {


                    if (results2.Count() == 0)
                    {
                        results2.Add(new TableKeys()
                        {
                            PartitionKey = results[i].PartitionKey,
                            RowKey = results[i].RowKey,
                            Counter = results[i].Counter
                        });
                    }

                    else
                    {

                        for (int ii = 0; ii < results2.Count(); ii++)
                        {



                            if (results[i].PartitionKey == (results2[ii].PartitionKey)) { }
                            else
                            {
                                results2.Add(new TableKeys() { PartitionKey = results[i].PartitionKey, RowKey = results[i].RowKey, Counter = results[i].Counter });

                            }
                        }
                    }

                }


                ViewBag.stelios = results2;

                var sortedList = from p in results2
                                 orderby p.Counter descending
                                 select p;



                //var sortedList2 = from p in sortedList
                //                  where p.Counter == words.Count()
                //                  orderby p.Counter descending
                //                  select p;



                foreach (var v in sortedList)
                {

                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                                                string title = vvv.title;
                        string[] tempTitle = title.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        int counterTitle = tempTitle.Count();


                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;
                        if (counterTitle == words.Count())
                        {
                            newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = v.RowKey, FlickrUrl = flickrUrl,PartitionKey=v.PartitionKey });
                        }
                        
                    }


                }







                foreach (var n in sortedList)
                {
                    itemExistSorted = 0;
                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, n.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, n.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                        string title = vvv.title;
                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;



                        for (int i = 0; i < newResults.Count(); i++)
                        {
                            if (newResults[i].PartitionKey.Equals(n.PartitionKey))
                            {
                                itemExistSorted = 1;
                            }
                        }




                        if (itemExistSorted == 0)
                        {
                            newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = n.RowKey, FlickrUrl = flickrUrl, PartitionKey=n.PartitionKey });
                        }
                    }


                }







               // ViewBag.stefan = newResults;
              // ViewBag.newResults = newResults;
                ViewBag.resultsChoice0 = newResults;


            }




            ///////////////////////////////////////////////////////////////////////////////////////////////
            //
            //      Choice4-Place
            //
            ////////////////////////////////////////////////////////////////////////////////////////////

            if (choice == 4)
            {


                foreach (var v in listOfPlaces)
                {

                    int itemExist = 0;

                    int counterAppearanceWord = 0;
                    foreach (string word in words)
                    {

                        string tempPartionkey = ((v.PartitionKey.ToString()).ToLower());

                        string[] temorarylist = tempPartionkey.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);



                        foreach (string tem in temorarylist)
                        {

                            if (tem.Equals(word))
                            {
                                itemExist = 1;
                                counterAppearanceWord++;

                            }
                        }


                    }


                    if (itemExist == 1)
                    {

                        results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterAppearanceWord });

                    }



                }


                ViewBag.results = results;









                var sortedList = from p in results

                                 orderby p.Counter descending
                                 select p;



                var sortedList2 = from p in results
                                  where p.Counter == words.Count()
                                  orderby p.Counter descending
                                  select p;



                foreach (var v in sortedList2)
                {

                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                        string title = vvv.title;
                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;

                        newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = v.RowKey, FlickrUrl = flickrUrl, PartitionKey=v.PartitionKey });
                    }


                }






                foreach (var v in sortedList)
                {
                    itemExistSorted = 0;
                    string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                    string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                    TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                    var finallist = mainbl.ExecuteQuery(finalquery).ToList();


                    foreach (var vvv in finallist)
                    {
                        string title = vvv.title;
                        string publisher = vvv.publisher;
                        string place = vvv.place;
                        string flickrUrl = vvv.flickr_original_jpeg;



                        for (int i = 0; i < newResults.Count(); i++)
                        {
                            if (newResults[i].RowKey.Equals(v.RowKey))
                            {
                                itemExistSorted = 1;
                            }
                        }




                        if (itemExistSorted == 0)
                        {
                            newResults.Add(new PrintEntity() { Title = title, Publisher = publisher, Place = place, RowKey = v.RowKey, FlickrUrl = flickrUrl, PartitionKey=v.PartitionKey });
                        }
                    }


                }


                ViewBag.resultsChoice0 = newResults;


            }








            return View();
        }
    }
}