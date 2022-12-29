namespace SimpleBookManagement.UnitTests.Domain.ValueObjects;

public sealed class CustomerStatusTests
{
    private const string AvailabilityErrorMessage = "You've reached the maximum amount of books per customer category";

    [Fact]
    public void CheckCustomerTypeBookAvailability_WithExplorerAndMaxAmount_ReturnsErrorMessage()
    {
        // Arrange:
        var customerStatus = new CustomerStatus(CustomerType.Explorer);
        var expectedErrorMessage = AvailabilityErrorMessage + " (Explorer: 2).";

        // Act:
        Result<CustomerStatus> customerTypeAvailability = customerStatus
            .CheckCustomerTypeBookAvailability(2);

        // Assert:
        Assert.Equal(expectedErrorMessage, customerTypeAvailability.ErrorMessage);
    }
}
