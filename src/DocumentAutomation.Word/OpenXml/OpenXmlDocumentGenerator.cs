using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentAutomation.Word.OpenXml;

public sealed class OpenXmlDocumentGenerator : IDocumentGenerator
{
    public Task<DocumentGenerationResult> GenerateAsync(DocumentGenerationRequest request, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(request.OutputPath)!);
        File.Copy(request.TemplatePath, request.OutputPath, overwrite: true);

        var warnings = new List<string>();
        var replacements = request.Fields
            .Where(field => field.FieldType is not TemplateFieldType.Image and not TemplateFieldType.Table)
            .ToDictionary(field => $"${field.FieldKey}$", field => ConvertToText(field.Value), StringComparer.OrdinalIgnoreCase);

        // Unsupported field kinds are reported explicitly instead of silently ignored.
        // That keeps the current generator honest while image/table support is still future work.
        warnings.AddRange(request.Fields
            .Where(field => field.FieldType is TemplateFieldType.Image or TemplateFieldType.Table)
            .Select(field => $"{field.DisplayName} ({field.FieldKey}) was recognized but {field.FieldType} generation is not implemented yet."));

        using var document = WordprocessingDocument.Open(request.OutputPath, true);
        foreach (var text in EnumerateTextNodes(document))
        {
            var updated = text.Text;
            foreach (var replacement in replacements)
            {
                updated = updated.Replace(replacement.Key, replacement.Value, StringComparison.OrdinalIgnoreCase);
            }

            text.Text = updated;
        }

        document.MainDocumentPart?.Document.Save();

        return Task.FromResult(new DocumentGenerationResult
        {
            Succeeded = true,
            OutputPath = request.OutputPath,
            Warnings = warnings,
            FinalFieldValues = request.Fields
        });
    }

    private static IEnumerable<Text> EnumerateTextNodes(WordprocessingDocument document)
    {
        if (document.MainDocumentPart?.Document is not null)
        {
            foreach (var text in document.MainDocumentPart.Document.Descendants<Text>())
            {
                yield return text;
            }
        }

        foreach (var headerPart in document.MainDocumentPart?.HeaderParts ?? Enumerable.Empty<HeaderPart>())
        {
            foreach (var text in headerPart.RootElement?.Descendants<Text>() ?? Enumerable.Empty<Text>())
            {
                yield return text;
            }
        }

        foreach (var footerPart in document.MainDocumentPart?.FooterParts ?? Enumerable.Empty<FooterPart>())
        {
            foreach (var text in footerPart.RootElement?.Descendants<Text>() ?? Enumerable.Empty<Text>())
            {
                yield return text;
            }
        }
    }

    private static string ConvertToText(object? value)
        => value switch
        {
            null => string.Empty,
            DateTime date => date.ToString("yyyy-MM-dd"),
            BaseFramework.Core.Notes.NoteDocument note => note.Text,
            _ => value.ToString() ?? string.Empty
        };
}
