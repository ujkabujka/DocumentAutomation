using BaseFramework.Core.Access;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Models;

public sealed record CurrentUserSession(
    Guid? UserId,
    string UserName,
    string DisplayName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    bool IsDemoMode)
{
    public InspectableAccessContext ToAccessContext()
        => InspectableAccessContext.Create(UserName, Roles, Permissions);
}

public sealed record TemplateScanResult(
    string TemplatePath,
    IReadOnlyList<TemplateFieldDefinition> Fields,
    IReadOnlyList<string> Warnings);

public sealed record DocumentPreparationResult(
    CurrentUserSession CurrentUser,
    ProjectRecord Project,
    TemplateDefinition Template,
    DynamicDocumentFormObject Form,
    IReadOnlyList<DocumentFieldValue> InitialFieldValues);
