namespace SpaceScience.Model;

public class SwathCore2D
{
    private enum SegmentType { Begin, End, Full, Single }
    private enum SpecialSegmentType { Top, BottomReverse }

    private List<FixPoint> _leftPoints;
    private List<FixPoint> _rightPoints;
    private readonly List<FixPoint> _beginPoints;
    private readonly List<FixPoint> _endPoints;
    private readonly List<SwathSegment> _segments;
    private const double _defaultExtrudeStep = 5.0 * Math.PI / 180.0;
    private readonly Dictionary<SwathSegment, Tuple<FixPoint, FixPoint>> _dict;

    private SwathCore2D()
    {
        _dict = new Dictionary<SwathSegment, Tuple<FixPoint, FixPoint>>();

        _segments = new List<SwathSegment>();

        _beginPoints = new List<FixPoint>();
        _endPoints = new List<FixPoint>();
        _leftPoints = new List<FixPoint>();
        _rightPoints = new List<FixPoint>();
    }

    public SwathCore2D(IList<Geo2D> near, IList<Geo2D> far, Func<double, bool> isCoverPolis) : this()
    {
        var NearPoints = new List<List<Geo2D>>();
        var FarPoints = new List<List<Geo2D>>();

        var truePointsNear = new List<Geo2D>();
        var truePointsFar = new List<Geo2D>();

        Geo2D old = near[0], cur;
        for (int i = 0; i < near.Count; i++)
        {
            cur = near[i];

            if (Math.Abs(cur.Lon - old.Lon) >= 3.2)
            {
                double cutLat = LinearInterpDiscontLat(old, cur);

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
                double cutLat = LinearInterpDiscontLat(old, cur);

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

        SwathCore2DInit(NearPoints, FarPoints, isCoverPolis);
    }

    public static double ExtrudeStep => _defaultExtrudeStep;

    public List<List<Geo2D>> CreateShapes(bool clockwise, bool extrudeMode = false)
    {
        InitTempVectors();

        var shapes = new List<List<Geo2D>>();

        while (GetShape(clockwise, out var point))
        {
            shapes.Add(new List<Geo2D>());

            while (NextStep(point!, clockwise, out var nextPoint, out List<Geo2D>? data) == true)
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

                            int signLat = data.Count == 2 ? first.Lat > 0.0 ? 1 : -1 : 0;
                            int signLon = first.Lon > 0.0 ? 1 : -1;

                            shapes.Last().Add(new Geo2D(first.Lon + signLon * ExtrudeStep, first.Lat + signLat * ExtrudeStep));
                        }

                        if (data.Count != 2)
                        {
                            shapes.Last().AddRange(data);
                        }

                        if (nextPoint!.Type == FixPoint.EType.Left || nextPoint.Type == FixPoint.EType.Right)
                        {
                            var last = new Geo2D(data.Last());

                            int signLat = data.Count == 2 ? last.Lat > 0.0 ? 1 : -1 : 0;
                            int signLon = last.Lon > 0.0 ? 1 : -1;

                            shapes.Last().Add(new Geo2D(last.Lon + signLon * ExtrudeStep, last.Lat + signLat * ExtrudeStep));
                        }
                    }
                }

                point = nextPoint;
            }
        }

        return shapes;
    }

    private void SwathCore2DInit(List<List<Geo2D>> near, List<List<Geo2D>> far, Func<double, bool> isCoverPolis)
    {
        SwathSegment.ResetId();
        FixPoint.ResetId();

        if (isCoverPolis(Math.PI / 2.0) == true)
        {
            AddSpecialSegment(SpecialSegmentType.Top);
        }

        if (isCoverPolis(-Math.PI / 2.0) == true)
        {
            AddSpecialSegment(SpecialSegmentType.BottomReverse);
        }

        if (near.Count == 1)
        {
            AddSegment(near.Single(), SegmentType.Single);
        }
        else
        {
            AddSegment(near.First(), SegmentType.Begin);

            foreach (var item in near.Skip(1).Take(near.Count - 2))
            {
                AddSegment(item, SegmentType.Full);
            }

            AddSegment(near.Last(), SegmentType.End);
        }

        if (far.Count == 1)
        {
            AddSegment(far.Single(), SegmentType.Single);
        }
        else
        {
            AddSegment(far.First(), SegmentType.Begin);

            foreach (var item in far.Skip(1).Take(far.Count - 2))
            {
                AddSegment(item, SegmentType.Full);
            }

            AddSegment(far.Last(), SegmentType.End);
        }

        #region Cut Points Test

        TestBeginCut();

        TestEndCut();

        #endregion
    }

    private void TestBeginCut()
    {
        var tempbegins = new List<FixPoint>();
        tempbegins.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Begin));
        tempbegins.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Begin));

        if (tempbegins.Count != 2)
        {
            throw new Exception();
        }

        var beg1 = tempbegins[0];
        var beg2 = tempbegins[1];

        if (Math.Abs(beg1.Fix.Lon - beg2.Fix.Lon) > Math.PI)
        {
            double latCut = LinearInterpDiscontLat(beg1.Fix, beg2.Fix);

            BeginCut(beg1, latCut);
            BeginCut(beg2, latCut);
        }
    }

    private void TestEndCut()
    {
        var tempends = new List<FixPoint>();
        tempends.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.End));
        tempends.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.End));

        if (tempends.Count != 2)
        {
            throw new Exception();
        }

        var end1 = tempends[0];
        var end2 = tempends[1];

        if (Math.Abs(end1.Fix.Lon - end2.Fix.Lon) > Math.PI)
        {
            double latCut = LinearInterpDiscontLat(end1.Fix, end2.Fix);

            EndCut(end1, latCut);
            EndCut(end2, latCut);
        }
    }

    private void BeginCut(FixPoint beg, double latCut)
    {
        FixPoint? neww;
        if (beg.Fix.Lon > 0.0)
        {
            neww = new FixPoint(new Geo2D(Math.PI, latCut), FixPoint.EType.Right);
        }
        else
        {
            neww = new FixPoint(new Geo2D(-Math.PI, latCut), FixPoint.EType.Left);
        }

        SwathSegment? bs = GetDictionaryIndex(beg);

        if (bs != null)
        {
            bs.Seg = SwathSegment.Segment.Full;
            bs.AddFirst(neww.Fix);
            _dict[bs] = Tuple.Create(neww, _dict[bs].Item2);
        }
    }

    private void EndCut(FixPoint end, double latCut)
    {
        FixPoint? neww;
        if (end.Fix.Lon > 0.0)
        {
            neww = new FixPoint(new Geo2D(Math.PI, latCut), FixPoint.EType.Right);
        }
        else
        {
            neww = new FixPoint(new Geo2D(-Math.PI, latCut), FixPoint.EType.Left);
        }

        SwathSegment? bs = GetDictionaryIndex(end);

        if (bs != null)
        {
            bs.Seg = SwathSegment.Segment.Full;
            bs.AddLast(neww.Fix);
            _dict[bs] = Tuple.Create(_dict[bs].Item1, neww);
        }
    }

    private void AddSegment(List<Geo2D> arr, SegmentType type)
    {
        switch (type)
        {
            case SegmentType.Begin:
                _dict.Add(new SwathSegment(arr, SwathSegment.Segment.Begin),
                    Tuple.Create(new FixPoint(arr.First(), FixPoint.EType.Begin), new FixPoint(arr.Last())));
                break;
            case SegmentType.End:
                _dict.Add(new SwathSegment(arr, SwathSegment.Segment.End),
                    Tuple.Create(new FixPoint(arr.First()), new FixPoint(arr.Last(), FixPoint.EType.End)));
                break;
            case SegmentType.Full:
                _dict.Add(new SwathSegment(arr, SwathSegment.Segment.Full),
                    Tuple.Create(new FixPoint(arr.First()), new FixPoint(arr.Last())));
                break;
            case SegmentType.Single:
                _dict.Add(new SwathSegment(arr, SwathSegment.Segment.Begin),
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
                _dict.Add(new SwathSegment(SwathSegment.TopArr, SwathSegment.Segment.Full),
                    Tuple.Create(new FixPoint(-Math.PI, Math.PI / 2.0), new FixPoint(Math.PI, Math.PI / 2.0)));
                break;
            case SpecialSegmentType.BottomReverse:
                _dict.Add(new SwathSegment(SwathSegment.BottomReverseArr, SwathSegment.Segment.Full),
                    Tuple.Create(new FixPoint(Math.PI, -Math.PI / 2.0), new FixPoint(-Math.PI, -Math.PI / 2.0)));
                break;
            default:
                break;
        }
    }

    private void InitTempVectors()
    {
        _segments.Clear();
        _beginPoints.Clear();
        _endPoints.Clear();
        _leftPoints.Clear();
        _rightPoints.Clear();

        _segments.AddRange(_dict.Keys);

        _beginPoints.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Begin));
        _beginPoints.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Begin));

        _endPoints.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.End));
        _endPoints.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.End));

        _leftPoints.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Left));
        _leftPoints.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Left));

        _leftPoints = _leftPoints.OrderBy(s => s.Fix.Lat).ToList();

        _rightPoints.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Right));
        _rightPoints.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Right));

        _rightPoints = _rightPoints.OrderBy(s => s.Fix.Lat).ToList();

        // if (this.BeginPoints.Count != 2 || this.EndPoints.Count != 2)
        //     throw new Exception();

#if MYLOG && DEBUG
        {
            Debug.WriteLine("===============================================");
            Debug.WriteLine("  SwathSegments: ({0})", Segments.Count);
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

    private SwathSegment? GetDictionaryIndex(FixPoint point)
    {
        foreach (var item in _dict)
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
                if (_beginPoints.Contains(point) == true)
                {
                    return point;
                }

                return null;
            case FixPoint.EType.Left:
                if (_leftPoints.Contains(point) == true)
                {
                    return point;
                }

                return null;
            case FixPoint.EType.Right:
                if (_rightPoints.Contains(point) == true)
                {
                    return point;
                }

                return null;
            case FixPoint.EType.End:
                if (_endPoints.Contains(point) == true)
                {
                    return point;
                }

                return null;
            default:
                throw new Exception();
        }
    }

    private FixPoint GetData(SwathSegment segment, FixPoint asBegin, out List<Geo2D> data)
    {
        if (_dict[segment].Item1 == asBegin)
        {
            data = segment.NewData;
            return _dict[segment].Item2;
        }
        else if (_dict[segment].Item2 == asBegin)
        {
            data = segment.NewReverseData;
            return _dict[segment].Item1;
        }
        else
        {
            throw new Exception();
        }
    }

    private bool GetShape(bool clockwise, out FixPoint? begin)
    {
        if (_leftPoints.Count != 0)
        {
            // clockwork wise => First
            if (clockwise == true)
            {
                begin = _leftPoints.First();
            }
            else
            {
                begin = _leftPoints.Last();
            }

            return true;
        }

        if (_rightPoints.Count != 0)
        {
            // clockwork wise => Lat
            if (clockwise == true)
            {
                begin = _rightPoints.Last();
            }
            else
            {
                begin = _rightPoints.First();
            }

            return true;
        }

        if (_beginPoints.Count != 0)
        {
            // этот вариант возможен при отсутствии соударения с left или right

            var point = _beginPoints.First();

            // это возможно т.к. метод NextStep, в случае begin или end всегда делает выбор в сторону сегмента, 
            // а не соседней точки, если это возможно
            if (clockwise == true)
            {
                begin = _dict[GetDictionaryIndex(point)!].Item1;// point.Segment.FixPointBegin;
            }
            else
            {
                begin = _dict[GetDictionaryIndex(point)!].Item2;// point.Segment.FixPointEnd;
            }

            return true;
        }

        if (_endPoints.Count != 0)
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
                _beginPoints.Remove(point);
                break;
            case FixPoint.EType.Left:
                _leftPoints.Remove(point);
                break;
            case FixPoint.EType.Right:
                _rightPoints.Remove(point);
                break;
            case FixPoint.EType.End:
                _endPoints.Remove(point);
                break;
            default:
                break;
        }
    }

    private void DeleteSegment(SwathSegment segment)
    {
        _segments.Remove(segment);
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

    //            Debug.WriteLine("  SwathSegments: ({0})", Segments.Count);
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

                    if (_segments.Contains(indexSegment) == true)
                    {
                        nextPoint = GetData(indexSegment, point, out data);

#if MYLOG && DEBUG
                        Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif

                        DeletePoint(point);
                        DeleteSegment(indexSegment);

                        return true;
                    }
                    else if (_beginPoints.Count == 2)
                    {
                        DeletePoint(point);

                        nextPoint = _beginPoints.First();

                        data = null;

#if MYLOG && DEBUG
                        Debug.WriteLine("{0} => {1}, data = empty", point, nextPoint);
#endif

                        return true;
                    }
                    else if (_beginPoints.Count == 1)
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
                        if (Even(_leftPoints.Count) == true)
                        {
                            int index = _leftPoints.FindIndex(s => s.Equals(point));

                            if (clockwise == true)
                            {
                                index += 1;
                            }
                            else
                            {
                                index -= 1;
                            }

                            nextPoint = _leftPoints[index];

                            data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
#endif

                            DeletePoint(point);

                            return true;
                        }
                        else
                        {
                            if (_segments.Contains(indexSegment) == true)
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
                        if (Even(_rightPoints.Count) == true)
                        {
                            int index = _rightPoints.FindIndex(s => s.Equals(point));

                            if (clockwise == true)
                            {
                                index -= 1;
                            }
                            else
                            {
                                index += 1;
                            }

                            nextPoint = _rightPoints[index];

                            data = null;

#if MYLOG && DEBUG
                            Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, null);
#endif
                            DeletePoint(point);

                            return true;
                        }
                        else
                        {
                            if (_segments.Contains(indexSegment) == true)
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
                    if (_segments.Contains(indexSegment) == true)
                    {
                        nextPoint = GetData(indexSegment, point, out data);
#if MYLOG && DEBUG
                        Debug.WriteLine("{0} => {1}, data = {2}", point, nextPoint, indexSegment);
#endif
                        DeletePoint(point);
                        DeleteSegment(indexSegment);
                        return true;
                    }
                    else if (_endPoints.Count == 2)
                    {
                        DeletePoint(point);

                        nextPoint = _endPoints.First();

                        data = null;

#if MYLOG && DEBUG
                        Debug.WriteLine("{0} => {1}, data = empty", point, nextPoint);
#endif

                        return true;
                    }
                    else if (_endPoints.Count == 1)
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
    private static double LinearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
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

        return lat1 + (Math.PI - lon1) * (lat2 - lat1) / (lon2 - lon1);
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
            return string.Format("FixPoint {0:00}({1},Lat={2:0,0})", id, Enum.GetName(typeof(EType), Type), Fix.Lat * SpaceMath.RadiansToDegrees);
        }

        public string ShortDescription()
        {
            return string.Format("FixPoint {0:00}", id);
        }
    }

    private class SwathSegment
    {
        public static List<Geo2D> TopArr = new() { new Geo2D(-Math.PI, SpaceMath.HALFPI), new Geo2D(Math.PI, SpaceMath.HALFPI) };
        public static List<Geo2D> TopReverseArr = new() { new Geo2D(Math.PI, SpaceMath.HALFPI), new Geo2D(-Math.PI, SpaceMath.HALFPI) };
        public static List<Geo2D> BottomArr = new() { new Geo2D(-Math.PI, -SpaceMath.HALFPI), new Geo2D(Math.PI, -SpaceMath.HALFPI) };
        public static List<Geo2D> BottomReverseArr = new() { new Geo2D(Math.PI, -SpaceMath.HALFPI), new Geo2D(-Math.PI, -SpaceMath.HALFPI) };

        public SwathSegment(IList<Geo2D> arr, Segment eseg) : this()
        {
            Seg = eseg;

            data = new LinkedList<Geo2D>(arr);
        }

        protected SwathSegment(IList<Geo2D> arr) : this()
        {
            Seg = Segment.Full;

            data = new LinkedList<Geo2D>(arr);
        }

        protected SwathSegment() { id = ++ClassCounter; }

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

        private readonly LinkedList<Geo2D> data = new();

        public void AddFirst(Geo2D point)
        {
            data.AddFirst(point);
        }

        public void AddLast(Geo2D point)
        {
            data.AddLast(point);
        }

    }

    //private static bool Odd(int value)
    //{
    //    if (value % 2 == 0)
    //    {
    //        return false;
    //    }

    //    return true;
    //}

    private static bool Even(int value)
    {
        if (value % 2 == 0)
        {
            return true;
        }

        return false;
    }
}
