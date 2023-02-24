using BookRentalManager.Domain.Enums;

namespace BookRentalManager.Infrastructure.Data.Config;

public sealed class CustomerEntityBuilder : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> customerBuilder)
    {
        customerBuilder
            .ToTable("Customer")
            .HasKey(customer => customer.Id);
        customerBuilder
            .OwnsOne(customer => customer.FullName)
            .Property(fullName => fullName.FirstName)
            .HasColumnName("FirstName")
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.FullName)
            .Property(fullName => fullName.LastName)
            .HasColumnName("LastName")
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.Email)
            .Property(email => email.EmailAddress)
            .HasColumnName("Email")
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.PhoneNumber)
            .Property(phoneNumber => phoneNumber.AreaCode)
            .HasColumnName("AreaCode")
            .HasMaxLength(12)
            .IsFixedLength()
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.PhoneNumber)
            .Property(phoneNumber => phoneNumber.PrefixAndLineNumber)
            .HasColumnName("PrefixAndLineNumber")
            .HasMaxLength(12)
            .IsFixedLength()
            .IsRequired();
        customerBuilder
            .HasMany(customer => customer.Books)
            .WithOne(books => books.Customer);
        customerBuilder
            .OwnsOne(customer => customer.CustomerStatus)
            .Property(customerStatus => customerStatus.CustomerType)
            .HasColumnName("CustomerStatus")
            .HasConversion(
                customerType => customerType.ToString(),
                customerType => (CustomerType)Enum.Parse(typeof(CustomerType), customerType))
            .IsRequired();
        customerBuilder
            .Property(customer => customer.CustomerPoints)
            .HasColumnName("CustomerPoints")
            .IsRequired();
        customerBuilder
            .Property(customer => customer.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();
    }
}
