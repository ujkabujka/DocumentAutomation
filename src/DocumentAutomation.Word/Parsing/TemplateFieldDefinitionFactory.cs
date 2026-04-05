using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Word.Models;

namespace DocumentAutomation.Word.Parsing;

/// <summary>
/// Converts raw placeholder tokens into the shared template field model used by the app.
/// The rules are intentionally simple heuristics so a learner can follow every decision.
/// </summary>
public sealed class TemplateFieldDefinitionFactory
{
    public IReadOnlyList<TemplateFieldDefinition> CreateFields(IEnumerable<PlaceholderToken> tokens)
    {
        var uniqueTokens = new List<PlaceholderToken>();
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var token in tokens)
        {
            // Templates often repeat the same placeholder more than once.
            // The field list should still contain one definition per business key.
            if (seenKeys.Add(token.Key))
            {
                uniqueTokens.Add(token);
            }
        }

        return uniqueTokens
            .Select((token, index) => CreateField(token, index + 1))
            .ToList();
    }

    public TemplateFieldDefinition CreateField(PlaceholderToken token, int order)
    {
        var fieldType = InferFieldType(token.Key);

        return new TemplateFieldDefinition
        {
            Id = Guid.NewGuid(),
            FieldKey = token.Key,
            DisplayName = ToDisplayName(token.Key),
            FieldType = fieldType,
            IsRequired = true,
            Order = order,
            Section = InferSection(fieldType),
            Category = InferCategory(fieldType),
            DatabaseKey = InferDatabaseKey(token.Key),
            SourcePriority = InferSourcePriority(token.Key, fieldType)
        };
    }

    public TemplateFieldType InferFieldType(string key)
    {
        if (key.StartsWith("image:", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.Image;
        }

        if (key.StartsWith("table:", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.Table;
        }

        if (key.Contains("date", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.Date;
        }

        if (key.Contains("count", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("number", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("total", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("quantity", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.Number;
        }

        if (key.Contains("summary", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("description", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.MultiLineText;
        }

        if (key.Contains("note", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("comment", StringComparison.OrdinalIgnoreCase))
        {
            return TemplateFieldType.Note;
        }

        return TemplateFieldType.Text;
    }

    public string ToDisplayName(string key)
    {
        var normalized = key
            .Replace("image:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("table:", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace('_', ' ');

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(normalized);
    }

    public string? InferDatabaseKey(string key)
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

    public string[] InferSourcePriority(string key, TemplateFieldType fieldType)
    {
        if (key.StartsWith("system.", StringComparison.OrdinalIgnoreCase))
        {
            return ["Computed"];
        }

        if (fieldType is TemplateFieldType.Image or TemplateFieldType.Table or TemplateFieldType.File)
        {
            return ["User"];
        }

        if (fieldType is TemplateFieldType.Note or TemplateFieldType.MultiLineText)
        {
            return ["User", "Default"];
        }

        return ["Database", "User", "Default"];
    }

    private static string InferSection(TemplateFieldType fieldType)
        => fieldType is TemplateFieldType.Image or TemplateFieldType.Table
            ? "Evidence"
            : "Template";

    private static string InferCategory(TemplateFieldType fieldType)
        => fieldType is TemplateFieldType.Image or TemplateFieldType.Table
            ? "Placeholder"
            : "Field";
}
