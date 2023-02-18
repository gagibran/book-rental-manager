namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomersBySearchParameterSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "20020000",
            "",
            TestFixtures.CreateDummyCustomer(),
        };
        yield return new object[]
        {
            "smith@email.",
            "",
            new Customer(
                FullName.Create("Sarah", "Smith").Value,
                Email.Create("sarah.smith@email.com").Value,
                PhoneNumber.Create(345, 6_453_243).Value)
        };
        yield return new object[]
        {
            "griffin",
            "",
            new Customer(
                FullName.Create("Peter", "Griffin").Value,
                Email.Create("peter.grifin@email.com").Value,
                PhoneNumber.Create(546, 4_056_780).Value)
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {
        yield return new object[]
        {
            "234",
            "",
            TestFixtures.CreateDummyCustomer(),
        };
        yield return new object[]
        {
            "john@email.",
            "",
            new Customer(
                FullName.Create("Sarah", "Smith").Value,
                Email.Create("sarah.smith@email.com").Value,
                PhoneNumber.Create(345, 6_453_243).Value)
        };
        yield return new object[]
        {
            "smith",
            "",
            new Customer(
                FullName.Create("Peter", "Griffin").Value,
                Email.Create("peter.grifin@email.com").Value,
                PhoneNumber.Create(546, 4_056_780).Value)
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithCustomersWithQuery_ReturnsTrue(string searchParameter, string sortOrderParameters, Customer customer)
    {
        // Arrange:
        var customersWithSearchParameterSpecification = new CustomersBySearchParameterSpecification(searchParameter, sortOrderParameters);

        // Act:
        bool isSatisfiedBy = customersWithSearchParameterSpecification.IsSatisfiedBy(customer);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailureTestParameters))]
    public void IsSatisfiedBy_WithoutCustomersWithQuery_ReturnsFalse(string searchParameter, string sortOrderParameters, Customer customer)
    {
        // Arrange:
        var customersWithSearchParameterSpecification = new CustomersBySearchParameterSpecification(searchParameter, sortOrderParameters);

        // Act:
        bool isSatisfiedBy = customersWithSearchParameterSpecification.IsSatisfiedBy(customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
