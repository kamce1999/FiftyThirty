using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using breadcrumb;

using Fifty.Lavu;
using Fifty.Smartsheet;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace test_client
{
	public class Program
	{
		private const string BaseUrl = "https://api.breadcrumb.com/ws/v2";
	    
        private static string dataPath = "C:\\code\\breadcrumb";

		private static List<string> tradingDayList = new List<string>();
        
		public static void Main(string[] args)
		{
			var reader = new LavuReader();

			var punches = reader.GetTable<ClockPunches>("clock_punches", "time").Result.Select(s => s.row).ToList();
			WriteData(punches);

			var orders = reader.GetTable<Orders>("orders", "closed").Result.Select(s => s.row).ToList();
		    WriteData(orders);



            Console.WriteLine("Done!");
			Console.ReadLine();
		}

		public static void MainOld(string[] args)
		{
			try
			{
				dataPath = Path.Combine(dataPath, DateTime.Now.ToString("yyyyMMdd HH.mm.ss"));
				Directory.CreateDirectory(dataPath);
				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Add("x-breadcrumb-api-key", "f095504eab9072f6c525f0308742190d");
					httpClient.DefaultRequestHeaders.Add("x-breadcrumb-username", "api@peel");
					httpClient.DefaultRequestHeaders.Add("x-breadcrumb-password", "75210934");

					GetData(httpClient, "categories.json");
					GetData(httpClient, "items.json");
					GetData(httpClient, "modifiers.json");
					GetData(httpClient, "tax.json");
					GetData(httpClient, "zones.json");
					GetData(httpClient, "employees.json");
					GetTradingDays(httpClient);
					GetData(httpClient, "day_parts.json");
					GetData(httpClient, "checks.json", "start_date=20160101&end_date=20190315", true);
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
				Console.ReadLine();
			}
		}
        
		public static void GetData(HttpClient client, string resource, string additionalQuery = null, bool chunkData = false)
		{
			try
			{
				var url = string.IsNullOrEmpty(additionalQuery)
					? $"{BaseUrl}/{resource}?limit=500&offset=0"
					: $"{BaseUrl}/{resource}?{additionalQuery}&limit=250&offset=0";

				var requestUri = new Uri(url);


				Console.WriteLine($"getting data for {resource}");

				var moreData = true;

				var current = 0;

				do
				{
					current++;

					var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
					request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

					var response = client.SendAsync(request).Result;

					if (response.IsSuccessStatusCode)
					{
						var x = JsonConvert.DeserializeObject<ResponseData>(response.Content.ReadAsStringAsync().Result);

						Console.WriteLine($"\t {x.Meta.Offset} of {x.Meta.Total_count}");

						if (x.Objects != null)
						{
							foreach (var item in x.Objects)
							{
								WriteJson(resource, current, JsonConvert.SerializeObject(item));
							}
						}

						if (x.Meta.Next == null)
						{
							moreData = false;
							continue;
						}

						requestUri = new Uri(x.Meta.Next);
					}
					else
					{
						var error = response.Content.ReadAsStringAsync().Result;
						throw new Exception(error);
					}
				}
				while (moreData);
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}

		public static void GetTimesheets(HttpClient client)
		{
			try
			{
				Console.WriteLine($"getting data for trading day dependent objects");

				int counter = 1;
				foreach (var id in tradingDayList)
				{
					Console.WriteLine($"\t trading day {counter}");
					WriteChildData(client, "timesheets.json", id);
					WriteChildData(client, "refunds.json", id);
					WriteChildData(client, "payment_auth.json", id);
					WriteChildData(client, "cashier_payments.json", id);
					counter++;
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}

		public static void WriteChildData(HttpClient client, string resource, string tradingDayId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{BaseUrl}/{resource}?trading_day={tradingDayId}"));
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
			var response = client.SendAsync(request).Result;

			if (response.IsSuccessStatusCode)
			{
				var x = JsonConvert.DeserializeObject<ResponseData>(response.Content.ReadAsStringAsync().Result);

				if (x.Objects != null && x.Objects.Count > 0)
				{
					foreach (var item in x.Objects)
					{
						WriteJson(resource, 1, JsonConvert.SerializeObject(item));
					}
					Console.WriteLine($"\t\t{resource}: {x.Objects.Count}");
				}
			}
			else
			{
				var error = response.Content.ReadAsStringAsync().Result;
				throw new Exception(error);
			}
		}

		public static void GetTradingDays(HttpClient client, string additionalQuery = null)
		{
			try
			{

				var requestUri = new Uri($"{BaseUrl}/trading_days.json?start=20160201limit=500&offset=0");

				Console.WriteLine($"getting data for trading_days.json");

				var moreData = true;

				var current = 0;

				do
				{
					current++;

					var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
					request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

					var response = client.SendAsync(request).Result;

					if (response.IsSuccessStatusCode)
					{
						var x = JsonConvert.DeserializeObject<ResponseData>(response.Content.ReadAsStringAsync().Result);

						Console.WriteLine($"\t {x.Meta.Offset} of {x.Meta.Total_count}");

						if (x.Objects != null)
						{
							foreach (var item in x.Objects)
							{
								WriteJson("trading_day.json", current, JsonConvert.SerializeObject(item));
								tradingDayList.Add(item.GetValue("id").ToString());
							}
						}

						if (x.Meta.Next == null)
						{
							moreData = false;
							continue;
						}

						requestUri = new Uri(x.Meta.Next);
					}
					else
					{
						var error = response.Content.ReadAsStringAsync().Result;
						throw new Exception(error);
					}
				}
				while (moreData);

				GetTimesheets(client);
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}
        
		private static void WriteData<T>(ICollection<T> data)
		{
			var type = typeof(T);
			var path = Path.Combine(dataPath, $"{type.Name}_{DateTime.Now.Ticks}.csv");
			if (!File.Exists(path))
			{
				var properties = type.GetProperties().AsQueryable().Select(p => p.Name);
				WriteCsvLine(path, properties.ToArray());
			}

			foreach (var item in data)
			{
				var line = item.GetType().GetProperties().AsQueryable().Select(p => p.GetValue(item) == null ? string.Empty : p.GetValue(item).ToString()).ToArray();
				WriteCsvLine(path, line);
			}
		}

		private static void WriteCsvLine(string path, string[] values)
		{
			File.AppendAllLines(path, new[] { string.Join(",", values) });
		}

		private static void WriteJson(string resource, int counter, string values)
		{
			File.AppendAllLines(Path.Combine(dataPath, $"{counter}-{resource}"), new[] { string.Join(",", values) });
		}
	}
}
