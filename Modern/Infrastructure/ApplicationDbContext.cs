using Microsoft.EntityFrameworkCore;
using Modern.Infrastructure.Entities;

namespace Modern.Infrastructure
{
	public class ApplicationDbContext:DbContext
	{
		
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Book> Books { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
