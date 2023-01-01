namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class CustomerStatusTests
{
    private const string AvailabilityErrorMessage = "You've reached the maximum amount of books per customer category";

    [Theory]
    [InlineData(CustomerType.Explorer, 2, AvailabilityErrorMessage + " (Explorer: 2).")]
    [InlineData(CustomerType.Adventurer, 5, AvailabilityErrorMessage + " (Adventurer: 5).")]
    [InlineData(CustomerType.Master, 7, AvailabilityErrorMessage + " (Master: 7).")]
    public void CheckCustomerTypeBookAvailability_WithCustomerStatusAndItsMaxAmount_ReturnsErrorMessage(
        CustomerType customerType,
        int customerBookCount,
        string expectedErrorMessage
    )
    {
        // Arrange:
        var customerStatus = new CustomerStatus(customerType);

        // Act:
        Result customerTypeAvailability = customerStatus
            .CheckCustomerTypeBookAvailability(customerBookCount);

        // Assert:
        Assert.Equal(expectedErrorMessage, customerTypeAvailability.ErrorMessage);
    }

    [Theory]
    [InlineData(4, CustomerType.Explorer)]
    [InlineData(10, CustomerType.Adventurer)]
    [InlineData(50, CustomerType.Master)]
    public void PromoteCustomerStatus_WithCustomerPoints_ReturnsAppropriatedCustomerStatus(
        int customerPoints,
        CustomerType customerType
    )
    {
        // Act:
        CustomerStatus newCustomerStatus = CustomerStatus.PromoteCustomerStatus(
            customerPoints
        );

        // Assert:
        Assert.Equal(customerType, newCustomerStatus.CustomerType);
    }
}
