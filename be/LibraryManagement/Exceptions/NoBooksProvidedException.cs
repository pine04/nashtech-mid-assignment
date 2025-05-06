namespace LibraryManagement.Exceptions
{
    public class NoBooksProvidedException : DomainException
    {
        public NoBooksProvidedException(string message = "No books have been provided in the borrowing request.") : base(message) { }
    }
}