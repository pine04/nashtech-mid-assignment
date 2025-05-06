using System.Security.Claims;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Auth;
using LibraryManagement.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetMyProfileAsync(CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            return await _authService.GetMyProfileAsync(userId, cancellationToken);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            AuthDto authDto = await _authService.RegisterAsync(registerDto, cancellationToken);
            SetRefreshTokenCookie(authDto.RefreshToken);
            return authDto.AccessToken;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            AuthDto authDto = await _authService.LoginAsync(loginDto, cancellationToken);
            SetRefreshTokenCookie(authDto.RefreshToken);
            return authDto.AccessToken;
        }

        [HttpGet("refresh")]
        public async Task<ActionResult<string>> RefreshAsync(CancellationToken cancellationToken = default)
        {
            string? refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                throw new UnauthorizedException("Refresh token not provided.");
            }

            AuthDto authDto = await _authService.RefreshAsync(refreshToken, cancellationToken);
            SetRefreshTokenCookie(authDto.RefreshToken);
            return authDto.AccessToken;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            await _authService.LogoutAsync(userId, cancellationToken);
            ClearRefreshTokenCookie();

            return Ok();
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken");
        }
    }
}