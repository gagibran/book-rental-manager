using SimpleBookManagement.Domain.Enums;

namespace SimpleBookManagement.Domain.Entities;

public sealed class Customer : BaseEntity
{
    private readonly List<Book> _books;

    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }
    public IReadOnlyList<Book> Books => _books;
    public CustomerStatus CustomerStatus { get; private set; }
    public int CustomerPoints { get; private set; }

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

    public Result<Book> RentBook(Book book)
    {
        Result<Book> availabilityResult = CheckBookAvailability(book);
        if (availabilityResult.ErrorMessage is not null)
        {
            return Result<Book>.Fail(availabilityResult.ErrorMessage);
        }
        book.IsAvailable = false;
        _books.Add(book);
        CustomerPoints++;
        CustomerStatus = CustomerStatus.PromoteCustomerStatus(CustomerPoints);
        return Result<Book>.Success(book);
    }

    public Result<Book> CheckBookAvailability(Book book)
    {
        if (!book.IsAvailable)
        {
            return Result<Book>.Fail($"The book '{book.BookTitle}' is not available.");
        }
        Result<CustomerStatus> customerStatusResult = CustomerStatus
            .CheckCustomerTypeBookAvailability(Books.Count());
        if (customerStatusResult.ErrorMessage is not null)
        {
            return Result<Book>.Fail(customerStatusResult.ErrorMessage);
        }
        return Result<Book>.Success(book);
    }

    public Result<Book> ReturnBook(Book book)
    {
        if (!Books.Contains(book))
        {
            return Result<Book>.Fail($"You don't have the book '{book.BookTitle}'.");
        }
        _books.Remove(book);
        book.IsAvailable = true;
        return Result<Book>.Success(book);
    }
}
