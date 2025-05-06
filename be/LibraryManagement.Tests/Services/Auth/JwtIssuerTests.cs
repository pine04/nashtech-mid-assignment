using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Auth;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Auth
{
    public class JwtIssuerTests
    {
        private IJwtIssuer _jwtIssuer;
        private User _testUser;

        [SetUp]
        public void Setup()
        {
            _jwtIssuer = new JwtIssuer(); // Use the actual implementation
            _testUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = ""
            };
        }

        [Test]
        public void IssueAccessToken_ValidUser_ReturnsValidJwtToken()
        {
            // Act
            string accessToken = _jwtIssuer.IssueAccessToken(_testUser);

            // Assert
            Assert.IsNotEmpty(accessToken);
            Assert.DoesNotThrow(() => new JwtSecurityTokenHandler().ReadJwtToken(accessToken)); // Check it's a valid JWT

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            Assert.AreEqual(_testUser.Id.ToString(), token.Subject);
            Assert.IsTrue(token.ValidTo > DateTime.UtcNow);
        }

        [Test]
        public void IssueAccessToken_ValidUser_ReturnsTokenWithCorrectClaims()
        {
            // Act
            string accessToken = _jwtIssuer.IssueAccessToken(_testUser);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            // Assert
            Assert.IsTrue(token.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.ToString()));
        }

        [Test]
        public void IssueRefreshToken_ValidUser_ReturnsValidJwtToken()
        {
            // Act
            string refreshToken = _jwtIssuer.IssueRefreshToken(_testUser);

            // Assert
            Assert.IsNotEmpty(refreshToken);
            Assert.DoesNotThrow(() => new JwtSecurityTokenHandler().ReadJwtToken(refreshToken)); // Check it's a valid JWT

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(refreshToken);

            Assert.IsTrue(token.ValidTo > DateTime.UtcNow);
        }

        [Test]
        public void IssueRefreshToken_ValidUser_ReturnsTokenWithCorrectClaims()
        {
            // Act
            string refreshToken = _jwtIssuer.IssueRefreshToken(_testUser);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(refreshToken);

            // Assert
            Assert.IsTrue(token.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.ToString()));
            Assert.IsTrue(token.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti));
        }

        [Test]
        public void AccessTokenExpirationTime_ReturnsCorrectValue()
        {
            // Act
            int expirationTime = _jwtIssuer.AccessTokenExpirationTime;

            // Assert
            Assert.AreEqual(15, expirationTime); // Make sure this matches the value in your JwtIssuer class
        }

        [Test]
        public void RefreshTokenExpirationTime_ReturnsCorrectValue()
        {
            // Act
            int expirationTime = _jwtIssuer.RefreshTokenExpirationTime;

            // Assert
            Assert.AreEqual(60, expirationTime);  // Make sure this matches the value in your JwtIssuer class
        }
    }
}
