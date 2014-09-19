using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Account name and key.
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string accountName = "blmc";
            string accountKey = "NzNKHqfmQiwqsFo9Y6IRlJj4vxziEDnaTQ23a0P1wDF4r+dqCk4WHqZWOp29RhYh99ywWNV2h2v54lz+gcqpMQ==";

            // Get a reference to the storage account and client with authentication credentials.
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);
            
            //client            
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            string container_name;
            Console.Write("Enter the container: ");
            container_name = Console.ReadLine();
            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(container_name);


         


			// Please enter here the location for the ouput file
            //string path = @"C" + container.Name + ".txt";
            Console.WriteLine("Working on Container {0}", container.Name);
            Console.WriteLine("The new file will be saved in: {0}", path);
            int count = 0;
            char[] delimiters = new char[] { '_', '.'};

				//Please enter here the location of your json file
                //using (StreamReader file = File.OpenText(@"C"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject o2 = (JObject)JToken.ReadFrom(reader);


                    int length = ((JArray)o2["d"]).Count;

                    JArray a = (JArray)o2["d"];

                    IList<Entity> entity1 = a.ToObject<IList<Entity>>();

                    // Create the file. 



                    foreach (var blobItem in container.ListBlobs())
                    {

                        Uri blobUri = blobItem.Uri;
                        
                        string blobUriToSplit = blobUri.Segments[2];
                        string[] words = blobUriToSplit.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                        int printsysnum = Int32.Parse(words[0]);
                        string vol = words[1];
                        string scannumber = null;
                        string idx = null;
                        string date = null;
                        string sizebracket = null;

                        if (words.Length == 7)
                        {

                            scannumber = words[2];
                            idx = words[3];
                            date = words[4];
                            sizebracket = words[5];

                        }
                        else if (words.Length == 5)
                        {

                            scannumber = "";
                            idx = "";
                            date = words[2];
                            sizebracket = words[3];

                        }



                        
                        for (int i = 0; i < length; i++)
                        {



                            //Console.WriteLine("This is the printsysnum of the entity {0}", entity1[i].printsysnum);
                            //Console.WriteLine("This is the printsysnum of the URI {0}", printsysnum);
                            if (entity1[i].printsysnum.ToString() == printsysnum.ToString())
                            {
                                
                                string flick_original_jpeg = "";
                                string flick_url = "";
                                if (scannumber == entity1[i].scannumber)
                                {
                                     flick_original_jpeg = entity1[i].flickr_original_jpeg;
                                     flick_url = entity1[i].flickr_url;
                                }
                               
                                //Console.WriteLine("Found");
                                string creators_and_contributors = "";
                                if (entity1[i].creators_and_contributors.Count > 0)
                                {
                                     creators_and_contributors = entity1[i].creators_and_contributors[0];
                                }
                                
                                string electronicsysnum = entity1[i].electronicsysnum;
                                
                                string fromshelfmark = entity1[i].fromshelfmark;
                                int height = entity1[i].height;
                                string ocrtext = entity1[i].ocrtext;
                                string place = entity1[i].place;
                                string publisher = entity1[i].publisher;
                                string tags = string.Join(",", entity1[i].tags);
                                string title = entity1[i].title;
                                int width = entity1[i].width;
                                string toWrite = "{'azure_url':" + "'" + blobUri + "'," + "'creators_and_contributors':" + "'" + creators_and_contributors + "',"
                                    + "'date':" + "'" + date + "'," + "'electronicsysnum':" + "'" + electronicsysnum + "'," + "'flick_original_jpeg':" + "'" + flick_original_jpeg + "',"
                                     + "'flick_url':" + "'" + flick_url + "'," + "'fromshelfmark':" + "'" + fromshelfmark + "'," + "'height':" + "'" + height + "'," +
                                     "'idx':" + "'" + idx + "'," + "'ocrtext':" + "'" + ocrtext + "'," + "'place':" + "'" + place + "'," + "'printsysnum':" + "'" + words[0] + "',"
                                     + "'publisher':" + "'" + publisher + "'," + "'scannumber':" + "'" + scannumber + "'," + "'sizebracket':" + "'" + sizebracket + "'," 
                                     + "'tags':" + "'" + tags + "'," + "'title':" + "'" + title + "'," + "'vol':" + "'" + vol + "'," + "'width':" + "'" + width + "'},";

                                using (System.IO.StreamWriter tt = new System.IO.StreamWriter(@"C:\Users\Stelios\Desktop\" + container.Name + ".txt", true))
                                {
                                    tt.WriteLine(toWrite);
                                }


                                count++;

                            }


                        }
                        



                    }
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    Console.WriteLine("RunTime " + elapsedTime);
                    Console.WriteLine("Finished. Total found {0}:" , count);
                    Console.ReadLine();
                }
            }


           

              

               







                
            
        }

        public class Entity
        {
            public string azure_url { get; set; }
            public List<string> creators_and_contributors { get; set; }
            public int? date { get; set; }
            public string electronicsysnum { get; set; }
            public string flickr_original_jpeg { get; set; }
            public string flickr_url { get; set; }
            public string fromshelfmark { get; set; }
            public int height { get; set; }
            public int idx { get; set; }
            public string ocrtext { get; set; }
            public string place { get; set; }
            public int printsysnum { get; set; }
            public string publisher { get; set; }
            public string scannumber { get; set; }
            public string sizebracket { get; set; }
            public List<string> tags { get; set; }
            public string title { get; set; }
            public int vol { get; set; }
            public int width { get; set; }
        }

       



    }

