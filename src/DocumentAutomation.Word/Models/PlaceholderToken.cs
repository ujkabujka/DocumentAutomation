namespace DocumentAutomation.Word.Models;

/// <summary>
/// Represents one balanced placeholder match such as "$project_name$".
/// Keeping this model small makes the parser easy to test and easy to explain.
/// </summary>
public sealed record PlaceholderToken(
    string RawText,
    string Key,
    int StartIndex,
    int Length);
