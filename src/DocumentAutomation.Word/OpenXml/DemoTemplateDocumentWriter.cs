using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentAutomation.Word.OpenXml;

public static class DemoTemplateDocumentWriter
{
    public static Task EnsureSeedTemplateAsync(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        if (File.Exists(outputPath))
        {
            return Task.CompletedTask;
        }

        using var document = WordprocessingDocument.Create(outputPath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
        var mainPart = document.AddMainDocumentPart();
        mainPart.Document = new Document(new Body(
            Paragraph("System Acceptance Report"),
            Paragraph("Project Name: $project_name$"),
            Paragraph("Project Code: $project_code$"),
            Paragraph("Test Date: $test_date$"),
            Paragraph("Lead Engineer: $lead_engineer$"),
            Paragraph("Prepared By: $system.current_user$"),
            Paragraph("Generated On: $system.generated_on$"),
            Paragraph("Summary Note: $summary_note$"),
            Paragraph("Test Setup Image Placeholder: $image:test_setup$"),
            Paragraph("Test Results Table Placeholder: $table:test_results$"),
            Paragraph("Approval Note: $approval_note$")));
        mainPart.Document.Save();

        return Task.CompletedTask;
    }

    private static Paragraph Paragraph(string text)
        => new(new Run(new Text(text)));
}
