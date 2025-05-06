using LibraryManagement.Models.Entities;

namespace LibraryManagement.Services.Auth
{
    public interface IJwtIssuer
    {
        public int AccessTokenExpirationTime { get; }
        public int RefreshTokenExpirationTime { get; }
        public string IssueAccessToken(User user);
        public string IssueRefreshToken(User user);
    }
}