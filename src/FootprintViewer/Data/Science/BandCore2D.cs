using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Science
{
    public class BandCore2D
    {
        private enum SegmentType { Begin, End, Full, Single }
        private enum SpecialSegmentType { Top, BottomReverse }

        private BandCore2D()
        {
            dict = new Dictionary<BandSegment, Tuple<FixPoint, FixPoint>>();

            Segments = new List<BandSegment>();

            BeginPoints = new List<FixPoint>();
            EndPoints = new List<FixPoint>();
            LeftPoints = new List<FixPoint>();
            RightPoints = new List<FixPoint>();
        }

        public BandCore2D(IList<Geo2D> near, IList<Geo2D> far, Func<double, bool> isCoverPolis) : this()
        {
            List<List<Geo2D>> NearPoints = new List<List<Geo2D>>();
            List<List<Geo2D>> FarPoints = new List<List<Geo2D>>();

            List<Geo2D> truePointsNear = new List<Geo2D>();
            List<Geo2D> truePointsFar = new List<Geo2D>();

            Geo2D old = near[0], cur;
            for (int i = 0; i < near.Count; i++)
            {
                cur = near[i];

                if (Math.Abs(cur.Lon - old.Lon) >= 3.2)
                {
                    double cutLat = linearInterpDiscontLat(old, cur);

                    if (old.Lon > 0.0)
                    {
                        truePointsNear.Add(new Geo2D(Math.PI, cutLat));
                        NearPoints.Add(new List<Geo2D>(truePointsNear));
                        truePointsNear.Clear();

                        truePointsNear.Add(new Geo2D(-Math.PI, cutLat));

                        truePointsNear.Add(cur);
                    }
                    else
                    {
                        truePointsNear.Add(new Geo2D(-Math.PI, cutLat));
                        NearPoints.Add(new List<Geo2D>(truePointsNear));
                        truePointsNear.Clear();

                        truePointsNear.Add(new Geo2D(Math.PI, cutLat));

                        truePointsNear.Add(cur);
                    }
                }
                else
                {
                    truePointsNear.Add(cur);
                }

                old = cur;
            }

            if (truePointsNear.Count != 0)
            {
                NearPoints.Add(new List<Geo2D>(truePointsNear));
                truePointsNear.Clear();
            }

            old = far[0];
            for (int i = 0; i < far.Count; i++)
            {
                cur = far[i];

                if (Math.Abs(cur.Lon - old.Lon) >= 3.2)
                {
                    double cutLat = linearInterpDiscontLat(old, cur);

                    if (old.Lon > 0.0)
                    {
                        truePointsFar.Add(new Geo2D(Math.PI, cutLat));
                        FarPoints.Add(new List<Geo2D>(truePointsFar));
                        truePointsFar.Clear();

                        truePointsFar.Add(new Geo2D(-Math.PI, cutLat));

                        truePointsFar.Add(cur);
                    }
                    else
                    {
                        truePointsFar.Add(new Geo2D(-Math.PI, cutLat));
                        FarPoints.Add(new List<Geo2D>(truePointsFar));
                        truePointsFar.Clear();

                        truePointsFar.Add(new Geo2D(Math.PI, cutLat));

                        truePointsFar.Add(cur);
                    }
                }
                else
                {
                    truePointsFar.Add(cur);
                }

                old = cur;
            }

            if (truePointsFar.Count != 0)
            {
                FarPoints.Add(new List<Geo2D>(truePointsFar));
                truePointsFar.Clear();
            }

            BandCore2DInit(NearPoints, FarPoints, isCoverPolis);
        }


        public static double DefaultExtrudeStep { get; } = 5.0 * Math.PI / 180.0;

        public double ExtrudeStep { get; set; } = DefaultExtrudeStep;

        public List<List<Geo2D>> CreateShapes(bool clockwise, bool extrudeMode = false)
        {
            InitTempVectors();

            var shapes = new List<List<Geo2D>>();

            FixPoint? point, nextPoint;

            while (GetShape(clockwise, out point))
            {
                shapes.Add(new List<Geo2D>());

                while (NextStep(point!, clockwise, out nextPoint, out List<Geo2D>? data) == true)
                {
                    if (data != null)
                    {
                        if (extrudeMode == false)
                        {
                            shapes.Last().AddRange(data);
                        }
                        else
                        {
                            if (point!.Type == FixPoint.EType.Left || point.Type == FixPoint.EType.Right)
                            {
                                var first = new Geo2D(data.First());

                                int signLat = (data.Count == 2) ? ((first.Lat > 0.0) ? 1 : -1) : 0;
                                int signLon = (first.Lon > 0.0) ? 1 : -1;

                                shapes.Last().Add(new Geo2D(first.Lon + signLon * ExtrudeStep, first.Lat + signLat * ExtrudeStep));
                            }

                            if (data.Count != 2)
                            {
                                shapes.Last().AddRange(data);
                            }

                            if (nextPoint!.Type == FixPoint.EType.Left || nextPoint.Type == FixPoint.EType.Right)
                            {
                                var last = new Geo2D(data.Last());

                                int signLat = (data.Count == 2) ? ((last.Lat > 0.0) ? 1 : -1) : 0;
                                int signLon = (last.Lon > 0.0) ? 1 : -1;

                                shapes.Last().Add(new Geo2D(last.Lon + signLon * ExtrudeStep, last.Lat + signLat * ExtrudeStep));
                            }
                        }
                    }

                    point = nextPoint;
                }
            }

            return shapes;
        }


        private readonly Dictionary<BandSegment, Tuple<FixPoint, FixPoint>> dict;

        private void BandCore2DInit(List<List<Geo2D>> near, List<List<Geo2D>> far, Func<double, bool> isCoverPolis)
        {
            BandSegment.ResetId();
            FixPoint.ResetId();

            if (isCoverPolis(Math.PI / 2.0) == true)
            {
                AddSpecialSegment(BandCore2D.SpecialSegmentType.Top);
            }

            if (isCoverPolis(-Math.PI / 2.0) == true)
            {
                AddSpecialSegment(BandCore2D.SpecialSegmentType.BottomReverse);
            }

            if (near.Count == 1)
            {
                AddSegment(near.Single(), BandCore2D.SegmentType.Single);
            }
            else
            {
                AddSegment(near.First(), BandCore2D.SegmentType.Begin);

                foreach (var item in near.Skip(1).Take(near.Count - 2))
                {
                    AddSegment(item, BandCore2D.SegmentType.Full);
                }

                AddSegment(near.Last(), BandCore2D.SegmentType.End);
            }

            if (far.Count == 1)
            {
                AddSegment(far.Single(), BandCore2D.SegmentType.Single);
            }
            else
            {
                AddSegment(far.First(), BandCore2D.SegmentType.Begin);

                foreach (var item in far.Skip(1).Take(far.Count - 2))
                {
                    AddSegment(item, BandCore2D.SegmentType.Full);
                }

                AddSegment(far.Last(), BandCore2D.SegmentType.End);
            }

            #region Cut Points Test

            TestBeginCut();

            TestEndCut();

            #endregion
        }

        private void TestBeginCut()
        {
            List<FixPoint> tempbegins = new List<FixPoint>();
            tempbegins.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Begin));
            tempbegins.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Begin));

            if (tempbegins.Count != 2)
            {
                throw new Exception();
            }

            var beg1 = tempbegins[0];
            var beg2 = tempbegins[1];

            if (Math.Abs(beg1.Fix.Lon - beg2.Fix.Lon) > Math.PI)
            {
                double latCut = linearInterpDiscontLat(beg1.Fix, beg2.Fix);

                BeginCut(beg1, latCut);
                BeginCut(beg2, latCut);
            }
        }

        private void TestEndCut()
        {
            List<FixPoint> tempends = new List<FixPoint>();
            tempends.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.End));
            tempends.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.End));

            if (tempends.Count != 2)
            {
                throw new Exception();
            }

            var end1 = tempends[0];
            var end2 = tempends[1];

            if (Math.Abs(end1.Fix.Lon - end2.Fix.Lon) > Math.PI)
            {
                double latCut = linearInterpDiscontLat(end1.Fix, end2.Fix);

                EndCut(end1, latCut);
                EndCut(end2, latCut);
            }
        }

        private void BeginCut(FixPoint beg, double latCut)
        {
            FixPoint? neww = null;
            if (beg.Fix.Lon > 0.0)
            {
                neww = new FixPoint(new Geo2D(Math.PI, latCut), FixPoint.EType.Right);
            }
            else
            {
                neww = new FixPoint(new Geo2D(-Math.PI, latCut), FixPoint.EType.Left);
            }

            BandSegment? bs = GetDictionaryIndex(beg);

            if (bs != null)
            {
                bs.Seg = BandSegment.Segment.Full;
                bs.AddFirst(neww.Fix);
                dict[bs] = Tuple.Create<FixPoint, FixPoint>(neww, dict[bs].Item2);
            }
        }

        private void EndCut(FixPoint end, double latCut)
        {
            FixPoint? neww = null;
            if (end.Fix.Lon > 0.0)
            {
                neww = new FixPoint(new Geo2D(Math.PI, latCut), FixPoint.EType.Right);
            }
            else
            {
                neww = new FixPoint(new Geo2D(-Math.PI, latCut), FixPoint.EType.Left);
            }

            BandSegment? bs = GetDictionaryIndex(end);

            if (bs != null)
            {
                bs.Seg = BandSegment.Segment.Full;
                bs.AddLast(neww.Fix);
                dict[bs] = Tuple.Create<FixPoint, FixPoint>(dict[bs].Item1, neww);
            }
        }

        private void AddSegment(List<Geo2D> arr, SegmentType type)
        {
            switch (type)
            {
                case SegmentType.Begin:
                    dict.Add(new BandSegment(arr, BandSegment.Segment.Begin),
                        Tuple.Create(new FixPoint(arr.First(), FixPoint.EType.Begin), new FixPoint(arr.Last())));
                    break;
                case SegmentType.End:
                    dict.Add(new BandSegment(arr, BandSegment.Segment.End),
                        Tuple.Create(new FixPoint(arr.First()), new FixPoint(arr.Last(), FixPoint.EType.End)));
                    break;
                case SegmentType.Full:
                    dict.Add(new BandSegment(arr, BandSegment.Segment.Full),
                        Tuple.Create(new FixPoint(arr.First()), new FixPoint(arr.Last())));
                    break;
                case SegmentType.Single:
                    dict.Add(new BandSegment(arr, BandSegment.Segment.Begin),
                        Tuple.Create(new FixPoint(arr.First(), FixPoint.EType.Begin), new FixPoint(arr.Last(), FixPoint.EType.End)));
                    break;
                default:
                    throw new Exception();
            }

        }

        private void AddSpecialSegment(SpecialSegmentType type)
        {
            switch (type)
            {
                case SpecialSegmentType.Top:
                    dict.Add(new BandSegment(BandSegment.TopArr, BandSegment.Segment.Full),
                        Tuple.Create(new FixPoint(-Math.PI, Math.PI / 2.0), new FixPoint(Math.PI, Math.PI / 2.0)));
                    break;
                case SpecialSegmentType.BottomReverse:
                    dict.Add(new BandSegment(BandSegment.BottomReverseArr, BandSegment.Segment.Full),
                        Tuple.Create(new FixPoint(Math.PI, -Math.PI / 2.0), new FixPoint(-Math.PI, -Math.PI / 2.0)));
                    break;
                default:
                    break;
            }
        }

        private List<FixPoint> LeftPoints { get; set; }
        private List<FixPoint> RightPoints { get; set; }

        private List<FixPoint> BeginPoints { get; set; }
        private List<FixPoint> EndPoints { get; set; }

        private List<BandSegment> Segments { get; set; }

        private void InitTempVectors()
        {
            #region Segments

            Segments.Clear();

            Segments.AddRange(dict.Keys);

            #endregion

            #region BeginPoints

            BeginPoints.Clear();

            BeginPoints.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Begin));
            BeginPoints.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Begin));

            #endregion

            #region EndPoints

            EndPoints.Clear();

            EndPoints.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.End));
            EndPoints.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.End));

            #endregion

            #region LeftPoints

            LeftPoints.Clear();

            LeftPoints.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Left));
            LeftPoints.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Left));

            LeftPoints = LeftPoints.OrderBy(s => s.Fix.Lat).ToList();

            #endregion

            #region RightPoints

            RightPoints.Clear();

            RightPoints.AddRange(dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Right));
            RightPoints.AddRange(dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Right));

            RightPoints = RightPoints.OrderBy(s => s.Fix.Lat).ToList();

            #endregion

            // if (this.BeginPoints.Count != 2 || this.EndPoints.Count != 2)
            //     throw new Exception();

#if MYLOG && DEBUG
            {
                Debug.WriteLine("===============================================");
                Debug.WriteLine("  BandSegments: ({0})", Segments.Count);
                int i = 0;
                foreach (var item in Segments)
                    Debug.WriteLine("    {0} - {1}", i++, item);
                Debug.WriteLine("");
                Debug.WriteLine("  BeginPoints: ({0})", BeginPoints.Count);
                i = 0;
                foreach (var item in BeginPoints)
                    Debug.WriteLine("    {0} - {1}", i++, item);
                Debug.WriteLine("");

                Debug.WriteLine("  EndPoints: ({0})", EndPoints.Count);

                i = 0;
                foreach (var item in EndPoints)
                    Debug.WriteLine("    {0} - {1}", i++, item);
                Debug.WriteLine("");

                Debug.WriteLine("  LeftPoints: ({0})", LeftPoints.Count);
                i = 0;
                foreach (var item in LeftPoints)
                    Debug.WriteLine("    {0} - {1}", i++, item);
                Debug.WriteLine("");

                Debug.WriteLine("  RightPoints: ({0})", RightPoints.Count);
                i = 0;
                foreach (var item in RightPoints)
                    Debug.WriteLine("    {0} - {1}", i++, item);

                Debug.WriteLine("===============================================");
            }
#endif
        }

        private BandSegment? GetDictionaryIndex(FixPoint point)
        {
            foreach (var item in dict)
            {
                if (item.Value.Item1 == point || item.Value.Item2 == point)
                {
                    return item.Key;
                }
            }

            return null;
        }

        private FixPoint? IsPoint(FixPoint point)
        {
            switch (point.Type)
            {
                case FixPoint.EType.Begin:
                    if (BeginPoints.Contains(point) == true)
                    {
                        return point;
                    }

                    return null;
                case FixPoint.EType.Left:
                    if (LeftPoints.Contains(point) == true)
                    {
                        return point;
                    }

                    return null;
                case FixPoint.EType.Right:
                    if (RightPoints.Contains(point) == true)
                    {
                        return point;
                    }

                    return null;
                case FixPoint.EType.End:
                    if (EndPoints.Contains(point) == true)
                    {
                        return point;
                    }

                    return null;
                default:
                    throw new Exception();
            }
        }

        private FixPoint GetData(BandSegment segment, FixPoint asBegin, out List<Geo2D> data)
        {
            if (dict[segment].Item1 == asBegin)
            {
                data = segment.NewData;
                return dict[segment].Item2;
            }
            else if (dict[segment].Item2 == asBegin)
            {
                data = segment.NewReverseData;
                return dict[segment].Item1;
            }
            else
            {
                throw new Exception();
            }
        }

        private bool GetShape(bool clockwise, out FixPoint? begin)
        {
            if (LeftPoints.Count != 0)
            {
                // clockwork wise => First
                if (clockwise == true)
                {
                    begin = LeftPoints.First();
                }
                else
                {
                    begin = LeftPoints.Last();
                }

                return true;
            }

            if (RightPoints.Count != 0)
            {
                // clockwork wise => Lat
                if (clockwise == true)
                {
                    begin = RightPoints.Last();
                }
                else
                {
                    begin = RightPoints.First();
                }

                return true;
            }

            if (BeginPoints.Count != 0)
            {
                // этот вариант возможен при отсутствии соударения с left или right

                var point = BeginPoints.First();

                // это возможно т.к. метод NextStep, в случае begin или end всегда делает выбор в сторону сегмента, 
                // а не соседней точки, если это возможно
                if (clockwise == true)
                {
                    begin = dict[GetDictionaryIndex(point)!].Item1;// point.Segment.FixPointBegin;
                }
                else
                {
                    begin = dict[GetDictionaryIndex(point)!].Item2;// point.Segment.FixPointEnd;
                }

                return true;
            }

            if (EndPoints.Count != 0)
            {
                throw new Exception();
            }

            begin = null;
            return false;
        }

        private void DeletePoint(FixPoint point)
        {
            switch (point.Type)
            {
                case FixPoint.EType.Begin:
                    BeginPoints.Remove(point);
                    break;
                case FixPoint.EType.Left:
                    LeftPoints.Remove(point);
                    break;
                case FixPoint.EType.Right:
                    RightPoints.Remove(point);
                    break;
                case FixPoint.EType.End:
                    EndPoints.Remove(point);
                    break;
                default:
                    break;
            }
        }

        private void DeleteSegment(BandSegment segment)
        {
            Segments.Remove(segment);
        }

        //        private bool GetNextFixPoint(FixPoint point, out List<Geo2D> data, out FixPoint fixPoint)
        //        {
        //            if (point == null)
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



        //                        // LeftPoints.Remove(point);
        //                        // LeftPoints.Remove(leftFixPoint);

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

        //                        // RightPoints.Remove(point);
        //                        // RightPoints.Remove(rightFixPoint);

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

        //                                // EndPoints.Remove(point);



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
        //                                // EndPoints.Remove(point);

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

        private bool NextStep(FixPoint point, bool clockwise, out FixPoint? nextPoint, out List<Geo2D>? data)
        {
            if (point == null)
            {
                nextPoint = null;

                data = null;

                return false;
            }

            var indexSegment = GetDictionaryIndex(point)!;

            switch (point.Type)
            {
                case FixPoint.EType.Begin:
                {
                    // особенность реализации в том, что всегда выбирается вариант движения по сегменту если он имеется
                    // для поддержки случая без соударения с границами

                    if (Segments.Contains(indexSegment) == true)
                    {
                        nextPoint = GetData(indexSegment, point, out data);

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif

                        DeletePoint(point);
                        DeleteSegment(indexSegment);

                        return true;
                    }
                    else if (BeginPoints.Count == 2)
                    {
                        DeletePoint(point);

                        nextPoint = BeginPoints.First();

                        data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = empty", point, nextPoint);
#endif

                        return true;
                    }
                    else if (BeginPoints.Count == 1)
                    {
                        DeletePoint(point);

                        nextPoint = null;

                        data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => end", point);
#endif

                        return false;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                case FixPoint.EType.Left:
                {
                    if (IsPoint(point) != null)
                    {
                        if (Even(LeftPoints.Count) == true)
                        {
                            int index = LeftPoints.FindIndex(s => s.Equals(point));

                            if (clockwise == true)
                            {
                                index += 1;
                            }
                            else
                            {
                                index -= 1;
                            }

                            nextPoint = LeftPoints[index];

                            data = null;

#if MYLOG && DEBUG
                                Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
#endif

                            DeletePoint(point);

                            return true;
                        }
                        else
                        {
                            if (Segments.Contains(indexSegment) == true)
                            {
                                nextPoint = GetData(indexSegment, point, out data);
#if MYLOG && DEBUG
                                    Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif
                                DeletePoint(point);
                                DeleteSegment(indexSegment);
                                return true;
                            }
                            else
                            {
                                nextPoint = null;

                                data = null;

#if MYLOG && DEBUG
                                    Debug.WriteLine("{0} => end, data = empty", point);
#endif
                                DeletePoint(point);

                                return false;
                            }

                        }
                    }
                    else
                    {
                        nextPoint = null;

                        data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => end", point);
#endif

                        return false;
                    }
                }
                case FixPoint.EType.Right:
                {
                    if (IsPoint(point) != null)
                    {
                        if (Even(RightPoints.Count) == true)
                        {
                            int index = RightPoints.FindIndex(s => s.Equals(point));

                            if (clockwise == true)
                            {
                                index -= 1;
                            }
                            else
                            {
                                index += 1;
                            }

                            nextPoint = RightPoints[index];

                            data = null;

#if MYLOG && DEBUG
                                Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
#endif
                            DeletePoint(point);

                            return true;
                        }
                        else
                        {
                            if (Segments.Contains(indexSegment) == true)
                            {
                                nextPoint = GetData(indexSegment, point, out data);
#if MYLOG && DEBUG
                                    Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif
                                DeletePoint(point);
                                DeleteSegment(indexSegment);
                                return true;
                            }
                            else
                            {
                                nextPoint = null;

                                data = null;

#if MYLOG && DEBUG
                                    Debug.WriteLine("{0} => end, data = empty", point);
#endif

                                DeletePoint(point);

                                return false;
                            }
                        }
                    }
                    else
                    {
                        nextPoint = null;

                        data = null;
#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => end", point);
#endif

                        return false;
                    }
                }
                case FixPoint.EType.End:
                {
                    if (Segments.Contains(indexSegment) == true)
                    {
                        nextPoint = GetData(indexSegment, point, out data);
#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif
                        DeletePoint(point);
                        DeleteSegment(indexSegment);
                        return true;
                    }
                    else if (EndPoints.Count == 2)
                    {
                        DeletePoint(point);

                        nextPoint = EndPoints.First();

                        data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = empty", point, nextPoint);
#endif

                        return true;
                    }
                    else if (EndPoints.Count == 1)
                    {
                        nextPoint = null;

                        data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => end", point);
#endif
                        DeletePoint(point);

                        return false;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                default:
                    throw new Exception();
            }
        }

        // latitude for the longitude of +/- 180 (both)
        private double linearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
        {
            Geo2D p1 = pp1.ToRadians(), p2 = pp2.ToRadians();

            // one longitude should be negative one positive, make them both positive
            double lon1 = p1.Lon, lat1 = p1.Lat, lon2 = p2.Lon, lat2 = p2.Lat;
            if (lon1 > lon2)
            {
                lon2 += 2 * Math.PI; // in radians
            }
            else
            {
                lon1 += 2 * Math.PI;
            }

            return (lat1 + (Math.PI - lon1) * (lat2 - lat1) / (lon2 - lon1));
        }

        private class FixPoint
        {
            public FixPoint(Geo2D point)
            {
                if (point.Lon.Equals(-Math.PI))
                {
                    Fix = point;
                    Type = EType.Left;
                }
                else if (point.Lon.Equals(Math.PI))
                {
                    Fix = point;
                    Type = EType.Right;
                }
                else
                {
                    throw new Exception();
                }

                id = ++classCounter;
            }

            public FixPoint(Geo2D point, EType type)
            {
                Fix = point;
                Type = type;

                id = ++classCounter;
            }

            public FixPoint(double lon, double lat) : this(new Geo2D(lon, lat)) { }

            public enum EType { Begin, Left, Right, End }

            public EType Type { get; }

            public Geo2D Fix { get; }

            private static int classCounter = 0;

            public static void ResetId() { classCounter = 0; }

            private readonly int id;

            public override string ToString()
            {
                return string.Format("FixPoint {0:00}({1},Lat={2:0,0})", id, Enum.GetName(typeof(EType), Type), Fix.Lat * ScienceMath.RadiansToDegrees);
            }

            public string ShortDescription()
            {
                return string.Format("FixPoint {0:00}", id);
            }
        }

        private class BandSegment
        {
            #region Default Segments

            public static List<Geo2D> TopArr = new List<Geo2D>() { new Geo2D(-Math.PI, Math.PI / 2.0), new Geo2D(Math.PI, Math.PI / 2.0) };
            public static List<Geo2D> TopReverseArr = new List<Geo2D>() { new Geo2D(Math.PI, Math.PI / 2.0), new Geo2D(-Math.PI, Math.PI / 2.0) };
            public static List<Geo2D> BottomArr = new List<Geo2D>() { new Geo2D(-Math.PI, -Math.PI / 2.0), new Geo2D(Math.PI, -Math.PI / 2.0) };
            public static List<Geo2D> BottomReverseArr = new List<Geo2D>() { new Geo2D(Math.PI, -Math.PI / 2.0), new Geo2D(-Math.PI, -Math.PI / 2.0) };

            #endregion

            public BandSegment(IList<Geo2D> arr, Segment eseg) : this()
            {
                Seg = eseg;

                data = new LinkedList<Geo2D>(arr);
            }

            protected BandSegment(IList<Geo2D> arr) : this()
            {
                Seg = Segment.Full;

                data = new LinkedList<Geo2D>(arr);
            }

            protected BandSegment() { id = ++ClassCounter; }

            public enum Segment { Begin, Full, End };
            public Segment Seg { get; set; }

            public int Length { get { return data.Count; } }

            public Geo2D Begin
            {
                get
                {
                    return data.First();
                }
            }

            public Geo2D End
            {
                get
                {
                    return data.Last();
                }
            }

            private static int ClassCounter = 0;

            public static void ResetId() { ClassCounter = 0; }

            private readonly int id;

            public override string ToString()
            {
                return string.Format("Segment {0:00}", id);
            }

            public List<Geo2D> NewData { get { return new List<Geo2D>(data); } }
            public List<Geo2D> NewReverseData { get { var temp = new List<Geo2D>(data); temp.Reverse(); return temp; } }

            private readonly LinkedList<Geo2D> data = new LinkedList<Geo2D>();

            public void AddFirst(Geo2D point)
            {
                data.AddFirst(point);
            }

            public void AddLast(Geo2D point)
            {
                data.AddLast(point);
            }

        }

        #region Math's functions

        private bool Odd(int value)
        {
            if (value % 2 == 0)
            {
                return false;
            }

            return true;
        }

        private bool Even(int value)
        {
            if (value % 2 == 0)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
