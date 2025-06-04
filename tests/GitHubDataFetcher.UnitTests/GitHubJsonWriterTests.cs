using GitHubDataFetcher.Abstractions;
using GitHubDataFetcher.Implementation;
using GitHubDataFetcher.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubDataFetcher.UnitTests;

public class GitHubJsonWriterTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<IGitHubApi> _githubApiMock;
    private readonly GitHubJsonWriter _githubJsonWriter;

    public GitHubJsonWriterTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string>("OUTPUT_PATH", "GitHubDataFetcher")])
            .Build();

        _githubApiMock = new Mock<IGitHubApi>();

        _githubJsonWriter = new GitHubJsonWriter(
            NullLogger<GitHubJsonWriter>.Instance,
            _configuration,
            _githubApiMock.Object);
    }

    [Fact]
    public async Task GeneratePullRequestsAsync_CallsApiAndWritesFile()
    {
        _githubApiMock.Setup(x => x.GetPullRequestsAsync())
               .ReturnsAsync(new PullRequestStats());

        await _githubJsonWriter.GeneratePullRequestsAsync();

        _githubApiMock.Verify(x => x.GetPullRequestsAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateIssuesAsync_CallsApiAndWritesFile()
    {
        _githubApiMock.Setup(x => x.GetIssuesAsync())
            .ReturnsAsync(new IssueStats());

        await _githubJsonWriter.GenerateIssuesAsync();

        _githubApiMock.Verify(x => x.GetIssuesAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateOrganizationsAsync_CallsApiAndWritesFile()
    {
        _githubApiMock.Setup(x => x.GetOrganizationsAsync())
            .ReturnsAsync(new OrgData());

        await _githubJsonWriter.GenerateOrganizationsAsync();

        _githubApiMock.Verify(x => x.GetOrganizationsAsync(), Times.Once);
    }

    [Fact]
    public async Task GeneratePinnedProjectsAsync_CallsApiAndWritesFile()
    {
        _githubApiMock.Setup(x => x.GetPinnedProjectsAsync())
            .ReturnsAsync(new ProjectData());

        await _githubJsonWriter.GeneratePinnedProjectsAsync();

        _githubApiMock.Verify(x => x.GetPinnedProjectsAsync(), Times.Once);
    }
}
