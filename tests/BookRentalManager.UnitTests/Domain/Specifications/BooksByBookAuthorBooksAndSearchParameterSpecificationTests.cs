namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksByBookAuthorBooksAndSearchParameterSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "1",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "0-201-616",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "false",
            TestFixtures.CreateDummyBookAuthor()
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {

        yield return new object[]
        {
            "1984",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "5",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "345-6",
            TestFixtures.CreateDummyBookAuthor()
        };
        yield return new object[]
        {
            "true",
            TestFixtures.CreateDummyBookAuthor()
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter, BookAuthor bookAuthor)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        bookAuthor.AddBook(book);
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksByBookAuthorBooksAndSearchParameterSpecification(
            bookAuthor.Books,
            searchParameter);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter, BookAuthor bookAuthor)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        bookAuthor.AddBook(book);
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksByBookAuthorBooksAndSearchParameterSpecification(
            bookAuthor.Books,
            searchParameter);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
