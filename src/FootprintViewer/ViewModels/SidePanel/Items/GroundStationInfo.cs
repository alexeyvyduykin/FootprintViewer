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
        public Mapsui.Styles.Color? Color { get; set; }

        public double Angle { get; set; }
    }

    public class GroundStationInfo : ReactiveObject
    {
        private readonly Coordinate _center;
        private readonly string _name;
        private readonly string _countMode;

        public GroundStationInfo(GroundStation groundStation)
        {
            _name = groundStation.Name!;
            _center = groundStation.Center.Coordinate.Copy();
            _countMode = "Equal";

            InnerAngle = groundStation.Angles.First();

            OuterAngle = groundStation.Angles.Last();

            AreaCount = groundStation.Angles.Length - 1;

            AreaItems = CreateAreaItems(InnerAngle, OuterAngle, AreaCount);

            Change = ReactiveCommand.Create(ChangeImpl);
            Update = ReactiveCommand.Create(UpdateImpl);

            this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount)
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

            this.WhenAnyValue(s => s.InnerAngle, s => s.OuterAngle, s => s.AreaCount)
                .Select(s => CreateAreaItems(s.Item1, s.Item2, s.Item3))
                .ToPropertyEx(this, x => x.AreaItems);
        }

        public double[] GetAngles()
        {
            var list = new List<double>
            {
                InnerAngle
            };

            list.AddRange(AreaItems.Select(s => s.Angle));

            return list.ToArray();
        }

        private static List<GroundStationAreaItem> CreateAreaItems(double inner, double outer, int areaCount)
        {
            var areaStep = (outer - inner) / areaCount;

            var list = new List<GroundStationAreaItem>();

            var colors = LayerStyleManager.GetGroundTargetPalette(areaCount);

            for (int i = 1; i < areaCount + 1; i++)
            {
                var angle = inner + areaStep * i;

                list.Add(new GroundStationAreaItem()
                {
                    Color = colors[i - 1],
                    Angle = angle,
                });
            }

            return list;
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

        public string CountMode => _countMode;

        public IList<string> AvailableCountModes => new[] { "Equal", "Radial" };

        [Reactive]
        public int AreaCount { get; set; }

        public IList<int> AvailableAreaCounts => new int[] { 1, 2, 3, 4, 5 };

        [Reactive]
        public bool IsShow { get; set; } = false;
    }
}
