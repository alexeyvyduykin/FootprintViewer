using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Styles;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Items;

public class GroundStationViewModel : ViewModelBase, IViewerItem
{
    private readonly SourceList<GroundStationAreaViewModel> _gsAreas = new();
    private readonly ReadOnlyObservableCollection<GroundStationAreaViewModel> _items;
    private readonly Coordinate _center;
    private readonly string _name;
    private readonly double[] _defaultAngles;
    private readonly IObservable<GroundStationViewModel> _updateObservable;
    private readonly IList<string> _availableCountModes = new[] { "None", "Equal", "Geometric" };
    private readonly IList<int> _availableAreaCounts = new int[] { 1, 2, 3, 4, 5 };

    public GroundStationViewModel(GroundStation groundStation)
    {
        _name = groundStation.Name!;
        _center = groundStation.Center.Coordinate.Copy();
        _defaultAngles = groundStation.Angles;

        CountMode = "None";

        InnerAngle = groundStation.Angles.First();

        OuterAngle = groundStation.Angles.Last();

        AreaCount = groundStation.Angles.Length - 1;

        _gsAreas
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        this.WhenAnyValue(s => s.InnerAngle)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(value =>
            {
                if (value >= OuterAngle)
                {
                    InnerAngle -= 1;
                }
            });

        this.WhenAnyValue(s => s.OuterAngle)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(value =>
            {
                if (value == 0)
                {
                    OuterAngle = 1;
                }

                if (value <= InnerAngle)
                {
                    InnerAngle = value - 1;
                }
            });

        this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount, s => s.CountMode)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => CreateAngles(s.Item1, s.Item2, s.Item3, s.Item4))
            .InvokeCommand(ReactiveCommand.Create<double[]>(UpdateAreas));

        var observable1 = this.WhenAnyValue(s => s.IsShow)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);

        var observable2 = this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount, s => s.CountMode)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1.5))
            .Select(_ => this);

        _updateObservable = Observable.Merge(observable1, observable2);

        Observable.Start(() => UpdateAreas(_defaultAngles.Skip(1).ToArray()), RxApp.MainThreadScheduler).Subscribe();
    }

    public IObservable<GroundStationViewModel> UpdateObservable => _updateObservable;

    private void UpdateAreas(double[] angles)
    {
        var list = angles.Select((angle, index) => BuildGroundStationArea(angle, index, angles.Length));

        _gsAreas.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });

        static GroundStationAreaViewModel BuildGroundStationArea(double angle, int index, int count)
        {
            var color = LayerStyleManager.GroundStationPalette.GetColor(index, count);

            return new GroundStationAreaViewModel()
            {
                Color = new Mapsui.Styles.Color(color.R, color.G, color.B),
                Angle = angle
            };
        }
    }

    private double[] CreateAngles(double innerAngle, double outerAngle, int areaCount, string countMode)
    {
        if (string.Equals(countMode, "Equal") == true)
        {
            return CreateAreaItemsEqualMode(innerAngle, outerAngle, areaCount);
        }
        else if (string.Equals(countMode, "Geometric") == true)
        {
            return CreateAreaItemsGeometricMode(innerAngle, outerAngle, areaCount);
        }
        else if (string.Equals(countMode, "None") == true)
        {
            InnerAngle = _defaultAngles.First();
            OuterAngle = _defaultAngles.Last();
            AreaCount = _defaultAngles.Length - 1;

            return _defaultAngles.Skip(1).ToArray();
        }

        throw new Exception();
    }

    public double[] GetAngles()
    {
        var list = new List<double>
        {
            InnerAngle
        };

        var res = AreaItems.Select(s => s.Angle);

        list.AddRange(res);

        return list.ToArray();
    }

    private static double[] CreateAreaItemsEqualMode(double inner, double outer, int areaCount)
    {
        var areaStep = (outer - inner) / areaCount;

        var list = new List<double>();

        for (int i = 0; i < areaCount; i++)
        {
            var angle = inner + areaStep * (i + 1);

            list.Add(angle);
        }

        return list.ToArray();
    }

    private static double[] CreateAreaItemsGeometricMode(double inner, double outer, int areaCount)
    {
        var list = new List<double>();

        for (int i = 0; i < areaCount; i++)
        {
            var step = (outer - inner) * GetGeometricStep(i + 1, areaCount);

            var angle = inner + step;

            list.Add(angle);
        }

        return list.ToArray();
    }

    private static double GetGeometricStep(int n, int count)
    {
        if (n == count)
        {
            return 1.0;
        }

        double value = 0.0;

        for (int i = 0; i < n; i++)
        {
            value += 1 / Math.Pow(2, i + 1);
        }

        return value;
    }

    public ReadOnlyObservableCollection<GroundStationAreaViewModel> AreaItems => _items;

    public string Name => _name;

    public Coordinate Center => _center;

    [Reactive]
    public double InnerAngle { get; set; }

    [Reactive]
    public double OuterAngle { get; set; }

    [Reactive]
    public string CountMode { get; set; }

    public IList<string> AvailableCountModes => _availableCountModes;

    [Reactive]
    public int AreaCount { get; set; }

    public IList<int> AvailableAreaCounts => _availableAreaCounts;

    [Reactive]
    public bool IsShow { get; set; } = false;

    public bool IsShowInfo { get; set; }
}
