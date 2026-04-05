namespace DocumentAutomation.Domain.Security;

public sealed class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public sealed class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public sealed class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public sealed class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Designer = "Designer";
    public const string SystemEngineer = "SystemEngineer";
}

public static class PermissionNames
{
    public const string UsersView = "users.view";
    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";
    public const string TemplatesView = "templates.view";
    public const string TemplatesManage = "templates.manage";
    public const string DocumentsGenerate = "documents.generate";
    public const string DocumentsApprove = "documents.approve";
    public const string ProjectsView = "projects.view";
    public const string ProjectsEdit = "projects.edit";
    public const string FieldsEditRestricted = "fields.edit.restricted";
}
