using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Fifty.Lavu;

using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace Fifty.Smartsheet
{
    public static class Helper
    {
        private const long SheetId = 1854968576665476;
        private static List<Column> columnList = new List<Column>();

        public static void GetData(List<ServerHours> serverHours)
        {
            var smartsheet = new SmartsheetBuilder().SetAccessToken("lsazvdkpo5338ett4b2rpdrvm2").Build();

            var sheet = smartsheet.SheetResources.GetSheet(SheetId, null, null, null, null, null, null, null);

            columnList = GetColumns(sheet);

            GetEmployeeRange(sheet, columnList, out var beginRow, out var endRow);
            if (beginRow == null || endRow == null)
            {
                throw new Exception("Could not find Employee Rows");
            }

            var employeeRows = sheet.Rows.Where(r => r.RowNumber > beginRow.RowNumber && r.RowNumber < endRow.RowNumber).Select(r => r).ToList();
            
            foreach (var server in serverHours)
            {
                var cells = new List<Cell>
                {
                    new Cell { Value = server.ServerId, ColumnId = GetColumnId(ColumnType.ServerId) },
                    new Cell { Value = server.EmployeeName, ColumnId = GetColumnId(ColumnType.EmployeeName) },
                    new Cell { Value = server.Position, ColumnId = GetColumnId(ColumnType.Position) },
                    new Cell { Value = server.PayRate, ColumnId = GetColumnId(ColumnType.PayRate) },
                    new Cell { Value = server.Monday, ColumnId = GetColumnId(ColumnType.Monday) },
                    new Cell { Value = server.Tuesday, ColumnId = GetColumnId(ColumnType.Tuesday) },
                    new Cell { Value = server.Wednesday, ColumnId = GetColumnId(ColumnType.Wednesday) },
                    new Cell { Value = server.Thursday, ColumnId = GetColumnId(ColumnType.Thursday) },
                    new Cell { Value = server.Friday, ColumnId = GetColumnId(ColumnType.Friday) },
                    new Cell { Value = server.Saturday, ColumnId = GetColumnId(ColumnType.Saturday) },
                    new Cell { Value = server.Sunday, ColumnId = GetColumnId(ColumnType.Sunday) }
                };

                var row = employeeRows.FirstOrDefault(r => Convert.ToInt32(r.Cells[0].Value) == server.ServerId);

                if (row != null)
                {
                    var rowToUpdate = new Row { Id = row.Id, Cells = cells };

                    smartsheet.SheetResources.RowResources.UpdateRows(SheetId, new[] { rowToUpdate });
                }
                else
                {
                    var newRow = new Row { ParentId = beginRow.Id, Cells = cells };
                    smartsheet.SheetResources.RowResources.AddRows(SheetId, new[] { newRow });
                }
            }


            Console.WriteLine("Loaded " + sheet.Rows.Count + " rows from sheet: " + sheet.Name);
        }

        private static string GetStringValue(float value) => value > 0 ? value.ToString(CultureInfo.CurrentCulture) : string.Empty;

        public static List<Column> GetColumns(Sheet sheet)
        {
            var cols = new List<Column>
            {
                GetColumn(sheet, "ServerId", ColumnType.ServerId),
                GetColumn(sheet, "primary", ColumnType.EmployeeName),
                GetColumn(sheet, "Position", ColumnType.Position),
                GetColumn(sheet, "Pay Rate", ColumnType.PayRate),
                GetColumn(sheet, "Monday", ColumnType.Monday),
                GetColumn(sheet, "Tuesday", ColumnType.Tuesday),
                GetColumn(sheet, "Wednesday", ColumnType.Wednesday),
                GetColumn(sheet, "Thursday", ColumnType.Thursday),
                GetColumn(sheet, "Friday", ColumnType.Friday),
                GetColumn(sheet, "Saturday", ColumnType.Saturday),
                GetColumn(sheet, "Sunday", ColumnType.Sunday),
            };

            var errors = cols.Where(c => c.Index < 0 || c.Id < 1).Select(c => c.ColumnType.ToString()).ToArray();
            if (errors.Length > 0)
            {
                throw new Exception($"Could not find column headers for: {string.Join(',', errors)}");
            }

            return cols;
        }

        private static Column GetColumn(Sheet sheet, string name, ColumnType type)
        {
            var column = new Column { ColumnType = type };

            var result = sheet.Columns.FirstOrDefault(c => string.Compare(c.Title, name, StringComparison.OrdinalIgnoreCase) == 0);
            if (result != null)
            {
                column.Id = result.Id ?? -1;
            }

            return column;
        }

        private static long GetColumnId(ColumnType type)
        {
            var id = columnList.First(c => c.ColumnType == type).Id;

            return id;
        }

        private static void GetEmployeeRange(Sheet sheet, List<Column> columns, out Row beginEmployeeRow, out Row endEmployeeRow)
        {
            beginEmployeeRow = null;
            endEmployeeRow = null;

            var employeeNameCol = columns.First(c => c.ColumnType == ColumnType.EmployeeName);

            foreach (var row in sheet.Rows)
            {
                var cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == employeeNameCol.Id)?.Value;
                if ((cellValue ?? string.Empty).ToString() == "Employee Name")
                {
                    beginEmployeeRow = row;
                }

                if ((cellValue ?? string.Empty).ToString() == "Salary Employees")
                {
                    endEmployeeRow = row;
                }
            }
        }
    }
}