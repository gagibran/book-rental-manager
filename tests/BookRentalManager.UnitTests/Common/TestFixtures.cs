namespace BookRentalManager.UnitTests.Common;

public static class TestFixtures
{
    public static Customer CreateDummyCustomer()
    {
        return new Customer(
            FullName.Create("John", "Doe").Value,
            Email.Create("john.doe@email.com").Value,
            PhoneNumber.Create(200, 2_000_000).Value
        );
    }
}
