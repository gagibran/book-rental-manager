namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksWithSearchParamSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "1",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "0-201-616",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "false",
            TestFixtures.CreateDummyBook()
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {

        yield return new object[]
        {
            "1984",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "5",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "345-6",
            TestFixtures.CreateDummyBook()
        };
        yield return new object[]
        {
            "true",
            TestFixtures.CreateDummyBook()
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter, Book book)
    {
        // Arrange:
        var customer = TestFixtures.CreateDummyCustomer();
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksWithSearchParamSpecification(searchParameter);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter, Book book)
    {
        // Arrange:
        var customer = TestFixtures.CreateDummyCustomer();
        customer.RentBook(book);
        var booksWithSearchParameterSpecification = new BooksWithSearchParamSpecification(searchParameter);

        // Act:
        bool isSatisfiedBy = booksWithSearchParameterSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
