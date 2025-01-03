using System.Text.Json.Serialization;

namespace MagicAssets.Providers.AssemblyAssetServer;

internal record AssemblyAssetInfo
{
    [JsonPropertyName("ServerName")]
    public required string ServerName { get; set; }
}
