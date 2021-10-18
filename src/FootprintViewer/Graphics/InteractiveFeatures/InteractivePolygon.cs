using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer
{
    public class InteractivePolygon : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Feature _helpLineString;
        private Feature _helpPolygon;

        public InteractivePolygon() : base() { }

        public InteractivePolygon(IFeature feature) : base(feature) { }

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
            this["Name"] = FeatureType.AOIPolygonBorderDrawing.ToString();

            _helpLineString = new Feature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = FeatureType.AOIPolygonDrawing.ToString(),
            };

            _helpPolygon = new Feature
            {
                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0 })
                },
                ["Name"] = FeatureType.AOIPolygonAreaDrawing.ToString(),
            };

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>() { _helpLineString, _helpPolygon },
            };
        }

        public override void Drawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_helpLineString.Geometry).EndPoint;
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();

                ((Polygon)_helpPolygon.Geometry).ExteriorRing.Vertices.Add(p0);
                ((LineString)Geometry).Vertices.Add(p0); // and add it to the geometry
                ((LineString)_helpLineString.Geometry).Vertices = new[] { p1, p2 };

                RenderedGeometry?.Clear();
                _helpLineString.RenderedGeometry?.Clear();
                _helpPolygon.RenderedGeometry?.Clear();
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

                var vertices = ((LineString)Geometry).Vertices;

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                this["Name"] = FeatureType.AOIPolygon.ToString();
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                if (_isDrawing == true)
                {
                    return ((LineString)Geometry).Vertices;
                }
                else
                {
                    return ((Polygon)Geometry).ExteriorRing.Vertices;
                }
            }

            return new List<Point>();
        }
    }
}
