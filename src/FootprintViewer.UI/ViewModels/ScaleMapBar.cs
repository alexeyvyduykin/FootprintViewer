using Mapsui;
using Mapsui.Projections;
using Mapsui.Widgets.ScaleBar;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels;

public sealed class ScaleMapBar : ViewModelBase
{
    private static readonly double MaxWidthScaleBar = 100;
    private readonly ObservableAsPropertyHelper<MPoint?> _position;

    public ScaleMapBar()
    {
        this.WhenAnyValue(s => s.Position2)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.Position, out _position);
    }

    public void ChangedViewport(Viewport viewport)
    {
        Viewport = viewport;

        var cx = viewport.CenterX;
        var cy = viewport.CenterY;

        var (_, lat) = SphericalMercator.ToLonLat(cx, cy);

        double groundResolution0 = viewport.Resolution * Math.Cos(lat / 180.0 * Math.PI);

        var resolution = groundResolution0 * 96 / 0.0254;

        var unitConverter = MetricUnitConverter.Instance;

        var center = new MPoint(viewport.CenterX, viewport.CenterY);

        var proj = new Projection();

        var (_, y) = proj.Project("EPSG:3857", "EPSG:4326", center.X, center.Y);

        // Calc ground resolution in meters per pixel of viewport for this latitude
        double groundResolution = viewport.Resolution * Math.Cos(y / 180.0 * Math.PI);

        // Convert in units of UnitConverter
        groundResolution /= unitConverter.MeterRatio;

        var scaleBarValues = unitConverter.ScaleBarValues;

        double scaleBarLength = 0;

        int scaleBarValue = 0;

        foreach (int value in scaleBarValues)
        {
            scaleBarValue = value;

            scaleBarLength = (float)(scaleBarValue / groundResolution);

            if (scaleBarLength < MaxWidthScaleBar - 10)
            {
                break;
            }
        }

        var scaleBarText = unitConverter.GetScaleText(scaleBarValue);

        Resolution = resolution;
        Scale = scaleBarText;
        ScaleLength = scaleBarLength;
    }

    public void ChangedPosition(MPoint worldPosition)
    {
        var (lon, lat) = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        Position2 = new MPoint(lon, lat);
    }

    public Viewport? Viewport { get; set; }

    [Reactive]
    private MPoint? Position2 { get; set; }

    public MPoint? Position => _position.Value;

    [Reactive]
    public double Resolution { get; set; }

    [Reactive]
    public string? Scale { get; set; }

    [Reactive]
    public double ScaleLength { get; set; }
}
