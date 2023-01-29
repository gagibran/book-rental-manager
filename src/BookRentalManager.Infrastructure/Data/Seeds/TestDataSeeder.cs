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
                    Email.Create("peter.grifin@email.com").Value!,
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
                new Author(FullName.Create("Lewis", "Carroll").Value!),
                new Author(FullName.Create("Franz", "Kafka").Value!),
                new Author(FullName.Create("Howard", "Lovecraft").Value!),
                new Author(FullName.Create("Isabel", "Allende").Value!),
                new Author(FullName.Create("Edgar Allan", "Poe").Value!),
                new Author(FullName.Create("Andy", "Hunt").Value!),
                new Author(FullName.Create("John Ronald", "Tolkien").Value!),
                new Author(FullName.Create("Stephen", "King").Value!),
                new Author(FullName.Create("George Raymond", "Martin").Value!),
                new Author(FullName.Create("Agatha", "Christie").Value!),
                new Author(FullName.Create("Leo", "Tolstoy").Value!),
                new Author(FullName.Create("William", "Shakespeare").Value!),
                new Author(FullName.Create("James", "Joyce").Value!),
                new Author(FullName.Create("Vladimir", "Nabokov").Value!),
                new Author(FullName.Create("Fyodor", "Dostoevsky").Value!),
                new Author(FullName.Create("William", "Faulkner").Value!),
                new Author(FullName.Create("Charles", "Dickens").Value!),
                new Author(FullName.Create("Anton", "Chekhov").Value!),
                new Author(FullName.Create("Gustave", "Flaubert").Value!),
                new Author(FullName.Create("Jane", "Austen").Value!),
                new Author(FullName.Create("Joanne", "Rowling").Value!),
                new Author(FullName.Create("Ernest", "Hemingway").Value!),
                new Author(FullName.Create("Maria", "Popova").Value!),
                new Author(FullName.Create("Francis Scott", "Fitzgerald").Value!),
                new Author(FullName.Create("Edgar Allan", "Poe").Value!),
                new Author(FullName.Create("Nora", "Roberts").Value!),
                new Author(FullName.Create("Paulo", "Coelho").Value!),
                new Author(FullName.Create("Roald", "Dahl").Value!),
                new Author(FullName.Create("George", "Orwell").Value!),
                new Author(FullName.Create("Georges", "Simenon").Value!),
                new Author(FullName.Create("Anne", "Frank").Value!),
                new Author(FullName.Create("Dan", "Brown").Value!),
                new Author(FullName.Create("Enid", "Blyton").Value!),
                new Author(FullName.Create("Gilbert", "Patten").Value!),
                new Author(FullName.Create("Beatrix", "Potter").Value!),
                new Author(FullName.Create("Karl", "May").Value!),
                new Author(FullName.Create("Rex", "Stout").Value!),
                new Author(FullName.Create("Yasuo", "Uchida").Value!),
                new Author(FullName.Create("Stephenie", "Meyer").Value!),
                new Author(FullName.Create("Anne", "Golon").Value!),
                new Author(FullName.Create("Nicholas", "Sparks").Value!),
                new Author(FullName.Create("Debbie", "Macomber").Value!),
                new Author(FullName.Create("Dr.", "Seuss").Value!),
                new Author(FullName.Create("Jane", "Austen").Value!),
                new Author(FullName.Create("Nora", "Roberts").Value!),
                new Author(FullName.Create("Ken", "Follett").Value!),
                new Author(FullName.Create("Patricia", "Cornwell").Value!),
                new Author(FullName.Create("Hermann", "Hesse").Value!),
                new Author(FullName.Create("Harold", "Robbins").Value!),
                new Author(FullName.Create("Sidney", "Sheldon").Value!),
                new Author(FullName.Create("Barbara", "Catland").Value!)
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
            var newBooks = new List<Book> { cleanCodeBook, designPatternsBook };
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
