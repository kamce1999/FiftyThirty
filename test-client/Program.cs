using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fifty.Lavu;
using Fifty.Shared;
using Fifty.Smartsheet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace test_client
{
    public class Program
    {
        public static IConfigurationRoot configuration;
        private static bool overrideRunTimeCheck { get; set; }
        private static bool switchWeeks { get; set; }

        public static void Main(string[] args)
        {
            try
            {
				Console.WriteLine("updating peel tip out......");

				ConfigureServices();
                
                DoWork();

                Console.WriteLine("done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Read();
            }
        }


        private static void ConfigureServices()
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var sheetId = configuration.GetValue<string>("ConfigValues:SheetIdPeel");
        }

        public static void DoWork()
        {
			var headerValues = GetLavuHeader(ConfigurationVariables.LavuConfigPeel);
            
            var reader = new LavuReader(DateTime.Now.AddDays(-1));
            
            var classes = reader.GetTable<EmployeeClasses>(headerValues, "emp_classes", null).Result.Select(s => s.row).ToList();
            var orders = reader.GetTable<Orders>(headerValues, "orders", "closed").Result.Select(s => s.row).ToList();
            var punches = reader.GetTable<ClockPunches>(headerValues, "clock_punches", "time").Result.Where(r => r.row.PunchedOut == 1).Select(s => s.row).ToList();

            punches.ForEach(p => p.DayOfWeek = p.Time.DayOfWeek);

            var serverHours = GetServerSummary(punches, classes, orders);
            var orderSummary = GetOrderSummary(orders);

            TipoutUpdater.Update(serverHours, orderSummary, switchWeeks);
        }

        private static bool ValidRunTimes()
        {
	        var mstDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Mountain Standard Time");
			
			switch (mstDate.DayOfWeek)
	        {
				case DayOfWeek.Saturday:
				case DayOfWeek.Sunday:
			        return mstDate.Hour >= 12 && mstDate.Hour <= 23;
		        case DayOfWeek.Monday:
		        case DayOfWeek.Tuesday:
		        case DayOfWeek.Wednesday:
		        case DayOfWeek.Thursday:
		        case DayOfWeek.Friday:
					return mstDate.Hour >= 16 && mstDate.Hour <= 23;
				default:
			        throw new ArgumentOutOfRangeException();
	        }
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

        private static List<ServerHours> GetServerSummary(IList<ClockPunch> punches, IList<EmployeeClass> classes, IList<Order> orders)
        {
            var serverHours = new List<ServerHours>();
            foreach (var item in punches.OrderByDescending(x => x.Payrate))
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

                var dailyHours = server.Hours.FirstOrDefault(y => y.DayOfWeek == item.DayOfWeek);
                if (dailyHours == null)
                {
                    dailyHours = new DailyHours { DayOfWeek = item.DayOfWeek };
                    server.Hours.Add(dailyHours);
                }

                dailyHours.Hours += item.Hours;
            }

            foreach(var item in orders.GroupBy(x => x.ServerId).Select(x => new { ServerId = x.Key, AdditionalTips = x.Sum(y => y.CardGratuity) }))
            {
                var server = serverHours.FirstOrDefault(x => x.ServerId == item.ServerId);
                if (server != null)
                {
                    server.AdditionalTips = item.AdditionalTips;
                }
            }

            return serverHours;
        }

        #region debugstuff
        private static void WriteData<T>(ICollection<T> data)
        {
            var type = typeof(T);
            var path = Path.Combine(AppContext.BaseDirectory, $"{type.Name}_{DateTime.Now.Ticks}.csv");
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
