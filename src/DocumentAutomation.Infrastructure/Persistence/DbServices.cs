using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Infrastructure.Demo;
using DocumentAutomation.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DocumentAutomation.Infrastructure.Persistence;

public sealed class DbCurrentUserContext(
    DocumentAutomationDbContext dbContext,
    IAppRuntimeContext runtimeContext,
    IConfiguration configuration) : ICurrentUserContext
{
    private readonly DocumentAutomationDbContext _dbContext = dbContext;
    private readonly IAppRuntimeContext _runtimeContext = runtimeContext;
    private readonly IConfiguration _configuration = configuration;

    public async Task<CurrentUserSession> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var requestedUser = _configuration["Identity:OverrideUserName"]
            ?? Environment.UserName
            ?? "engineer";

        var user = await _dbContext.Users
            .Include(item => item.UserRoles)
            .ThenInclude(item => item.Role)
            .FirstOrDefaultAsync(item => item.UserName == requestedUser, cancellationToken)
            ?? await _dbContext.Users
                .Include(item => item.UserRoles)
                .ThenInclude(item => item.Role)
                .FirstAsync(item => item.UserName == "engineer", cancellationToken);

        var roleIds = user.UserRoles.Select(item => item.RoleId).ToList();
        var permissions = await _dbContext.RolePermissions
            .Where(item => roleIds.Contains(item.RoleId))
            .Select(item => item.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return new CurrentUserSession(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.UserRoles.Select(item => item.Role.Name).Distinct().ToList(),
            permissions,
            _runtimeContext.IsDemoMode);
    }
}

public sealed class DbSecurityAdministrationService(DocumentAutomationDbContext dbContext) : ISecurityAdministrationService
{
    private readonly DocumentAutomationDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Users.Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role).OrderBy(user => user.UserName).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Roles.OrderBy(role => role.Name).ToListAsync(cancellationToken);

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.UserRoles.AnyAsync(item => item.UserId == userId && item.RoleId == roleId, cancellationToken))
        {
            _dbContext.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

public sealed class DbProjectDataService(DocumentAutomationDbContext dbContext) : IProjectDataService
{
    private readonly DocumentAutomationDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<ProjectRecord>> GetProjectsAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Projects.OrderBy(project => project.ProjectCode).ToListAsync(cancellationToken);

    public Task<ProjectRecord?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        => _dbContext.Projects.FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);

    public Task<IReadOnlyDictionary<string, object?>> GetAutofillValuesAsync(ProjectRecord project, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyDictionary<string, object?>>(DemoProjectDataService.BuildProjectValues(project));

    public async Task SaveFieldValuesAsync(ProjectRecord project, IEnumerable<DocumentFieldValue> values, CancellationToken cancellationToken = default)
    {
        var storedProject = await _dbContext.Projects.FirstAsync(item => item.Id == project.Id, cancellationToken);
        foreach (var value in values.Where(item => item.CanPersistBack && item.Value is not null))
        {
            storedProject.Metadata[value.FieldKey] = value.Value.ToString() ?? string.Empty;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DbTemplateCatalogService(DocumentAutomationDbContext dbContext) : ITemplateCatalogService
{
    private readonly DocumentAutomationDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<TemplateDefinition>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.TemplateDefinitions.Include(template => template.Fields).OrderBy(template => template.Name).ToListAsync(cancellationToken);

    public Task<TemplateDefinition?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
        => _dbContext.TemplateDefinitions.Include(template => template.Fields).FirstOrDefaultAsync(template => template.Id == templateId, cancellationToken);

    public async Task SaveTemplateAsync(TemplateDefinition template, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.TemplateDefinitions.Include(item => item.Fields).FirstOrDefaultAsync(item => item.Id == template.Id, cancellationToken);
        if (existing is null)
        {
            _dbContext.TemplateDefinitions.Add(template);
        }
        else
        {
            existing.Name = template.Name;
            existing.DocumentType = template.DocumentType;
            existing.Description = template.Description;
            existing.RelativePath = template.RelativePath;
            existing.IsActive = template.IsActive;
            existing.Fields = template.Fields;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
