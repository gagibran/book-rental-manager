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
            "   The Call of Cthulhu    ",
            Edition.Create(1).Value!,
            Isbn.Create("978-1548234355").Value!);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedBookTitle, book.BookTitle);
        Assert.Equal(new List<Author>(), book.Authors);
        Assert.Null(book.RentedAt);
        Assert.Null(book.DueDate);
    }

    [Theory]
    [InlineData(0, "0-201-61622-X", "The edition number can't be smaller than 1.")]
    [InlineData(1, "201-61622-X", "Invalid ISBN format.")]
    [InlineData(0, "201-61622-X", "The edition number can't be smaller than 1.|Invalid ISBN format.")]
    public void UpdateBookTitleEditionAndIsbn_WithIncorrectData_ReturnsErrorMessage(int edition, string isbn, string expectedErrorMessage)
    {
        // Act:
        Result updateBookTitleEditionAndIsbnResult = _book.UpdateBookTitleEditionAndIsbn(_book.BookTitle, edition, isbn);

        // Assert:
        Assert.Equal(expectedErrorMessage, updateBookTitleEditionAndIsbnResult.ErrorMessage);
    }

    [Theory]
    [InlineData(1, "2-201-61622-X")]
    [InlineData(2, "0-201-61622-X")]
    [InlineData(4, "3-201-61622-X")]
    public void UpdateBookTitleEditionAndIsbn_WithValidData_ReturnsCorrectUpdatedValues(int edition, string isbn)
    {
        // Act:
        Result updateBookTitleEditionAndIsbnResult = _book.UpdateBookTitleEditionAndIsbn(_book.BookTitle, edition, isbn);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(edition, _book.Edition.EditionNumber);
        Assert.Equal(isbn, _book.Isbn.IsbnValue);
    }
}
