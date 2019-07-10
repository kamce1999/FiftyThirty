using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace Fifty.Smartsheet
{
	public static class Helper
	{
		private const long SheetIdLog = 2900180946184068;
		private const string AccessToken = "lsazvdkpo5338ett4b2rpdrvm2";
		private static Dictionary<string, long> logColumnIdMap = new Dictionary<string, long>();
		private static Dictionary<string, long> columnIdMap = new Dictionary<string, long>();
		private static Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();
		private static Sheet tipOutSheet;
		private static Sheet logSheet;
		private static SmartsheetClient smartsheet;
		private static long tipOutSheetId;


		public static void GetData(long sheetId, List<ServerHours> serverHours, List<SalesSummary> salesSummary)
		{
			try
			{
				tipOutSheetId = sheetId;

				smartsheet = new SmartsheetBuilder().SetAccessToken(AccessToken).Build();

				InitializeLogSheet();

				InitializeTipeOutSheet();

				UpdateServerHours(serverHours);

				UpdateDailySales(salesSummary);

				LogMessage(LogType.Info, $"Processing complete for: {tipOutSheet.Name}");
			}
			catch (Exception ex)
			{
				LogMessage(LogType.Error, ex.Message);
			}
		}

		private static void InitializeLogSheet()
		{
			logSheet = smartsheet.SheetResources.GetSheet(SheetIdLog, null, null, null, null, null, null, null);
			logColumnIdMap = new Dictionary<string, long>();

			foreach (var column in logSheet.Columns)
			{
				if (column.Id.HasValue)
				{
					logColumnIdMap.Add(column.Title, column.Id.Value);
				}
			}
		}

		private static void InitializeTipeOutSheet()
		{
			tipOutSheet = smartsheet.SheetResources.GetSheet(tipOutSheetId, new[] { SheetLevelInclusion.FORMAT }, null, null, null, null, null, null);
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
			var adjustedFeeRow = GetRow(ColumnNames.EmployeeName, "Adjusted Fee");
			var sales = GetEmptyCellValues().ToDictionary(k => k.ColumnId);
			var serviceFee = GetEmptyCellValues().ToDictionary(k => k.ColumnId);
			var adjustedFee = GetEmptyCellValues().ToDictionary(k => k.ColumnId);

			var serviceFeeAddjustment = GetValueAsDouble(adjustedFeeRow.Cells[columnIndexMap[ColumnNames.PayRate]].Value);

			foreach (var daySummary in salesSummary)
			{
				switch (daySummary.DayOfWeek)
				{
					case DayOfWeek.Monday:
						sales[columnIdMap[ColumnNames.Monday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Monday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Monday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Tuesday:
						sales[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Wednesday:
						sales[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Thursday:
						sales[columnIdMap[ColumnNames.Thursday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Thursday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Thursday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Friday:
						sales[columnIdMap[ColumnNames.Friday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Friday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Friday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Saturday:
						sales[columnIdMap[ColumnNames.Saturday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Saturday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Saturday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
					case DayOfWeek.Sunday:
						sales[columnIdMap[ColumnNames.Sunday]].Value = daySummary.NetSales;
						serviceFee[columnIdMap[ColumnNames.Sunday]].Value = daySummary.ServiceFee;
						adjustedFee[columnIdMap[ColumnNames.Sunday]].Value = daySummary.ServiceFee * serviceFeeAddjustment;
						break;
				}
			}

			var rowsToUpdate = new[]
			{
				new Row { Id = salesRow.Id, Cells = sales.Values.ToList() },
				new Row { Id = serviceFeeRow.Id, Cells = serviceFee.Values.ToList() },
				new Row { Id = adjustedFeeRow.Id, Cells = adjustedFee.Values.ToList() }
			};

			smartsheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, rowsToUpdate);
		}

		private static double GetValueAsDouble(object value)
		{
			if (value != null && double.TryParse(value.ToString(), out var parsedValue))
			{
				return parsedValue;
			}

			return 1d;
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
					smartsheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, new[] { rowToUpdate });
				}
				else
				{
					var newRow = new Row { ParentId = parentRowId, Cells = cells };
					smartsheet.SheetResources.RowResources.AddRows(tipOutSheetId, new[] { newRow });
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
				smartsheet.SheetResources.RowResources.UpdateRows(tipOutSheetId, rowsToUpdate);
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
					new Cell { Value = string.Empty, ColumnId = columnIdMap[ColumnNames.Sunday] }
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

			if (server.Monday > 0)
			{
				cells.Add(new Cell { Value = server.Monday, ColumnId = columnIdMap[ColumnNames.Monday] });
			}

			if (server.Tuesday > 0)
			{
				cells.Add(new Cell { Value = server.Tuesday, ColumnId = columnIdMap[ColumnNames.Tuesday] });
			}

			if (server.Wednesday > 0)
			{
				cells.Add(new Cell { Value = server.Wednesday, ColumnId = columnIdMap[ColumnNames.Wednesday] });
			}

			if (server.Thursday > 0)
			{
				cells.Add(new Cell { Value = server.Thursday, ColumnId = columnIdMap[ColumnNames.Thursday] });
			}

			if (server.Friday > 0)
			{
				cells.Add(new Cell { Value = server.Friday, ColumnId = columnIdMap[ColumnNames.Friday] });
			}

			if (server.Saturday > 0)
			{
				cells.Add(new Cell { Value = server.Saturday, ColumnId = columnIdMap[ColumnNames.Saturday] });
			}

			if (server.Sunday > 0)
			{
				cells.Add(new Cell { Value = server.Sunday, ColumnId = columnIdMap[ColumnNames.Sunday] });
			}

			return cells;
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

			smartsheet.SheetResources.RowResources.AddRows(SheetIdLog, new[] { newRow });
		}
	}
}