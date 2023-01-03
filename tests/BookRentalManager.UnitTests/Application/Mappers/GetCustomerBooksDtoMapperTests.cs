namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerBooksDtoMapperTests
{
    [Fact]
    public void Map_WithValidBooks_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetCustomerBooksDto = new GetCustomerBooksDto(
            book.BookTitle,
            new List<GetBookAuthorsForCustomerBooksDto>(),
            book.Edition,
            book.Isbn
        );
        var getBookAuthorsForCustomerBooksDtoMapperStub =
            new Mock<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>>>();
        getBookAuthorsForCustomerBooksDtoMapperStub
            .Setup(getBookAuthorsForCustomerBooksDtoMapper => getBookAuthorsForCustomerBooksDtoMapper
                .Map(It.IsAny<IReadOnlyList<BookAuthor>>()))
            .Returns(new List<GetBookAuthorsForCustomerBooksDto>());
        var getCustomerBooksDtoMapper = new GetCustomerBooksDtoMapper(getBookAuthorsForCustomerBooksDtoMapperStub.Object);

        // Act:
        IReadOnlyList<GetCustomerBooksDto> getCustomerBooksDtos = getCustomerBooksDtoMapper
            .Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerBooksDto.BookTitle, getCustomerBooksDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetCustomerBooksDto.BookAuthors, getCustomerBooksDtos.FirstOrDefault().BookAuthors);
        Assert.Equal(expectedGetCustomerBooksDto.Edition, getCustomerBooksDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetCustomerBooksDto.Isbn, getCustomerBooksDtos.FirstOrDefault().Isbn);
    }
}
