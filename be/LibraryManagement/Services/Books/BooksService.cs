using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Books;
using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Books
{
    public class BooksService : IBooksService
    {
        private ApplicationDbContext _dbContext;

        public BooksService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<BookDto>> GetBooksAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            int skip = (pageNumber - 1) * pageSize;

            List<BookDto> books = await _dbContext.Books
                .Include(book => book.Category)
                .Skip(skip)
                .Take(pageSize)
                .Select(book => book.ToBookDto())
                .ToListAsync(cancellationToken);

            int totalRecordCount = await _dbContext.Books.CountAsync(cancellationToken);

            return new PagedResult<BookDto>()
            {
                Results = books,
                TotalRecordCount = totalRecordCount
            };
        }

        public async Task<BookDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Book? book = await _dbContext.Books.Include(book => book.Category).FirstOrDefaultAsync(book => book.Id == id, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException("Book not found.");
            }

            return book.ToBookDto();
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default)
        {
            if (createBookDto.CategoryId != null)
            {
                bool categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == createBookDto.CategoryId, cancellationToken);

                if (!categoryExists)
                {
                    throw new NonExistentCategoryException($"The category with ID {createBookDto.CategoryId} does not exist.");
                }
            }

            Book book = createBookDto.ToBook();

            await _dbContext.Books.AddAsync(book, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(book).Reference(b => b.Category).LoadAsync(cancellationToken);

            return book.ToBookDto();
        }

        public async Task<BookDto> UpdateBookAsync(int id, UpdateBookDto updateBookDto, CancellationToken cancellationToken = default)
        {
            Book? book = await _dbContext.Books.FindAsync(id, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException("Book not found.");
            }

            if (updateBookDto.CategoryId != null)
            {
                bool categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == updateBookDto.CategoryId, cancellationToken);

                if (!categoryExists)
                {
                    throw new NonExistentCategoryException($"The category with ID {updateBookDto.CategoryId} does not exist.");
                }
            }

            if (updateBookDto.Quantity != null && book.Available + updateBookDto.Quantity - book.Quantity < 0)
            {
                throw new InvalidBookQuantityException("The new quantity you just entered would reduce the available quantity to below 0, which is invalid.");
            }

            book.ApplyUpdates(updateBookDto);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(book).Reference(b => b.Category).LoadAsync(cancellationToken);

            return book.ToBookDto();
        }

        public async Task DeleteBookAsync(int id, CancellationToken cancellationToken = default)
        {
            Book? book = await _dbContext.Books.FindAsync(id, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException("Book not found.");
            }

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}