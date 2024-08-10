using BookRentalManager.Domain.Enums;

namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class CustomerControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IntegrationTest(integrationTestsWebApplicationFactory)
{
    private static readonly List<GetCustomerDto> s_expectedCustomers =
    [
        new(
            It.IsAny<Guid>(),
            "John Doe",
            "john.doe@email.com",
            "+12002000000",
            [],
            CustomerType.Explorer.ToString(),
            0),
        new(
            It.IsAny<Guid>(),
            "Peter Griffin",
            "peter.griffin@email.com",
            "+15464056780",
            [],
            CustomerType.Explorer.ToString(),
            0),
        new(
            It.IsAny<Guid>(),
            "Rosanne Johnson",
            "rosane.johnson@email.com",
            "+15597852361",
            [
                new(
                    "Clean Code: A Handbook of Agile Software Craftsmanship",
                    1,
                    "978-0132350884",
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>())
            ],
            CustomerType.Explorer.ToString(),
            1),
        new(
            It.IsAny<Guid>(),
            "Sarah Smith",
            "sarah.smith@email.com",
            "+12352204063",
            [],
            CustomerType.Explorer.ToString(),
            0),
    ];

    public static TheoryData<string, List<GetCustomerDto>, IEnumerable<string>> GetCustomersByQueryParametersAsyncTestData()
    {
        return new()
        {
            {
                "sortBy=PhoneNumberDesc",
                s_expectedCustomers.OrderByDescending(customer => customer.PhoneNumber).ToList(),
                new List<string> { "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "sortBy=CustomerStatusDesc,FullName,CustomerPoints&pageSize=1&pageIndex=2",
                s_expectedCustomers
                    .OrderByDescending(customer => customer.CustomerStatus)
                    .ThenBy(customer => customer.FullName)
                    .ThenBy(customer => customer.CustomerPoints)
                    .Skip(1)
                    .Take(1)
                    .ToList(),
                new List<string> { "{\"totalAmountOfItems\":4,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":4}" }
            },
        };
    }

    public static TheoryData<string, string, string, int, int, ValidationProblemDetails> WithValidationErrorsTestData()
    {
        return new()
        {
            {
                "  ",
                "Doe",
                "juan.doe@email.com",
                866,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "firstName", ["First name cannot be empty."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "Juan",
                "",
                "juan.doe@email.com",
                866,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "lastName", ["Last name cannot be empty."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "Juan",
                "Doe",
                "emailIsIncorrect@email",
                866,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "email", ["Email address is not in a valid format."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "Juan",
                "Doe",
                "",
                866,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "email", ["Email address is not in a valid format."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "John",
                "Doe",
                "john.doe@email.com",
                866,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "customerEmailAlreadyExists", ["A customer with the email 'john.doe@email.com' already exists."]},
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "Juan",
                "Doe",
                "juan.doe@email.com",
                199,
                4254817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "invalidAreaCode", ["Invalid area code."]},
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "John",
                "",
                "john.doe@email.com",
                -1,
                42542817,
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "lastName", ["Last name cannot be empty."]},
                        { "invalidAreaCode", ["Invalid area code."]},
                        { "invalidPhoneNumber", ["Invalid phone number."]},
                        { "customerEmailAlreadyExists", ["A customer with the email 'john.doe@email.com' already exists."]},
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            }
        };
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithMediaTypeVendorSpecific_Returns200OkWithHateoasLinks()
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(CustomerBaseUri);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        CustomersWithHateoasLinks customersWithHateoasLinks = JsonSerializer.Deserialize<CustomersWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(Guid.Empty, customersWithHateoasLinks.Values[0].Id);
        Assert.Equal("John Doe", customersWithHateoasLinks.Values[0].FullName);
        Assert.Equal("john.doe@email.com", customersWithHateoasLinks.Values[0].Email);
        Assert.Equal("+12002000000", customersWithHateoasLinks.Values[0].PhoneNumber);
        Assert.Empty(customersWithHateoasLinks.Values[0].Books);
        Assert.Equal(CustomerType.Explorer.ToString(), customersWithHateoasLinks.Values[0].CustomerStatus);
        Assert.Equal(0, customersWithHateoasLinks.Values[0].CustomerPoints);
        Assert.NotEmpty(customersWithHateoasLinks.Values[0].Links);
        Assert.Empty(customersWithHateoasLinks.Links);
    }

    [Theory]
    [MemberData(nameof(GetCustomersByQueryParametersAsyncTestData))]
    public async Task GetCustomersByQueryParametersAsync_WithMediaTypeNotVendorSpecific_Returns200WithObjectAndXPaginationHeaders(
        string queryParameters,
        List<GetCustomerDto> expectedCustomers,
        IEnumerable<string> expectedXPaginationHeaders)
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}?{queryParameters}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        List<GetCustomerDto>? actualCustomers = await httpResponseMessage.Content.ReadFromJsonAsync<List<GetCustomerDto>>();
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
        for (int customerIndex = 0; customerIndex < expectedCustomers.Count; customerIndex++)
        {
            Assert.Equal(expectedCustomers[customerIndex].FullName, actualCustomers!.ElementAt(customerIndex).FullName);
            Assert.Equal(expectedCustomers[customerIndex].Email, actualCustomers!.ElementAt(customerIndex).Email);
            Assert.Equal(expectedCustomers[customerIndex].PhoneNumber, actualCustomers!.ElementAt(customerIndex).PhoneNumber);
            Assert.Equal(expectedCustomers[customerIndex].CustomerStatus, actualCustomers!.ElementAt(customerIndex).CustomerStatus);
            Assert.Equal(expectedCustomers[customerIndex].CustomerPoints, actualCustomers!.ElementAt(customerIndex).CustomerPoints);
        }
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithIncorrectParameterType_Returns400WithErrors()
    {
        // Arrange:
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "PageSize", ["The value 'notANumber' is not valid for PageSize."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}?pageSize=notANumber");

        // Assert:
        Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task GetCustomersByQueryParametersAsync_WithNonexistingQueryParameter_Returns422WithError()
    {
        // Arrange:
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "invalidProperty", ["The property 'notAValidParameter' does not exist for 'GetCustomerDto'."]}
            },
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "One or more validation errors occurred.",
            Status = 422
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}?sortBy=notAValidParameter");

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Theory]
    [InlineData("", "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=PhoneNumberDesc", "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=PhoneNumber&pageSize=1&pageIndex=2", "{\"totalAmountOfItems\":4,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":4}")]
    public async Task GetCustomersByQueryParametersAsync_WithHead_Returns200WithXPaginationHeaders(
        string queryParameters,
        string expectedReturnedXPaginationHeaders)
    {
        // Arrange:
        var expectedXPaginationHeaders = new List<string> { expectedReturnedXPaginationHeaders };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{CustomerBaseUri}{queryParameters}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
    }

    [Fact]
    public async Task GetCustomerById_WithNonexistingCustomer_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingCustomerId = Guid.NewGuid();
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "idNotFound", [$"No customer with the ID of '{nonexistingCustomerId}' was found."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "One or more validation errors occurred.",
            Status = 404
        };
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}/{nonexistingCustomerId}");

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetCustomerByIdAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks(int currentCustomerIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetCustomerDto>(
            currentCustomerIndex,
            CustomerBaseUri,
            getCustomerDto => getCustomerDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        CustomerWithHateoasLinks customerWithLinks = JsonSerializer.Deserialize<CustomerWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].FullName, customerWithLinks.FullName);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].Email, customerWithLinks.Email);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].PhoneNumber, customerWithLinks.PhoneNumber);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].CustomerStatus, customerWithLinks.CustomerStatus);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].CustomerPoints, customerWithLinks.CustomerPoints);
        Assert.Equal("self", customerWithLinks.Links[0].Rel);
        Assert.Equal("patch_customer", customerWithLinks.Links[1].Rel);
        Assert.Equal("rent_books", customerWithLinks.Links[2].Rel);
        Assert.Equal("return_books", customerWithLinks.Links[3].Rel);
        Assert.Equal("delete_customer", customerWithLinks.Links[4].Rel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetCustomerByIdAsync_WithMediaTypeNotVendorSpecific_Returns200WithObject(int currentCustomerIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetCustomerDto>(
            currentCustomerIndex,
            CustomerBaseUri,
            getCustomerDto => getCustomerDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{CustomerBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetCustomerDto? actualCustomer = await httpResponseMessage.Content.ReadFromJsonAsync<GetCustomerDto>();
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].FullName, actualCustomer!.FullName);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].Email, actualCustomer.Email);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].PhoneNumber, actualCustomer.PhoneNumber);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].CustomerStatus, actualCustomer.CustomerStatus);
        Assert.Equal(s_expectedCustomers[currentCustomerIndex].CustomerPoints, actualCustomer.CustomerPoints);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetCustomerByIdAsync_WithHeadAndMediaTypeVendorSpecific_Returns200WithContentTypeHeaders(int currentCustomerIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetCustomerDto>(
            currentCustomerIndex,
            CustomerBaseUri,
            getCustomerDto => getCustomerDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
        var expectedContentTypeHeaders = new List<string>
        {
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{CustomerBaseUri}/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetCustomerByIdAsync_WithHeadAndMediaTypeNotVendorSpecific_Returns200WithContentTypeHeaders(int currentCustomerIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetCustomerDto>(
            currentCustomerIndex,
            CustomerBaseUri,
            getCustomerDto => getCustomerDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        var expectedContentTypeHeaders = new List<string>
        {
            MediaTypeNames.Application.Json + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{CustomerBaseUri}/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithMediaTypeVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedFirstName = "Juan";
        const string ExpectedLastName = "Doe";
        const string ExpectedEmail = "juan.doe@email.com";
        const string ExpectedAreaCode = "834";
        const string ExpectedPrefixAndLineNumber = "4552897";
        var expectedHateoasLinks = new List<HateoasLinkDto>
        {
            new(It.IsAny<string>(), "self", "GET"),
            new(It.IsAny<string>(), "patch_customer", "PATCH"),
            new(It.IsAny<string>(), "rent_books", "PATCH"),
            new(It.IsAny<string>(), "return_books", "PATCH"),
            new(It.IsAny<string>(), "delete_customer", "DELETE")
        };
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CustomerBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{ExpectedFirstName}\", \"lastName\": \"{ExpectedLastName}\", \"email\": \"{ExpectedEmail}\", \"phoneNumber\": {{\"areaCode\": {ExpectedAreaCode}, \"prefixAndLineNumber\": {ExpectedPrefixAndLineNumber}}}}}",
                Encoding.UTF8,
                CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedCustomerId = JsonSerializer.Deserialize<CustomerCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        var actualCustomerWithLinks = await GetAsync<CustomerWithHateoasLinks>(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            $"{CustomerBaseUri}/{actualCreatedCustomerId}");
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualCustomerWithLinks.FullName);
        Assert.Equal(ExpectedEmail, actualCustomerWithLinks.Email);
        Assert.Equal($"+1{ExpectedAreaCode}{ExpectedPrefixAndLineNumber}", actualCustomerWithLinks.PhoneNumber);
        Assert.Empty(actualCustomerWithLinks.Books);
        Assert.Equal(CustomerType.Explorer.ToString(), actualCustomerWithLinks.CustomerStatus);
        Assert.Equal(0, actualCustomerWithLinks.CustomerPoints);
        Assert.Equal(actualCreatedCustomerId, actualCustomerWithLinks.Id);
        Assert.Equal(expectedHateoasLinks[0].Rel, actualCustomerWithLinks.Links[0].Rel);
        Assert.Equal(expectedHateoasLinks[1].Rel, actualCustomerWithLinks.Links[1].Rel);
        Assert.Equal(expectedHateoasLinks[2].Rel, actualCustomerWithLinks.Links[2].Rel);
        Assert.Equal(expectedHateoasLinks[0].Method, actualCustomerWithLinks.Links[0].Method);
        Assert.Equal(expectedHateoasLinks[1].Method, actualCustomerWithLinks.Links[1].Method);
        Assert.Equal(expectedHateoasLinks[2].Method, actualCustomerWithLinks.Links[2].Method);

        // Clean up:
        await HttpClient.DeleteAsync($"{CustomerBaseUri}/{actualCreatedCustomerId}");
    }

    [Fact]
    public async Task CreateCustomerAsync_WithMediaTypeNotVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedFirstName = "Juan";
        const string ExpectedLastName = "Doe";
        const string ExpectedEmail = "juan.doe@email.com";
        const string ExpectedAreaCode = "834";
        const string ExpectedPrefixAndLineNumber = "4552897";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CustomerBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{ExpectedFirstName}\", \"lastName\": \"{ExpectedLastName}\", \"email\": \"{ExpectedEmail}\", \"phoneNumber\": {{\"areaCode\": {ExpectedAreaCode}, \"prefixAndLineNumber\": {ExpectedPrefixAndLineNumber}}}}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedCustomerId = JsonSerializer.Deserialize<CustomerCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        GetCustomerDto actualCustomer = await GetAsync<GetCustomerDto>(
            MediaTypeNames.Application.Json,
            $"{CustomerBaseUri}/{actualCreatedCustomerId}");
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualCustomer.FullName);
        Assert.Equal(ExpectedEmail, actualCustomer.Email);
        Assert.Equal($"+1{ExpectedAreaCode}{ExpectedPrefixAndLineNumber}", actualCustomer.PhoneNumber);
        Assert.Empty(actualCustomer.Books);
        Assert.Equal(CustomerType.Explorer.ToString(), actualCustomer.CustomerStatus);
        Assert.Equal(0, actualCustomer.CustomerPoints);

        // Clean up:
        await HttpClient.DeleteAsync($"{CustomerBaseUri}/{actualCreatedCustomerId}");
    }

    [Theory]
    [MemberData(nameof(WithValidationErrorsTestData))]
    public async Task CreateCustomerAsync_WithInvalidParameters_Returns422WithErrorMessage(
        string firstName,
        string lastName,
        string email,
        int areaCode,
        int prefixAndLineNumber,
        ValidationProblemDetails expectedValidationProblemDetails)
    {
        // Arrange:
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CustomerBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{firstName}\", \"lastName\": \"{lastName}\", \"email\": \"{email}\", \"phoneNumber\": {{\"areaCode\": {areaCode}, \"prefixAndLineNumber\": {prefixAndLineNumber}}}}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }
}
