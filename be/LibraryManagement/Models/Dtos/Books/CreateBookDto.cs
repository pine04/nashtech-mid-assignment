using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Books
{
    public class CreateBookDto
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public int? CategoryId { get; set; }
        public int Quantity { get; set; }

        public Book ToBook()
        {
            return new Book()
            {
                Title = Title,
                Author = Author,
                Description = Description,
                CategoryId = CategoryId,
                Quantity = Quantity,
                Available = Quantity
            };
        }
    }
}