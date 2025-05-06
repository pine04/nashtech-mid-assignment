using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Categories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Categories
{
    public class CategoriesServiceTests
    {
        private CategoriesService _categoriesService;
        private ApplicationDbContext _dbContext;
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCategoryDatabase") // Give it a unique name
                .Options;

            // Create a new instance of ApplicationDbContext for each test
            _dbContext = new ApplicationDbContext(_dbContextOptions); // Pass null for config and seeder

            // Ensure the database is clean before each test.
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _categoriesService = new CategoriesService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the db context
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetCategoriesAsync_NoCategories_ReturnsEmptyPagedResult()
        {
            // Arrange (empty database)

            // Act
            var result = await _categoriesService.GetCategoriesAsync(1, 10, "", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Results);
            Assert.That(result.TotalRecordCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetCategoriesAsync_WithCategories_ReturnsPagedResult()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Name = "Fiction" },
                new Category { Name = "Non-Fiction" },
                new Category { Name = "Sci-Fi" }
            };

            _dbContext.Categories.AddRange(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoriesService.GetCategoriesAsync(1, 10, "", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Results);
            Assert.That(result.Results.Count, Is.EqualTo(3));
            Assert.That(result.TotalRecordCount, Is.EqualTo(3));

            // Check the content of the first category.
            Assert.That(result.Results.First().Name, Is.EqualTo("Fiction"));
        }

        [Test]
        public async Task GetCategoriesAsync_WithSearchQuery_ReturnsFilteredResult()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Name = "Fiction" },
                new Category { Name = "Non-Fiction" },
                new Category { Name = "Sci-Fi" },
                new Category { Name = "Fantasy" }
            };

            _dbContext.Categories.AddRange(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoriesService.GetCategoriesAsync(1, 10, "Fic", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Results);
            Assert.That(result.Results.Count, Is.EqualTo(2)); // "Fiction" and "Non-Fiction" match
            Assert.That(result.TotalRecordCount, Is.EqualTo(2));
            Assert.IsTrue(result.Results.All(c => c.Name.Contains("Fic")));
        }

        [Test]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsCategoryDto()
        {
            // Arrange
            var category = new Category { Name = "Mystery" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(category).Collection(c => c.Books).LoadAsync(); // Load books collection

            // Act
            var result = await _categoriesService.GetCategoryByIdAsync(category.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo("Mystery"));
            Assert.That(result.BookCount, Is.EqualTo(0)); // Ensure BookCount is correct
        }

        [Test]
        public void GetCategoryByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange (no categories in the database)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _categoriesService.GetCategoryByIdAsync(1, CancellationToken.None));
        }

        [Test]
        public async Task CreateCategoryAsync_ValidDto_CreatesCategoryAndReturnsDto()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto { Name = "Biography" };

            // Act
            var result = await _categoriesService.CreateCategoryAsync(createCategoryDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Biography"));
            Assert.That(result.BookCount, Is.EqualTo(0));

            // Verify that the category is actually in the database
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Biography");
            Assert.IsNotNull(category);
            Assert.That(category.Name, Is.EqualTo("Biography"));
        }

        [Test]
        public async Task UpdateCategoryAsync_ValidDto_UpdatesCategoryAndReturnsDto()
        {
            // Arrange
            var category = new Category { Name = "Original Name" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(category).Collection(c => c.Books).LoadAsync();

            var updateCategoryDto = new UpdateCategoryDto { Name = "Updated Name" };

            // Act
            var result = await _categoriesService.UpdateCategoryAsync(category.Id, updateCategoryDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.BookCount, Is.EqualTo(0));

            // Verify that the category is updated in the database
            var updatedCategory = await _dbContext.Categories.FindAsync(category.Id);
            Assert.IsNotNull(updatedCategory);
            Assert.That(updatedCategory.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public void UpdateCategoryAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange (no categories)
            var updateCategoryDto = new UpdateCategoryDto { Name = "New Name" };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _categoriesService.UpdateCategoryAsync(999, updateCategoryDto, CancellationToken.None));
        }

        [Test]
        public async Task DeleteCategoryAsync_ValidId_DeletesCategory()
        {
            // Arrange
            var category = new Category { Name = "Category to Delete" };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            // Act
            await _categoriesService.DeleteCategoryAsync(category.Id, CancellationToken.None);

            // Assert
            Assert.IsNull(await _dbContext.Categories.FindAsync(category.Id)); // Category should be deleted
            Assert.That(_dbContext.Categories.Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteCategoryAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange (no categories)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _categoriesService.DeleteCategoryAsync(999, CancellationToken.None));
        }
    }
}
