using Mapsui.Styles;
using System.Collections.Generic;

namespace FootprintViewer
{
    public static class FeatureStyles
    {
        private static readonly Dictionary<string, VectorStyle> _dict;
        private static readonly Color _color = new Color { R = 76, G = 154, B = 231, A = 255 };
        private static readonly Pen _pen = new Pen(_color, 3);
        private static readonly Pen _penDot = new Pen(_color, 4) { PenStyle = PenStyle.Dot };
        private static readonly Brush _brush = new Brush(Color.Opacity(_color, 0.25f));
        private static readonly Color _darkColor = new Color { R = 67, G = 135, B = 202, A = 255 };
        private static readonly Pen _darkPen = new Pen(_darkColor, 3);
        private static readonly Pen _darkPenDash = new Pen(_darkColor, 3) { PenStyle = PenStyle.Dash };

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
            Line = _darkPen
        };

        private static readonly VectorStyle AOIRectangleStyle = new VectorStyle
        {
            Fill = null,
            Outline = _pen,
        };

        private static readonly VectorStyle AOIPolygonStyle = new VectorStyle
        {
            Fill = null,
            Line = _pen,
            Outline = _pen,
        };

        private static readonly VectorStyle AOICircleStyle = new VectorStyle
        {
            Fill = null,
            Outline = _pen,
        };

        private static readonly VectorStyle RouteDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = _darkPenDash,
        };

        private static readonly VectorStyle AOIRectangleDrawingStyle = new VectorStyle
        {
            Fill = _brush,
            Outline = _pen,
        };

        private static readonly VectorStyle AOIPolygonBorderStyle = new VectorStyle
        {
            Fill = null,
            Line = _pen,
        };

        private static readonly VectorStyle AOIPolygonAreaStyle = new VectorStyle
        {
            Fill = _brush,
            Line = null,
            Outline = null,
        };

        private static readonly VectorStyle AOIPolygonDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = _penDot,
        };

        private static readonly VectorStyle AOICircleDrawingStyle = new VectorStyle
        {
            Fill = _brush,
            Outline = _pen,
        };
    }
}
