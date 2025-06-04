using System.Threading.Tasks;

namespace GitHubDataFetcher.Abstractions;

public interface IGitHubJsonWriter
{
    Task GeneratePullRequestsAsync();
    Task GenerateIssuesAsync();
    Task GenerateOrganizationsAsync();
    Task GeneratePinnedProjectsAsync();
}
