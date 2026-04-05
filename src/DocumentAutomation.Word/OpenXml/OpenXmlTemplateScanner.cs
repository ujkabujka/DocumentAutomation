using System.Text.RegularExpressions;
using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Templates;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentAutomation.Word.OpenXml;

public sealed class OpenXmlTemplateScanner : ITemplateScanner, ITemplateFieldExtractor
{
    // V1 intentionally starts with a simple placeholder convention that is easy to teach and easy to scan.
    private static readonly Regex PlaceholderRegex = new(@"\$(?<field>[A-Za-z0-9_:\.-]+)\$", RegexOptions.Compiled);

    public async Task<TemplateScanResult> ScanAsync(string templatePath, CancellationToken cancellationToken = default)
    {
        var warnings = new List<string>();
        if (!File.Exists(templatePath))
        {
            warnings.Add($"Template '{templatePath}' does not exist.");
            return new TemplateScanResult(templatePath, Array.Empty<TemplateFieldDefinition>(), warnings);
        }

        var fields = await ExtractFieldsAsync(templatePath, cancellationToken);
        if (fields.Count == 0)
        {
            warnings.Add("No placeholders were discovered in the template.");
        }

        return new TemplateScanResult(templatePath, fields, warnings);
    }

    public Task<IReadOnlyList<TemplateFieldDefinition>> ExtractFieldsAsync(string templatePath, CancellationToken cancellationToken = default)
    {
        using var document = WordprocessingDocument.Open(templatePath, false);
        // This first pass scans text nodes directly.
        // It works well for the committed demo template, but future work will need to handle split runs more robustly.
        var discoveredKeys = document.MainDocumentPart?.Document
            .Descendants<Text>()
            .SelectMany(text => PlaceholderRegex.Matches(text.Text).Select(match => match.Groups["field"].Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            ?? [];

        IReadOnlyList<TemplateFieldDefinition> result = discoveredKeys
            .Select((key, index) => new TemplateFieldDefinition
            {
                Id = Guid.NewGuid(),
                FieldKey = key,
                DisplayName = ToDisplayName(key),
                FieldType = InferFieldType(key),
                IsRequired = true,
                Order = index + 1,
                Section = InferSection(key),
                Category = InferCategory(key),
                SourcePriority = ["Database", "Default"]
            })
            .ToList();

        return Task.FromResult(result);
    }

    private static TemplateFieldType InferFieldType(string key)
        => key.StartsWith("image:", StringComparison.OrdinalIgnoreCase)
            ? TemplateFieldType.Image
            : key.StartsWith("table:", StringComparison.OrdinalIgnoreCase)
                ? TemplateFieldType.Table
                : key.Contains("date", StringComparison.OrdinalIgnoreCase)
                    ? TemplateFieldType.Date
                    : TemplateFieldType.Text;

    private static string InferSection(string key)
        => key.StartsWith("image:", StringComparison.OrdinalIgnoreCase) || key.StartsWith("table:", StringComparison.OrdinalIgnoreCase)
            ? "Evidence"
            : "Template";

    private static string InferCategory(string key)
        => key.StartsWith("image:", StringComparison.OrdinalIgnoreCase) || key.StartsWith("table:", StringComparison.OrdinalIgnoreCase)
            ? "Placeholder"
            : "Field";

    private static string ToDisplayName(string key)
    {
        var normalized = key.Replace("image:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("table:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace('_', ' ');
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(normalized);
    }
}
