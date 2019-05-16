namespace fifty.DataModel.Models
{
	public class Timesheet
	{
		public string Id { get; set; }
		public string Employee_id { get; set; }
		public string Name { get; set; }
		public string Role_name { get; set; }
		public string Zone_id { get; set; }
		public string Trading_day_id { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public System.DateTimeOffset? Start_time { get; set; }
		public System.DateTimeOffset? End_time { get; set; }
		public string Regular_hours { get; set; }
		public string Break_time { get; set; }
		public string _15x_hours { get; set; }
		public string _2x_hours { get; set; }
		public string Bonus_hour { get; set; }
		public string Pay_rate { get; set; }
		public string Wages { get; set; }
		public string Cash_tips { get; set; }
		public string Cc_tips { get; set; }
		public string Other_tender_tips { get; set; }
		public string Total_tips { get; set; }
		public string Declared_tips { get; set; }
	}
}