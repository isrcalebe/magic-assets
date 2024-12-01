using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicAssets.Core;
using MagicAssets.Providers.AssemblyAssetServer;
using Moq;
using Xunit;

namespace MagicAssets.Tests;

public class AssemblyAssetServerTest
{
    [Fact]
    public async Task FetchAsync_SuccessfulAsyncLookup()
    {
        var assetServer = new AssemblyAssetServer(typeof(AssemblyAssetServerTest).Assembly);
        var bytes = await assetServer.FetchAsync("test://Assets/sample-text.txt", TestContext.Current.CancellationToken);

        Assert.NotNull(bytes);
    }

    [Fact]
    public async Task FetchAsync_FailedAsyncLookup()
    {
        var assetServer = new AssemblyAssetServer(typeof(AssemblyAssetServerTest).Assembly);
        var bytes = await assetServer.FetchAsync("test://Assets/sample-text.json", TestContext.Current.CancellationToken);

        Assert.Null(bytes);
    }
}
