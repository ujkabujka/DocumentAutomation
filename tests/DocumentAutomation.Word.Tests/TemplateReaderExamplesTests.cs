using DocumentAutomation.Word.Examples;

namespace DocumentAutomation.Word.Tests;

public sealed class TemplateReaderExamplesTests
{
    [Fact]
    public void Example01_ShouldProduceTwoTextFields()
    {
        var result = TemplateReaderExamples.ScanSimpleTextExample();

        Assert.Equal(2, result.Fields.Count);
        Assert.All(result.Fields, field => Assert.Equal("Text", field.FieldType.ToString()));
    }

    [Fact]
    public void Example02_ShouldProduceReadableInspectionReport()
    {
        var result = TemplateReaderExamples.ScanMixedPlaceholderExample();

        var report = TemplateInspectionReporter.BuildReport(result);

        Assert.Contains("Key: project_name", report);
        Assert.Contains("Type: Date", report);
        Assert.Contains("Type: Image", report);
        Assert.Contains("Type: Table", report);
        Assert.Contains("Likely Source: Database -> User -> Default", report);
        Assert.Contains("Likely Source: User", report);
        Assert.Contains("Required: True", report);
    }
}
