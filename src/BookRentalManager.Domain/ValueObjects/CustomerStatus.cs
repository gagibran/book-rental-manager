namespace BookRentalManager.Domain.ValueObjects;

public sealed class CustomerStatus : ValueObject
{
    private const int MaxCustomerPoints = 50;
    private const int MaximumAmountOfBooksExplorer = 2;
    private const int MaximumAmountOfBooksAdventurer = 5;
    private const int MaximumAmountOfBooksMaster = 7;
    private const string AvailabilityErrorMessage = "You've reached the maximum amount of books per customer category";

    public CustomerType CustomerType { get; }

    private CustomerStatus()
    {
        CustomerType = default;
    }

    public CustomerStatus(CustomerType customerType)
    {
        CustomerType = customerType;
    }

    public Result<CustomerStatus> CheckCustomerTypeBookAvailability(int customerBookCount)
    {
        if (customerBookCount >= MaximumAmountOfBooksExplorer && CustomerType is CustomerType.Explorer)
        {
            return Result.Fail<CustomerStatus>(AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksExplorer}).");
        }
        if (customerBookCount >= MaximumAmountOfBooksAdventurer && CustomerType is CustomerType.Adventurer)
        {
            return Result.Fail<CustomerStatus>(AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksAdventurer}).");
        }
        if (customerBookCount >= MaximumAmountOfBooksMaster && CustomerType is CustomerType.Master)
        {
            return Result.Fail<CustomerStatus>(AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksMaster}).");
        }
        return Result.Success<CustomerStatus>(new CustomerStatus(CustomerType));
    }

    public static CustomerStatus PromoteCustomerStatus(int customerPoints)
    {
        if (customerPoints >= MaxCustomerPoints)
        {
            return new CustomerStatus(CustomerType.Master);
        }
        if (customerPoints >= 10)
        {
            return new CustomerStatus(CustomerType.Adventurer);
        }
        return new CustomerStatus(CustomerType.Explorer);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CustomerType;
    }
}
