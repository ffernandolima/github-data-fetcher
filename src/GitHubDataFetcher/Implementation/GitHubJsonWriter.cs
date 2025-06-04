using GitHubDataFetcher.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubDataFetcher.Implementation;

public class GitHubJsonWriter : IGitHubJsonWriter
{
    private readonly ILogger _logger;
    private readonly IGitHubApi _githubApi;
    private readonly string _outputPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public GitHubJsonWriter(ILogger<GitHubJsonWriter> logger, IConfiguration configuration, IGitHubApi githubApi)
    {
        _logger = logger;
        _githubApi = githubApi;
        _outputPath = configuration["OUTPUT_PATH"]!;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    private async Task SaveJsonAsync(string fileName, object content)
    {
        var fullPath = Path.Combine(Path.GetTempPath(), _outputPath, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await File.WriteAllTextAsync(fullPath, JsonSerializer.Serialize(content, _jsonOptions));
    }

    public async Task GeneratePullRequestsAsync()
    {
        _logger.LogInformation("Fetching the Pull Request Data.");

        var prs = await _githubApi.GetPullRequestsAsync();
        await SaveJsonAsync("pullRequests.json", prs);
    }

    public async Task GenerateIssuesAsync()
    {
        _logger.LogInformation("Fetching the Issues Data.");

        var issues = await _githubApi.GetIssuesAsync();
        await SaveJsonAsync("issues.json", issues);
    }

    public async Task GenerateOrganizationsAsync()
    {
        _logger.LogInformation("Fetching the Contributed Organization Data.");

        var organizations = await _githubApi.GetOrganizationsAsync();
        await SaveJsonAsync("organizations.json", new { data = organizations.Data });
    }

    public async Task GeneratePinnedProjectsAsync()
    {
        _logger.LogInformation("Fetching the Pinned Projects Data.");

        var projects = await _githubApi.GetPinnedProjectsAsync();
        await SaveJsonAsync("projects.json", new { data = projects.Data });
    }
}
