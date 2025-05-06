using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        private IConfiguration _configuration;
        private IApplicationSeeder _applicationSeeder;

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public ApplicationDbContext(IConfiguration configuration, IApplicationSeeder applicationSeeder)
        {
            _configuration = configuration;
            _applicationSeeder = applicationSeeder;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _configuration != null)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionString"])
                    .UseSeeding((context, _) =>
                    {
                        _applicationSeeder.Seed(context);
                    })
                    .UseAsyncSeeding(async (context, _, cancellationToken) =>
                    {
                        await _applicationSeeder.SeedAsync(context, cancellationToken);
                    });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BookBorrowingRequest>()
                .HasMany(r => r.Books)
                .WithMany(b => b.BookBorrowingRequests)
                .UsingEntity<BookBorrowingRequestDetail>();

            modelBuilder.Entity<BookBorrowingRequestDetail>()
                .ToTable("BookBorrowingRequestDetails");

            modelBuilder.Entity<Token>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId);
        }
    }
}