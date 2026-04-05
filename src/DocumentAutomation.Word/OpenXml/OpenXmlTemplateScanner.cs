using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Word.Abstractions;
using DocumentAutomation.Word.Parsing;

namespace DocumentAutomation.Word.OpenXml;

/// <summary>
/// Adapts the plain-text scanner to real .docx files.
/// OpenXML is only used to read the document text; placeholder interpretation stays in the simpler parser layer.
/// </summary>
public sealed class OpenXmlTemplateScanner : ITemplateScanner, ITemplateFieldExtractor
{
    private readonly ITemplateTextReader _templateTextReader;
    private readonly TextTemplateScanner _textTemplateScanner;

    public OpenXmlTemplateScanner()
        : this(new OpenXmlTemplateTextReader(), new TextTemplateScanner())
    {
    }

    public OpenXmlTemplateScanner(ITemplateTextReader templateTextReader, TextTemplateScanner textTemplateScanner)
    {
        _templateTextReader = templateTextReader;
        _textTemplateScanner = textTemplateScanner;
    }

    public async Task<TemplateScanResult> ScanAsync(string templatePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(templatePath))
        {
            return new TemplateScanResult(
                templatePath,
                Array.Empty<TemplateFieldDefinition>(),
                [$"Template '{templatePath}' does not exist."]);
        }

        var templateText = await _templateTextReader.ReadTextAsync(templatePath, cancellationToken);
        return _textTemplateScanner.Scan(templateText, templatePath);
    }

    public async Task<IReadOnlyList<TemplateFieldDefinition>> ExtractFieldsAsync(string templatePath, CancellationToken cancellationToken = default)
        => (await ScanAsync(templatePath, cancellationToken)).Fields;
}
