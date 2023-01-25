using BookRentalManager.Infrastructure.Data.Config;

namespace BookRentalManager.Infrastructure.Data;

public sealed class BookRentalManagerDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }

    public BookRentalManagerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        Customers = default!;
        Books = default!;
        BookAuthors = default!;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookAuthorEntityBuilder());
    }
}
