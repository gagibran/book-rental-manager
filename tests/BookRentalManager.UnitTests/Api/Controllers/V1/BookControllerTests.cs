namespace BookRentalManager.UnitTests;

public class BookControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly BookController _bookController;

    public BookControllerTests()
    {
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
        var getAuthorFromBookDtos = new List<GetAuthorFromBookDto>
        {
            new(FullName.Create("Erich", "Gamma").Value!),
            new(FullName.Create("John", "Vlissides").Value!),
            new(FullName.Create("Ralph", "Johnson").Value!),
            new(FullName.Create("Richard", "Helm").Value!),
        };
        var getBookDtos = new List<GetBookDto>
        {
            new(
                Guid.NewGuid(),
                "Design Patterns: Elements of Reusable Object-Oriented Software",
                getAuthorFromBookDtos,
                Edition.Create(1).Value!,
                Isbn.Create("0-201-63361-2").Value!,
                null,
                null,
                null)
        };
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetBooksByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetBookDto>(getBookDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _bookController.GetBooksByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            MediaTypeConstants.BookRentalManagerHateoasMediaType,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)okObjectResult!.Value!;
        dynamic returnedExpandoObject = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal("url", returnedExpandoObject.links[0].Href);
        Assert.Equal("url", returnedExpandoObject.links[1].Href);
        Assert.Equal("url", returnedExpandoObject.links[2].Href);
        Assert.Equal(returnedExpandoObject.authors.Count, 4);
        Assert.Equal(returnedExpandoObject.bookTitle, "Design Patterns: Elements of Reusable Object-Oriented Software");
        Assert.Equal(returnedExpandoObject.isbn, "0-201-63361-2");
    }
}
