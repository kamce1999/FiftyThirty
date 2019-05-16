using System.Collections.Generic;

namespace fifty.DataModel.Models
{
	public class Category
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Parent_id { get; set; }

		public ICollection<object> Posts { get; set; }
	}
}