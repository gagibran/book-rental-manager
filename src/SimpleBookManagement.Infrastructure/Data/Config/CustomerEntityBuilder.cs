namespace SimpleBookManagement.Infrastructure.Data.Config;

public sealed class CustomerEntityBuilder : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> customerBuilder)
    {
        customerBuilder
            .ToTable("Customer")
            .HasKey(customer => customer.Id);
        customerBuilder
            .OwnsOne(customer => customer.FullName)
            .Property(fullName => fullName.CompleteName)
            .HasColumnName("FullName")
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.Email)
            .Property(email => email.EmailAddress)
            .HasColumnName("Email")
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.PhoneNumber)
            .Property(phoneNumber => phoneNumber.CompletePhoneNumber)
            .HasColumnName("PhoneNumber")
            .IsRequired();
        customerBuilder
            .HasMany(customer => customer.Books)
            .WithOne(books => books.Customer)
            .IsRequired();
        customerBuilder
            .OwnsOne(customer => customer.CustomerStatus)
            .Property(customerStatus => customerStatus.CustomerType)
            .HasColumnName("CustomerStatus")
            .IsRequired();
        customerBuilder
            .Property(customer => customer.CustomerPoints)
            .HasColumnName("CustomerPoints")
            .IsRequired();
    }
}
