using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Persistence;
using DocumentAutomation.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace DocumentAutomation.Persistence.Tests;

public sealed class PersistenceModelTests
{
    private static readonly string ConnectionString = "Host=localhost;Database=document_automation_tests;Username=postgres;Password=postgres";

    [Fact]
    public void Model_ShouldUseExpectedTablesAndJsonbColumns()
    {
        using var dbContext = new DocumentAutomationDbContext(ConnectionString);

        var templateEntity = dbContext.Model.FindEntityType(typeof(TemplateFieldDefinition));
        var projectEntity = dbContext.Model.FindEntityType(typeof(ProjectRecord));
        var generatedEntity = dbContext.Model.FindEntityType(typeof(GeneratedDocumentRecord));

        Assert.NotNull(templateEntity);
        Assert.NotNull(projectEntity);
        Assert.NotNull(generatedEntity);

        var template = templateEntity!;
        var project = projectEntity!;
        var generated = generatedEntity!;

        Assert.Equal("template_fields", template.GetTableName());
        Assert.Equal("jsonb", template.FindProperty(nameof(TemplateFieldDefinition.SourcePriority))!.GetColumnType());
        Assert.Equal("jsonb", template.FindProperty(nameof(TemplateFieldDefinition.ValidationHints))!.GetColumnType());
        Assert.Equal("jsonb", template.FindProperty(nameof(TemplateFieldDefinition.AllowedValues))!.GetColumnType());

        Assert.Equal("projects", project.GetTableName());
        Assert.Equal("jsonb", project.FindProperty(nameof(ProjectRecord.Metadata))!.GetColumnType());

        Assert.Equal("generated_documents", generated.GetTableName());
        Assert.Equal("jsonb", generated.FindProperty(nameof(GeneratedDocumentRecord.Warnings))!.GetColumnType());
    }

    [Fact]
    public void Model_ShouldConfigureCompositeRelationshipKeys()
    {
        using var dbContext = new DocumentAutomationDbContext(ConnectionString);

        var userRoleEntity = dbContext.Model.FindEntityType(typeof(UserRole));
        var rolePermissionEntity = dbContext.Model.FindEntityType(typeof(RolePermission));

        Assert.Equal(["UserId", "RoleId"], userRoleEntity!.FindPrimaryKey()!.Properties.Select(property => property.Name).ToArray());
        Assert.Equal(["RoleId", "PermissionId"], rolePermissionEntity!.FindPrimaryKey()!.Properties.Select(property => property.Name).ToArray());
    }

    [Fact]
    public void SeedData_ShouldContainCoreSecurityAndTemplateRecords()
    {
        Assert.Contains(DocumentAutomationSeedData.Roles, role => role.Name == RoleNames.Admin);
        Assert.Contains(DocumentAutomationSeedData.Roles, role => role.Name == RoleNames.Designer);
        Assert.Contains(DocumentAutomationSeedData.Roles, role => role.Name == RoleNames.SystemEngineer);

        Assert.Contains(DocumentAutomationSeedData.Permissions, permission => permission.Name == PermissionNames.UsersManage);
        Assert.Contains(DocumentAutomationSeedData.Permissions, permission => permission.Name == PermissionNames.DocumentsGenerate);
        Assert.Contains(DocumentAutomationSeedData.RolePermissions, mapping => mapping.RoleId == DocumentAutomationSeedData.AdminRoleId && mapping.PermissionId == DocumentAutomationSeedData.RestrictedFieldEditPermissionId);

        Assert.Equal("System Acceptance Report", DocumentAutomationSeedData.Template.Name);
        Assert.Contains(DocumentAutomationSeedData.TemplateFields, field => field.FieldKey == "project_name");
        Assert.Contains(DocumentAutomationSeedData.TemplateFields, field => field.FieldKey == "image:test_setup" && field.FieldType == TemplateFieldType.Image);
        Assert.Equal("PRJ-001", DocumentAutomationSeedData.Project.ProjectCode);
    }

    [Fact]
    public void GenerateCreateScript_ShouldContainCoreTablesAndJsonbUsage()
    {
        using var dbContext = new DocumentAutomationDbContext(ConnectionString);

        var script = dbContext.Database.GenerateCreateScript();

        Assert.Contains("template_definitions", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("template_fields", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("generated_documents", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("jsonb", script, StringComparison.OrdinalIgnoreCase);
    }
}
