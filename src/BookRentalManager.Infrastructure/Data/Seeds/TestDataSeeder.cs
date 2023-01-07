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
                )
            };
            await customers.AddRangeAsync(newCustomers);
        }
        if (!books.Any() && !bookAuthors.Any())
        {
            var newBookAuthors = new List<BookAuthor>
            {
                new BookAuthor(FullName.Create("Lewis", "Carroll").Value!),
                new BookAuthor(FullName.Create("Franz", "Kafka").Value!),
                new BookAuthor(FullName.Create("Howard", "Lovecraft").Value!),
                new BookAuthor(FullName.Create("Isabel", "Allende").Value!),
                new BookAuthor(FullName.Create("Edgar Allan", "Poe").Value!),
                new BookAuthor(FullName.Create("Andy", "Hunt").Value!),
                new BookAuthor(FullName.Create("John Ronald", "Tolkien").Value!),
                new BookAuthor(FullName.Create("Stephen", "King").Value!),
                new BookAuthor(FullName.Create("George Raymond", "Martin").Value!),
                new BookAuthor(FullName.Create("Agatha", "Christie").Value!),
                new BookAuthor(FullName.Create("Leo", "Tolstoy").Value!),
                new BookAuthor(FullName.Create("William", "Shakespeare").Value!),
                new BookAuthor(FullName.Create("James", "Joyce").Value!),
                new BookAuthor(FullName.Create("Vladimir", "Nabokov").Value!),
                new BookAuthor(FullName.Create("Fyodor", "Dostoevsky").Value!),
                new BookAuthor(FullName.Create("William", "Faulkner").Value!),
                new BookAuthor(FullName.Create("Charles", "Dickens").Value!),
                new BookAuthor(FullName.Create("Anton", "Chekhov").Value!),
                new BookAuthor(FullName.Create("Gustave", "Flaubert").Value!),
                new BookAuthor(FullName.Create("Jane", "Austen").Value!),
                new BookAuthor(FullName.Create("Joanne", "Rowling").Value!),
                new BookAuthor(FullName.Create("Ernest", "Hemingway").Value!),
                new BookAuthor(FullName.Create("Maria", "Popova").Value!),
                new BookAuthor(FullName.Create("Francis Scott", "Fitzgerald").Value!),
                new BookAuthor(FullName.Create("Edgar Allan", "Poe").Value!),
                new BookAuthor(FullName.Create("Nora", "Roberts").Value!),
                new BookAuthor(FullName.Create("Paulo", "Coelho").Value!),
                new BookAuthor(FullName.Create("Roald", "Dahl").Value!),
                new BookAuthor(FullName.Create("George", "Orwell").Value!),
                new BookAuthor(FullName.Create("Georges", "Simenon").Value!),
                new BookAuthor(FullName.Create("Anne", "Frank").Value!),
                new BookAuthor(FullName.Create("Dan", "Brown").Value!),
                new BookAuthor(FullName.Create("Enid", "Blyton").Value!),
                new BookAuthor(FullName.Create("Gilbert", "Patten").Value!),
                new BookAuthor(FullName.Create("Beatrix", "Potter").Value!),
                new BookAuthor(FullName.Create("Karl", "May").Value!),
                new BookAuthor(FullName.Create("Rex", "Stout").Value!),
                new BookAuthor(FullName.Create("Yasuo", "Uchida").Value!),
                new BookAuthor(FullName.Create("Stephenie", "Meyer").Value!),
                new BookAuthor(FullName.Create("Anne", "Golon").Value!),
                new BookAuthor(FullName.Create("Nicholas", "Sparks").Value!),
                new BookAuthor(FullName.Create("Debbie", "Macomber").Value!),
                new BookAuthor(FullName.Create("Dr.", "Seuss").Value!),
                new BookAuthor(FullName.Create("Jane", "Austen").Value!),
                new BookAuthor(FullName.Create("Nora", "Roberts").Value!),
                new BookAuthor(FullName.Create("Ken", "Follett").Value!),
                new BookAuthor(FullName.Create("Patricia", "Cornwell").Value!),
                new BookAuthor(FullName.Create("Hermann", "Hesse").Value!),
                new BookAuthor(FullName.Create("Harold", "Robbins").Value!),
                new BookAuthor(FullName.Create("Sidney", "Sheldon").Value!),
                new BookAuthor(FullName.Create("Barbara", "Catland").Value!)
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
