namespace BookRentalManager.UnitTests.Api.Controllers.V1;

public sealed class BookControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly BookController _bookController;
    private readonly List<GetBookDto> _getBookDtos;

    public BookControllerTests()
    {
        Customer customer = TestFixtures.CreateDummyCustomer();
        var getCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto(
            customer.FullName.ToString(),
            customer.Email.ToString());
        var gangOfFour = new List<GetAuthorFromBookDto>
        {
            new("Erich Gamma"),
            new("John Vlissides"),
            new("Ralph Johnson"),
            new("Richard Helm"),
        };
        _getBookDtos =
        [
            new(
                Guid.NewGuid(),
                "Design Patterns: Elements of Reusable Object-Oriented Software",
                gangOfFour,
                Edition.Create(1).Value!.EditionNumber!,
                Isbn.Create("0-201-63361-2").Value!.ToString()!,
                new DateTime(2020, 1, 1),
                new DateTime(2020, 2, 1),
                getCustomerThatRentedBookDto),
            new(
                Guid.NewGuid(),
                "The Shadow Over Innsmouth",
                [new("Howard Lovecraft")],
                Edition.Create(1).Value!.EditionNumber!,
                Isbn.Create("978-1878252180").Value!.ToString()!,
                null,
                null,
                getCustomerThatRentedBookDto)
        ];
        var urlHelperStub = new Mock<IUrlHelper>();
        _dispatcherStub = new();
        _bookController = new(_dispatcherStub.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            Url = urlHelperStub.Object
        };
        urlHelperStub
            .Setup(urlHelper => urlHelper.Link(It.IsAny<string>(), It.IsAny<object?>()))
            .Returns("url");
    }

    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithGetAllBooksResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<PaginatedList<GetBookDto>>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = (await _bookController.GetBooksByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _bookController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetBookDto>(_getBookDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _bookController.GetBooksByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)okObjectResult!.Value!;
        dynamic bookWithHateosLinks = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal(_getBookDtos[0].Id, bookWithHateosLinks.id);
        Assert.Equal(_getBookDtos[0].BookTitle, bookWithHateosLinks.bookTitle);
        Assert.Equal(_getBookDtos[0].Authors, bookWithHateosLinks.authors);
        Assert.Equal(_getBookDtos[0].Edition, bookWithHateosLinks.edition);
        Assert.Equal(_getBookDtos[0].Isbn, bookWithHateosLinks.isbn);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal(_getBookDtos[0].DueDate, bookWithHateosLinks.dueDate);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal("self", bookWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_book", bookWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_book", bookWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithAllBooks()
    {
        // Arrange:
        var expectedPaginatedGetBookDtos = new PaginatedList<GetBookDto>(_getBookDtos, 3, 3, 2, 1);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedPaginatedGetBookDtos));

        // Act:
        var okObjectResult = (await _bookController.GetBooksByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(expectedPaginatedGetBookDtos, (PaginatedList<GetBookDto>)okObjectResult!.Value!);
    }

    [Fact]
    public async Task GeBookByIdAsync_WithGetBookByIdResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBookByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetBookDto>("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = (await _bookController.GetBookByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _bookController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetBookByIdAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateosLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBookByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(_getBookDtos[0]));

        // Act:
        var okObjectResult = (await _bookController.GetBookByIdAsync(
            It.IsAny<Guid>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        dynamic bookWithHateosLinks = (ExpandoObject)okObjectResult!.Value!;
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(_getBookDtos[0].Id, bookWithHateosLinks.id);
        Assert.Equal(_getBookDtos[0].BookTitle, bookWithHateosLinks.bookTitle);
        Assert.Equal(_getBookDtos[0].Authors, bookWithHateosLinks.authors);
        Assert.Equal(_getBookDtos[0].Edition, bookWithHateosLinks.edition);
        Assert.Equal(_getBookDtos[0].Isbn, bookWithHateosLinks.isbn);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal(_getBookDtos[0].DueDate, bookWithHateosLinks.dueDate);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal("self", bookWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_book", bookWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_book", bookWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task GetBookByIdAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithBook()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var getAuthorFromBookDto = new GetAuthorFromBookDto("Howard Lovecraft");
        var getBookDto = new GetBookDto(
            Guid.NewGuid(),
            "The Shadow Over Innsmouth",
            [getAuthorFromBookDto],
            Edition.Create(1).Value!.EditionNumber,
            Isbn.Create("978-1878252180").Value!.ToString(),
            new DateTime(2020, 1, 1),
            new DateTime(2020, 2, 1),
            new GetCustomerThatRentedBookDto(customer.FullName.ToString(), customer.Email.ToString())
        );
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBookByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(getBookDto));

        // Act:
        var okObjectResult = (await _bookController.GetBookByIdAsync(
            It.IsAny<Guid>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var actualBook = okObjectResult!.Value as GetBookDto;
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(getBookDto.Id, actualBook!.Id);
        Assert.Equal(getBookDto.BookTitle, actualBook.BookTitle);
        Assert.Equal(getBookDto.Authors, actualBook.Authors);
        Assert.Equal(getBookDto.Edition, actualBook.Edition);
        Assert.Equal(getBookDto.Isbn, actualBook.Isbn);
        Assert.Equal(getBookDto.RentedBy, actualBook.RentedBy);
        Assert.Equal(getBookDto.DueDate, actualBook.DueDate);
        Assert.Equal(getBookDto.RentedBy, actualBook.RentedBy);
    }

    [Fact]
    public async Task CreateBookAsync_WithCreateBookResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateBookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<BookCreatedDto>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _bookController.CreateBookAsync(
            It.IsAny<CreateBookCommand>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _bookController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateBookAsync_WithMediaTypeVendorSpecific_ReturnsCreatedAtActionWithHateoasLinks()
    {
        // Arrange:
        var bookCreatedDto = new BookCreatedDto(
            Guid.NewGuid(),
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            1,
            "978-0132350884");
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateBookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(bookCreatedDto));

        // Act:
        var createdAtActionResult = await _bookController.CreateBookAsync(
            It.IsAny<CreateBookCommand>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        dynamic bookWithHateosLinks = (ExpandoObject)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(bookCreatedDto.Id, bookWithHateosLinks.id);
        Assert.Equal(bookCreatedDto.BookTitle, bookWithHateosLinks.bookTitle);
        Assert.Equal(bookCreatedDto.Edition, bookWithHateosLinks.edition);
        Assert.Equal(bookCreatedDto.Isbn, bookWithHateosLinks.isbn);
        Assert.Equal("self", bookWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_book", bookWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_book", bookWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task CreateBookAsync_WithMediaTypeNotVendorSpecific_ReturnsCreatedAtActionWithBook()
    {
        // Arrange:
        var expectedBookCreatedDto = new BookCreatedDto(
            Guid.NewGuid(),
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            1,
            "978-0132350884");
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateBookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedBookCreatedDto));

        // Act:
        var createdAtActionResult = await _bookController.CreateBookAsync(
            It.IsAny<CreateBookCommand>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        var actualBookCreatedDto = (BookCreatedDto)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(expectedBookCreatedDto.Id, actualBookCreatedDto.Id);
        Assert.Equal(expectedBookCreatedDto.BookTitle, actualBookCreatedDto.BookTitle);
        Assert.Equal(expectedBookCreatedDto.Edition, actualBookCreatedDto.Edition);
        Assert.Equal(expectedBookCreatedDto.Isbn, actualBookCreatedDto.Isbn);
    }

    [Fact]
    public async Task PatchBookTitleEditionAndIsbnByIdAsync_WithPatchBookTitleEditionAndIsbnByIdResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchBookTitleEditionAndIsbnByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _bookController.PatchBookTitleEditionAndIsbnByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto>>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _bookController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task PatchBookTitleEditionAndIsbnByIdAsync_WithPatchBookTitleEditionAndIsbnByIdResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchBookTitleEditionAndIsbnByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        var noContentResult = await _bookController.PatchBookTitleEditionAndIsbnByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto>>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Fact]
    public async Task DeleteBookByIdAsync_WithUnavailableBook_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteBookByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _bookController.DeleteBookByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _bookController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task DeleteBookByIdAsync_WithAvailableBook_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteBookByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        var noContentResult = await _bookController.DeleteBookByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Fact]
    public async Task GetBooksByQueryParametersExcludingFromAuthorAsync_WithGetAllBooksResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersExcludingFromAuthorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<PaginatedList<GetBookDto>>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = (await _bookController.GetBooksByQueryParametersExcludingFromAuthorAsync(
            It.IsAny<Guid>(),
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_bookController.ModelState.IsValid);
        Assert.Equal(1, _bookController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _bookController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetBooksByQueryParametersExcludingFromAuthorAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersExcludingFromAuthorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetBookDto>(_getBookDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _bookController.GetBooksByQueryParametersExcludingFromAuthorAsync(
            It.IsAny<Guid>(),
            new GetAllItemsQueryParameters(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)okObjectResult!.Value!;
        dynamic bookWithHateosLinks = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal(_getBookDtos[0].Id, bookWithHateosLinks.id);
        Assert.Equal(_getBookDtos[0].BookTitle, bookWithHateosLinks.bookTitle);
        Assert.Equal(_getBookDtos[0].Authors, bookWithHateosLinks.authors);
        Assert.Equal(_getBookDtos[0].Edition, bookWithHateosLinks.edition);
        Assert.Equal(_getBookDtos[0].Isbn, bookWithHateosLinks.isbn);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal(_getBookDtos[0].DueDate, bookWithHateosLinks.dueDate);
        Assert.Equal(_getBookDtos[0].RentedBy, bookWithHateosLinks.rentedBy);
        Assert.Equal("self", bookWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_book", bookWithHateosLinks.links[1].Rel);
        Assert.Equal("delete_book", bookWithHateosLinks.links[2].Rel);
    }

    [Fact]
    public async Task GetBooksByQueryParametersExcludingFromAuthorAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersExcludingFromAuthorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetBookDto>(_getBookDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _bookController.GetBooksByQueryParametersExcludingFromAuthorAsync(
            It.IsAny<Guid>(),
            new GetAllItemsQueryParameters(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(new PaginatedList<GetBookDto>(_getBookDtos, 3, 3, 2, 1), (PaginatedList<GetBookDto>)okObjectResult!.Value!);
    }
}
