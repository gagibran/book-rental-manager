using System.Dynamic;
using BookRentalManager.Application.Common;
using Microsoft.AspNetCore.Http;

namespace BookRentalManager.UnitTests;

public class AuthorControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly AuthorController _authorController;
    private readonly Author _author;

    public AuthorControllerTests()
    {
        var httpContext = new DefaultHttpContext();
        var urlHelperStub = new Mock<IUrlHelper>();
        _author = TestFixtures.CreateDummyAuthor();
        _dispatcherStub = new();
        _authorController = new(_dispatcherStub.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
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
            .ReturnsAsync(Result.Fail<PaginatedList<GetAuthorDto>>("errorType", "errorMessage422"));

        // Act:
        var result = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _authorController.ModelState["errorType"]!.Errors[0].ErrorMessage);
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
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()),
        };
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorsByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetAuthorDto>(getAuthorDtos, 3, 3, 2, 1)));

        // Act:
        var result = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            MediaTypeConstants.BookRentalManagerHateoasMediaType,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)result!.Value!;
        dynamic returnedExpandoObject = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal("url", returnedExpandoObject.links[0].Href);
        Assert.Equal("url", returnedExpandoObject.links[1].Href);
        Assert.Equal("url", returnedExpandoObject.links[2].Href);
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
        var result = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
        Assert.Equal(expectedPaginatedGetAuthorDtos, (PaginatedList<GetAuthorDto>)result!.Value!);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_WithUnsuccessfulGetAuthorByIdResult_ReturnsError()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetAuthorDto>("ID error", "errorMessage404"));

        // Act:
        var result = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["ID error"]!.Errors[0].ErrorMessage);
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
        var result = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            MediaTypeConstants.BookRentalManagerHateoasMediaType,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        dynamic authorWithHateosLinks = (ExpandoObject)result!.Value!;
        var authorBook = ((List<GetBookFromAuthorDto>)authorWithHateosLinks.books).ElementAt(0);
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
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
        var result = (await _authorController.GetAuthorByIdAsync(
            It.IsAny<Guid>(),
            MediaTypeConstants.ApplicationJsonMediaType,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var returnedAuthor = result!.Value as GetAuthorDto;
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
        Assert.Equal(_author.Id, returnedAuthor!.Id);
        Assert.Equal(_author.FullName.ToString(), returnedAuthor.FullName);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).BookTitle, returnedAuthor.Books.ElementAt(0).BookTitle);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Edition, returnedAuthor.Books.ElementAt(0).Edition);
        Assert.Equal(getBookFromAuthorDtos.ElementAt(0).Isbn, returnedAuthor.Books.ElementAt(0).Isbn);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithCreateAuthorResultUnsuccessful_ReturnsError()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<AuthorCreatedDto>("authorAlreadyExists", "errorMessage422"));

        // Act:
        var result = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _authorController.ModelState["authorAlreadyExists"]!.Errors[0].ErrorMessage);
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
        var result = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            MediaTypeConstants.BookRentalManagerHateoasMediaType,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        dynamic authorWithHateosLinks = (ExpandoObject)result!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, result!.StatusCode);
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
        var result = await _authorController.CreateAuthorAsync(
            It.IsAny<CreateAuthorCommand>(),
            MediaTypeConstants.ApplicationJsonMediaType,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        var authorCreated = (AuthorCreatedDto)result!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, result!.StatusCode);
        Assert.Equal(_author.Id, authorCreated.Id);
        Assert.Equal(_author.FullName.FirstName, authorCreated.FirstName);
        Assert.Equal(_author.FullName.LastName, authorCreated.LastName);
    }

    [Fact]
    public async Task AddExistingBooksToAuthor_WithPatchAuthorBooksResultUnsuccessful_ReturnsError()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchAuthorBooksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("ID error", "errorMessage404"));

        // Act:
        var result = await _authorController.AddExistingBooksToAuthor(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchAuthorBooksDto>>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["ID error"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task AddExistingBooksToAuthor_WithPatchAuthorBooksResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchAuthorBooksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<Result>()));

        // Act:
        var result = await _authorController.AddExistingBooksToAuthor(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchAuthorBooksDto>>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, result!.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithDeleteAuthorByIdResultUnsuccessful_ReturnsError()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteAuthorByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("ID error", "errorMessage404"));

        // Act:
        var result = await _authorController.DeleteAuthorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["ID error"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithDeleteAuthorByIdResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteAuthorByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<Result>()));

        // Act:
        var result = await _authorController.DeleteAuthorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, result!.StatusCode);
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithGetAuthorByIdResultUnsuccessful_ReturnsError()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetAuthorDto>("ID error", "errorMessage404"));

        // Act:
        var result = await _authorController.GetAuthorAddBooksOptionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, result!.StatusCode);
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _authorController.ModelState["ID error"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithGetAuthorByIdResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(It.IsAny<GetAuthorDto>()));

        // Act:
        var result = await _authorController.GetAuthorAddBooksOptionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()) as OkResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
        Assert.Equal("PATCH, OPTIONS", _authorController.Response.Headers["Allow"]);
    }
}
