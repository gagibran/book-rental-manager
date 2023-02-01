namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookByIdSpecificationTests
{
    private readonly Book _book;

    public BookByIdSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var bookWithBooksByIdSpecification = new BookByIdSpecification(_book.Id);

        // Act:
        bool isSatisfiedBy = bookWithBooksByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var bookWithBooksByIdSpecification = new BookByIdSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = bookWithBooksByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
