namespace LibraryManagement.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message = "Resource not found.") : base(message) { }
    }
}