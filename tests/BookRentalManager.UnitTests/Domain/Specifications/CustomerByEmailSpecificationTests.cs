namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerByEmailSpecificationTests
{
    private readonly Customer _customer;

    public CustomerByEmailSpecificationTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingEmail_ReturnsTrue()
    {
        // Arrange:
        var customerByEmailSpecification = new CustomerByEmailSpecification(
            "john.doe@email.com"
        );

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingEmail_ReturnsFalse()
    {
        // Arrange:
        var customerByEmailSpecification = new CustomerByEmailSpecification(
            "john.doe2@email.com"
        );

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
