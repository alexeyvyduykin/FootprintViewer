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

    public SwathCore2D(IList<(double lonRad, double latRad)> near, IList<(double lonRad, double latRad)> far, Func<double, bool> isCoverPolis) : this()
    {
        var NearPoints = new List<List<(double lonRad, double latRad)>>();
        var FarPoints = new List<List<(double lonRad, double latRad)>>();

        var truePointsNear = new List<(double lonRad, double latRad)>();
        var truePointsFar = new List<(double lonRad, double latRad)>();

        var (oldLonRad, oldLatRad) = near[0];

        for (int i = 0; i < near.Count; i++)
        {
            var (curLonRad, curLatRad) = near[i];

            if (Math.Abs(curLonRad - oldLonRad) >= 3.2)
            {
                double cutLat = LinearInterpDiscontLat(oldLonRad, oldLatRad, curLonRad, curLatRad);

                if (oldLonRad > 0.0)
                {
                    truePointsNear.Add((Math.PI, cutLat));
                    NearPoints.Add(truePointsNear.ToList());
                    truePointsNear.Clear();

                    truePointsNear.Add((-Math.PI, cutLat));

                    truePointsNear.Add((curLonRad, curLatRad));
                }
                else
                {
                    truePointsNear.Add((-Math.PI, cutLat));
                    NearPoints.Add(truePointsNear.ToList());
                    truePointsNear.Clear();

                    truePointsNear.Add((Math.PI, cutLat));

                    truePointsNear.Add((curLonRad, curLatRad));
                }
            }
            else
            {
                truePointsNear.Add((curLonRad, curLatRad));
            }

            (oldLonRad, oldLatRad) = (curLonRad, curLatRad);
        }

        if (truePointsNear.Count != 0)
        {
            NearPoints.Add(truePointsNear.ToList());
            truePointsNear.Clear();
        }

        (oldLonRad, oldLatRad) = far[0];

        for (int i = 0; i < far.Count; i++)
        {
            var (curLonRad, curLatRad) = far[i];

            if (Math.Abs(curLonRad - oldLonRad) >= 3.2)
            {
                double cutLat = LinearInterpDiscontLat(oldLonRad, oldLatRad, curLonRad, curLatRad);

                if (oldLonRad > 0.0)
                {
                    truePointsFar.Add((Math.PI, cutLat));
                    FarPoints.Add(truePointsFar.ToList());
                    truePointsFar.Clear();

                    truePointsFar.Add((-Math.PI, cutLat));

                    truePointsFar.Add((curLonRad, curLatRad));
                }
                else
                {
                    truePointsFar.Add((-Math.PI, cutLat));
                    FarPoints.Add(truePointsFar.ToList());
                    truePointsFar.Clear();

                    truePointsFar.Add((Math.PI, cutLat));

                    truePointsFar.Add((curLonRad, curLatRad));
                }
            }
            else
            {
                truePointsFar.Add((curLonRad, curLatRad));
            }

            (oldLonRad, oldLatRad) = (curLonRad, curLatRad);
        }

        if (truePointsFar.Count != 0)
        {
            FarPoints.Add(truePointsFar.ToList());
            truePointsFar.Clear();
        }

        SwathCore2DInit(NearPoints, FarPoints, isCoverPolis);
    }

    public static double ExtrudeStep => _defaultExtrudeStep;

    public List<List<(double lonRad, double latRad)>> CreateShapes(bool clockwise, bool extrudeMode = false)
    {
        InitTempVectors();

        var shapes = new List<List<(double lonRad, double latRad)>>();

        while (GetShape(clockwise, out var point))
        {
            shapes.Add(new List<(double lonRad, double latRad)>());

            while (NextStep(point!, clockwise, out var nextPoint, out List<(double lonRad, double latRad)>? data) == true)
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
                            var first = data.First();

                            int signLat = data.Count == 2 ? first.latRad > 0.0 ? 1 : -1 : 0;
                            int signLon = first.lonRad > 0.0 ? 1 : -1;

                            shapes.Last().Add((first.lonRad + signLon * ExtrudeStep, first.latRad + signLat * ExtrudeStep));
                        }

                        if (data.Count != 2)
                        {
                            shapes.Last().AddRange(data);
                        }

                        if (nextPoint!.Type == FixPoint.EType.Left || nextPoint.Type == FixPoint.EType.Right)
                        {
                            var last = data.Last();

                            int signLat = data.Count == 2 ? last.latRad > 0.0 ? 1 : -1 : 0;
                            int signLon = last.lonRad > 0.0 ? 1 : -1;

                            shapes.Last().Add((last.lonRad + signLon * ExtrudeStep, last.latRad + signLat * ExtrudeStep));
                        }
                    }
                }

                point = nextPoint;
            }
        }

        return shapes;
    }

    private void SwathCore2DInit(List<List<(double lonRad, double latRad)>> near, List<List<(double lonRad, double latRad)>> far, Func<double, bool> isCoverPolis)
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

        if (Math.Abs(beg1.Fix.lonRad - beg2.Fix.lonRad) > Math.PI)
        {
            double latCut = LinearInterpDiscontLat(beg1.Fix.lonRad, beg1.Fix.latRad, beg2.Fix.lonRad, beg2.Fix.latRad);

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

        if (Math.Abs(end1.Fix.lonRad - end2.Fix.lonRad) > Math.PI)
        {
            double latCut = LinearInterpDiscontLat(end1.Fix.lonRad, end1.Fix.latRad, end2.Fix.lonRad, end2.Fix.latRad);

            EndCut(end1, latCut);
            EndCut(end2, latCut);
        }
    }

    private void BeginCut(FixPoint beg, double latCut)
    {
        FixPoint? neww;
        if (beg.Fix.lonRad > 0.0)
        {
            neww = new FixPoint((Math.PI, latCut), FixPoint.EType.Right);
        }
        else
        {
            neww = new FixPoint((-Math.PI, latCut), FixPoint.EType.Left);
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
        if (end.Fix.lonRad > 0.0)
        {
            neww = new FixPoint((Math.PI, latCut), FixPoint.EType.Right);
        }
        else
        {
            neww = new FixPoint((-Math.PI, latCut), FixPoint.EType.Left);
        }

        SwathSegment? bs = GetDictionaryIndex(end);

        if (bs != null)
        {
            bs.Seg = SwathSegment.Segment.Full;
            bs.AddLast(neww.Fix);
            _dict[bs] = Tuple.Create(_dict[bs].Item1, neww);
        }
    }

    private void AddSegment(List<(double lonRad, double latRad)> arr, SegmentType type)
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

        _leftPoints = _leftPoints.OrderBy(s => s.Fix.latRad).ToList();

        _rightPoints.AddRange(_dict.Select(s => s.Value.Item1).Where(s => s.Type == FixPoint.EType.Right));
        _rightPoints.AddRange(_dict.Select(s => s.Value.Item2).Where(s => s.Type == FixPoint.EType.Right));

        _rightPoints = _rightPoints.OrderBy(s => s.Fix.latRad).ToList();

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

    private FixPoint GetData(SwathSegment segment, FixPoint asBegin, out List<(double lonRad, double latRad)> data)
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

    private bool NextStep(FixPoint point, bool clockwise, out FixPoint? nextPoint, out List<(double lonRad, double latRad)>? data)
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
    private static double LinearInterpDiscontLat(double lonRad1, double latRad1, double lonRad2, double latRad2)
    {
        // one longitude should be negative one positive, make them both positive  
        if (lonRad1 > lonRad2)
        {
            lonRad2 += 2 * Math.PI; // in radians
        }
        else
        {
            lonRad1 += 2 * Math.PI;
        }

        return latRad1 + (Math.PI - lonRad1) * (latRad2 - latRad1) / (lonRad2 - lonRad1);
    }

    private class FixPoint
    {
        public FixPoint((double lonRad, double latRad) point)
        {
            if (point.lonRad.Equals(-Math.PI))
            {
                Fix = point;
                Type = EType.Left;
            }
            else if (point.lonRad.Equals(Math.PI))
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

        public FixPoint((double lonRad, double latRad) point, EType type)
        {
            Fix = point;
            Type = type;

            id = ++classCounter;
        }

        public FixPoint(double lon, double lat) : this((lon, lat)) { }

        public enum EType { Begin, Left, Right, End }

        public EType Type { get; }

        public (double lonRad, double latRad) Fix { get; }

        private static int classCounter = 0;

        public static void ResetId() { classCounter = 0; }

        private readonly int id;

        public override string ToString()
        {
            return string.Format("FixPoint {0:00}({1},Lat={2:0,0})", id, Enum.GetName(typeof(EType), Type), Fix.latRad * SpaceMath.RadiansToDegrees);
        }

        public string ShortDescription()
        {
            return string.Format("FixPoint {0:00}", id);
        }
    }

    private class SwathSegment
    {
        public static List<(double lonRad, double latRad)> TopArr = new() { (-Math.PI, SpaceMath.HALFPI), (Math.PI, SpaceMath.HALFPI) };
        public static List<(double lonRad, double latRad)> TopReverseArr = new() { (Math.PI, SpaceMath.HALFPI), (-Math.PI, SpaceMath.HALFPI) };
        public static List<(double lonRad, double latRad)> BottomArr = new() { (-Math.PI, -SpaceMath.HALFPI), (Math.PI, -SpaceMath.HALFPI) };
        public static List<(double lonRad, double latRad)> BottomReverseArr = new() { (Math.PI, -SpaceMath.HALFPI), (-Math.PI, -SpaceMath.HALFPI) };

        public SwathSegment(IList<(double lonRad, double latRad)> arr, Segment eseg) : this()
        {
            Seg = eseg;

            data = new LinkedList<(double lonRad, double latRad)>(arr);
        }

        protected SwathSegment(IList<(double lonRad, double latRad)> arr) : this()
        {
            Seg = Segment.Full;

            data = new LinkedList<(double lonRad, double latRad)>(arr);
        }

        protected SwathSegment() { id = ++ClassCounter; }

        public enum Segment { Begin, Full, End };

        public Segment Seg { get; set; }

        public int Length { get { return data.Count; } }

        public (double lonRad, double latRad) Begin
        {
            get
            {
                return data.First();
            }
        }

        public (double lonRad, double latRad) End
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

        public List<(double lonRad, double latRad)> NewData { get { return new List<(double lonRad, double latRad)>(data); } }
        public List<(double lonRad, double latRad)> NewReverseData { get { var temp = new List<(double lonRad, double latRad)>(data); temp.Reverse(); return temp; } }

        private readonly LinkedList<(double lonRad, double latRad)> data = new();

        public void AddFirst((double lonRad, double latRad) point)
        {
            data.AddFirst(point);
        }

        public void AddLast((double lonRad, double latRad) point)
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
