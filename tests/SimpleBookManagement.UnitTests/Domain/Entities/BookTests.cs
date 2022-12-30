namespace SimpleBookManagement.UnitTests.Domain.Entities;

public sealed class BookTests
{
    private readonly Book _book;

    public BookTests()
    {
        _book = new Book(
            "Domain-Driven Design: Tackling Complexity in the Heart of Software",
            new List<FullName> { FullName.Create("Eric", "Evans").Value },
            Volume.Create(1).Value,
            Isbn.Create(9780321125217).Value
        );
    }

    [Fact]
    public void AddBookAuthor_WithExistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        FullName author = FullName.Create("Eric", "Evans").Value;
        var expectedErrorMessage = $"Eric Evans has already been added as an author to this book.";

        // Act:
        Result addAuthorResult = _book.AddBookAuthor(author);

        // Assert:
        Assert.Equal(expectedErrorMessage, addAuthorResult.ErrorMessage);
    }

    [Fact]
    public void AddBookAuthor_WithNonexistingAuthor_ReturnsAddedAuthor()
    {
        // Arrange:
        FullName author = FullName.Create("John", "Doe").Value;

        // Act:
        _book.AddBookAuthor(author);

        // Assert:
        Assert.Contains(author, _book.BookAuthors);
    }
}
