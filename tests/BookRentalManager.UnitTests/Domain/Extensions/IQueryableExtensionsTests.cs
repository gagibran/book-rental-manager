using BookRentalManager.Domain.Extensions;

namespace BookRentalManager.UnitTests.Domain.Extensions;

public sealed class IQueryableExtensionsTests
{
    private readonly List<int> _numbers;

    public IQueryableExtensionsTests()
    {
        _numbers = new List<int> { 1, 2, 3 };
    }

    public static IEnumerable<object[]> GetOrderedBooksByParameters()
    {
        Book book1 = TestFixtures.CreateDummyBook();
        Book book2 = new Book(
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            Edition.Create(1).Value!,
            Isbn.Create("978-0132350884").Value!);
        Book book3 = new Book(
            "Design Patterns: Elements of Reusable Object-Oriented Software",
            Edition.Create(1).Value!,
            Isbn.Create("0-201-63361-2").Value!);
        Customer customer = TestFixtures.CreateDummyCustomer();
        Author author = TestFixtures.CreateDummyAuthor();
        author.AddBook(book1);
        author.AddBook(book2);
        author.AddBook(book3);
        customer.RentBook(book2);
        List<Book> books = new List<Book> { book1, book2, book3 };
        yield return new object[]
        {
            "",
            books,
            books
        };
        yield return new object[]
        {
            "BookTitleDesc,IsAvailable",
            books,
            books
                .OrderByDescending(book => book.BookTitle)
                .ThenBy(book => book.IsAvailable)
                .ToList()
        };
        yield return new object[]
        {
            "BookTitle",
            books,
            books.OrderBy(book => book.BookTitle).ToList()
        };
        yield return new object[]
        {
            "Edition.EditionNumberDesc",
            books,
            books.OrderByDescending(book => book.Edition.EditionNumber).ToList()
        };
        yield return new object[]
        {
            "Edition.EditionNumber,IsAvailableDesc",
            books,
            books
                .OrderBy(book => book.Edition.EditionNumber)
                .ThenByDescending(book => book.IsAvailable)
                .ToList()
        };
        yield return new object[]
        {
            "Isbn.IsbnValue",
            books,
            books.OrderBy(book => book.Isbn.IsbnValue).ToList()
        };
        yield return new object[]
        {
            "Edition.EditionNumberDesc,IsAvailable,BookTitleDesc,Isbn.IsbnValueDesc",
            books,
            books
                .OrderByDescending(book => book.Edition.EditionNumber)
                .ThenBy(book => book.IsAvailable)
                .ThenByDescending(book => book.BookTitle)
                .ThenByDescending(book => book.Isbn.IsbnValue)
                .ToList()
        };
    }

    [Fact]
    public void AsAsyncEnumerable_WithQueryNotAsyncEnumerable_ThrowsException()
    {
        // Assert:
        Assert.Throws<QueryableIsNotAsyncEnumerableException>(() => _numbers.AsQueryable().AsAsyncEnumerable());
    }

    [Fact]
    public void OrderByPropertyName_WithInvalidPropertyNames_ThrowsException()
    {
        // Assert:
        Assert.Throws<ArgumentException>(() => _numbers.AsQueryable().OrderByPropertyName("NotAValidProperty,AnotherInvalidOne"));
    }

    [Theory]
    [MemberData(nameof(GetOrderedBooksByParameters))]
    public void OrderByPropertyName_WithPropertyNames_ReturnsOrderedEntity(
        string propertyNamesSeparatedByComma,
        List<Book> unsortedBooks,
        List<Book> expectedSortedBooks)
    {
        // Act:
        List<Book> actualSortedBooks = unsortedBooks
            .AsQueryable()
            .OrderByPropertyName(propertyNamesSeparatedByComma)
            .ToList();

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedSortedBooks.ElementAt(0), actualSortedBooks.ElementAt(0));
        Assert.Equal(expectedSortedBooks.ElementAt(1), actualSortedBooks.ElementAt(1));
        Assert.Equal(expectedSortedBooks.ElementAt(2), actualSortedBooks.ElementAt(2));
    }
}
