using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using BaseFramework.Core.Attributes;
using BaseFramework.Core.Metadata;
using BaseFramework.Core.Generated;
using BaseFramework.Core.Notes;

namespace BaseFramework.Core.Services;

public sealed class ReflectionObjectMetadataProvider : IObjectMetadataProvider
{
    private readonly ConcurrentDictionary<Type, InspectableTypeMetadata> _cache = new();

    public InspectableTypeMetadata GetMetadata(Type targetType)
    {
        if (GeneratedMetadataRegistry.TryCreate(targetType, out var generated) && generated.Members.Count > 0)
        {
            return generated;
        }

        return _cache.GetOrAdd(targetType, BuildMetadata);
    }

    private static InspectableTypeMetadata BuildMetadata(Type targetType)
    {
        var list = new List<InspectableMemberMetadata>();

        foreach (var property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var attr = property.GetCustomAttribute<InspectableMemberAttribute>();
            if (attr is null)
            {
                continue;
            }

            var valueSource = ResolveValueSourceProperty(targetType, attr.ValueSourcePropertyName);

            list.Add(new InspectableMemberMetadata(
                attr.Key,
                attr.DisplayName,
                ResolveKind(property.PropertyType),
                attr.ReadOnly || !property.CanWrite,
                attr.Order,
                property.PropertyType,
                property,
                null,
                valueSource));
        }

        foreach (var method in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var attr = method.GetCustomAttribute<InspectableMemberAttribute>();
            if (attr is null)
            {
                continue;
            }

            var parameters = method.GetParameters()
                .Select(p => new InspectableMemberMetadata(
                    p.Name ?? p.ParameterType.Name,
                    p.Name ?? p.ParameterType.Name,
                    ResolveKind(p.ParameterType),
                    false,
                    0,
                    p.ParameterType,
                    null,
                    null,
                    null,
                    null,
                    p.HasDefaultValue ? p.DefaultValue : GetDefaultValue(p.ParameterType)))
                .ToList();

            list.Add(new InspectableMemberMetadata(
                attr.Key,
                attr.DisplayName,
                MemberKind.Method,
                true,
                attr.Order,
                method.ReturnType,
                null,
                method,
                null,
                parameters));
        }

        var ordered = list.OrderBy(m => m.Order).ThenBy(m => m.DisplayName).ToList();
        return new InspectableTypeMetadata(targetType, ordered);
    }

    private static object? GetDefaultValue(Type type)
    {
        if (!type.IsValueType) return null;
        return Activator.CreateInstance(type);
    }

    private static MemberKind ResolveKind(Type type)
    {
        if (type.IsEnum) return MemberKind.Enum;
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return MemberKind.Integer;
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return MemberKind.Double;
        if (type == typeof(string)) return MemberKind.String;
        if (type == typeof(NoteDocument)) return MemberKind.Note;
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return MemberKind.DateTime;
        if (type == typeof(bool)) return MemberKind.Boolean;
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string)) return MemberKind.Collection;
        if (!type.IsPrimitive && !type.IsValueType && type != typeof(string)) return MemberKind.Class;

        return MemberKind.Unknown;
    }

    private static PropertyInfo? ResolveValueSourceProperty(Type targetType, string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        var property = targetType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property is null)
        {
            return null;
        }

        if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            return null;
        }

        return property;
    }
}
