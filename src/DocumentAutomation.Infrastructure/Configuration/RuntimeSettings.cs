namespace DocumentAutomation.Infrastructure.Configuration;

public sealed record DocumentAutomationConnectionResolution(
    string? ConnectionString,
    string? Description,
    bool CanConnect);

public sealed record DocumentAutomationStorageSettings(
    string TemplateRootPath,
    string OutputRootPath);
