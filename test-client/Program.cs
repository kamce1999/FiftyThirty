using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fifty.Lavu;
using Fifty.Smartsheet;

namespace test_client
{
	public class Program
	{
		private static string dataPath = "C:\\code\\breadcrumb";

		private const long SheetId5030 = 8475254654822276;
		private const long SheetIdPeel = 1854968576665476;

		private static ApiHeaderValues peelHeaderValues = new ApiHeaderValues
		{
			DataName = "peel",
			Key = "h6yIkHNczEpwfNnH2wWL",
			Token = "xQYqce8EImAp2hsxn4TQ"
		};

		private static ApiHeaderValues fiftyHeaderValues = new ApiHeaderValues
		{
			DataName = "50_30_",
			Key = "XEiEDX2Ub2w6MmFafDkV",
			Token = "UJp8Dsx9par3RIatMbYA"
		};
		
		public static void Main(string[] args)
		{
			try
			{
				DoWork(peelHeaderValues, SheetIdPeel);
				DoWork(fiftyHeaderValues, SheetId5030);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Read();
			}
		}

		public static void DoWork(ApiHeaderValues headerValues, long sheetId)
		{
			var reader = new LavuReader(DateTime.Now.AddDays(-1));

			var classes = reader.GetTable<EmployeeClasses>(headerValues, "emp_classes", null).Result.Select(s => s.row).ToList();
			var orders = reader.GetTable<Orders>(headerValues, "orders", "closed").Result.Select(s => s.row).ToList();
			var punches = reader.GetTable<ClockPunches>(headerValues, "clock_punches", "time").Result.Where(r => r.row.PunchedOut == 1).Select(s => s.row).ToList();
			var orderSummary = GetOrderSummary(orders);

			punches.ForEach(p => p.DayOfWeek = p.Time.DayOfWeek);

			var serverHours = GetServerSummary(punches, classes);

			WriteData(punches);
			WriteData(orderSummary);
			WriteData(orders);
			WriteData(serverHours);

			Helper.GetData(sheetId, serverHours, orderSummary);
		}

		private static List<SalesSummary> GetOrderSummary(List<Order> orders)
		{
			return (from order in orders
					group order by order.ClosedDate.DayOfWeek into orderGroup
					select new SalesSummary
					{
						DayOfWeek = orderGroup.Key,
						ServiceFee = orderGroup.Sum(x => x.AutoGratuityCard + x.AutoGratuityCash + x.AutoGratuityOther),
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
	}
}
