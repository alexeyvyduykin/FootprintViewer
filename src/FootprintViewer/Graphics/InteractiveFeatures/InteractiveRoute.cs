using Mapsui.Geometries;
using Mapsui.Providers;
//using NetTopologySuite.GeometriesGraph;
using System.Collections.Generic;

namespace FootprintViewer
{
    public class InteractiveRoute : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Feature _helpLineString;

        public override AddInfo BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                return new AddInfo();
            }

            _isDrawing = true;

            var p0 = worldPosition.Clone();
            // Add a second point right away. The second one will be the 'hover' vertex
            var p1 = worldPosition.Clone();

            Geometry = new LineString(new[] { p0 });

            this["Name"] = FeatureType.Route.ToString();

            _helpLineString = new Feature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = FeatureType.RouteDrawing.ToString(),
            };

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>() { _helpLineString },
            };
        }

        public override void Drawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_helpLineString.Geometry).EndPoint;
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();

                ((LineString)Geometry).Vertices.Add(p0); // and add it to the geometry
                ((LineString)_helpLineString.Geometry).Vertices = new[] { p1, p2 };

                RenderedGeometry?.Clear();
                _helpLineString.RenderedGeometry?.Clear();
            }
        }

        public override void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_helpLineString.Geometry).EndPoint.X = worldPosition.X;
                ((LineString)_helpLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _helpLineString.RenderedGeometry?.Clear();
            }
        }

        public override void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                return ((LineString)Geometry).Vertices;
            }

            return new List<Point>();
        }
    }
}
