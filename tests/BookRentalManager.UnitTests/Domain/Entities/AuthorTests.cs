namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class AuthorTests
{
    private readonly Book _book;
    private readonly Author _author;

    public AuthorTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void AddBook_WithExistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"A book with the ISBN '{_book.Isbn}' has already been added to this author.";
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
