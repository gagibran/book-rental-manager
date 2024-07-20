namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class BookTests
{
    private readonly Book _book;

    public BookTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void Book_WithCorrectValues_ReturnsBook()
    {
        // Arrange:
        var expectedBookTitle = "The Call of Cthulhu";

        // Act:
        var book = new Book(
            BookTitle.Create("   The Call of Cthulhu    ").Value!,
            Edition.Create(1).Value!,
            Isbn.Create("978-1548234355").Value!);

        // Assert:
        Assert.Equal(expectedBookTitle, book.BookTitle.Title);
        Assert.Equal([], book.Authors);
        Assert.Null(book.RentedAt);
        Assert.Null(book.DueDate);
    }

    [Theory]
    [InlineData("A Cool Book", 0, "0-201-61622-X", "editionNumber", "The edition number can't be smaller than 1.")]
    [InlineData("", 1, "0-201-61622-X", "bookTitle", "The title can't be empty.")]
    [InlineData("A Cool Book", 1, "201-61622-X", "isbnFormat", "Invalid ISBN format.")]
    [InlineData("A Cool Book", 0, "201-61622-X", "editionNumber|isbnFormat", "The edition number can't be smaller than 1.|Invalid ISBN format.")]
    public void UpdateBookTitleEditionAndIsbn_WithIncorrectData_ReturnsErrorMessage(
        string bookTitle,
        int edition,
        string isbn,
        string errorType,
        string expectedErrorMessage)
    {
        // Act:
        Result updateBookTitleEditionAndIsbnResult = _book.UpdateBookTitleEditionAndIsbn(bookTitle, edition, isbn);

        // Assert:
        Assert.Equal(errorType, updateBookTitleEditionAndIsbnResult.ErrorType);
        Assert.Equal(expectedErrorMessage, updateBookTitleEditionAndIsbnResult.ErrorMessage);
    }

    [Theory]
    [InlineData("A Cool Book", 1, "2-201-61622-X")]
    [InlineData("A Cool Book", 2, "0-201-61622-X")]
    [InlineData("A Cool Book", 4, "3-201-61622-X")]
    public void UpdateBookTitleEditionAndIsbn_WithValidData_ReturnsCorrectUpdatedValues(string bookTitle, int edition, string isbn)
    {
        // Act:
        _book.UpdateBookTitleEditionAndIsbn(bookTitle, edition, isbn);

        // Assert:
        Assert.Equal(edition, _book.Edition.EditionNumber);
        Assert.Equal(isbn, _book.Isbn.IsbnValue);
    }
}
