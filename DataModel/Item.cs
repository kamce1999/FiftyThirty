namespace fifty.DataModel.Models
{
	public class Item
	{
		public string Item_id { get; set; }
		public string Name { get; set; }
		public string Price { get; set; }
		public string Item_identifier { get; set; }
		public string Category { get; set; }
		public string Category_id { get; set; }
		public string Tax { get; set; }
		public string Tax_rate_id { get; set; }
		public ItemStatus? Status { get; set; }
		public ItemType? Item_type { get; set; }
	}
}