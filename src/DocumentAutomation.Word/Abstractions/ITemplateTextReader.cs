namespace DocumentAutomation.Word.Abstractions;

/// <summary>
/// Reads raw text from a template file.
/// The scanner can then focus on placeholder parsing instead of file-format details.
/// </summary>
public interface ITemplateTextReader
{
    Task<string> ReadTextAsync(string templatePath, CancellationToken cancellationToken = default);
}
