using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace LibraryManagement.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private Mock<ICategoriesService> _categoriesServiceMock;
        private CategoriesController _controller;

        [SetUp]
        public void Setup()
        {
            _categoriesServiceMock = new Mock<ICategoriesService>();
            _controller = new CategoriesController(_categoriesServiceMock.Object);
        }

        [Test]
        public async Task GetCategoriesAsync_ValidParameters_ReturnsOkResultWithPagedResult()
        {
            // Arrange
            var expectedResult = new PagedResult<CategoryDto>
            {
                Results = new List<CategoryDto>
                {
                    new CategoryDto { Id = 1, Name = "Category 1", BookCount = 10 },
                    new CategoryDto { Id = 2, Name = "Category 2", BookCount = 5 }
                },
                TotalRecordCount = 2
            };
            _categoriesServiceMock.Setup(s => s.GetCategoriesAsync(1, 10, "", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetCategoriesAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResult<CategoryDto>>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsOkResultWithCategoryDto()
        {
            // Arrange
            var expectedCategory = new CategoryDto { Id = 1, Name = "Category 1", BookCount = 10 };
            _categoriesServiceMock.Setup(s => s.GetCategoryByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await _controller.GetCategoryByIdAsync(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<CategoryDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedCategory));
        }

        [Test]
        public async Task CreateCategoryAsync_ValidCreateCategoryDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto { Name = "New Category" };
            var createdCategoryDto = new CategoryDto { Id = 1, Name = "New Category", BookCount = 0 };
            _categoriesServiceMock.Setup(s => s.CreateCategoryAsync(createCategoryDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCategoryDto);

            // Act
            var result = await _controller.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.IsInstanceOf<ActionResult<CategoryDto>>(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(createdCategoryDto, createdAtResult.Value);
            Assert.AreEqual(nameof(CategoriesController.GetCategoryByIdAsync), createdAtResult.ActionName);
        }

        [Test]
        public async Task UpdateCategoryAsync_ValidIdAndDto_ReturnsOkResultWithUpdatedCategoryDto()
        {
            // Arrange
            var updateCategoryDto = new UpdateCategoryDto { Name = "Updated Category" };
            var updatedCategoryDto = new CategoryDto { Id = 1, Name = "Updated Category", BookCount = 10 };
            _categoriesServiceMock.Setup(s => s.UpdateCategoryAsync(1, updateCategoryDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedCategoryDto);

            // Act
            var result = await _controller.UpdateCategoryAsync(1, updateCategoryDto);

            // Assert
            Assert.IsInstanceOf<ActionResult<CategoryDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(updatedCategoryDto));
        }

        [Test]
        public async Task DeleteCategoryAsync_ValidId_ReturnsNoContentResult()
        {
            // Arrange
            _categoriesServiceMock.Setup(s => s.DeleteCategoryAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCategoryAsync(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
