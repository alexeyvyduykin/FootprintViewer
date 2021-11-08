using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using System.Reactive.Linq;
using System.Threading;
using DynamicData;
using Mapsui.Providers;
using Mapsui.Geometries.Utilities;
using Mapsui.Geometries;
//sing NetTopologySuite.Geometries;

namespace FootprintViewer.ViewModels
{
    public class Sensor : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; } = string.Empty;

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class SceneSearchFilter : ReactiveObject
    {
        private readonly IObservable<Func<Footprint, bool>> _observableFilter;


        public event EventHandler? Update;

        public SceneSearchFilter()
        {
            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;

            this.WhenAnyValue(s => s.AOI).Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));

            this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation).
                Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));

            //_observableFilter = 
            //    this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation).
            //    Select(_ => MakeFilter());
        }

        public void ForceUpdate()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }

        public void AddSensors(IEnumerable<string> sensors)
        {
            Sensors.Clear();

            foreach (var item in sensors)
            {
                Sensors.Add(new Sensor() { Name = item });
            }

            var databasesValid = Sensors
                .ToObservableChangeSet()
                .AutoRefresh(model => model.IsActive)
                .Subscribe(s => 
                {
                    var temp = Cloudiness;
                    Cloudiness = temp + 1;
                    Cloudiness = temp;
                });

            // HACK: call observable
            var temp = Cloudiness;
            Cloudiness = temp + 1;
            Cloudiness = temp;
        }

        public bool Filtering(Footprint footprint)
        {
            bool isAoiCondition = false; 

            if (AOI == null)
            {
                isAoiCondition = true;
            }
            else
            {
                var footprintPolygon = (Polygon)footprint.Geometry;
                var aoiPolygon = (Polygon)AOI;
                                    
                isAoiCondition = PolygonContain(aoiPolygon, footprintPolygon);                              
            }

            if (isAoiCondition == true)
            {
                if (Sensors.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
                {
                    if (footprint.CloudCoverFull >= Cloudiness)
                    {
                        if (footprint.SunElevation >= MinSunElevation && footprint.SunElevation <= MaxSunElevation)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool PolygonContain(Polygon parent, Polygon child)
        {
            var points1 = parent.ExteriorRing.Vertices;
            var points2 = child.ExteriorRing.Vertices;

            bool fullContain = false;

            foreach (var item in points2)
            {
                if (parent.Contains(item) == false)
                {
                    fullContain = false;
                    break;
                }

                fullContain = true;
            }

            if (fullContain == true)
            {
                return true;
            }


            IList<(Point, Point)> lines1 = new List<(Point, Point)>();
            IList<(Point, Point)> lines2 = new List<(Point, Point)>();

            for (int i = 0; i < points1.Count; i++)
            {
                if (i == points1.Count - 1)
                {
                    lines1.Add((points1[i], points1[0]));
                }
                else
                {
                    lines1.Add((points1[i], points1[i + 1]));
                }
            }

            for (int i = 0; i < points2.Count; i++)
            {
                if (i == points2.Count - 1)
                {
                    lines2.Add((points2[i], points2[0]));
                }
                else
                {
                    lines2.Add((points2[i], points2[i + 1]));
                }
            }

            foreach (var (p1, p2) in lines1)
            {
                foreach (var (q1, q2) in lines2)
                {
                    var res = doIntersect(p1, p2, q1, q2);

                    if (res == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public IObservable<Func<Footprint, bool>> Observable => _observableFilter;

        private Func<Footprint, bool> MakeFilter()
        {
            return footprint => 
            {
                if (Sensors.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
                {                   
                    if (footprint.CloudCoverFull >= Cloudiness)
                    {
                        if (footprint.SunElevation >= MinSunElevation && footprint.SunElevation <= MaxSunElevation)
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
        }


        // Given three collinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        static bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        static int orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
                return 0; // collinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        static bool doIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1))
                return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1))
                return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2))
                return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2))
                return true;

            return false; // Doesn't fall in any of the above cases
        }


        private static Point? Intersection(Point p1, Point p2, Point q1, Point q2)
        {
            // compute midpoint of "kernel envelope"
            double minX0 = p1.X < p2.X ? p1.X : p2.X;
            double minY0 = p1.Y < p2.Y ? p1.Y : p2.Y;
            double maxX0 = p1.X > p2.X ? p1.X : p2.X;
            double maxY0 = p1.Y > p2.Y ? p1.Y : p2.Y;

            double minX1 = q1.X < q2.X ? q1.X : q2.X;
            double minY1 = q1.Y < q2.Y ? q1.Y : q2.Y;
            double maxX1 = q1.X > q2.X ? q1.X : q2.X;
            double maxY1 = q1.Y > q2.Y ? q1.Y : q2.Y;

            double intMinX = minX0 > minX1 ? minX0 : minX1;
            double intMaxX = maxX0 < maxX1 ? maxX0 : maxX1;
            double intMinY = minY0 > minY1 ? minY0 : minY1;
            double intMaxY = maxY0 < maxY1 ? maxY0 : maxY1;

            double midx = (intMinX + intMaxX) / 2.0;
            double midy = (intMinY + intMaxY) / 2.0;

            // condition ordinate values by subtracting midpoint
            double p1x = p1.X - midx;
            double p1y = p1.Y - midy;
            double p2x = p2.X - midx;
            double p2y = p2.Y - midy;
            double q1x = q1.X - midx;
            double q1y = q1.Y - midy;
            double q2x = q2.X - midx;
            double q2y = q2.Y - midy;

            // unrolled computation using homogeneous coordinates eqn
            double px = p1y - p2y;
            double py = p2x - p1x;
            double pw = p1x * p2y - p2x * p1y;

            double qx = q1y - q2y;
            double qy = q2x - q1x;
            double qw = q1x * q2y - q2x * q1y;

            double x = py * qw - qy * pw;
            double y = qx * pw - px * qw;
            double w = px * qy - qx * py;

            double xInt = x / w;
            double yInt = y / w;

            // check for parallel lines
            if ((double.IsNaN(xInt)) || (double.IsInfinity(xInt)
                || double.IsNaN(yInt)) || (double.IsInfinity(yInt)))
            {
                return null;
            }
            // de-condition intersection point
            return new Point(xInt + midx, yInt + midy);
        }

        [Reactive]
        public DateTime FromDate { get; set; }

        [Reactive]
        public DateTime ToDate { get; set; }

        [Reactive]
        public double Cloudiness { get; set; }

        [Reactive]
        public double MinSunElevation { get; set; }

        [Reactive]
        public double MaxSunElevation { get; set; }

        [Reactive]
        public bool IsFullCoverAOI { get; set; }

        [Reactive]
        public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();

        [Reactive]
        public bool IsAllSensorActive { get; set; }

        [Reactive]
        public IGeometry? AOI { get; set; }
    }

    public class SceneSearchFilterDesigner : SceneSearchFilter
    {
        public SceneSearchFilterDesigner()
        {
            var sensor1 = new Sensor() { Name = "Satellite1 SNS-1" };
            var sensor2 = new Sensor() { Name = "Satellite1 SNS-2" };
            var sensor3 = new Sensor() { Name = "Satellite2 SNS-1" };
            var sensor4 = new Sensor() { Name = "Satellite3 SNS-1" };
            var sensor5 = new Sensor() { Name = "Satellite3 SNS-2" };

            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

            Sensors = new ObservableCollection<Sensor>(new[] { sensor1, sensor2, sensor3, sensor4, sensor5 });
        }
    }
}
