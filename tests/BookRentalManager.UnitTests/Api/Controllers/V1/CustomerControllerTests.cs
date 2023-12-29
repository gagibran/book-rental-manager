namespace BookRentalManager.UnitTests.Api.Controllers.V1;

public sealed class CustomerControllerTests
{
    private readonly Mock<IDispatcher> _dispatcherStub;
    private readonly List<GetCustomerDto> _getCustomerDtos;
    private readonly CustomerController _customerController;

    public CustomerControllerTests()
    {
        var urlHelperStub = new Mock<IUrlHelper>();
        _getCustomerDtos =
        [
            new (TestFixtures.CreateDummyCustomer()),
            new(new Customer(
                FullName.Create("Jane", "Doe").Value!,
                Email.Create("jane.doe@email.com").Value!,
                PhoneNumber.Create(400, 4_000_000).Value!)),
            new(new Customer(
                FullName.Create("James", "Smith").Value!,
                Email.Create("james.smith@email.com").Value!,
                PhoneNumber.Create(300, 3_000_000).Value!)),
        ];
        _dispatcherStub = new();
        _customerController = new(_dispatcherStub.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            Url = urlHelperStub.Object
        };
        urlHelperStub
            .Setup(urlHelper => urlHelper.Link(It.IsAny<string>(), It.IsAny<object?>()))
            .Returns("url");
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithGetAllCustomersResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomersByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<PaginatedList<GetCustomerDto>>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = (await _customerController.GetCustomersByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _customerController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateoasLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomersByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<GetCustomerDto>(_getCustomerDtos, 3, 3, 2, 1)));

        // Act:
        var okObjectResult = (await _customerController.GetCustomersByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var collectionWithHateoasLinksDto = (CollectionWithHateoasLinksDto)okObjectResult!.Value!;
        dynamic customerWithHateosLinks = collectionWithHateoasLinksDto.Values[0];
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal("previous_page", collectionWithHateoasLinksDto.Links[0].Rel);
        Assert.Equal("next_page", collectionWithHateoasLinksDto.Links[1].Rel);
        Assert.Equal(_getCustomerDtos[0].Id, customerWithHateosLinks.id);
        Assert.Equal(_getCustomerDtos[0].FullName, customerWithHateosLinks.fullName);
        Assert.Equal(_getCustomerDtos[0].Email, customerWithHateosLinks.email);
        Assert.Equal(_getCustomerDtos[0].PhoneNumber, customerWithHateosLinks.phoneNumber);
        Assert.Equal(_getCustomerDtos[0].Books, customerWithHateosLinks.books);
        Assert.Equal(_getCustomerDtos[0].CustomerStatus, customerWithHateosLinks.customerStatus);
        Assert.Equal(_getCustomerDtos[0].CustomerPoints, customerWithHateosLinks.customerPoints);
        Assert.Equal("self", customerWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_customer", customerWithHateosLinks.links[1].Rel);
        Assert.Equal("rent_books", customerWithHateosLinks.links[2].Rel);
        Assert.Equal("return_books", customerWithHateosLinks.links[3].Rel);
        Assert.Equal("delete_customer", customerWithHateosLinks.links[4].Rel);
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithAllCustomers()
    {
        // Arrange:
        var expectedPaginatedGetCustomerDtos = new PaginatedList<GetCustomerDto>(_getCustomerDtos, 3, 3, 2, 1);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomersByQueryParametersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedPaginatedGetCustomerDtos));

        // Act:
        var okObjectResult = (await _customerController.GetCustomersByQueryParametersAsync(
            new GetAllItemsQueryParameters(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(expectedPaginatedGetCustomerDtos, (PaginatedList<GetCustomerDto>)okObjectResult!.Value!);
    }

    [Fact]
    public async Task GeCustomerByIdAsync_WithGetCustomerByIdResultUnsuccessful_ReturnsNotFound()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<GetCustomerDto>("idNotFound", "errorMessage404"));

        // Act:
        var objectResult = (await _customerController.GetCustomerByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Result as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NotFound, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage404", _customerController.ModelState["idNotFound"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithMediaTypeVendorSpecific_ReturnsOkWithHateosLinks()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(_getCustomerDtos[0]));

        // Act:
        var okObjectResult = (await _customerController.GetCustomerByIdAsync(
            It.IsAny<Guid>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        dynamic customerWithHateosLinks = (ExpandoObject)okObjectResult!.Value!;
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(_getCustomerDtos[0].Id, customerWithHateosLinks.id);
        Assert.Equal(_getCustomerDtos[0].FullName, customerWithHateosLinks.fullName);
        Assert.Equal(_getCustomerDtos[0].Email, customerWithHateosLinks.email);
        Assert.Equal(_getCustomerDtos[0].PhoneNumber, customerWithHateosLinks.phoneNumber);
        Assert.Equal(_getCustomerDtos[0].Books, customerWithHateosLinks.books);
        Assert.Equal(_getCustomerDtos[0].CustomerStatus, customerWithHateosLinks.customerStatus);
        Assert.Equal(_getCustomerDtos[0].CustomerPoints, customerWithHateosLinks.customerPoints);
        Assert.Equal("self", customerWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_customer", customerWithHateosLinks.links[1].Rel);
        Assert.Equal("rent_books", customerWithHateosLinks.links[2].Rel);
        Assert.Equal("return_books", customerWithHateosLinks.links[3].Rel);
        Assert.Equal("delete_customer", customerWithHateosLinks.links[4].Rel);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithMediaTypeNotVendorSpecific_ReturnsOkWithCustomer()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(_getCustomerDtos[0]));

        // Act:
        var okObjectResult = (await _customerController.GetCustomerByIdAsync(
            It.IsAny<Guid>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>())).Result as OkObjectResult;

        // Assert:
        var actualCustomer = okObjectResult!.Value as GetCustomerDto;
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult!.StatusCode);
        Assert.Equal(_getCustomerDtos[0].Id, actualCustomer!.Id);
        Assert.Equal(_getCustomerDtos[0].FullName, actualCustomer.FullName);
        Assert.Equal(_getCustomerDtos[0].Email, actualCustomer.Email);
        Assert.Equal(_getCustomerDtos[0].PhoneNumber, actualCustomer.PhoneNumber);
        Assert.Equal(_getCustomerDtos[0].Books, actualCustomer.Books);
        Assert.Equal(_getCustomerDtos[0].CustomerStatus, actualCustomer.CustomerStatus);
        Assert.Equal(_getCustomerDtos[0].CustomerPoints, actualCustomer.CustomerPoints);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithCreateCustomerResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<CustomerCreatedDto>("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _customerController.CreateCustomerAsync(
            It.IsAny<CreateCustomerCommand>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _customerController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithMediaTypeVendorSpecific_ReturnsCreatedAtActionWithHateoasLinks()
    {
        // Arrange:
        var customerCreatedDto = new CustomerCreatedDto(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@email.com",
            200,
            2_000_000,
            "Explorer",
            0);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(customerCreatedDto));

        // Act:
        var createdAtActionResult = await _customerController.CreateCustomerAsync(
            It.IsAny<CreateCustomerCommand>(),
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        dynamic customerWithHateosLinks = (ExpandoObject)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(customerCreatedDto.Id, customerWithHateosLinks.id);
        Assert.Equal(customerCreatedDto.FirstName, customerWithHateosLinks.firstName);
        Assert.Equal(customerCreatedDto.LastName, customerWithHateosLinks.lastName);
        Assert.Equal(customerCreatedDto.AreaCode, customerWithHateosLinks.areaCode);
        Assert.Equal(customerCreatedDto.PrefixAndLineNumber, customerWithHateosLinks.prefixAndLineNumber);
        Assert.Equal(customerCreatedDto.CustomerStatus, customerWithHateosLinks.customerStatus);
        Assert.Equal(customerCreatedDto.CustomerPoints, customerWithHateosLinks.customerPoints);
        Assert.Equal("self", customerWithHateosLinks.links[0].Rel);
        Assert.Equal("patch_customer", customerWithHateosLinks.links[1].Rel);
        Assert.Equal("rent_books", customerWithHateosLinks.links[2].Rel);
        Assert.Equal("return_books", customerWithHateosLinks.links[3].Rel);
        Assert.Equal("delete_customer", customerWithHateosLinks.links[4].Rel);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithMediaTypeNotVendorSpecific_ReturnsCreatedAtActionWithCustomer()
    {
        // Arrange:
        var expectedCustomerCreatedDto = new CustomerCreatedDto(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@email.com",
            200,
            2_000_000,
            "Explorer",
            0);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedCustomerCreatedDto));

        // Act:
        var createdAtActionResult = await _customerController.CreateCustomerAsync(
            It.IsAny<CreateCustomerCommand>(),
            MediaTypeNames.Application.Json,
            It.IsAny<CancellationToken>()) as CreatedAtActionResult;

        // Assert:
        var actualCustomerCreatedDto = (CustomerCreatedDto)createdAtActionResult!.Value!;
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult!.StatusCode);
        Assert.Equal(expectedCustomerCreatedDto.Id, actualCustomerCreatedDto.Id);
        Assert.Equal(expectedCustomerCreatedDto.FirstName, actualCustomerCreatedDto.FirstName);
        Assert.Equal(expectedCustomerCreatedDto.LastName, actualCustomerCreatedDto.LastName);
        Assert.Equal(expectedCustomerCreatedDto.AreaCode, actualCustomerCreatedDto.AreaCode);
        Assert.Equal(expectedCustomerCreatedDto.PrefixAndLineNumber, actualCustomerCreatedDto.PrefixAndLineNumber);
        Assert.Equal(expectedCustomerCreatedDto.CustomerStatus, actualCustomerCreatedDto.CustomerStatus);
        Assert.Equal(expectedCustomerCreatedDto.CustomerPoints, actualCustomerCreatedDto.CustomerPoints);
    }

    [Fact]
    public async Task PatchCustomerNameAndPhoneNumberByIdAsync_WithPatchCustomerNameAndPhoneNumberResultUnsuccessful_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchCustomerNameAndPhoneNumberByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _customerController.PatchCustomerNameAndPhoneNumberByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _customerController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task PatchCustomerNameAndPhoneNumberByIdAsync_WithPatchCustomerNameAndPhoneNumberResultSuccessful_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<PatchCustomerNameAndPhoneNumberByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        var noContentResult = await _customerController.PatchCustomerNameAndPhoneNumberByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Theory]
    [InlineData("/rent", false)]
    [InlineData("/return", true)]
    public async Task ChangeCustomerBooksByBookIdsAsync_WithChangeCustomerBooksByBookIdsResultUnsuccessful_ReturnsUnprocessableEntityAndCallsDispatchAsyncWithExpectedChangeAction(
        string changeAction,
        bool expectedIsReturn)
    {
        // Arrange:
        _customerController.Request.Path = new PathString(changeAction);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<ChangeCustomerBooksByBookIdsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _customerController.ChangeCustomerBooksByBookIdsAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<ChangeCustomerBooksByBookIdsDto>>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        _dispatcherStub.Verify(
            dispatcher => dispatcher.DispatchAsync(
                It.Is<ChangeCustomerBooksByBookIdsCommand>(changeCustomerBooksByBookIdsCommand => changeCustomerBooksByBookIdsCommand.IsReturn == expectedIsReturn),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _customerController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("/test", false)]
    [InlineData("/return", true)]
    public async Task ChangeCustomerBooksByBookIdsAsync_WithChangeCustomerBooksByBookIdsResultSuccessful_ReturnsNoContentAndCallsDispatchAsyncWithExpectedChangeAction(
        string changeAction,
        bool expectedIsReturn)
    {
        // Arrange:
        _customerController.Request.Path = new PathString(changeAction);
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<ChangeCustomerBooksByBookIdsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        var noContentResult = await _customerController.ChangeCustomerBooksByBookIdsAsync(
            It.IsAny<Guid>(),
            It.IsAny<JsonPatchDocument<ChangeCustomerBooksByBookIdsDto>>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        _dispatcherStub.Verify(
            dispatcher => dispatcher.DispatchAsync(
                It.Is<ChangeCustomerBooksByBookIdsCommand>(changeCustomerBooksByBookIdsCommand => changeCustomerBooksByBookIdsCommand.IsReturn == expectedIsReturn),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomerByIdAsync_WithUnavailableCustomer_ReturnsUnprocessableEntity()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteCustomerByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("unprocessableEntity", "errorMessage422"));

        // Act:
        var objectResult = await _customerController.DeleteCustomerByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()) as ObjectResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult!.StatusCode);
        Assert.False(_customerController.ModelState.IsValid);
        Assert.Equal(1, _customerController.ModelState.ErrorCount);
        Assert.Equal("errorMessage422", _customerController.ModelState["unprocessableEntity"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task DeleteCustomerByIdAsync_WithAvailableCustomer_ReturnsNoContent()
    {
        // Arrange:
        _dispatcherStub
            .Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<DeleteCustomerByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act:
        var noContentResult = await _customerController.DeleteCustomerByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()) as NoContentResult;

        // Assert:
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult!.StatusCode);
    }
}
