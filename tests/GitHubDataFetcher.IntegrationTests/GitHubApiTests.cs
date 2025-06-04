using GitHubDataFetcher.Abstractions;
using GitHubDataFetcher.Implementation;
using GitHubDataFetcher.Models.Responses;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GitHubDataFetcher.IntegrationTests;

public class GitHubApiTests
{
    private readonly IGitHubApi _githubApi;
    private readonly IConfiguration _configuration;

    public GitHubApiTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        _githubApi = new GitHubApi(_configuration, new HttpClientFactoryStub());
    }

    [Fact]
    public async Task GetPullRequestsAsync_ShouldReturnData()
    {
        var result = await _githubApi.GetPullRequestsAsync();

        Assert.NotNull(result);
        Assert.IsType<PullRequestStats>(result);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldReturnData()
    {
        var result = await _githubApi.GetIssuesAsync();

        Assert.NotNull(result);
        Assert.IsType<IssueStats>(result);
    }

    [Fact]
    public async Task GetOrganizationsAsync_ShouldReturnData()
    {
        var result = await _githubApi.GetOrganizationsAsync();

        Assert.NotNull(result);
        Assert.IsType<OrgData>(result);
    }

    [Fact]
    public async Task GetPinnedProjectsAsync_ShouldReturnData()
    {
        var result = await _githubApi.GetPinnedProjectsAsync();

        Assert.NotNull(result);
        Assert.IsType<ProjectData>(result);
    }
}
