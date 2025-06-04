using GitHubDataFetcher.Abstractions;
using GitHubDataFetcher.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GitHubDataFetcher;

public class Program
{
    public static async Task Main(string[] _)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IGitHubApi, GitHubApi>();
        services.AddSingleton<IGitHubJsonWriter, GitHubJsonWriter>();
        services.AddHttpClient();

        var provider = services.BuildServiceProvider();
        var writer = provider.GetRequiredService<IGitHubJsonWriter>();

        await writer.GeneratePullRequestsAsync();
        await writer.GenerateIssuesAsync();
        await writer.GenerateOrganizationsAsync();
        await writer.GeneratePinnedProjectsAsync();
    }
}
