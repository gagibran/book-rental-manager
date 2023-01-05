using BookRentalManager.Domain.ValueObjects;

namespace BookRentalManager.Infrastructure.Data.Seeds;

public sealed class TestDataSeeder
{
    private readonly AppDbContext _appDbContext;

    public TestDataSeeder(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task SeedTestDataAsync()
    {
        DbSet<Customer> customers = _appDbContext.Set<Customer>();
        DbSet<Book> books = _appDbContext.Set<Book>();
        DbSet<BookAuthor> bookAuthors = _appDbContext.Set<BookAuthor>();
        if (!customers.Any())
        {
            var newCustomers = new List<Customer>
            {
                new Customer(
                    FullName.Create("John", "Doe").Value!,
                    Email.Create("john.doe@email.com").Value!,
                    PhoneNumber.Create(200, 2000000).Value!
                ),
                new Customer(
                    FullName.Create("Sarah", "Smith").Value!,
                    Email.Create("sarah.smith@email.com").Value!,
                    PhoneNumber.Create(235, 2204063).Value!
                ),
                new Customer(
                    FullName.Create("Peter", "Griffin").Value!,
                    Email.Create("peter.grifin@email.com").Value!,
                    PhoneNumber.Create(546, 4056780).Value!
                ),
            };
            await customers.AddRangeAsync(newCustomers);
        }
        if (!books.Any() && !bookAuthors.Any())
        {
            var newBookAuthors = new List<BookAuthor>
            {
                new BookAuthor(
                    FullName.Create("Robert", "Martin").Value!
                ),
                new BookAuthor(
                    FullName.Create("Erich", "Gamma").Value!
                ),
                new BookAuthor(
                    FullName.Create("Richard", "Helm").Value!
                ),
                new BookAuthor(
                    FullName.Create("Ralph", "Johnson").Value!
                ),
                new BookAuthor(
                    FullName.Create("John", "Vlissides").Value!
                )
            };
            var designPatterns = new Book(
                "Design Patterns: Elements of Reusable Object-Oriented Software",
                Edition.Create(1).Value!,
                Isbn.Create("0-201-63361-2").Value!
            );
            designPatterns.AddBookAuthor(newBookAuthors[1]);
            designPatterns.AddBookAuthor(newBookAuthors[2]);
            designPatterns.AddBookAuthor(newBookAuthors[3]);
            designPatterns.AddBookAuthor(newBookAuthors[4]);
            var cleanCode = new Book(
                "Clean Code: A Handbook of Agile Software Craftsmanship",
                Edition.Create(1).Value!,
                Isbn.Create("978-0132350884").Value!
            );
            cleanCode.AddBookAuthor(newBookAuthors[0]);
            var newBooks = new List<Book> { cleanCode, designPatterns };
            await bookAuthors.AddRangeAsync(newBookAuthors);
            await books.AddRangeAsync(newBooks);
        }
        await _appDbContext.SaveChangesAsync();
    }
}
