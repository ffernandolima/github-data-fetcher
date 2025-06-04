using System.Collections.Generic;

namespace GitHubDataFetcher.Models.Responses;

public record ProjectData
{
    public IEnumerable<Project> Data { get; init; } = [];
}
