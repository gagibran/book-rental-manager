namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksByIdsSpecificationTests
{
    private readonly Book _book;

    public BooksByIdsSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingIds_ReturnsTrue()
    {
        // Arrange:
        var id2 = Guid.NewGuid();
        var ids = new List<Guid> { _book.Id, id2 }.AsReadOnly();
        var booksByIdsSpecification = new BooksByIdsSpecification(ids);

        // Act:
        bool isSatisfiedBy = booksByIdsSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var ids = new List<Guid> { Guid.NewGuid() }.AsReadOnly();
        var booksByIdsSpecification = new BooksByIdsSpecification(ids);

        // Act:
        bool isSatisfiedBy = booksByIdsSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
