using DocumentAutomation.Application.Models;

namespace DocumentAutomation.Word.Parsing;

/// <summary>
/// Scans plain text content for placeholders.
/// This is the easiest class to read first because it has no OpenXML details.
/// </summary>
public sealed class TextTemplateScanner
{
    private readonly PlaceholderParser _placeholderParser;
    private readonly TemplateFieldDefinitionFactory _fieldFactory;

    public TextTemplateScanner()
        : this(new PlaceholderParser(), new TemplateFieldDefinitionFactory())
    {
    }

    public TextTemplateScanner(PlaceholderParser placeholderParser, TemplateFieldDefinitionFactory fieldFactory)
    {
        _placeholderParser = placeholderParser;
        _fieldFactory = fieldFactory;
    }

    public TemplateScanResult Scan(string templateContent, string templatePath = "inline-template")
    {
        var warnings = new List<string>();
        var tokens = _placeholderParser.Parse(templateContent);
        var fields = _fieldFactory.CreateFields(tokens);

        if (fields.Count == 0)
        {
            warnings.Add("No placeholders were discovered in the template.");
        }

        return new TemplateScanResult(templatePath, fields, warnings);
    }
}
