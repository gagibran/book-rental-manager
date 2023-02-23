namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookByIdWithAuthorsAndCustomersSpecificationTests
{
    private readonly Book _book;

    public BookByIdWithAuthorsAndCustomersSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(_book.Id);

        // Act:
        bool isSatisfiedBy = bookByIdWithAuthorsAndCustomersSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = bookByIdWithAuthorsAndCustomersSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
