namespace LibraryManagement.Services.Auth
{
    public interface IPasswordHasher
    {
        public string Hash(string password);

        public bool Verify(string password, string storedPassword);
    }
}