using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Books
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? Quantity { get; set; }
    }

    public static partial class BookExtensions
    {
        public static void ApplyUpdates(this Book book, UpdateBookDto updateBookDto)
        {
            if (updateBookDto.Title != null)
            {
                book.Title = updateBookDto.Title;
            }

            if (updateBookDto.Author != null)
            {
                book.Author = updateBookDto.Author;
            }

            if (updateBookDto.Description != null)
            {
                book.Description = updateBookDto.Description;
            }

            book.CategoryId = updateBookDto.CategoryId;

            if (updateBookDto.Quantity != null)
            {
                book.Available += (int)updateBookDto.Quantity - book.Quantity;
                book.Quantity = (int)updateBookDto.Quantity;
            }
        }
    }
}