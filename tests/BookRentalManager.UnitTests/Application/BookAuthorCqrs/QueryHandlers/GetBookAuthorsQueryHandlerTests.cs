using BookRentalManager.Application.BookAuthorCqrs.Queries;
using BookRentalManager.Application.BookAuthorCqrs.QueryHandlers;

namespace BookRentalManager.UnitTests.Application.BookAuthorCqrs.QueryHandlers;

public sealed class GetBookAuthorsWithBooksAndSearchParamQueryHandlerTests
{
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IMapper<BookAuthor, GetBookAuthorDto>> _getBookAuthorDtoMapperStub;
    private readonly GetBookAuthorsWithBooksAndSearchParamQueryHandler _getBookAuthorsWithBooksAndSearchParamQueryHandler;
    private readonly GetBookAuthorDto _getBookAuthorDto;
    private readonly BookAuthor _bookAuthor;
    private readonly List<BookAuthor> _bookAuthors;

    public GetBookAuthorsWithBooksAndSearchParamQueryHandlerTests()
    {
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _bookAuthors = new List<BookAuthor> { _bookAuthor };
        _getBookAuthorDto = new(Guid.NewGuid(), _bookAuthor.FullName, new List<GetBookAuthorBookDto>());
        _getBookAuthorDtoMapperStub = new();
        _bookAuthorRepositoryStub = new();
        _getBookAuthorsWithBooksAndSearchParamQueryHandler = new(
            _bookAuthorRepositoryStub.Object,
            _getBookAuthorDtoMapperStub.Object);
        _getBookAuthorDtoMapperStub
            .Setup(getBookAuthorDtoMapper => getBookAuthorDtoMapper.Map(It.IsAny<BookAuthor>()))
            .Returns(_getBookAuthorDto);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyBookAuthorsWithSearchParameter_ReturnsEmptyList()
    {
        // Assert:
        var getBookAuthorsWithBooksAndSearchParamQuery = new GetBookAuthorsWithBooksAndSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            "Name");
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(new List<BookAuthor>());

        // Act:
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsWithBooksAndSearchParamQueryHandler.HandleAsync(
            getBookAuthorsWithBooksAndSearchParamQuery,
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneBookAuthorWithSearchParameter_ReturnsListWithMatchingBookAuthor()
    {
        // Assert:
        var getBookAuthorsWithBooksAndSearchParamQuery = new GetBookAuthorsWithBooksAndSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            _bookAuthor.FullName.CompleteName);
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(_bookAuthors);

        // Act:
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsWithBooksAndSearchParamQueryHandler.HandleAsync(
            getBookAuthorsWithBooksAndSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(_getBookAuthorDto, handlerResult.Value.FirstOrDefault());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task HandleAsync_WithEmptySearchParameter_ReturnsListWithAllBookAuthors(string searchParam)
    {
        // Arrange:
        var getBookAuthorsWithBooksAndSearchParamQuery = new GetBookAuthorsWithBooksAndSearchParamQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            searchParam);
        _bookAuthors.Add(new BookAuthor(FullName.Create("Sarah", "Smith").Value));
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(_bookAuthors);

        // Act:
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsWithBooksAndSearchParamQueryHandler.HandleAsync(
            getBookAuthorsWithBooksAndSearchParamQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
