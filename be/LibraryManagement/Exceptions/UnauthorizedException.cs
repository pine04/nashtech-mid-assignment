namespace LibraryManagement.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message = "Unauthorized.") : base(message) { }
    }
}