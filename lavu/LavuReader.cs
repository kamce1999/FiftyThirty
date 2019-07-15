using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Fifty.Lavu
{
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed. Suppression is OK here.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed. Suppression is OK here.")]
	public class LavuReader
	{
		private const string ApiUrl = "https://api.poslavu.com/cp/reqserv/";

		private readonly DateTime runDate;

		private string Start { get; set; }

		private string End { get; set; }

		public LavuReader(DateTime runDate)
		{
			this.runDate = runDate.Date;
		}

		public async Task<IList<T>> GetTable<T>(LavuApiHeaderValues headerValues, string table, string filter)
		{
			var results = new List<T>();
			var skip = 0;
			var take = 40;

			SetupDateRange();

			using (var client = new HttpClient())
			{
				var hasMoreData = false;

				do
				{
					var doc = await GetResponse(client, headerValues, table, filter, skip, take);

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

					hasMoreData = rows.Count == take;
					skip = skip + take;
				}
				while (hasMoreData);

				return results;
			}
		}

		private void SetupDateRange()
		{
			var dayOfWeek = runDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)runDate.DayOfWeek;
			var diff = -dayOfWeek + (int)DayOfWeek.Monday;
			var monday = runDate.AddDays(diff);

			End = monday.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
			Start = monday.ToString("yyyy-MM-dd HH:mm:ss");
		}

		private async Task<XmlDocument> GetResponse(HttpClient client, LavuApiHeaderValues headerValues, string table, string filter, int skip, int take)
		{
			var uri = new Uri(ApiUrl);

			var data = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("dataname", headerValues.DataName),
				new KeyValuePair<string, string>("key", headerValues.Key),
				new KeyValuePair<string, string>("token", headerValues.Token),
				new KeyValuePair<string, string>("table", table),
				new KeyValuePair<string, string>("value_min", Start),
				new KeyValuePair<string, string>("value_max", End),
				new KeyValuePair<string, string>("limit", $"{skip},{take}"),
				new KeyValuePair<string, string>("valid_xml", "1"),
			};

			if (!string.IsNullOrEmpty(filter))
			{
				data.Add(new KeyValuePair<string, string>("column", filter));
			}

			var content = new FormUrlEncodedContent(data);

			var response = client.PostAsync(uri, content).Result;

			response.EnsureSuccessStatusCode();

			var xml = await response.Content.ReadAsStringAsync();

			var doc = new XmlDocument();
			doc.LoadXml(xml);

			return doc;
		}
	}
}
