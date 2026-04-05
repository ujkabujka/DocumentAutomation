using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Persistence.Seed;

namespace DocumentAutomation.Infrastructure.Demo;

public sealed class DemoDataStore
{
    public List<User> Users { get; } = DocumentAutomationSeedData.Users.Select(Clone).ToList();
    public List<Role> Roles { get; } = DocumentAutomationSeedData.Roles.Select(role => new Role
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description
    }).ToList();
    public List<Permission> Permissions { get; } = DocumentAutomationSeedData.Permissions.Select(permission => new Permission
    {
        Id = permission.Id,
        Name = permission.Name,
        Description = permission.Description
    }).ToList();
    public List<UserRole> UserRoles { get; } = DocumentAutomationSeedData.UserRoles.Select(userRole => new UserRole
    {
        UserId = userRole.UserId,
        RoleId = userRole.RoleId
    }).ToList();
    public List<RolePermission> RolePermissions { get; } = DocumentAutomationSeedData.RolePermissions.Select(rolePermission => new RolePermission
    {
        RoleId = rolePermission.RoleId,
        PermissionId = rolePermission.PermissionId
    }).ToList();
    public List<TemplateDefinition> Templates { get; } = [Clone(DocumentAutomationSeedData.Template)];
    public List<TemplateFieldDefinition> TemplateFields { get; } = DocumentAutomationSeedData.TemplateFields.Select(Clone).ToList();
    public List<ProjectRecord> Projects { get; } = [Clone(DocumentAutomationSeedData.Project)];
    public List<GeneratedDocumentRecord> GeneratedDocuments { get; } = [];

    private static User Clone(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        DisplayName = user.DisplayName,
        IsActive = user.IsActive
    };

    private static TemplateDefinition Clone(TemplateDefinition template) => new()
    {
        Id = template.Id,
        Name = template.Name,
        DocumentType = template.DocumentType,
        Description = template.Description,
        RelativePath = template.RelativePath,
        IsActive = template.IsActive,
        CreatedAtUtc = template.CreatedAtUtc
    };

    private static TemplateFieldDefinition Clone(TemplateFieldDefinition field) => new()
    {
        Id = field.Id,
        TemplateDefinitionId = field.TemplateDefinitionId,
        FieldKey = field.FieldKey,
        DisplayName = field.DisplayName,
        Description = field.Description,
        Section = field.Section,
        Category = field.Category,
        FieldType = field.FieldType,
        IsRequired = field.IsRequired,
        Order = field.Order,
        DefaultValue = field.DefaultValue,
        PersistenceKey = field.PersistenceKey,
        DatabaseKey = field.DatabaseKey,
        EditorHint = field.EditorHint,
        AllowSaveBackToProject = field.AllowSaveBackToProject,
        SourcePriority = field.SourcePriority.ToArray(),
        VisibleRoles = field.VisibleRoles.ToArray(),
        VisiblePermissions = field.VisiblePermissions.ToArray(),
        EditableRoles = field.EditableRoles.ToArray(),
        EditablePermissions = field.EditablePermissions.ToArray(),
        ValidationHints = new Dictionary<string, string>(field.ValidationHints, StringComparer.OrdinalIgnoreCase),
        AllowedValues = field.AllowedValues.ToArray()
    };

    private static ProjectRecord Clone(ProjectRecord project) => new()
    {
        Id = project.Id,
        ProjectCode = project.ProjectCode,
        ProjectName = project.ProjectName,
        CustomerName = project.CustomerName,
        LeadEngineer = project.LeadEngineer,
        TestDate = project.TestDate,
        Metadata = new Dictionary<string, string>(project.Metadata, StringComparer.OrdinalIgnoreCase)
    };
}
