namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookAuthorByIdSpecificationTests
{
    private readonly BookAuthor _bookAuthor;

    public BookAuthorByIdSpecificationTests()
    {
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookAuthorByIdSpecification(_bookAuthor.Id);

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_bookAuthor);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var bookAuthorWithBookAuthorsByIdSpecification = new BookAuthorByIdSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = bookAuthorWithBookAuthorsByIdSpecification.IsSatisfiedBy(_bookAuthor);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
