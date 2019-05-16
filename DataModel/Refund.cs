namespace fifty.DataModel.Models
{
	public class Refund
	{
		public string Id { get; set; }
		public string Check_id { get; set; }
		public string Amount { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public string Cc_name { get; set; }
		public RefundCc_type? Cc_type { get; set; }
		public System.Guid? Trading_day { get; set; }
	}
}