namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class AuthorTests
{
    private readonly Book _book;
    private readonly Author _author;

    public AuthorTests()
    {
        _author = new(FullName.Create("Eric", "Evans").Value);
        _book = new(
            "Domain-Driven Design: Tackling Complexity in the Heart of Software",
            Edition.Create(1).Value,
            Isbn.Create("978-0321125217").Value);
    }

    [Fact]
    public void AddBook_WithExistingBookWithIsbn_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "A book with the ISBN '978-0321125217' has already been added to this author.";

        _author.AddBook(_book);

        // Act:
        Result addBookResult = _author.AddBook(_book);

        // Assert:
        Assert.Equal(expectedErrorMessage, addBookResult.ErrorMessage);
    }

    [Fact]
    public void AddBook_WithNonexistingBook_ReturnsAddedBook()
    {
        // Act:
        _author.AddBook(_book);

        // Assert:
        Assert.Contains(_book, _author.Books);
    }
}
