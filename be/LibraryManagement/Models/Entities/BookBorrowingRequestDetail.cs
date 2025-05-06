namespace LibraryManagement.Models.Entities
{
    public class BookBorrowingRequestDetail
    {
        public int BookBorrowingRequestId { get; set; }
        public BookBorrowingRequest? BookBorrowingRequest { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public bool Returned { get; set; }
    }
}