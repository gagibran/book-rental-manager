using SimpleBookManagement.Domain.Entities;
using SimpleBookManagement.Domain.Enums;

namespace SimpleBookManagement.UnitTests.Domain.Entities;

public sealed class CustomerTests
{
    [Fact]
    public void Customer_WithCorrectValues_ReturnsExplorerCustomer()
    {
        // Arrange:
        FullName fullName = FullName.Create("John", "Doe").Value;
        Email email = Email.Create("johndoe@email.com").Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(200, 2_000_000).Value;
        var customer = new Customer(fullName, email, phoneNumber);

        // Assert:
        Assert.Equal(CustomerStatus.Explorer, customer.CustomerStatus);
    }
}
