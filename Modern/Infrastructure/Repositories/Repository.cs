using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Modern.Infrastructure.Repositories
{

	public class Repository<T> where T : class
	{
		public DbSet<T> _entities { get; set; }

		public Repository(ApplicationDbContext dbContext) 
		{
		_entities = dbContext.Set<T>();	
		}

		public async Task<T> Get(Expression<Func<T, bool>>  query)
		{
			var response = await _entities.Where(query).FirstOrDefaultAsync();
			return response;

		}

		public async Task<List<T>> GetAll()
		{
			var responses = await _entities.ToListAsync();
			return responses;
		}

		public async Task<bool> Add(T entity)
		{
			await _entities.AddAsync(entity);
			return true;
		}

		public async Task<bool> Update(T entity)
		{

			 _entities.Update(entity);
			return true;
		}


		public async Task<bool> Delete(T entity)
		{
			_entities.Remove(entity);	
			return true;
		}

		
	}
}
