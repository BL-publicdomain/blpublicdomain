using Newtonsoft.Json.Linq;
using RazorTesting.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Ionic.Zip;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Web.UI;
using blProto.Models;

namespace blProto.Controllers
{
    public class AzureSearchServiceController : Controller
    {
        string userInput;
        List<BookModel> resultingList = new List<BookModel>();
        int bookListTotal = 0;

        private const string ApiVersion = "api-version=2014-03-17-Preview";
        private static string serviceUrl = ConfigurationManager.AppSettings["serviceUrl"] ?? "";
        private static string indexName = ConfigurationManager.AppSettings["indexName"] ?? "";

        // Admin key should be in config or some other secure location
        private static string primaryKey = ConfigurationManager.AppSettings["primaryKey"] ?? "";
        int maximumNumberOfJsonEntries = 1000;
        

        //The CreateIndex function creates the Search Service and its indexes.
        //Also loads the schema.
        static bool CreateIndex()
        {
            // Add some data to the newly created index
            Uri requestUri = new Uri(serviceUrl + indexName + "?" + ApiVersion);

            // Load the json containing the schema from an external file
            //string json = System.IO.File.ReadAllText("schema.json");
            string json = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Data/schema.json"));
            using (HttpClient client = new HttpClient())
            {
                // Create the index
                client.DefaultRequestHeaders.Add("api-key", primaryKey);
                HttpResponseMessage response = client.PutAsync(requestUri,        // To create index use PUT
                    new StringContent(json, Encoding.UTF8, "application/json")).Result;

                if (response.StatusCode == HttpStatusCode.NoContent)   // For Posts we want to know response had no content
                {
                    //Console.WriteLine("Index created. \n");
                    return true;
                }

                // Console.WriteLine("Index creation failed: {0} {1} \n", (int)response.StatusCode, response.Content.ReadAsStringAsync().Result.ToString());
                return false;

            }
        }



        //This function uploads the data to the index 
        //documents are POSTed to the newly created index
        static bool PostDocuments(string fileName)
        {
            bool succeeded = false;

            // Add some documents to the newly created index
            Uri requestUri = new Uri(serviceUrl + indexName + "/index?" + ApiVersion);

            // Load the json containing the data from an external file
            // string json = System.IO.File.ReadAllText(fileName);
            string json = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Data/" + fileName));

            using (HttpClient client = new HttpClient())
            {
                // Create the index
                client.DefaultRequestHeaders.Add("api-key", primaryKey);
                HttpResponseMessage response = client.PostAsync(requestUri,       // To add data use POST
                    new StringContent(json, Encoding.UTF8, "application/json")).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic r = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    IEnumerable<dynamic> items = r.items;
                    if (items.Any(i => !(bool)i.status))
                    {
                        //Console.WriteLine("The documents with the following keys failed to upload: " +
                        //string.Join(",", items.Where(i => !(bool)i.status).Select(i => (string)i.key)));
                    }
                    else
                    {
                        //Console.WriteLine("Documents successfully loaded");
                        succeeded = true;
                    }
                }
                else
                {
                    // Console.WriteLine("Documents failed to upload: {0} {1} \r\n", (int)response.StatusCode, response.Content.ReadAsStringAsync().Result.ToString());
                }
            }

            return succeeded;
        }

        static JObject ExecuteRequest(string action, ref List<BookModel> bookListResponse, string query = "")
        {


            string url = serviceUrl + indexName + "/" + action + "?" + ApiVersion;

            if (!String.IsNullOrEmpty(query))
            {
                url += "&" + query;
            }




            string response = ExecuteGetRequest(url);

            BooksCollection bookCollection = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<BooksCollection>(response);




            foreach (var item in bookCollection.documents)
            {
                //adding the fields of an item to the bookListResponse List
                bookListResponse.Add(new BookModel()
                {
                    key = item.key,
                    score = item.score,
                    title = item.data.title,
                    creators_and_contributors = item.data.creators_and_contributors,
                    flickr_original_jpeg = item.data.flickr_original_jpeg,
                    ocrtext = item.data.ocrtext,
                    idx = item.data.idx,
                    printsysnum = item.data.printsysnum,
                    vol = item.data.vol,
                    scannumber = item.data.scannumber,
                    height = item.data.height,
                    width = item.data.width,
                    fromshelfmark = item.data.fromshelfmark,
                    place = item.data.place,
                    sizebracket = item.data.sizebracket,
                    electronicsysnum = item.data.electronicsysnum,
                    date = item.data.date,
                    flickr_url = item.data.flickr_url,
                    azure_url = item.data.azure_url,
                    tags = item.data.tags,
                    publisher = item.data.publisher
                });

            }




            return JObject.Parse(response);
        }

        static string ExecuteGetRequest(string requestUri)
        {
            //This will execute a get request and return the response
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", primaryKey);
                HttpResponseMessage response = client.GetAsync(requestUri).Result;        // Searches are done over port 80 using Get
                return response.Content.ReadAsStringAsync().Result;
            }

        }

        static bool DeleteIndex(string requestUri)
        {
            //This will execute a delete request
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", primaryKey);
                HttpResponseMessage response = client.DeleteAsync(requestUri + "?" + ApiVersion).Result;
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    //Console.WriteLine("Index Deleted. \n");
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    //Console.WriteLine("Could not find existing index, continuing... \n");
                    return true;
                }

                //Console.WriteLine("Index Deletion Failed: : {0} {1} \n", (int)response.StatusCode, response.Content.ReadAsStringAsync().Result.ToString());
                return false;
            }

        }

        public class BooksCollection
        {

            public List<KeyValue> documents { get; set; }
        }

        public class KeyValue
        {

            public double score { get; set; }
            public int key { get; set; }
            public Book data { get; set; }


        }



       //Book class has all the fields from the JSON file
        /// </summary>
        public class Book
        {
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

        }


        public void setUserInput(string input) { userInput = input; }
        public string getUserInput() { return userInput; }



        //initialiseListsFromAzureSearch function is is responsible to execute the queries, retrieve and store their results into lists 
        public void initialiseListsFromAzureSearch(string queryID, out string[] keywordOfFields, out List<int> booleanSelection, out List<List<BookModel>> listOfQueries,ref string toPrintQuerySelection)
        {
            

            //keywordOfFields is a 9-element array, 
            //the position 0 is the keywordTitle, the position 1 is the keywordContributorsandContibutions, the position 2 is the keywordPublisher
            //the position 3 is the keywordLocation, the position 4 is the keywordFromDate, the position 5 is the keywordToDate 
            //the keyword 6 is the keywordOCR, the position 7 is the keywordHome
            keywordOfFields = new string[8];
            booleanSelection = new List<int>();
            JObject response;



            char[] delimiters = new char[] { '¬' };
            string[] words = queryID.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            string tempAND = "and", tempOR = "or", tempNOT = "not";

            listOfQueries = new List<List<BookModel>>();
            //bookListTitle to store all the results from the field title
            List<BookModel> bookListTitle = new List<BookModel>();
            //bookListCreatorsAndContributors to store all the results from the field creators_and_contributors
            List<BookModel> bookListCreatorsAndContributors = new List<BookModel>();
            //bookListFromDate to store all the results from a specific date and after
            List<BookModel> bookListFromDate = new List<BookModel>();
            //bookListToDate to store all the results from a specific year and before
            List<BookModel> bookListToDate = new List<BookModel>();
            //bookListPublisher to store all the results for the field publisher
            List<BookModel> bookListPublisher = new List<BookModel>();
            //bookListLocation to store all the results for the field place
            List<BookModel> bookListLocation = new List<BookModel>();
            //bookListOcr to store all the results about the ocr
            List<BookModel> bookListOcr = new List<BookModel>();
            //bookListFinal is the final list, which stores the final results (after the performing of the booolean operators)
            List<BookModel> bookListFinal = new List<BookModel>();
            List<BookModel> bookListIntersection = new List<BookModel>();
            string searchField = "";
            string keyword = "";
            int maximumNumberOfJson = 1000;
            //check if the word contains number
            int foundNumber = 0;
          
        

            //Identify the selection of the user and execute the suitable queries
            for (int i = 0; i < words.Length; i++)
            {
                if (i % 2 == 0)
                {
                    //Execute the appropriate queries
                    if (words[i] == blProto.Models.Enums.SearchFields.title.ToString()) { searchField = blProto.Models.Enums.AzureQueryFields.title.ToString(); keywordOfFields[0] = words[i + 1]; keyword = keywordOfFields[0]; response = ExecuteRequest("search", ref bookListTitle, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson); booleanSelection.Add(0); toPrintQuerySelection += "Title: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.author.ToString()) { searchField = blProto.Models.Enums.AzureQueryFields.creators_and_contributors.ToString(); keywordOfFields[1] = words[i + 1]; keyword = keywordOfFields[1]; response = ExecuteRequest("search", ref bookListCreatorsAndContributors, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson); toPrintQuerySelection += "Creators and Contributors: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.publisher.ToString()) { searchField =  blProto.Models.Enums.AzureQueryFields.publisher.ToString(); keywordOfFields[2] = words[i + 1]; keyword = keywordOfFields[2]; response = ExecuteRequest("search", ref bookListPublisher, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson); toPrintQuerySelection += "Publisher: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.location.ToString()) { searchField =  blProto.Models.Enums.AzureQueryFields.place.ToString(); keywordOfFields[3] = words[i + 1]; keyword = keywordOfFields[3]; response = ExecuteRequest("search", ref bookListLocation, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson); toPrintQuerySelection += "Location: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.fromDate.ToString()) { searchField =  blProto.Models.Enums.AzureQueryFields.date.ToString(); keywordOfFields[4] = words[i + 1]; toPrintQuerySelection += "From date: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.toDate.ToString()) { searchField =  blProto.Models.Enums.AzureQueryFields.date.ToString(); keywordOfFields[5] = words[i + 1]; toPrintQuerySelection += "To date: \""; toPrintQuerySelection += words[i + 1] + "\""; }
                    else if (words[i] == blProto.Models.Enums.SearchFields.ocr.ToString()) { searchField =  blProto.Models.Enums.AzureQueryFields.title.ToString();  keywordOfFields[6] = words[i + 1]; toPrintQuerySelection += "OCR Keyword: \""; toPrintQuerySelection += words[i + 1] + "\""; response = ExecuteRequest("search", ref bookListOcr, "text=*&$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson); }
                    else if (words[i] == blProto.Models.Enums.SearchFields.boolean.ToString()) { if (words[i + 1].Contains(blProto.Models.Enums.booleanOperators.and.ToString())) { booleanSelection.Add(0); toPrintQuerySelection += " AND "; } else if (words[i + 1].Contains(blProto.Models.Enums.booleanOperators.or.ToString())) { booleanSelection.Add(1); toPrintQuerySelection += " OR "; } else if (words[i + 1].Contains(blProto.Models.Enums.booleanOperators.not.ToString())) { booleanSelection.Add(2); toPrintQuerySelection += " NOT "; } }
                    else if (words[i] == blProto.Models.Enums.SearchFields.home.ToString())
                    {
                        toPrintQuerySelection += "Keyword: \""; toPrintQuerySelection += words[i + 1] + "\"";
                        //keyword = keywordOfFields[0] = keywordOfFields[1] = keywordOfFields[2] = keywordOfFields[3] = keywordOfFields[4]= keywordOfFields[5]=keywordOfFields[7] = words[i + 1];

                        keywordOfFields[0] = words[i + 1]; keywordOfFields[1] = words[i + 1]; keywordOfFields[2] = words[i + 1]; keywordOfFields[3] = words[i + 1]; keywordOfFields[4] = words[i + 1]; keywordOfFields[5] = words[i + 1];
                        //keywordOfFields[7] = words[i + 1];
                        keyword = keywordOfFields[0];

                        searchField = blProto.Models.Enums.AzureQueryFields.title.ToString(); searchField = blProto.Models.Enums.AzureQueryFields.title.ToString(); response = ExecuteRequest("search", ref bookListTitle, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson);
                        searchField = blProto.Models.Enums.AzureQueryFields.creators_and_contributors.ToString(); searchField = blProto.Models.Enums.AzureQueryFields.creators_and_contributors.ToString(); response = ExecuteRequest("search", ref bookListCreatorsAndContributors, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson);
                        searchField = blProto.Models.Enums.AzureQueryFields.publisher.ToString(); searchField = blProto.Models.Enums.AzureQueryFields.publisher.ToString(); response = ExecuteRequest("search", ref bookListPublisher, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson);
                        searchField = blProto.Models.Enums.AzureQueryFields.place.ToString(); searchField = blProto.Models.Enums.AzureQueryFields.place.ToString(); response = ExecuteRequest("search", ref bookListLocation, "text=" + keyword + "$textMode=any&searchFields=" + searchField + "&$skip=0&$top=" + maximumNumberOfJson);
                        searchField = blProto.Models.Enums.AzureQueryFields.date.ToString();  
                   
                        
                        
                        booleanSelection.Add(1); booleanSelection.Add(1); booleanSelection.Add(1); booleanSelection.Add(1); booleanSelection.Add(1);


                       
                    }
                }


            }


            //For the FromDate and toDate we use OData syntax which includes the comparison operators
            if ((!String.IsNullOrEmpty(keywordOfFields[4])) &&(String.IsNullOrEmpty(keywordOfFields[5]))) { keyword = keywordOfFields[4].Trim(); response = ExecuteRequest("search", ref bookListFromDate, "text=*&$filter=" + searchField + " " + "ge" + " " + Int32.Parse(keyword) + "&$skip=0&$top=" + maximumNumberOfJson); }
            else if ((String.IsNullOrEmpty(keywordOfFields[4])) && (!String.IsNullOrEmpty(keywordOfFields[5]))) { keyword = keywordOfFields[5].Trim(); response = ExecuteRequest("search", ref bookListToDate, "text=*&$filter=" + searchField + " " + "le" + " " + Int32.Parse(keyword) + "&$skip=0&$top=" + maximumNumberOfJson);}
            else if ((!String.IsNullOrEmpty(keywordOfFields[4])) && (!String.IsNullOrEmpty(keywordOfFields[5]))) 
            {

                //check whether the user performs a search from the Home Page
                if (queryID.Contains(blProto.Models.Enums.SearchFields.home.ToString())) 
                {

                    char[] delimitersWhiteSpace = new char[] { ' ' };

                    string[] wordsDate = keywordOfFields[4].Split(delimitersWhiteSpace, StringSplitOptions.RemoveEmptyEntries);


                    for (int i = 0; i < wordsDate.Count(); i++)
                    {
                        foundNumber = 0;
                        //Check if the keyword is number or a word
                        Regex expression = new Regex(@"\b\d+\b*");
                        var results = expression.Matches(wordsDate[i].ToString());
                        foreach (Match match in results)
                        {
                            foundNumber = 1;
                        }

                        if (foundNumber == 1)
                        {
                            keyword = wordsDate[i].Trim();
                            string keywordToDate = keyword;
                            response = ExecuteRequest("search", ref bookListFromDate, "text=*&$filter=(" + searchField + " " + "ge" + " " + Int32.Parse(keyword) + " and " + searchField + " le " + Int32.Parse(keywordToDate) + ")" + "&$skip=0&$top=" + maximumNumberOfJson); keywordOfFields[5] = "";
               
                        }

                    }
                    if (foundNumber == 0) { keywordOfFields[4] = null; keywordOfFields[5] = null; booleanSelection.RemoveAt(4); }
                
                
                }
                else
                {
                    keyword = keywordOfFields[4].Trim();
                    string keywordToDate = keywordOfFields[5].Trim();
                    response = ExecuteRequest("search", ref bookListFromDate, "text=*&$filter=(" + searchField + " " + "ge" + " " + Int32.Parse(keyword) + " and " + searchField + " le " + Int32.Parse(keywordToDate) + ")" + "&$skip=0&$top=" + maximumNumberOfJson); keywordOfFields[5] = "";
                }
                    
              
                  


                    
              }


        




            //In order to identify if one word is included in the ocr
            List<BookModel> tempBookListOcr=new List<BookModel>();
            int foundUserInputInOcr=0;
            if (!String.IsNullOrEmpty(keywordOfFields[6])) 
            {

                //for (int j = 0; j < bookListOcr.Count; j++)
                //{

                int value = maximumNumberOfJsonEntries/2;
                for (int j = 0; j < value; j++)
                {



                    foundUserInputInOcr = 0;
                    string filename = bookListOcr[j].ocrtext;
                    char[] delimiterWhiteSpace = new char[] { ' ' };
                    string[] userInputForOCR = keywordOfFields[6].Split(delimiterWhiteSpace, StringSplitOptions.RemoveEmptyEntries);
                    int foundOfWord = 0;
                    for (int i = 0; i < userInputForOCR.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(filename))
                        {
                            frequentWordInOCR(filename, userInputForOCR[i], out foundOfWord);
                        }

                        if (foundOfWord != 0) { foundUserInputInOcr = 1; }

                    }
                    if (foundUserInputInOcr != 0) { tempBookListOcr.Add(bookListOcr[j]); }

                }


            
            }

            bookListOcr = tempBookListOcr.ToList();
            
            //adding the bookListTitle to the listOFQueries
            listOfQueries.Add(bookListTitle);
            //adding the bookListCreatorsAndContributors to listOfQueries
            listOfQueries.Add(bookListCreatorsAndContributors);
            //adding the bookListPublisher to the listOfQueries
            listOfQueries.Add(bookListPublisher);
            //adding the bookListLocation to the ListOfQueries
            listOfQueries.Add(bookListLocation);
            //adding the bookListFromDate to the ListOfQueries
            listOfQueries.Add(bookListFromDate);
            //adding the bookListToDate to the ListOfQueries
            listOfQueries.Add(bookListToDate);
            //adding the bookListOcr to the ListOfQueries
            listOfQueries.Add(bookListOcr);
            listOfQueries.Add(bookListOcr);


        }




        //booleanFunctionality, to perform the boolean operators between the fields
        public void booleanFunctionality(string queryID, List<int> booleanSelection, string[] keywordOfFields, out int position, List<List<BookModel>> listOfQueries, ref IEnumerable<BookModel> finalResults, ref List<BookModel> finalList)
        {

            //We execute three different loops; one for each boolean operator

            position = 0;
            int j = 0; //Go through the keywordOfFields array
            //////////////////////////////////////////////AND/////////////////////////////////////////////////////////////
            position = 0;
            j = 0;
            for (int i = 0; i < booleanSelection.Count(); i++)
            {
                if (booleanSelection[i] == 0)
                {

                    int counter = 0;
                    for (j = 0; j < keywordOfFields.Length; j++)
                    {

                        if (!String.IsNullOrEmpty(keywordOfFields[j]))
                        {
                            counter++;
                            if (counter == i + 1) { break; }
                        }


                    }

                    position = j;
                    //insertANDElemensToFinalList to identify all the items that are common in every list 
                    insertANDElemensToFinalList(position, listOfQueries, ref finalResults, ref finalList);



                }

            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////OR/////////////////////////////////////////////////////////////////////////


            //We execute another loop for the boolean operator "or" in order to combine all the items from lists where the user has defined the boolean operator to be "or". 

            position = 0;
            j = 0;
            for (int i = 0; i < booleanSelection.Count(); i++)
            {
                if (booleanSelection[i] == 1)
                {

                    int counter = 0;
                    for (j = 0; j < keywordOfFields.Length; j++)
                    {

                        if (!String.IsNullOrEmpty(keywordOfFields[j]))
                        {
                            counter++;
                            if (counter == i + 1) { break; }
                        }


                    }

                    position = j;
                    if (position != 0)
                    {
                        insertORElemensToFinalList(position, listOfQueries, ref finalResults, ref finalList);

                    }
                    else if (position == 0 && queryID.Contains(blProto.Models.Enums.SearchFields.home.ToString())) 
                    {

                        //We call the fucntion insertORElemensToFinalList to combine all the items from lists where the user has defined the boolean operator to be "or". 

                        insertORElemensToFinalList(position, listOfQueries, ref finalResults, ref finalList);
                    }


                }

            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //execute another loop for the boolean operator "not" in order to exclude from the final results some items 
            position = 0;
            j = 0;
            for (int i = 0; i < booleanSelection.Count(); i++)
            {
                if (booleanSelection[i] == 2)
                {

                    int counter = 0;
                    for (j = 0; j < keywordOfFields.Length; j++)
                    {

                        if (!String.IsNullOrEmpty(keywordOfFields[j]))
                        {
                            counter++;
                            if (counter == i + 1) { break; }
                        }


                    }

                    position = j;
                    if (position != 0)
                    {

                        //call the insertNOTElementToFinalLis to exclude from the final results some items 
                        insertNOTElemensToFinalList(position, listOfQueries, ref finalResults, ref finalList);

                    }

                }

            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////










        }



        // insertNOTElemensToFinalList, in order to exclude from the final results some items that appear in a specific field and have as value the keyword search input.
        public void insertNOTElemensToFinalList(int position, List<List<BookModel>> listOfQueries, ref IEnumerable<BookModel> finalResults, ref List<BookModel> finalList)
        {


            var resultsOfPosition = listOfQueries[position].Select(item => item.title);
            var listSecondPosition = listOfQueries[position];
            var unionOfBooks = listSecondPosition;
            listOfQueries.RemoveAt(7);
            listOfQueries.Add(unionOfBooks.ToList());
            var results = listOfQueries[7].Where(item => resultsOfPosition.Contains(item.title));
            results = results.Distinct();
            var unionWithFinalResults = finalResults.Except(results).ToList();
            finalResults = unionWithFinalResults;



        }




        //insertANDElemensToFinalList to identify all the items that are common in every list 
        public void insertANDElemensToFinalList(int position, List<List<BookModel>> listOfQueries, ref IEnumerable<BookModel> finalResults, ref List<BookModel> finalList)
        {

            //We identify the common items that appear in the Lists by performing a linq command that distinguishes based on the title.
            if (finalResults == null || !finalResults.Any())
            {

                var resultsOfPosition = listOfQueries[position].Select(item => item.title);
                resultsOfPosition = resultsOfPosition.Distinct();
                var listOfPosition = listOfQueries[position];
                listOfQueries.RemoveAt(7);
                listOfQueries.Add(listOfPosition.ToList());
                var results = listOfQueries[position].Where(item => resultsOfPosition.Contains(item.title));
                results = results.Distinct();
                finalResults = results;



            }
            else
            {
                var resultsOfPosition = listOfQueries[position].Select(item => item.key);
                var listOfPosition = listOfQueries[position];
                foreach (var item in resultsOfPosition)
                {
                    Debug.WriteLine(item.ToString());

                }
                var results = finalResults.Where(item => resultsOfPosition.Contains(item.key));
                var temporary = (results.Select(item => item.title));
                temporary = temporary.Distinct();
                finalResults = finalResults.Where(item => temporary.Contains(item.title));
            }
            //insertElements(finalResults, ref finalList);
        }



        // insertORElemensToFinalList to combine all the items from lists where the user has defined the boolean operator to be "or". 
        public void insertORElemensToFinalList(int position, List<List<BookModel>> listOfQueries, ref IEnumerable<BookModel> finalResults, ref List<BookModel> finalList)
        {
            var resultsOfPosition = listOfQueries[position].Select(item => item.title);
            var listOfPosition = listOfQueries[position];
            listOfQueries.RemoveAt(7);
            listOfQueries.Add(listOfPosition.ToList());
            var results = listOfQueries[7].Where(item => resultsOfPosition.Contains(item.title));
            results = results.Distinct();
            var unionWithFinalResults = finalResults.Union(results);
            finalResults = unionWithFinalResults;
            //insertElements(finalResults,ref finalList);

        }


        public void insertElements(IEnumerable<BookModel> finalResults, ref List<BookModel> finalList)
        {
            foreach (var item in finalResults)
            {
                finalList.Add(new BookModel()
                {
                    key = item.key,
                    score = item.score,
                    title = item.title,
                    flickr_original_jpeg = item.flickr_original_jpeg,
                    creators_and_contributors = item.creators_and_contributors,
                    ocrtext = item.ocrtext,
                    idx = item.idx,
                    printsysnum = item.printsysnum,
                    vol = item.vol,
                    scannumber = item.scannumber,
                    height = item.height,
                    width = item.width,
                    fromshelfmark = item.fromshelfmark,
                    place = item.place,
                    sizebracket = item.sizebracket,
                    electronicsysnum = item.electronicsysnum,
                    date = item.date,
                    flickr_url = item.flickr_url,
                    azure_url = item.azure_url,
                    tags = item.tags,
                    publisher = item.publisher
                });

            }

        }




        //private bool RemoteFileExists(string url)
        //{
        //    try
        //    {
        //        //Creating the HttpWebRequest
        //        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        //        //Setting the Request method HEAD, you can also use GET too.
        //        request.Method = "HEAD";
        //        //Getting the Web Response.
        //        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        //        //Returns TRUE if the Status code == 200
        //        return (response.StatusCode == HttpStatusCode.OK);
        //    }
        //    catch
        //    {
        //        //Any exception will returns false.
        //        return false;
        //    }
        //}




        // private bool RemoteFileExists(string url)
        //{
          

        //try {
        //    System.Net.WebClient webClient = new System.Net.WebClient();

        //      Stream strm = webClient.OpenRead(url); return true;                                  
        //     }
        //     catch (WebException we) {
        //         throw we; return false;
        //    }
        // }





         public bool RemoteFileExists(string url)
         {


             WebRequest wr = WebRequest.Create(url);
             wr.Method = WebRequestMethods.Http.Head;
             try
             {
                 using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse())
                 {
                     return true;
                         //Label1.Text = (response.StatusCode.ToString());
                 }
             }

             catch (Exception)
             {
                 return false;
                 //Label1.Text = ("Invalid URL");

             }

         }



        public void frequentWordInOCR(String filename, string keyword, out int counter)
        {
            counter = 0;

            keyword = keyword.ToLower();
            System.Net.WebClient wc = new System.Net.WebClient();

            if (!String.IsNullOrEmpty(filename))
            {
                if (RemoteFileExists(filename))
                {
                    byte[] raw = wc.DownloadData(filename);
                    string webData = System.Text.Encoding.UTF8.GetString(raw);
                    var result2 = Regex.Split(webData.ToLower(), @"\W+").Where(s1 => s1.ToString() == keyword); //in order to get the number of appearance
                    counter = result2.Count();
                }
                else
                {
                    counter = 0;
                }
            }
           





        }

        public ActionResult Index(string queryID, int? page)
        {
            ViewBag.queryID = queryID;

            if (!serviceUrl.EndsWith("/"))
            {
                serviceUrl += "/";
            }

            if (!serviceUrl.EndsWith("indexes/"))
            {
                serviceUrl += "indexes/";
            }

            try
            {
                //Delete the Index if it exists
                DeleteIndex(serviceUrl + indexName);

                // Create the index
                if (CreateIndex() == false)
                {
                    ViewBag.Error = "Error From Create Index";
                }


                // Add documents to index - loading in 100 document batches
                if (PostDocuments("data2.json") == false)
                {
                    ViewBag.ErrorPostDoc = "Error from post documents0";
                }



                if (PostDocuments("data3.json") == false)
                {
                    ViewBag.ErrorPostDoc = "Error from post documents0";
                }

                //Wait 5 seconds
                // Console.WriteLine("Waiting 5 seconds for data to be indexed...");
                Thread.Sleep(TimeSpan.FromSeconds(5));

                setUserInput(queryID);


                //Construct of an Object
                AzureSearchServiceController azureSearchService = new AzureSearchServiceController();
                List<int> booleanSelection;
                string[] keywordOfFields; //declare keywordOfFields as string array of any size

                String toPrintQuerySelection = "";
                List<List<BookModel>> listOfQueries;
                azureSearchService.initialiseListsFromAzureSearch(queryID, out keywordOfFields, out booleanSelection, out listOfQueries, ref toPrintQuerySelection);
                ViewBag.searchQuery = toPrintQuerySelection;
                var title = listOfQueries[0].Select(item => item.title);
                var finalResults = listOfQueries[7].Where(item => title.Contains(item.title));
                int position;
                List<BookModel> finalList = new List<BookModel>();
                azureSearchService.booleanFunctionality(queryID,booleanSelection, keywordOfFields, out position, listOfQueries, ref finalResults, ref finalList);
                insertElements(finalResults, ref finalList);
                finalList = finalList.GroupBy(x => x.title).Select(x => x.First()).ToList();


                List<BookModel> bookListIntersection = new List<BookModel>();
                bookListIntersection = finalList.ToList();

                /*
                ViewBag.list = listOfQueries;
                ViewBag.keywords = keywordOfFields;
                ViewBag.boolean = booleanSelection;
                ViewBag.count = position;
                ViewBag.final = finalList;
                ViewBag.finalCount = finalList.Count;
                */

                //ViewBag.countIntersection = bookListIntersection.Count();
                ViewBag.ListResponse = bookListIntersection;

                resultingList = bookListIntersection.ToList();
                int bookListCount;

                if ((bookListIntersection.Count() != 0))
                    {
                bookListCount = bookListIntersection.Count();
                }
                else
                {
                    bookListCount = 0;
                }



                bookListTotal = bookListCount;
                ViewBag.resultCount = bookListCount;
            }
            catch (Exception e)
            {
                // Console.WriteLine("Unhandled exception caught:");

                while (e != null)
                {
                    //Console.WriteLine("\t{0}", e.Message);
                    e = e.InnerException;
                }

                //Console.WriteLine("\nDid you remember to paste your service URL and API key into App.config?\n");
            }

            // Console.Write("Complete.  Press <enter> to continue: ");
            //var name = Console.ReadLine();

            var pagedResultList = resultingList.ToList();
            int pageResultTotal = pagedResultList.Count();
            ViewBag.resultTotal = pageResultTotal;
            var pageNumber = page ?? 1;
            var onePageOfProducts = pagedResultList.ToPagedList(pageNumber, 15);

            ViewBag.OnePageOfProducts = onePageOfProducts;

            if (page > 1)
            {
                ViewBag.PageNumber = page;
            }
            else
            {
                ViewBag.PageNumber = 1;
            }

            /********************* CODE FOR "DISPLAYING XX - YY ... RESULTS" *************************/

            //amount of pages
            int pageTotal = 0;
            if (bookListTotal != 0)
            {
                pageTotal = (bookListTotal / 15) + 1;

            }
            else
            {
                pageTotal = 1;
            }

            // from and to
            int fromResult = 0;
            int toResult = 0;
            
            
            int lastPageResults = (bookListTotal % 15); //amount of results to display on last page

                //calculating resutlts to and from indexes

            //last page - checked first
            if (pageNumber == pageTotal) //last page
            {
                if (bookListTotal == 0) //if no results
                {
                    fromResult = 0;
                    toResult = 0;
                }
                else
                {
                    fromResult = (bookListTotal - lastPageResults) + 1;
                    toResult = bookListTotal;
                }
            }
            else
            {
                if (pageNumber != 1) //pages 2 - penulti
                {
                    fromResult = (15 * (pageNumber - 1)) + 1;
                    toResult = (15 * pageNumber);
                }
                else //first page
                {
                    fromResult = 1;

                    if (bookListTotal < 15) //when less than 15 results to display
                    {
                        toResult = bookListTotal;
                    }
                    else
                    {
                        toResult = 15;
                    }
                }
            }

            ViewBag.fromResult = fromResult;
            ViewBag.toResult = toResult;

            return View();
        }

        //[HttpPost]
        //public ActionResult Test()
        //{

        //    var selectedValues = Request.Form["mySharedName"];
        //    List<int> ocr = selectedValues.Split(',').Select(int.Parse).ToList();

        //    return View();
            
        //}

        //public ActionResult DownloadAll(IEnumerable<Uri> coursePrices)
        //{

        //    var outputStream = new MemoryStream();
        //    using (var zip = new ZipFile())
        //    {

        //        foreach (Uri uri in coursePrices)
        //        {

        //            System.Net.WebClient wc = new System.Net.WebClient();
        //            Stream s = wc.OpenRead(uri);
        //            zip.AddEntry(uri.AbsolutePath +  ".txt", s);
                   

        //        }


        //        zip.Save(outputStream);

        //    }
        //    Response.ContentType = "application/force-download";
            
        //    outputStream.Position = 0;
        //    return File(outputStream, "application/zip", "all.zip");
            
        //}
        
        
        public ActionResult Download(Uri ocr_url, string azure_url, List<string> creators_and_contributors, string date, string electronicsysnum,
         string flickr_original_jpeg, string flickr_url, string fromshelfmark, string height, string idx, string place, string printsysnum,
            string publisher, string scannumber, string sizebracket, string tags, string title, string vol, string width)
        {

            if (RemoteFileExists(ocr_url.ToString()))
            {
                if (azure_url == null)
                {
                    azure_url = "No Info";
                }

                if (date == null)
                {
                    date = "No Info";
                }
                if (electronicsysnum == null)
                {
                    electronicsysnum = "No Info";
                }
                if (flickr_original_jpeg == null)
                {
                    flickr_original_jpeg = "No Info";
                }
                if (flickr_url == null)
                {
                    flickr_url = "No Info";
                }
                if (fromshelfmark == null)
                {
                    fromshelfmark = "No Info";
                }
                if (height == null)
                {
                    height = "No Info";
                }
                if (idx == null)
                {
                    idx = "No Info";
                }
                if (place == null)
                {
                    place = "No Info";
                }
                if (printsysnum == null)
                {
                    printsysnum = "No Info";
                }
                if (publisher == null)
                {
                    publisher = "No Info";
                }
                if (scannumber == null)
                {
                    scannumber = "No Info";
                }
                if (sizebracket == null)
                {
                    sizebracket = "No Info";
                }
                if (tags == null)
                {
                    tags = "No Info";
                }
                if (title == null)
                {
                    title = "No Info";
                }
                if (vol == null)
                {
                    vol = "No Info";
                }
                if (width == null)
                {
                    width = "No Info";
                }
                var outputStream = new MemoryStream();
                XDocument doc = new XDocument(new XElement("body",
                                             new XElement("metadata",
                                                 new XElement("azure_url", azure_url),
                                                 new XElement("date", date),
                                                 new XElement("electronicsysnum", electronicsysnum),
                                                 new XElement("flickr_original_jpeg", flickr_original_jpeg),
                                                 new XElement("flickr_url", flickr_url),
                                                 new XElement("fromshelfmark", fromshelfmark),
                                                 new XElement("height", height),
                                                 new XElement("idx", idx),
                                                 new XElement("ocrtext", ocr_url.ToString()),
                                                 new XElement("place", place),
                                                 new XElement("printsysnum", printsysnum),
                                                 new XElement("publisher", publisher),
                                                 new XElement("scannumber", scannumber),
                                                 new XElement("sizebracket", sizebracket),
                                                 new XElement("tags", tags),
                                                 new XElement("title", title),
                                                 new XElement("vol", vol),
                                                 new XElement("width", width))));






                MemoryStream xmlStream = new MemoryStream();
                doc.Save(xmlStream);

                xmlStream.Flush();//Adjust this if you want read your data 
                xmlStream.Position = 0;


                using (var zip = new ZipFile())
                {

                    System.Net.WebClient wc = new System.Net.WebClient();
                    Stream s = wc.OpenRead(ocr_url);

                    zip.AddEntry(date + "_" + printsysnum + ".txt", s);
                    zip.AddEntry(date + "_" + printsysnum + ".xml", xmlStream);


                    zip.Save(outputStream);
                }

                outputStream.Position = 0;
                return File(outputStream, "application/zip", date + "_" + printsysnum + ".zip");

            } else
            {
                

                return RedirectToAction("ErrorPage", "ErrorPage");

            }
           



        }
        public ActionResult SearchByYear(string year)
        {


            
            string yearURL = null;
            Regex expression = new Regex(@"\b\d{4}\b*");
            
            var results = expression.Matches(year);
            foreach (Match match in results)
            {
                yearURL = match.Value;
            }

            string redirection = "boolean¬and¬fromDate¬" + year + "¬toDate¬" + year + "¬";


            return RedirectToAction("Index", "AzureSearchService", new RouteValueDictionary(new { queryID = redirection }));
        }
        public ActionResult SearchByLocation(string location)
        {



           
            string redirection = "boolean¬and¬location¬" + location + "¬";
            

            return RedirectToAction("Index", "AzureSearchService", new RouteValueDictionary(new { queryID = redirection }));
        }
        public ActionResult SearchByPublisher(string publisher)
        {


           

            string redirection = "boolean¬and¬publisher¬" + publisher + "¬";


            return RedirectToAction("Index", "AzureSearchService", new RouteValueDictionary(new { queryID = redirection }));
        }
    }
}