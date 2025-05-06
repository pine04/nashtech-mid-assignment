namespace LibraryManagement.Models.Entities
{
    public class BookBorrowingRequest
    {
        public int Id { get; set; }

        public DateTime DateRequested { get; set; }
        public Status Status { get; set; }

        public int RequestorId { get; set; }
        public User? Requestor { get; set; }

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = new List<BookBorrowingRequestDetail>();
    }

    public enum Status
    {
        Approved,
        Rejected,
        Waiting
    }
}