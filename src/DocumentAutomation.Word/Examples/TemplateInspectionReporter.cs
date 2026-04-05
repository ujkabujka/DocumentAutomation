using DocumentAutomation.Application.Models;

namespace DocumentAutomation.Word.Examples;

/// <summary>
/// Formats a scan result into a simple text report.
/// The output is meant for docs, tests, and quick demos.
/// </summary>
public static class TemplateInspectionReporter
{
    public static string BuildReport(TemplateScanResult scanResult)
    {
        var lines = new List<string>
        {
            $"Template: {scanResult.TemplatePath}",
            $"Fields: {scanResult.Fields.Count}"
        };

        foreach (var field in scanResult.Fields)
        {
            var source = field.SourcePriority.Length == 0
                ? "Unknown"
                : string.Join(" -> ", field.SourcePriority);

            lines.Add($"- Key: {field.FieldKey}");
            lines.Add($"  Type: {field.FieldType}");
            lines.Add($"  Label: {field.DisplayName}");
            lines.Add($"  Likely Source: {source}");
            lines.Add($"  Required: {field.IsRequired}");
        }

        if (scanResult.Warnings.Count > 0)
        {
            lines.Add("Warnings:");
            lines.AddRange(scanResult.Warnings.Select(warning => $"- {warning}"));
        }

        return string.Join(Environment.NewLine, lines);
    }
}
