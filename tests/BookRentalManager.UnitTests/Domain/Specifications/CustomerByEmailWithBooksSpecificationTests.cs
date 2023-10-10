namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerByEmailWithBooksSpecificationTests
{
    private readonly Customer _customer;

    public CustomerByEmailWithBooksSpecificationTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingEmail_ReturnsTrue()
    {
        // Arrange:
        var customerByIdWithBooksSpecification = new CustomerByEmailWithBooksSpecification("john.doe@email.com");

        // Act:
        bool isSatisfiedBy = customerByIdWithBooksSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingEmail_ReturnsFalse()
    {
        // Arrange:
        var customerByIdWithBooksSpecification = new CustomerByEmailWithBooksSpecification("john.doe2@email.com");

        // Act:
        bool isSatisfiedBy = customerByIdWithBooksSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
