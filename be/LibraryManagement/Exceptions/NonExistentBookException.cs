namespace LibraryManagement.Exceptions
{
    public class NonExistentBookException : DomainException
    {
        public NonExistentBookException(string message = "The book does not exist.") : base(message) { }
    }
}