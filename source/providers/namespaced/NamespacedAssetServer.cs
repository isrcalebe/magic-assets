using System;
using System.Collections.Generic;
using System.Linq;
using MagicAssets.Core;
using MagicAssets.Core.Extensions.StringExtensions;

namespace MagicAssets.Providers.NamespacedAssetServer;

/// <summary>
/// An asset server that provides namespaced assets.
/// </summary>
/// <typeparam name="T">The type of asset to provide.</typeparam>
public partial class NamespacedAssetServer<T> : AssetServer<T>
    where T : class
{
    /// <summary>
    /// The namespace of the asset server.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamespacedAssetServer{T}"/> class.
    /// </summary>
    /// <param name="underlyingServer">The underlying server to use.</param>
    /// <param name="namespace">The namespace of the asset server.</param>
    /// <exception cref="ArgumentException">Thrown if the namespace is null or empty.</exception>
    public NamespacedAssetServer(IAssetServer<T>? underlyingServer, string @namespace)
        : base(underlyingServer)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException("Namespace cannot be null or empty.", nameof(@namespace));

        Namespace = @namespace;
    }

    protected override IEnumerable<string> GetFilenames(string name)
        => base.GetFilenames($"{Namespace}/{name}");

    public override IEnumerable<string> GetAvailableAssets()
    {
        var assets = base.GetAvailableAssets()
            .Where(asset =>
            {
                var (serverName, path) = asset.ParseUri();

                return path.StartsWith($"{Namespace}/", StringComparison.Ordinal);
            })
            .Select(asset =>
            {
                var (serverName, path) = asset.ParseUri();

                return $"{serverName}://{path[(Namespace.Length + 1)..]}";
            });

        return assets;
    }
}
