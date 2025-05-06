namespace LibraryManagement.Exceptions
{
    public class NonExistentCategoryException : DomainException
    {
        public NonExistentCategoryException(string message = "The category does not exist.") : base(message) { }
    }
}