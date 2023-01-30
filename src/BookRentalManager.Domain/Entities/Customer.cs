namespace BookRentalManager.Domain.Entities;

public sealed class Customer : Entity
{
    private readonly List<Book> _books;

    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }
    public IReadOnlyList<Book> Books => _books.AsReadOnly();
    public CustomerStatus CustomerStatus { get; private set; }
    public int CustomerPoints { get; private set; }

    private Customer()
    {
        _books = new();
        FullName = default!;
        Email = default!;
        PhoneNumber = default!;
        CustomerStatus = default!;
        CustomerPoints = default;
    }

    public Customer(
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber)
    {
        _books = new();
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CustomerStatus = new CustomerStatus(CustomerType.Explorer);
        CustomerPoints = 0;
    }

    public Result RentBook(Book book)
    {
        Result availabilityResult = CheckBookAvailability(book);
        if (!string.IsNullOrWhiteSpace(availabilityResult.ErrorMessage))
        {
            return Result.Fail(availabilityResult.ErrorType, availabilityResult.ErrorMessage);
        }
        book.IsAvailable = false;
        _books.Add(book);
        book.SetRentedBy(this);
        CustomerPoints++;
        CustomerStatus = CustomerStatus.PromoteCustomerStatus(CustomerPoints);
        return Result.Success();
    }

    public Result CheckBookAvailability(Book book)
    {
        Result bookAvailabilityResult = Result.Success();
        if (!book.IsAvailable)
        {
            bookAvailabilityResult = Result.Fail("bookNotAvailable", $"The book '{book.BookTitle}' is not available.");
        }
        Result<CustomerStatus> customerStatusResult = CustomerStatus.CheckCustomerTypeBookAvailability(Books.Count());
        if (!string.IsNullOrWhiteSpace(customerStatusResult.ErrorMessage))
        {
            customerStatusResult = Result.Fail<CustomerStatus>(customerStatusResult.ErrorType, customerStatusResult.ErrorMessage);
        }
        return Result.Combine(bookAvailabilityResult, customerStatusResult);
    }

    public Result ReturnBook(Book book)
    {
        if (!Books.Contains(book))
        {
            return Result.Fail("noBook", $"You don't have the book '{book.BookTitle}'.");
        }
        _books.Remove(book);
        book.IsAvailable = true;
        return Result.Success();
    }
}
