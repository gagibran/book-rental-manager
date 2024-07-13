using System.Text;

namespace BookRentalManager.IntegrationTests.Common;

public abstract class IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected const string AuthorBaseUri = "api/v1/author";
    protected const string BookBaseUri = "api/v1/book";

    protected static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    protected HttpClient HttpClient { get; } = integrationTestsWebbApplicationFactory.CreateClient();

    protected async Task<TReturn> GetAsync<TReturn>(string acceptHeaderValue, string uri)
        where TReturn : class
    {
        HttpClient.DefaultRequestHeaders.Add("Accept", acceptHeaderValue);
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(uri);
        HttpClient.DefaultRequestHeaders.Clear();
        string content = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TReturn>(content, jsonSerializerOptions)!;
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
