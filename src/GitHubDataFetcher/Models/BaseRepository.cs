namespace GitHubDataFetcher.Models;

public class BaseRepository
{
    public string Name { get; init; }
    public string Url { get; init; }
    public User Owner { get; init; }
}
