using Modern.Infrastructure.Repositories;
using Modern.Models.Requests;
using Modern.Models.Response;
using System.Collections;

namespace Modern.Services
{
	public interface IBookService
	{
		Task<BookResponse> CreateBook(BookRequest request);
		Task<BookResponse> Get(string Id);
		Task<IEnumerable<BookResponse>> GetAll();
		Task<BookResponse?> UpdateBook(BookRequest request,string id);
		Task<bool> DeleteBook(string Id);
	}

	public class BookService(BookRepository repository) : IBookService
	{
		public async Task<BookResponse> CreateBook(BookRequest request)
		{
			var book = BookRequest.MapToBook(request);
			await repository.Add(book);
			await repository.SaveChanges();
			return BookResponse.MapFromBook(book);
		}
		public async Task<BookResponse?> UpdateBook(BookRequest request,string id)
		{
			var item = await repository.Get(e => e.Id == id);
			if (item == null)
				return null;
			item.Name = request.Name;
			await repository.Update(item);
			await repository.SaveChanges();
			return BookResponse.MapFromBook(item);
		}
		public async Task<BookResponse> Get(string Id)
		{
			var item =  await repository.Get(e => e.Id == Id);
			if (item == null)
				return null;
			return BookResponse.MapFromBook(item);
		}
		
		
		public async Task<IEnumerable<BookResponse>> GetAll()
		{
			var items = await repository.GetAll();
			var responses = items.ConvertAll(u => BookResponse.MapFromBook(u));
			return responses;
		}

		public async Task<bool> DeleteBook(string Id)
		{
			var item = await repository.Get(e => e.Id == Id);
			if (item == null)
				return false;
			await repository.Delete(item);
			await repository.SaveChanges();
			return true;
		}
	}
}
