namespace BookRentalManager.Domain.Specifications;

public sealed class BookInBookAuthorBooksByTitleSpecificationTests : Specification<Book>
{
    private readonly IReadOnlyList<Book> _books;

    public BookInBookAuthorBooksByTitleSpecificationTests()
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
    [InlineData("The Pragmatic Programmer: From Journeyman to Master", 0)]
    [InlineData("The Call of Cthulhu", 1)]
    [InlineData("Coraline", 2)]
    [InlineData("Neuromancer", 3)]
    public void IsSatisfiedBy_WithExistingBookAndCorrectBookTitle_ReturnsTrue(string bookTitle, int bookIndex)
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByTitleSpecification(_books, bookTitle);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_books[bookIndex]);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [InlineData("The Call of Cthulhu", 0)]
    [InlineData("The Pragmatic Programmer: From Journeyman to Master", 1)]
    [InlineData("Neuromancer", 2)]
    [InlineData("Coraline", 3)]
    public void IsSatisfiedBy_WithExistingBookButDifferentBookTitle_ReturnsFalse(string bookTitle, int bookIndex)
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByTitleSpecification(_books, bookTitle);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_books[bookIndex]);

        // Assert:
        Assert.False(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingBookAndCorrectBookTitle_ReturnsFalse()
    {
        // Arrange:
        var bookTitle = "Do Androids Dream Of Electric Sheep?";
        var book = new Book(bookTitle, Edition.Create(1).Value, Isbn.Create("978-0575094185").Value);
        var bookAuthorWithBookAuthorsByIdSpecification = new BookInBookAuthorBooksByTitleSpecification(_books, bookTitle);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
