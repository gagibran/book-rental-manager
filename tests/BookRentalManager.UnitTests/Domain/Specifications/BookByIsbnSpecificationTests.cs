namespace BookRentalManager.UnitTests.Domain.Specifications;

public sealed class BookByIsbnSpecificationTests
{
    public static IEnumerable<object[]> GetSuccessTestData()
    {
        yield return new object[]
        {
            new Book(
                "The Fellowship of the Ring",
                Edition.Create(1).Value!,
                Isbn.Create("978-0808520764").Value!),
            "978-0808520764"
        };
        yield return new object[]
        {
            new Book(
                "The Two Towers",
                Edition.Create(1).Value!,
                Isbn.Create("978-0008567132").Value!),
            "978-0008567132"
        };
        yield return new object[]
        {
            new Book(
                "The Return of the King",
                Edition.Create(1).Value!,
                Isbn.Create("978-0008376147").Value!),
            "978-0008376147"
        };
    }

    public static IEnumerable<object[]> GetFailTestData()
    {
        yield return new object[]
        {
            new Book(
                "The Fellowship of the Ring",
                Edition.Create(1).Value!,
                Isbn.Create("978-0808520764").Value!),
            "978-0208520764"
        };
        yield return new object[]
        {
            new Book(
                "The Two Towers",
                Edition.Create(1).Value!,
                Isbn.Create("978-0008567132").Value!),
            "978-0008567832"
        };
        yield return new object[]
        {
            new Book(
                "The Return of the King",
                Edition.Create(1).Value!,
                Isbn.Create("978-0008376147").Value!),
            "978-000837614X"
        };
    }

    [Theory]
    [MemberData(nameof(GetSuccessTestData))]
    public void IsSatisfiedBy_WithIsbnContainingBookIsbn_ReturnsTrue(Book book, string isbn)
    {
        // Arrange:
        var bookByIsbnSpecification = new BookByIsbnSpecification(isbn);

        // Act:
        bool isSatisfiedBy = bookByIsbnSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.True(isSatisfiedBy);
    }

    [Theory]
    [MemberData(nameof(GetFailTestData))]
    public void IsSatisfiedBy_WithIsbnNotContainingBookIsbn_ReturnsFalse(Book book, string isbn)
    {
        // Arrange:
        var bookByIsbnSpecification = new BookByIsbnSpecification(isbn);

        // Act:
        bool isSatisfiedBy = bookByIsbnSpecification.IsSatisfiedBy(book);

        // Assert:
        Assert.False(isSatisfiedBy);
    }
}
