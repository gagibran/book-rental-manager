namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookByIdFromAuthorSpecificationTests
{
    private readonly Book _book;

    public BookByIdFromAuthorSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var bookWithBooksByIdSpecification = new BookByIdFromAuthorSpecification(_book.Id);

        // Act:
        bool isSatisfiedBy = bookWithBooksByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var bookWithBooksByIdSpecification = new BookByIdFromAuthorSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = bookWithBooksByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
