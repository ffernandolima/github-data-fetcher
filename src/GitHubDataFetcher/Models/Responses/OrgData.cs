using System.Collections.Generic;

namespace GitHubDataFetcher.Models.Responses;

public record OrgData
{
    public IEnumerable<Organization> Data { get; init; } = [];
}
