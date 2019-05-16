using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Fifty.Lavu
{
	public class LavuReader
	{
		private const string ApiUrl = "https://api.poslavu.com/cp/reqserv/";

        public async Task<IList<T>> GetTable<T>(string table, string filter)
		{
			var results = new List<T>();
			
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

				var uri = new Uri(ApiUrl);

				StartOfWeek(out var start, out var end);

				var data = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("dataname", "peel"),
					new KeyValuePair<string, string>("key", "h6yIkHNczEpwfNnH2wWL"),
					new KeyValuePair<string, string>("token", "xQYqce8EImAp2hsxn4TQ"),
					new KeyValuePair<string, string>("table", table),
					new KeyValuePair<string, string>("column", filter),
					new KeyValuePair<string, string>("value_min", start),
					new KeyValuePair<string, string>("value_max", end),
					new KeyValuePair<string, string>("valid_xml", "1"),
				};

                var content = new FormUrlEncodedContent(data);

				var response = client.PostAsync(uri, content).Result;

				response.EnsureSuccessStatusCode();

				var xml = await response.Content.ReadAsStringAsync();
				
				var doc = new XmlDocument();
				doc.LoadXml(xml);

				var rows = doc.SelectSingleNode("results")?.SelectNodes("row");
				if (rows == null)
				{
					return results;
				}

				foreach (XmlNode node in rows)
				{
					var json = JsonConvert.SerializeXmlNode(node);
					results.Add(JsonConvert.DeserializeObject<T>(json));
				}

				return results; 
			}
		}
        
		private static void StartOfWeek(out string start, out string end)
		{
			var date = DateTime.Now.Date;
			var diff = -(int)date.DayOfWeek + (int)DayOfWeek.Monday;
			var monday = date.AddDays(diff);

			end = monday.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
			start = monday.ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}
