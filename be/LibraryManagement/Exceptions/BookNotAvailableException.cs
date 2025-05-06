namespace LibraryManagement.Exceptions
{
    public class BookNotAvailableException : DomainException
    {
        public BookNotAvailableException(string message = "Book not available.") : base(message) { }
    }
}