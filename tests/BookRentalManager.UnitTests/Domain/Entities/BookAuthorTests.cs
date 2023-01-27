namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class BookAuthorTests
{
    private readonly Book _book;
    private readonly BookAuthor _bookAuthor;

    public BookAuthorTests()
    {
        _bookAuthor = new(FullName.Create("Eric", "Evans").Value);
        _book = new(
            "Domain-Driven Design: Tackling Complexity in the Heart of Software",
            Edition.Create(1).Value,
            Isbn.Create("978-0321125217").Value);
    }

    [Fact]
    public void AddBook_WithExistingBookWithIsbn_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "A book with the ISBN '978-0321125217' has already been added to this book author.";

        _bookAuthor.AddBook(_book);

        // Act:
        Result addBookResult = _bookAuthor.AddBook(_book);

        // Assert:
        Assert.Equal(expectedErrorMessage, addBookResult.ErrorMessage);
    }

    [Fact]
    public void AddBook_WithNonexistingBook_ReturnsAddedBook()
    {
        // Act:
        _bookAuthor.AddBook(_book);

        // Assert:
        Assert.Contains(_book, _bookAuthor.Books);
    }
}
