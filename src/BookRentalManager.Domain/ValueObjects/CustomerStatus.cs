namespace BookRentalManager.Domain.ValueObjects;

public sealed class CustomerStatus : ValueObject
{
    private const int MaximumAmountOfBooksExplorer = 2;
    private const int MaximumAmountOfBooksAdventurer = 5;
    private const int MaximumAmountOfBooksMaster = 7;
    private const string AvailabilityErrorMessage = "The customer has reached the maximum amount of books per customer category";

    public CustomerType CustomerType { get; }

    private CustomerStatus()
    {
        CustomerType = default;
    }

    private CustomerStatus(CustomerType customerType)
    {
        CustomerType = customerType;
    }

    public static CustomerStatus Create(int customerPoints)
    {
        if (customerPoints >= 0 && customerPoints < 10)
        {
            return new CustomerStatus(CustomerType.Explorer);
        }
        else if (customerPoints >= 10 && customerPoints < 50)
        {
            return new CustomerStatus(CustomerType.Adventurer);
        }
        return new CustomerStatus(CustomerType.Master);
    }

    public Result CheckRentPossibilityByCustomerType(int customerBookCount)
    {
        if (customerBookCount >= MaximumAmountOfBooksExplorer && CustomerType is CustomerType.Explorer)
        {
            return Result.Fail(
                "explorerMaximumAmountReached",
                AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksExplorer}).");
        }
        else if (customerBookCount >= MaximumAmountOfBooksAdventurer && CustomerType is CustomerType.Adventurer)
        {
            return Result.Fail(
                "adventurerMaximumAmountReached",
                AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksAdventurer}).");
        }
        else if (customerBookCount >= MaximumAmountOfBooksMaster && CustomerType is CustomerType.Master)
        {
            return Result.Fail(
                "masterMaximumAmountReached",
                AvailabilityErrorMessage + $" ({CustomerType}: {MaximumAmountOfBooksMaster}).");
        }
        return Result.Success();
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CustomerType;
    }
}
