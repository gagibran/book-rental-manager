namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class BookTests
{
    [Fact]
    public void Book_WithCorrectValues_ReturnsBook()
    {
        // Arrange:
        var expectedBookTitle = "The Call of Cthulhu";

        // Act:
        var book = new Book(
            "   The Call of Cthulhu    ",
            Edition.Create(1).Value!,
            Isbn.Create("978-1548234355").Value!);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedBookTitle, book.BookTitle);
        Assert.Equal(new List<Author>(), book.Authors);
        Assert.Null(book.RentedAt);
        Assert.Null(book.DueDate);
    }
}
