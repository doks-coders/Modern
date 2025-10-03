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
		
		public class WeatherApiIntegrationTests
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