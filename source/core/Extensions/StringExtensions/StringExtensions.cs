using System;
using System.Text.RegularExpressions;

namespace MagicAssets.Core.Extensions.StringExtensions;

public static partial class StringExtensions
{
    internal static bool IsKebabCase(this string self)
    {
        if (string.IsNullOrEmpty(self))
            return false;

        var kebabCaseRegex = getKebabCaseRegex();

        return kebabCaseRegex.IsMatch(self);
    }

    /// <summary>
    /// Parses the URI into the server name and path.
    /// </summary>
    /// <param name="self">The URI to parse.</param>
    /// <returns>The server name and path.</returns>
    /// <exception cref="FormatException">Can be thrown if the URI is invalid, or if the server name is not in kebab-case.</exception>
    public static (string serverName, string path) ParseUri(this string self)
    {
        var match = GetUriRegex().Match(self);

        if (!match.Success)
        {
            if (!self.Contains("://"))
                throw new FormatException($"The URI '{self}' is invalid. Missing the '://' separator. A valid URI should follow the format '<server-name>://<path>'.");

            var parts = self.Split(["://"], StringSplitOptions.None);

            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new FormatException(
                    $"The URI '{self}' is invalid. Ensure it follows the format '<server-name>://<path>' with a valid kebab-case server name."
                );
            }

            var server = parts[0];
            if (!server.IsKebabCase())
            {
                throw new FormatException(
                    $"The server name '{server}' is invalid. Server names must be in kebab-case, " +
                    "containing only lowercase letters and hyphens, and must not start or end with a hyphen."
                );
            }

            throw new FormatException(
                $"The URI '{self}' is invalid. Ensure it follows the format '<server-name>://<path>' with a valid kebab-case server name."
            );
        }

        var serverName = match.Groups["ServerName"].Value;
        var path = match.Groups["Path"].Value;

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new FormatException(
                $"The URI '{self}' is invalid. The path part after '://' cannot be empty or whitespace."
            );
        }

        return (serverName, path);
    }

    /// <summary>
    /// Gets the regular expression for parsing URIs.
    /// </summary>
    /// <returns>The regular expression for parsing URIs.</returns>
    [GeneratedRegex(@"^(?<ServerName>[a-z]+(-[a-z]+)*)://(?<Path>.+)$")]
    public static partial Regex GetUriRegex();

    [GeneratedRegex(@"^[a-z]+(-[a-z]+)*$")]
    private static partial Regex getKebabCaseRegex();
}
