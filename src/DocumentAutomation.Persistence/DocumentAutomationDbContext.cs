using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Domain.Templates;
using DocumentAutomation.Persistence.Infrastructure;
using DocumentAutomation.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace DocumentAutomation.Persistence;

public sealed class DocumentAutomationDbContext(string connectionString) : DbContext
{
    private readonly string _connectionString = connectionString;

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TemplateDefinition> TemplateDefinitions => Set<TemplateDefinition>();
    public DbSet<TemplateFieldDefinition> TemplateFields => Set<TemplateFieldDefinition>();
    public DbSet<ProjectRecord> Projects => Set<ProjectRecord>();
    public DbSet<GeneratedDocumentRecord> GeneratedDocuments => Set<GeneratedDocumentRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureSecurity(modelBuilder);
        ConfigureTemplates(modelBuilder);
        ConfigureProjects(modelBuilder);
        ConfigureGeneratedDocuments(modelBuilder);
        Seed(modelBuilder);
    }

    private static void ConfigureSecurity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.UserName).IsRequired().HasMaxLength(120);
            entity.Property(user => user.DisplayName).IsRequired().HasMaxLength(160);
            entity.HasIndex(user => user.UserName).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Name).IsRequired().HasMaxLength(80);
            entity.Property(role => role.Description).HasMaxLength(240);
            entity.HasIndex(role => role.Name).IsUnique();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(permission => permission.Id);
            entity.Property(permission => permission.Name).IsRequired().HasMaxLength(120);
            entity.Property(permission => permission.Description).HasMaxLength(240);
            entity.HasIndex(permission => permission.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(item => new { item.UserId, item.RoleId });
            entity.HasOne(item => item.User).WithMany(user => user.UserRoles).HasForeignKey(item => item.UserId);
            entity.HasOne(item => item.Role).WithMany(role => role.UserRoles).HasForeignKey(item => item.RoleId);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(item => new { item.RoleId, item.PermissionId });
            entity.HasOne(item => item.Role).WithMany(role => role.RolePermissions).HasForeignKey(item => item.RoleId);
            entity.HasOne(item => item.Permission).WithMany(permission => permission.RolePermissions).HasForeignKey(item => item.PermissionId);
        });
    }

    private static void ConfigureTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemplateDefinition>(entity =>
        {
            entity.ToTable("template_definitions");
            entity.HasKey(template => template.Id);
            entity.Property(template => template.Name).IsRequired().HasMaxLength(160);
            entity.Property(template => template.DocumentType).IsRequired().HasMaxLength(120);
            entity.Property(template => template.Description).HasMaxLength(400);
            entity.Property(template => template.RelativePath).IsRequired().HasMaxLength(260);
            entity.HasMany(template => template.Fields)
                .WithOne(field => field.TemplateDefinition)
                .HasForeignKey(field => field.TemplateDefinitionId);
        });

        modelBuilder.Entity<TemplateFieldDefinition>(entity =>
        {
            entity.ToTable("template_fields");
            entity.HasKey(field => field.Id);
            entity.Property(field => field.FieldKey).IsRequired().HasMaxLength(160);
            entity.Property(field => field.DisplayName).IsRequired().HasMaxLength(160);
            entity.Property(field => field.Description).HasMaxLength(400);
            entity.Property(field => field.Section).HasMaxLength(120);
            entity.Property(field => field.Category).HasMaxLength(120);
            entity.Property(field => field.FieldType).HasConversion<string>().HasMaxLength(40);
            entity.Property(field => field.DefaultValue).HasMaxLength(4000);
            entity.Property(field => field.PersistenceKey).HasMaxLength(160);
            entity.Property(field => field.DatabaseKey).HasMaxLength(160);
            entity.Property(field => field.EditorHint).HasMaxLength(80);

            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.SourcePriority), () => Array.Empty<string>());
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.VisibleRoles), () => Array.Empty<string>());
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.VisiblePermissions), () => Array.Empty<string>());
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.EditableRoles), () => Array.Empty<string>());
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.EditablePermissions), () => Array.Empty<string>());
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.ValidationHints), () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(field => field.AllowedValues), () => Array.Empty<string>());

            entity.HasIndex(field => new { field.TemplateDefinitionId, field.FieldKey }).IsUnique();
        });
    }

    private static void ConfigureProjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectRecord>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(project => project.Id);
            entity.Property(project => project.ProjectCode).IsRequired().HasMaxLength(80);
            entity.Property(project => project.ProjectName).IsRequired().HasMaxLength(160);
            entity.Property(project => project.CustomerName).HasMaxLength(160);
            entity.Property(project => project.LeadEngineer).HasMaxLength(160);
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(project => project.Metadata), () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            entity.HasIndex(project => project.ProjectCode).IsUnique();
        });
    }

    private static void ConfigureGeneratedDocuments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeneratedDocumentRecord>(entity =>
        {
            entity.ToTable("generated_documents");
            entity.HasKey(document => document.Id);
            entity.Property(document => document.OutputRelativePath).IsRequired().HasMaxLength(260);
            entity.Property(document => document.GeneratedByUserName).IsRequired().HasMaxLength(120);
            entity.Property(document => document.Status).IsRequired().HasMaxLength(60);
            JsonValueConverterFactory.ApplyJsonConversion(entity.Property(document => document.Warnings), () => Array.Empty<string>());
        });
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(DocumentAutomationSeedData.Roles);
        modelBuilder.Entity<Permission>().HasData(DocumentAutomationSeedData.Permissions);
        modelBuilder.Entity<User>().HasData(DocumentAutomationSeedData.Users);
        modelBuilder.Entity<UserRole>().HasData(DocumentAutomationSeedData.UserRoles);
        modelBuilder.Entity<RolePermission>().HasData(DocumentAutomationSeedData.RolePermissions);
        modelBuilder.Entity<TemplateDefinition>().HasData(DocumentAutomationSeedData.Template);
        modelBuilder.Entity<TemplateFieldDefinition>().HasData(DocumentAutomationSeedData.TemplateFields);
        modelBuilder.Entity<ProjectRecord>().HasData(DocumentAutomationSeedData.Project);
    }
}
