using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.Infrastructure;
using Modern.Models.Response;
using Modern.Models.Requests;
using Modern.Services;
using System.Net;
using System.Text.Json;

namespace Modern.Test
{
	public class Tests
	{
		
		public class ApiIntegrationTests
		{
			private ApiFactory _factory = null!;

			[SetUp] public void SetUp() => _factory = new ApiFactory(); 
			
			private static readonly JsonSerializerOptions JsonOptions = new()
			{
				PropertyNameCaseInsensitive = true,
				// converters, default-ignore-condition, etc.
			};


			[Test]
			public async Task GET_forecast_returns_200()
			{
				var client = _factory.CreateClient();

				var resp = await client.GetAsync("/weatherforecast");

				Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			}


			[Test]
			public async Task GET_Books_returns_200()
			{
				var client = _factory.CreateClient();

				var resp = await client.GetAsync("/book");

			

				Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

				var json = await resp.Content.ReadAsStringAsync();

				try
				{
					var books = JsonSerializer.Deserialize<List<BookResponse>>(json, JsonOptions);
					Assert.That(books is not null, Is.True);

				}
				catch (JsonException ex)
				{
					// You now *know* the payload is not compatible with List<BookResponse>.
					// Log and convert to a domain-specific error as you see fit.
					throw new InvalidOperationException(
						$"Response could not be parsed as {nameof(BookResponse)} list.", ex);
				}
			}
		 /*
		 [Test]

		 public async Task GET_Book_By_Id_Returns_200_And_Book()
		 {
			 var client = _factory.CreateClient();
			 var resp = await client.GetAsync("/book/1");
			 Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			 var json = await resp.Content.ReadAsStringAsync();
			 var book = JsonSerializer.Deserialize<BookResponse>(json, JsonOptions);
			 Assert.That(book, Is.Not.Null);
			   // BookResponse does not have Id, so just check Name is not null
			   Assert.That(book.Name, Is.Not.Null);
		 }
		 */
		 


		[Test]
        public async Task GET_Book_By_Id_Returns_404_For_Invalid_Id()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/book/9999");
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task POST_CreateBook_Returns_200_And_Creates_Book()
        {
            var client = _factory.CreateClient();
              var request = new BookRequest { Name = "Test Book" };
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
              var resp = await client.PostAsync("/book/create-book", content);
              Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
              var json = await resp.Content.ReadAsStringAsync();
              var book = JsonSerializer.Deserialize<BookResponse>(json, JsonOptions);
              Assert.That(book, Is.Not.Null);
              Assert.That(book.Name, Is.EqualTo("Test Book"));
        }
		/*
        [Test]
        public async Task POST_UpdateBook_Returns_200_And_Updates_Book()
        {
            var client = _factory.CreateClient();
              var request = new BookRequest { Name = "Updated Title" };
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
              var resp = await client.PostAsync("/book/update-book/1", content);
              Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
              var json = await resp.Content.ReadAsStringAsync();
              var book = JsonSerializer.Deserialize<BookResponse>(json, JsonOptions);
              Assert.That(book, Is.Not.Null);
              Assert.That(book.Name, Is.EqualTo("Updated Title"));
        }
		*/
        [Test]
        public async Task POST_UpdateBook_Returns_404_For_Invalid_Id()
        {
            var client = _factory.CreateClient();
              var request = new BookRequest { Name = "Updated Title" };
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
              var resp = await client.PostAsync("/book/update-book/9999", content);
              Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        /*
        [Test]
        public async Task DELETE_Book_Returns_200_And_Removes_Book()
        {
            var client = _factory.CreateClient();
            var resp = await client.DeleteAsync("/book/delete-book/1");
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        */
        [Test]
        public async Task DELETE_Book_Returns_404_For_Invalid_Id()
        {
            var client = _factory.CreateClient();
            var resp = await client.DeleteAsync("/book/delete-book/9999");
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
		}

		public class BookUserFlowTests
		{
			private ApiFactory _factory = null!;
			[SetUp] public void SetUp() => _factory = new ApiFactory();
			private static readonly JsonSerializerOptions JsonOptions = new()
			{
				PropertyNameCaseInsensitive = true,
			};

			[Test]
			public async Task Create_Get_Update_Delete_Flow()
			{
				var client = _factory.CreateClient();
				// 1. Create
				var createRequest = new BookRequest { Name = "Flow Book" };
				var createContent = new StringContent(JsonSerializer.Serialize(createRequest), System.Text.Encoding.UTF8, "application/json");
				var createResp = await client.PostAsync("/book/create-book", createContent);
				Assert.That(createResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				var createdJson = await createResp.Content.ReadAsStringAsync();
				var createdBook = JsonSerializer.Deserialize<BookResponse>(createdJson, JsonOptions);
				Assert.That(createdBook, Is.Not.Null);
				Assert.That(createdBook.Name, Is.EqualTo("Flow Book"));

				// 2. Get by id (simulate by searching for name)
				var allResp = await client.GetAsync("/book");
				var allJson = await allResp.Content.ReadAsStringAsync();
				var allBooks = JsonSerializer.Deserialize<List<BookResponse>>(allJson, JsonOptions);
				var book = allBooks?.FirstOrDefault(b => b.Name == "Flow Book");
				Assert.That(book, Is.Not.Null);

				// 3. Update
				var updateRequest = new BookRequest { Name = "Flow Book Updated" };
				var updateContent = new StringContent(JsonSerializer.Serialize(updateRequest), System.Text.Encoding.UTF8, "application/json");
				var updateResp = await client.PostAsync($"/book/update-book/{book?.Id}", updateContent); // If you have ID, use it
				Assert.That(updateResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				var updatedJson = await updateResp.Content.ReadAsStringAsync();
				var updatedBook = JsonSerializer.Deserialize<BookResponse>(updatedJson, JsonOptions);
				Assert.That(updatedBook, Is.Not.Null);
				Assert.That(updatedBook.Name, Is.EqualTo("Flow Book Updated"));

				// 4. Get again
				var getAgainResp = await client.GetAsync($"/book/{book?.Id}");
				Assert.That(getAgainResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				var getAgainJson = await getAgainResp.Content.ReadAsStringAsync();
				var getAgainBook = JsonSerializer.Deserialize<BookResponse>(getAgainJson, JsonOptions);
				Assert.That(getAgainBook.Name, Is.EqualTo("Flow Book Updated"));

				// 5. Delete
				var deleteResp = await client.DeleteAsync($"/book/delete-book/{book?.Id}");
				Assert.That(deleteResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

				// 6. Get after delete
				var getAfterDeleteResp = await client.GetAsync($"/book/{book?.Id}");
				Assert.That(getAfterDeleteResp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			}

			[Test]
			public async Task CreateMultipleAndListAll_Flow()
			{
				var client = _factory.CreateClient();
				var names = new[] { "Book A", "Book B", "Book C" };
				foreach (var name in names)
				{
					var req = new BookRequest { Name = name };
					var content = new StringContent(JsonSerializer.Serialize(req), System.Text.Encoding.UTF8, "application/json");
					var resp = await client.PostAsync("/book/create-book", content);
					Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				}
				var allResp = await client.GetAsync("/book");
				var allJson = await allResp.Content.ReadAsStringAsync();
				var allBooks = JsonSerializer.Deserialize<List<BookResponse>>(allJson, JsonOptions);
				foreach (var name in names)
				{
					Assert.That(allBooks.Any(b => b.Name == name), Is.True);
				}
			}
		}

		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void First_Test()
		{
			Assert.Pass();
		}
	}
}