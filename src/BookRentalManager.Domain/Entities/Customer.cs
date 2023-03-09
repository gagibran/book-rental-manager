namespace BookRentalManager.Domain.Entities;

public sealed class Customer : Entity
{
    private readonly List<Book> _books;

    public FullName FullName { get; private set; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; private set; }
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
        CustomerPoints = 0;
        CustomerStatus = CustomerStatus.Create(CustomerPoints);
    }

    public Result RentBook(Book book)
    {
        Result checkRentPossibilityByCustomerBooksResult = CheckRentPossibilityByCustomerBooks(book);
        if (!checkRentPossibilityByCustomerBooksResult.IsSuccess)
        {
            return checkRentPossibilityByCustomerBooksResult;
        }
        Result checkRentPossibilityByCustomerTypeResult = CustomerStatus.CheckRentPossibilityByCustomerType(Books.Count());
        if (!checkRentPossibilityByCustomerTypeResult.IsSuccess)
        {
            return checkRentPossibilityByCustomerTypeResult;
        }
        book.RentedAt = DateTime.UtcNow;
        book.DueDate = GetReturnDueDate();
        _books.Add(book);
        book.SetRentedBy(this);
        CustomerPoints++;
        CustomerStatus = CustomerStatus.Create(CustomerPoints);
        return Result.Success();
    }

    public Result ReturnBook(Book book)
    {
        if (!Books.Contains(book))
        {
            return Result.Fail("noBook", $"The book '{book.BookTitle}' does not exist for this customer.");
        }
        if (DateTime.UtcNow > book.DueDate && CustomerPoints > 0)
        {
            CustomerPoints--;
            CustomerStatus = CustomerStatus.Create(CustomerPoints);
        }
        book.RentedAt = null;
        book.DueDate = null;
        _books.Remove(book);
        return Result.Success();
    }

    public Result UpdateFullName(FullName fullName)
    {
        if (fullName is null)
        {
            return Result.Fail("fullName", "'fullName' cannot be null.");
        }
        FullName = fullName;
        return Result.Success();
    }

    public Result UpdatePhoneNumber(PhoneNumber phoneNumber)
    {
        if (phoneNumber is null)
        {
            return Result.Fail("phoneNumber", "'phoneNumber' cannot be null.");
        }
        PhoneNumber = phoneNumber;
        return Result.Success();
    }

    private Result CheckRentPossibilityByCustomerBooks(Book book)
    {
        if (book.Customer is not null)
        {
            return Result.Fail(
                "bookNotAvailable", $"The book '{book.BookTitle}' is unavailable at the moment. Return due date: {book.DueDate!.Value.ToLocalTime().ToShortDateString()}.");
        }
        Result bookPastDueDateResult = Result.Success();
        foreach (Book customerBook in Books)
        {
            if (customerBook.DueDate.HasValue && DateTime.UtcNow > customerBook.DueDate)
            {
                Result currentBookPastDueDateResult = Result.Fail(
                    "dueDate",
                    $"The book '{customerBook.BookTitle}' is past its return due date ({customerBook.DueDate}) and it needs to be returned before renting another one.");
                bookPastDueDateResult = Result.Combine(bookPastDueDateResult, currentBookPastDueDateResult);
            }
        }
        if (!bookPastDueDateResult.IsSuccess)
        {
            return bookPastDueDateResult;
        }
        return Result.Success();
    }

    private DateTime GetReturnDueDate()
    {
        switch (CustomerStatus.CustomerType)
        {
            case CustomerType.Adventurer:
                return DateTime.UtcNow.AddDays(16);
            case CustomerType.Master:
                return DateTime.UtcNow.AddDays(30);
            default:
                return DateTime.UtcNow.AddDays(7);
        }
    }
}
