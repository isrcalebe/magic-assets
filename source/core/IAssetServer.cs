using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicAssets.Core;

/// <summary>
/// An asset server that provides assets.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAssetServer<T> : IDisposable
{
    /// <summary>
    /// Fetches the asset from the server.
    /// </summary>
    /// <param name="uri">The URI of the asset to fetch.</param>
    /// <returns>The asset, or null if the asset could not be found.</returns>
    T? Fetch(string uri);

    /// <summary>
    /// Fetches the asset from the server asynchronously.
    /// </summary>
    /// <param name="uri">The URI of the asset to fetch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asset, or null if the asset could not be found.</returns>
    Task<T?> FetchAsync(string uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the <see cref="Stream"/> of the asset from the server.
    /// </summary>
    /// <param name="uri">The URI of the asset to fetch.</param>
    /// <returns>The <see cref="Stream"/> of the asset, or null if the asset could not be found.</returns>
    Stream? FetchStream(string uri);

    /// <summary>
    /// Gets a collection of string representation of available assets.
    /// </summary>
    /// <returns>A collection of string representation of available assets.</returns>
    IEnumerable<string> GetAvailableAssets();
}

/// <summary>
/// The extension methods for the <see cref="IAssetServer{T}"/> interface.
/// </summary>
public static class AssetServerExtensions
{
    private static readonly string[] system_filename_ignore_list =
    {
        // Mac-specific
        "__MACOSX",
        ".DS_Store",

        // Windows-specific
        "Thumbs.db"
    };

    /// <summary>
    /// Remove common noise files generated by the system.
    /// </summary>
    /// <param name="source">The source collection of file names.</param>
    /// <returns>The collection of file names with common noise files removed.</returns>
    public static IEnumerable<string> ExcludeSystemFileNames(this IEnumerable<string> source)
        => source.Where(entry => !system_filename_ignore_list.Any(ignored => entry.Contains(ignored, StringComparison.OrdinalIgnoreCase)));
}