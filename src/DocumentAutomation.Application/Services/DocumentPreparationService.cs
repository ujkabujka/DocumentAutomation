using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Services;

public sealed class DocumentPreparationService(
    ICurrentUserContext currentUserContext,
    IAuthorizationService authorizationService,
    IProjectDataService projectDataService,
    ITemplateCatalogService templateCatalogService,
    ITemplateStorage templateStorage,
    IOutputStorage outputStorage) : IDocumentPreparationService
{
    private readonly ICurrentUserContext _currentUserContext = currentUserContext;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IProjectDataService _projectDataService = projectDataService;
    private readonly ITemplateCatalogService _templateCatalogService = templateCatalogService;
    private readonly ITemplateStorage _templateStorage = templateStorage;
    private readonly IOutputStorage _outputStorage = outputStorage;

    public async Task<DocumentPreparationResult> PrepareAsync(Guid projectId, Guid templateId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _currentUserContext.GetCurrentUserAsync(cancellationToken);
        var project = await _projectDataService.GetProjectAsync(projectId, cancellationToken)
            ?? throw new InvalidOperationException($"Project '{projectId}' was not found.");
        var template = await _templateCatalogService.GetTemplateAsync(templateId, cancellationToken)
            ?? throw new InvalidOperationException($"Template '{templateId}' was not found.");

        var autofillValues = await _projectDataService.GetAutofillValuesAsync(project, cancellationToken);
        var preparedValues = template.Fields
            .OrderBy(field => field.Order)
            .Select(field => BuildFieldValue(field, project, currentUser, autofillValues))
            .ToList();

        var form = new DynamicDocumentFormObject(
            template.Fields.Where(field => _authorizationService.CanView(field, currentUser)),
            preparedValues.Where(value => template.Fields.Any(field => string.Equals(field.FieldKey, value.FieldKey, StringComparison.OrdinalIgnoreCase))));

        return new DocumentPreparationResult(currentUser, project, template, form, preparedValues);
    }

    public async Task<DocumentGenerationRequest> CreateGenerationRequestAsync(DocumentPreparationResult preparation, CancellationToken cancellationToken = default)
    {
        var outputPath = await _outputStorage.CreateOutputPathAsync(preparation.Project, preparation.Template, cancellationToken);
        return new DocumentGenerationRequest
        {
            TemplateDefinitionId = preparation.Template.Id,
            ProjectRecordId = preparation.Project.Id,
            RequestedBy = preparation.CurrentUser.UserName,
            RequestedAtUtc = DateTime.UtcNow,
            TemplatePath = _templateStorage.ResolveTemplatePath(preparation.Template.RelativePath),
            OutputPath = outputPath,
            Fields = preparation.Form.SnapshotValues()
        };
    }

    private static DocumentFieldValue BuildFieldValue(
        TemplateFieldDefinition field,
        ProjectRecord project,
        CurrentUserSession currentUser,
        IReadOnlyDictionary<string, object?> autofillValues)
    {
        foreach (var source in ResolvePriority(field))
        {
            switch (source)
            {
                case FieldValueSource.Database:
                    if (TryResolveFromDictionary(field, autofillValues, out var databaseValue))
                    {
                        return CreateValue(field, databaseValue, FieldValueSource.Database);
                    }
                    break;
                case FieldValueSource.Computed:
                    if (TryResolveComputed(field, project, currentUser, out var computedValue))
                    {
                        return CreateValue(field, computedValue, FieldValueSource.Computed);
                    }
                    break;
                case FieldValueSource.Default:
                    if (!string.IsNullOrWhiteSpace(field.DefaultValue))
                    {
                        return CreateValue(field, field.DefaultValue, FieldValueSource.Default);
                    }
                    break;
            }
        }

        return CreateValue(field, null, FieldValueSource.Missing);
    }

    private static DocumentFieldValue CreateValue(TemplateFieldDefinition field, object? value, FieldValueSource source)
        => new()
        {
            FieldKey = field.FieldKey,
            DisplayName = field.DisplayName,
            FieldType = field.FieldType,
            Value = value,
            Source = source,
            IsMissing = field.IsRequired && (value is null || value is string text && string.IsNullOrWhiteSpace(text)),
            CanPersistBack = field.AllowSaveBackToProject
        };

    private static IEnumerable<FieldValueSource> ResolvePriority(TemplateFieldDefinition field)
    {
        if (field.SourcePriority.Length == 0)
        {
            yield return FieldValueSource.Database;
            yield return FieldValueSource.Computed;
            yield return FieldValueSource.Default;
            yield break;
        }

        foreach (var source in field.SourcePriority)
        {
            if (Enum.TryParse<FieldValueSource>(source, true, out var parsed))
            {
                yield return parsed;
            }
        }
    }

    private static bool TryResolveFromDictionary(TemplateFieldDefinition field, IReadOnlyDictionary<string, object?> values, out object? value)
    {
        foreach (var key in new[] { field.DatabaseKey, field.PersistenceKey, field.FieldKey })
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            if (values.TryGetValue(key, out value) && value is not null)
            {
                return true;
            }
        }

        value = null;
        return false;
    }

    private static bool TryResolveComputed(TemplateFieldDefinition field, ProjectRecord project, CurrentUserSession currentUser, out object? value)
    {
        var key = field.DatabaseKey ?? field.FieldKey;
        switch (key)
        {
            case "system.current_user":
                value = currentUser.DisplayName;
                return true;
            case "system.generated_on":
                value = DateTime.Now;
                return true;
            case "project.display":
                value = $"{project.ProjectCode} - {project.ProjectName}";
                return true;
            default:
                value = null;
                return false;
        }
    }
}
