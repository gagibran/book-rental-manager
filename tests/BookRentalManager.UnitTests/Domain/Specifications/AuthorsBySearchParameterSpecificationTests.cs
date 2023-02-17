namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class AuthorsBySearchParameterSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "John",
            "",
            TestFixtures.CreateDummyAuthor(),
        };
        yield return new object[]
        {
            "sarah smith",
            "",
            new Author(FullName.Create("Sarah", "Smith").Value)
        };
        yield return new object[]
        {
            "griffin",
            "",
            new Author(FullName.Create("Peter", "Griffin").Value)
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {
        yield return new object[]
        {
            "234",
            "",
            TestFixtures.CreateDummyAuthor(),
        };
        yield return new object[]
        {
            "john@email.",
            "",
            new Author(FullName.Create("Sarah", "Smith").Value)
        };
        yield return new object[]
        {
            "smith",
            "",
            new Author(FullName.Create("Peter", "Griffin").Value)
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithAuthorsWithQuery_ReturnsTrue(string searchParameter, string sortParameter, Author author)
    {
        // Arrange:
        var authorsWithSearchParameterSpecification = new AuthorsBySearchParameterSpecification(searchParameter, sortParameter);

        // Act:
        bool isSatisfiedBy = authorsWithSearchParameterSpecification.IsSatisfiedBy(author);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutAuthorsWithQuery_ReturnsFalse(string searchParameter, string sortParameter, Author author)
    {
        // Arrange:
        var authorsWithSearchParameterSpecification = new AuthorsBySearchParameterSpecification(searchParameter, sortParameter);

        // Act:
        bool isSatisfiedBy = authorsWithSearchParameterSpecification.IsSatisfiedBy(author);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
