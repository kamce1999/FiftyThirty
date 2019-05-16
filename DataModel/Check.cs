using System.Collections.Generic;

namespace fifty.DataModel.Models
{
	public class Check
	{
		public string Id { get; set; }
		public string Trading_day_id { get; set; }
		public string Name { get; set; }
		public string Number { get; set; }
		public CheckStatus? Status { get; set; }
		public string Sub_total { get; set; }
		public string Tax_total { get; set; }
		public string Total { get; set; }
		public string Mandatory_tip_amount { get; set; }
		public System.DateTimeOffset? Open_time { get; set; }
		public System.DateTimeOffset? Close_time { get; set; }
		public string Employee_name { get; set; }
		public string Employee_role_name { get; set; }
		public string Employee_id { get; set; }
		public double? Guest_count { get; set; }
		public CheckType? Type { get; set; }
		public CheckType_id? Type_id { get; set; }
		public CheckTaxed_type? Taxed_type { get; set; }
		public ICollection<CheckItem> Items { get; set; }
		public System.Collections.Generic.ICollection<Anonymous> Payments { get; set; }
		public Voidcomp Voidcomp { get; set; }
		public string Table_name { get; set; }
		public string Zone { get; set; }
		public string Zone_id { get; set; }
	}
}