namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookAuthorsWithSearchParamSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "John",
            TestFixtures.CreateDummyBookAuthor(),
        };
        yield return new object[]
        {
            "sarah smith",
            new BookAuthor(FullName.Create("Sarah", "Smith").Value)
        };
        yield return new object[]
        {
            "griffin",
            new BookAuthor(FullName.Create("Peter", "Griffin").Value)
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {
        yield return new object[]
        {
            "234",
            TestFixtures.CreateDummyBookAuthor(),
        };
        yield return new object[]
        {
            "john@email.",
            new BookAuthor(FullName.Create("Sarah", "Smith").Value)
        };
        yield return new object[]
        {
            "smith",
            new BookAuthor(FullName.Create("Peter", "Griffin").Value)
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithBookAuthorsWithQuery_ReturnsTrue(string searchParameter, BookAuthor bookAuthor)
    {
        // Arrange:
        var bookAuthorsWithSearchParameterSpecification = new BookAuthorsWithSearchParamSpecification(searchParameter);

        // Act:
        bool isSatisfiedBy = bookAuthorsWithSearchParameterSpecification.IsSatisfiedBy(bookAuthor);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBookAuthorsWithQuery_ReturnsFalse(string searchParameter, BookAuthor bookAuthor)
    {
        // Arrange:
        var bookAuthorsWithSearchParameterSpecification = new BookAuthorsWithSearchParamSpecification(searchParameter);

        // Act:
        bool isSatisfiedBy = bookAuthorsWithSearchParameterSpecification.IsSatisfiedBy(bookAuthor);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
