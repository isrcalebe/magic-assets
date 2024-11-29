using System.Reflection;

namespace MagicAssets.Samples.BasicExample;

public static class ExampleAssetProvider
{
    public static Assembly Assembly => typeof(ExampleAssetProvider).Assembly;
}
