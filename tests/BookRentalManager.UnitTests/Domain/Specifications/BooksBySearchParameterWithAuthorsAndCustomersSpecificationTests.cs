namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "pragmatic Progr",
            string.Empty
        };
        yield return new object[]
        {
            "1",
            string.Empty
        };
        yield return new object[]
        {
            "0-201-616",
            string.Empty
        };
        yield return new object[]
        {
            "false",
            string.Empty
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {

        yield return new object[]
        {
            "1984",
            string.Empty
        };
        yield return new object[]
        {
            "5",
            string.Empty
        };
        yield return new object[]
        {
            "345-6",
            string.Empty
        };
        yield return new object[]
        {
            "true",
            string.Empty
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter, string sortParameters)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        customer.RentBook(book);
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter, string sortParameters)
    {
        // Arrange:
        var book = TestFixtures.CreateDummyBook();
        var customer = TestFixtures.CreateDummyCustomer();
        customer.RentBook(book);
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            searchParameter,
            sortParameters);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
