namespace LibraryManagement.Exceptions
{
    public class MonthlyLimitReachedException : DomainException
    {
        public MonthlyLimitReachedException(string message = "The monthly limit of 3 borrowing requests has been reached.") : base(message) { }
    }
}