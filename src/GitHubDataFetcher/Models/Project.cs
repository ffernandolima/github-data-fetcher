using System.Collections.Generic;

namespace GitHubDataFetcher.Models;

public class Project
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string CreatedAt { get; init; }
    public string Url { get; init; }
    public string Description { get; init; }
    public bool IsFork { get; init; }
    public IEnumerable<Language> Languages { get; init; } = [];
}
