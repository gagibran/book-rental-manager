namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecificationTests
{
    private readonly Author _author;
    private readonly Book _book;
    private readonly Customer _customer;

    public BooksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecificationTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _book = TestFixtures.CreateDummyBook();
        _customer = TestFixtures.CreateDummyCustomer();
        _customer.RentBook(_book);
    }

    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "1",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "0-201-616",
            TestFixtures.CreateDummyAuthor()
        };
        yield return new object[]
        {
            "false",
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
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter, Author author)
    {
        // Arrange:
        var booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification = new BooksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            string.Empty);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter, string sortParameters, Author author)
    {
        // Arrange:
        _author.AddBook(_book);
        var booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification = new BooksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification(
            author.Books,
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
