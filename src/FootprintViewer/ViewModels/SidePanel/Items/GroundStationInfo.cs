using FootprintViewer.Data;
using FootprintViewer.Styles;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundStationAreaItem
    {
        public Mapsui.Styles.Color Color { get; set; } = new Mapsui.Styles.Color();

        public double Angle { get; set; }
    }

    public class GroundStationInfo : ReactiveObject
    {
        private readonly Coordinate _center;
        private readonly string _name;
        private readonly double[] _defaultAngles;

        public GroundStationInfo(GroundStation groundStation)
        {
            _name = groundStation.Name!;
            _center = groundStation.Center.Coordinate.Copy();
            _defaultAngles = groundStation.Angles;

            CountMode = "None";

            InnerAngle = groundStation.Angles.First();

            OuterAngle = groundStation.Angles.Last();

            AreaCount = groundStation.Angles.Length - 1;

            AreaItems = CreateAreaItems(_defaultAngles);

            Change = ReactiveCommand.Create(ChangeImpl);
            Update = ReactiveCommand.Create(UpdateImpl);
            ChangeAreaItems = ReactiveCommand.Create<List<GroundStationAreaItem>, List<GroundStationAreaItem>>(s => s);
            ChangeAreaItems.ToPropertyEx(this, x => x.AreaItems);

            this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount, s => s.CountMode)
                .Throttle(TimeSpan.FromSeconds(1.5))
                .Select(args => Unit.Default)
                .InvokeCommand(this, v => v.Change);

            this.WhenAnyValue(s => s.IsShow)
                .Select(args => Unit.Default)
                .InvokeCommand(this, v => v.Update);

            this.WhenAnyValue(s => s.InnerAngle).Subscribe(value =>
            {
                if (value >= OuterAngle)
                {
                    InnerAngle -= 1;
                }
            });

            this.WhenAnyValue(s => s.OuterAngle).Subscribe(value =>
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
                .Where(s => s.Item4 == "Equal")
                .Select(s => CreateAreaItemsEqualMode(s.Item1, s.Item2, s.Item3))
                .InvokeCommand(ChangeAreaItems);

            this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount, s => s.CountMode)
                .Where(s => s.Item4 == "Geometric")
                .Select(s => CreateAreaItemsGeometricMode(s.Item1, s.Item2, s.Item3))
                .InvokeCommand(ChangeAreaItems);

            this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount, s => s.CountMode)
                .Where(s => s.Item4 == "None")
                .Select(s =>
                {
                    InnerAngle = _defaultAngles.First();
                    OuterAngle = _defaultAngles.Last();
                    AreaCount = _defaultAngles.Length - 1;
                    return CreateAreaItems(_defaultAngles);
                })
                .InvokeCommand(ChangeAreaItems);
        }


        private ReactiveCommand<List<GroundStationAreaItem>, List<GroundStationAreaItem>> ChangeAreaItems { get; }

        public double[] GetAngles()
        {
            var list = new List<double>
            {
                InnerAngle
            };

            list.AddRange(AreaItems.Select(s => s.Angle));

            return list.ToArray();
        }

        private static List<GroundStationAreaItem> CreateAreaItems(double[] angles)
        {
            var areaCount = angles.Length - 1;

            var list = new List<GroundStationAreaItem>();

            for (int i = 0; i < areaCount; i++)
            {
                var angle = angles[i + 1];

                var color = LayerStyleManager.GroundStationPalette.GetColor(i, areaCount);

                list.Add(new GroundStationAreaItem()
                {
                    Color = new Mapsui.Styles.Color(color.R, color.G, color.B),
                    Angle = angle,
                });
            }

            return list;
        }

        private static List<GroundStationAreaItem> CreateAreaItemsEqualMode(double inner, double outer, int areaCount)
        {
            var areaStep = (outer - inner) / areaCount;

            var list = new List<GroundStationAreaItem>();

            for (int i = 0; i < areaCount; i++)
            {
                var angle = inner + areaStep * (i + 1);

                var color = LayerStyleManager.GroundStationPalette.GetColor(i, areaCount);

                list.Add(new GroundStationAreaItem()
                {
                    Color = new Mapsui.Styles.Color(color.R, color.G, color.B),
                    Angle = angle,
                });
            }

            return list;
        }

        private static List<GroundStationAreaItem> CreateAreaItemsGeometricMode(double inner, double outer, int areaCount)
        {
            var list = new List<GroundStationAreaItem>();

            for (int i = 0; i < areaCount; i++)
            {
                var step = (outer - inner) * GetGeometricStep(i + 1, areaCount);

                var angle = inner + step;

                var color = LayerStyleManager.GroundStationPalette.GetColor(i, areaCount);

                list.Add(new GroundStationAreaItem()
                {
                    Color = new Mapsui.Styles.Color(color.R, color.G, color.B),
                    Angle = angle,
                });
            }

            return list;
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

        [ObservableAsProperty]
        public List<GroundStationAreaItem> AreaItems { get; }

        private GroundStationInfo ChangeImpl()
        {
            //var count = AreaCount;
            //var inner = InnerAngle;
            //var outer = OuterAngle;

            //var val = (outer - inner) / count;

            //var list = new List<double>();

            //for (int i = 0; i < count + 1; i++)
            //{
            //    list.Add(inner + val * i);
            //}

            return this;
        }

        private void UpdateImpl()
        {
            return;
        }

        public readonly ReactiveCommand<Unit, GroundStationInfo> Change;
        public readonly ReactiveCommand<Unit, Unit> Update;

        public string Name => _name;

        public Coordinate Center => _center;

        [Reactive]
        public double InnerAngle { get; set; }

        [Reactive]
        public double OuterAngle { get; set; }

        [Reactive]
        public string CountMode { get; set; }

        public IList<string> AvailableCountModes => new[] { "None", "Equal", "Geometric" };

        [Reactive]
        public int AreaCount { get; set; }

        public IList<int> AvailableAreaCounts => new int[] { 1, 2, 3, 4, 5 };

        [Reactive]
        public bool IsShow { get; set; } = false;
    }
}
