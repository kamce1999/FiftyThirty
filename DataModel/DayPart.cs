namespace fifty.DataModel.Models
{
	public class DayPart
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public System.DateTimeOffset? Start { get; set; }
		public System.DateTimeOffset? End { get; set; }
	}
}