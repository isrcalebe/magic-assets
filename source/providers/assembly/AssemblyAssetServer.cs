using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MagicAssets.Core;
using MagicAssets.Core.AssemblyInfo;
using MagicAssets.Core.Extensions.StreamExtensions;
using MagicAssets.Core.Extensions.StringExtensions;

namespace MagicAssets.Providers.AssemblyAssetServer;

/// <summary>
/// An asset server that provides assets from an assembly.
/// </summary>
public class AssemblyAssetServer : IAssetServer<byte[]>
{
    private readonly Assembly assembly;
    private readonly string prefix;
    private readonly string serverName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAssetServer"/> class.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load.</param>
    /// <exception cref="InvalidOperationException">Thrown if the assembly does not contain the required <see cref="AssetServerAttribute"/>.</exception>
    public AssemblyAssetServer(string assemblyName)
    {
        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, assemblyName);

        assembly = File.Exists(filePath)
                 ? Assembly.LoadFile(filePath)
                 : Assembly.Load(Path.GetFileNameWithoutExtension(assemblyName));

        prefix = Path.GetFileNameWithoutExtension(assemblyName);

        var attribute = assembly.GetCustomAttribute(typeof(AssetServerAttribute));

        if (attribute is AssetServerAttribute serverNameAttribute)
            serverName = serverNameAttribute.ServerName;
        else
            throw new InvalidOperationException($"The assembly \"{assembly.FullName}\" does not contain the required {typeof(AssetServerAttribute)}.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAssetServer"/> class.
    /// </summary>
    /// <param name="assembly">The assembly to load.</param>
    /// <exception cref="InvalidOperationException">Thrown if the assembly does not contain the required <see cref="AssetServerAttribute"/>.</exception>
    public AssemblyAssetServer(Assembly assembly)
    {
        this.assembly = assembly;
        prefix = assembly.GetName().Name ?? string.Empty;

        var attribute = assembly.GetCustomAttribute(typeof(AssetServerAttribute));

        if (attribute is AssetServerAttribute serverNameAttribute)
            serverName = serverNameAttribute.ServerName;
        else
            throw new InvalidOperationException($"The assembly \"{assembly.FullName}\" does not contain the required {typeof(AssetServerAttribute)}.");

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAssetServer"/> class.
    /// </summary>
    /// <param name="assembly">The name of the assembly to load.</param>
    public AssemblyAssetServer(AssemblyName assembly)
        : this(Assembly.Load(assembly))
    { }

    public virtual byte[]? Fetch(string uri)
    {
        using var input = FetchStream(uri);

        return input?.ReadAllBytesToArray();
    }

    public virtual async Task<byte[]?> FetchAsync(string uri, CancellationToken cancellationToken = default)
    {
        await using var input = FetchStream(uri);

        if (input != null)
            return await input.ReadAllBytesToArrayAsync(cancellationToken).ConfigureAwait(false);

        return null;
    }

    public Stream? FetchStream(string uri)
    {
        var (server, path) = uri.ParseUri();

        if (server != serverName)
            throw new FormatException($"The URI's server name '{server}' does not match the expected server name '{serverName}'.");

        var split = path.Split('/');

        for (var i = 0; i < split.Length - 1; i++)
            split[i] = split[i].Replace('-', '_');

        return assembly.GetManifestResourceStream($"{prefix}.{string.Join('.', split)}");
    }

    public IEnumerable<string> GetAvailableAssets()
        => assembly.GetManifestResourceNames().Select(resource =>
        {
            resource = resource[(resource.StartsWith(prefix, StringComparison.Ordinal) ? prefix.Length + 1 : 0)..];

            var lastDot = resource.LastIndexOf('.');
            var chars = resource.ToCharArray();

            for (var i = 0; i < lastDot; i++)
            {
                if (chars[i] == '.')
                    chars[i] = '/';
            }

            var assetName = new string(chars);

            return new string($"{serverName}://" + assetName);
        });

    #region IDisposable Support

    public void Dispose()
    {
    }

    #endregion
}
