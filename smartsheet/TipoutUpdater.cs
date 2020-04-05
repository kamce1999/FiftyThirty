using System;
using System.Collections.Generic;
using System.Linq;
using Fifty.Shared;
using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace Fifty.Smartsheet
{
	public static class TipoutUpdater
	{
		private static Dictionary<string, long> logColumnIdMap = new Dictionary<string, long>();
		private static Dictionary<string, long> columnIdMap = new Dictionary<string, long>();
		private static Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();
		private static Sheet tipOutSheet;
		private static Sheet logSheet;
		private static SmartsheetClient smartSheet;
		private static long tipOutSheetId;
		private static long sheetIdLog;
		private static string accessToken;

		public static void Update(List<ServerHours> serverHours, List<SalesSummary> salesSummary, bool switchWeeks)
		{
			try
			{
				accessToken = ConfigurationVariables.SmartSheetToken;
				
				tipOutSheetId = ConfigurationVariables.SheetIdPeel;
								
				smartSheet = new SmartsheetBuilder().SetAccessToken(accessToken).Build();

				InitializeLogSheet();

				InitializeTipOutSheet();

                CopyLastWeek(switchWeeks);

				UpdateServerHours(serverHours);

				UpdateDailySales(salesSummary);

				LogMessage(LogType.Info, $"Processing complete for: {tipOutSheet.Name}");
			}
			catch (Exception ex)
			{
				LogMessage(LogType.Error, ex.Message);
			}
		}

        public static void CopyLastWeek(bool switchWeeks)
        {
            if (!switchWeeks)
            {
                return;
            }

            var lastMonday = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);

            var destination = new ContainerDestination
            {
                DestinationId = 3725417489164164,
                DestinationType = DestinationType.FOLDER,
                NewName = $"Tip Out Model {lastMonday.ToShortDateString()} - {lastMonday.AddDays(6).ToShortDateString()}"
            };
        
            smartSheet.SheetResources.CopySheet(tipOutSheetId, destination, new List<SheetCopyInclusion> { SheetCopyInclusion.ALL });
        }

        private static void InitializeLogSheet()
		{
			sheetIdLog = ConfigurationVariables.SheetIdLog;

			logSheet = smartSheet.SheetResources.GetSheet(sheetIdLog, null, null, null, null, null, null, null);
			logColumnIdMap = new Dictionary<string, long>();

			foreach (var column in logSheet.Columns)
			{
				if (column.Id.HasValue)
				{
					logColumnIdMap.Add(column.Title, column.Id.Value);
				}
			}
		}

		private static void InitializeTipOutSheet()
		{
			tipOutSheet = smartSheet.SheetResources.GetSheet(tipOutSheetId, new[] { SheetLevelInclusion.FORMAT }, null, null, null, null, null, null);

			columnIdMap = new Dictionary<string, long>();
			columnIndexMap = new Dictionary<string, int>();

			foreach (var column in tipOutSheet.Columns)
			{
				if (column.Id.HasValue)
				{
					columnIdMap.Add(column.Title, column.Id.Value);
				}

				if (column.Index.HasValue)
				{
					columnIndexMap.Add(column.Title, column.Index.Value);
				}
			}
		}

		private static void UpdateDailySales(IEnumerable<SalesSummary> salesSummary)
		{
			var salesRow = GetRow(ColumnNames.EmployeeName, "Daily Sales");
			var serviceFeeRow = GetRow(ColumnNames.EmployeeName, "Service Fee");
			var sales = GetEmptyCellValues().ToDictionary(k => k.ColumnId);
			var serviceFee = GetEmptyCellValues().ToDictionary(k => k.ColumnId);
			
			foreach (var daySummary in salesSummary)
			{
				switch (daySummary.DayOfWeek)
				{
					case DayOfWeek.Monday:
						sales[columnIdMap[ColumnNames.Monday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Monday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Tuesday:
						sales[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Wednesday:
						sales[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Thursday:
						sales[columnIdMap[ColumnNames.Thursday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Thursday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Friday:
						sales[columnIdMap[ColumnNames.Friday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Friday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Saturday:
						sales[columnIdMap[ColumnNames.Saturday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Saturday]].Value = daySummary.ServiceFee;
						break;
					case DayOfWeek.Sunday:
						sales[columnIdMap[ColumnNames.Sunday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Sunday]].Value = daySummary.ServiceFee;
						break;
				}
			}

			var rowsToUpdate = new[]
			{
				new Row { Id = salesRow.Id, Cells = sales.Values.ToList() },
				new Row { Id = serviceFeeRow.Id, Cells = serviceFee.Values.ToList() }
			};

			smartSheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, rowsToUpdate);
		}
		
		private static void UpdateServerHours(IEnumerable<ServerHours> serverHours)
		{
			var employeeRows = GetEmployeeRows(out var parentRowId);

			ClearEmployeeValues(employeeRows);

			foreach (var server in serverHours.OrderBy(it => it.ServerId).ToList())
			{
				if (server.ServerId <= 0)
				{
					LogMessage(LogType.Error, $"Invalid record! ServerId can't be zero ${JsonConvert.SerializeObject(server)}");
					continue;
				}

				var cells = GetCellValues(server);

				var row = employeeRows.FirstOrDefault(
					r => Convert.ToInt32(r.Cells[columnIndexMap[ColumnNames.ServerId]].Value) == server.ServerId
						&& r.Cells[columnIndexMap[ColumnNames.Position]].Value.ToString() == server.Position);

				if (row != null)
				{
					var rowToUpdate = new Row { Id = row.Id, Cells = cells };
					smartSheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, new[] { rowToUpdate });
				}
				else
				{
					var newRow = new Row { ParentId = parentRowId, Cells = cells };
					smartSheet.SheetResources.RowResources.AddRows(tipOutSheetId, new[] { newRow });
				}
			}
		}

		private static void ClearEmployeeValues(IEnumerable<Row> employeeRows)
		{
			var rowsToUpdate = employeeRows
				.Select(row => new Row { Id = row.Id, Cells = GetEmptyCellValues() })
				.ToList();

			if (rowsToUpdate.Count > 0)
			{
				smartSheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, rowsToUpdate);
			}
		}

		private static List<Cell> GetEmptyCellValues()
		{
			return new List<Cell>
				{
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Monday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Tuesday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Wednesday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Thursday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Friday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Saturday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Sunday] },
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.AdditionalTips] } 
				};
		}

		private static List<Cell> GetCellValues(ServerHours server)
		{
            var cells = new List<Cell>
				{
					new Cell { Value = server.ServerId, ColumnId = columnIdMap[ColumnNames.ServerId] },
					new Cell { Value = server.EmployeeName, ColumnId = columnIdMap[ColumnNames.EmployeeName] },
					new Cell { Value = server.Position, ColumnId = columnIdMap[ColumnNames.Position] },
					new Cell { Value = server.PayRate, ColumnId = columnIdMap[ColumnNames.PayRate] }
				};

            if (server.AdditionalTips > 0)
            {
                cells.Add(new Cell { Value = server.AdditionalTips, ColumnId = columnIdMap[ColumnNames.AdditionalTips] });
            }

            var totalHours = 0F;
            
            foreach (var dailyHours in server.Hours.OrderBy(h => h.DayOfWeek))
            {
                var adjusted = GetMaxHours(totalHours, dailyHours.Hours, server.PayRate);

                totalHours += adjusted;

                cells.Add(new Cell { Value = adjusted, ColumnId = GetColumnId(dailyHours.DayOfWeek) });
            }
            return cells;
		}

        private static long GetColumnId(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return columnIdMap[ColumnNames.Friday];
                case DayOfWeek.Monday:
                    return columnIdMap[ColumnNames.Monday];
                case DayOfWeek.Saturday:
                    return columnIdMap[ColumnNames.Saturday];
                case DayOfWeek.Sunday:
                    return columnIdMap[ColumnNames.Sunday];
                case DayOfWeek.Thursday:
                    return columnIdMap[ColumnNames.Thursday];
                case DayOfWeek.Tuesday:
                    return columnIdMap[ColumnNames.Tuesday];
                case DayOfWeek.Wednesday:
                    return columnIdMap[ColumnNames.Wednesday];
                default:
                    throw new ArgumentOutOfRangeException(nameof(day), day, null);
            }
        }

		internal static float GetMaxHours(float totalHours, float value, float payRate)
		{
			if (payRate > 0)
			{
				return value;

			}

            if (totalHours + value <= 45)
            {
                return value > 8 ? 8 : value;
            }

            var remainingHours = 45 - totalHours;

            return (remainingHours <= 45 && remainingHours >= 0) ? remainingHours : 0;
        }

		private static List<Row> GetEmployeeRows(out long parentRowId)
		{
			var beginRow = GetRow(ColumnNames.EmployeeName, "Employee Name");
			var endRow = GetRow(ColumnNames.EmployeeName, "Salary Employees");

			if (beginRow == null || endRow == null)
			{
				throw new Exception("Could not find Employee Rows");
			}

			parentRowId = beginRow.Id ?? -1;

			return tipOutSheet.Rows.Where(r => r.RowNumber > beginRow.RowNumber && r.RowNumber < endRow.RowNumber).Select(r => r).ToList();
		}

		private static Row GetRow(string columnName, string filter)
		{
			return (from row in tipOutSheet.Rows
					let cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == columnIdMap[columnName])?.Value
					where (cellValue ?? string.Empty).ToString() == filter
					select row).FirstOrDefault();
		}

		private static void LogMessage(LogType type, string message)
		{
			var newRow = new Row
			{
				ToTop = true,
				Cells = new List<Cell>
				{
					new Cell { ColumnId = logColumnIdMap[LogColumnNames.LogType], Value = type.ToString() },
					new Cell { ColumnId = logColumnIdMap[LogColumnNames.LogDate], Value = DateTime.Now },
					new Cell { ColumnId = logColumnIdMap[LogColumnNames.Sheet], Value = tipOutSheet.Name },
					new Cell { ColumnId = logColumnIdMap[LogColumnNames.Message], Value = message },
				}
			};

			smartSheet.SheetResources.RowResources.AddRows(sheetIdLog, new[] { newRow });
		}
    }
}