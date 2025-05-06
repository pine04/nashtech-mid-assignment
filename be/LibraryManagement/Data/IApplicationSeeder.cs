using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public interface IApplicationSeeder
    {
        public void Seed(DbContext context);
        public Task SeedAsync(DbContext context, CancellationToken cancellationToken);
    }
}
