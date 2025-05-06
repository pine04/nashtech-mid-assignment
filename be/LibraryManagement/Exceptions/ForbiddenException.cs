namespace LibraryManagement.Exceptions
{
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message = "Forbiden.") : base(message) { }
    }
}