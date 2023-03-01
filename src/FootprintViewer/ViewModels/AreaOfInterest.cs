using FootprintViewer.Factories;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System;

namespace FootprintViewer.ViewModels;

public class AreaOfInterest
{
    private readonly Map _map;

    public AreaOfInterest(Map map)
    {
        AOIChanged = ReactiveCommand.Create<Geometry?, Geometry?>(s => s, outputScheduler: RxApp.MainThreadScheduler);

        _map = map;
    }

    public void Update(GeometryFeature? feature, FeatureType? type = null)
    {
        var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

        if (feature == null || type == null)
        {
            layer?.ResetAOI();

            AOIChanged.Execute(null).Subscribe();

            return;
        }

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

        AOIChanged.Execute(feature.Geometry).Subscribe();
    }

    public ReactiveCommand<Geometry?, Geometry?> AOIChanged { get; }
}
