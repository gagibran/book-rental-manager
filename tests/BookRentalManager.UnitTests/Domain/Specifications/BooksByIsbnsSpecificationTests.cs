namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BooksByIsbnsSpecificationTests
{
    private readonly Book _book;

    public BooksByIsbnsSpecificationTests()
    {
        _book = TestFixtures.CreateDummyBook();
    }

    [Fact]
    public void IsSatisfiedBy_WithIsbnsContainingBookIsbn_ReturnsTrue()
    {
        // Arrange:
        var isbns = new List<string> { "0-201-61622-X", "978-0132350884", "978-0140444308" };
        var booksByIsbnsSpecification = new BooksByIsbnsSpecification(isbns);

        // Act:
        bool isSatisfiedBy = booksByIsbnsSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Fact]
    public void IsSatisfiedBy_WithIsbnsNotContainingBookIsbn_ReturnsFalse()
    {
        // Arrange:
        var isbns = new List<string> { "0-201-63361-2", "978-0132350884", "978-0140444308" };
        var booksByIsbnsSpecification = new BooksByIsbnsSpecification(isbns);

        // Act:
        bool isSatisfiedBy = booksByIsbnsSpecification.IsSatisfiedBy(_book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
