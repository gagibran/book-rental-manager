using BookRentalManager.Domain.Enums;

namespace BookRentalManager.Domain.Entities;

public sealed class Customer : BaseEntity
{
    private readonly List<Book> _books = default!;

    public FullName FullName { get; } = default!;
    public Email Email { get; } = default!;
    public PhoneNumber PhoneNumber { get; } = default!;
    public IReadOnlyList<Book> Books => _books;
    public CustomerStatus CustomerStatus { get; private set; } = default!;
    public int CustomerPoints { get; private set; } = default!;

    private Customer()
    {
    }

    public Customer(
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber
    )
    {
        _books = new List<Book>();
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CustomerStatus = new CustomerStatus(CustomerType.Explorer);
    }

    public Result RentBook(Book book)
    {
        Result availabilityResult = CheckBookAvailability(book);
        if (!string.IsNullOrWhiteSpace(availabilityResult.ErrorMessage))
        {
            return Result.Fail(availabilityResult.ErrorMessage);
        }
        book.IsAvailable = false;
        _books.Add(book);
        CustomerPoints++;
        CustomerStatus = CustomerStatus.PromoteCustomerStatus(CustomerPoints);
        return Result.Success();
    }

    public Result CheckBookAvailability(Book book)
    {
        if (!book.IsAvailable)
        {
            return Result.Fail($"The book '{book.BookTitle}' is not available.");
        }
        Result<CustomerStatus> customerStatusResult = CustomerStatus
            .CheckCustomerTypeBookAvailability(Books.Count());
        if (!string.IsNullOrWhiteSpace(customerStatusResult.ErrorMessage))
        {
            return Result.Fail(customerStatusResult.ErrorMessage);
        }
        return Result.Success();
    }

    public Result ReturnBook(Book book)
    {
        if (!Books.Contains(book))
        {
            return Result.Fail($"You don't have the book '{book.BookTitle}'.");
        }
        _books.Remove(book);
        book.IsAvailable = true;
        return Result.Success();
    }
}
