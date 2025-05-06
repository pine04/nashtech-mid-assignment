using LibraryManagement.Models.Dtos.Auth;

namespace LibraryManagement.Services.Auth
{
    public interface IAuthService
    {
        public Task<UserDto> GetMyProfileAsync(int userId, CancellationToken cancellationToken);
        public Task<AuthDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
        public Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
        public Task LogoutAsync(int userId, CancellationToken cancellationToken);
        public Task<AuthDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    }
}