namespace fifty.DataModel.Models
{
	public class TradingDay
	{
		public string Id { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public System.DateTimeOffset? Start { get; set; }
		public System.DateTimeOffset? End { get; set; }
		public TradingDayStatus? Status { get; set; }
	}
}