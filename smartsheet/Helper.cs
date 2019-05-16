using System;
using System.Linq;

using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace Fifty.Smartsheet
{
	public class Helper
	{
		public void GetData()
		{
			var smartsheet = new SmartsheetBuilder().SetAccessToken("lsazvdkpo5338ett4b2rpdrvm2").Build();

			var sheet = smartsheet.SheetResources.GetSheet(1854968576665476, null, null, null, null, null, null, null);

			var primaryColumn = sheet.Columns.FirstOrDefault(c => string.Compare(c.Title, "primary", StringComparison.OrdinalIgnoreCase) == 0);
			//5773327257102212

			if (primaryColumn?.Index == null)
			{
				throw new Exception("Could not find primary column");	
			}

			foreach (var row in sheet.Rows)
			{
				//5773327257102212
				if ((row.Cells[primaryColumn.Index.Value].Value ?? string.Empty).ToString() == "Employee Name")
				{
					Console.WriteLine(row.ToString());
				}
			}

			Console.WriteLine("Loaded " + sheet.Rows.Count + " rows from sheet: " + sheet.Name);
		}
	}
}