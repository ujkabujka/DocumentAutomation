using DocumentAutomation.Word.Abstractions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentAutomation.Word.OpenXml;

/// <summary>
/// Reads visible text from a .docx file.
/// It does not interpret placeholders; it only turns OpenXML parts into plain text.
/// </summary>
public sealed class OpenXmlTemplateTextReader : ITemplateTextReader
{
    public Task<string> ReadTextAsync(string templatePath, CancellationToken cancellationToken = default)
    {
        using var document = WordprocessingDocument.Open(templatePath, false);
        var textNodes = EnumerateTextNodes(document).Select(node => node.Text);
        return Task.FromResult(string.Join(Environment.NewLine, textNodes));
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
}
