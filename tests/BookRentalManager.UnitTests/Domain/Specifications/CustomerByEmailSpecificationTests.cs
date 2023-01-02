using BookRentalManager.Domain.Specifications;

namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomerByEmailSpecificationTests
{
    private readonly Customer _customer;

    public CustomerByEmailSpecificationTests()
    {
        _customer = new(
            FullName.Create("John", "Doe").Value,
            Email.Create("john.doe@email.com").Value,
            PhoneNumber.Create(200, 2_000_000).Value
        );
    }

    [Fact]
    public void IsSatisfiedBy_WithExistingEmail_ReturnsTrue()
    {
        // Arrange:
        var email = Email.Create("john.doe@email.com").Value;
        var customerByEmailSpecification = new CustomerByEmailSpecification(email);

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithNonexistingEmail_ReturnsFalse()
    {
        // Arrange:
        var email = Email.Create("john.doe2@email.com").Value;
        var customerByEmailSpecification = new CustomerByEmailSpecification(email);

        // Act:
        bool isSatisfiedBy = customerByEmailSpecification.IsSatisfiedBy(_customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}