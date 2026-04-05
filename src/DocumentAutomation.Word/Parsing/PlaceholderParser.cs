using System.Text.RegularExpressions;
using DocumentAutomation.Word.Models;

namespace DocumentAutomation.Word.Parsing;

/// <summary>
/// Parses the first placeholder convention used by this repository:
/// "$project_name$", "$image:test_setup$", "$table:test_results$".
/// The parser only recognizes balanced "$...$" tokens and ignores malformed fragments.
/// </summary>
public sealed class PlaceholderParser
{
    private static readonly Regex PlaceholderRegex = new(@"\$(?<field>[A-Za-z0-9_:\.-]+)\$", RegexOptions.Compiled);

    public IReadOnlyList<PlaceholderToken> Parse(string templateContent)
    {
        if (string.IsNullOrWhiteSpace(templateContent))
        {
            return Array.Empty<PlaceholderToken>();
        }

        return PlaceholderRegex.Matches(templateContent)
            .Select(match => new PlaceholderToken(
                match.Value,
                match.Groups["field"].Value,
                match.Index,
                match.Length))
            .ToList();
    }
}
