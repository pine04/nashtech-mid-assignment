using System.Text.Json;
using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationSeeder : IApplicationSeeder
    {
        class RawBook
        {
            public required string Title { get; set; }
            public required string Author { get; set; }
            public required string Description { get; set; }
            public string? Category { get; set; }
            public int Quantity { get; set; }
            public int Available { get; set; }
        }

        public void Seed(DbContext context)
        {
            // Make sure seeded tables are empty.
            if (!TablesEmpty(context))
            {
                return;
            }

            // Seed categories
            string categoryJsonData = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "Categories.json"));
            List<Category>? categories = JsonSerializer.Deserialize<List<Category>>(categoryJsonData);

            if (categories == null)
            {
                return;
            }

            context.Set<Category>().AddRange(categories);
            context.SaveChanges();

            // Seed books
            string booksJsonData = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "Books.json"));
            List<RawBook>? rawBooks = JsonSerializer.Deserialize<List<RawBook>>(booksJsonData);

            if (rawBooks == null)
            {
                return;
            }

            // Create actual book entities with the correct category IDs.
            List<Book> books = rawBooks.Select(rb => new Book()
            {
                Title = rb.Title,
                Author = rb.Author,
                Description = rb.Description,
                CategoryId = categories.Find(c => c.Name == rb.Category)?.Id,
                Quantity = rb.Quantity,
                Available = rb.Available
            }).ToList();

            context.Set<Book>().AddRange(books);
            context.SaveChanges();
        }

        public async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
        {
            // Make sure seeded tables are empty.
            if (!TablesEmpty(context))
            {
                return;
            }

            // Seed categories
            string categoryJsonData = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "Categories.json"), cancellationToken);
            List<Category>? categories = JsonSerializer.Deserialize<List<Category>>(categoryJsonData);

            if (categories == null)
            {
                return;
            }

            await context.Set<Category>().AddRangeAsync(categories, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Seed books
            string booksJsonData = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "Books.json"), cancellationToken);
            List<RawBook>? rawBooks = JsonSerializer.Deserialize<List<RawBook>>(booksJsonData);

            if (rawBooks == null)
            {
                return;
            }

            // Create actual book entities with the correct category IDs.
            List<Book> books = rawBooks.Select(rb => new Book()
            {
                Title = rb.Title,
                Author = rb.Author,
                Description = rb.Description,
                CategoryId = categories.Find(c => c.Name == rb.Category)?.Id,
                Quantity = rb.Quantity,
                Available = rb.Available
            }).ToList();

            await context.Set<Book>().AddRangeAsync(books, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public bool TablesEmpty(DbContext context)
        {
            bool hasCategoryData = context.Set<Category>().Any();
            bool hasBookData = context.Set<Book>().Any();

            return !hasCategoryData && !hasBookData;
        }
    }
}
