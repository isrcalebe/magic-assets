using System.Reflection;

namespace MagicAssets.Samples.BasicExample;

public class ExampleAssetProvider
{
    public static Assembly Assembly => typeof(ExampleAssetProvider).Assembly;
}
