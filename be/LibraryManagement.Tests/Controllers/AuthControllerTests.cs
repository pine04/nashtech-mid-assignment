using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Auth;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace LibraryManagement.Tests.Controllers
{
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<HttpResponse> _httpResponseMock;
        private Mock<IResponseCookies> _responseCookiesMock;
        private Mock<IRequestCookieCollection> _requestCookiesMock;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpResponseMock = new Mock<HttpResponse>();
            _responseCookiesMock = new Mock<IResponseCookies>();
            _requestCookiesMock = new Mock<IRequestCookieCollection>();

            _httpContextMock.Setup(c => c.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.Setup(c => c.Response).Returns(_httpResponseMock.Object);
            _httpResponseMock.Setup(r => r.Cookies).Returns(_responseCookiesMock.Object);
            _httpRequestMock.Setup(r => r.Cookies).Returns(_requestCookiesMock.Object);

            _controller = new AuthController(_authServiceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContextMock.Object,
                    ActionDescriptor = new ControllerActionDescriptor(),
                    RouteData = new RouteData(),
                }
            };
        }

        [Test]
        public async Task GetMyProfileAsync_ValidUser_ReturnsOkResultWithUserDto()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserDto { Id = userId, FirstName = "testuser", LastName = "", Email = "", Role = "" };
            _authServiceMock.Setup(s => s.GetMyProfileAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await _controller.GetMyProfileAsync(CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(userDto));
        }

        [Test]
        public void GetMyProfileAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "invalid") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.GetMyProfileAsync(CancellationToken.None));
        }

        [Test]
        public async Task RegisterAsync_ValidRegisterDto_ReturnsOkResultWithAccessTokenAndSetsRefreshTokenCookie()
        {
            // Arrange
            var registerDto = new RegisterDto { FirstName = "newuser", LastName = "", Email = "", Password = "password" };
            var authDto = new AuthDto { AccessToken = "fake_access_token", RefreshToken = "fake_refresh_token" };
            _authServiceMock.Setup(s => s.RegisterAsync(registerDto, It.IsAny<CancellationToken>())).ReturnsAsync(authDto);

            // Act
            var result = await _controller.RegisterAsync(registerDto);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(authDto.AccessToken));
            _responseCookiesMock.Verify(c => c.Append("refreshToken", authDto.RefreshToken, It.IsAny<CookieOptions>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_ValidLoginDto_ReturnsOkResultWithAccessTokenAndSetsRefreshTokenCookie()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "existinguser", Password = "password" };
            var authDto = new AuthDto { AccessToken = "fake_access_token", RefreshToken = "fake_refresh_token" };
            _authServiceMock.Setup(s => s.LoginAsync(loginDto, It.IsAny<CancellationToken>())).ReturnsAsync(authDto);

            // Act
            var result = await _controller.LoginAsync(loginDto);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(authDto.AccessToken));
            _responseCookiesMock.Verify(c => c.Append("refreshToken", authDto.RefreshToken, It.IsAny<CookieOptions>()), Times.Once);
        }

        [Test]
        public async Task RefreshAsync_ValidRefreshToken_ReturnsOkResultWithAccessTokenAndSetsRefreshTokenCookie()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var authDto = new AuthDto { AccessToken = "new_access_token", RefreshToken = "new_refresh_token" };
            _requestCookiesMock.Setup(c => c["refreshToken"]).Returns(refreshToken);
            _authServiceMock.Setup(s => s.RefreshAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(authDto);

            // Act
            var result = await _controller.RefreshAsync();

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(authDto.AccessToken));
            _responseCookiesMock.Verify(c => c.Append("refreshToken", authDto.RefreshToken, It.IsAny<CookieOptions>()), Times.Once);
        }

        [Test]
        public void RefreshAsync_MissingRefreshToken_ThrowsUnauthorizedException()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c["refreshToken"]).Returns((string)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedException>(async () => await _controller.RefreshAsync());
        }

        [Test]
        public async Task Logout_ValidUser_ReturnsOkResultAndClearsRefreshTokenCookie()
        {
            // Arrange
            var userId = 1;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;
            _authServiceMock.Setup(s => s.LogoutAsync(userId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _responseCookiesMock.Verify(c => c.Delete("refreshToken"), Times.Once);
        }

        [Test]
        public void Logout_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "invalid") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.Logout());
        }
    }
}