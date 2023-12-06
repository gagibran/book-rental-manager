namespace BookRentalManager.UnitTests;

public class AuthorControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly AuthorController _authorController;
    private readonly Author _author;

    public AuthorControllerTests()
    {
        var urlHelperStub = new Mock<IUrlHelper>();
        _author = TestFixtures.CreateDummyAuthor();
        _dispatcherStub = new();
        _authorController = new(_dispatcherStub.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            Url = urlHelperStub.Object
        };
        urlHelperStub
            .Setup(urlHelper => urlHelper.Link(It.IsAny<string>(), It.IsAny<object?>()))
            .Returns("url");
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithGetAllAuthorsResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorsByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<PaginatedList<GetAuthorDto>>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _authorController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        var getAuthorDtos = new List<GetAuthorDto>
        {
            new(
                Guid.NewGuid(),
                FullName.Create("John", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()),
            new(
                Guid.NewGuid(),
                FullName.Create("Jane", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()),
            new(
                Guid.NewGuid(),
                FullName.Create("Robert", "Plant").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>())
        };
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorsByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetAuthorDto>(getAuthorDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)okObjectResult!.Value!;
        dynamic actualExpandoObject = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal("url", actualExpandoObject.links[0].Href);
        Assert.Equal("url", actualExpandoObject.links[1].Href);
        Assert.Equal("url", actualExpandoObject.links[2].Href);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithAllAuthors()
    {
        // Arrange:
        var getAuthorDtos = new List<GetAuthorDto>
        {
            new(
                Guid.NewGuid(),
                FullName.Create("John", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>())
        };
        var expectedPaginatedGetAuthorDtos = new PaginatedList<GetAuthorDto>(getAuthorDtos, 1, 1, 1, 1);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorsByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedPaginatedGetAuthorDtos));

        // Act:
        var okObjectResult = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(expectedPaginatedGetAuthorDtos, (PaginatedList<GetAuthorDto>)okObjectResult!.Value!);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_WithGetAuthorByIdResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetAuthorDto>("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetAuthorsByIdAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var getBookFromAuthorDtos = new List<GetBookFromAuthorDto>
        {
            new(book.BookTitle, book.Edition, book.Isbn)
        };
        var getAuthorDto = new GetAuthorDto(_author.Id, _author.FullName, getBookFromAuthorDtos);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(getAuthorDto));

        // Act:
        var okObjectResult = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        dynamic authorWithHateosLinks = (ExpandoObject)okObjectResult!.Value!;
        var authorBook = ((List<GetBookFromAuthorDto>)authorWithHateosLinks.books).ElementAt(0);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(_author.Id, authorWithHateosLinks.id);
        Assert.Equal(_author.FullName.ToString(), (string)authorWithHateosLinks.fullName);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).BookTitle, authorBook.BookTitle);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Edition, authorBook.Edition);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Isbn, authorBook.Isbn);
        Assert.Equal("self", authorWithHateosLinks.links[0].Rel);
        Assert.Equal("add_existing_books_to_author", authorWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_author", authorWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task GetAuthorsByIdAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithAuthor()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var getBookFromAuthorDtos = new List<GetBookFromAuthorDto>
        {
            new(book.BookTitle, book.Edition, book.Isbn)
        };
        var getAuthorDto = new GetAuthorDto(_author.Id, _author.FullName, getBookFromAuthorDtos);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(getAuthorDto));

        // Act:
        var okObjectResult = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var actualAuthor = okObjectResult!.Value as GetAuthorDto;
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(_author.Id, actualAuthor!.Id);
        Assert.Equal(_author.FullName.ToString(), actualAuthor.FullName);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).BookTitle, actualAuthor.Books.ElementAt(0).BookTitle);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Edition, actualAuthor.Books.ElementAt(0).Edition);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Isbn, actualAuthor.Books.ElementAt(0).Isbn);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithCreateAuthorResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<AuthorCreatedDto>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _authorController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeVendorSpecific_ReturnsCreatedAtActionWithHateoasLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new AuthorCreatedDto(
                _author.Id,
                _author.FullName.FirstName,
                _author.FullName.LastName)));

        // Act:
        var createdAtActionResult = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        dynamic authorWithHateosLinks = (ExpandoObject)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(_author.Id, authorWithHateosLinks.id);
        Assert.Equal(_author.FullName.FirstName, (string)authorWithHateosLinks.firstName);
        Assert.Equal(_author.FullName.LastName, (string)authorWithHateosLinks.lastName);
        Assert.Equal("self", authorWithHateosLinks.links[0].Rel);
        Assert.Equal("add_existing_books_to_author", authorWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_author", authorWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeNotVendorSpecific_ReturnsCreatedAtActionWithAuthor()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new AuthorCreatedDto(
                _author.Id,
                _author.FullName.FirstName,
                _author.FullName.LastName)));

        // Act:
        var createdAtActionResult = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        var authorCreated = (AuthorCreatedDto)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(_author.Id, authorCreated.Id);
        Assert.Equal(_author.FullName.FirstName, authorCreated.FirstName);
        Assert.Equal(_author.FullName.LastName, authorCreated.LastName);
    }

    [Fact]
    public async Task AddExistingBooksToAuthor_WithPatchAuthorBooksResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchAuthorBooksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = await _authorController.AddExistingBooksToAuthor(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchAuthorBooksDto>>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task AddExistingBooksToAuthor_WithPatchAuthorBooksResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchAuthorBooksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<Result>()));

        // Act:
        var noContentResult = await _authorController.AddExistingBooksToAuthor(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchAuthorBooksDto>>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithDeleteAuthorByIdResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteAuthorByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = await _authorController.DeleteAuthorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithDeleteAuthorByIdResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteAuthorByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<Result>()));

        // Act:
        var noContentResult = await _authorController.DeleteAuthorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithGetAuthorByIdResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetAuthorDto>("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = await _authorController.GetAuthorAddBooksOptionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithGetAuthorByIdResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<GetAuthorDto>()));

        // Act:
        var okResult = await _authorController.GetAuthorAddBooksOptionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as OkResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, okResult!.StatusCode);
        Assert.Equal("PATCH, OPTIONS", _authorController.Response.Headers["Allow"]);
    }
}
