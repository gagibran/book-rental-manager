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
            "The Pragmatic Programmer: From Journeyman to Master",
            Edition.Create(1).Value,
            Isbn.Create("0-201-61622-X").Value
        );
    }

    public static BookAuthor CreateDummyBookAuthor()
    {
        return new BookAuthor(FullName.Create("John", "Doe").Value);
    }
}
