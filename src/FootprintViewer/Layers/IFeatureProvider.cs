using Mapsui;

namespace FootprintViewer.Layers;

public interface IFeatureProvider
{
    IFeature? Find(object? value, string fieldName);
}
