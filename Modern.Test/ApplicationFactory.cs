using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modern.Infrastructure;
using Modern.Infrastructure.Entities;
using Modern.Models.Response;
using Modern.Services;
using System.Net;
using System.Text.Json;

namespace Modern.Test
{
    public class ApiFactory : WebApplicationFactory<Program>
    {


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add PostgreSQL test DB
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql("Host=localhost;Port=5432;Database=test_modern;Username=postgres;Password=password");
                });

                services.AddScoped<IBookService, BookService>();

                // Apply migrations or recreate DB schema
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureDeleted();   // optional: for clean test runs
                    db.Database.EnsureCreated();   // or db.Database.Migrate();


                    db.Books.AddRange(new List<Book>()
                    {
                        new Book { Name = "Book 1" },
                         new Book { Name = "Book 2" },
                           new Book { Name = "Book 3" },
                            new Book { Name = "Book 4" },
                    });
                    db.SaveChanges();
                }
            });
        }


        /*
       private SqliteConnection _connection;

       protected override void ConfigureWebHost(IWebHostBuilder builder)
       {


           builder.ConfigureServices(services =>
           {
               // 🔁 Remove existing DbContext registration
               var descriptor = services.SingleOrDefault(
                   d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

               if (descriptor != null)
               {
                   services.Remove(descriptor);
               }

               // ⚙️ Create and open SQLite in-memory connection
               _connection = new SqliteConnection("DataSource=:memory:");
               _connection.Open();

               // ✅ Register DbContext using SQLite in-memory
               services.AddDbContext<ApplicationDbContext>(options =>
               {
                   options.UseSqlite(_connection);
               });

               // Register your service
               services.AddScoped<IBookService, BookService>();

               // 🧪 Build service provider and seed data
               var sp = services.BuildServiceProvider();

               using (var scope = sp.CreateScope())
               {
                   var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                   db.Database.EnsureCreated(); // 🔑 This is required for in-memory SQLite

                   // Seed test data if needed
                   // db.Books.Add(new Book { Title = "Seed Book", Author = "Test Author" });
                   // db.SaveChanges();
               }
           });
       }
         protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Close(); // 🔒 Ensure the in-memory DB is properly closed
        }
           */


        /* In Memory
        builder.ConfigureServices(services =>
        {
            // 🔄 Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // ✅ Add in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // 🧪 Optional: Seed test data after service provider is built
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                // Seed some data (optional)
                // db.Books.Add(new Book { Title = "Test Book", Author = "Test Author" });
                // db.SaveChanges();
            }

            // 🔧 Register your test/mock service implementations
            services.AddScoped<IBookService, BookService>();
        });

        */

        /* In Memory
        builder.ConfigureServices(services =>
        {
            // Remove existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            services.AddScoped<IBookService, BookService>();
        });
        */
        /* Direct to our DB
        builder.ConfigureServices((context, services) =>
        {

            // If the host already registered ApplicationDbContext, drop it first.

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql("Server=localhost; Port=5432; User Id=postgres; Password=password; Database=Modern;"));

            services.AddScoped<IBookService, BookService>();
        });
        */
    }
}


