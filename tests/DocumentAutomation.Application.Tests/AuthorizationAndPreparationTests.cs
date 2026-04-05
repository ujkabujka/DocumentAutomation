using BaseFramework.Core.Access;
using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Application.Services;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Tests;

public sealed class AuthorizationAndPreparationTests
{
    [Fact]
    public void AuthorizationService_ShouldApplyPermissionAndFieldRules()
    {
        var service = new ApplicationAuthorizationService();
        var admin = CreateSession(RoleNames.Admin, PermissionNames.FieldsEditRestricted, PermissionNames.UsersManage, PermissionNames.DocumentsGenerate);
        var engineer = CreateSession(RoleNames.SystemEngineer, PermissionNames.DocumentsGenerate);
        var restrictedField = new TemplateFieldDefinition
        {
            FieldKey = "approval_note",
            DisplayName = "Approval Note",
            EditablePermissions = [PermissionNames.FieldsEditRestricted],
            VisiblePermissions = [PermissionNames.DocumentsGenerate]
        };

        Assert.True(service.HasPermission(admin, PermissionNames.UsersManage));
        Assert.False(service.HasPermission(engineer, PermissionNames.UsersManage));
        Assert.True(service.CanView(restrictedField, engineer));
        Assert.False(service.CanEdit(restrictedField, engineer));
        Assert.True(service.CanEdit(restrictedField, admin));
    }

    [Fact]
    public async Task DocumentPreparationService_ShouldAutofillComputeDefaultsAndFilterHiddenFields()
    {
        var project = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            ProjectCode = "PRJ-42",
            ProjectName = "Relay Upgrade",
            LeadEngineer = "Aylin Demir",
            TestDate = new DateTime(2026, 4, 8, 10, 0, 0, DateTimeKind.Local),
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["project.name"] = "Relay Upgrade",
                ["project.code"] = "PRJ-42"
            }
        };

        var template = new TemplateDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Acceptance",
            RelativePath = "templates\\acceptance.docx",
            Fields =
            [
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "project_name",
                    DisplayName = "Project Name",
                    FieldType = TemplateFieldType.Text,
                    DatabaseKey = "project.name",
                    Order = 1
                },
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "system.current_user",
                    DisplayName = "Prepared By",
                    FieldType = TemplateFieldType.Text,
                    DatabaseKey = "system.current_user",
                    SourcePriority = ["Computed"],
                    Order = 2
                },
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "summary_note",
                    DisplayName = "Summary",
                    FieldType = TemplateFieldType.MultiLineText,
                    DefaultValue = "Initial summary",
                    AllowSaveBackToProject = true,
                    Order = 3
                },
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "serial_number",
                    DisplayName = "Serial Number",
                    FieldType = TemplateFieldType.Text,
                    IsRequired = true,
                    Order = 4
                },
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "admin_note",
                    DisplayName = "Admin Note",
                    FieldType = TemplateFieldType.MultiLineText,
                    VisibleRoles = [RoleNames.Admin],
                    Order = 5
                },
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "approval_note",
                    DisplayName = "Approval Note",
                    FieldType = TemplateFieldType.MultiLineText,
                    EditablePermissions = [PermissionNames.FieldsEditRestricted],
                    Order = 6
                }
            ]
        };

        var currentUser = CreateSessionWithDisplayName(RoleNames.SystemEngineer, "Engineer User", PermissionNames.DocumentsGenerate, PermissionNames.ProjectsEdit);
        var authorizationService = new ApplicationAuthorizationService();
        var service = new DocumentPreparationService(
            new StubCurrentUserContext(currentUser),
            authorizationService,
            new StubProjectDataService(project),
            new StubTemplateCatalogService(template),
            new StubTemplateStorage(),
            new StubOutputStorage());

        var result = await service.PrepareAsync(project.Id, template.Id);
        var metadata = result.Form.GetRuntimeMetadata();
        var accessEvaluator = new DefaultMemberAccessEvaluator();

        Assert.Equal("Relay Upgrade", result.InitialFieldValues.Single(value => value.FieldKey == "project_name").Value);
        Assert.Equal("Engineer User", result.InitialFieldValues.Single(value => value.FieldKey == "system.current_user").Value);
        Assert.Equal("Initial summary", result.InitialFieldValues.Single(value => value.FieldKey == "summary_note").Value);
        Assert.True(result.InitialFieldValues.Single(value => value.FieldKey == "serial_number").IsMissing);
        Assert.DoesNotContain(metadata.Members, member => member.Key == "admin_note");

        var approvalMetadata = Assert.Single(metadata.Members, member => member.Key == "approval_note");
        var approvalAccess = accessEvaluator.Evaluate(approvalMetadata, result.Form, currentUser.ToAccessContext());
        Assert.True(approvalAccess.CanView);
        Assert.False(approvalAccess.CanEdit);
    }

    [Fact]
    public async Task CreateGenerationRequestAsync_ShouldUseResolvedPathsAndCurrentFormValues()
    {
        var project = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            ProjectCode = "PRJ-77",
            ProjectName = "Generator Test",
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["project.name"] = "Generator Test"
            }
        };
        var template = new TemplateDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Report",
            RelativePath = "templates\\report.docx",
            Fields =
            [
                new TemplateFieldDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateDefinitionId = Guid.NewGuid(),
                    FieldKey = "project_name",
                    DisplayName = "Project Name",
                    FieldType = TemplateFieldType.Text,
                    DatabaseKey = "project.name",
                    Order = 1
                }
            ]
        };

        var outputStorage = new StubOutputStorage();
        var service = new DocumentPreparationService(
            new StubCurrentUserContext(CreateSession(RoleNames.SystemEngineer, PermissionNames.DocumentsGenerate)),
            new ApplicationAuthorizationService(),
            new StubProjectDataService(project),
            new StubTemplateCatalogService(template),
            new StubTemplateStorage(),
            outputStorage);

        var preparation = await service.PrepareAsync(project.Id, template.Id);
        preparation.Form.ApplyExternalValue("project_name", "Updated Project");

        var request = await service.CreateGenerationRequestAsync(preparation);

        Assert.Equal("C:\\templates\\templates\\report.docx", request.TemplatePath);
        Assert.Equal("C:\\outputs\\PRJ-77-Report.docx", request.OutputPath);
        Assert.Equal("Updated Project", request.Fields.Single(field => field.FieldKey == "project_name").Value);
    }

    private static CurrentUserSession CreateSession(string role, params string[] permissions)
        => new(Guid.NewGuid(), role.ToLowerInvariant(), $"{role} User", [role], permissions, false);

    private static CurrentUserSession CreateSessionWithDisplayName(string role, string displayName, params string[] permissions)
        => new(Guid.NewGuid(), role.ToLowerInvariant(), displayName, [role], permissions, false);

    private sealed class StubCurrentUserContext(CurrentUserSession session) : ICurrentUserContext
    {
        public Task<CurrentUserSession> GetCurrentUserAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(session);
    }

    private sealed class StubProjectDataService(ProjectRecord project) : IProjectDataService
    {
        public Task<IReadOnlyList<ProjectRecord>> GetProjectsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProjectRecord>>([project]);

        public Task<ProjectRecord?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult(projectId == project.Id ? project : null);

        public Task<IReadOnlyDictionary<string, object?>> GetAutofillValuesAsync(ProjectRecord selectedProject, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyDictionary<string, object?>>(selectedProject.Metadata.ToDictionary(item => item.Key, item => (object?)item.Value, StringComparer.OrdinalIgnoreCase));

        public Task SaveFieldValuesAsync(ProjectRecord selectedProject, IEnumerable<DocumentFieldValue> values, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class StubTemplateCatalogService(TemplateDefinition template) : ITemplateCatalogService
    {
        public Task<IReadOnlyList<TemplateDefinition>> GetTemplatesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TemplateDefinition>>([template]);

        public Task<TemplateDefinition?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
            => Task.FromResult(templateId == template.Id ? template : null);

        public Task SaveTemplateAsync(TemplateDefinition updatedTemplate, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class StubTemplateStorage : ITemplateStorage
    {
        public Task<IReadOnlyList<string>> GetTemplateFilesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<string>>(["templates\\report.docx"]);

        public Task EnsureSeedTemplatesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public string ResolveTemplatePath(string relativePath)
            => Path.Combine("C:\\templates", relativePath);
    }

    private sealed class StubOutputStorage : IOutputStorage
    {
        public Task<string> CreateOutputPathAsync(ProjectRecord project, TemplateDefinition template, CancellationToken cancellationToken = default)
            => Task.FromResult(Path.Combine("C:\\outputs", $"{project.ProjectCode}-{template.Name}.docx"));
    }
}
