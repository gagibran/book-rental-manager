using BookRentalManager.Infrastructure.Data.Config;

namespace BookRentalManager.Infrastructure.Data;

public sealed class BookRentalManagerDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<Author> Authors { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookEntityBuilder());
        modelBuilder.ApplyConfiguration(new AuthorEntityBuilder());
    }
}
