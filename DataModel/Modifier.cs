namespace fifty.DataModel.Models
{
	public class Modifier
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Price { get; set; }
		public string Tax { get; set; }
		public string Tax_rate_id { get; set; }
		public ModifierStatus? Status { get; set; }
	}
}