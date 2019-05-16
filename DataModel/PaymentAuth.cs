namespace fifty.DataModel.Models
{
	public class PaymentAuth
	{
		public string Id { get; set; }
		public string Check_id { get; set; }
		public string Amount { get; set; }
		public string Tip_amount { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public string Cc_name { get; set; }
		public PaymentAuthCc_type? Cc_type { get; set; }
		public string Last_4 { get; set; }
		public string Auth_code { get; set; }
	}
}