namespace AuthorRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorByFullNameSpecificationTests
{
    private readonly Author _author;

    public AuthorByFullNameSpecificationTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingAuthorWithFullName_ReturnsTrue()
    {
        // Arrange:
        var authorByFullNameSpecification = new AuthorByFullNameSpecification(_author.FullName.FirstName, _author.FullName.LastName);

        // Act:
        bool isSatisfiedBy = authorByFullNameSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingAuthorWithFullName_ReturnsFalse()
    {
        // Arrange:
        var authorByFullNameSpecification = new AuthorByFullNameSpecification("Sarah", "Smith");

        // Act:
        bool isSatisfiedBy = authorByFullNameSpecification.IsSatisfiedBy(_author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
