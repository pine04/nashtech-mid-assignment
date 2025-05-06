namespace LibraryManagement.Exceptions
{
    public class TooManyBooksException : DomainException
    {
        public TooManyBooksException(string message = "Only 5 books maybe borrowed in one request.") : base(message) { }
    }
}