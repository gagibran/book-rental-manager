namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksBySearchParameterInBooksFromAuthorSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "1",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "0-201-616",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "false",
            "",
            TestFixtures.CreateDummyAuthor()
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {

        yield return new object[]
        {
            "1984",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "5",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "345-6",
            "",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "true",
            "",
            TestFixtures.CreateDummyAuthor()
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter, string sortParameters, Author author)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        author.AddBook(book);
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksBySearchParameterInBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter, string sortParameters, Author author)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        author.AddBook(book);
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksBySearchParameterInBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
