using Microsoft.EntityFrameworkCore;

namespace fifty.DataModel.Models
{
	public class BreadcrumbContext : DbContext
	{
		public BreadcrumbContext(DbContextOptions<BreadcrumbContext> options)
			: base(options)
		{
		}

		public DbSet<Category> Blogs { get; set; }
	}
}