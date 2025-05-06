namespace LibraryManagement.Exceptions
{
    public class RequestAlreadySettledException : DomainException
    {
        public RequestAlreadySettledException(string message = "This request has already been approved or rejected.") : base(message) { }
    }
}