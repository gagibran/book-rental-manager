namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class CustomerTests
{
    private readonly Customer _customer;
    private readonly Book _book;
    private readonly Book _book2;
    private readonly Book _book3;

    public CustomerTests()
    {
        _customer = TestFixtures.CreateDummyCustomer();
        _book = TestFixtures.CreateDummyBook();
        _book2 = new Book(
            BookTitle.Create("Design Patterns: Elements of Reusable Object-Oriented Software").Value!,
            Edition.Create(1).Value!,
            Isbn.Create("978-0201633610 ").Value!);
        _book3 = new Book(
            BookTitle.Create("Introduction to Algorithms").Value!,
            Edition.Create(3).Value!,
            Isbn.Create("978-0262033848").Value!);
    }

    [Fact]
    public void Customer_WithCorrectValues_ReturnsExplorerCustomer()
    {
        // Arrange:
        var expectedCustomerStatus = CustomerStatus.Create(0);

        // Assert:
        Assert.Equal(expectedCustomerStatus, _customer.CustomerStatus);
    }

    [Fact]
    public void RentBook_WithUnavailableBook_ReturnsErrorMessage()
    {
        // Arrange:
        _customer.RentBook(_book);
        var expectedErrorMessage = $"The book '{_book.BookTitle}' is unavailable at the moment. Return due date: {_book.DueDate!.Value.ToLocalTime().ToShortDateString()}.";

        // Act:
        Result rentBookResult = _customer.RentBook(_book);

        // Assert:
        Assert.Equal("bookNotAvailable", rentBookResult.ErrorType);
        Assert.Equal(expectedErrorMessage, rentBookResult.ErrorMessage);
    }

    [Fact]
    public void RentBook_WithBooksPastTheirDueDates_ReturnsErrorMessage()
    {
        // Arrange:
        _customer.RentBook(_book2);
        _customer.RentBook(_book3);
        _book2.DueDate = DateTime.UtcNow.AddDays(-1);
        _book3.DueDate = DateTime.UtcNow.AddDays(-2);
        var expectedErrorMessage = $"The book '{_book2.BookTitle}' is past its return due date ({_book2.DueDate}) and it needs to be returned before renting another one.|The book '{_book3.BookTitle}' is past its return due date ({_book3.DueDate}) and it needs to be returned before renting another one.";

        // Act:
        Result rentBookResult = _customer.RentBook(_book);

        // Assert:
        Assert.Equal("dueDate|dueDate", rentBookResult.ErrorType);
        Assert.Equal(expectedErrorMessage, rentBookResult.ErrorMessage);
    }

    [Fact]
    public void RentBook_WithMaximumAmountOfBooksByCustomerTypeReached_ReturnsErrorMessage()
    {
        // Arrange:
        _customer.RentBook(_book);
        _customer.RentBook(_book2);

        // Act:
        Result rentBookResult = _customer.RentBook(_book3);

        // Assert:
        Assert.Equal("explorerMaximumAmountReached", rentBookResult.ErrorType);
        Assert.Equal(
            "The customer has reached the maximum amount of books per customer category (Explorer: 2).",
            rentBookResult.ErrorMessage);
    }

    [Theory]
    [InlineData(9, 7, CustomerType.Explorer)]
    [InlineData(10, 7, CustomerType.Adventurer)]
    [InlineData(49, 16, CustomerType.Adventurer)]
    [InlineData(50, 16, CustomerType.Master)]
    [InlineData(51, 30, CustomerType.Master)]
    public void RentBook_WithCorrectAvailability_ReturnsExpectedCustomerType(
        int targetCustomerPoints,
        int targetDueDate,
        CustomerType expectedCustomerType)
    {
        // Arrange:
        var expectedDueDate = DateTime.UtcNow.AddDays(targetDueDate);
        for (int i = 0; i < targetCustomerPoints; i++)
        {
            Book book = TestFixtures.CreateDummyBook();
            if (i == targetCustomerPoints - 1)
            {
                break;
            }
            _customer.RentBook(book);
            _customer.ReturnBook(book);
        }
        Book bookToRent = TestFixtures.CreateDummyBook();

        // Act:
        _customer.RentBook(bookToRent);

        // Assert:
        Assert.Equal(expectedCustomerType, _customer.CustomerStatus.CustomerType);
        Assert.Equal(expectedDueDate.Date, _customer.Books[0].DueDate!.Value.Date);
    }

    [Fact]
    public void ReturnBook_WithNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"The book '{_book.BookTitle}' has not been rented by this customer.";

        // Act:
        Result rentBookResult = _customer.ReturnBook(_book);

        // Assert:
        Assert.Equal("noBook", rentBookResult.ErrorType);
        Assert.Equal(expectedErrorMessage, rentBookResult.ErrorMessage);
    }

    [Theory]
    [InlineData(0, 0, CustomerType.Explorer)]
    [InlineData(10, 9, CustomerType.Explorer)]
    [InlineData(50, 49, CustomerType.Adventurer)]
    [InlineData(51, 50, CustomerType.Master)]
    public void ReturnBook_WithBookPastItsDueDate_DecreasesCustomerPointsAndStatus(
        int targetCustomerPoints,
        int expectedCustomerPoints,
        CustomerType expectedCustomerType)
    {
        // Arrange:
        for (int i = 0; i < targetCustomerPoints; i++)
        {
            Book book = TestFixtures.CreateDummyBook();
            if (i == targetCustomerPoints - 1)
            {
                break;
            }
            _customer.RentBook(book);
            _customer.ReturnBook(book);
        }
        Book bookToReturn = TestFixtures.CreateDummyBook();
        _customer.RentBook(bookToReturn);
        bookToReturn.DueDate = DateTime.UtcNow.AddDays(-1);

        // Act:
        _customer.ReturnBook(bookToReturn);

        // Assert:
        Assert.Equal(expectedCustomerPoints, _customer.CustomerPoints);
        Assert.Equal(expectedCustomerType, _customer.CustomerStatus.CustomerType);
    }

    [Fact]
    public void ReturnBook_WithUnavailableBook_ReturnsBookAvailable()
    {
        // Arrange:
        _customer.RentBook(_book);

        // Act:
        _customer.ReturnBook(_book);

        // Assert:
        Assert.False(_book.DueDate.HasValue);
    }

    [Theory]
    [InlineData("", "Doe", 200, 2_000_000, "First name cannot be empty.")]
    [InlineData("John", "Doe", 2002, 2_000_000, "Invalid area code.")]
    public void UpdateFullNameAndPhoneNumber_WithInvalidData_ReturnsErrorMessage(
        string firstName,
        string lastName,
        int areaCode,
        int prefixAndLineNumber,
        string expectedErrorMessage)
    {
        // Act:
        Result updateFullNameResult = _customer.UpdateFullNameAndPhoneNumber(firstName, lastName, areaCode, prefixAndLineNumber);

        // Assert:
        Assert.Equal(expectedErrorMessage, updateFullNameResult.ErrorMessage);
    }

    [Theory]
    [InlineData("Johannes", "Doe", 200, 2_000_000)]
    [InlineData("John", "Doe", 333, 2_000_000)]
    public void UpdateFullNameAndPhoneNumber_WithValidData_ReturnsUpdatedValues(
        string firstName,
        string lastName,
        int areaCode,
        int prefixAndLineNumber)
    {
        // Act:
        _customer.UpdateFullNameAndPhoneNumber(firstName, lastName, areaCode, prefixAndLineNumber);

        // Assert:
        Assert.Equal(firstName, _customer.FullName.FirstName);
        Assert.Equal(lastName, _customer.FullName.LastName);
        Assert.Equal(areaCode, _customer.PhoneNumber.AreaCode);
        Assert.Equal(prefixAndLineNumber, _customer.PhoneNumber.PrefixAndLineNumber);
    }
}
