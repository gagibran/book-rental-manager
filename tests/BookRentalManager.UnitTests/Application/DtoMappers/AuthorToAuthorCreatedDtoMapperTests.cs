namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class AuthorToAuthorCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidAuthors_ReturnsValidGetAuthorFromBookDtos()
    {
        // Arrange:
        Author author = TestFixtures.CreateDummyAuthor();
        Book book = TestFixtures.CreateDummyBook();
        author.AddBook(book);
        var bookCreatedForAuthorDto = new BookCreatedForAuthorDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);
        var bookCreatedForAuthorDtos = new List<BookCreatedForAuthorDto> { bookCreatedForAuthorDto };
        var expectedAuthorCreatedDto = new AuthorCreatedDto(
            author.Id,
            author.FullName.CompleteName,
            bookCreatedForAuthorDtos.AsReadOnly());
        var bookToBookCreatedForAuthorDtoMapperStub = new Mock<IMapper<Book, BookCreatedForAuthorDto>>();
        bookToBookCreatedForAuthorDtoMapperStub
            .Setup(bookToBookCreatedForAuthorDtoMapper => bookToBookCreatedForAuthorDtoMapper.Map(It.IsAny<Book>()))
            .Returns(bookCreatedForAuthorDto);
        var authorsToGetAuthorFromBookDtosMapper = new AuthorToAuthorCreatedDtoMapper(bookToBookCreatedForAuthorDtoMapperStub.Object);

        // Act:
        AuthorCreatedDto actualAuthorCreatedDto = authorsToGetAuthorFromBookDtosMapper.Map(author);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedAuthorCreatedDto.Id, actualAuthorCreatedDto.Id);
        Assert.Equal(expectedAuthorCreatedDto.FullName, actualAuthorCreatedDto.FullName);
        Assert.Equal(expectedAuthorCreatedDto.Books, actualAuthorCreatedDto.Books);
    }
}
