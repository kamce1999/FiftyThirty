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
		private static Dictionary<string, long> columnMap = new Dictionary<string, long>();
        private static Sheet sheet;
        private static SmartsheetClient smartsheet;

        public static void GetData(List<ServerHours> serverHours, List<SalesSummary> salesSummary)
        {
            smartsheet = new SmartsheetBuilder().SetAccessToken(AccessToken).Build();

            sheet = smartsheet.SheetResources.GetSheet(SheetId, null, null, null, null, null, null, null);

            InitializeColumnMap();

            UpdateServerHours(serverHours);

            UpdateDailySales(salesSummary);

            Console.WriteLine("Loaded " + sheet.Rows.Count + " rows from sheet: " + sheet.Name);
        }

        private static void UpdateDailySales(List<SalesSummary> salesSummary)
        {
            var salesRow = GetRow(columnMap[ColumnNames.EmployeeName], "Daily Sales");
            var serviceFeeRow = GetRow(columnMap[ColumnNames.EmployeeName], "Service Fee");
            var sales = new List<Cell>();
            var serviceFee = new List<Cell>();

            foreach (var daySummary in salesSummary)
            {
                switch (daySummary.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Monday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Monday] });
                        break;
                    case DayOfWeek.Tuesday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Tuesday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Tuesday] });
                        break;
                    case DayOfWeek.Wednesday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Wednesday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Wednesday] });
                        break;
                    case DayOfWeek.Thursday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Thursday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Thursday] });
                        break;
                    case DayOfWeek.Friday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Friday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Friday] });
                        break;
                    case DayOfWeek.Saturday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Saturday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Saturday] });
                        break;
                    case DayOfWeek.Sunday:
                        sales.Add(new Cell { Value = daySummary.NetSales, ColumnId = columnMap[ColumnNames.Sunday] });
                        serviceFee.Add(new Cell { Value = daySummary.ServiceFee, ColumnId = columnMap[ColumnNames.Sunday] });
                        break;
                }
            }

            var rowsToUpdate = new[]
            {
                new Row { Id = salesRow.Id, Cells = sales },
                new Row { Id = serviceFeeRow.Id, Cells = serviceFee }
            };

            smartsheet.SheetResources.RowResources.UpdateRows(SheetId, rowsToUpdate);
        }

        private static void UpdateServerHours(List<ServerHours> serverHours)
        {
            var employeeRows = GetEmployeeRows(out var parentRowId);

            foreach (var server in serverHours)
            {
                var cells = GetCellValues(server);

                var row = employeeRows.FirstOrDefault(r => Convert.ToInt32(r.Cells[0].Value) == server.ServerId);

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

        private static void InitializeColumnMap()
		{
			foreach (var column in sheet.Columns)
			{
				if (column.Id.HasValue)
				{
					columnMap.Add(column.Title, column.Id.Value);
				}
			}
		}

		private static List<Cell> GetCellValues(ServerHours server)
		{
			var cells = new List<Cell>
				{
					new Cell { Value = server.ServerId, ColumnId = columnMap[ColumnNames.ServerId] },
					new Cell { Value = server.EmployeeName, ColumnId = columnMap[ColumnNames.EmployeeName] },
					new Cell { Value = server.Position, ColumnId = columnMap[ColumnNames.Position] },
					new Cell { Value = server.PayRate, ColumnId = columnMap[ColumnNames.PayRate] }
				};
            
			if (server.Monday > 0)
			{
				cells.Add(new Cell { Value = server.Monday, ColumnId = columnMap[ColumnNames.Monday] });
			}

			if (server.Tuesday > 0)
			{
				cells.Add(new Cell { Value = server.Tuesday, ColumnId = columnMap[ColumnNames.Tuesday] });
			}

			if (server.Wednesday > 0)
			{
				cells.Add(new Cell { Value = server.Wednesday, ColumnId = columnMap[ColumnNames.Wednesday] });
			}

			if (server.Thursday > 0)
			{
				cells.Add(new Cell { Value = server.Thursday, ColumnId = columnMap[ColumnNames.Thursday] });
			}

			if (server.Friday > 0)
			{
				cells.Add(new Cell { Value = server.Friday, ColumnId = columnMap[ColumnNames.Friday] });
			}

			if (server.Saturday > 0)
			{
				cells.Add(new Cell { Value = server.Saturday, ColumnId = columnMap[ColumnNames.Saturday] });
			}

			if (server.Sunday > 0)
			{
				cells.Add(new Cell { Value = server.Sunday, ColumnId = columnMap[ColumnNames.Sunday] });
			}

			return cells;
		}

        private static float AdjustSalriedHours(bool salaried, float hours)
        {
            return (salaried && hours > 8) ? 8.0f : hours;
        }

        private static List<Row> GetEmployeeRows(out long parentRowId)
        {
            var beginRow = GetRow(columnMap[ColumnNames.EmployeeName], "Employee Name");
            var endRow = GetRow(columnMap[ColumnNames.EmployeeName], "Salary Employees");
            
	        if (beginRow == null || endRow == null)
	        {
		        throw new Exception("Could not find Employee Rows");
	        }

	        parentRowId = beginRow.Id ?? -1;

			return sheet.Rows.Where(r => r.RowNumber > beginRow.RowNumber && r.RowNumber < endRow.RowNumber).Select(r => r).ToList();
		}

        private static Row GetRow(long columnId, string filter)
        {
            foreach (var row in sheet.Rows)
            {
                var cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == columnId)?.Value;
                if ((cellValue ?? string.Empty).ToString() == filter)
                {
                    return row;
                }
            }

            return null;
        }
    }
}