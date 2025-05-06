namespace LibraryManagement.Models.Entities
{
    public class Book
    {
        public int Id { get; set; }

        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int Quantity { get; set; }
        public int Available { get; set; }

        public ICollection<BookBorrowingRequest>? BookBorrowingRequests { get; set; }
        public ICollection<BookBorrowingRequestDetail>? BookBorrowingRequestDetails { get; set; }
    }
}