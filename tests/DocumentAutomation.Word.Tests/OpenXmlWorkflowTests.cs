using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Word.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentAutomation.Word.Tests;

public sealed class OpenXmlWorkflowTests
{
    [Fact]
    public async Task TemplateScanner_ShouldDiscoverTextImageAndTablePlaceholders()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var templatePath = Path.Combine(tempDirectory, "system-acceptance-report.docx");
            await DemoTemplateDocumentWriter.EnsureSeedTemplateAsync(templatePath);
            var scanner = new OpenXmlTemplateScanner();

            var result = await scanner.ScanAsync(templatePath);

            Assert.Empty(result.Warnings);
            Assert.Contains(result.Fields, field => field.FieldKey == "project_name" && field.FieldType == TemplateFieldType.Text);
            Assert.Contains(result.Fields, field => field.FieldKey == "image:test_setup" && field.FieldType == TemplateFieldType.Image);
            Assert.Contains(result.Fields, field => field.FieldKey == "table:test_results" && field.FieldType == TemplateFieldType.Table);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    [Fact]
    public async Task DocumentGenerator_ShouldReplaceTextPlaceholders_AndWarnForUnsupportedFieldKinds()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var templatePath = Path.Combine(tempDirectory, "template.docx");
            var outputPath = Path.Combine(tempDirectory, "output.docx");
            await DemoTemplateDocumentWriter.EnsureSeedTemplateAsync(templatePath);

            var generator = new OpenXmlDocumentGenerator();
            var request = new DocumentGenerationRequest
            {
                TemplateDefinitionId = Guid.NewGuid(),
                ProjectRecordId = Guid.NewGuid(),
                TemplatePath = templatePath,
                OutputPath = outputPath,
                Fields =
                [
                    new DocumentFieldValue { FieldKey = "project_name", DisplayName = "Project Name", FieldType = TemplateFieldType.Text, Value = "Relay Upgrade" },
                    new DocumentFieldValue { FieldKey = "project_code", DisplayName = "Project Code", FieldType = TemplateFieldType.Text, Value = "PRJ-42" },
                    new DocumentFieldValue { FieldKey = "test_date", DisplayName = "Test Date", FieldType = TemplateFieldType.Date, Value = new DateTime(2026, 4, 8) },
                    new DocumentFieldValue { FieldKey = "image:test_setup", DisplayName = "Setup Image", FieldType = TemplateFieldType.Image, Value = "image.png" },
                    new DocumentFieldValue { FieldKey = "table:test_results", DisplayName = "Results Table", FieldType = TemplateFieldType.Table, Value = "table payload" }
                ]
            };

            var result = await generator.GenerateAsync(request);

            Assert.True(result.Succeeded);
            Assert.Equal(outputPath, result.OutputPath);
            Assert.Contains(result.Warnings, warning => warning.Contains("Setup Image", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result.Warnings, warning => warning.Contains("Results Table", StringComparison.OrdinalIgnoreCase));

            using var document = WordprocessingDocument.Open(outputPath, false);
            var text = string.Join(Environment.NewLine, document.MainDocumentPart!.Document.Descendants<Text>().Select(node => node.Text));
            Assert.Contains("Relay Upgrade", text);
            Assert.Contains("PRJ-42", text);
            Assert.DoesNotContain("$project_name$", text, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "DocumentAutomation.Word.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
