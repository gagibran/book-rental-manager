namespace BookRentalManager.Domain.Specifications;

public sealed class BookInBookAuthorBooksByIsbnSpecificationTests : Specification<Book>
{
    private readonly IReadOnlyList<Book> _books;

    public BookInBookAuthorBooksByIsbnSpecificationTests()
    {
        _books = new List<Book>
        {
            TestFixtures.CreateDummyBook(),
            new Book("The Call of Cthulhu", Edition.Create(1).Value, Isbn.Create("978-1975628079").Value),
            new Book("Coraline", Edition.Create(1).Value, Isbn.Create("978-0380977789").Value),
            new Book("Neuromancer", Edition.Create(1).Value, Isbn.Create("978-0143111603").Value),
        };
    }

    [Theory]
    [InlineData("0-201-61622-X", 0)]
    [InlineData("978-1975628079", 1)]
    [InlineData("978-0380977789", 2)]
    [InlineData("978-0143111603", 3)]
    public void IsSatisfiedBy_WithExistingBookAndCorrectBookTitle_ReturnsTrue(string isbn, int bookIndex)
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByIsbnSpecification(_books, isbn);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_books[bookIndex]);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [InlineData("978-1975628079", 0)]
    [InlineData("0-201-61622-X", 1)]
    [InlineData("978-0143111603", 2)]
    [InlineData("978-0380977789", 3)]
    public void IsSatisfiedBy_WithExistingBookButDifferentBookTitle_ReturnsFalse(string isbnValue, int bookIndex)
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByIsbnSpecification(_books, isbnValue);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_books[bookIndex]);

        // Assert:
        Assert.False(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingBookAndCorrectBookTitle_ReturnsFalse()
    {
        // Arrange:
        Isbn isbn = Isbn.Create("978-0575094185").Value;
        var book = new Book("Do Androids Dream Of Electric Sheep?", Edition.Create(1).Value, isbn);
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByIsbnSpecification(_books, isbn.IsbnValue);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
