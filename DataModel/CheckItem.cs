namespace fifty.DataModel.Models
{
	public class CheckItem
	{
		public string Id { get; set; }
		public string Check_id { get; set; }
		public string Name { get; set; }
		public System.DateTimeOffset? Date { get; set; }
		public string Category_id { get; set; }
		public string Item_id { get; set; }
		public double? Quantity { get; set; }
		public string Price { get; set; }
		public string Pre_tax_price { get; set; }
		public string Regular_price { get; set; }
		public string Cost { get; set; }
		public System.Collections.Generic.ICollection<Side> Sides { get; set; }
		public System.Collections.Generic.ICollection<Modifier> Modifiers { get; set; }
		public Voidcomp2 Voidcomp { get; set; }
	}
}