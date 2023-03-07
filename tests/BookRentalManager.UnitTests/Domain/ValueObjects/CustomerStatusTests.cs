namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class CustomerStatusTests
{
    private const string AvailabilityErrorMessage = "The customer has reached the maximum amount of books per customer category";

    [Theory]
    [InlineData(0, 2, AvailabilityErrorMessage + " (Explorer: 2).")]
    [InlineData(11, 5, AvailabilityErrorMessage + " (Adventurer: 5).")]
    [InlineData(51, 7, AvailabilityErrorMessage + " (Master: 7).")]
    public void CheckCustomerTypeBookAvailability_WithCustomerStatusAndItsMaxAmount_ReturnsErrorMessage(
        int customerPoints,
        int customerBookCount,
        string expectedErrorMessage)
    {
        // Arrange:
        var customerStatus = CustomerStatus.Create(customerPoints);

        // Act:
        Result customerTypeAvailability = customerStatus.CheckRentPossibilityByCustomerType(customerBookCount);

        // Assert:
        Assert.Equal(expectedErrorMessage, customerTypeAvailability.ErrorMessage);
    }

    [Theory]
    [InlineData(4, CustomerType.Explorer)]
    [InlineData(10, CustomerType.Adventurer)]
    [InlineData(50, CustomerType.Master)]
    public void PromoteCustomerStatus_WithCustomerPoints_ReturnsAppropriatedCustomerStatus(
        int customerPoints,
        CustomerType customerType)
    {
        // Act:
        CustomerStatus newCustomerStatus = CustomerStatus.Create(customerPoints);

        // Assert:
        Assert.Equal(customerType, newCustomerStatus.CustomerType);
    }
}
