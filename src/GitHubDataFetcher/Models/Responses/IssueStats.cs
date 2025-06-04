using System.Collections.Generic;

namespace GitHubDataFetcher.Models.Responses;

public record IssueStats
{
    public IEnumerable<Issue> Data { get; init; } = [];
    public int Open { get; init; }
    public int Closed { get; init; }
    public int TotalCount { get; init; }
}
