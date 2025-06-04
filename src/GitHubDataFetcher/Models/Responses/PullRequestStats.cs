using System.Collections.Generic;

namespace GitHubDataFetcher.Models.Responses;

public record PullRequestStats
{
    public IEnumerable<PullRequest> Data { get; init; } = [];
    public int Open { get; init; }
    public int Merged { get; init; }
    public int Closed { get; init; }
    public int TotalCount { get; init; }
}
