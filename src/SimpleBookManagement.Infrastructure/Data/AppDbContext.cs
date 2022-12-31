namespace SimpleBookManagement.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Book> Books { get; set; }

    public AppDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityBuilder());
        modelBuilder.ApplyConfiguration(new BookEntityBuilder());
    }
}
