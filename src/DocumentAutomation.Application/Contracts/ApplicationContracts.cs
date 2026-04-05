using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Contracts;

public interface ICurrentUserContext
{
    Task<CurrentUserSession> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}

public interface IAppRuntimeContext
{
    bool IsDemoMode { get; }
    string? ConnectionDescription { get; }
    string TemplateRootPath { get; }
    string OutputRootPath { get; }
}

public interface IAuthorizationService
{
    bool HasPermission(CurrentUserSession session, string permission);
    bool CanView(TemplateFieldDefinition field, CurrentUserSession session);
    bool CanEdit(TemplateFieldDefinition field, CurrentUserSession session);
}

public interface ISecurityAdministrationService
{
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}

public interface IProjectDataService
{
    Task<IReadOnlyList<ProjectRecord>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProjectRecord?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, object?>> GetAutofillValuesAsync(ProjectRecord project, CancellationToken cancellationToken = default);
    Task SaveFieldValuesAsync(ProjectRecord project, IEnumerable<DocumentFieldValue> values, CancellationToken cancellationToken = default);
}

public interface ITemplateCatalogService
{
    Task<IReadOnlyList<TemplateDefinition>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<TemplateDefinition?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task SaveTemplateAsync(TemplateDefinition template, CancellationToken cancellationToken = default);
}

public interface IDocumentPreparationService
{
    Task<DocumentPreparationResult> PrepareAsync(Guid projectId, Guid templateId, CancellationToken cancellationToken = default);
    Task<DocumentGenerationRequest> CreateGenerationRequestAsync(DocumentPreparationResult preparation, CancellationToken cancellationToken = default);
}

public interface ITemplateScanner
{
    Task<TemplateScanResult> ScanAsync(string templatePath, CancellationToken cancellationToken = default);
}

public interface ITemplateFieldExtractor
{
    Task<IReadOnlyList<TemplateFieldDefinition>> ExtractFieldsAsync(string templatePath, CancellationToken cancellationToken = default);
}

public interface IDocumentGenerator
{
    Task<DocumentGenerationResult> GenerateAsync(DocumentGenerationRequest request, CancellationToken cancellationToken = default);
}

public interface ITemplateStorage
{
    Task<IReadOnlyList<string>> GetTemplateFilesAsync(CancellationToken cancellationToken = default);
    string ResolveTemplatePath(string relativePath);
    Task EnsureSeedTemplatesAsync(CancellationToken cancellationToken = default);
}

public interface IOutputStorage
{
    Task<string> CreateOutputPathAsync(ProjectRecord project, TemplateDefinition template, CancellationToken cancellationToken = default);
}
