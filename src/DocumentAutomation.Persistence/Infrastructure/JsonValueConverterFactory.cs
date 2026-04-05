using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DocumentAutomation.Persistence.Infrastructure;

internal static class JsonValueConverterFactory
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static void ApplyJsonConversion<TProperty>(
        PropertyBuilder<TProperty> propertyBuilder,
        Func<TProperty> emptyFactory)
        where TProperty : class
    {
        var converter = new ValueConverter<TProperty, string>(
            value => JsonSerializer.Serialize(value, JsonOptions),
            value => JsonSerializer.Deserialize<TProperty>(value, JsonOptions) ?? emptyFactory());

        var comparer = new ValueComparer<TProperty>(
            (left, right) => JsonSerializer.Serialize(left, JsonOptions) == JsonSerializer.Serialize(right, JsonOptions),
            value => JsonSerializer.Serialize(value, JsonOptions).GetHashCode(StringComparison.Ordinal),
            value => JsonSerializer.Deserialize<TProperty>(JsonSerializer.Serialize(value, JsonOptions), JsonOptions) ?? emptyFactory());

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        propertyBuilder.HasColumnType("jsonb");
    }
}
