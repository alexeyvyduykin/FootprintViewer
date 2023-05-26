using FootprintViewer.Factories;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FootprintViewer.Models;

public class AreaOfInterest
{
    private readonly Map _map;
    private readonly Subject<Geometry?> _callbackSubj = new();

    public AreaOfInterest(Map map)
    {
        _map = map;
    }

    public IObservable<Geometry?> Changed => _callbackSubj.AsObservable();

    public void Update(GeometryFeature feature, FeatureType type)
    {
        var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

        switch (type)
        {
            case FeatureType.AOIRectangle:
                layer?.AddAOI(new InteractivePolygon(feature), nameof(FeatureType.AOIRectangle));
                break;
            case FeatureType.AOIPolygon:
                layer?.AddAOI(new InteractivePolygon(feature), nameof(FeatureType.AOIPolygon));
                break;
            case FeatureType.AOICircle:
                layer?.AddAOI(new InteractiveCircle(feature), nameof(FeatureType.AOICircle));
                break;
            default:
                throw new Exception();
        }

        _callbackSubj.OnNext(feature.Geometry);
    }

    public void Reset()
    {
        var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

        layer?.ResetAOI();

        _callbackSubj.OnNext(null);

        return;
    }
}
