using Modern.Infrastructure.Entities;
using Modern.Models.Requests;

namespace Modern.Models.Response
{
	public class BookResponse
	{
		public string Name { get; set; }

		public static BookResponse MapFromBook(Book request)
		{
			var book = new BookResponse();
			book.Name = request.Name;
			return book;
		}
	}
}
