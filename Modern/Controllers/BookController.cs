using Microsoft.AspNetCore.Mvc;
using Modern.Models.Requests;
using Modern.Services;

namespace Modern.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class BookController(IBookService bookService) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await bookService.GetAll();
			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string Id)
		{
			var book = await bookService.Get(Id);
			if (book == null)
				return NotFound();
			return Ok(book);
		}

	

		[HttpPost("create-book")]
		public async Task<IActionResult> CreateBook([FromBody] BookRequest request)
		{
			var created = await bookService.CreateBook(request);
			if (created == null)
				return BadRequest();
			return Ok(created);
		}


		[HttpPost("update-book/{id}")]
		public async Task<IActionResult> UpdateBook([FromRoute] string id, [FromBody] BookRequest request)
		{
			var updated = await bookService.UpdateBook(request, id);

			if (updated == null)
				return NotFound();
			return Ok(updated);
		}

		[HttpDelete("delete-book/{id}")]
		public async Task<IActionResult> DeleteBook( string id)
		{
			var deleted = await bookService.DeleteBook(id);
			if (!deleted)
				return NotFound();
			return Ok();
		}
	}
}
