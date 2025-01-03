using System;
using MagicAssets.Core;
using MagicAssets.Providers.AssemblyAssetServer;
using MagicAssets.Providers.NamespacedAssetServer;
using MagicAssets.Samples.BasicExample;
using MagicAssets.Samples.BasicExample.Servers;

var assetServer = new AssetServer<byte[]>();
var assemblyServer = new AssemblyAssetServer(ExampleAssetProvider.Assembly);

assetServer.AddServer(assemblyServer);

var namespacedServer = new NamespacedAssetServer<byte[]>(assetServer, "Texts");

var stringServer = new StringServer(assetServer);

// testServer(assetServer, "Global Asset Server");
// testServer(assemblyServer, "Assembly Asset Server");
// testServer(namespacedServer, "Namespaced Asset Server");
// testServer(stringServer, "Custom String Server");


Console.WriteLine(stringServer.Fetch("resx://Texts/SampleText"));

return;

static void testServer<T>(IAssetServer<T> server, string name)
{
    {
        var displayName = $"# {name} #";
        var displayNameLength = displayName.Length;
        var titleLine = new string('#', displayNameLength);

        Console.WriteLine(titleLine);
        Console.WriteLine(displayName);
        Console.WriteLine(titleLine);
        Console.WriteLine();
    }

    Console.WriteLine("- GetAvailableAssets():");

    foreach (var asset in server.GetAvailableAssets())
        Console.WriteLine($"    {asset}");

    foreach (var asset in server.GetAvailableAssets())
    {
        Console.WriteLine($"- Fetch(\"{asset}\")");

        var sampleText = server.Fetch(asset);
        var type = sampleText?.GetType();
        var hasCustomDecoding = sampleText?.ToString() != type?.ToString();

        Console.WriteLine(
            $"    <{type}> -> {(sampleText != null ? hasCustomDecoding ? sampleText : $"{type} (no custom decoding)" : "(null)")}"
        );
    }

    Console.WriteLine();
}
