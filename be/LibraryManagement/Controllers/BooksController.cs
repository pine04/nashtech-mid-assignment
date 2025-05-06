using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Books;
using LibraryManagement.Services.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("/api/books")]
    [Produces(MediaTypeNames.Application.Json)]
    public class BooksController : ControllerBase
    {
        private IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BookDto>>> GetBooksAsync(
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            PagedResult<BookDto> result = await _booksService.GetBooksAsync(pageNumber, pageSize, cancellationToken);
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BookDto bookDto = await _booksService.GetBookByIdAsync(id, cancellationToken);
            return bookDto;
        }

        [HttpPost]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDto>> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default)
        {
            BookDto bookDto = await _booksService.CreateBookAsync(createBookDto, cancellationToken);
            return CreatedAtAction(nameof(GetBookByIdAsync), new { id = bookDto.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> UpdateBookAsync(int id, UpdateBookDto updateBookDto, CancellationToken cancellationToken = default)
        {
            return await _booksService.UpdateBookAsync(id, updateBookDto, cancellationToken);
        }

        [HttpDelete("{id}")]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
        {
            await _booksService.DeleteBookAsync(id, cancellationToken);
            return NoContent();
        }
    }
}