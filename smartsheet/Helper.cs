using System;
using System.Collections.Generic;
using System.Linq;
using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace Fifty.Smartsheet
{
	public static class Helper
	{
		private const long SheetId = 1854968576665476;
		private const string AccessToken = "lsazvdkpo5338ett4b2rpdrvm2";
		private static Dictionary<string, long> columnIdMap = new Dictionary<string, long>();
		private static Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();
		private static Sheet sheet;
		private static SmartsheetClient smartsheet;

		public static void GetData(List<ServerHours> serverHours, List<SalesSummary> salesSummary)
		{
			smartsheet = new SmartsheetBuilder().SetAccessToken(AccessToken).Build();

			sheet = smartsheet.SheetResources.GetSheet(SheetId, new[] { SheetLevelInclusion.FORMAT }, null, null, null, null, null, null);

			InitializeColumnMap();

			UpdateServerHours(serverHours);

			UpdateDailySales(salesSummary);

			Console.WriteLine("Loaded " + sheet.Rows.Count + " rows from sheet: " + sheet.Name);
		}

		private static void UpdateDailySales(List<SalesSummary> salesSummary)
		{
			var salesRow = GetRow(columnIdMap[ColumnNames.EmployeeName], "Daily Sales");
			var serviceFeeRow = GetRow(columnIdMap[ColumnNames.EmployeeName], "Service Fee");
			var sales = new List<Cell>();
			var serviceFee = new List<Cell>();

			var test = GetEmptyRow().ToDictionary(k => k.ColumnId);
			
			foreach (var daySummary in salesSummary)
			{
				switch (daySummary.DayOfWeek)
				{
					case DayOfWeek.Monday:
						test[columnIdMap[ColumnNames.Monday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Monday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Monday] });
						break;
					case DayOfWeek.Tuesday:
						test[columnIdMap[ColumnNames.Tuesday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Tuesday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Tuesday] });
						break;
					case DayOfWeek.Wednesday:
						test[columnIdMap[ColumnNames.Wednesday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Wednesday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Wednesday] });
						break;
					case DayOfWeek.Thursday:
						test[columnIdMap[ColumnNames.Thursday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Thursday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Thursday] });
						break;
					case DayOfWeek.Friday:
						test[columnIdMap[ColumnNames.Friday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Friday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Friday] });
						break;
					case DayOfWeek.Saturday:
						test[columnIdMap[ColumnNames.Saturday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Saturday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Saturday] });
						break;
					case DayOfWeek.Sunday:
						test[columnIdMap[ColumnNames.Sunday]].Value = daySummary.NetSales;
						sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnIdMap[ColumnNames.Sunday] });
						serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnIdMap[ColumnNames.Sunday] });
						break;
				}
			}

			var tests = test.Values.ToList();

			var rowsToUpdate = new[]
			{
				new Row { Id = salesRow.Id, Cells = sales },
				new Row { Id = serviceFeeRow.Id, Cells = serviceFee }
			};

			smartsheet.SheetResources.RowResources.UpdateRows(SheetId, rowsToUpdate);
		}

		private static void UpdateServerHours(IList<ServerHours> serverHours)
		{
			var employeeRows = GetEmployeeRows(out var parentRowId);

			ClearEmployeeRows(employeeRows);

			foreach (var server in serverHours.OrderBy(it => it.ServerId).ToList())
			{
				var cells = GetCellValues(server);

				var row = employeeRows.FirstOrDefault(
					r => Convert.ToInt32(r.Cells[columnIndexMap[ColumnNames.ServerId]].Value) == server.ServerId
						&& r.Cells[columnIndexMap[ColumnNames.Position]].Value.ToString() == server.Position);

				if (row != null)
				{
					var rowToUpdate = new Row { Id = row.Id, Cells = cells };
					smartsheet.SheetResources.RowResources.UpdateRows(SheetId, new[] { rowToUpdate });
				}
				else
				{
					var newRow = new Row { ParentId = parentRowId, Cells = cells };
					smartsheet.SheetResources.RowResources.AddRows(SheetId, new[] { newRow });
				}
			}
		}

		private static void ClearEmployeeRows(List<Row> employeeRows)
		{
			foreach (var item in employeeRows)
			{
				smartsheet.SheetResources.RowResources.UpdateRows(SheetId, new[] { new Row { Id = item.Id, Cells = GetEmptyRow() } });
			}
		}

		private static List<Cell> GetEmptyRow()
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

		private static void InitializeColumnMap()
		{
			foreach (var column in sheet.Columns)
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

		private static float AdjustSalriedHours(bool salaried, float hours)
		{
			return (salaried && hours > 8) ? 8.0f : hours;
		}

		private static List<Row> GetEmployeeRows(out long parentRowId)
		{
			var beginRow = GetRow(columnIdMap[ColumnNames.EmployeeName], "Employee Name");
			var endRow = GetRow(columnIdMap[ColumnNames.EmployeeName], "Salary Employees");

			if (beginRow == null || endRow == null)
			{
				throw new Exception("Could not find Employee Rows");
			}

			parentRowId = beginRow.Id ?? -1;

			return sheet.Rows.Where(r => r.RowNumber > beginRow.RowNumber && r.RowNumber < endRow.RowNumber).Select(r => r).ToList();
		}

		private static Row GetRow(long columnId, string filter)
		{
			return (from row in sheet.Rows
					let cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == columnId)?.Value
					where (cellValue ?? string.Empty).ToString() == filter
					select row).FirstOrDefault();
		}
	}
}