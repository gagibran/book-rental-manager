namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class CustomerTests
{
    private readonly Book _book;
    private readonly Customer _customer;

    public CustomerTests()
    {
        _book = new Book(
            "Domain-Driven Design: Tackling Complexity in the Heart of Software",
            Volume.Create(1).Value,
            Isbn.Create(9780321125217).Value
        );
        _customer = new Customer(
            FullName.Create("John", "Doe").Value,
            Email.Create("johndoe@email.com").Value,
            PhoneNumber.Create(200, 2_000_000).Value
        );
    }

    [Fact]
    public void Customer_WithCorrectValues_ReturnsExplorerCustomer()
    {
        // Arrange:
        var expectedCustomerStatus = new CustomerStatus(CustomerType.Explorer);

        // Assert:
        Assert.Equal(expectedCustomerStatus, _customer.CustomerStatus);
    }

    [Fact]
    public void RentBook_WithUnavailableBookByCustomerType_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "You've reached the maximum amount of books per customer category (Explorer: 2).";
        var book2 = new Book(
            "Design Patterns: Elements of Reusable Object-Oriented Software",
            Volume.Create(1).Value,
            Isbn.Create(9780201633610).Value
        );
        var book3 = new Book(
            "Introduction to Algorithms",
            Volume.Create(3).Value,
            Isbn.Create(9780262270823).Value
        );
        _customer.RentBook(_book);
        _customer.RentBook(book2);

        // Act:
        Result availabilityResult = _customer.RentBook(book3);

        // Assert:
        Assert.Equal(expectedErrorMessage, availabilityResult.ErrorMessage);
    }

    [Fact]
    public void RentBook_WithUnavailableBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "The book 'Domain-Driven Design: Tackling Complexity in the Heart of Software' is not available.";
        _customer.RentBook(_book);

        // Act:
        Result availabilityResult = _customer.RentBook(_book);

        // Assert:
        Assert.Equal(expectedErrorMessage, availabilityResult.ErrorMessage);
    }

    [Fact]
    public void RentBook_WithCorrectAvailibitly_ReturnsIncreasedCustomerPoints()
    {
        // Arrange:
        var expectedCustomerPoints = 1;

        // Act:
        Result availabilityResult = _customer.RentBook(_book);

        // Then
        Assert.Equal(expectedCustomerPoints, _customer.CustomerPoints);
    }

    [Fact]
    public void ReturnBook_WithUnavailableBook_ReturnsBookAvailable()
    {
        // Arrange:
        _customer.RentBook(_book);

        // Act:
        _customer.ReturnBook(_book);

        // Assert:
        Assert.True(_book.IsAvailable);
    }

    [Fact]
    public void ReturnBook_WithNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "You don't have the book 'Domain-Driven Design: Tackling Complexity in the Heart of Software'.";

        // Act:
        Result availabilityResult = _customer.ReturnBook(_book);

        // Assert:
        Assert.Equal(expectedErrorMessage, availabilityResult.ErrorMessage);
    }
}