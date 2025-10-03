using Microsoft.EntityFrameworkCore;
using Modern.Infrastructure.Entities;

namespace Modern.Infrastructure.Repositories
{
	public class BookRepository : Repository<Book>
	{
		private readonly ApplicationDbContext _dbContext;
		public BookRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}


		public async Task<bool> SaveChanges()
		{
			 _dbContext.SaveChanges();
			return true;
		}
	}
}
