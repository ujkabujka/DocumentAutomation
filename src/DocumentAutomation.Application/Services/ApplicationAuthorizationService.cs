using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Services;

public sealed class ApplicationAuthorizationService : IAuthorizationService
{
    public bool HasPermission(CurrentUserSession session, string permission)
        => session.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);

    public bool CanView(TemplateFieldDefinition field, CurrentUserSession session)
        => Satisfies(field.VisibleRoles, field.VisiblePermissions, session);

    public bool CanEdit(TemplateFieldDefinition field, CurrentUserSession session)
        => Satisfies(field.EditableRoles, field.EditablePermissions, session);

    private static bool Satisfies(IReadOnlyList<string> roles, IReadOnlyList<string> permissions, CurrentUserSession session)
    {
        var rolesSatisfied = roles.Count == 0 || roles.Any(role => session.Roles.Contains(role, StringComparer.OrdinalIgnoreCase));
        var permissionsSatisfied = permissions.Count == 0 || permissions.Any(permission => session.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase));
        return rolesSatisfied && permissionsSatisfied;
    }
}
