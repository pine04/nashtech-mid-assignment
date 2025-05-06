using LibraryManagement.Services.Auth;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Auth
{
    public class PasswordHasherTests
    {
        private IPasswordHasher _passwordHasher;

        [SetUp]
        public void Setup()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Test]
        public void Hash_ValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            string password = "test_password";

            // Act
            string hashedPassword = _passwordHasher.Hash(password);

            // Assert
            Assert.IsNotEmpty(hashedPassword);
            Assert.AreNotEqual(password, hashedPassword);
            Assert.That(hashedPassword.Contains("-"));
        }

        [Test]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "test_password";
            string hashedPassword = _passwordHasher.Hash(password);

            // Act
            bool result = _passwordHasher.Verify(password, hashedPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Verify_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string correctPassword = "test_password";
            string incorrectPassword = "wrong_password";
            string hashedPassword = _passwordHasher.Hash(correctPassword);

            // Act
            bool result = _passwordHasher.Verify(incorrectPassword, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Verify_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            string password = "test_password";
            string hashedPassword = _passwordHasher.Hash(password);

            // Act
            bool result = _passwordHasher.Verify(string.Empty, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Verify_EmptyHashedPassword_ReturnsFalse()
        {
            // Arrange
            string password = "test_password";

            // Act
            bool result = _passwordHasher.Verify(password, string.Empty);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
