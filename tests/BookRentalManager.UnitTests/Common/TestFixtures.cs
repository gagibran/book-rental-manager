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

    public static Book CreateDummyBook()
    {
        return new Book(
            "Test Book",
            Volume.Create(1).Value,
            Isbn.Create(1_000_000_000).Value
        );
    }

    public static BookAuthor CreateDummyBookAuthor()
    {
        return new BookAuthor(FullName.Create("John", "Doe").Value);
    }
}
