using BookRentalManager.Domain.Extensions;

namespace BookRentalManager.UnitTests.Domain.Extensions;

public sealed class IQueryableExtensionsTests
{
    private readonly List<int> _numbers;

    public IQueryableExtensionsTests()
    {
        _numbers = [1, 2, 3];
    }

    public static IEnumerable<object[]> GetOrderedBooksByParameters()
    {
        Book book1 = TestFixtures.CreateDummyBook();
        Book book2 = new(
            BookTitle.Create("Clean Code: A Handbook of Agile Software Craftsmanship").Value!,
            Edition.Create(1).Value!,
            Isbn.Create("978-0132350884").Value!);
        Book book3 = new(
            BookTitle.Create("Design Patterns: Elements of Reusable Object-Oriented Software").Value!,
            Edition.Create(1).Value!,
            Isbn.Create("0-201-63361-2").Value!);
        Customer customer = TestFixtures.CreateDummyCustomer();
        Author author = TestFixtures.CreateDummyAuthor();
        author.AddBook(book1);
        author.AddBook(book2);
        author.AddBook(book3);
        customer.RentBook(book2);
        List<Book> books = [book1, book2, book3];
        yield return new object[]
        {
            "BookTitle.TitleDesc,DueDate",
            books,
            books
                .OrderByDescending(book => book.BookTitle.Title)
                .ThenBy(book => book.DueDate)
                .ToList()
        };
        yield return new object[]
        {
            "BookTitle.Title",
            books,
            books.OrderBy(book => book.BookTitle.Title).ToList()
        };
        yield return new object[]
        {
            "Edition.EditionNumberDesc",
            books,
            books.OrderByDescending(book => book.Edition.EditionNumber).ToList()
        };
        yield return new object[]
        {
            "Edition.EditionNumber,RentedAtDesc",
            books,
            books
                .OrderBy(book => book.Edition.EditionNumber)
                .ThenByDescending(book => book.RentedAt)
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
            "Edition.EditionNumberDesc,RentedAt,BookTitle.TitleDesc,Isbn.IsbnValueDesc",
            books,
            books
                .OrderByDescending(book => book.Edition.EditionNumber)
                .ThenBy(book => book.RentedAt)
                .ThenByDescending(book => book.BookTitle.Title)
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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("NotAValidProperty,AnotherInvalidOne")]
    public void OrderByPropertyName_WithInvalidPropertyNames_ThrowsException(string propertyNamesSeparatedByComma)
    {
        // Assert:
        Assert.Throws<ArgumentException>(() => _numbers.AsQueryable().OrderByPropertyName(propertyNamesSeparatedByComma));
    }

    [Theory]
    [MemberData(nameof(GetOrderedBooksByParameters))]
    public void OrderByPropertyName_WithPropertyNames_ReturnsOrderedEntity(
        string propertyNamesSeparatedByComma,
        List<Book> unsortedBooks,
        List<Book> expectedSortedBooks)
    {
        // Act:
        List<Book> actualSortedBooks =
        [
            ..unsortedBooks
                .AsQueryable()
                .OrderByPropertyName(propertyNamesSeparatedByComma)
        ];

        // Assert:
        Assert.True(expectedSortedBooks.SequenceEqual(actualSortedBooks));
    }
}
