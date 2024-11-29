using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicAssets.Core.Extensions.StringExtensions;

namespace MagicAssets.Core;

public class AssetServer<T> : IAssetServer<T>
    where T : class
{
    private readonly List<IAssetServer<T>> servers = [];
    private readonly List<string> searchExtensions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetServer{T}"/> class, with no servers.
    /// </summary>
    public AssetServer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetServer{T}"/> class, with a single server.
    /// </summary>
    /// <param name="server">The server to add.</param>
    public AssetServer(IAssetServer<T>? server = null)
    {
        if (server != null)
            AddServer(server);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetServer{T}"/> class, with multiple servers.
    /// </summary>
    /// <param name="servers">The servers to add.</param>
    public AssetServer(IAssetServer<T>[] servers)
    {
        foreach (var server in servers)
            AddServer(server);
    }

    public virtual T? Fetch(string uri)
    {
        var (serverName, path) = uri.ParseUri();

        var filenames = GetFilenames(path);

        return (from filename in filenames
                from server in getServers()
                select server.Fetch($"{serverName}://{filename}")).FirstOrDefault();
    }

    public virtual async Task<T?> FetchAsync(string uri, CancellationToken cancellationToken = default)
    {
        var (serverName, path) = uri.ParseUri();

        var filenames = GetFilenames(path);

        foreach (var filename in filenames)
        {
            foreach (var server in getServers())
            {
                var result = await server.FetchAsync($"{serverName}://{filename}", cancellationToken);

                if (result != null)
                    return result;
            }
        }

        return null;
    }

    public Stream? FetchStream(string uri)
    {
        var (serverName, path) = uri.ParseUri();

        var filenames = GetFilenames(path);

        return (from filename in filenames
                from server in getServers()
                select server.FetchStream($"{serverName}://{filename}")).FirstOrDefault();
    }

    /// <summary>
    /// Adds an extension to the list of extensions to search for.
    /// </summary>
    /// <param name="extension">The extension to add.</param>
    public void AddExtension(string extension)
    {
        extension = extension.Trim('.');

        if (!searchExtensions.Contains(extension))
            searchExtensions.Add(extension);
    }

    /// <summary>
    /// Adds a server to the list of servers to search for assets.
    /// </summary>
    /// <param name="server">The server to add.</param>
    public virtual void AddServer(IAssetServer<T> server)
    {
        lock (servers)
            servers.Add(server);
    }

    /// <summary>
    /// Removes a server from the list of servers to search for assets.
    /// </summary>
    /// <param name="server">The server to remove.</param>
    /// <returns>True if the server was removed, false otherwise.</returns>
    public virtual bool RemoveServer(IAssetServer<T> server)
    {
        lock (servers)
            return servers.Remove(server);
    }

    public virtual IEnumerable<string> GetAvailableAssets()
    {
        lock (servers)
            return servers.SelectMany(server => server.GetAvailableAssets()).ExcludeSystemFileNames();
    }

    /// <summary>
    /// Gets the filenames to search for.
    /// </summary>
    /// <param name="name">The name of the asset.</param>
    /// <returns>The filenames to search for.</returns>
    protected virtual IEnumerable<string> GetFilenames(string name)
    {
        yield return name;

        foreach (var extension in searchExtensions)
            yield return $"{name}.{extension}";
    }

    private IAssetServer<T>[] getServers()
    {
        lock (servers)
            return servers.ToArray();
    }

    #region IDisposable Support

    private bool isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;

        isDisposed = true;

        lock (servers)
            servers.ForEach(server => server.Dispose());
    }

    #endregion
}

