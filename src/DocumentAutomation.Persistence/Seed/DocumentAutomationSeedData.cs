using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Persistence.Seed;

public static class DocumentAutomationSeedData
{
    public static readonly Guid AdminRoleId = Guid.Parse("64F75D40-BCA0-4E84-8E16-2F4E507D2DA1");
    public static readonly Guid DesignerRoleId = Guid.Parse("B2752865-E4E0-45CA-BF20-6F835D7D516F");
    public static readonly Guid SystemEngineerRoleId = Guid.Parse("19C459A6-7B16-49BD-9DF6-46E773F53A33");

    public static readonly Guid AdminUserId = Guid.Parse("37A6F5F6-8529-4FF4-A368-21440DAD6370");
    public static readonly Guid DesignerUserId = Guid.Parse("86C2917A-3A94-44CC-8CB8-7EE0254E6A39");
    public static readonly Guid EngineerUserId = Guid.Parse("0D67CC67-AE3A-4CBA-A771-71D05D0C4205");

    public static readonly Guid TemplateId = Guid.Parse("33F0C241-C725-4919-9A57-E6A7925A2036");
    public static readonly Guid ProjectId = Guid.Parse("52FB7817-F0C2-4E20-B4A0-3CE3EA1D8623");
    public static readonly Guid ProjectNameFieldId = Guid.Parse("5A5187D2-3FA8-4DDD-80C4-C001DEBE4DF6");
    public static readonly Guid ProjectCodeFieldId = Guid.Parse("38183582-0D4E-45BB-9A81-EAB16FF1E44C");
    public static readonly Guid TestDateFieldId = Guid.Parse("A93D4890-9BB5-4EAD-8D98-BEAC04E48709");
    public static readonly Guid LeadEngineerFieldId = Guid.Parse("D58164C7-C47F-4B46-A835-642BC84E0BCB");
    public static readonly Guid PreparedByFieldId = Guid.Parse("12752C1A-4C1F-4B10-BE1F-6028FBDBB0D0");
    public static readonly Guid GeneratedOnFieldId = Guid.Parse("0BD1B7DF-8CA2-4EF9-BB51-18446133593A");
    public static readonly Guid SummaryNoteFieldId = Guid.Parse("28331E6F-B8EC-4F7D-B8DD-05A36D8FFBFC");
    public static readonly Guid TestSetupImageFieldId = Guid.Parse("A26151A2-7BEE-425A-81B0-1A80C3FC0AA5");
    public static readonly Guid TestResultsTableFieldId = Guid.Parse("AD10CE37-B2F8-4068-A831-C47A84357F35");
    public static readonly Guid ApprovalNoteFieldId = Guid.Parse("2962D966-BCA8-48A8-B1DA-BE023F8C0AB0");

    public static readonly Guid UsersViewPermissionId = Guid.Parse("A44CB6F0-C52B-4C38-9A55-0FC33BD7EC4B");
    public static readonly Guid UsersManagePermissionId = Guid.Parse("D4579347-7164-4123-943D-F188C4B9787F");
    public static readonly Guid RolesManagePermissionId = Guid.Parse("1A8FE1D8-C4D2-42A7-B39D-E4C657BF4EA5");
    public static readonly Guid TemplatesViewPermissionId = Guid.Parse("E5A7DAE4-5D4C-4F32-B856-FCA0EC9AA4FF");
    public static readonly Guid TemplatesManagePermissionId = Guid.Parse("5FDB2C7B-1C58-46AE-B54E-B9F8C155B846");
    public static readonly Guid DocumentsGeneratePermissionId = Guid.Parse("BE1C8999-D915-4D6F-AEB8-04BF66E6EC31");
    public static readonly Guid DocumentsApprovePermissionId = Guid.Parse("C3DE4AE4-C931-49A3-A271-2A43630DCD4D");
    public static readonly Guid ProjectsViewPermissionId = Guid.Parse("1E562906-10A0-46D0-8581-E7FB5989555C");
    public static readonly Guid ProjectsEditPermissionId = Guid.Parse("0E3968F2-D4B0-49CF-99ED-F2E915C5D1B6");
    public static readonly Guid RestrictedFieldEditPermissionId = Guid.Parse("F74FDD91-BC0E-4B42-A2A1-86372AC7C4EE");

    public static IReadOnlyList<Role> Roles =>
    [
        new() { Id = AdminRoleId, Name = RoleNames.Admin, Description = "Full administration access." },
        new() { Id = DesignerRoleId, Name = RoleNames.Designer, Description = "Manages templates and mappings." },
        new() { Id = SystemEngineerRoleId, Name = RoleNames.SystemEngineer, Description = "Prepares data and generates documents." }
    ];

    public static IReadOnlyList<Permission> Permissions =>
    [
        new() { Id = UsersViewPermissionId, Name = PermissionNames.UsersView, Description = "View users." },
        new() { Id = UsersManagePermissionId, Name = PermissionNames.UsersManage, Description = "Manage users." },
        new() { Id = RolesManagePermissionId, Name = PermissionNames.RolesManage, Description = "Manage roles." },
        new() { Id = TemplatesViewPermissionId, Name = PermissionNames.TemplatesView, Description = "View templates." },
        new() { Id = TemplatesManagePermissionId, Name = PermissionNames.TemplatesManage, Description = "Manage templates." },
        new() { Id = DocumentsGeneratePermissionId, Name = PermissionNames.DocumentsGenerate, Description = "Generate documents." },
        new() { Id = DocumentsApprovePermissionId, Name = PermissionNames.DocumentsApprove, Description = "Approve generated documents." },
        new() { Id = ProjectsViewPermissionId, Name = PermissionNames.ProjectsView, Description = "View projects." },
        new() { Id = ProjectsEditPermissionId, Name = PermissionNames.ProjectsEdit, Description = "Edit project data." },
        new() { Id = RestrictedFieldEditPermissionId, Name = PermissionNames.FieldsEditRestricted, Description = "Edit restricted fields." }
    ];

    public static IReadOnlyList<User> Users =>
    [
        new() { Id = AdminUserId, UserName = "admin", DisplayName = "Admin User" },
        new() { Id = DesignerUserId, UserName = "designer", DisplayName = "Template Designer" },
        new() { Id = EngineerUserId, UserName = "engineer", DisplayName = "System Engineer" }
    ];

    public static IReadOnlyList<UserRole> UserRoles =>
    [
        new() { UserId = AdminUserId, RoleId = AdminRoleId },
        new() { UserId = DesignerUserId, RoleId = DesignerRoleId },
        new() { UserId = EngineerUserId, RoleId = SystemEngineerRoleId }
    ];

    public static IReadOnlyList<RolePermission> RolePermissions =>
    [
        Map(AdminRoleId, UsersViewPermissionId),
        Map(AdminRoleId, UsersManagePermissionId),
        Map(AdminRoleId, RolesManagePermissionId),
        Map(AdminRoleId, TemplatesViewPermissionId),
        Map(AdminRoleId, TemplatesManagePermissionId),
        Map(AdminRoleId, DocumentsGeneratePermissionId),
        Map(AdminRoleId, DocumentsApprovePermissionId),
        Map(AdminRoleId, ProjectsViewPermissionId),
        Map(AdminRoleId, ProjectsEditPermissionId),
        Map(AdminRoleId, RestrictedFieldEditPermissionId),

        Map(DesignerRoleId, TemplatesViewPermissionId),
        Map(DesignerRoleId, TemplatesManagePermissionId),
        Map(DesignerRoleId, ProjectsViewPermissionId),

        Map(SystemEngineerRoleId, ProjectsViewPermissionId),
        Map(SystemEngineerRoleId, ProjectsEditPermissionId),
        Map(SystemEngineerRoleId, TemplatesViewPermissionId),
        Map(SystemEngineerRoleId, DocumentsGeneratePermissionId)
    ];

    public static TemplateDefinition Template => new()
    {
        Id = TemplateId,
        Name = "System Acceptance Report",
        DocumentType = "AcceptanceReport",
        Description = "Demo template for the first product foundation.",
        RelativePath = "system-acceptance-report.docx",
        IsActive = true,
        CreatedAtUtc = new DateTime(2026, 4, 5, 12, 0, 0, DateTimeKind.Utc)
    };

    public static IReadOnlyList<TemplateFieldDefinition> TemplateFields =>
    [
        CreateField(ProjectNameFieldId, "project_name", "Project Name", TemplateFieldType.Text, 1, "Project", "Identity", databaseKey: "project.name"),
        CreateField(ProjectCodeFieldId, "project_code", "Project Code", TemplateFieldType.Text, 2, "Project", "Identity", databaseKey: "project.code"),
        CreateField(TestDateFieldId, "test_date", "Test Date", TemplateFieldType.Date, 3, "Project", "Schedule", databaseKey: "project.test_date"),
        CreateField(LeadEngineerFieldId, "lead_engineer", "Lead Engineer", TemplateFieldType.Text, 4, "Project", "People", databaseKey: "project.lead_engineer"),
        CreateField(PreparedByFieldId, "system.current_user", "Prepared By", TemplateFieldType.Text, 5, "Workflow", "Audit", databaseKey: "system.current_user", sourcePriority: ["Computed"]),
        CreateField(GeneratedOnFieldId, "system.generated_on", "Generated On", TemplateFieldType.Date, 6, "Workflow", "Audit", databaseKey: "system.generated_on", sourcePriority: ["Computed"]),
        CreateField(SummaryNoteFieldId, "summary_note", "Summary Note", TemplateFieldType.MultiLineText, 7, "Report", "Content", defaultValue: "Initial summary goes here."),
        CreateField(TestSetupImageFieldId, "image:test_setup", "Test Setup Image", TemplateFieldType.Image, 8, "Report", "Evidence"),
        CreateField(TestResultsTableFieldId, "table:test_results", "Test Results Table", TemplateFieldType.Table, 9, "Report", "Evidence"),
        CreateField(ApprovalNoteFieldId, "approval_note", "Approval Note", TemplateFieldType.MultiLineText, 10, "Report", "Approval", editablePermissions: [PermissionNames.FieldsEditRestricted])
    ];

    public static ProjectRecord Project => new()
    {
        Id = ProjectId,
        ProjectCode = "PRJ-001",
        ProjectName = "Document Automation Foundation",
        CustomerName = "Contoso Power Systems",
        LeadEngineer = "Elif Kaya",
        TestDate = new DateTime(2026, 4, 7, 9, 0, 0, DateTimeKind.Utc),
        Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["project.name"] = "Document Automation Foundation",
            ["project.code"] = "PRJ-001",
            ["project.customer"] = "Contoso Power Systems",
            ["project.lead_engineer"] = "Elif Kaya",
            ["project.test_date"] = "2026-04-07"
        }
    };

    private static RolePermission Map(Guid roleId, Guid permissionId)
        => new() { RoleId = roleId, PermissionId = permissionId };

    private static TemplateFieldDefinition CreateField(
        Guid id,
        string fieldKey,
        string displayName,
        TemplateFieldType fieldType,
        int order,
        string section,
        string category,
        string? databaseKey = null,
        string? defaultValue = null,
        string[]? sourcePriority = null,
        string[]? editablePermissions = null)
        => new()
        {
            Id = id,
            TemplateDefinitionId = TemplateId,
            FieldKey = fieldKey,
            DisplayName = displayName,
            FieldType = fieldType,
            IsRequired = true,
            Order = order,
            Section = section,
            Category = category,
            DatabaseKey = databaseKey,
            DefaultValue = defaultValue,
            AllowSaveBackToProject = fieldKey is "summary_note" or "approval_note",
            SourcePriority = sourcePriority ?? ["Database", "Default"],
            EditablePermissions = editablePermissions ?? Array.Empty<string>(),
            ValidationHints = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        };
}
