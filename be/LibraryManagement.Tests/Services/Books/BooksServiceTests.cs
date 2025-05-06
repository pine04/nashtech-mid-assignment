using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Books;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Books;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Books
{
    public class BooksServiceTests
    {
        private BooksService _booksService;
        private ApplicationDbContext _dbContext;
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Give it a unique name
                .Options;

            // Create a new instance of ApplicationDbContext for each test
            _dbContext = new ApplicationDbContext(_dbContextOptions); // Pass null for config and seeder

            // Ensure the database is clean before each test.  This is CRUCIAL.
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _booksService = new BooksService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            //  Dispose the db context
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetBooksAsync_NoBooks_ReturnsEmptyPagedResult()
        {
            // Arrange (empty database)

            // Act
            var result = await _booksService.GetBooksAsync(1, 10, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Results);
            Assert.That(result.TotalRecordCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetBooksAsync_WithBooks_ReturnsPagedResult()
        {
            // Arrange
            var category = new Category { Name = "Fiction" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            var books = new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1", Description = "Description 1", CategoryId = category.Id, Quantity = 5, Available = 5 },
                new Book { Title = "Book 2", Author = "Author 2", Description = "Description 2", CategoryId = category.Id, Quantity = 10, Available = 10 },
                new Book { Title = "Book 3", Author = "Author 3", Description = "Description 3", CategoryId = category.Id, Quantity = 2, Available = 2 }
            };

            _dbContext.Books.AddRange(books);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _booksService.GetBooksAsync(1, 10, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Results);
            Assert.That(result.Results.Count, Is.EqualTo(3));
            Assert.That(result.TotalRecordCount, Is.EqualTo(3));

            // Check the content of the first book.  Use .First() but be aware of exceptions.
            Assert.That(result.Results.First().Title, Is.EqualTo("Book 1"));
            Assert.That(result.Results.First().Author, Is.EqualTo("Author 1"));
        }

        [Test]
        public async Task GetBookByIdAsync_ValidId_ReturnsBookDto()
        {
            // Arrange
            var category = new Category { Name = "Non-Fiction" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            var book = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                Description = "Test Description",
                CategoryId = category.Id,
                Quantity = 3,
                Available = 3
            };

            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _booksService.GetBookByIdAsync(book.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(book.Id));
            Assert.That(result.Title, Is.EqualTo("Test Book"));
            Assert.That(result.Author, Is.EqualTo("Test Author"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
            Assert.That(result.Category, Is.EqualTo("Non-Fiction")); // Assuming you have included category name in BookDto
            Assert.That(result.Quantity, Is.EqualTo(3));
            Assert.That(result.Available, Is.EqualTo(3));
        }

        [Test]
        public void GetBookByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange (no books in the database)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _booksService.GetBookByIdAsync(1, CancellationToken.None));
        }

        [Test]
        public async Task CreateBookAsync_ValidDto_CreatesBookAndReturnsDto()
        {
            // Arrange
            var category = new Category { Name = "Sci-Fi" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                CategoryId = category.Id,
                Quantity = 7
            };

            // Act
            var result = await _booksService.CreateBookAsync(createBookDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("New Book"));
            Assert.That(result.Author, Is.EqualTo("New Author"));
            Assert.That(result.Description, Is.EqualTo("New Description"));
            Assert.That(result.Category, Is.EqualTo("Sci-Fi"));
            Assert.That(result.Quantity, Is.EqualTo(7));
            Assert.That(result.Available, Is.EqualTo(7));

            // Verify that the book is actually in the database
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Title == "New Book");
            Assert.IsNotNull(book);
            Assert.That(book.Author, Is.EqualTo("New Author"));
            Assert.That(book.Description, Is.EqualTo("New Description"));
            Assert.That(book.CategoryId, Is.EqualTo(category.Id));
            Assert.That(book.Quantity, Is.EqualTo(7));
            Assert.That(book.Available, Is.EqualTo(7));
        }

        [Test]
        public void CreateBookAsync_InvalidCategoryId_ThrowsNonExistentCategoryException()
        {
            // Arrange
            var createBookDto = new CreateBookDto { Title = "Invalid Book", Author = "Author", Description = "Description", CategoryId = 999, Quantity = 1 }; // Invalid CategoryId

            // Act & Assert
            Assert.ThrowsAsync<NonExistentCategoryException>(async () =>
                await _booksService.CreateBookAsync(createBookDto, CancellationToken.None));
        }

        [Test]
        public async Task UpdateBookAsync_ValidDto_UpdatesBookAndReturnsDto()
        {
            // Arrange
            var category1 = new Category { Name = "Fantasy" };
            var category2 = new Category { Name = "Mystery" };
            _dbContext.Categories.AddRange(category1, category2);
            await _dbContext.SaveChangesAsync();

            var book = new Book
            {
                Title = "Original Title",
                Author = "Original Author",
                Description = "Original Description",
                CategoryId = category1.Id,
                Quantity = 4,
                Available = 4
            };

            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Description = "Updated Description",
                CategoryId = category2.Id,
                Quantity = 6
            };

            // Act
            var result = await _booksService.UpdateBookAsync(book.Id, updateBookDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(book.Id));
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
            Assert.That(result.Author, Is.EqualTo("Updated Author"));
            Assert.That(result.Description, Is.EqualTo("Updated Description"));
            Assert.That(result.Category, Is.EqualTo("Mystery")); // Updated Category Name
            Assert.That(result.Quantity, Is.EqualTo(6));
            Assert.That(result.Available, Is.EqualTo(6)); // Available should be updated too.

            // Verify that the book is updated in the database
            var updatedBook = await _dbContext.Books.FindAsync(book.Id);
            Assert.IsNotNull(updatedBook);
            Assert.That(updatedBook.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedBook.Author, Is.EqualTo("Updated Author"));
            Assert.That(updatedBook.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedBook.CategoryId, Is.EqualTo(category2.Id)); // Updated CategoryId
            Assert.That(updatedBook.Quantity, Is.EqualTo(6));
            Assert.That(updatedBook.Available, Is.EqualTo(6));
        }

        [Test]
        public void UpdateBookAsync_InvalidBookId_ThrowsNotFoundException()
        {
            // Arrange (no books)
            var updateBookDto = new UpdateBookDto { Title = "Title" };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _booksService.UpdateBookAsync(999, updateBookDto, CancellationToken.None));
        }

        [Test]
        public void UpdateBookAsync_InvalidCategoryId_ThrowsNonExistentCategoryException()
        {
            // Arrange
            var book = new Book { Title = "Book", Author = "Author", Description = "Description", Quantity = 1, Available = 1 };
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            var updateBookDto = new UpdateBookDto { CategoryId = 999 }; // Invalid CategoryId

            // Act & Assert
            Assert.ThrowsAsync<NonExistentCategoryException>(async () =>
                await _booksService.UpdateBookAsync(book.Id, updateBookDto, CancellationToken.None));
        }

        [Test]
        public async Task DeleteBookAsync_ValidId_DeletesBook()
        {
            // Arrange
            var book = new Book { Title = "Book to Delete", Author = "Author", Description = "Description", Quantity = 1, Available = 1 };
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            await _booksService.DeleteBookAsync(book.Id, CancellationToken.None);

            // Assert
            Assert.IsNull(await _dbContext.Books.FindAsync(book.Id)); // Book should be deleted
            Assert.That(_dbContext.Books.Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteBookAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange (no books)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _booksService.DeleteBookAsync(999, CancellationToken.None));
        }

        [Test]
        public async Task UpdateBookAsync_InvalidQuantity_ThrowsInvalidBookQuantityException()
        {
            // Arrange
            var book = new Book { Title = "Book", Author = "Author", Description = "Description", Quantity = 5, Available = 1 };
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            var updateBookDto = new UpdateBookDto { Quantity = 2 }; // Trying to reduce available below 0 (5 + 2 - 5 = 2 which is not < 0)

            // Act & Assert
            Assert.ThrowsAsync<InvalidBookQuantityException>(async () =>
                await _booksService.UpdateBookAsync(book.Id, updateBookDto, CancellationToken.None));
        }
    }
}
