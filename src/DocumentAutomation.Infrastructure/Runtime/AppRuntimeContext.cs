using DocumentAutomation.Application.Contracts;

namespace DocumentAutomation.Infrastructure.Runtime;

public sealed class AppRuntimeContext(
    bool isDemoMode,
    string? connectionDescription,
    string templateRootPath,
    string outputRootPath) : IAppRuntimeContext
{
    public bool IsDemoMode { get; } = isDemoMode;
    public string? ConnectionDescription { get; } = connectionDescription;
    public string TemplateRootPath { get; } = templateRootPath;
    public string OutputRootPath { get; } = outputRootPath;
}
