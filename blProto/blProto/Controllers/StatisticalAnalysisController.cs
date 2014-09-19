using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace blProto.Controllers
{
    public class StatisticalAnalysisController : Controller
    {
        //
        // GET: /StatisticalAnalysis/
        public ActionResult Index(Uri website, string titleOfBook)
        
        {
            AzureSearchServiceController checkUrl = new AzureSearchServiceController();

            if (checkUrl.RemoteFileExists(website.ToString()))
            {
               
                List<string> MostFrequentWords = new List<string>();
                List<int> FrequencyOfMostFrequentWords = new List<int>();

                frequentWords(website, out MostFrequentWords, out FrequencyOfMostFrequentWords);
                ViewBag.FrequencyOfMostFrequentWords = FrequencyOfMostFrequentWords;
                ViewBag.MostFrequentWords = MostFrequentWords;

                ViewBag.titleOfBook = titleOfBook;
                ViewBag.website = website;
                //stores the user keyword to identify the frequency of a word
                string keyword = Request["txtSearch"];
                ViewBag.keyword = Request["txtSearch"];
               
                /////////////////////////////////////////
                //stores the user keyword to identify the frequency of a biagram
                string keywordBiagram = Request["txtSearchBiagram"];
                ViewBag.keywordBiagram = Request["txtSearchBiagram"];
             ////////////////////////////////////////////////////
                //stores the user keyword to identify the frequency of a triagram
                string keywordTriagram = Request["txtSearchTriagram"];
                ViewBag.keywordBiagram = Request["txtSearchTriagram"];

                char[] delimiters = new char[] { ' ' };

                int frequencyWordInOCR = 0;
                if (String.IsNullOrEmpty(keyword))
                {
                    ViewBag.initialState = 0;
                }
                else 
                {

                    string[] userKeyword = keyword.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    //display a message to the user (the user has entered more than one word)
                    if (userKeyword.Count() != 1) { ViewBag.MessageOneWord = "Please enter one word"; }
                    else
                    {


                        frequencyWordInOCR = frequentWordInOCR(website, keyword.ToLower().Trim());
                        if (frequencyWordInOCR == 0) { ViewBag.WordNotFound = "Word: " + keyword + " " + "was not found"; }
                        else { ViewBag.wordFound = "The frequency of the word " + keyword + " " + "is " + frequencyWordInOCR; }
                    }
                }
                
                //////////////////////////////Biagram/////////////////////////////
                int frequencyOfBiagram = 0;

              

              
                    if (String.IsNullOrEmpty(keywordBiagram))
                    {
                        ViewBag.initialStateBiagram = 0;
                    }
                    else
                    {
                        
                        string[] wordsBiagram = keywordBiagram.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                        //check whether the user has entered two words
                        if (wordsBiagram.Count() != 2) { ViewBag.MessageBiagram = "Please enter a valid biagram(biagram consists of two words)"; }
                        else
                        {


                            frequencyOfBiagram = frequentOfBiagrams(website, keywordBiagram.ToLower().Trim());
                            if (frequencyOfBiagram == 0) { ViewBag.WordNotFoundBiagram = "Word: " + keywordBiagram + " " + "was not found"; }
                            else { ViewBag.wordFoundBiagram = "The frequency of a biagram " + keywordBiagram + " " + "is " + frequencyOfBiagram; }

                        }
                    }
                
                ///////////////////////////////////////////////////////////////////////////////////////
                //this section calculate the frequency word of a triagram and checks whether the user has entered three words in the document
                int frequencyOfTriagram = 0;

                if (String.IsNullOrEmpty(keywordTriagram))
                {
                    ViewBag.initialStateTriagram = 0;
                }
                else
                {

                    string[] wordsTriagram = keywordTriagram.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    //check whether the user has entered three words
                    if (wordsTriagram.Count() != 3) { ViewBag.MessageTriagram = "Please enter a valid triagram(triagram consists of two words)"; }

                    else
                    {


                        frequencyOfTriagram = frequentOfBiagrams(website, keywordTriagram.ToLower().Trim());
                        if (frequencyOfTriagram == 0) { ViewBag.WordNotFoundTriagram = "Word: " + keywordTriagram + " " + "was not found"; }
                        else { ViewBag.wordFoundTriagram = "The frequency of a triagram " + keywordTriagram + " " + "is " + frequencyOfTriagram; }


                    }
                    }




                
               
                return View();

            }
            else
            {

                return RedirectToAction("ErrorPage", "ErrorPage");
            }
          
        }


        //The funciton frequentWordInOCR(Uri filename, string keyword) calculates the frequency of a specific word within a text document. 
        //The user inputs a specific word and the system returns the number of occurrences.
        public static int frequentWordInOCR(Uri filename, string keyword)
        {

            
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);


            //Console.WriteLine("{0}",webData);

            //we use linq in order to retrieve the number of appearance of the word.
            var result2 = Regex.Split(webData.ToLower(), @"\W+").Where(s1 => s1.ToString() == keyword); //in order to get the number of appearance

            // Console.WriteLine("" + keyword + "(" + result2.Count() + ")");  //for a specific word

            //frequency of a word
            return result2.Count();
        }





        //The functions frequentOfBiagrams(Uri filename, string keyword) caculate the frequency of a sequence of two words
        //The user inputs two words  for  a biagram and the frequency of that bigram is calculated and returned .


        public static int frequentOfBiagrams(Uri filename, string keyword)
        {


            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);
            int count = 0;


            webData = webData.ToLower();
            //a Match class is used in order to identify all matches of the user input within the document.
            foreach (Match m in Regex.Matches(webData, keyword))
            { count++; }
            return count;
        }


        //frequentOfTriagrams(Uri filename, string keyword) calculate the frequency of three words 
        //in a text document respectively. The user inputs three words in a triagram and the frequency of that triagram within the document
        //is calculated and returned .
        public static int frequentOfTriagrams(Uri filename, string keyword)
        {


            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);
            int count = 0;


            webData = webData.ToLower();
            //a Match class is used in order to identify all matches of the user input within the document.
            foreach (Match m in Regex.Matches(webData, keyword))
            { count++; }
            return count;
        }



        // The function frequentWords (Uri filename, out List<string> MostFrequentWords, out  List<int> FrequencyOfMostFrequentWords)
        //calculates the frequency of words in a text document.
        //The URI of the ocr of a book is passed in order to scan the entire text and the result
        //is the most frequency words and their counters into accumulated into two lists.  
        public void frequentWords(Uri filename, out List<string> MostFrequentWords, out  List<int> FrequencyOfMostFrequentWords)
        {
            //List<string> stopWord = new List<string>(new string[] { "a", "able", "about", "across", "after", "all", "almost", "also", "am", "among", "an", "and", "any", "are", "as", "at", "be", "because", "been", "but", "by", "can", "cannot", "could", "dear", "did", "do", "does", "either", "else", "ever", "every", "for", "from", "get", "got", "had", "has", "have", "he", "her", "hers", "him", "his", "how", "however", "i", "if", "in", "into", "is", "it", "its", "just", "least", "let", "like", "likely", "may", "me", "might", "most", "must", "my", "neither", "no", "nor", "not", "of", "off", "often", "on", "only", "or", "other", "our", "own", "rather", "said", "say", "says", "she", "should", "since", "so", "some", "than", "that", "the", "their", "them", "then", "there", "these", "they", "this", "tis", "to", "too", "twas", "us", "wants", "was", "we", "were", "what", "when", "where", "which", "while", "who", "whom", "why", "will", "with", "would", "yet", "you", "your", "ain't", "aren't", "can't", "could've", "couldn't", "didn't", "doesn't", "don't", "hasn't", "he'd", "he'll", "he's", "how'd", "how'll", "how's", "i'd", "i'll", "i'm", "i've", "isn't", "it's", "might've", "mightn't", "must've", "mustn't", "shan't", "she'd", "she'll", "she's", "should've", "shouldn't", "that'll", "that's", "there's", "they'd", "they'll", "they're", "they've", "wasn't", "we'd", "we'll", "we're", "weren't", "what'd", "what's", "when'd", "when'll", "when's", "where'd", "where'll", "where's", "who'd", "who'll", "who's", "why'd", "why'll", "why's", "won't", "would've", "wouldn't", "you'd", "you'll", "you're", "you've" });
            
            //We want a set of words to be excluded from search results including for instance the articles such that "the", "and" and "an" and therefore, we use a stopword list which has been taken from the Voyant Tools 
            List<string> stopWord = new List<string>(new string[] { "!", "$", "%", "&", ",", "-", ".", "0", "1", "10", "100", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2", "20", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "21", "22", "23", "24", "25", "26", "27", "28", "29", "3", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "4", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "5", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "6", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "7", "70", "71", "72", "73", "74", "75", "76", "77", "78", "8", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "9", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", ":", ";", "<", ">", "@", "Ap.", "Apr.", "GHz", "MHz", "USD", "a", "ab", "abbia", "abbiamo", "abbiano", "abbiate", "aber", "abgerufen", "abgerufene", "abgerufener", "abgerufenes", "about", "above", "acht", "across", "ad", "afin", "after", "afterwards", "again", "against", "agl", "agli", "ah", "ai", "aie", "aient", "aies", "ait", "al", "alguna", "algunas", "alguno", "algunos", "algun", "all", "alla", "alle", "allein", "allem", "allen", "aller", "allerdings", "allerlei", "alles", "allgemein", "allmahlich", "allo", "allzu", "almost", "alone", "along", "alors", "already", "als", "alsbald", "also", "although", "always", "am", "ambos", "among", "amongst", "amoungst", "amount", "ampleamos", "an", "anche", "and", "ander", "andere", "anderem", "anderen", "anderer", "andererseits", "anderes", "anderm", "andern", "andernfalls", "anders", "anerkannt", "anerkannte", "anerkannter", "anerkanntes", "anfangen", "anfing", "angefangen", "angesetze", "angesetzt", "angesetzten", "angesetzter", "another", "ansetzen", "anstatt", "ante", "antes", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "apres", "aquel", "aquellas", "aquellos", "aqui", "arbeiten", "are", "around", "arriba", "as", "at", "atras", "attendu", "au", "au-dela", "au-devant", "auch", "aucun", "aucune", "audit", "auf", "aufgehort", "aufgrund", "aufhoren", "aufhorte", "aufzusuchen", "aupres", "auquel", "aura", "aurai", "auraient", "aurais", "aurait", "auras", "aurez", "auriez", "aurions", "aurons", "auront", "aus", "ausdrucken", "ausdruckt", "ausdruckte", "ausgenommen", "ausser", "ausserdem", "aussi", "author", "autor", "autour", "autre", "autres", "autrui", "aux", "auxdites", "auxdits", "auxquelles", "auxquels", "au?en", "au?er", "au?erdem", "au?erhalb", "avaient", "avais", "avait", "avant", "avec", "avemmo", "avendo", "avesse", "avessero", "avessi", "avessimo", "aveste", "avesti", "avete", "aveva", "avevamo", "avevano", "avevate", "avevi", "avevo", "avez", "aviez", "avions", "avons", "avrai", "avranno", "avrebbe", "avrebbero", "avrei", "avremmo", "avremo", "avreste", "avresti", "avrete", "avra", "avro", "avuta", "avute", "avuti", "avuto", "ayant", "ayez", "ayons", "b", "back", "bah", "bajo", "bald", "banco", "bastante", "be", "bearbeite", "bearbeiten", "bearbeitete", "bearbeiteten", "because", "bedarf", "bedurfte", "bedurfen", "been", "before", "beforehand", "befragen", "befragte", "befragten", "befragter", "begann", "beginnen", "begonnen", "behalten", "behielt", "bei", "beide", "beiden", "beiderlei", "beides", "beim", "beinahe", "being", "beitragen", "beitrugen", "bekannt", "bekannte", "bekannter", "bekennen", "ben", "benutzt", "bereits", "berichten", "berichtet", "berichtete", "berichteten", "beside", "besides", "besonders", "besser", "bestehen", "besteht", "betrachtlich", "between", "bevor", "bezuglich", "bien", "bietet", "bin", "bis", "bisher", "bislang", "bist", "bleiben", "blieb", "bloss", "blo?", "both", "bottom", "brachte", "brachten", "brauchen", "braucht", "bringen", "brauchte", "bsp.", "but", "by", "bzw", "be", "boden", "c", "c'", "c'est", "c'etait", "ca.", "cada", "call", "can", "cannot", "cant", "car", "ce", "ceci", "cela", "celle", "celle-ci", "celle-la", "celles", "celles-ci", "celles-la", "celui", "celui-ci", "celui-la", "cela", "cent", "cents", "cependant", "certain", "certaine", "certaines", "certains", "ces", "cet", "cette", "ceux", "ceux-ci", "ceux-la", "cf.", "cg", "cgr", "chacun", "chacune", "chaque", "che", "chez", "chi", "ci", "cierta", "ciertas", "ciertos", "cinq", "cinquante", "cinquante-cinq", "cinquante-deux", "cinquante-et-un", "cinquante-huit", "cinquante-neuf", "cinquante-quatre", "cinquante-sept", "cinquante-six", "cinquante-trois", "cl", "cm", "cm²", "co", "coi", "col", "come", "comme", "como", "con", "conseguimos", "conseguir", "consigo", "consigue", "consiguen", "consigues", "contre", "contro", "could", "couldnt", "cual", "cuando", "cui", "d", "d'", "d'apres", "d'un", "d'une", "da", "dabei", "dadurch", "dafur", "dagegen", "dagl", "dagli", "daher", "dahin", "dai", "dal", "dall", "dalla", "dalle", "dallo", "damals", "damit", "danach", "daneben", "dank", "danke", "danken", "dann", "dannen", "dans", "daran", "darauf", "daraus", "darf", "darfst", "darin", "darum", "darunter", "daruber", "daruberhinaus", "das", "dass", "dasselbe", "davon", "davor", "dazu", "da?", "de", "degl", "degli", "dei", "dein", "deine", "deinem", "deinen", "deiner", "deines", "del", "dell", "della", "delle", "dello", "dem", "demnach", "demselben", "den", "denen", "denn", "dennoch", "denselben", "dentro", "depuis", "der", "derart", "derartig", "derem", "deren", "derer", "derjenige", "derjenigen", "derriere", "derselbe", "derselben", "derzeit", "des", "desdites", "desdits", "deshalb", "desquelles", "desquels", "desselben", "dessen", "desto", "deswegen", "deux", "devant", "devers", "dg", "di", "dich", "did", "didn't", "die", "diejenige", "dies", "diese", "dieselbe", "dieselben", "diesem", "diesen", "dieser", "dieses", "diesseits", "differentes", "differents", "dinge", "dir", "direkt", "direkte", "direkten", "direkter", "divers", "diverses", "dix", "dix-huit", "dix-neuf", "dix-sept", "dl", "dm", "do", "doch", "does", "doesn't", "don't", "donc", "donde", "done", "dont", "doppelt", "dort", "dorther", "dorthin", "dos", "douze", "dov", "dove", "down", "drauf", "drei", "drei?ig", "drin", "dritte", "drunter", "druber", "du", "dudit", "due", "dunklen", "duquel", "durant", "durch", "durchaus", "durfte", "durften", "during", "des", "deja", "durfen", "durfte", "e", "each", "ebbe", "ebbero", "ebbi", "eben", "ebenfalls", "ebenso", "ed", "eg", "eh", "ehe", "eher", "eigenen", "eigenes", "eigentlich", "eight", "ein", "einbaun", "eine", "einem", "einen", "einer", "einerseits", "eines", "einfach", "einfuhren", "einfuhrte", "einfuhrten", "eingesetzt", "einig", "einige", "einigem", "einigen", "einiger", "einigerma?en", "einiges", "einmal", "eins", "einseitig", "einseitige", "einseitigen", "einseitiger", "einst", "einstmals", "einzig", "either", "el", "eleven", "ellas", "elle", "elles", "ellos", "else", "elsewhere", "empleais", "emplean", "emplear", "empleas", "empleo", "en", "en-dehors", "encima", "encore", "ende", "enfin", "enough", "entonces", "entre", "entsprechend", "entweder", "envers", "er", "era", "eramos", "eran", "erano", "eras", "eravamo", "eravate", "eres", "erganze", "erganzen", "erganzte", "erganzten", "erhalten", "erhielt", "erhielten", "erhalt", "eri", "erneut", "ero", "erst", "erste", "ersten", "erster", "eroffne", "eroffnen", "eroffnet", "eroffnete", "eroffnetes", "es", "essendo", "est", "esta", "estaba", "estado", "estais", "estamos", "estan", "estoy", "et", "etc", "etliche", "etwa", "etwas", "eu", "euch", "eue", "euer", "eues", "euh", "eure", "eurem", "euren", "eurent", "eurer", "eures", "eus", "eusse", "eussent", "eusses", "eussiez", "eussions", "eut", "eux", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "eumes", "eut", "eutes", "f", "faccia", "facciamo", "facciano", "facciate", "faccio", "facemmo", "facendo", "facesse", "facessero", "facessi", "facessimo", "faceste", "facesti", "faceva", "facevamo", "facevano", "facevate", "facevi", "facevo", "fai", "fait", "fall", "falls", "fand", "fanno", "farai", "faranno", "farebbe", "farebbero", "farei", "faremmo", "faremo", "fareste", "faresti", "farete", "fara", "faro", "fast", "fece", "fecero", "feci", "ferner", "few", "fi", "fifteen", "fify", "fill", "fin", "find", "finden", "findest", "findet", "fire", "first", "five", "flac", "folgende", "folgenden", "folgender", "folgendes", "folglich", "for", "fordern", "fordert", "forderte", "forderten", "former", "formerly", "fors", "fortsetzen", "fortsetzt", "fortsetzte", "fortsetzten", "forty", "fosse", "fossero", "fossi", "fossimo", "foste", "fosti", "found", "four", "fragte", "frau", "frei", "freie", "freier", "freies", "from", "front", "fu", "fue", "fuer", "fueron", "fui", "fuimos", "full", "fummo", "furent", "furono", "further", "fus", "fusse", "fussent", "fusses", "fussiez", "fussions", "fut", "fumes", "fut", "futes", "funf", "fur", "g", "gab", "ganz", "ganze", "ganzem", "ganzen", "ganzer", "ganzes", "gar", "gbr", "geb", "geben", "geblieben", "gebracht", "gedurft", "geehrt", "geehrte", "geehrten", "geehrter", "gefallen", "gefiel", "gefalligst", "gefallt", "gegeben", "gegen", "gehabt", "gehen", "geht", "gekommen", "gekonnt", "gemacht", "gemocht", "gemass", "genommen", "genug", "gern", "gesagt", "gesehen", "gestern", "gestrige", "get", "getan", "geteilt", "geteilte", "getragen", "gewesen", "gewisserma?en", "gewollt", "geworden", "ggf", "gib", "gibt", "give", "gleich", "gleichwohl", "gleichzeitig", "gli", "glucklicherweise", "gmbh", "go", "gr", "gratulieren", "gratuliert", "gratulierte", "gueno", "gute", "guten", "gangig", "gangige", "gangigen", "gangiger", "gangiges", "ganzlich", "h", "ha", "hab", "habe", "haben", "hace", "haceis", "hacemos", "hacen", "hacer", "haces", "had", "haette", "hago", "hai", "halb", "hallo", "han", "hanno", "has", "hasnt", "hast", "hat", "hatte", "hatten", "hattest", "hattet", "have", "he", "hein", "hem", "hence", "her", "heraus", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "heu", "heute", "heutige", "hg", "hier", "hiermit", "hiesige", "him", "himself", "hin", "hinein", "hinten", "hinter", "hinterher", "his", "hl", "hm", "hm³", "ho", "hoch", "hola", "hop", "hormis", "hors", "how", "however", "huit", "hum", "hundert", "hundred", "hatt", "hatte", "hatten", "he", "hochstens", "i", "ich", "ici", "ie", "if", "igitt", "ihm", "ihn", "ihnen", "ihr", "ihre", "ihrem", "ihren", "ihrer", "ihres", "il", "ils", "im", "immer", "immerhin", "important", "in", "inc", "incluso", "indeed", "indem", "indessen", "info", "infolge", "innen", "innerhalb", "ins", "insofern", "intenta", "intentais", "intentamos", "intentan", "intentar", "intentas", "intento", "into", "inzwischen", "io", "ir", "irgend", "irgendeine", "irgendwas", "irgendwen", "irgendwer", "irgendwie", "irgendwo", "is", "ist", "it", "its", "itself", "j", "j'", "j'ai", "j'avais", "j'etais", "ja", "jamais", "je", "jede", "jedem", "jeden", "jedenfalls", "jeder", "jederlei", "jedes", "jedoch", "jemand", "jene", "jenem", "jenen", "jener", "jenes", "jenseits", "jetzt", "jusqu'", "jusqu'au", "jusqu'aux", "jusqu'a", "jusque", "jahrig", "jahrige", "jahrigen", "jahriges", "k", "kam", "kann", "kannst", "kaum", "keep", "kein", "keine", "keinem", "keinen", "keiner", "keinerlei", "keines", "keineswegs", "kg", "klar", "klare", "klaren", "klares", "klein", "kleinen", "kleiner", "kleines", "km", "km²", "koennen", "koennt", "koennte", "koennten", "komme", "kommen", "kommt", "konkret", "konkrete", "konkreten", "konkreter", "konkretes", "konnte", "konnten", "konn", "konnen", "konnt", "konnte", "konnten", "kunftig", "l", "l'", "l'autre", "l'on", "l'un", "l'une", "la", "lag", "lagen", "langsam", "laquelle", "largo", "las", "lassen", "last", "latter", "latterly", "laut", "le", "least", "lediglich", "leer", "legen", "legte", "legten", "lei", "leicht", "leider", "lequel", "les", "lesen", "lesquelles", "lesquels", "less", "letze", "letzten", "letztendlich", "letztens", "letztes", "letztlich", "leur", "leurs", "lez", "li", "lichten", "liegt", "liest", "links", "lo", "loro", "lors", "lorsqu'", "lorsque", "los", "ltd", "lui", "langst", "langstens", "les", "m", "m'", "ma", "mache", "machen", "machst", "macht", "machte", "machten", "made", "mag", "magst", "maint", "mainte", "maintes", "maints", "mais", "mal", "malgre", "man", "manche", "manchem", "manchen", "mancher", "mancherorts", "manches", "manchmal", "mann", "many", "margin", "may", "me", "meanwhile", "mehr", "mehrere", "mein", "meine", "meinem", "meinen", "meiner", "meines", "meist", "meiste", "meisten", "mes", "meta", "mg", "mgr", "mi", "mia", "mich", "mie", "miei", "mientras", "might", "mil", "mill", "mille", "milliards", "millions", "mindestens", "mine", "mio", "mir", "mit", "mithin", "ml", "mm", "mm²", "mochte", "modo", "moi", "moins", "mon", "more", "moreover", "morgen", "morgige", "most", "mostly", "move", "moyennant", "mt", "much", "muchos", "muessen", "muesst", "muesste", "muss", "musst", "musste", "mussten", "must", "muy", "mu?", "mu?t", "my", "myself", "m²", "m³", "meme", "memes", "mochte", "mochten", "mochtest", "mogen", "moglich", "mogliche", "moglichen", "moglicher", "moglicherweise", "mussen", "musste", "mussten", "mu?t", "mu?te", "n", "n'avait", "n'y", "nach", "nachdem", "nacher", "nachhinein", "nacht", "nahm", "name", "namely", "naturlich", "ne", "neben", "nebenan", "negl", "negli", "nehmen", "nei", "nein", "neither", "nel", "nell", "nella", "nelle", "nello", "neu", "neue", "neuem", "neuen", "neuer", "neues", "neuf", "neun", "never", "nevertheless", "next", "ni", "nicht", "nichts", "nie", "niemals", "niemand", "nimm", "nimmer", "nimmt", "nine", "nirgends", "nirgendwo", "no", "nobody", "noch", "noi", "non", "nonante", "none", "nonobstant", "noone", "nor", "nos", "nosotros", "nostra", "nostre", "nostri", "nostro", "not", "nothing", "notre", "nous", "now", "nowhere", "nul", "nulle", "nun", "nur", "nutzen", "nutzt", "nutzung", "n?", "nachste", "namlich", "neanmoins", "notigenfalls", "nutzt", "o", "ob", "oben", "oberhalb", "obgleich", "obschon", "obwohl", "octante", "oder", "of", "off", "oft", "often", "oh", "ohne", "on", "once", "one", "only", "ont", "onto", "onze", "or", "other", "others", "otherwise", "otro", "ou", "our", "ours", "ourselves", "out", "outre", "over", "own", "ou", "p", "par", "par-dela", "para", "parbleu", "parce", "parmi", "part", "pas", "passe", "pendant", "per", "perche", "perhaps", "pero", "personne", "peu", "pfui", "piu", "please", "plus", "plus_d'un", "plus_d'une", "plusieurs", "plotzlich", "podeis", "podemos", "poder", "podria", "podriais", "podriamos", "podrian", "podrias", "por", "por que", "porque", "pour", "pourquoi", "pourtant", "pourvu", "primero desde", "pro", "pres", "puede", "pueden", "puedo", "puisqu'", "puisque", "put", "q", "qu", "qu'", "qu'elle", "qu'elles", "qu'il", "qu'ils", "qu'on", "quale", "quand", "quant", "quanta", "quante", "quanti", "quanto", "quarante", "quarante-cinq", "quarante-deux", "quarante-et-un", "quarante-huit", "quarante-neuf", "quarante-quatre", "quarante-sept", "quarante-six", "quarante-trois", "quatorze", "quatre", "quatre-vingt", "quatre-vingt-cinq", "quatre-vingt-deux", "quatre-vingt-dix", "quatre-vingt-dix-huit", "quatre-vingt-dix-neuf", "quatre-vingt-dix-sept", "quatre-vingt-douze", "quatre-vingt-huit", "quatre-vingt-neuf", "quatre-vingt-onze", "quatre-vingt-quatorze", "quatre-vingt-quatre", "quatre-vingt-quinze", "quatre-vingt-seize", "quatre-vingt-sept", "quatre-vingt-six", "quatre-vingt-treize", "quatre-vingt-trois", "quatre-vingt-un", "quatre-vingt-une", "quatre-vingts", "que", "quel", "quella", "quelle", "quelles", "quelli", "quello", "quelqu'", "quelqu'un", "quelqu'une", "quelque", "quelques", "quelques-unes", "quelques-uns", "quels", "questa", "queste", "questi", "questo", "qui", "quiconque", "quien", "quinze", "quoi", "quoiqu'", "quoique", "r", "rather", "re", "reagiere", "reagieren", "reagiert", "reagierte", "rechts", "regelma?ig", "revoici", "revoila", "rief", "rien", "rund", "s", "s'", "sa", "sabe", "sabeis", "sabemos", "saben", "saber", "sabes", "sage", "sagen", "sagt", "sagte", "sagten", "sagtest", "same", "sang", "sangen", "sans", "sarai", "saranno", "sarebbe", "sarebbero", "sarei", "saremmo", "saremo", "sareste", "saresti", "sarete", "sara", "saro", "sauf", "schlechter", "schlie?lich", "schnell", "schon", "schreibe", "schreiben", "schreibens", "schreiber", "schwierig", "schatzen", "schatzt", "schatzte", "schatzten", "se", "sechs", "sect", "see", "seem", "seemed", "seeming", "seems", "sehe", "sehen", "sehr", "sehrwohl", "seht", "sei", "seid", "sein", "seine", "seinem", "seinen", "seiner", "seines", "seit", "seitdem", "seite", "seiten", "seither", "seize", "selber", "selbst", "selon", "senke", "senken", "senkt", "senkte", "senkten", "sept", "septante", "ser", "sera", "serai", "seraient", "serais", "serait", "seras", "serez", "seriez", "serions", "serious", "serons", "seront", "ses", "setzen", "setzt", "setzte", "setzten", "several", "she", "should", "si", "sia", "siamo", "siano", "siate", "sich", "sicher", "sicherlich", "sie", "sieben", "siebte", "siehe", "sieht", "siendo", "siete", "sin", "since", "sind", "singen", "singt", "sinon", "six", "sixty", "so", "sobald", "sobre", "soda?", "soeben", "sofern", "sofort", "sog", "sogar", "soi", "soient", "sois", "soit", "soixante", "soixante-cinq", "soixante-deux", "soixante-dix", "soixante-dix-huit", "soixante-dix-neuf", "soixante-dix-sept", "soixante-douze", "soixante-et-onze", "soixante-et-un", "soixante-et-une", "soixante-huit", "soixante-neuf", "soixante-quatorze", "soixante-quatre", "soixante-quinze", "soixante-seize", "soixante-sept", "soixante-six", "soixante-treize", "soixante-trois", "solamente", "solange", "solc hen", "solch", "solche", "solchem", "solchen", "solcher", "solches", "soll", "sollen", "sollst", "sollt", "sollte", "sollten", "solltest", "solo", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "somit", "sommes", "somos", "son", "sondern", "sono", "sonst", "sonstwo", "sont", "sooft", "sous", "soviel", "soweit", "sowie", "sowohl", "soy", "soyez", "soyons", "spielen", "spater", "sta", "stai", "stando", "stanno", "starai", "staranno", "starebbe", "starebbero", "starei", "staremmo", "staremo", "stareste", "staresti", "starete", "startet", "startete", "starteten", "stara", "staro", "statt", "stattdessen", "stava", "stavamo", "stavano", "stavate", "stavi", "stavo", "steht", "steige", "steigen", "steigt", "stemmo", "stesse", "stessero", "stessi", "stessimo", "steste", "stesti", "stets", "stette", "stettero", "stetti", "stia", "stiamo", "stiano", "stiate", "stieg", "stiegen", "still", "sto", "su", "sua", "such", "suchen", "sue", "sugl", "sugli", "sui", "suis", "suite", "sul", "sull", "sulla", "sulle", "sullo", "suo", "suoi", "sur", "sus", "system", "samtliche", "t", "t'", "ta", "tacatac", "tages", "take", "tambien", "tandis", "tat", "tatsachlich", "tatsachlichen", "tatsachlicher", "tatsachliches", "tausend", "te", "teile", "teilen", "teilte", "teilten", "tel", "telle", "telles", "tels", "ten", "teneis", "tenemos", "tener", "tengo", "tes", "than", "that", "the", "thee", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thing", "third", "this", "those", "thou", "though", "three", "through", "throughout", "thru", "thus", "thy", "ti", "tiempo", "tiene", "tienen", "titel", "to", "todo", "together", "toi", "ton", "too", "total", "toujours", "tous", "tout", "toute", "toutefois", "toutes", "toward", "towards", "tra", "trabaja", "trabajais", "trabajamos", "trabajan", "trabajar", "trabajas", "trabajo", "trage", "tragen", "tras", "treize", "trente", "trente-cinq", "trente-deux", "trente-et-un", "trente-huit", "trente-neuf", "trente-quatre", "trente-sept", "trente-six", "trente-trois", "trois", "trotzdem", "trug", "tragt", "tres", "tu", "tua", "tue", "tun", "tuo", "tuoi", "tust", "tut", "tutti", "tutto", "tuyo", "twelve", "twenty", "two", "txt", "tat", "u", "ueber", "ultimo", "um", "umso", "un", "una", "unas", "unbedingt", "und", "under", "une", "unes", "ungefahr", "unmoglich", "unmogliche", "unmoglichen", "unmoglicher", "unnotig", "uno", "unos", "uns", "unse", "unsem", "unsen", "unser", "unsere", "unserem", "unseren", "unserer", "unseres", "unserm", "unses", "unten", "unter", "unterbrach", "unterbrechen", "unterhalb", "until", "unwichtig", "up", "upon", "us", "usa", "usais", "usamos", "usan", "usar", "usas", "uso", "usw", "v", "va", "vais", "valor", "vamos", "van", "vaya", "verdad", "verdadera cierto", "verdadero", "vergangen", "vergangene", "vergangener", "vergangenes", "vermag", "vermutlich", "vermogen", "verrate", "verraten", "verriet", "verrieten", "vers", "version", "versorge", "versorgen", "versorgt", "versorgte", "versorgten", "versorgtes", "very", "veroffentlichen", "veroffentlicher", "veroffentlicht", "veroffentlichte", "veroffentlichten", "veroffentlichtes", "vi", "via", "viel", "viele", "vielen", "vieler", "vieles", "vielleicht", "vielmals", "vier", "vingt", "vingt-cinq", "vingt-deux", "vingt-huit", "vingt-neuf", "vingt-quatre", "vingt-sept", "vingt-six", "vingt-trois", "vis-a-vis", "voi", "voici", "voila", "vollstandig", "vom", "von", "vor", "voran", "vorbei", "vorgestern", "vorher", "vorne", "voruber", "vos", "vosotras", "vosotros", "vostra", "vostre", "vostri", "vostro", "votre", "vous", "voy", "vollig", "w", "wachen", "waere", "wann", "war", "waren", "warst", "warum", "was", "we", "weder", "weg", "wegen", "weil", "weiter", "weitere", "weiterem", "weiteren", "weiterer", "weiteres", "weiterhin", "wei?", "welche", "welchem", "welchen", "welcher", "welches", "well", "wem", "wen", "wenig", "wenige", "weniger", "wenigstens", "wenn", "wenngleich", "wer", "werde", "werden", "werdet", "were", "weshalb", "wessen", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "wichtig", "wie", "wieder", "wieso", "wieviel", "wiewohl", "will", "willst", "wir", "wird", "wirklich", "wirst", "with", "within", "without", "wo", "wodurch", "wogegen", "woher", "wohin", "wohingegen", "wohl", "wohlweislich", "wolle", "wollen", "wollt", "wollte", "wollten", "wolltest", "wolltet", "womit", "woraufhin", "woraus", "worin", "would", "wurde", "wurden", "wahrend", "wahrenddessen", "war", "ware", "waren", "wurde", "wurden", "x", "y", "yet", "yo", "you", "your", "yours", "yourself", "yourselves", "z", "z.B.", "zahlreich", "zehn", "zeitweise", "ziehen", "zieht", "zog", "zogen", "zu", "zudem", "zuerst", "zufolge", "zugleich", "zuletzt", "zum", "zumal", "zur", "zuruck", "zusammen", "zuviel", "zwanzig", "zwar", "zwei", "zwischen", "zwolf", "zero", "|", "a", "ahnlich", "c'", "ca", "e", "es", "etaient", "etais", "etait", "etant", "etiez", "etions", "ete", "etee", "etees", "etes", "etes", "etre", "o", "ubel", "uber", "uberall", "uberallhin", "uberdies", "ubermorgen", "ubrig", "ubrigens", "(", ")", "*", "+", "?", "[", "]", "^", "{", "}" });

            
            
            MostFrequentWords = new List<string>();
            FrequencyOfMostFrequentWords = new List<int>();

            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);


           // using (StreamReader sr = new StreamReader(filename))
           // {
                //Reads all the text
               
                //http://stackoverflow.com/questions/8707208/find-the-highest-occuring-words-in-a-string-c-sharp



             

                //linq command to retrieve the 20 most frequent words of a document
            var result1 = Regex.Split(webData.ToLower(), @"\W+") //split the text with whitespace and make it lowercase
         .Where(s => s.Length > 3 && !stopWord.Contains(s)) //take the words that have length more than 3 and exclude the stopwords
         .GroupBy(s => s) //make a group for each word,for example the word assign will appear in only one group and will have 3 appearances in that group
         .OrderByDescending(g => g.Count()).Take(20); //decending order of the groups which are actually the words according to how many times that word appears, we take the first 10 words

              

                //the result gives us ten words
                foreach (var v in result1)//for each word
                {
                    //we take the word s
                    string s = v.Key.ToString(); //v.key is the word
                    var result2 = Regex.Split(webData.ToLower(), @"\W+").Where(s1 => s1.ToString() == s); //in order to get the number of appearance

                    //Console.WriteLine("" + s + "(" + result2.Count() + ")");  //for a specific word
                    FrequencyOfMostFrequentWords.Add(result2.Count());
                    MostFrequentWords.Add(s);
                }

                //Console.WriteLine(text);
                //char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '<', '>', '?', '.', '!', '[', ']', '{', '}' };
                //string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
           // }

        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="filename"></param>

        public void adjacentWordsForMostFrequents(Uri filename, out List<string> MostFrequentWords, out  List<string> wordsBeside1, List<string> wordsBeside2, List<string> wordsBeside3, List<string> wordsBeside4, List<string> wordsBeside5)
        {

            List<string> stopWord = new List<string>(new string[] { "!", "$", "%", "&", ",", "-", ".", "0", "1", "10", "100", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2", "20", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "21", "22", "23", "24", "25", "26", "27", "28", "29", "3", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "4", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "5", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "6", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "7", "70", "71", "72", "73", "74", "75", "76", "77", "78", "8", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "9", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", ":", ";", "<", ">", "@", "Ap.", "Apr.", "GHz", "MHz", "USD", "a", "ab", "abbia", "abbiamo", "abbiano", "abbiate", "aber", "abgerufen", "abgerufene", "abgerufener", "abgerufenes", "about", "above", "acht", "across", "ad", "afin", "after", "afterwards", "again", "against", "agl", "agli", "ah", "ai", "aie", "aient", "aies", "ait", "al", "alguna", "algunas", "alguno", "algunos", "algun", "all", "alla", "alle", "allein", "allem", "allen", "aller", "allerdings", "allerlei", "alles", "allgemein", "allmahlich", "allo", "allzu", "almost", "alone", "along", "alors", "already", "als", "alsbald", "also", "although", "always", "am", "ambos", "among", "amongst", "amoungst", "amount", "ampleamos", "an", "anche", "and", "ander", "andere", "anderem", "anderen", "anderer", "andererseits", "anderes", "anderm", "andern", "andernfalls", "anders", "anerkannt", "anerkannte", "anerkannter", "anerkanntes", "anfangen", "anfing", "angefangen", "angesetze", "angesetzt", "angesetzten", "angesetzter", "another", "ansetzen", "anstatt", "ante", "antes", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "apres", "aquel", "aquellas", "aquellos", "aqui", "arbeiten", "are", "around", "arriba", "as", "at", "atras", "attendu", "au", "au-dela", "au-devant", "auch", "aucun", "aucune", "audit", "auf", "aufgehort", "aufgrund", "aufhoren", "aufhorte", "aufzusuchen", "aupres", "auquel", "aura", "aurai", "auraient", "aurais", "aurait", "auras", "aurez", "auriez", "aurions", "aurons", "auront", "aus", "ausdrucken", "ausdruckt", "ausdruckte", "ausgenommen", "ausser", "ausserdem", "aussi", "author", "autor", "autour", "autre", "autres", "autrui", "aux", "auxdites", "auxdits", "auxquelles", "auxquels", "au?en", "au?er", "au?erdem", "au?erhalb", "avaient", "avais", "avait", "avant", "avec", "avemmo", "avendo", "avesse", "avessero", "avessi", "avessimo", "aveste", "avesti", "avete", "aveva", "avevamo", "avevano", "avevate", "avevi", "avevo", "avez", "aviez", "avions", "avons", "avrai", "avranno", "avrebbe", "avrebbero", "avrei", "avremmo", "avremo", "avreste", "avresti", "avrete", "avra", "avro", "avuta", "avute", "avuti", "avuto", "ayant", "ayez", "ayons", "b", "back", "bah", "bajo", "bald", "banco", "bastante", "be", "bearbeite", "bearbeiten", "bearbeitete", "bearbeiteten", "because", "bedarf", "bedurfte", "bedurfen", "been", "before", "beforehand", "befragen", "befragte", "befragten", "befragter", "begann", "beginnen", "begonnen", "behalten", "behielt", "bei", "beide", "beiden", "beiderlei", "beides", "beim", "beinahe", "being", "beitragen", "beitrugen", "bekannt", "bekannte", "bekannter", "bekennen", "ben", "benutzt", "bereits", "berichten", "berichtet", "berichtete", "berichteten", "beside", "besides", "besonders", "besser", "bestehen", "besteht", "betrachtlich", "between", "bevor", "bezuglich", "bien", "bietet", "bin", "bis", "bisher", "bislang", "bist", "bleiben", "blieb", "bloss", "blo?", "both", "bottom", "brachte", "brachten", "brauchen", "braucht", "bringen", "brauchte", "bsp.", "but", "by", "bzw", "be", "boden", "c", "c'", "c'est", "c'etait", "ca.", "cada", "call", "can", "cannot", "cant", "car", "ce", "ceci", "cela", "celle", "celle-ci", "celle-la", "celles", "celles-ci", "celles-la", "celui", "celui-ci", "celui-la", "cela", "cent", "cents", "cependant", "certain", "certaine", "certaines", "certains", "ces", "cet", "cette", "ceux", "ceux-ci", "ceux-la", "cf.", "cg", "cgr", "chacun", "chacune", "chaque", "che", "chez", "chi", "ci", "cierta", "ciertas", "ciertos", "cinq", "cinquante", "cinquante-cinq", "cinquante-deux", "cinquante-et-un", "cinquante-huit", "cinquante-neuf", "cinquante-quatre", "cinquante-sept", "cinquante-six", "cinquante-trois", "cl", "cm", "cm²", "co", "coi", "col", "come", "comme", "como", "con", "conseguimos", "conseguir", "consigo", "consigue", "consiguen", "consigues", "contre", "contro", "could", "couldnt", "cual", "cuando", "cui", "d", "d'", "d'apres", "d'un", "d'une", "da", "dabei", "dadurch", "dafur", "dagegen", "dagl", "dagli", "daher", "dahin", "dai", "dal", "dall", "dalla", "dalle", "dallo", "damals", "damit", "danach", "daneben", "dank", "danke", "danken", "dann", "dannen", "dans", "daran", "darauf", "daraus", "darf", "darfst", "darin", "darum", "darunter", "daruber", "daruberhinaus", "das", "dass", "dasselbe", "davon", "davor", "dazu", "da?", "de", "degl", "degli", "dei", "dein", "deine", "deinem", "deinen", "deiner", "deines", "del", "dell", "della", "delle", "dello", "dem", "demnach", "demselben", "den", "denen", "denn", "dennoch", "denselben", "dentro", "depuis", "der", "derart", "derartig", "derem", "deren", "derer", "derjenige", "derjenigen", "derriere", "derselbe", "derselben", "derzeit", "des", "desdites", "desdits", "deshalb", "desquelles", "desquels", "desselben", "dessen", "desto", "deswegen", "deux", "devant", "devers", "dg", "di", "dich", "did", "didn't", "die", "diejenige", "dies", "diese", "dieselbe", "dieselben", "diesem", "diesen", "dieser", "dieses", "diesseits", "differentes", "differents", "dinge", "dir", "direkt", "direkte", "direkten", "direkter", "divers", "diverses", "dix", "dix-huit", "dix-neuf", "dix-sept", "dl", "dm", "do", "doch", "does", "doesn't", "don't", "donc", "donde", "done", "dont", "doppelt", "dort", "dorther", "dorthin", "dos", "douze", "dov", "dove", "down", "drauf", "drei", "drei?ig", "drin", "dritte", "drunter", "druber", "du", "dudit", "due", "dunklen", "duquel", "durant", "durch", "durchaus", "durfte", "durften", "during", "des", "deja", "durfen", "durfte", "e", "each", "ebbe", "ebbero", "ebbi", "eben", "ebenfalls", "ebenso", "ed", "eg", "eh", "ehe", "eher", "eigenen", "eigenes", "eigentlich", "eight", "ein", "einbaun", "eine", "einem", "einen", "einer", "einerseits", "eines", "einfach", "einfuhren", "einfuhrte", "einfuhrten", "eingesetzt", "einig", "einige", "einigem", "einigen", "einiger", "einigerma?en", "einiges", "einmal", "eins", "einseitig", "einseitige", "einseitigen", "einseitiger", "einst", "einstmals", "einzig", "either", "el", "eleven", "ellas", "elle", "elles", "ellos", "else", "elsewhere", "empleais", "emplean", "emplear", "empleas", "empleo", "en", "en-dehors", "encima", "encore", "ende", "enfin", "enough", "entonces", "entre", "entsprechend", "entweder", "envers", "er", "era", "eramos", "eran", "erano", "eras", "eravamo", "eravate", "eres", "erganze", "erganzen", "erganzte", "erganzten", "erhalten", "erhielt", "erhielten", "erhalt", "eri", "erneut", "ero", "erst", "erste", "ersten", "erster", "eroffne", "eroffnen", "eroffnet", "eroffnete", "eroffnetes", "es", "essendo", "est", "esta", "estaba", "estado", "estais", "estamos", "estan", "estoy", "et", "etc", "etliche", "etwa", "etwas", "eu", "euch", "eue", "euer", "eues", "euh", "eure", "eurem", "euren", "eurent", "eurer", "eures", "eus", "eusse", "eussent", "eusses", "eussiez", "eussions", "eut", "eux", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "eumes", "eut", "eutes", "f", "faccia", "facciamo", "facciano", "facciate", "faccio", "facemmo", "facendo", "facesse", "facessero", "facessi", "facessimo", "faceste", "facesti", "faceva", "facevamo", "facevano", "facevate", "facevi", "facevo", "fai", "fait", "fall", "falls", "fand", "fanno", "farai", "faranno", "farebbe", "farebbero", "farei", "faremmo", "faremo", "fareste", "faresti", "farete", "fara", "faro", "fast", "fece", "fecero", "feci", "ferner", "few", "fi", "fifteen", "fify", "fill", "fin", "find", "finden", "findest", "findet", "fire", "first", "five", "flac", "folgende", "folgenden", "folgender", "folgendes", "folglich", "for", "fordern", "fordert", "forderte", "forderten", "former", "formerly", "fors", "fortsetzen", "fortsetzt", "fortsetzte", "fortsetzten", "forty", "fosse", "fossero", "fossi", "fossimo", "foste", "fosti", "found", "four", "fragte", "frau", "frei", "freie", "freier", "freies", "from", "front", "fu", "fue", "fuer", "fueron", "fui", "fuimos", "full", "fummo", "furent", "furono", "further", "fus", "fusse", "fussent", "fusses", "fussiez", "fussions", "fut", "fumes", "fut", "futes", "funf", "fur", "g", "gab", "ganz", "ganze", "ganzem", "ganzen", "ganzer", "ganzes", "gar", "gbr", "geb", "geben", "geblieben", "gebracht", "gedurft", "geehrt", "geehrte", "geehrten", "geehrter", "gefallen", "gefiel", "gefalligst", "gefallt", "gegeben", "gegen", "gehabt", "gehen", "geht", "gekommen", "gekonnt", "gemacht", "gemocht", "gemass", "genommen", "genug", "gern", "gesagt", "gesehen", "gestern", "gestrige", "get", "getan", "geteilt", "geteilte", "getragen", "gewesen", "gewisserma?en", "gewollt", "geworden", "ggf", "gib", "gibt", "give", "gleich", "gleichwohl", "gleichzeitig", "gli", "glucklicherweise", "gmbh", "go", "gr", "gratulieren", "gratuliert", "gratulierte", "gueno", "gute", "guten", "gangig", "gangige", "gangigen", "gangiger", "gangiges", "ganzlich", "h", "ha", "hab", "habe", "haben", "hace", "haceis", "hacemos", "hacen", "hacer", "haces", "had", "haette", "hago", "hai", "halb", "hallo", "han", "hanno", "has", "hasnt", "hast", "hat", "hatte", "hatten", "hattest", "hattet", "have", "he", "hein", "hem", "hence", "her", "heraus", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "heu", "heute", "heutige", "hg", "hier", "hiermit", "hiesige", "him", "himself", "hin", "hinein", "hinten", "hinter", "hinterher", "his", "hl", "hm", "hm³", "ho", "hoch", "hola", "hop", "hormis", "hors", "how", "however", "huit", "hum", "hundert", "hundred", "hatt", "hatte", "hatten", "he", "hochstens", "i", "ich", "ici", "ie", "if", "igitt", "ihm", "ihn", "ihnen", "ihr", "ihre", "ihrem", "ihren", "ihrer", "ihres", "il", "ils", "im", "immer", "immerhin", "important", "in", "inc", "incluso", "indeed", "indem", "indessen", "info", "infolge", "innen", "innerhalb", "ins", "insofern", "intenta", "intentais", "intentamos", "intentan", "intentar", "intentas", "intento", "into", "inzwischen", "io", "ir", "irgend", "irgendeine", "irgendwas", "irgendwen", "irgendwer", "irgendwie", "irgendwo", "is", "ist", "it", "its", "itself", "j", "j'", "j'ai", "j'avais", "j'etais", "ja", "jamais", "je", "jede", "jedem", "jeden", "jedenfalls", "jeder", "jederlei", "jedes", "jedoch", "jemand", "jene", "jenem", "jenen", "jener", "jenes", "jenseits", "jetzt", "jusqu'", "jusqu'au", "jusqu'aux", "jusqu'a", "jusque", "jahrig", "jahrige", "jahrigen", "jahriges", "k", "kam", "kann", "kannst", "kaum", "keep", "kein", "keine", "keinem", "keinen", "keiner", "keinerlei", "keines", "keineswegs", "kg", "klar", "klare", "klaren", "klares", "klein", "kleinen", "kleiner", "kleines", "km", "km²", "koennen", "koennt", "koennte", "koennten", "komme", "kommen", "kommt", "konkret", "konkrete", "konkreten", "konkreter", "konkretes", "konnte", "konnten", "konn", "konnen", "konnt", "konnte", "konnten", "kunftig", "l", "l'", "l'autre", "l'on", "l'un", "l'une", "la", "lag", "lagen", "langsam", "laquelle", "largo", "las", "lassen", "last", "latter", "latterly", "laut", "le", "least", "lediglich", "leer", "legen", "legte", "legten", "lei", "leicht", "leider", "lequel", "les", "lesen", "lesquelles", "lesquels", "less", "letze", "letzten", "letztendlich", "letztens", "letztes", "letztlich", "leur", "leurs", "lez", "li", "lichten", "liegt", "liest", "links", "lo", "loro", "lors", "lorsqu'", "lorsque", "los", "ltd", "lui", "langst", "langstens", "les", "m", "m'", "ma", "mache", "machen", "machst", "macht", "machte", "machten", "made", "mag", "magst", "maint", "mainte", "maintes", "maints", "mais", "mal", "malgre", "man", "manche", "manchem", "manchen", "mancher", "mancherorts", "manches", "manchmal", "mann", "many", "margin", "may", "me", "meanwhile", "mehr", "mehrere", "mein", "meine", "meinem", "meinen", "meiner", "meines", "meist", "meiste", "meisten", "mes", "meta", "mg", "mgr", "mi", "mia", "mich", "mie", "miei", "mientras", "might", "mil", "mill", "mille", "milliards", "millions", "mindestens", "mine", "mio", "mir", "mit", "mithin", "ml", "mm", "mm²", "mochte", "modo", "moi", "moins", "mon", "more", "moreover", "morgen", "morgige", "most", "mostly", "move", "moyennant", "mt", "much", "muchos", "muessen", "muesst", "muesste", "muss", "musst", "musste", "mussten", "must", "muy", "mu?", "mu?t", "my", "myself", "m²", "m³", "meme", "memes", "mochte", "mochten", "mochtest", "mogen", "moglich", "mogliche", "moglichen", "moglicher", "moglicherweise", "mussen", "musste", "mussten", "mu?t", "mu?te", "n", "n'avait", "n'y", "nach", "nachdem", "nacher", "nachhinein", "nacht", "nahm", "name", "namely", "naturlich", "ne", "neben", "nebenan", "negl", "negli", "nehmen", "nei", "nein", "neither", "nel", "nell", "nella", "nelle", "nello", "neu", "neue", "neuem", "neuen", "neuer", "neues", "neuf", "neun", "never", "nevertheless", "next", "ni", "nicht", "nichts", "nie", "niemals", "niemand", "nimm", "nimmer", "nimmt", "nine", "nirgends", "nirgendwo", "no", "nobody", "noch", "noi", "non", "nonante", "none", "nonobstant", "noone", "nor", "nos", "nosotros", "nostra", "nostre", "nostri", "nostro", "not", "nothing", "notre", "nous", "now", "nowhere", "nul", "nulle", "nun", "nur", "nutzen", "nutzt", "nutzung", "n?", "nachste", "namlich", "neanmoins", "notigenfalls", "nutzt", "o", "ob", "oben", "oberhalb", "obgleich", "obschon", "obwohl", "octante", "oder", "of", "off", "oft", "often", "oh", "ohne", "on", "once", "one", "only", "ont", "onto", "onze", "or", "other", "others", "otherwise", "otro", "ou", "our", "ours", "ourselves", "out", "outre", "over", "own", "ou", "p", "par", "par-dela", "para", "parbleu", "parce", "parmi", "part", "pas", "passe", "pendant", "per", "perche", "perhaps", "pero", "personne", "peu", "pfui", "piu", "please", "plus", "plus_d'un", "plus_d'une", "plusieurs", "plotzlich", "podeis", "podemos", "poder", "podria", "podriais", "podriamos", "podrian", "podrias", "por", "por que", "porque", "pour", "pourquoi", "pourtant", "pourvu", "primero desde", "pro", "pres", "puede", "pueden", "puedo", "puisqu'", "puisque", "put", "q", "qu", "qu'", "qu'elle", "qu'elles", "qu'il", "qu'ils", "qu'on", "quale", "quand", "quant", "quanta", "quante", "quanti", "quanto", "quarante", "quarante-cinq", "quarante-deux", "quarante-et-un", "quarante-huit", "quarante-neuf", "quarante-quatre", "quarante-sept", "quarante-six", "quarante-trois", "quatorze", "quatre", "quatre-vingt", "quatre-vingt-cinq", "quatre-vingt-deux", "quatre-vingt-dix", "quatre-vingt-dix-huit", "quatre-vingt-dix-neuf", "quatre-vingt-dix-sept", "quatre-vingt-douze", "quatre-vingt-huit", "quatre-vingt-neuf", "quatre-vingt-onze", "quatre-vingt-quatorze", "quatre-vingt-quatre", "quatre-vingt-quinze", "quatre-vingt-seize", "quatre-vingt-sept", "quatre-vingt-six", "quatre-vingt-treize", "quatre-vingt-trois", "quatre-vingt-un", "quatre-vingt-une", "quatre-vingts", "que", "quel", "quella", "quelle", "quelles", "quelli", "quello", "quelqu'", "quelqu'un", "quelqu'une", "quelque", "quelques", "quelques-unes", "quelques-uns", "quels", "questa", "queste", "questi", "questo", "qui", "quiconque", "quien", "quinze", "quoi", "quoiqu'", "quoique", "r", "rather", "re", "reagiere", "reagieren", "reagiert", "reagierte", "rechts", "regelma?ig", "revoici", "revoila", "rief", "rien", "rund", "s", "s'", "sa", "sabe", "sabeis", "sabemos", "saben", "saber", "sabes", "sage", "sagen", "sagt", "sagte", "sagten", "sagtest", "same", "sang", "sangen", "sans", "sarai", "saranno", "sarebbe", "sarebbero", "sarei", "saremmo", "saremo", "sareste", "saresti", "sarete", "sara", "saro", "sauf", "schlechter", "schlie?lich", "schnell", "schon", "schreibe", "schreiben", "schreibens", "schreiber", "schwierig", "schatzen", "schatzt", "schatzte", "schatzten", "se", "sechs", "sect", "see", "seem", "seemed", "seeming", "seems", "sehe", "sehen", "sehr", "sehrwohl", "seht", "sei", "seid", "sein", "seine", "seinem", "seinen", "seiner", "seines", "seit", "seitdem", "seite", "seiten", "seither", "seize", "selber", "selbst", "selon", "senke", "senken", "senkt", "senkte", "senkten", "sept", "septante", "ser", "sera", "serai", "seraient", "serais", "serait", "seras", "serez", "seriez", "serions", "serious", "serons", "seront", "ses", "setzen", "setzt", "setzte", "setzten", "several", "she", "should", "si", "sia", "siamo", "siano", "siate", "sich", "sicher", "sicherlich", "sie", "sieben", "siebte", "siehe", "sieht", "siendo", "siete", "sin", "since", "sind", "singen", "singt", "sinon", "six", "sixty", "so", "sobald", "sobre", "soda?", "soeben", "sofern", "sofort", "sog", "sogar", "soi", "soient", "sois", "soit", "soixante", "soixante-cinq", "soixante-deux", "soixante-dix", "soixante-dix-huit", "soixante-dix-neuf", "soixante-dix-sept", "soixante-douze", "soixante-et-onze", "soixante-et-un", "soixante-et-une", "soixante-huit", "soixante-neuf", "soixante-quatorze", "soixante-quatre", "soixante-quinze", "soixante-seize", "soixante-sept", "soixante-six", "soixante-treize", "soixante-trois", "solamente", "solange", "solc hen", "solch", "solche", "solchem", "solchen", "solcher", "solches", "soll", "sollen", "sollst", "sollt", "sollte", "sollten", "solltest", "solo", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "somit", "sommes", "somos", "son", "sondern", "sono", "sonst", "sonstwo", "sont", "sooft", "sous", "soviel", "soweit", "sowie", "sowohl", "soy", "soyez", "soyons", "spielen", "spater", "sta", "stai", "stando", "stanno", "starai", "staranno", "starebbe", "starebbero", "starei", "staremmo", "staremo", "stareste", "staresti", "starete", "startet", "startete", "starteten", "stara", "staro", "statt", "stattdessen", "stava", "stavamo", "stavano", "stavate", "stavi", "stavo", "steht", "steige", "steigen", "steigt", "stemmo", "stesse", "stessero", "stessi", "stessimo", "steste", "stesti", "stets", "stette", "stettero", "stetti", "stia", "stiamo", "stiano", "stiate", "stieg", "stiegen", "still", "sto", "su", "sua", "such", "suchen", "sue", "sugl", "sugli", "sui", "suis", "suite", "sul", "sull", "sulla", "sulle", "sullo", "suo", "suoi", "sur", "sus", "system", "samtliche", "t", "t'", "ta", "tacatac", "tages", "take", "tambien", "tandis", "tat", "tatsachlich", "tatsachlichen", "tatsachlicher", "tatsachliches", "tausend", "te", "teile", "teilen", "teilte", "teilten", "tel", "telle", "telles", "tels", "ten", "teneis", "tenemos", "tener", "tengo", "tes", "than", "that", "the", "thee", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thing", "third", "this", "those", "thou", "though", "three", "through", "throughout", "thru", "thus", "thy", "ti", "tiempo", "tiene", "tienen", "titel", "to", "todo", "together", "toi", "ton", "too", "total", "toujours", "tous", "tout", "toute", "toutefois", "toutes", "toward", "towards", "tra", "trabaja", "trabajais", "trabajamos", "trabajan", "trabajar", "trabajas", "trabajo", "trage", "tragen", "tras", "treize", "trente", "trente-cinq", "trente-deux", "trente-et-un", "trente-huit", "trente-neuf", "trente-quatre", "trente-sept", "trente-six", "trente-trois", "trois", "trotzdem", "trug", "tragt", "tres", "tu", "tua", "tue", "tun", "tuo", "tuoi", "tust", "tut", "tutti", "tutto", "tuyo", "twelve", "twenty", "two", "txt", "tat", "u", "ueber", "ultimo", "um", "umso", "un", "una", "unas", "unbedingt", "und", "under", "une", "unes", "ungefahr", "unmoglich", "unmogliche", "unmoglichen", "unmoglicher", "unnotig", "uno", "unos", "uns", "unse", "unsem", "unsen", "unser", "unsere", "unserem", "unseren", "unserer", "unseres", "unserm", "unses", "unten", "unter", "unterbrach", "unterbrechen", "unterhalb", "until", "unwichtig", "up", "upon", "us", "usa", "usais", "usamos", "usan", "usar", "usas", "uso", "usw", "v", "va", "vais", "valor", "vamos", "van", "vaya", "verdad", "verdadera cierto", "verdadero", "vergangen", "vergangene", "vergangener", "vergangenes", "vermag", "vermutlich", "vermogen", "verrate", "verraten", "verriet", "verrieten", "vers", "version", "versorge", "versorgen", "versorgt", "versorgte", "versorgten", "versorgtes", "very", "veroffentlichen", "veroffentlicher", "veroffentlicht", "veroffentlichte", "veroffentlichten", "veroffentlichtes", "vi", "via", "viel", "viele", "vielen", "vieler", "vieles", "vielleicht", "vielmals", "vier", "vingt", "vingt-cinq", "vingt-deux", "vingt-huit", "vingt-neuf", "vingt-quatre", "vingt-sept", "vingt-six", "vingt-trois", "vis-a-vis", "voi", "voici", "voila", "vollstandig", "vom", "von", "vor", "voran", "vorbei", "vorgestern", "vorher", "vorne", "voruber", "vos", "vosotras", "vosotros", "vostra", "vostre", "vostri", "vostro", "votre", "vous", "voy", "vollig", "w", "wachen", "waere", "wann", "war", "waren", "warst", "warum", "was", "we", "weder", "weg", "wegen", "weil", "weiter", "weitere", "weiterem", "weiteren", "weiterer", "weiteres", "weiterhin", "wei?", "welche", "welchem", "welchen", "welcher", "welches", "well", "wem", "wen", "wenig", "wenige", "weniger", "wenigstens", "wenn", "wenngleich", "wer", "werde", "werden", "werdet", "were", "weshalb", "wessen", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "wichtig", "wie", "wieder", "wieso", "wieviel", "wiewohl", "will", "willst", "wir", "wird", "wirklich", "wirst", "with", "within", "without", "wo", "wodurch", "wogegen", "woher", "wohin", "wohingegen", "wohl", "wohlweislich", "wolle", "wollen", "wollt", "wollte", "wollten", "wolltest", "wolltet", "womit", "woraufhin", "woraus", "worin", "would", "wurde", "wurden", "wahrend", "wahrenddessen", "war", "ware", "waren", "wurde", "wurden", "x", "y", "yet", "yo", "you", "your", "yours", "yourself", "yourselves", "z", "z.B.", "zahlreich", "zehn", "zeitweise", "ziehen", "zieht", "zog", "zogen", "zu", "zudem", "zuerst", "zufolge", "zugleich", "zuletzt", "zum", "zumal", "zur", "zuruck", "zusammen", "zuviel", "zwanzig", "zwar", "zwei", "zwischen", "zwolf", "zero", "|", "a", "ahnlich", "c'", "ca", "e", "es", "etaient", "etais", "etait", "etant", "etiez", "etions", "ete", "etee", "etees", "etes", "etes", "etre", "o", "ubel", "uber", "uberall", "uberallhin", "uberdies", "ubermorgen", "ubrig", "ubrigens", "(", ")", "*", "+", "?", "[", "]", "^", "{", "}" });



            MostFrequentWords = new List<string>();
            wordsBeside1 = new List<string>();
            wordsBeside2 = new List<string>();
            wordsBeside3 = new List<string>();
            wordsBeside4 = new List<string>();
            wordsBeside5 = new List<string>();
          


            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(filename);

            string webData = System.Text.Encoding.UTF8.GetString(raw);


            var result1 = Regex.Split(webData.ToLower(), @"\W+") //split the text with whitespace and make it lowercase
  .Where(s => s.Length > 3 && !stopWord.Contains(s)) //take the words that have length more than 3 and exclude the stopwords
  .GroupBy(s => s) //make a group for each word,for example the word assign will appear in only one group and will have 3 appearances in that group
  .OrderByDescending(g => g.Count()).Take(5); //decending order of the groups which are actually the words according to how many times that word appears, we take the first 10 words

                //the result gives us ten words
                  foreach (var v in result1)//for each word
                {
                

                    List<string> wordsBeside = new List<string>();

                   //we take the word s
                    string s = v.Key.ToString(); //v.key is the word


                    string[] words = Regex.Split(webData.ToLower(), @"\W+");


                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        if

                          (((words[i].Equals(s)) && (!wordsBeside.Contains(words[i + 1])))


                         )
                        {

                            if (i == 0) { wordsBeside1.Add(words[i + 1]); }
                            else if (i == 1) { wordsBeside2.Add(words[i + 1]); }
                            else if (i == 2) { wordsBeside3.Add(words[i + 1]); }
                            else if (i == 3) { wordsBeside4.Add(words[i + 1]); }
                            else if (i == 4) { wordsBeside5.Add(words[i + 1]); }


                            wordsBeside.Add(words[i + 1]);
                            Console.WriteLine("The adjacent word of {0} are {1}", s, words[i + 1]);
                        }

                        
                    }

                }

            

        }











	}
}