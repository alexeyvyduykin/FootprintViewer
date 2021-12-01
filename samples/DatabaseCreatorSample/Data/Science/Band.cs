using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DatabaseCreatorSample.Data
{
        //public enum BandMode
        //{
        //    Middle,
        //    Left,
        //    Right
        //}

        public class Band
        {
            public Band(Orbit orbit, double gam1DEG, double gam2DEG, BandMode mode)
            {
                this.Orbit = orbit;

                TrackPointDirection[] dir = new TrackPointDirection[2];
                switch (mode)
                {
                    case BandMode.Middle:
                        dir[0] = TrackPointDirection.Left;
                        dir[1] = TrackPointDirection.Right;
                        break;
                    case BandMode.Left:
                        dir[0] = dir[1] = TrackPointDirection.Left;
                        break;
                    case BandMode.Right:
                        dir[0] = dir[1] = TrackPointDirection.Right;
                        break;
                }

                FactorShiftTrack factor = new FactorShiftTrack(orbit, gam1DEG, gam2DEG, mode);

                NearLine = new FactorTrack(new CustomTrack(orbit, gam1DEG, dir[0]), factor);
                FarLine = new FactorTrack(new CustomTrack(orbit, gam2DEG, dir[1]), factor);
            }

            public Band(Orbit orbit, double verticalHalfAngleDEG, double rollAngleDEG)
            {
                this.Orbit = orbit;

                BandMode mode;

                if (rollAngleDEG == 0)
                    mode = BandMode.Middle;
                else if (rollAngleDEG > 0.0)
                    mode = BandMode.Left;
                else
                    mode = BandMode.Right;

                TrackPointDirection[] dir = new TrackPointDirection[2];
                switch (mode)
                {
                    case BandMode.Middle:
                        dir[0] = TrackPointDirection.Left;
                        dir[1] = TrackPointDirection.Right;
                        break;
                    case BandMode.Left:
                        dir[0] = dir[1] = TrackPointDirection.Left;
                        break;
                    case BandMode.Right:
                        dir[0] = dir[1] = TrackPointDirection.Right;
                        break;
                }

                double gam1DEG = Math.Abs(rollAngleDEG) - verticalHalfAngleDEG;
                double gam2DEG = Math.Abs(rollAngleDEG) + verticalHalfAngleDEG;

                FactorShiftTrack factor = new FactorShiftTrack(orbit, gam1DEG, gam2DEG, mode);

                NearLine = new FactorTrack(new CustomTrack(orbit, gam1DEG, dir[0]), factor);
                FarLine = new FactorTrack(new CustomTrack(orbit, gam2DEG, dir[1]), factor);
            }

            public bool IsCoverPolis(double latRAD, ref double timeFromANToPolis)
            {
                double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
                if (NearLine.polisMod(latRAD, ref angleToPolis1) == true &&
                    FarLine.polisMod(latRAD, ref angleToPolis2) == true)
{
                    if (ScienceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
                    {
                        if (latRAD >= 0.0)
                            timeFromANToPolis = Orbit.Quart1;
                        else
                            timeFromANToPolis = Orbit.Quart3;
                        return true;
                    }
                }
                return false;
            }

            public bool IsCoverPolis(double latRAD)
            {
                double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
                if (NearLine.polisMod(latRAD, ref angleToPolis1) == true &&
                    FarLine.polisMod(latRAD, ref angleToPolis2) == true)
{
                    if (ScienceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
                        return true;
                }
                return false;
            }

            public Orbit Orbit { get; }

        public IList<Geo2D> GetNearGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D> converter = null)
        {                
            CustomTrack track1 = new CustomTrack(Orbit, NearLine.Alpha1 * ScienceMath.RadiansToDegrees, NearLine.Direction);                
            return GetGroundTrack(track1, satellite, node, converter);            
        }

            public IList<Geo2D> GetFarGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D> converter = null)
{
                CustomTrack track2 = new CustomTrack(Orbit, FarLine.Alpha1 * ScienceMath.RadiansToDegrees, FarLine.Direction);
                return GetGroundTrack(track2, satellite, node, converter);
            }

        private IList<Geo2D> GetGroundTrack(CustomTrack track, PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D> converter = null)
        {
            converter = converter ?? ScienceConverters.From0To360;

            var points = new List<Geo2D>();

            var nodes = satellite.Nodes();
            for (int q = 0; q < nodes[node].Quarts.Count; q++)
            {
                for (double t = nodes[node].Quarts[q].TimeBegin; t <= nodes[node].Quarts[q].TimeEnd; t += 5.0)
                {
                    var point = track.ContinuousTrack(node, t, satellite.TrueTimePastAN, nodes[node].Quarts[q].Quart);
                    points.Add(converter.Invoke(point));
                }
            }
            return points;
        }

        public static void ToFile(string path, PRDCTSatellite satellite, PRDCTSensor sensor, int node)
            {
                Band b1 = new Band(satellite.Orbit, -Math.Abs(sensor.VerticalHalfAngleDEG), sensor.RollAngleDEG);
                Band b2 = new Band(satellite.Orbit, Math.Abs(sensor.VerticalHalfAngleDEG), sensor.RollAngleDEG);

                var near1 = b1.GetNearGroundTrack(satellite, node).ToList();
                var far1 = b1.GetFarGroundTrack(satellite, node).ToList();

                near1.ForEach(s => { Geo2D.OffsetLeftLon(s); });
                far1.ForEach(s => { Geo2D.OffsetLeftLon(s); });

                var near2 = b2.GetNearGroundTrack(satellite, node).ToList();
                var far2 = b2.GetFarGroundTrack(satellite, node).ToList();

                near2.ForEach(s => { Geo2D.OffsetLeftLon(s); });
                far2.ForEach(s => { Geo2D.OffsetLeftLon(s); });

                var temp = System.Globalization.CultureInfo.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.WriteLine("BEGIN OBJECT");
                    writer.WriteLine("BEGIN DATA");

                    for (int i = 0; i < near1.Count; i++)
                    {
                        writer.WriteLine("{0:0.00000000} {1:0.00000000} {2:0.00000000} {3:0.00000000} {4:0.00000000} {5:0.00000000} {6:0.00000000} {7:0.00000000}",
                            far1[i].Lon,
                            far1[i].Lat,
                            near1[i].Lon,
                            near1[i].Lat,
                            near2[i].Lon,
                            near2[i].Lat,
                            far2[i].Lon,
                            far2[i].Lat);
                    }

                    writer.WriteLine("END");
                    writer.WriteLine("END");
                }

                System.Threading.Thread.CurrentThread.CurrentCulture = temp;
            }

            //public List<List<Geo2D>> To2D(PRDCTSatellite satellite, int node, bool extrude = false, bool clockwise = true)
            //{
            //    var near = GetNearGroundTrack(satellite, node).ToList();
            //    var far = GetFarGroundTrack(satellite, node).ToList();

            //    near.ForEach(s => { Geo2D.OffsetLeftLon(s); });
            //    far.ForEach(s => { Geo2D.OffsetLeftLon(s); });


            //    BandCore2D engine2D = new BandCore2D(near, far, IsCoverPolis);

            //    engine2D.ExtrudeMode = extrude;
            //    engine2D.CreateShapes(clockwise, out List<List<Geo2D>> shapes);

            //    return shapes;
            //}

            public FactorTrack NearLine { get; private set; }
            public FactorTrack FarLine { get; private set; }
        }




        //    internal class FixPoint
        //    {
        //        public FixPoint(Geo2D point, EType type, BandSegment segment)
        //        {
        //            this.Fix = point;
        //            this.Type = type;
        //            this.Segment = segment;

        //            id = ++classCounter;
        //        }

        //        public enum EType { Begin, Left, Right, End }

        //        public EType Type { get; }

        //        public Geo2D Fix { get; }

        //        public List<Geo2D> DataAsBegin
        //        {
        //            get
        //            {
        //                if (Segment.Begin.Equals(Fix))
        //                {
        //                    return Segment.NewData;
        //                }
        //                else if (Segment.End.Equals(Fix))
        //                {
        //                    return Segment.NewReverseData;
        //                }
        //                else
        //                {
        //                    throw new Exception();
        //                }
        //            }
        //        }

        //        public FixPoint FixPointBounded
        //        {
        //            get
        //            {
        //                if (Segment.FixPointBegin == this)
        //                {
        //                    return Segment.FixPointEnd;
        //                }
        //                else if (Segment.FixPointEnd == this)
        //                {
        //                    return Segment.FixPointBegin;
        //                }

        //                return null;                
        //            }
        //        }


        //        public FixPoint ToSegmentPoint(out List<Geo2D> data)
        //        {
        //            data = DataAsBegin;
        //            return FixPointBounded;
        //        }

        //        static int classCounter = 0;

        //        int id;

        //        public override string ToString()
        //        {
        //            return string.Format("FixPoint {0:00}({1},Lat={2:0,0})(Segm: {3})", id, Enum.GetName(typeof(EType), Type), Fix.Lat * MyMath.RadiansToDegrees, Segment);
        //        }

        //        public string ShortDescription()
        //        {
        //            return string.Format("FixPoint {0:00}", id);
        //        }

        //        public void DeleteSegment()
        //        {
        //            Segment = null;
        //        }

        //        public BandSegment Segment { get; private set; }
        //    }

        //    internal class BandSegment
        //    {
        //        #region Default Segments

        //        public static BandSegment Top =
        //            new BandSegment(new List<Geo2D>() { new Geo2D(-Math.PI, Math.PI / 2.0), new Geo2D(Math.PI, Math.PI / 2.0) });                   
        //        public static BandSegment TopReverse = 
        //            new BandSegment(new List<Geo2D>() { new Geo2D(Math.PI, Math.PI / 2.0), new Geo2D(-Math.PI, Math.PI / 2.0) });
        //        public static BandSegment Bottom = 
        //            new BandSegment(new List<Geo2D>() { new Geo2D(-Math.PI, -Math.PI / 2.0), new Geo2D(Math.PI, -Math.PI / 2.0) });
        //        public static BandSegment BottomReverse =
        //            new BandSegment(new List<Geo2D>() { new Geo2D(Math.PI, -Math.PI / 2.0), new Geo2D(-Math.PI, -Math.PI / 2.0) });

        //        #endregion

        //        public BandSegment(IList<Geo2D> arr, Segment eseg) : this()
        //        {                
        //            this.Seg = eseg;

        //            this.data = new List<Geo2D>(arr);
        //        }

        //        protected BandSegment(IList<Geo2D> arr) : this()
        //        {
        //            this.Seg = Segment.Cut;

        //            this.data = new List<Geo2D>(arr);
        //        }

        //        protected BandSegment() { id = ++ClassCounter; }

        //        public enum Segment { Begin, Cut, End };    
        //        public Segment Seg { get; set; }

        //        public int Length { get { return data.Count; } }

        //        public Geo2D Begin
        //        {
        //            get
        //            {
        //                return data.First();
        //            }
        //        }

        //        public Geo2D End
        //        {
        //            get
        //            {
        //                return data.Last();
        //            }
        //        }

        //        static int ClassCounter = 0;

        //        int id;

        //        public override string ToString()
        //        {
        //            return string.Format("Segment {0:00}", id);
        //        }

        //        FixPoint _fixPointBegin, _fixPointEnd;

        //        public FixPoint FixPointBegin
        //        {
        //            get
        //            {
        //                return _fixPointBegin;
        //            }
        //            set
        //            {
        //                _fixPointBegin = value;
        //            }
        //        }

        //        public FixPoint FixPointEnd
        //        {
        //            get
        //            {
        //                return _fixPointEnd;
        //            }
        //            set
        //            {
        //                _fixPointEnd = value;
        //            }
        //        }

        //        public List<Geo2D> NewData { get { return new List<Geo2D>(data); } }
        //        public List<Geo2D> NewReverseData { get { var temp = new List<Geo2D>(data); temp.Reverse(); return temp; } }

        //        List<Geo2D> data;
        //    }

        //    internal class BandCore2D
        //    {
        //        public BandCore2D(List<BandSegment> bs)
        //        {
        //            this.Segments = bs;

        //            this.LeftPoints = new List<FixPoint>();
        //            {
        //                foreach (var item in bs)
        //                {
        //                    if (item.Begin.Lon.Equals(-Math.PI))
        //                    {
        //                        item.FixPointBegin = new FixPoint(item.Begin, FixPoint.EType.Left, item);
        //                        this.LeftPoints.Add(item.FixPointBegin);
        //                    }
        //                    if (item.End.Lon.Equals(-Math.PI))
        //                    {
        //                        item.FixPointEnd = new FixPoint(item.End, FixPoint.EType.Left, item);
        //                        this.LeftPoints.Add(item.FixPointEnd);
        //                    }
        //                }

        //                this.LeftPoints = this.LeftPoints.OrderBy(s => s.Fix.Lat).ToList();
        //            }

        //            this.RightPoints = new List<FixPoint>();
        //            {
        //                foreach (var item in bs)
        //                {
        //                    if (item.Begin.Lon.Equals(Math.PI))
        //                    {
        //                        item.FixPointBegin = new FixPoint(item.Begin, FixPoint.EType.Right, item);
        //                        this.RightPoints.Add(item.FixPointBegin);
        //                    }
        //                    if (item.End.Lon.Equals(Math.PI))
        //                    {
        //                        item.FixPointEnd = new FixPoint(item.End, FixPoint.EType.Right, item);
        //                        this.RightPoints.Add(item.FixPointEnd);
        //                    }
        //                }

        //                this.RightPoints = this.RightPoints.OrderBy(s => s.Fix.Lat).ToList();
        //            }

        //            this.BeginPoints = new List<FixPoint>();
        //            this.EndPoints = new List<FixPoint>();

        //            foreach (var item in bs.Where(s => s.Seg == BandSegment.Segment.Begin))
        //            {
        //                item.FixPointBegin = new FixPoint(item.Begin, FixPoint.EType.Begin, item);
        //                this.BeginPoints.Add(item.FixPointBegin);
        //            }

        //            foreach (var item in bs.Where(s => s.Seg == BandSegment.Segment.End))
        //            {
        //                item.FixPointEnd = new FixPoint(item.End, FixPoint.EType.End, item);
        //                this.EndPoints.Add(item.FixPointEnd);
        //            }

        //            if (this.BeginPoints.Count != 2 || this.EndPoints.Count != 2)
        //                throw new Exception();

        //#if MYLOG && DEBUG
        //            Debug.WriteLine("===============================================");
        //            Debug.WriteLine("  BandSegments: ({0})", bs.Count);
        //            int i = 0;
        //            foreach (var item in bs)
        //                Debug.WriteLine("    {0} - {1}", i++, item);
        //            Debug.WriteLine("");
        //            Debug.WriteLine("  BeginPoints: ({0})", BeginPoints.Count);
        //            i = 0;
        //            foreach (var item in BeginPoints)
        //                Debug.WriteLine("    {0} - {1}", i++, item);
        //            Debug.WriteLine("");

        //            Debug.WriteLine("  EndPoints: ({0})", EndPoints.Count);

        //            i = 0;
        //            foreach (var item in EndPoints)
        //                Debug.WriteLine("    {0} - {1}", i++, item);
        //            Debug.WriteLine("");

        //            Debug.WriteLine("  LeftPoints: ({0})", LeftPoints.Count);
        //            i = 0;
        //            foreach (var item in LeftPoints)
        //                Debug.WriteLine("    {0} - {1}", i++, item);
        //            Debug.WriteLine("");

        //            Debug.WriteLine("  RightPoints: ({0})", RightPoints.Count);
        //            i = 0;
        //            foreach (var item in RightPoints)
        //                Debug.WriteLine("    {0} - {1}", i++, item);

        //            Debug.WriteLine("===============================================");
        //#endif
        //        }

        //        List<FixPoint> LeftPoints { get; set; }
        //        List<FixPoint> RightPoints { get; set; }

        //        List<FixPoint> BeginPoints { get; set; }
        //        List<FixPoint> EndPoints { get; set; }

        //        List<BandSegment> Segments { get; set; }

        //        // Dictionary<BandSegment, Tuple<FixPoint, FixPoint>> dict;
        //        public static double DefaultExtrudeStep { get; } = 5.0 * Math.PI / 180.0;

        //        public double ExtrudeStep { get; set; } = DefaultExtrudeStep;

        //        public bool ExtrudeMode { get; set; } = false;

        //        public void CreateShapes(bool clockwise, out List<List<Geo2D>> shapes)
        //        {
        //            shapes = new List<List<Geo2D>>();

        //            FixPoint point, nextPoint;

        //            while (GetShape(clockwise, out point))
        //            {
        //                shapes.Add(new List<Geo2D>());

        //                while (NextStep(point, clockwise, out nextPoint, out List<Geo2D> data) == true)
        //                {
        //                    if (data != null)
        //                    {
        //                        if (ExtrudeMode == false)
        //                        {
        //                            shapes.Last().AddRange(data);
        //                        }
        //                        else
        //                        {                                                                            
        //                            if (point.Type == FixPoint.EType.Left || point.Type == FixPoint.EType.Right)
        //                            {
        //                                var first = new Geo2D(data.First());

        //                                int signLat = (data.Count == 2) ? ((first.Lat > 0.0) ? 1 : -1) : 0;
        //                                int signLon = (first.Lon > 0.0) ? 1 : -1;

        //                                shapes.Last().Add(new Geo2D(first.Lon + signLon * ExtrudeStep, first.Lat + signLat * ExtrudeStep));
        //                            }

        //                            shapes.Last().AddRange(data);

        //                            if (nextPoint.Type == FixPoint.EType.Left || nextPoint.Type == FixPoint.EType.Right)
        //                            {
        //                                var last = new Geo2D(data.Last());

        //                                int signLat = (data.Count == 2) ? ((last.Lat > 0.0) ? 1 : -1) : 0;
        //                                int signLon = (last.Lon > 0.0) ? 1 : -1;

        //                                shapes.Last().Add(new Geo2D(last.Lon + signLon * ExtrudeStep, last.Lat + signLat * ExtrudeStep));
        //                            }                                                   
        //                        }
        //                    }

        //                    point = nextPoint;
        //                }
        //            }
        //        }

        //        bool GetShape(bool clockwise, out FixPoint begin)
        //        {
        //            if (LeftPoints.Count != 0)
        //            {
        //                // clockwork wise => First
        //                if (clockwise == true)
        //                    begin = LeftPoints.First();
        //                else
        //                    begin = LeftPoints.Last();
        //                return true;
        //            }

        //            if (RightPoints.Count != 0)
        //            {
        //                // clockwork wise => Lat
        //                if (clockwise == true)
        //                    begin = RightPoints.Last();
        //                else
        //                    begin = RightPoints.First();
        //                return true;
        //            }

        //            if (BeginPoints.Count != 0)
        //            {
        //                // этот вариант возможен при отсутствии соударения с left или right

        //                var point = BeginPoints.First();

        //                // это возможно т.к. метод NextStep, в случае begin или end всегда делает выбор в сторону сегмента, 
        //                // а не соседней точки, если это возможно
        //                if (clockwise == true)
        //                {                    
        //                    begin = point.Segment.FixPointBegin;
        //                }
        //                else
        //                {                  
        //                    begin = point.Segment.FixPointEnd;
        //                }

        //                return true;
        //            }

        //            if (EndPoints.Count != 0)
        //                throw new Exception();

        //            begin = null;
        //            return false;
        //        }


        //        void DeletePoint(FixPoint point)
        //        {
        //            switch (point.Type)
        //            {
        //                case FixPoint.EType.Begin:
        //                    BeginPoints.Remove(point);
        //                    break;
        //                case FixPoint.EType.Left:
        //                    LeftPoints.Remove(point);
        //                    break;
        //                case FixPoint.EType.Right:
        //                    RightPoints.Remove(point);
        //                    break;
        //                case FixPoint.EType.End:
        //                    EndPoints.Remove(point);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            if (point.Segment != null)
        //            {
        //                if (Segments.Contains(point.Segment) == true)
        //                {
        //                    int index = Segments.FindIndex(s=> s.Equals(point.Segment));

        //                    if (Segments[index].FixPointBegin == point)
        //                    {
        //                        Segments[index].FixPointBegin = null;
        //                    }
        //                    else if(Segments[index].FixPointEnd == point)
        //                    {
        //                        Segments[index].FixPointEnd = null;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception();
        //                    }
        //                }
        //            }
        //        }

        //        void DeleteSegment(FixPoint point)
        //        {
        //            Segments.Remove(point.Segment);

        //            if (point.Segment != null)
        //            {
        //                if (point.Segment.FixPointBegin != null)
        //                {
        //                    point.Segment.FixPointBegin.DeleteSegment();
        //                }

        //                if (point.Segment.FixPointEnd != null)
        //                {
        //                    point.Segment.FixPointEnd.DeleteSegment();
        //                }
        //            }
        //        }

        //        bool GetNextFixPoint(FixPoint point, out List<Geo2D> data, out FixPoint fixPoint)
        //        {
        //            if(point == null)
        //            {
        //                data = null;
        //                fixPoint = null;
        //                return false;
        //            }

        //            bool isNext = false;

        //#if MYLOG && DEBUG
        //            Debug.WriteLine("====================================================");
        //            Debug.WriteLine("=================== Next Point =====================");
        //            Debug.WriteLine("");
        //            Debug.WriteLine("  FixPoint = {0}", point);
        //#endif
        //            switch (point.Type)
        //            {
        //                case FixPoint.EType.Begin:
        //                    {
        //                        if (BeginPoints.Count == 2)
        //                        {
        //                            if (BeginPoints.Contains(point) == false)
        //                                throw new Exception();

        //                            bool isFind = Segments.Contains(point.Segment);

        //                            //BeginPoints.Remove(point);

        //                            if (isFind == true)
        //                            {
        //                                Segments.Remove(point.Segment);

        //                                data = point.DataAsBegin;
        //                                fixPoint = point.FixPointBounded;

        //                                point.DeleteSegment();
        //                            }
        //                            else
        //                            {
        //                                if (BeginPoints.Count != 1)
        //                                    throw new Exception();

        //                                data = null;
        //                                fixPoint = BeginPoints[0];
        //                            }

        //                            DeletePoint(point);

        //                            isNext = true;
        //                            break;
        //                        }
        //                        else if (BeginPoints.Count == 1)
        //                        {
        //                            if (point.Segment != null)
        //                                throw new Exception();

        //                            //BeginPoints.Remove(point);
        //                            DeletePoint(point);

        //                            data = null;
        //                            fixPoint = null;

        //                            isNext = false;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            throw new Exception();
        //                        }
        //                    }
        //                case FixPoint.EType.Left:
        //                    {
        //                        if (LeftPoints.Contains(point) == false)
        //                        {
        //                            data = null;
        //                            fixPoint = null;

        //                            isNext = false;
        //                            break;
        //                        }

        //                        var index = LeftPoints.FindIndex(s => s.Equals(point));

        //                        if (Even(index) == true)
        //                            index = index + 1;
        //                        else
        //                            index = index - 1;

        //                        if (index < 0 || index >= LeftPoints.Count)
        //                            throw new Exception();


        //                        var leftFixPoint = LeftPoints[index];



        //                       // LeftPoints.Remove(point);
        //                       // LeftPoints.Remove(leftFixPoint);

        //                        data = leftFixPoint.DataAsBegin;
        //                        fixPoint = leftFixPoint.FixPointBounded;

        //                        DeletePoint(point);
        //                        DeletePoint(leftFixPoint);

        //                        if (fixPoint != null)
        //                        {
        //                            Segments.Remove(fixPoint.Segment);
        //                            fixPoint.DeleteSegment();
        //                        }

        //                        isNext = true;
        //                        break;
        //                    }
        //                case FixPoint.EType.Right:
        //                    {
        //                        if (RightPoints.Contains(point) == false)
        //                            throw new Exception();

        //                        var index = RightPoints.FindIndex(s => s.Equals(point));

        //                        if (Even(index) == true)
        //                            index = index + 1;
        //                        else
        //                            index = index - 1;

        //                        if (index < 0 || index >= RightPoints.Count)
        //                            throw new Exception();

        //                        var rightFixPoint = RightPoints[index];

        //                       // RightPoints.Remove(point);
        //                       // RightPoints.Remove(rightFixPoint);

        //                        data = rightFixPoint.DataAsBegin;
        //                        fixPoint = rightFixPoint.FixPointBounded;

        //                        DeletePoint(point);
        //                        DeletePoint(rightFixPoint);

        //                        if (fixPoint != null)
        //                        {
        //                            Segments.Remove(fixPoint.Segment);
        //                            fixPoint.DeleteSegment();
        //                        }

        //                        isNext = true;
        //                        break;
        //                    }
        //                case FixPoint.EType.End:
        //                    {
        //                        if (EndPoints.Count == 2)
        //                        {
        //                            if (EndPoints.Contains(point) == false)
        //                                throw new Exception();

        //                            bool isFind = Segments.Contains(point.Segment);

        //                            EndPoints.Remove(point);

        //                            if (isFind == true)
        //                            {
        //                                Segments.Remove(point.Segment);

        //                                data = point.DataAsBegin;
        //                                fixPoint = point.FixPointBounded;
        //                            }
        //                            else
        //                            {
        //                                if (EndPoints.Count != 1)
        //                                    throw new Exception();

        //                                data = null;
        //                                fixPoint = EndPoints[0];
        //                            }

        //                            isNext = true;
        //                            break;
        //                        }
        //                        else if (EndPoints.Count == 1)
        //                        {
        //                            if (point.Segment != null)
        //                            {
        //                                bool isFind = Segments.Contains(point.Segment);

        //                               // EndPoints.Remove(point);



        //                                if (isFind == true)
        //                                {
        //                                    Segments.Remove(point.Segment);

        //                                    data = point.DataAsBegin;
        //                                    fixPoint = point.FixPointBounded;

        //                                    point.DeleteSegment();
        //                                }
        //                                else
        //                                {
        //                                    throw new Exception();
        //                                }

        //                                DeletePoint(point);

        //                                isNext = true;
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                DeletePoint(point);
        //                               // EndPoints.Remove(point);

        //                                data = null;
        //                                fixPoint = null;

        //                                isNext = false;
        //                                break;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new Exception();
        //                        }
        //                    }
        //                default:
        //                    throw new Exception();
        //            }


        //#if MYLOG && DEBUG
        //            Debug.WriteLine("  Next Fix Point: {0}", fixPoint);
        //            Debug.WriteLine("");

        //            Debug.WriteLine("  BandSegments: ({0})", Segments.Count);
        //            Debug.Write("    ");
        //            foreach (var item in Segments)
        //                Debug.Write(string.Format("{0} ", item));
        //            Debug.WriteLine("");

        //            Debug.WriteLine("");
        //            Debug.WriteLine("  LeftPoints: ({0})", LeftPoints.Count);
        //            Debug.Write("    ");
        //            foreach (var item in LeftPoints)
        //                Debug.Write(string.Format("{0} ", item.ShortDescription()));
        //            Debug.WriteLine("");

        //            Debug.WriteLine("");
        //            Debug.WriteLine("  RightPoints: ({0})", RightPoints.Count);
        //            Debug.Write("    ");
        //            foreach (var item in RightPoints)
        //                Debug.Write(string.Format("{0} ", item.ShortDescription()));
        //            Debug.WriteLine("");

        //            Debug.WriteLine("");
        //            Debug.WriteLine("====================================================");
        //#endif
        //            return isNext;
        //        }


        //        bool NextStep(FixPoint point, bool clockwise, out FixPoint nextPoint, out List<Geo2D> data)
        //        {
        //            if(point == null)
        //            {
        //                nextPoint = null;

        //                data = null;

        //                return false;
        //            }

        //            switch (point.Type)
        //            {
        //                case FixPoint.EType.Begin:
        //                    {
        //                        // особенность реализации в том, что всегда выбирается вариант движения по сегменту если он имеется
        //                        // для поддержки случая без соударения с границами

        //                        if (point.Segment != null)
        //                        {
        //                            nextPoint = point.ToSegmentPoint(out data);

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, point.Segment);
        //#endif

        //                            DeletePoint(point);
        //                            DeleteSegment(point);

        //                            return true;
        //                        }
        //                        else if (BeginPoints.Count == 2)
        //                        {
        //                            DeletePoint(point);

        //                            nextPoint = BeginPoints.First();

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
        //#endif

        //                            return true;
        //                        }
        //                        else if (BeginPoints.Count == 1)
        //                        {
        //                            DeletePoint(point);

        //                            nextPoint = null;

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, null, null);
        //#endif

        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            throw new Exception();
        //                        }
        //                    }
        //                case FixPoint.EType.Left:
        //                    {                       
        //                        if (Even(LeftPoints.Count) == true)
        //                        {
        //                            int index = LeftPoints.FindIndex(s => s.Equals(point));

        //                            if (clockwise == true)                            
        //                                index += 1;                            
        //                            else                            
        //                                index -= 1;

        //                            nextPoint = LeftPoints[index];

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
        //#endif

        //                            DeletePoint(point);

        //                            return true;
        //                        }
        //                        else
        //                        {
        //                            if(point.Segment != null)
        //                            {
        //                                nextPoint = point.ToSegmentPoint(out data);

        //#if MYLOG && DEBUG 
        //                                Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, point.Segment);
        //#endif
        //                                DeletePoint(point);
        //                                DeleteSegment(point);
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                nextPoint = null;

        //                                data = null;

        //#if MYLOG && DEBUG
        //                                Debug.WriteLine("{0} => {1}, data = {2}", point, null, null);
        //#endif
        //                                DeletePoint(point);

        //                                return false;
        //                            }

        //                        }
        //                    }                 
        //                case FixPoint.EType.Right:
        //                    {
        //                        if (Even(RightPoints.Count) == true)
        //                        {
        //                            int index = RightPoints.FindIndex(s => s.Equals(point));

        //                            if (clockwise == true)
        //                                index -= 1;
        //                            else
        //                                index += 1;

        //                            nextPoint = RightPoints[index];

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
        //#endif
        //                            DeletePoint(point);

        //                            return true;
        //                        }
        //                        else
        //                        {
        //                            if (point.Segment != null)
        //                            {
        //                                nextPoint = point.ToSegmentPoint(out data);

        //#if MYLOG && DEBUG
        //                                Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, point.Segment);
        //#endif
        //                                DeletePoint(point);
        //                                DeleteSegment(point);
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                nextPoint = null;

        //                                data = null;

        //#if MYLOG && DEBUG
        //                                Debug.WriteLine("{0} => {1}, data = {2}", point, null, null);
        //#endif

        //                                DeletePoint(point);

        //                                return false;
        //                            }
        //                        }
        //                    }
        //                case FixPoint.EType.End:
        //                    {                     
        //                        if(point.Segment != null)
        //                        {
        //                            nextPoint = point.ToSegmentPoint(out data);

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, point.Segment);
        //#endif
        //                            DeletePoint(point);
        //                            DeleteSegment(point);
        //                            return true;
        //                        }
        //                        else if(EndPoints.Count == 2)
        //                        {                            
        //                            nextPoint = EndPoints.First();

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
        //#endif
        //                            DeletePoint(point);

        //                            return true;
        //                        }
        //                        else if(EndPoints.Count == 1)
        //                        {                           
        //                            nextPoint = null;

        //                            data = null;

        //#if MYLOG && DEBUG
        //                            Debug.WriteLine("{0} => {1}, data = {2}", point, null, null);
        //#endif
        //                            DeletePoint(point);

        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            throw new Exception();
        //                        }
        //                    }
        //                default:
        //                    throw new Exception();
        //            }
        //        }

        //        #region Math's functions

        //        bool Odd(int value)
        //        {
        //            if (value % 2 == 0)
        //                return false;
        //            return true;
        //        }

        //        bool Even(int value)
        //        {
        //            if (value % 2 == 0)
        //                return true;
        //            return false;
        //        }

        //        #endregion
        //    }    
}
