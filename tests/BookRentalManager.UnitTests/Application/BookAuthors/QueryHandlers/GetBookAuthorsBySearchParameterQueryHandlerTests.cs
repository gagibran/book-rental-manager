using BookRentalManager.Application.BooksAuthors.Queries;
using BookRentalManager.Application.BooksAuthors.QueryHandlers;

namespace BookRentalManager.UnitTests.Application.BookAuthors.QueryHandlers;

public sealed class GetBookAuthorsBySearchParameterQueryHandlerTests
{
    private readonly BookAuthor _bookAuthor;
    private readonly List<BookAuthor> _bookAuthors;
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IMapper<BookAuthor, GetBookAuthorDto>> _getBookAuthorDtoMapperStub;
    private readonly GetBookAuthorsBySearchParameterQueryHandler _getBookAuthorsBySearchParameterQueryHandler;
    private readonly GetBookAuthorDto _getBookAuthorDto;

    public GetBookAuthorsBySearchParameterQueryHandlerTests()
    {
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _bookAuthors = new List<BookAuthor> { _bookAuthor };
        _getBookAuthorDto = new(Guid.NewGuid(), _bookAuthor.FullName, new List<GetBookAuthorBookDto>());
        _getBookAuthorDtoMapperStub = new();
        _bookAuthorRepositoryStub = new();
        _getBookAuthorsBySearchParameterQueryHandler = new(
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
        var getBookAuthorsBySearchParameterQuery = new GetBookAuthorsBySearchParameterQuery(
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
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsBySearchParameterQueryHandler.HandleAsync(
            getBookAuthorsBySearchParameterQuery,
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneBookAuthorWithSearchParameter_ReturnsListWithMatchingBookAuthor()
    {
        // Assert:
        var getBookAuthorsBySearchParameterQuery = new GetBookAuthorsBySearchParameterQuery(
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
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsBySearchParameterQueryHandler.HandleAsync(
            getBookAuthorsBySearchParameterQuery,
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
        var getBookAuthorsBySearchParameterQuery = new GetBookAuthorsBySearchParameterQuery(
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
        Result<IReadOnlyList<GetBookAuthorDto>> handlerResult = await _getBookAuthorsBySearchParameterQueryHandler.HandleAsync(
            getBookAuthorsBySearchParameterQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
