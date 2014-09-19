using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using RazorTesting.Models;
using System.Text.RegularExpressions;
using System.IO;

namespace RazorTesting.Controllers
{
    public class AzureStorageController : Controller
    {
        //
        // GET: /Class/
        public ActionResult Index()
        {
            CloudTable mainbl;
            CloudTable placebl;
            CloudTable publisherbl;
            CloudTable titlebl;

            List<TableKeys> results = new List<TableKeys>(); 

            var classController = new AzureStorageController();

            classController.connectionDatabase(out mainbl, out placebl, out publisherbl, out titlebl);

         
            String keyword = "Manchester";


            queryForPlaceSimple(placebl, ref results, keyword);

            searchMainTable(mainbl, ref results, keyword);

            return View();
        }



        public void connectionDatabase(out CloudTable mainbl, out CloudTable placebl, out CloudTable publisherbl, out CloudTable titlebl)
        {

            string accountName = "azurestore2";
            string accountKey = "wUZiDY+c+OnH58amw1X8TXRtH2t8cDWljp06B02NozvJqP5WV2MC/LtjZx+NZCCh4aTxFGtVZDgWZEFqZ8aIVw==";

            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            CloudTableClient client = account.CreateCloudTableClient();


            connectionTables(client, out mainbl, out placebl, out publisherbl, out titlebl);


        }



        public void connectionTables(CloudTableClient client, out CloudTable mainbl, out  CloudTable placebl, out CloudTable publisherbl, out CloudTable titlebl)
        {

            mainbl = client.GetTableReference("mainbl");
            // mainbl.CreateIfNotExists();

            placebl = client.GetTableReference("placebl");
            //placebl.CreateIfNotExists();

            publisherbl = client.GetTableReference("publisherbl");
            //placebl.CreateIfNotExists();

            titlebl = client.GetTableReference("titlebl");
            //placebl.CreateIfNotExists();

            
        }



        public void queryForPlaceSimple(CloudTable placebl, ref List<TableKeys> results, string keyword)
        {

            //split user-query into array
            string[] words = keyword.Split();
            int counterOfKeyword = words.Count();



            TableQuery<PlaceEntity> queryPlaces =
       new TableQuery<PlaceEntity>()
          .Where(TableQuery.GenerateFilterCondition("PartitionKey",
              QueryComparisons.GreaterThanOrEqual, " "));

            var listOfPlaces = placebl.ExecuteQuery(queryPlaces).ToList();


            //Console.WriteLine("Lista {0}", listOfPlaces.Count());



            foreach (var v in listOfPlaces)
            {
                //Console.WriteLine("the partition key is {0}", v.newPartitionKey);


                foreach (string word in words)
                {

                    int wordAdded = 0;
                    int itemExist = 0;

                    string tem = (v.PartitionKey.ToString()).ToLower();




                    if (tem.Equals(keyword))
                    {
                        itemExist = 0;
                        // Console.WriteLine("The list contains {0} ",results.Count());

                        for (int i = 0; i < results.Count(); i++)
                        {
                            if (results[i].RowKey.Equals(v.RowKey))
                            {
                                //Console.WriteLine("The item already exists");
                                itemExist = 1;
                            }
                        }

                        if (itemExist == 0)
                        {
                            results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterOfKeyword });
                            //Console.WriteLine("Insert of item");
                        }

                    }

                }

            }

        }


        public void queryForPlace1(CloudTable placebl, ref List<TableKeys> results, string keyword)
        {


            ////split user-query into array
            //string[] words = keyword.Split();
            //int counterOfKeyword = words.Count();



            int counterOfKeyword = 0;
            string[] words = keyword.Split();




            TableQuery<PlaceEntity> queryPlaces =
       new TableQuery<PlaceEntity>()
          .Where(TableQuery.GenerateFilterCondition("PartitionKey",
              QueryComparisons.GreaterThanOrEqual, " "));

            var listOfPlaces = placebl.ExecuteQuery(queryPlaces).ToList();


            //Console.WriteLine("Lista {0}", listOfPlaces.Count());



            foreach (var v in listOfPlaces)
            {
                //Console.WriteLine("the partition key is {0}", v.newPartitionKey);

                int itemExist = 0;
                counterOfKeyword = 0;


                foreach (string word in words)
                {

                    int wordAdded = 0;



                    string temp = (v.PartitionKey.ToString()).ToLower();
                    string[] temporarylist = temp.Split();

                    foreach (string tem in temporarylist)
                    {


                        if (tem.Equals(word))
                        {
                            counterOfKeyword++;
                            // itemExist = 0;
                            // Console.WriteLine("The list contains {0} ",results.Count());

                            for (int i = 0; i < results.Count(); i++)
                            {
                                if (results[i].RowKey.Equals(v.RowKey))
                                {
                                    //Console.WriteLine("The item already exists");
                                    itemExist = 1;
                                }
                            }
                        }

                    }


                }

                if (itemExist == 0)
                {
                    results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterOfKeyword });
                    //Console.WriteLine("Insert of item");
                }

            }

        }


        public void queryForTitle1(CloudTable placebl, ref List<TableKeys> results, string keyword)
        {
            char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}', '"' };


            ////split user-query into array
            //string[] words = keyword.Split();
            //int counterOfKeyword = words.Count();



            int counterOfKeyword = 0;
            //string[] words = keyword.Split();

            string[] words = keyword.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);


            TableQuery<PlaceEntity> queryPlaces =
       new TableQuery<PlaceEntity>()
          .Where(TableQuery.GenerateFilterCondition("PartitionKey",
              QueryComparisons.GreaterThanOrEqual, " "));

            var listOfPlaces = placebl.ExecuteQuery(queryPlaces).ToList();


            //Console.WriteLine("{0}",listOfPlaces.Count());


            //Console.WriteLine("Lista {0}", listOfPlaces.Count());


            int counter = 0;



            foreach (var v in listOfPlaces)
            {
                //counter++;
                //Console.Write("{0}  ", counter);

                int itemExist = 0;
                counterOfKeyword = 0;


                foreach (string word in words)
                {

                    //Console.WriteLine("We examine the word {0}", word);


                    int wordAdded = 0;


                    //Console.WriteLine(text);
                    //char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}' , '"'};
                    //string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);



                    string temp = (v.PartitionKey.ToString()).ToLower();
                    string[] temporarylist = temp.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string tem in temporarylist)
                    {
                        //Console.Write("the partition key is {0}",tem);

                        if (tem.Equals(word))
                        {



                            counterOfKeyword++;
                            // itemExist = 0;
                            // Console.WriteLine("The list contains {0} ",results.Count());

                            for (int i = 0; i < results.Count(); i++)
                            {
                                if (results[i].RowKey.Equals(v.RowKey))
                                {
                                    //Console.WriteLine("The item already exists");
                                    itemExist = 1;
                                }
                            }
                        }

                    }

                }



                if (itemExist == 0)
                {
                    results.Add(new TableKeys() { PartitionKey = v.newPartitionKey, RowKey = v.RowKey, Counter = counterOfKeyword });
                    //Console.WriteLine("Insert of item");
                }

            }

        }


        public void queryForOCR1(CloudTable placebl, ref List<TableKeys> results, string keyword)
        {
            char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}', '"' };


            //frequency of word in OCR
            int counterOCR = 0;




            TableQuery<BookEntity> queryPlaces =
       new TableQuery<BookEntity>()
          .Where(TableQuery.GenerateFilterCondition("PartitionKey",
              QueryComparisons.GreaterThanOrEqual, " ")).Take(10);

            var listOfPlaces = placebl.ExecuteQuery(queryPlaces).ToList();

            //Console.WriteLine("{0}",listOfPlaces.Count());


            string ocr = "";
            int itemExist = 0;

            foreach (var v in listOfPlaces)
            {


                ocr = v.ocrtext;
                //Console.WriteLine("{0}", ocr);
                itemExist = 0;


                if (ocr != "")
                {


                    //Console.WriteLine("Found");






                    frequentWordInOCR(ocr, keyword, out counterOCR);
                    if (counterOCR != 0)
                    {

                        for (int i = 0; i < results.Count(); i++)
                        {
                            //Console.Write("{0} ", results[i].RowKey);
                            //Console.WriteLine("{0} ", v.RowKey);

                            string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, results[i].PartitionKey);
                            string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, results[i].RowKey);

                            TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                            var finallist = placebl.ExecuteQuery(finalquery).ToList();

                            foreach (var vv in finallist)
                            {

                                bool resultEqual = vv.ocrtext.Equals(v.ocrtext);

                                if (resultEqual)
                                {

                                    //Console.WriteLine("The item already exists");
                                    itemExist = 1;
                                }

                            }

                        }

                    }


                    if (itemExist == 0 && counterOCR != 0)
                    {
                        results.Add(new TableKeys() { PartitionKey = v.PartitionKey, RowKey = v.RowKey, Counter = counterOCR });
                        //Console.WriteLine("Insert of item");
                    }

                }

            }

        }


        public void searchMainTable(CloudTable mainbl, ref List<TableKeys> results, string keyword)
        {


            int rank = 1;

            // string[] words = keyword.Split();


            char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}' };
            string[] words = keyword.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);



            int counterOfKeyword = words.Count();

            Console.WriteLine();

            for (int i = counterOfKeyword; i > 0; i--)
            {
                string finalPK = "";
                string finalRK = "";
                foreach (TableKeys entry in results)
                {


                    if (entry.Counter == i)
                    {


                        finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, entry.PartitionKey);
                        finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entry.RowKey);

                        TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                        var finallist = mainbl.ExecuteQuery(finalquery).ToList();

                        //Console.WriteLine("The result in the main {0}", finallist.Count());

                        foreach (var vv in finallist)
                        {
                            Console.WriteLine("{0} {1}", rank, vv.title);
                            Console.WriteLine("by {0} ", vv.publisher);
                            Console.WriteLine(vv.place);
                            Console.WriteLine();
                            rank += 1;


                            //Console.WriteLine("The publisher is {0}", vv.publisher);
                        }

                    }

                }

            }


            Console.WriteLine("Total Results Found {0}", rank - 1);

        }


        public void searchMainTableOCR(CloudTable mainbl, ref List<TableKeys> results, string keyword)
        {

            int rank = 1;

            var sortedList = from p in results
                             orderby p.Counter descending
                             select p;
            //sortedList.ToList().ForEach(p => Console.WriteLine(p.Counter));

            Console.WriteLine("");

            foreach (var v in sortedList)
            {
                

                string finalPK = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, v.PartitionKey);
                string finalRK = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, v.RowKey);

                TableQuery<BookEntity> finalquery = new TableQuery<BookEntity>().Where(TableQuery.CombineFilters(finalPK, TableOperators.And, finalRK));

                var finallist = mainbl.ExecuteQuery(finalquery).ToList();

                foreach (var vv in finallist)
                {
                    Console.WriteLine("{0} {1}", rank, vv.title);
                    Console.WriteLine("by {0} ", vv.publisher);
                    Console.WriteLine(vv.place);
                    Console.WriteLine("The Keyword '{0}' was found {1} times", keyword, v.Counter);
                    Console.WriteLine();
                    rank += 1;
                }

            }

        }


        public void frequentWordInOCR(String filename, string keyword, out int counter)
        {


            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);


            //Console.WriteLine("{0}",webData);


            var result2 = Regex.Split(webData.ToLower(), @"\W+").Where(s1 => s1.ToString() == keyword); //in order to get the number of appearance

            // Console.WriteLine("" + keyword + "(" + result2.Count() + ")");  //for a specific word

            //frequency of a word
            counter = result2.Count();
        }

        public void frequentWords(String filename)
        {
            List<string> stopWord = new List<string>(new string[] { "a", "able", "about", "across", "after", "all", "almost", "also", "am", "among", "an", "and", "any", "are", "as", "at", "be", "because", "been", "but", "by", "can", "cannot", "could", "dear", "did", "do", "does", "either", "else", "ever", "every", "for", "from", "get", "got", "had", "has", "have", "he", "her", "hers", "him", "his", "how", "however", "i", "if", "in", "into", "is", "it", "its", "just", "least", "let", "like", "likely", "may", "me", "might", "most", "must", "my", "neither", "no", "nor", "not", "of", "off", "often", "on", "only", "or", "other", "our", "own", "rather", "said", "say", "says", "she", "should", "since", "so", "some", "than", "that", "the", "their", "them", "then", "there", "these", "they", "this", "tis", "to", "too", "twas", "us", "wants", "was", "we", "were", "what", "when", "where", "which", "while", "who", "whom", "why", "will", "with", "would", "yet", "you", "your", "ain't", "aren't", "can't", "could've", "couldn't", "didn't", "doesn't", "don't", "hasn't", "he'd", "he'll", "he's", "how'd", "how'll", "how's", "i'd", "i'll", "i'm", "i've", "isn't", "it's", "might've", "mightn't", "must've", "mustn't", "shan't", "she'd", "she'll", "she's", "should've", "shouldn't", "that'll", "that's", "there's", "they'd", "they'll", "they're", "they've", "wasn't", "we'd", "we'll", "we're", "weren't", "what'd", "what's", "when'd", "when'll", "when's", "where'd", "where'll", "where's", "who'd", "who'll", "who's", "why'd", "why'll", "why's", "won't", "would've", "wouldn't", "you'd", "you'll", "you're", "you've" });

            using (StreamReader sr = new StreamReader("C://Users/user/Desktop/test.txt"))
            {
                //Reads all the text
                String text = sr.ReadToEnd();
                //http://stackoverflow.com/questions/8707208/find-the-highest-occuring-words-in-a-string-c-sharp






                //linq command
                var result1 = Regex.Split(text.ToLower(), @"\W+") //split the text with whitespace and make it lowercase
         .Where(s => s.Length > 3 && !stopWord.Contains(s)) //take the words that have length more than 3 and exclude the stopwords
         .GroupBy(s => s) //make a group for each word,for example the word assign will appear in only one group and will have 3 appearances in that group
         .OrderByDescending(g => g.Count()).Take(20); //decending order of the groups which are actually the words according to how many times that word appears, we take the first 10 words



                //the result gives us ten words
                foreach (var v in result1)//for each word
                {
                    //we take the word s
                    string s = v.Key.ToString(); //v.key is the word
                    var result2 = Regex.Split(text.ToLower(), @"\W+").Where(s1 => s1.ToString() == s); //in order to get the number of appearance

                    Console.WriteLine("" + s + "(" + result2.Count() + ")");  //for a specific word

                }

                //Console.WriteLine(text);
                //char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}' };
                //string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            }

        }


        public void frequentOfWord(String filename)
        {

            using (StreamReader sr = new StreamReader("C://Users/user/Desktop/test.txt"))


            //using (StreamReader sr = new StreamReader("http://blmc.blob.core.windows.net/ocrplaintext/000001079_0.txt"))
            {
                //Reads all the text
                String text = sr.ReadToEnd();
                Console.WriteLine("Please enter a word");
                String s = Console.ReadLine();
                var result2 = Regex.Split(text.ToLower(), @"\W+").Where(s1 => s1.ToString() == s); //in order to get the number of appearance

                Console.WriteLine("" + s + "(" + result2.Count() + ")");  //for a specific word
            }

        }


        public void adjacentWordsForMostFrequents(String filename)
        {
            List<string> stopWord = new List<string>(new string[] { "a", "able", "about", "across", "after", "all", "almost", "also", "am", "among", "an", "and", "any", "are", "as", "at", "be", "because", "been", "but", "by", "can", "cannot", "could", "dear", "did", "do", "does", "either", "else", "ever", "every", "for", "from", "get", "got", "had", "has", "have", "he", "her", "hers", "him", "his", "how", "however", "i", "if", "in", "into", "is", "it", "its", "just", "least", "let", "like", "likely", "may", "me", "might", "most", "must", "my", "neither", "no", "nor", "not", "of", "off", "often", "on", "only", "or", "other", "our", "own", "rather", "said", "say", "says", "she", "should", "since", "so", "some", "than", "that", "the", "their", "them", "then", "there", "these", "they", "this", "tis", "to", "too", "twas", "us", "wants", "was", "we", "were", "what", "when", "where", "which", "while", "who", "whom", "why", "will", "with", "would", "yet", "you", "your", "ain't", "aren't", "can't", "could've", "couldn't", "didn't", "doesn't", "don't", "hasn't", "he'd", "he'll", "he's", "how'd", "how'll", "how's", "i'd", "i'll", "i'm", "i've", "isn't", "it's", "might've", "mightn't", "must've", "mustn't", "shan't", "she'd", "she'll", "she's", "should've", "shouldn't", "that'll", "that's", "there's", "they'd", "they'll", "they're", "they've", "wasn't", "we'd", "we'll", "we're", "weren't", "what'd", "what's", "when'd", "when'll", "when's", "where'd", "where'll", "where's", "who'd", "who'll", "who's", "why'd", "why'll", "why's", "won't", "would've", "wouldn't", "you'd", "you'll", "you're", "you've" });

            using (StreamReader sr = new StreamReader("C://Users/user/Desktop/test.txt"))
            {
                //Reads all the text
                String text = sr.ReadToEnd();
                //http://stackoverflow.com/questions/8707208/find-the-highest-occuring-words-in-a-string-c-sharp


                //linq command
                var result1 = Regex.Split(text.ToLower(), @"\W+") //split the text with whitespace and make it lowercase
         .Where(s => s.Length > 3 && !stopWord.Contains(s)) //take the words that have length more than 3 and exclude the stopwords
         .GroupBy(s => s) //make a group for each word,for example the word assign will appear in only one group and will have 3 appearances in that group
         .OrderByDescending(g => g.Count()).Take(10); //decending order of the groups which are actually the words according to how many times that word appears, we take the first 10 words

                //the result gives us ten words
                foreach (var v in result1)//for each word
                {


                    List<string> wordsBeside = new List<string>();


                    //we take the word s
                    string s = v.Key.ToString(); //v.key is the word

                    /*******************************************************************************************************************************/
                    // var result2 = Regex.Split(text.ToLower(), @"\W+").Where(s1 => s1.ToString() == s); //in order to get the number of appearance
                    //Console.WriteLine("" + s + "(" + result2.Count() + ")");  //for a specific word
                    /********************************************************************************************************************************/


                    string[] words = Regex.Split(text.ToLower(), @"\W+");


                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        if

                          (((words[i].Equals(s)) && (!wordsBeside.Contains(words[i + 1])))


                         )
                        {
                            wordsBeside.Add(words[i + 1]);
                            Console.WriteLine("The adjacent word of {0} are {1}", s, words[i + 1]);
                        }

                        // wordsBesideNot.Add(words[i + 1]);
                    }

                }

            }

        }


        public void frequentOfBiagrams(String filename)
        {

            using (StreamReader sr = new StreamReader("C://Users/user/Desktop/test.txt"))
            {
                //Reads all the text
                String text = sr.ReadToEnd();
                Console.WriteLine("Please enter a word");
                String s = Console.ReadLine();


                int count = 0;
                foreach (Match m in Regex.Matches(text, s))
                    count++;
                Console.WriteLine("{0}", count);

                //var frequency = text
                //    .SelectMany(u => text.Split(new string[] { ", " }, StringSplitOptions.None))
                //    .GroupBy(u => u)
                //    .ToDictionary(g => g.Key, g => g.Count());

            }

        }

        public void frequentOfTriagram(String filename)
        {

            using (StreamReader sr = new StreamReader("C://Users/user/Desktop/test.txt"))
            {
                //Reads all the text
                String text = sr.ReadToEnd();
                Console.WriteLine("Please enter a word");
                String s = Console.ReadLine();



                //http://stackoverflow.com/questions/541954/how-would-you-count-occurrences-of-a-string-within-a-string
                int count = 0;
                foreach (Match m in Regex.Matches(text, s))
                    count++;
                Console.WriteLine("{0}", count);

                //var frequency = text
                //    .SelectMany(u => text.Split(new string[] { ", " }, StringSplitOptions.None))
                //    .GroupBy(u => u)
                //    .ToDictionary(g => g.Key, g => g.Count());

            }

        }
    
    }

}