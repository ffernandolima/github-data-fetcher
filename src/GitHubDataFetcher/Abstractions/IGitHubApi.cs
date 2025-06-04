using GitHubDataFetcher.Models.Responses;
using System.Threading.Tasks;

namespace GitHubDataFetcher.Abstractions;

public interface IGitHubApi
{
    Task<PullRequestStats> GetPullRequestsAsync();
    Task<IssueStats> GetIssuesAsync();
    Task<OrgData> GetOrganizationsAsync();
    Task<ProjectData> GetPinnedProjectsAsync();
}
