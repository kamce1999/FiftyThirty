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
        private static string dataPath = "C:\\code\\breadcrumb";
        
		public static void Main(string[] args)
		{
		    try
		    {
		        var reader = new LavuReader();

		        var orders = reader.GetTable<Orders>("orders", "closed").Result.Select(s => s.row).ToList();
		        WriteData(orders);

                var punches = reader.GetTable<ClockPunches>("clock_punches", "time").Result.Where(r => r.row.PunchedOut == 1).Select(s => s.row).ToList();
                punches.ForEach(p => p.DayOfWeek = p.Time.DayOfWeek);
		        WriteData(punches);

                var classes = reader.GetTable<EmployeeClasses>("emp_classes", null).Result.Select(s => s.row).ToList();

                var serverHours = new List<ServerHours>();
		        foreach (var item in punches)
		        {
		            var server = serverHours.FirstOrDefault(s => s.ServerId == item.ServerId);
		            if (server == null)
		            {
		                server = new ServerHours
		                {
		                    ServerId = item.ServerId,
                            EmployeeName = item.Server,
		                    Position = classes.FirstOrDefault(c => c.Id == item.RoleId)?.Title ?? item.RoleId.ToString(),
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

		        WriteData(serverHours);
                
                Helper.GetData(serverHours);
		    }
		    catch (Exception ex)
		    {
		        Console.WriteLine(ex.ToString());
		    }


		    Console.WriteLine("Done!");
			Console.ReadLine();
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
