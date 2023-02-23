namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorByIdWithBooksSpecificationTests
{
    private readonly Author _author;

    public AuthorByIdWithBooksSpecificationTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var authorWithAuthorsByIdSpecification = new AuthorByIdWithBooksSpecification(_author.Id);

        // Act:
        bool isSatisfiedBy = authorWithAuthorsByIdSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var authorWithAuthorsByIdSpecification = new AuthorByIdWithBooksSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = authorWithAuthorsByIdSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
