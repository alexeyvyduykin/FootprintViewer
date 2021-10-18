using Mapsui.Styles;
using System.Collections.Generic;

namespace FootprintViewer
{
    public static class FeatureStyles
    {
        private static readonly Dictionary<string, VectorStyle> _dict;

        static FeatureStyles()
        {
            _dict = new Dictionary<string, VectorStyle>()
            {
                { FeatureType.Route.ToString(), RouteStyle },
                { FeatureType.RouteDrawing.ToString(), RouteDrawingStyle },
                { FeatureType.AOIRectangle.ToString(), AOIRectangleStyle },
                { FeatureType.AOIRectangleDrawing.ToString(), AOIRectangleDrawingStyle },
                { FeatureType.AOIPolygon.ToString(), AOIPolygonStyle },
                { FeatureType.AOIPolygonBorderDrawing.ToString(), AOIPolygonBorderStyle },
                { FeatureType.AOIPolygonAreaDrawing.ToString(), AOIPolygonAreaStyle },
                { FeatureType.AOIPolygonDrawing.ToString(), AOIPolygonDrawingStyle },
                { FeatureType.AOICircle.ToString(), AOICircleStyle },
                { FeatureType.AOICircleDrawing.ToString(), AOICircleDrawingStyle },
            };
        }

        public static VectorStyle? Get(string feature)
        {
            return _dict.TryGetValue(feature, out var style) ? style : null;
        }

        private static readonly VectorStyle RouteStyle = new VectorStyle
        {
            Fill = null,
            Line = new Pen(Color.Red, 8)
        };

        private static readonly VectorStyle RouteDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = new Pen(Color.Red, 8)
            {
                PenStyle = PenStyle.Dash,
            },
        };

        private static readonly VectorStyle AOIRectangleStyle = new VectorStyle
        {
            Fill = null,
            Outline = new Pen(Color.Orange, 4),
        };


        private static readonly VectorStyle AOIRectangleDrawingStyle = new VectorStyle
        {
            Fill = new Brush(Color.Opacity(Color.Orange, 0.3f)),
            Outline = new Pen(Color.Orange, 4),
        };

        private static readonly VectorStyle AOIPolygonStyle = new VectorStyle
        {
            Fill = null,
            Line = new Pen(Color.Green, 4),
            Outline = new Pen(Color.Green, 4),
        };

        private static readonly VectorStyle AOIPolygonBorderStyle = new VectorStyle
        {
            Fill = null,
            Line = new Pen(Color.Green, 4),
        };

        private static readonly VectorStyle AOIPolygonAreaStyle = new VectorStyle
        {
            Fill = new Brush(Color.Opacity(Color.Green, 0.3f)),
            Line = null,
            Outline = null,
        };

        private static readonly VectorStyle AOIPolygonDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = new Pen(Color.Green, 4)
            {
                PenStyle = PenStyle.Dot,
            },
        };

        private static readonly VectorStyle AOICircleStyle = new VectorStyle
        {
            Fill = null,
            Outline = new Pen(Color.Gray, 4),
        };


        private static readonly VectorStyle AOICircleDrawingStyle = new VectorStyle
        {
            Fill = new Brush(Color.Opacity(Color.Gray, 0.3f)),
            Outline = new Pen(Color.Gray, 4),
        };
    }
}
