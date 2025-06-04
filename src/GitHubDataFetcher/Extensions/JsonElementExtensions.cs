using System.Text.Json;

namespace GitHubDataFetcher.Extensions;

internal static class JsonElementExtensions
{
    public static bool TryGetObject(this JsonElement element, string property, out JsonElement result)
        => element.TryGetProperty(property, out result) && result.ValueKind == JsonValueKind.Object;
}
