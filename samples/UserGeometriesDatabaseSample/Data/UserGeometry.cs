using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using ReactiveUI;

namespace UserGeometriesDatabaseSample.Data
{
    public enum UserGeometryType { Point, Rectangle, Polygon, Circle }

    public class UserGeometry : ReactiveObject
    {
        public string? Name { get; set; }

        public UserGeometryType Type { get; set; }

        public Geometry? Geometry { get; set; }
    }
}
