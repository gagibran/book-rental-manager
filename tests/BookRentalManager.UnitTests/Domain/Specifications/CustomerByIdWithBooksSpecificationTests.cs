namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerByIdWithBooksSpecificationTests
{
    private readonly Customer _customer;

    public CustomerByIdWithBooksSpecificationTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var customerWithBooksByIdSpecification = new CustomerByIdWithBooksSpecification(_customer.Id);

        // Act:
        bool isSatisfiedBy = customerWithBooksByIdSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var customerWithBooksByIdSpecification = new CustomerByIdWithBooksSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = customerWithBooksByIdSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
