using BaseFramework.Core;
using BaseFramework.Core.Access;
using BaseFramework.Core.Metadata;
using BaseFramework.Core.Notes;
using BaseFramework.Core.Services;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;

namespace DocumentAutomation.Application.Models;

public sealed class DynamicDocumentFormObject : ObservableObject, IRuntimeInspectableMetadataSource
{
    private readonly IReadOnlyList<DynamicFieldState> _fields;

    public DynamicDocumentFormObject(
        IEnumerable<TemplateFieldDefinition> fieldDefinitions,
        IEnumerable<DocumentFieldValue> initialValues)
    {
        var valuesByKey = initialValues.ToDictionary(value => value.FieldKey, StringComparer.OrdinalIgnoreCase);
        _fields = fieldDefinitions
            .OrderBy(field => field.Order)
            .Select(field =>
            {
                var existing = valuesByKey.TryGetValue(field.FieldKey, out var value)
                    ? value
                    : new DocumentFieldValue
                    {
                        FieldKey = field.FieldKey,
                        DisplayName = field.DisplayName,
                        FieldType = field.FieldType,
                        Source = FieldValueSource.Missing,
                        IsMissing = true,
                        CanPersistBack = field.AllowSaveBackToProject
                    };

                ApplyExternalValue(field.FieldKey, ConvertValue(field, existing.Value));
                return new DynamicFieldState(field, existing);
            })
            .ToList();
    }

    public IReadOnlyList<DocumentFieldValue> SnapshotValues()
        => _fields.Select(field =>
        {
            var currentValue = GetRaw(field.Definition.FieldKey);
            return new DocumentFieldValue
            {
                FieldKey = field.Definition.FieldKey,
                DisplayName = field.Definition.DisplayName,
                FieldType = field.Definition.FieldType,
                Value = currentValue,
                Source = IsEmpty(currentValue) ? FieldValueSource.Missing : ResolveSource(field.Initial.Source, currentValue),
                IsMissing = field.Definition.IsRequired && IsEmpty(currentValue),
                CanPersistBack = field.Definition.AllowSaveBackToProject
            };
        }).ToList();

    public InspectableTypeMetadata GetRuntimeMetadata()
        => new(
            GetType(),
            // The inspector expects member metadata, not CLR properties.
            // We synthesize that metadata at runtime so a scanned template can behave like a normal model.
            _fields.Select(BuildMemberMetadata).ToList());

    protected override void OnUpdate()
    {
    }

    private static InspectableMemberMetadata BuildMemberMetadata(DynamicFieldState state)
    {
        var definition = state.Definition;
        return new InspectableMemberMetadata(
            definition.FieldKey,
            definition.DisplayName,
            ResolveKind(definition),
            false,
            definition.Order,
            ResolveValueType(definition.FieldType),
            null,
            null)
        {
            ClrName = definition.FieldKey,
            Description = definition.Description,
            Category = definition.Category,
            Section = definition.Section,
            EditorHint = ResolveEditorHint(definition),
            PersistenceKey = definition.PersistenceKey,
            DatabaseKey = definition.DatabaseKey,
            ValidationHints = BuildValidation(definition),
            AccessRules = new InspectableAccessRules(
                definition.VisibleRoles,
                definition.VisiblePermissions,
                definition.EditableRoles,
                definition.EditablePermissions,
                Array.Empty<string>(),
                Array.Empty<string>()),
            Getter = static (target, metadata) => ((DynamicDocumentFormObject)target).GetRaw(metadata.Key),
            Setter = static (target, value, metadata) => ((DynamicDocumentFormObject)target).SetFieldValue(metadata.Key, value),
            ValueSourceAccessor = definition.AllowedValues.Length == 0
                ? null
                : static (target, metadata) => ((DynamicDocumentFormObject)target).GetAllowedValues(metadata.Key)
        };
    }

    private IEnumerable<string> GetAllowedValues(string key)
        => _fields.First(field => string.Equals(field.Definition.FieldKey, key, StringComparison.OrdinalIgnoreCase)).Definition.AllowedValues;

    private void SetFieldValue(string key, object? value)
        => ApplyExternalValue(key, value);

    private static object? ConvertValue(TemplateFieldDefinition field, object? value)
    {
        if (value is null)
        {
            return null;
        }

        return field.FieldType switch
        {
            TemplateFieldType.Number when value is string text && double.TryParse(text, out var number) => number,
            TemplateFieldType.Date when value is string text && DateTime.TryParse(text, out var date) => date,
            TemplateFieldType.Boolean when value is string text && bool.TryParse(text, out var boolean) => boolean,
            TemplateFieldType.Note when value is string text => new NoteDocument(text),
            _ => value
        };
    }

    private static bool IsEmpty(object? value)
        => value switch
        {
            null => true,
            string text => string.IsNullOrWhiteSpace(text),
            _ => false
        };

    private static FieldValueSource ResolveSource(FieldValueSource existing, object? value)
        => IsEmpty(value)
            ? FieldValueSource.Missing
            : existing == FieldValueSource.Missing
                ? FieldValueSource.User
                : existing;

    private static InspectableValidationHints BuildValidation(TemplateFieldDefinition definition)
    {
        definition.ValidationHints.TryGetValue("regex", out var regex);
        definition.ValidationHints.TryGetValue("min", out var minText);
        definition.ValidationHints.TryGetValue("max", out var maxText);

        return new InspectableValidationHints(
            definition.IsRequired,
            double.TryParse(minText, out var min) ? min : null,
            double.TryParse(maxText, out var max) ? max : null,
            regex);
    }

    private static MemberKind ResolveKind(TemplateFieldDefinition definition)
        => definition.FieldType switch
        {
            TemplateFieldType.Text => MemberKind.String,
            TemplateFieldType.Number => MemberKind.Double,
            TemplateFieldType.Date => MemberKind.DateTime,
            TemplateFieldType.Boolean => MemberKind.Boolean,
            TemplateFieldType.Enum => MemberKind.Selection,
            TemplateFieldType.Image => MemberKind.Image,
            TemplateFieldType.Table => MemberKind.Table,
            TemplateFieldType.Note => MemberKind.Note,
            TemplateFieldType.MultiLineText => MemberKind.MultiLineText,
            TemplateFieldType.File => MemberKind.File,
            _ => MemberKind.String
        };

    private static Type ResolveValueType(TemplateFieldType fieldType)
        => fieldType switch
        {
            TemplateFieldType.Number => typeof(double),
            TemplateFieldType.Date => typeof(DateTime),
            TemplateFieldType.Boolean => typeof(bool),
            TemplateFieldType.Note => typeof(NoteDocument),
            _ => typeof(string)
        };

    private static string? ResolveEditorHint(TemplateFieldDefinition definition)
        => definition.FieldType switch
        {
            TemplateFieldType.Enum => EditorHints.Selection,
            TemplateFieldType.Image => EditorHints.Image,
            TemplateFieldType.File => EditorHints.File,
            TemplateFieldType.Table => EditorHints.Table,
            TemplateFieldType.MultiLineText => EditorHints.Multiline,
            _ => definition.EditorHint
        };

    private sealed record DynamicFieldState(TemplateFieldDefinition Definition, DocumentFieldValue Initial);
}
