using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Books
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
    }

    public static partial class BookExtensions
    {
        public static BookDto ToBookDto(this Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Category = book.Category?.Name,
                CategoryId = book.CategoryId,
                Quantity = book.Quantity,
                Available = book.Available
            };
        }
    }
}