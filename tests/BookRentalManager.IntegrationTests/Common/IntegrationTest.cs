namespace BookRentalManager.IntegrationTests.Common;

public abstract class IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected const string AuthorBaseUri = "api/v1/author";
    protected const string BookBaseUri = "api/v1/book";

    protected static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    protected HttpClient HttpClient { get; } = integrationTestsWebApplicationFactory.CreateClient();

    protected async Task<TReturn> GetAsync<TReturn>(string acceptHeaderValue, string uri)
        where TReturn : class
    {
        HttpClient.DefaultRequestHeaders.Add("Accept", acceptHeaderValue);
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(uri);
        HttpClient.DefaultRequestHeaders.Clear();
        string content = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TReturn>(content, jsonSerializerOptions)!;
    }

    protected async Task<Guid> GetIdOrderedByConditionAsync<TDto>(int index, string baseUri, Func<TDto, string> condition)
        where TDto : IdentifiableDto
    {
        return (await GetAsync<List<TDto>>(MediaTypeNames.Application.Json, baseUri))
            .OrderBy(condition)
            .ElementAt(index).Id;
    }

    protected async Task<TReturn> CreateAsync<TReturn>(string content, string contentType, string uri)
        where TReturn : class
    {
        var stringContent = new StringContent(content, Encoding.UTF8, contentType);
        var httpResponseMessage = await HttpClient.PostAsync(uri, stringContent);
        string author = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TReturn>(author, jsonSerializerOptions)!;
    }
}
