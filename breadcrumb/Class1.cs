using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace breadcrumb
{
    public class Downloader
    {
	    public void GetData()
	    {
		    using (var httpClient = new HttpClient())
		    {
			    httpClient.DefaultRequestHeaders.Add("x-breadcrumb-api-key", "f095504eab9072f6c525f0308742190d");
			    httpClient.DefaultRequestHeaders.Add("x-breadcrumb-username", "api@peel");
			    httpClient.DefaultRequestHeaders.Add("x-breadcrumb-password", "75210934");

			    Client x = new Client(httpClient);

			    var categories = x.CategoriesGet(500, 0).Result;
				
				Console.ReadLine();
		    }
		}
    }
}
