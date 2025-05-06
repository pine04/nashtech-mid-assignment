namespace LibraryManagement.Models.Dtos.Auth
{
    public class AuthDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}