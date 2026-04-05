using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;
using Microsoft.Extensions.Configuration;

namespace DocumentAutomation.Infrastructure.Demo;

public sealed class DemoCurrentUserContext(
    DemoDataStore dataStore,
    IAppRuntimeContext runtimeContext,
    IConfiguration configuration) : ICurrentUserContext
{
    private readonly DemoDataStore _dataStore = dataStore;
    private readonly IAppRuntimeContext _runtimeContext = runtimeContext;
    private readonly IConfiguration _configuration = configuration;

    public Task<CurrentUserSession> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var requestedUser = _configuration["Identity:OverrideUserName"]
            ?? Environment.UserName
            ?? "engineer";

        var user = _dataStore.Users.FirstOrDefault(candidate => string.Equals(candidate.UserName, requestedUser, StringComparison.OrdinalIgnoreCase))
            ?? _dataStore.Users.First(candidate => string.Equals(candidate.UserName, "engineer", StringComparison.OrdinalIgnoreCase));

        var roleIds = _dataStore.UserRoles.Where(item => item.UserId == user.Id).Select(item => item.RoleId).ToHashSet();
        var roles = _dataStore.Roles.Where(role => roleIds.Contains(role.Id)).Select(role => role.Name).ToList();
        var permissionIds = _dataStore.RolePermissions.Where(item => roleIds.Contains(item.RoleId)).Select(item => item.PermissionId).ToHashSet();
        var permissions = _dataStore.Permissions.Where(permission => permissionIds.Contains(permission.Id)).Select(permission => permission.Name).ToList();

        return Task.FromResult(new CurrentUserSession(user.Id, user.UserName, user.DisplayName, roles, permissions, _runtimeContext.IsDemoMode));
    }
}

public sealed class DemoSecurityAdministrationService(DemoDataStore dataStore) : ISecurityAdministrationService
{
    private readonly DemoDataStore _dataStore = dataStore;

    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(_dataStore.Users);

    public Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Role>>(_dataStore.Roles);

    public Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        if (_dataStore.UserRoles.All(item => item.UserId != userId || item.RoleId != roleId))
        {
            _dataStore.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }

        return Task.CompletedTask;
    }
}

public sealed class DemoProjectDataService(DemoDataStore dataStore) : IProjectDataService
{
    private readonly DemoDataStore _dataStore = dataStore;

    public Task<IReadOnlyList<ProjectRecord>> GetProjectsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProjectRecord>>(_dataStore.Projects);

    public Task<ProjectRecord?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        => Task.FromResult(_dataStore.Projects.FirstOrDefault(project => project.Id == projectId));

    public Task<IReadOnlyDictionary<string, object?>> GetAutofillValuesAsync(ProjectRecord project, CancellationToken cancellationToken = default)
    {
        var values = BuildProjectValues(project);
        return Task.FromResult<IReadOnlyDictionary<string, object?>>(values);
    }

    public Task SaveFieldValuesAsync(ProjectRecord project, IEnumerable<DocumentFieldValue> values, CancellationToken cancellationToken = default)
    {
        foreach (var value in values.Where(item => item.CanPersistBack && item.Value is not null))
        {
            project.Metadata[value.FieldKey] = value.Value.ToString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }

    internal static Dictionary<string, object?> BuildProjectValues(ProjectRecord project)
    {
        var values = project.Metadata.ToDictionary(item => item.Key, item => (object?)item.Value, StringComparer.OrdinalIgnoreCase);
        values["project.name"] = project.ProjectName;
        values["project.code"] = project.ProjectCode;
        values["project.customer"] = project.CustomerName;
        values["project.lead_engineer"] = project.LeadEngineer;
        values["project.test_date"] = project.TestDate;
        return values;
    }
}

public sealed class DemoTemplateCatalogService(DemoDataStore dataStore) : ITemplateCatalogService
{
    private readonly DemoDataStore _dataStore = dataStore;

    public Task<IReadOnlyList<TemplateDefinition>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = _dataStore.Templates
            .Select(template =>
            {
                template.Fields = _dataStore.TemplateFields
                    .Where(field => field.TemplateDefinitionId == template.Id)
                    .OrderBy(field => field.Order)
                    .ToList();
                return template;
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<TemplateDefinition>>(templates);
    }

    public async Task<TemplateDefinition?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
        => (await GetTemplatesAsync(cancellationToken)).FirstOrDefault(template => template.Id == templateId);

    public Task SaveTemplateAsync(TemplateDefinition template, CancellationToken cancellationToken = default)
    {
        var existing = _dataStore.Templates.FirstOrDefault(item => item.Id == template.Id);
        if (existing is null)
        {
            _dataStore.Templates.Add(template);
            return Task.CompletedTask;
        }

        existing.Name = template.Name;
        existing.DocumentType = template.DocumentType;
        existing.Description = template.Description;
        existing.RelativePath = template.RelativePath;
        existing.IsActive = template.IsActive;
        existing.Fields = template.Fields;
        return Task.CompletedTask;
    }
}
