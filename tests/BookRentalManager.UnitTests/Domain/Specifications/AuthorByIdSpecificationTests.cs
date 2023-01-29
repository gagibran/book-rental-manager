namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorByIdSpecificationTests
{
    private readonly Author _author;

    public AuthorByIdSpecificationTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var authorWithAuthorsByIdSpecification = new AuthorByIdSpecification(_author.Id);

        // Act:
        bool isSatisfiedBy = authorWithAuthorsByIdSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var authorWithAuthorsByIdSpecification = new AuthorByIdSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = authorWithAuthorsByIdSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
