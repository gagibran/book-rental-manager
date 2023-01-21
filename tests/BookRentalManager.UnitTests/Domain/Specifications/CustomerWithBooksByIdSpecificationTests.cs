namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerWithBooksByIdSpecificationTests
{
    private readonly Customer _customer;

    public CustomerWithBooksByIdSpecificationTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingId_ReturnsTrue()
    {
        // Arrange:
        var customerWithBooksByIdSpecification = new CustomerWithBooksByIdSpecification(_customer.Id);

        // Act:
        bool isSatisfiedBy = customerWithBooksByIdSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingId_ReturnsFalse()
    {
        // Arrange:
        var customerWithBooksByIdSpecification = new CustomerWithBooksByIdSpecification(Guid.NewGuid());

        // Act:
        bool isSatisfiedBy = customerWithBooksByIdSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
