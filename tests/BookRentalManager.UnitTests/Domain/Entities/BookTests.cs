namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class BookTests
{
    [Theory]
    [InlineData("The Call of Cthulhu")]
    [InlineData("The Call of Cthulhu    ")]
    [InlineData("   The Call of Cthulhu    ")]
    public void Customer_WithCorrectValues_ReturnsExplorerCustomer(string bookTitle)
    {
        // Arrange:
        var expectedBookTitle = "The Call of Cthulhu";

        // Act:
        var book = new Book(bookTitle, Edition.Create(1).Value, Isbn.Create("978-1548234355").Value);

        // Assert:
        Assert.Equal(expectedBookTitle, book.BookTitle);
    }
}
