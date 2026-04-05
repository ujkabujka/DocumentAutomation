using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Word.Parsing;

namespace DocumentAutomation.Word.Tests;

public sealed class TextTemplateScannerTests
{
    [Fact]
    public void Scan_ShouldInferFieldTypes_FromSimpleHeuristics()
    {
        var scanner = new TextTemplateScanner();

        var result = scanner.Scan("""
Project Name: $project_name$
Test Date: $test_date$
Image: $image:test_setup$
Table: $table:test_results$
Approval Note: $approval_note$
Summary: $summary_note$
""");

        Assert.Contains(result.Fields, field => field.FieldKey == "project_name" && field.FieldType == TemplateFieldType.Text);
        Assert.Contains(result.Fields, field => field.FieldKey == "test_date" && field.FieldType == TemplateFieldType.Date);
        Assert.Contains(result.Fields, field => field.FieldKey == "image:test_setup" && field.FieldType == TemplateFieldType.Image);
        Assert.Contains(result.Fields, field => field.FieldKey == "table:test_results" && field.FieldType == TemplateFieldType.Table);
        Assert.Contains(result.Fields, field => field.FieldKey == "approval_note" && field.FieldType == TemplateFieldType.Note);
        Assert.Contains(result.Fields, field => field.FieldKey == "summary_note" && field.FieldType == TemplateFieldType.MultiLineText);
    }

    [Fact]
    public void Scan_ShouldCollapseDuplicateKeys_IntoOneFieldDefinition()
    {
        var scanner = new TextTemplateScanner();

        var result = scanner.Scan("""
Project Name: $project_name$
Again: $project_name$
One More Time: $PROJECT_NAME$
""");

        var fields = result.Fields.Where(field => field.FieldKey.Equals("project_name", StringComparison.OrdinalIgnoreCase)).ToList();

        Assert.Single(fields);
        Assert.Equal(1, fields[0].Order);
    }

    [Fact]
    public void Scan_ShouldWarnWhenNoBalancedPlaceholdersAreFound()
    {
        var scanner = new TextTemplateScanner();

        var result = scanner.Scan("This text has no valid placeholders.");

        Assert.Empty(result.Fields);
        Assert.Single(result.Warnings);
    }
}
