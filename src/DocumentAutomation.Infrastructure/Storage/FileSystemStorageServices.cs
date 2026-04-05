using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Infrastructure.Storage;

public sealed class FileSystemTemplateStorage(string templateRootPath) : ITemplateStorage
{
    private readonly string _templateRootPath = templateRootPath;

    public Task<IReadOnlyList<string>> GetTemplateFilesAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_templateRootPath);
        IReadOnlyList<string> files = Directory
            .GetFiles(_templateRootPath, "*.docx", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList()!;
        return Task.FromResult(files);
    }

    public string ResolveTemplatePath(string relativePath)
        => Path.Combine(_templateRootPath, relativePath);

    public Task EnsureSeedTemplatesAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_templateRootPath);
        return Task.CompletedTask;
    }
}

public sealed class FileSystemOutputStorage(string outputRootPath) : IOutputStorage
{
    private readonly string _outputRootPath = outputRootPath;

    public Task<string> CreateOutputPathAsync(ProjectRecord project, TemplateDefinition template, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_outputRootPath);

        var safeProject = Sanitize(project.ProjectCode);
        var safeTemplate = Sanitize(template.Name);
        var fileName = $"{safeProject}_{safeTemplate}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
        return Task.FromResult(Path.Combine(_outputRootPath, fileName));
    }

    private static string Sanitize(string value)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        return value.Replace(' ', '_');
    }
}
