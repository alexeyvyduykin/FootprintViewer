using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.InteractivityEx
{
    public class InteractiveRoute : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Feature _helpLineString;

        private bool _isEditing = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        protected InteractiveRoute() : base() { }

        public static InteractiveRoute Build()
        {
            return new InteractiveRoute();
        }

        public override bool IsEndDrawing(Point worldPosition, Predicate<Point> isEnd)
        {
            var routeGeometry = (LineString)Geometry;

            if (routeGeometry.Vertices.Count > 1)
            {         
                foreach (var item in routeGeometry.Vertices)
                {
                    var click = isEnd.Invoke(item);              
                    if (click == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

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

        public override bool BeginEditing(Point worldPosition, double screenDistance)
        {
            if (_isEditing == true)
            {
                return false;
            }

            var vertices = EditVertices();

            var vertexTouched = vertices.OrderBy(v => v.Distance(worldPosition)).FirstOrDefault(v => v.Distance(worldPosition) < screenDistance);
            
            if (vertexTouched != null)
            {
                _vertex = vertexTouched;
                _startOffsetToVertex = worldPosition - vertexTouched;
                _isEditing = true;

                return true; // to indicate start of drag
            }

            return false;
        }

        public override bool Editing(Point worldPosition)
        {
            if (_isEditing == false)
            {
                return false;
            }

            var position = worldPosition - _startOffsetToVertex;
            
            _vertex.X = position.X;
            _vertex.Y = position.Y;

            RenderedGeometry.Clear();

            return true;
        }

        public override void EndEditing()
        {
            if (_isEditing == true)
            {
                _isEditing = false;
            }
        }
    }
}
