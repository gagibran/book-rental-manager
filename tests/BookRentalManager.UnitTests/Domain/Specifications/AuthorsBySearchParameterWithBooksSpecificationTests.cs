namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorsBySearchParameterWithBooksSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "John",
            string.Empty,
            TestFixtures.CreateDummyAuthor(),
        };
        yield return new object[]
        {
            "sarah smith",
            string.Empty,
            new Author(FullName.Create("Sarah", "Smith").Value!)
        };
        yield return new object[]
        {
            "griffin",
            string.Empty,
            new Author(FullName.Create("Peter", "Griffin").Value!)
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {
        yield return new object[]
        {
            "234",
            string.Empty,
            TestFixtures.CreateDummyAuthor(),
        };
        yield return new object[]
        {
            "john@email.",
            string.Empty,
            new Author(FullName.Create("Sarah", "Smith").Value!)
        };
        yield return new object[]
        {
            "smith",
            string.Empty,
            new Author(FullName.Create("Peter", "Griffin").Value!)
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithAuthorsWithQuery_ReturnsTrue(string searchParameter, string sortParameters, Author author)
    {
        // Arrange:
        var authorsWithSearchParameterSpecification = new AuthorsBySearchParameterWithBooksSpecification(searchParameter, sortParameters);

        // Act:
        bool isSatisfiedBy = authorsWithSearchParameterSpecification.IsSatisfiedBy(author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutAuthorsWithQuery_ReturnsFalse(string searchParameter, string sortParameters, Author author)
    {
        // Arrange:
        var authorsWithSearchParameterSpecification = new AuthorsBySearchParameterWithBooksSpecification(searchParameter, sortParameters);

        // Act:
        bool isSatisfiedBy = authorsWithSearchParameterSpecification.IsSatisfiedBy(author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
