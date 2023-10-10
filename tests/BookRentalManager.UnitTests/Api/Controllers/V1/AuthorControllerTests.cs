using BookRentalManager.Application.Common;
using Microsoft.AspNetCore.Http;

namespace BookRentalManager.UnitTests;

public class AuthorControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly AuthorController _authorController;

    public AuthorControllerTests()
    {
        var httpContext = new DefaultHttpContext();
        var urlHelperStub = new Mock<IUrlHelper>();
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
            .ReturnsAsync(Result.Fail<PaginatedList<GetAuthorDto>>(
                "errorType",
                "errorMessage"));

        // Act:
        var result = (await _authorController.GetAuthorsByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.False(_authorController.ModelState.IsValid);
        Assert.Equal(1, _authorController.ModelState.ErrorCount);
        Assert.Equal("errorMessage", _authorController.ModelState["errorType"]!.Errors[0].ErrorMessage);
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, result!.StatusCode);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        var getAuthorDtos = new List<GetAuthorDto>
        {
            new GetAuthorDto(
                Guid.NewGuid(),
                FullName.Create("John", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()),
            new GetAuthorDto(
                Guid.NewGuid(),
                FullName.Create("Jane", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()),
            new GetAuthorDto(
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
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
        Assert.Equal("url", returnedExpandoObject.links[0].Href);
        Assert.Equal("url", returnedExpandoObject.links[1].Href);
        Assert.Equal("url", returnedExpandoObject.links[2].Href);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeNotVendorSpecific_ReturnsOkAllAuthors()
    {
        // Arrange:
        var getAuthorDtos = new List<GetAuthorDto>
        {
            new GetAuthorDto(
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
        Assert.Equal(expectedPaginatedGetAuthorDtos, (PaginatedList<GetAuthorDto>)result!.Value!);
        Assert.Equal((int)HttpStatusCode.OK, result!.StatusCode);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_WithUnsuccessfulGetAuthorByIdResult_ReturnsError()
    {
        // Arrange:
        
    
        // Act:
        
    
        // Assert:
        
    }
}
