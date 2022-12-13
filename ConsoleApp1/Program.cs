using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    //https://learn.microsoft.com/tr-tr/azure/private-link/create-private-endpoint-portal?tabs=dynamic-ip
    //https://learn.microsoft.com/en-us/azure/cognitive-services/cognitive-services-custom-subdomains;
    public class Program
    {
        const string subscriptionKey = "8361622b-6bd0-4e0a-b226-6fbe1051352a";
        const string uriBase = "https://eastasia.api.cognitive.microsoft.com";
        const string searchTerm = "tropical ocean";

        struct SearchResult
        {
            public String jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }
        static SearchResult BingImageSearch(string SearchTerm)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(SearchTerm);
            WebRequest request = WebRequest.Create(uriQuery);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //Create the result object for return
            var searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };

            // Extract Bing HHTP headers
            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingApIs-")||header.StartsWith("X-MSEdge-"))
                    searchResult.relevantHeaders[header] = response.Headers[header];
            }
            return searchResult;
        }
        static void Main(string[] args)
        {
            SearchResult result=BingImageSearch(searchTerm);
            //deserialize the json rom the bing Image Search Api
            dynamic jsonObj=Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

            var firstJsonObj = jsonObj["value"][0];
            Console.WriteLine("Title for the first image result: " + firstJsonObj["name"]+"\n");
            Console.WriteLine("Url for the first image result: " + firstJsonObj["webSearch"]+"\n");
        }
    }
}
