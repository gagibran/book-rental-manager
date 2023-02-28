namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorsByIdsSpecificationTests
{
    private readonly Author _author;

    public AuthorsByIdsSpecificationTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingIds_ReturnsTrue()
    {
        // Arrange:
        var id2 = Guid.NewGuid();
        var ids = new List<Guid> { _author.Id, id2 }.AsReadOnly();
        var booksByIdsSpecification = new AuthorsByIdsSpecification(ids);

        // Act:
        bool isSatisfiedBy = booksByIdsSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var ids = new List<Guid> { Guid.NewGuid() }.AsReadOnly();
        var booksByIdsSpecification = new AuthorsByIdsSpecification(ids);

        // Act:
        bool isSatisfiedBy = booksByIdsSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
