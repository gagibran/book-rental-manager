namespace BookRentalManager.UnitTests;

public class BookByBookTitleEditionAndIsbnSpecificationTests
{
    private readonly Book _book;

    public BookByBookTitleEditionAndIsbnSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingEmail_ReturnsTrue()
    {
        // Arrange:
        var bookByBookTitleEditionAndIsbnSpecification = new BookByTitleEditionAndIsbnSpecification(
            "The Pragmatic Programmer: From Journeyman to Master",
            1,
            "0-201-61622-X");

        // Act:
        bool isSatisfiedBy = bookByBookTitleEditionAndIsbnSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [InlineData("The Pragmatic Programmer: From Journeyman to Master", 1, "0-201-61622-Y")]
    [InlineData("The Pragmatic Programmer: From Journeyman to Master", 2, "0-201-61622-X")]
    [InlineData("The Pragmatic Programmer: From Journeyman to Masters", 1, "0-201-61622-X")]
    [InlineData("The Pragmatic Programmer: From Journeyman to Master", 4, "0-201-63622-X")]
    [InlineData("The Pragmatic Programmer: From Journeyman", 1, "0-201-61552-X")]
    [InlineData("The Pragmatic Programmer", 5, "1-201-99876-X")]
    public void IsSatisfiedBy_WithNonexistingEmail_ReturnsFalse(string bookTitle, int edition, string isbn)
    {
        // Arrange:
        var bookByBookTitleEditionAndIsbnSpecification = new BookByTitleEditionAndIsbnSpecification(bookTitle, edition, isbn);

        // Act:
        bool isSatisfiedBy = bookByBookTitleEditionAndIsbnSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
