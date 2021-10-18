using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer
{
    public class InteractiveRectangle : InteractiveFeature
    {
        private bool _isDrawing = false;

        public override AddInfo BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                var p0 = worldPosition.Clone();
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();
                var p3 = worldPosition.Clone();

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0, p1, p2, p3 })
                };

                this["Name"] = FeatureType.AOIRectangleDrawing.ToString();
            }

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>(),
            };
        }

        public override void Drawing(Point worldPosition)
        {
            EndDrawing();
        }

        public override void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p2 = worldPosition.Clone();
                var rectangle = (Polygon)Geometry;
                var p0 = rectangle.ExteriorRing.Vertices[0];

                var p1 = new Point(p2.X, p0.Y);
                var p3 = new Point(p0.X, p2.Y);

                ((Polygon)Geometry).ExteriorRing.Vertices[0] = p0;
                ((Polygon)Geometry).ExteriorRing.Vertices[1] = p1;
                ((Polygon)Geometry).ExteriorRing.Vertices[2] = p2;
                ((Polygon)Geometry).ExteriorRing.Vertices[3] = p3;

                RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public override void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var vertices = ((Polygon)Geometry).ExteriorRing.Vertices;

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                this["Name"] = FeatureType.AOIRectangle.ToString();

                RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null && _isDrawing == false)
            {
                return ((Polygon)Geometry).ExteriorRing.Vertices;
            }

            return new List<Point>();
        }
    }
}
