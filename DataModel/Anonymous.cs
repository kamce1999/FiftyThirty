namespace fifty.DataModel.Models
{
	public class Anonymous
	{
		public string Amount { get; set; }
		public string Tip_amount { get; set; }
		public string Autograt_amount { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public string Employee_name { get; set; }
		public string Employee_role_name { get; set; }
		public string Employee_id { get; set; }
		public string Type { get; set; }
		public string Tender_description { get; set; }
		public string Cc_name { get; set; }
		public Cc_type4? Cc_type { get; set; }
		public string ToJson()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
		public static Anonymous FromJson(string data)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Anonymous>(data);
		}
	}
}