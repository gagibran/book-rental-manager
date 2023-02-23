namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "1",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "0-201-616",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "false",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {

        yield return new object[]
        {
            "1984",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "5",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "345-6",
            string.Empty,
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "true",
            string.Empty,
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
        var booksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification = new BooksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification.IsSatisfiedBy(book);

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
        var booksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification = new BooksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersInBooksFromAuthorSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
