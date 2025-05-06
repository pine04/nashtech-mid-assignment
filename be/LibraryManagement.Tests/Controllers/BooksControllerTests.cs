using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Books;
using LibraryManagement.Services.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace LibraryManagement.Tests.Controllers
{
    public class BooksControllerTests
    {
        private Mock<IBooksService> _booksServiceMock;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            _booksServiceMock = new Mock<IBooksService>();
            _controller = new BooksController(_booksServiceMock.Object);
        }

        [Test]
        public async Task GetBooksAsync_ValidParameters_ReturnsOkResultWithPagedResult()
        {
            // Arrange
            var expectedResult = new PagedResult<BookDto>
            {
                Results = new List<BookDto> { new BookDto { Id = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1" } },
                TotalRecordCount = 1
            };
            _booksServiceMock.Setup(s => s.GetBooksAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetBooksAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResult<BookDto>>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetBooksAsync_ServiceReturnsEmptyResult_ReturnsOkWithEmptyPagedResult()
        {
            // Arrange
            var expectedResult = new PagedResult<BookDto>
            {
                Results = new List<BookDto>(),
                TotalRecordCount = 0
            };
            _booksServiceMock.Setup(s => s.GetBooksAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetBooksAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResult<BookDto>>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetBookByIdAsync_ValidId_ReturnsOkResultWithBookDto()
        {
            // Arrange
            var expectedBook = new BookDto { Id = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1" };
            _booksServiceMock.Setup(s => s.GetBookByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _controller.GetBookByIdAsync(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<BookDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedBook));
        }

        [Test]
        public async Task CreateBookAsync_ValidCreateBookDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createBookDto = new CreateBookDto { Title = "New Book", Author = "New Author", Description = "New Description" };
            var createdBookDto = new BookDto { Id = 1, Title = "New Book", Author = "New Author", Description = "New Description" };
            _booksServiceMock.Setup(s => s.CreateBookAsync(createBookDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdBookDto);

            // Act
            var result = await _controller.CreateBookAsync(createBookDto);

            // Assert
            Assert.IsInstanceOf<ActionResult<BookDto>>(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(createdBookDto, createdAtResult.Value);
            Assert.AreEqual(nameof(BooksController.GetBookByIdAsync), createdAtResult.ActionName);
        }

        [Test]
        public async Task UpdateBookAsync_ValidIdAndDto_ReturnsOkResultWithUpdatedBook()
        {
            // Arrange
            var id = 1;
            var updateBookDto = new UpdateBookDto { Title = "Updated Title" };
            var updatedBookDto = new BookDto { Id = 1, Title = "Updated Title", Author = "Author 1", Description = "Description 1" };

            _booksServiceMock.Setup(s => s.UpdateBookAsync(id, updateBookDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedBookDto);

            // Act
            var result = await _controller.UpdateBookAsync(id, updateBookDto);

            // Assert
            Assert.IsInstanceOf<ActionResult<BookDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(updatedBookDto));
        }

        [Test]
        public async Task DeleteBookAsync_ValidId_ReturnsNoContentResult()
        {
            // Arrange
            var id = 1;
            _booksServiceMock.Setup(s => s.DeleteBookAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBookAsync(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
