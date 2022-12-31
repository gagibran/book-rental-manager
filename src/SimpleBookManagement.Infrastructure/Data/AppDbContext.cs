namespace SimpleBookManagement.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<BookAuthor> BookAuthors { get; set; } = default!;

    public AppDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookAuthorEntityBuilder());
    }
}
