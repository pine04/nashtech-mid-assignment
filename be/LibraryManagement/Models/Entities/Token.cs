namespace LibraryManagement.Models.Entities
{
    public class Token
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public required string TokenValue { get; set; }
        public TokenType TokenType { get; set; }
        public DateTime Expires { get; set; }
    }

    public enum TokenType
    {
        Access,
        Refresh
    }
}