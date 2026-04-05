namespace DocumentAutomation.Domain.Templates;

public enum TemplateFieldType
{
    Text,
    Number,
    Date,
    Boolean,
    Enum,
    Image,
    Table,
    Note,
    MultiLineText,
    File
}

public enum FieldValueSource
{
    Database,
    User,
    Computed,
    Default,
    Missing
}

public sealed class TemplateDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<TemplateFieldDefinition> Fields { get; set; } = new List<TemplateFieldDefinition>();
}

public sealed class TemplateFieldDefinition
{
    public Guid Id { get; set; }
    public Guid TemplateDefinitionId { get; set; }
    public TemplateDefinition TemplateDefinition { get; set; } = null!;
    public string FieldKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Section { get; set; }
    public string? Category { get; set; }
    public TemplateFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public string? DefaultValue { get; set; }
    public string? PersistenceKey { get; set; }
    public string? DatabaseKey { get; set; }
    public string? EditorHint { get; set; }
    public bool AllowSaveBackToProject { get; set; }
    public string[] SourcePriority { get; set; } = Array.Empty<string>();
    public string[] VisibleRoles { get; set; } = Array.Empty<string>();
    public string[] VisiblePermissions { get; set; } = Array.Empty<string>();
    public string[] EditableRoles { get; set; } = Array.Empty<string>();
    public string[] EditablePermissions { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> ValidationHints { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string[] AllowedValues { get; set; } = Array.Empty<string>();
}
