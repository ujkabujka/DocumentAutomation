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
            .Select((key, index) =>
            {
                var fieldType = InferFieldType(key);
                return new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    FieldKey = key,
                    DisplayName = ToDisplayName(key),
                    FieldType = fieldType,
                    IsRequired = true,
                    Order = index + 1,
                    Section = InferSection(key),
                    Category = InferCategory(key),
                    DatabaseKey = InferDatabaseKey(key),
                    SourcePriority = InferSourcePriority(key, fieldType)
                };
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

    private static string? InferDatabaseKey(string key)
        => key.ToLowerInvariant() switch
        {
            "project_name" => "project.name",
            "project_code" => "project.code",
            "lead_engineer" => "project.lead_engineer",
            "test_date" => "project.test_date",
            "system.current_user" => "system.current_user",
            "system.generated_on" => "system.generated_on",
            _ => null
        };

    private static string[] InferSourcePriority(string key, TemplateFieldType fieldType)
    {
        // These are intentionally simple teaching heuristics.
        // The scanner is not deciding the final business rules forever; it is producing a helpful first draft.
        if (key.StartsWith("system.", StringComparison.OrdinalIgnoreCase))
        {
            return ["Computed"];
        }

        if (fieldType is TemplateFieldType.Image or TemplateFieldType.Table or TemplateFieldType.File)
        {
            return ["User"];
        }

        if (key.Contains("note", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("summary", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("comment", StringComparison.OrdinalIgnoreCase))
        {
            return ["User", "Default"];
        }

        return ["Database", "User", "Default"];
    }

    private static string ToDisplayName(string key)
    {
        var normalized = key.Replace("image:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("table:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace('_', ' ');
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(normalized);
    }
}
