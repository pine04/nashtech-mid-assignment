using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Books;

namespace LibraryManagement.Services.Books
{
    public interface IBooksService
    {
        public Task<PagedResult<BookDto>> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        public Task<BookDto> GetBookByIdAsync(int id, CancellationToken cancellationToken);
        public Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken);
        public Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto, CancellationToken cancellationToken);
        public Task DeleteBookAsync(int id, CancellationToken cancellationToken);
    }
}