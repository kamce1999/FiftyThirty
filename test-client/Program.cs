using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fifty.Lavu;
using Fifty.Shared;
using Fifty.Smartsheet;
using Newtonsoft.Json;

namespace test_client
{
	public class Program
	{
		private static string dataPath = "C:\\code\\breadcrumb";

		private static string location = string.Empty;


		public static void Main(string[] args)
		{
			try
			{

				Console.WriteLine("updating peel tip out......");
				location = "peel";
				DoWork(GetLavuHeader(ConfigurationVariables.LavuConfigPeel), ConfigurationVariables.SheetIdPeel);

				location = "fifty";
				Console.WriteLine("updating 5030 tip out......");
				DoWork(GetLavuHeader(ConfigurationVariables.LavuConfigFifty), ConfigurationVariables.SheetId5030);

				Console.WriteLine("done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Read();
			}
		}

		public static void DoWork(LavuApiHeaderValues headerValues, long sheetId)
		{
			var reader = new LavuReader(DateTime.Now.AddDays(-1));
			
			var classes = reader.GetTable<EmployeeClasses>(headerValues, "emp_classes", null).Result.Select(s => s.row).ToList();
			var orders = reader.GetTable<Orders>(headerValues, "orders", "closed").Result.Select(s => s.row).ToList();
			var punches = reader.GetTable<ClockPunches>(headerValues, "clock_punches", "time").Result.Where(r => r.row.PunchedOut == 1).Select(s => s.row).ToList();
			
			punches.ForEach(p => p.DayOfWeek = p.Time.DayOfWeek);

			var serverHours = GetServerSummary(punches, classes);

			var orderSummary = GetOrderSummary(orders);

			TipoutUpdater.Update(sheetId, serverHours, orderSummary);

			//WriteData(orderSummary);
			WriteData(orders);
		}

		private static LavuApiHeaderValues GetLavuHeader(string encodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(encodedData);

			var decoded = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

			return JsonConvert.DeserializeObject<LavuApiHeaderValues>(decoded);
		}
		
		private static List<SalesSummary> GetOrderSummary(IEnumerable<Order> orders)
		{
			return (from order in orders
					where order.Void == 0
					group order by order.ClosedDate.DayOfWeek into orderGroup
					select new SalesSummary
					{
						DayOfWeek = orderGroup.Key,
						ServiceFee = orderGroup.Sum(x => x.Gratuity),
						AutoGratuityTotal = orderGroup.Sum(x => x.AutoGratuityCard + x.AutoGratuityCash + x.AutoGratuityOther),
						NetSales = orderGroup.Sum(x => x.Subtotal - x.Discount),
					}).ToList();
		}

		private static List<ServerHours> GetServerSummary(List<ClockPunch> punches, List<EmployeeClass> classes)
		{
			var serverHours = new List<ServerHours>();
			foreach (var item in punches)
			{
				var server = serverHours.FirstOrDefault(s => s.ServerId == item.ServerId && s.RoleId == item.RoleId);
				if (server == null)
				{
					server = new ServerHours
					{
						ServerId = item.ServerId,
						EmployeeName = item.Server,
						Position = classes.FirstOrDefault(c => c.Id == item.RoleId)?.Title ?? item.RoleId.ToString(),
						RoleId = item.RoleId,
						PayRate = item.Payrate
					};

					serverHours.Add(server);
				}

				switch (item.Time.DayOfWeek)
				{
					case DayOfWeek.Monday:
						server.Monday += item.Hours;
						break;
					case DayOfWeek.Tuesday:
						server.Tuesday += item.Hours;
						break;
					case DayOfWeek.Wednesday:
						server.Wednesday += item.Hours;
						break;
					case DayOfWeek.Thursday:
						server.Thursday += item.Hours;
						break;
					case DayOfWeek.Friday:
						server.Friday += item.Hours;
						break;
					case DayOfWeek.Saturday:
						server.Saturday += item.Hours;
						break;
					case DayOfWeek.Sunday:
						server.Sunday += item.Hours;
						break;
				}
			}

			return serverHours;
		}

		#region debugstuff
		private static void WriteData<T>(ICollection<T> data)
		{
			var type = typeof(T);
			var path = Path.Combine(dataPath, $"{type.Name}_{location}_{DateTime.Now.Ticks}.csv");
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
		#endregion
	}
}
