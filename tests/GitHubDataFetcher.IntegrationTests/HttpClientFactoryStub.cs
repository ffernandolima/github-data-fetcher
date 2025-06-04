using System.Net.Http;

namespace GitHubDataFetcher.IntegrationTests;

public class HttpClientFactoryStub : IHttpClientFactory
{
    public HttpClient CreateClient(string name = "") => new();
}
