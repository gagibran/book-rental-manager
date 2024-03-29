using BookRentalManager.Domain.ValueObjects;

namespace BookRentalManager.Infrastructure.Data.Seeds;

public sealed class TestDataSeeder
{
    private readonly BookRentalManagerDbContext _bookRentalManagerDbContext;

    public TestDataSeeder(BookRentalManagerDbContext bookRentalManagerDbContext)
    {
        _bookRentalManagerDbContext = bookRentalManagerDbContext;
    }

    public async Task SeedTestDataAsync()
    {
        DbSet<Customer> customers = _bookRentalManagerDbContext.Set<Customer>();
        DbSet<Book> books = _bookRentalManagerDbContext.Set<Book>();
        DbSet<Author> authors = _bookRentalManagerDbContext.Set<Author>();
        if (!customers.Any())
        {
            var newCustomers = new List<Customer>
            {
                new Customer(
                    FullName.Create("John", "Doe").Value!,
                    Email.Create("john.doe@email.com").Value!,
                    PhoneNumber.Create(200, 2000000).Value!),
                new Customer(
                    FullName.Create("Sarah", "Smith").Value!,
                    Email.Create("sarah.smith@email.com").Value!,
                    PhoneNumber.Create(235, 2204063).Value!),
                new Customer(
                    FullName.Create("Peter", "Griffin").Value!,
                    Email.Create("peter.griffin@email.com").Value!,
                    PhoneNumber.Create(546, 4056780).Value!)
            };
            await customers.AddRangeAsync(newCustomers);
        }
        if (!books.Any() && !authors.Any())
        {
            var newAuthors = new List<Author>
            {
                new Author(FullName.Create("Erich", "Gamma").Value!),
                new Author(FullName.Create("John", "Vlissides").Value!),
                new Author(FullName.Create("Ralph", "Johnson").Value!),
                new Author(FullName.Create("Richard", "Helm").Value!),
                new Author(FullName.Create("Bob", "Martin").Value!),
                new Author(FullName.Create("Howard", "Lovecraft").Value!),
                new Author(FullName.Create("Edgar Allan", "Poe").Value!),
            };
            var designPatternsBook = new Book(
                "Design Patterns: Elements of Reusable Object-Oriented Software",
                Edition.Create(1).Value!,
                Isbn.Create("0-201-63361-2").Value!);
            newAuthors[0].AddBook(designPatternsBook);
            newAuthors[1].AddBook(designPatternsBook);
            newAuthors[2].AddBook(designPatternsBook);
            newAuthors[3].AddBook(designPatternsBook);
            var cleanCodeBook = new Book(
                "Clean Code: A Handbook of Agile Software Craftsmanship",
                Edition.Create(1).Value!,
                Isbn.Create("978-0132350884").Value!);
            newAuthors[4].AddBook(cleanCodeBook);
            var callOfCthulhuBook = new Book(
                "The Call Of Cthulhu",
                Edition.Create(1).Value!,
                Isbn.Create("978-1515424437").Value!);
            var shadowOverInnsmouthBook = new Book(
                "The Shadow Over Innsmouth",
                Edition.Create(1).Value!,
                Isbn.Create("978-1878252180").Value!);
            newAuthors[5].AddBook(callOfCthulhuBook);
            newAuthors[5].AddBook(shadowOverInnsmouthBook);
            var newBooks = new List<Book>
            {
                cleanCodeBook,
                designPatternsBook,
                callOfCthulhuBook,
                shadowOverInnsmouthBook
            };
            var customer = new Customer(
                FullName.Create("Rosanne", "Johnson").Value!,
                Email.Create("rosane.johnson@email.com").Value!,
                PhoneNumber.Create(559, 7852361).Value!);
            customer.RentBook(cleanCodeBook);
            await authors.AddRangeAsync(newAuthors);
            await books.AddRangeAsync(newBooks);
            await customers.AddAsync(customer);
        }
        await _bookRentalManagerDbContext.SaveChangesAsync();
    }
}
