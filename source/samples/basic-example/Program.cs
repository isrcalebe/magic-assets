
using System;
using System.Text;
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

testServer(assetServer, "Global Asset Server");
testServer(assemblyServer, "Assembly Asset Server");
testServer(namespacedServer, "Namespaced Asset Server");
testServer(stringServer, "Custom String Server");

return;

static void testServer<T>(IAssetServer<T> server, string name)
{
    {
        var displayName = $"# {name} #";
        var displayNameLength = displayName.Length;
        var padding = new string('#', displayNameLength);

        Console.WriteLine(padding);
        Console.WriteLine(displayName);
        Console.WriteLine(padding);
        Console.WriteLine();
    }

    Console.WriteLine($"- GetAvailableAssets():");

    foreach (var asset in server.GetAvailableAssets())
        Console.WriteLine($"    {asset}");

    foreach (var asset in server.GetAvailableAssets())
    {
        Console.WriteLine($"- Fetch(\"{asset}\")");

        var sampleText = server.Fetch(asset);

        if (sampleText is byte[] byteArray)
        {
            var text = Encoding.UTF8.GetString(byteArray).ReplaceLineEndings("");

            Console.WriteLine($"    <{sampleText.GetType()}> -> {text}");
        }
        else
            Console.WriteLine($"    <{sampleText?.GetType()}> -> {(sampleText != null ? sampleText : "(no custom decoding)")})");
    }

    Console.WriteLine();
}
