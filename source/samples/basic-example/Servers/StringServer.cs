using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicAssets.Core;

namespace MagicAssets.Samples.BasicExample.Servers;

public class StringServer : AssetServer<string>
{
    private readonly AssetServer<byte[]> underlyingStore;

    public StringServer(AssetServer<byte[]> underlyingStore)
    {
        this.underlyingStore = underlyingStore;
    }

    public override string? Fetch(string uri)
    {
        var result = underlyingStore.Fetch(uri);

        return result != null ? Encoding.UTF8.GetString(result) : null;
    }

    public override async Task<string?> FetchAsync(string uri, CancellationToken cancellationToken = default)
    {
        var result = await underlyingStore.FetchAsync(uri, cancellationToken);

        return result != null ? Encoding.UTF8.GetString(result) : null;
    }

    public override IEnumerable<string> GetAvailableAssets()
        => underlyingStore.GetAvailableAssets();
}
