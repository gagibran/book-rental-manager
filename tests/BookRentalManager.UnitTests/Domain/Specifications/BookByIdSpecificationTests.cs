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
        var bookByIdSpecification = new BookByIdSpecification(_book.Id);

        // Act:
        bool isSatisfiedBy = bookByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var bookByIdSpecification = new BookByIdSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = bookByIdSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
