using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocumentAutomation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "generated_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutputRelativePath = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    GeneratedByUserName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    GeneratedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Warnings = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generated_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    LeadEngineer = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    TestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "template_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    RelativePath = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Section = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    FieldType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DefaultValue = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PersistenceKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    DatabaseKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    EditorHint = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    AllowSaveBackToProject = table.Column<bool>(type: "boolean", nullable: false),
                    SourcePriority = table.Column<string>(type: "jsonb", nullable: false),
                    VisibleRoles = table.Column<string>(type: "jsonb", nullable: false),
                    VisiblePermissions = table.Column<string>(type: "jsonb", nullable: false),
                    EditableRoles = table.Column<string>(type: "jsonb", nullable: false),
                    EditablePermissions = table.Column<string>(type: "jsonb", nullable: false),
                    ValidationHints = table.Column<string>(type: "jsonb", nullable: false),
                    AllowedValues = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_template_fields_template_definitions_TemplateDefinitionId",
                        column: x => x.TemplateDefinitionId,
                        principalTable: "template_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0e3968f2-d4b0-49cf-99ed-f2e915c5d1b6"), "Edit project data.", "projects.edit" },
                    { new Guid("1a8fe1d8-c4d2-42a7-b39d-e4c657bf4ea5"), "Manage roles.", "roles.manage" },
                    { new Guid("1e562906-10a0-46d0-8581-e7fb5989555c"), "View projects.", "projects.view" },
                    { new Guid("5fdb2c7b-1c58-46ae-b54e-b9f8c155b846"), "Manage templates.", "templates.manage" },
                    { new Guid("a44cb6f0-c52b-4c38-9a55-0fc33bd7ec4b"), "View users.", "users.view" },
                    { new Guid("be1c8999-d915-4d6f-aeb8-04bf66e6ec31"), "Generate documents.", "documents.generate" },
                    { new Guid("c3de4ae4-c931-49a3-a271-2a43630dcd4d"), "Approve generated documents.", "documents.approve" },
                    { new Guid("d4579347-7164-4123-943d-f188c4b9787f"), "Manage users.", "users.manage" },
                    { new Guid("e5a7dae4-5d4c-4f32-b856-fca0ec9aa4ff"), "View templates.", "templates.view" },
                    { new Guid("f74fdd91-bc0e-4b42-a2a1-86372ac7c4ee"), "Edit restricted fields.", "fields.edit.restricted" }
                });

            migrationBuilder.InsertData(
                table: "projects",
                columns: new[] { "Id", "CustomerName", "LeadEngineer", "Metadata", "ProjectCode", "ProjectName", "TestDate" },
                values: new object[] { new Guid("52fb7817-f0c2-4e20-b4a0-3ce3ea1d8623"), "Contoso Power Systems", "Elif Kaya", "{\"project.name\":\"Document Automation Foundation\",\"project.code\":\"PRJ-001\",\"project.customer\":\"Contoso Power Systems\",\"project.lead_engineer\":\"Elif Kaya\",\"project.test_date\":\"2026-04-07\"}", "PRJ-001", "Document Automation Foundation", new DateTime(2026, 4, 7, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33"), "Prepares data and generates documents.", "SystemEngineer" },
                    { new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1"), "Full administration access.", "Admin" },
                    { new Guid("b2752865-e4e0-45ca-bf20-6f835d7d516f"), "Manages templates and mappings.", "Designer" }
                });

            migrationBuilder.InsertData(
                table: "template_definitions",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "DocumentType", "IsActive", "Name", "RelativePath" },
                values: new object[] { new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), new DateTime(2026, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Demo template for the first product foundation.", "AcceptanceReport", true, "System Acceptance Report", "system-acceptance-report.docx" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "DisplayName", "IsActive", "UserName" },
                values: new object[,]
                {
                    { new Guid("0d67cc67-ae3a-4cba-a771-71d05d0c4205"), "System Engineer", true, "engineer" },
                    { new Guid("37a6f5f6-8529-4ff4-a368-21440dad6370"), "Admin User", true, "admin" },
                    { new Guid("86c2917a-3a94-44cc-8cb8-7ee0254e6a39"), "Template Designer", true, "designer" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("0e3968f2-d4b0-49cf-99ed-f2e915c5d1b6"), new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33") },
                    { new Guid("1e562906-10a0-46d0-8581-e7fb5989555c"), new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33") },
                    { new Guid("be1c8999-d915-4d6f-aeb8-04bf66e6ec31"), new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33") },
                    { new Guid("e5a7dae4-5d4c-4f32-b856-fca0ec9aa4ff"), new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33") },
                    { new Guid("0e3968f2-d4b0-49cf-99ed-f2e915c5d1b6"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("1a8fe1d8-c4d2-42a7-b39d-e4c657bf4ea5"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("1e562906-10a0-46d0-8581-e7fb5989555c"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("5fdb2c7b-1c58-46ae-b54e-b9f8c155b846"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("a44cb6f0-c52b-4c38-9a55-0fc33bd7ec4b"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("be1c8999-d915-4d6f-aeb8-04bf66e6ec31"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("c3de4ae4-c931-49a3-a271-2a43630dcd4d"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("d4579347-7164-4123-943d-f188c4b9787f"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("e5a7dae4-5d4c-4f32-b856-fca0ec9aa4ff"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("f74fdd91-bc0e-4b42-a2a1-86372ac7c4ee"), new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1") },
                    { new Guid("1e562906-10a0-46d0-8581-e7fb5989555c"), new Guid("b2752865-e4e0-45ca-bf20-6f835d7d516f") },
                    { new Guid("5fdb2c7b-1c58-46ae-b54e-b9f8c155b846"), new Guid("b2752865-e4e0-45ca-bf20-6f835d7d516f") },
                    { new Guid("e5a7dae4-5d4c-4f32-b856-fca0ec9aa4ff"), new Guid("b2752865-e4e0-45ca-bf20-6f835d7d516f") }
                });

            migrationBuilder.InsertData(
                table: "template_fields",
                columns: new[] { "Id", "AllowSaveBackToProject", "AllowedValues", "Category", "DatabaseKey", "DefaultValue", "Description", "DisplayName", "EditablePermissions", "EditableRoles", "EditorHint", "FieldKey", "FieldType", "IsRequired", "Order", "PersistenceKey", "Section", "SourcePriority", "TemplateDefinitionId", "ValidationHints", "VisiblePermissions", "VisibleRoles" },
                values: new object[,]
                {
                    { new Guid("0bd1b7df-8ca2-4ef9-bb51-18446133593a"), false, "[]", "Audit", "system.generated_on", null, null, "Generated On", "[]", "[]", null, "system.generated_on", "Date", true, 6, null, "Workflow", "[\"Computed\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("12752c1a-4c1f-4b10-be1f-6028fbdbb0d0"), false, "[]", "Audit", "system.current_user", null, null, "Prepared By", "[]", "[]", null, "system.current_user", "Text", true, 5, null, "Workflow", "[\"Computed\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("28331e6f-b8ec-4f7d-b8dd-05a36d8ffbfc"), true, "[]", "Content", null, "Initial summary goes here.", null, "Summary Note", "[]", "[]", null, "summary_note", "MultiLineText", true, 7, null, "Report", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("2962d966-bca8-48a8-b1da-be023f8c0ab0"), true, "[]", "Approval", null, null, null, "Approval Note", "[\"fields.edit.restricted\"]", "[]", null, "approval_note", "MultiLineText", true, 10, null, "Report", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("38183582-0d4e-45bb-9a81-eab16ff1e44c"), false, "[]", "Identity", "project.code", null, null, "Project Code", "[]", "[]", null, "project_code", "Text", true, 2, null, "Project", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("5a5187d2-3fa8-4ddd-80c4-c001debe4df6"), false, "[]", "Identity", "project.name", null, null, "Project Name", "[]", "[]", null, "project_name", "Text", true, 1, null, "Project", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("a26151a2-7bee-425a-81b0-1a80c3fc0aa5"), false, "[]", "Evidence", null, null, null, "Test Setup Image", "[]", "[]", null, "image:test_setup", "Image", true, 8, null, "Report", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("a93d4890-9bb5-4ead-8d98-beac04e48709"), false, "[]", "Schedule", "project.test_date", null, null, "Test Date", "[]", "[]", null, "test_date", "Date", true, 3, null, "Project", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("ad10ce37-b2f8-4068-a831-c47a84357f35"), false, "[]", "Evidence", null, null, null, "Test Results Table", "[]", "[]", null, "table:test_results", "Table", true, 9, null, "Report", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" },
                    { new Guid("d58164c7-c47f-4b46-a835-642bc84e0bcb"), false, "[]", "People", "project.lead_engineer", null, null, "Lead Engineer", "[]", "[]", null, "lead_engineer", "Text", true, 4, null, "Project", "[\"Database\",\"Default\"]", new Guid("33f0c241-c725-4919-9a57-e6a7925a2036"), "{}", "[]", "[]" }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("19c459a6-7b16-49bd-9df6-46e773f53a33"), new Guid("0d67cc67-ae3a-4cba-a771-71d05d0c4205") },
                    { new Guid("64f75d40-bca0-4e84-8e16-2f4e507d2da1"), new Guid("37a6f5f6-8529-4ff4-a368-21440dad6370") },
                    { new Guid("b2752865-e4e0-45ca-bf20-6f835d7d516f"), new Guid("86c2917a-3a94-44cc-8cb8-7ee0254e6a39") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name",
                table: "permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_ProjectCode",
                table: "projects",
                column: "ProjectCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_template_fields_TemplateDefinitionId_FieldKey",
                table: "template_fields",
                columns: new[] { "TemplateDefinitionId", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "generated_documents");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "template_fields");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "template_definitions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
