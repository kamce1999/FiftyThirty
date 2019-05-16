namespace fifty.DataModel.Models
{
	public class CashierPayment
	{
		public string Id { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public System.Guid? Trading_day { get; set; }
		public CashierPaymentPipo_type? Pipo_type { get; set; }
		public string Memo { get; set; }
		public string Amount { get; set; }
		public string Authorized_id { get; set; }
		public string Authorizer_name { get; set; }
		public string Receipt_id { get; set; }
		public string Recipient_name { get; set; }
	}
}