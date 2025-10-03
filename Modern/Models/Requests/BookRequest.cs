using Modern.Infrastructure.Entities;

namespace Modern.Models.Requests
{
	
	public class BookRequest
	{
		public string Name { get; set; }

		public static Book MapToBook(BookRequest request)
		{
			var book = new Book();
			book.Name = request.Name;
			return book;
		}
	}



}
