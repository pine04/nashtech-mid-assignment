namespace LibraryManagement.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Role Role { get; set; }
    }

    public enum Role
    {
        NormalUser,
        SuperUser
    }
}