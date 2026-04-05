using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Domain.Documents;

public sealed class ProjectRecord
{
    public Guid Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string LeadEngineer { get; set; } = string.Empty;
    public DateTime? TestDate { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class DocumentFieldValue
{
    public string FieldKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public TemplateFieldType FieldType { get; set; }
    public object? Value { get; set; }
    public FieldValueSource Source { get; set; }
    public bool IsMissing { get; set; }
    public bool CanPersistBack { get; set; }
}

public sealed class DocumentGenerationRequest
{
    public Guid TemplateDefinitionId { get; set; }
    public Guid ProjectRecordId { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public string TemplatePath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public IReadOnlyList<DocumentFieldValue> Fields { get; set; } = Array.Empty<DocumentFieldValue>();
}

public sealed class DocumentGenerationResult
{
    public bool Succeeded { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public IReadOnlyList<string> Warnings { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DocumentFieldValue> FinalFieldValues { get; set; } = Array.Empty<DocumentFieldValue>();
}

public sealed class GeneratedDocumentRecord
{
    public Guid Id { get; set; }
    public Guid TemplateDefinitionId { get; set; }
    public Guid ProjectRecordId { get; set; }
    public string OutputRelativePath { get; set; } = string.Empty;
    public string GeneratedByUserName { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Generated";
    public string[] Warnings { get; set; } = Array.Empty<string>();
}
