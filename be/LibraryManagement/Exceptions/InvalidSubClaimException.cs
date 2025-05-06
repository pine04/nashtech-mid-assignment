namespace LibraryManagement.Exceptions
{
    public class InvalidSubClaimException : DomainException
    {
        public InvalidSubClaimException(string message = "Invalid sub claim in token.") : base(message) { }
    }
}