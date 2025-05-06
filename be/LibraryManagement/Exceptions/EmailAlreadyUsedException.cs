namespace LibraryManagement.Exceptions
{
    public class EmailAlreadyUsedException : DomainException
    {
        public EmailAlreadyUsedException(string message = "This email has already been used.") : base(message) { }
    }
}