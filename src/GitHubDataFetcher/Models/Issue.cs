using System;
using System.Collections.Generic;

namespace GitHubDataFetcher.Models;

public class Issue
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string Url { get; init; }
    public int Number { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool Closed { get; init; }
    public IEnumerable<User> Assignees { get; init; } = [];
    public Repository Repository { get; init; }
}
