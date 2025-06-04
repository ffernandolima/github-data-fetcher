using GitHubDataFetcher.Abstractions;
using GitHubDataFetcher.Models;
using GitHubDataFetcher.Models.Responses;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubDataFetcher.Implementation;

public class GitHubApi : IGitHubApi
{
    private const string BaseUrl = "https://api.github.com/graphql";

    private readonly string _username;
    private readonly HttpClient _httpClient;

    public GitHubApi(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        var token = configuration["GITHUB_TOKEN"]!;
        _username = configuration["GITHUB_USERNAME"]!;

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("GitHubDataFetcher");
    }

    private async Task<JsonDocument> PostGraphQLAsync(string query)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, new { query });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    public async Task<PullRequestStats> GetPullRequestsAsync()
    {
        var document = await PostGraphQLAsync($@"
            query {{
                user(login: ""{_username}"") {{
                    pullRequests(last: 100, orderBy: {{ field: CREATED_AT, direction: DESC }}) {{
                        totalCount
                        nodes {{
                            id
                            title
                            url
                            state
                            mergedBy {{
                                avatarUrl
                                url
                                login
                            }}
                            createdAt
                            number
                            changedFiles
                            additions
                            deletions
                            baseRepository {{
                                name
                                url
                                owner {{
                                    avatarUrl
                                    url
                                    login
                                }}
                            }}
                        }}
                    }}
                }}
            }}");

        PullRequest[] nodes = [.. document.RootElement
            .GetProperty("data").GetProperty("user").GetProperty("pullRequests").GetProperty("nodes")
            .EnumerateArray()
            .Select(node => new PullRequest
            {
                Id = node.GetProperty("id").GetString()!,
                Title = node.GetProperty("title").GetString()!,
                Url = node.GetProperty("url").GetString()!,
                State = node.GetProperty("state").GetString()!,
                CreatedAt = node.GetProperty("createdAt").GetDateTime(),
                Number = node.GetProperty("number").GetInt32(),
                ChangedFiles = node.GetProperty("changedFiles").GetInt32(),
                Additions = node.GetProperty("additions").GetInt32(),
                Deletions = node.GetProperty("deletions").GetInt32(),
                MergedBy = node.TryGetProperty("mergedBy", out var mergedBy) && mergedBy.ValueKind == JsonValueKind.Object
                    ? new User
                    {
                        AvatarUrl = mergedBy.GetProperty("avatarUrl").GetString()!,
                        Login = mergedBy.GetProperty("login").GetString()!,
                        Url = mergedBy.GetProperty("url").GetString()!
                    }
                    : null,
                BaseRepository = new BaseRepository
                {
                    Name = node.GetProperty("baseRepository").GetProperty("name").GetString()!,
                    Url = node.GetProperty("baseRepository").GetProperty("url").GetString()!,
                    Owner = new User
                    {
                        AvatarUrl = node.GetProperty("baseRepository").GetProperty("owner").GetProperty("avatarUrl").GetString()!,
                        Login = node.GetProperty("baseRepository").GetProperty("owner").GetProperty("login").GetString()!,
                        Url = node.GetProperty("baseRepository").GetProperty("owner").GetProperty("url").GetString()!
                    }
                }
            })];

        return new PullRequestStats
        {
            Data = nodes,
            Open = nodes.Count(x => x.State == "OPEN"),
            Merged = nodes.Count(x => x.State == "MERGED"),
            Closed = nodes.Count(x => x.State != "OPEN" && x.State != "MERGED"),
            TotalCount = nodes.Length
        };
    }

    public async Task<IssueStats> GetIssuesAsync()
    {
        var document = await PostGraphQLAsync($@"
            query {{
                user(login: ""{_username}"") {{
                    issues(last: 100, orderBy: {{ field: CREATED_AT, direction: DESC }}) {{
                        totalCount
                        nodes {{
                            id
                            title
                            url
                            createdAt
                            number
                            closed
                            assignees(first:100) {{
                                nodes {{
                                    avatarUrl
                                    url
                                    name
                                }}
                            }}
                            repository {{
                                name
                                url
                                owner {{
                                    avatarUrl
                                    url
                                    login
                                }}
                            }}
                        }}
                    }}
                }}
            }}");

        Issue[] nodes = [.. document.RootElement
            .GetProperty("data").GetProperty("user").GetProperty("issues").GetProperty("nodes")
            .EnumerateArray()
            .Select(node => new Issue
            {
                Id = node.GetProperty("id").GetString()!,
                Title = node.GetProperty("title").GetString()!,
                Url = node.GetProperty("url").GetString()!,
                Number = node.GetProperty("number").GetInt32(),
                CreatedAt = node.GetProperty("createdAt").GetDateTime(),
                Closed = node.GetProperty("closed").GetBoolean(),
                Assignees = [.. node.GetProperty("assignees").GetProperty("nodes")
                    .EnumerateArray()
                    .Select(node => new User
                    {
                        AvatarUrl = node.GetProperty("avatarUrl").GetString()!,
                        Url = node.GetProperty("url").GetString()!,
                        Name = node.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty
                    })],
                Repository = new Repository
                {
                    Name = node.GetProperty("repository").GetProperty("name").GetString()!,
                    Url = node.GetProperty("repository").GetProperty("url").GetString()!,
                    Owner = new User
                    {
                        AvatarUrl = node.GetProperty("repository").GetProperty("owner").GetProperty("avatarUrl").GetString()!,
                        Login = node.GetProperty("repository").GetProperty("owner").GetProperty("login").GetString()!,
                        Url = node.GetProperty("repository").GetProperty("owner").GetProperty("url").GetString()!
                    }
                }
            })];

        return new IssueStats
        {
            Data = nodes,
            Open = nodes.Count(x => !x.Closed),
            Closed = nodes.Count(x => x.Closed),
            TotalCount = nodes.Length
        };
    }

    public async Task<OrgData> GetOrganizationsAsync()
    {
        var document = await PostGraphQLAsync($@"
            query {{
                user(login: ""{_username}"") {{
                    repositoriesContributedTo(last: 100) {{
                        totalCount
                        nodes {{
                            owner {{
                              login
                              avatarUrl
                              url
                              __typename
                            }}
                        }}
                    }}
                }}
            }}");

        Organization[] nodes = [.. document.RootElement
            .GetProperty("data").GetProperty("user").GetProperty("repositoriesContributedTo").GetProperty("nodes")
            .EnumerateArray()
            .Select(node => node.GetProperty("owner"))
            .Where(node => node.GetProperty("__typename").GetString() == "Organization")
            .Select(node => new Organization
            {
                Login = node.GetProperty("login").GetString()!,
                AvatarUrl = node.GetProperty("avatarUrl").GetString()!,
                Url = node.GetProperty("url").GetString()!
            })];

        return new OrgData { Data = nodes };
    }

    public async Task<ProjectData> GetPinnedProjectsAsync()
    {
        var document = await PostGraphQLAsync($@"
            query {{
                user(login: ""{_username}"") {{
                    pinnedItems(first: 6, types: REPOSITORY) {{
                        nodes {{
                            ... on Repository {{
                                id
                                name
                                createdAt
                                url
                                description
                                isFork
                                languages(first: 10) {{
                                    nodes {{
                                        name
                                    }}
                                }}
                            }}
                        }}
                    }}
                }}
            }}");

        Project[] nodes = [.. document.RootElement
            .GetProperty("data").GetProperty("user").GetProperty("pinnedItems").GetProperty("nodes")
            .EnumerateArray()
            .Select(node => new Project
            {
                Id = node.GetProperty("id").GetString()!,
                Name = node.GetProperty("name").GetString()!,
                CreatedAt = node.GetProperty("createdAt").GetString()!,
                Url = node.GetProperty("url").GetString()!,
                Description = node.GetProperty("description").GetString()!,
                IsFork = node.GetProperty("isFork").GetBoolean(),
                Languages = [.. node.GetProperty("languages").GetProperty("nodes")
                    .EnumerateArray()
                    .Select(node => new Language
                    {
                        Name = node.GetProperty("name").GetString()!,
                        IconifyClass = LanguageIconMapper.GetIconifyClass(node.GetProperty("name").GetString()!)
                    })]
            })];

        return new ProjectData { Data = nodes };
    }
}
