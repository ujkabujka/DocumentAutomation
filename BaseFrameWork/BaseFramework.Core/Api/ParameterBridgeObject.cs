using System.Reflection;
using BaseFramework.Core.Attributes;

namespace BaseFramework.Core.Api;

public abstract class ParameterBridgeObject : ObservableObject
{
    public IReadOnlyDictionary<string, object?> GetParameters()
    {
        var values = new Dictionary<string, object?>();

        foreach (var property in GetInspectableProperties())
        {
            values[property.Name] = property.GetValue(this);
        }

        return values;
    }

    public void SetParameters(IReadOnlyDictionary<string, object?> values)
    {
        var properties = GetInspectableProperties()
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var pair in values)
        {
            if (!properties.TryGetValue(pair.Key, out var property))
            {
                continue;
            }

            property.SetValue(this, pair.Value);
        }
    }

    private IEnumerable<PropertyInfo> GetInspectableProperties()
        => GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<InspectableMemberAttribute>() is not null);
}
