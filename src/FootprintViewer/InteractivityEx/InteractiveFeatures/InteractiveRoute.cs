﻿using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.InteractivityEx
{
    public class InteractiveRoute : InteractiveFeature
    {
        private bool _isEditing = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        public InteractiveRoute(IFeature feature) : base(feature) { }

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
