using System;
using MagicAssets.Core.Extensions.StringExtensions;

namespace MagicAssets.Core.AssemblyInfo;

/// <summary>
/// Specifies the name of the asset server.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssetServerAttribute : Attribute
{
    private string serverName = string.Empty;

    /// <summary>
    /// The name of the asset server.
    /// </summary>
    public required string ServerName
    {
        get => serverName;
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (!value.IsKebabCase())
                throw new FormatException($"{nameof(serverName)} must be in kebab-case format.");

            serverName = value;
        }
    }
}
