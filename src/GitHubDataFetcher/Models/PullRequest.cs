using System;

namespace GitHubDataFetcher.Models;

public class PullRequest
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string Url { get; init; }
    public string State { get; init; }
    public DateTime CreatedAt { get; init; }
    public int Number { get; init; }
    public int ChangedFiles { get; init; }
    public int Additions { get; init; }
    public int Deletions { get; init; }
    public User MergedBy { get; init; }
    public BaseRepository BaseRepository { get; init; }
}
