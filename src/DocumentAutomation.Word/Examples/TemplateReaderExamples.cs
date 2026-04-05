using DocumentAutomation.Application.Models;
using DocumentAutomation.Word.Parsing;

namespace DocumentAutomation.Word.Examples;

/// <summary>
/// Small example templates that teach the scanner without forcing the reader through OpenXML first.
/// </summary>
public static class TemplateReaderExamples
{
    public const string SimpleTextTemplate = """
Project Name: $project_name$
Project Code: $project_code$
""";

    public const string MixedTemplate = """
Project Name: $project_name$
Test Date: $test_date$
Test Setup Image: $image:test_setup$
Test Results Table: $table:test_results$
""";

    public static TemplateScanResult ScanSimpleTextExample()
        => new TextTemplateScanner().Scan(SimpleTextTemplate, "example-01-simple-text");

    public static TemplateScanResult ScanMixedPlaceholderExample()
        => new TextTemplateScanner().Scan(MixedTemplate, "example-02-mixed-placeholders");
}
