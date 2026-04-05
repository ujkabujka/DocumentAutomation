using BaseFramework.Core.Metadata;

namespace BaseFramework.Core.Services;

public interface IObjectMetadataProvider
{
    InspectableTypeMetadata GetMetadata(Type targetType);
}
