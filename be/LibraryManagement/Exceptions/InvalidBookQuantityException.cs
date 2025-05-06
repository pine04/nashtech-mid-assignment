namespace LibraryManagement.Exceptions
{
    public class InvalidBookQuantityException : DomainException
    {
        public InvalidBookQuantityException(string message = "Invalid book quantity.") : base(message) { }
    }
}