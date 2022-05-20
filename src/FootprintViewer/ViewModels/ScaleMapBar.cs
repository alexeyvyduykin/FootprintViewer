using Mapsui;
using Mapsui.Projections;
using Mapsui.Widgets.ScaleBar;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace FootprintViewer.ViewModels
{
    public class ScaleMapBar : ReactiveObject
    {
        private static readonly double MaxWidthScaleBar = 100;

        public static (double lon, double lat) ChangedPosition(double X, double Y, IReadOnlyViewport viewport)
        {
            var worldPosition = viewport.ScreenToWorld(X, Y);
            return SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
        }

        public static (double, string, double) ChangedViewport(IReadOnlyViewport viewport)
        {
            var center0 = viewport.Center;

            var (_, lat) = SphericalMercator.ToLonLat(center0.X, center0.Y);

            double groundResolution0 = viewport.Resolution * Math.Cos(lat / 180.0 * Math.PI);

            var resolution = groundResolution0 * 96 / 0.0254;

            var unitConverter = MetricUnitConverter.Instance;

            var center = new MPoint(viewport.Center.X, viewport.Center.Y);

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

            return (resolution, scaleBarText, scaleBarLength);
        }

        [Reactive]
        public MPoint? Position { get; set; }

        [Reactive]
        public double Resolution { get; set; }

        [Reactive]
        public string? Scale { get; set; }

        [Reactive]
        public double ScaleLength { get; set; }
    }
}
