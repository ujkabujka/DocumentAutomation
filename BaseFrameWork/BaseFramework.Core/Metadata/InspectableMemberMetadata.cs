using System.Reflection;

namespace BaseFramework.Core.Metadata;

public sealed record InspectableMemberMetadata(
    string Key,
    string DisplayName,
    MemberKind Kind,
    bool ReadOnly,
    int Order,
    Type ValueType,
    PropertyInfo? Property,
    MethodInfo? Method,
    PropertyInfo? ValueSourceProperty = null,
    IReadOnlyList<InspectableMemberMetadata>? Parameters = null,
    object? DefaultValue = null);
