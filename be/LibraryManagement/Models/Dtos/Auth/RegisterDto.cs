using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Auth
{
    public class RegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Role Role { get; set; }
    }
}