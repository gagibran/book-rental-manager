using SimpleBookManagement.Domain.Enums;

namespace SimpleBookManagement.Domain.Entities;

public sealed class Customer : BaseEntity
{
    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }
    public CustomerStatus CustomerStatus { get; }

    public Customer(
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber
    )
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CustomerStatus = CustomerStatus.Explorer;
    }
}
