namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class CustomersBySearchParameterWithBooksSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessfulTestParameters()
    {
        yield return new object[]
        {
            "jOhn",
            string.Empty,
            TestFixtures.CreateDummyCustomer(),
        };
        yield return new object[]
        {
            "smith@email.",
            string.Empty,
            new Customer(
                FullName.Create("Sarah", "Smith").Value!,
                Email.Create("sarah.smith@email.com").Value!,
                PhoneNumber.Create(345, 6_453_243).Value!)
        };
        yield return new object[]
        {
            "griffin",
            string.Empty,
            new Customer(
                FullName.Create("Peter", "Griffin").Value!,
                Email.Create("peter.grifin@email.com").Value!,
                PhoneNumber.Create(546, 4_056_780).Value!)
        };
    }

    public static IEnumerable<object[]> GetFailureTestParameters()
    {
        yield return new object[]
        {
            "234",
            string.Empty,
            TestFixtures.CreateDummyCustomer(),
        };
        yield return new object[]
        {
            "john@email.",
            string.Empty,
            new Customer(
                FullName.Create("Sarah", "Smith").Value!,
                Email.Create("sarah.smith@email.com").Value!,
                PhoneNumber.Create(345, 6_453_243).Value!)
        };
        yield return new object[]
        {
            "smith",
            string.Empty,
            new Customer(
                FullName.Create("Peter", "Griffin").Value!,
                Email.Create("peter.grifin@email.com").Value!,
                PhoneNumber.Create(546, 4_056_780).Value!)
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulTestParameters))]
    public void IsSatisfiedBy_WithCustomersWithQuery_ReturnsTrue(string searchParameter, string sortOrderParameters, Customer customer)
    {
        // Arrange:
        var customersWithSearchParameterSpecification = new CustomersBySearchParameterWithBooksSpecification(searchParameter, sortOrderParameters);

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
        var customersWithSearchParameterSpecification = new CustomersBySearchParameterWithBooksSpecification(searchParameter, sortOrderParameters);

        // Act:
        bool isSatisfiedBy = customersWithSearchParameterSpecification.IsSatisfiedBy(customer);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
