using BookRentalManager.Application.BookAuthorCqrs.Queries;
using BookRentalManager.Application.BookAuthorCqrs.QueryHandlers;

namespace BookRentalManager.UnitTests.Application.BookAuthorCqrs.QueryHandlers;

public sealed class GetBookAuthorsQueryHandlerTests
{
    private readonly int _pageIndex;
    private readonly int _totalItemsPerPage;
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IMapper<BookAuthor, GetBookAuthorDto>> _getBookAuthorDtoMapperStub;
    private readonly GetBookAuthorsQueryHandler _getBookAuthorsQueryHandler;
    private readonly GetBookAuthorDto _getBookAuthorDto;

    public GetBookAuthorsQueryHandlerTests()
    {
        BookAuthor bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _pageIndex = 1;
        _totalItemsPerPage = 50;
        _getBookAuthorDto = new(Guid.NewGuid(), bookAuthor.FullName, new List<GetBookAuthorBookDto>());
        _getBookAuthorDtoMapperStub = new();
        _bookAuthorRepositoryStub = new();
        _getBookAuthorsQueryHandler = new(
            _bookAuthorRepositoryStub.Object,
            _getBookAuthorDtoMapperStub.Object);
        _getBookAuthorDtoMapperStub
            .Setup(getBookAuthorDtoMapper => getBookAuthorDtoMapper.Map(It.IsAny<BookAuthor>()))
            .Returns(_getBookAuthorDto);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyBookAuthors_ReturnsErrorMessage()
    {
        // Assert:
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(new List<BookAuthor>());

        // Act:
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsQueryHandler.HandleAsync(
            new GetBookAuthorsQuery(_pageIndex, _totalItemsPerPage),
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneBookAuthors_ReturnsListWithBookAuthor()
    {
        // Assert:
        var expectedListOfBookAuthors = new List<BookAuthor>
        {
            TestFixtures.CreateDummyBookAuthor()
        };
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(expectedListOfBookAuthors);

        // Act:
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsQueryHandler.HandleAsync(
            new GetBookAuthorsQuery(_pageIndex, _totalItemsPerPage),
            default);

        // Assert:
        Assert.Equal(_getBookAuthorDto, handlerResult.Value.FirstOrDefault());
    }
}
