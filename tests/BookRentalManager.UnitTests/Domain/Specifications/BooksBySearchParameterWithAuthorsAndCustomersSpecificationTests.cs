namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksBySearchParameterWithAuthorsAndCustomersSpecificationTests
{
    private readonly Book _book;
    private readonly Customer _customer;

    public BooksBySearchParameterWithAuthorsAndCustomersSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _customer = TestFixtures.CreateDummyCustomer();
        _customer.RentBook(_book);
    }

    [Theory]
    [InlineData("pragmatic Progr")]
    [InlineData("John doe")]
    [InlineData("1")]
    [InlineData("0-201-616")]
    [InlineData("2")]
    public void IsSatisfiedBy_WithBooksWithQuery_ReturnsTrue(string searchParameter)
    {
        // Arrange:
        TestFixtures.CreateDummyAuthor().AddBook(_book);
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            searchParameter,
            string.Empty);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [InlineData("1984")]
    [InlineData("23453")]
    [InlineData("345-6")]
    [InlineData("3/4/2023")]
    public void IsSatisfiedBy_WithoutBooksWithQuery_ReturnsFalse(string searchParameter)
    {
        // Arrange:
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            searchParameter,
            string.Empty);

        // Act:
        bool isSatisfiedBy = booksBySearchParameterWithAuthorsAndCustomersSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
