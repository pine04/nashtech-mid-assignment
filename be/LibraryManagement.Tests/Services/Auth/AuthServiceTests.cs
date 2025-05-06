using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Auth;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Auth
{
    public class AuthServiceTests
    {
        private AuthService _authService;
        private ApplicationDbContext _dbContext;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<IJwtIssuer> _jwtIssuerMock;
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)) // Give it a unique name
                .Options;

            // Create a new instance of ApplicationDbContext for each test
            _dbContext = new ApplicationDbContext(_dbContextOptions); // Pass null for config and seeder

            // Ensure the database is clean before each test.  This is CRUCIAL.
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _passwordHasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
            _jwtIssuerMock = new Mock<IJwtIssuer>(MockBehavior.Strict);

            _authService = new AuthService(_dbContext, _passwordHasherMock.Object, _jwtIssuerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            //  Dispose the db context
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetMyProfileAsync_ValidUserId_ReturnsUserDto()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = Role.NormalUser, Password = "" };

            // Add user directly to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            UserDto result = await _authService.GetMyProfileAsync(userId, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
            Assert.That(result.Email, Is.EqualTo("john.doe@example.com"));
            Assert.That(result.Role, Is.EqualTo("NormalUser"));
        }

        [Test]
        public void GetMyProfileAsync_InvalidUserId_ThrowsNotFoundException()
        {
            // Arrange
            int userId = 1;

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _authService.GetMyProfileAsync(userId, CancellationToken.None));
        }

        [Test]
        public async Task RegisterAsync_ValidRegisterDto_ReturnsAuthDtoWithTokens()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "test_password",
                Role = Role.NormalUser
            };

            _passwordHasherMock.Setup(h => h.Hash(registerDto.Password))
                .Returns("hashed_password");

            _jwtIssuerMock.Setup(j => j.IssueAccessToken(It.IsAny<User>()))
                .Returns("access_token");
            _jwtIssuerMock.Setup(j => j.IssueRefreshToken(It.IsAny<User>()))
                .Returns("refresh_token");
            _jwtIssuerMock.Setup(j => j.AccessTokenExpirationTime).Returns(15);
            _jwtIssuerMock.Setup(j => j.RefreshTokenExpirationTime).Returns(60);

            // Act
            AuthDto result = await _authService.RegisterAsync(registerDto, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.AccessToken, Is.EqualTo("access_token"));
            Assert.That(result.RefreshToken, Is.EqualTo("refresh_token"));

            // Verify that the user is actually in the database
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            Assert.IsNotNull(user);
            Assert.That(user.FirstName, Is.EqualTo(registerDto.FirstName));
            Assert.That(user.LastName, Is.EqualTo(registerDto.LastName));
            Assert.That(user.Email, Is.EqualTo(registerDto.Email));
            Assert.That(user.Password, Is.EqualTo("hashed_password"));
            Assert.That(user.Role, Is.EqualTo(registerDto.Role));

            _passwordHasherMock.Verify(h => h.Hash(registerDto.Password), Times.Once);
            _jwtIssuerMock.Verify(j => j.IssueAccessToken(It.IsAny<User>()), Times.Once);
            _jwtIssuerMock.Verify(j => j.IssueRefreshToken(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public void RegisterAsync_DuplicateEmail_ThrowsEmailAlreadyTakenException()
        {
            // Arrange
            var registerDto = new RegisterDto { Email = "john.doe@example.com", FirstName = "", LastName = "", Password = "" };
            var existingUser = new User { Email = "john.doe@example.com", FirstName = "", LastName = "", Password = "" };
            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            // Act & Assert
            Assert.ThrowsAsync<EmailAlreadyUsedException>(async () =>
                await _authService.RegisterAsync(registerDto, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_InvalidEmail_ThrowsWrongCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "any_password" };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _authService.LoginAsync(loginDto, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_InvalidPassword_ThrowsWrongCredentialsException()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "john.doe@example.com", Password = "wrong_password" };
            var user = new User { Id = 1, Email = "john.doe@example.com", Password = "hashed_password", FirstName = "", LastName = "" };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            _passwordHasherMock.Setup(h => h.Verify(loginDto.Password, "hashed_password"))
                .Returns(false); // Password is incorrect

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _authService.LoginAsync(loginDto, CancellationToken.None));
        }

        [Test]
        public void RefreshAsync_InvalidRefreshToken_ThrowsUnauthorizedException()
        {
            // Arrange
            string refreshTokenValue = "invalid_refresh_token";

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _authService.RefreshAsync(refreshTokenValue, CancellationToken.None));
        }

        [Test]
        public void RefreshAsync_ExpiredRefreshToken_ThrowsUnauthorizedException()
        {
            // Arrange
            string refreshTokenValue = "expired_refresh_token";
            var expiredToken = new Token
            {
                TokenId = 1,
                UserId = 1,
                TokenValue = refreshTokenValue,
                TokenType = TokenType.Refresh,
                Expires = DateTime.UtcNow.AddHours(-1) // Expired one hour ago
            };

            _dbContext.Tokens.Add(expiredToken);
            _dbContext.SaveChanges();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await _authService.RefreshAsync(refreshTokenValue, CancellationToken.None));
        }
    }
}
