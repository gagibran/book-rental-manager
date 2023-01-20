namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerWithBooksByEmailSpecificationTests
{
    private readonly Customer _customer;

    public CustomerWithBooksByEmailSpecificationTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingEmail_ReturnsTrue()
    {
        // Arrange:
        var customerByEmailSpecification = new CustomerWithBooksByEmailSpecification("john.doe@email.com");

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingEmail_ReturnsFalse()
    {
        // Arrange:
        var customerByEmailSpecification = new CustomerWithBooksByEmailSpecification("john.doe2@email.com");

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
