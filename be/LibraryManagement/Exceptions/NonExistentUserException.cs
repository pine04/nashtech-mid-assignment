namespace LibraryManagement.Exceptions
{
    public class NonExistentUserException : DomainException
    {
        public NonExistentUserException(string message = "The user does not exist.") : base(message) { }
    }
}