using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpaceScience.Model;
using SpaceScienceTest.Builders;
using SpaceScienceTest.Layers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SpaceScienceTest.ViewModels.Tests;

public class FootprintTestViewModel : ViewModelBase
{
    private readonly QueueLayer _layer1;

    public FootprintTestViewModel() : this(MainWindowViewModel.Instance) { }

    public FootprintTestViewModel(MainWindowViewModel viewModel)
    {
        _layer1 = (QueueLayer)viewModel.Map.Layers.FindLayer(MainWindowViewModel.FootprintTestKey1).FirstOrDefault()!;

        Update1 = ReactiveCommand.Create(UpdateTest1);

        this.WhenAnyValue(s => s.Node, s => s.U, s => s.IsLeft)
            .Select(_ => Unit.Default)
            .InvokeCommand(Update1);
    }

    public ReactiveCommand<Unit, Unit> Update1 { get; }

    [Reactive]
    public int Node { get; set; } = 1;

    [Reactive]
    public double U { get; set; } = 0.0;

    [Reactive]
    public bool IsLeft { get; set; } = true;

    [Reactive]
    public bool IsRight { get; set; }

    private void UpdateTest1()
    {
        var satellite = new Satellite()
        {
            Name = "Satellite1",
            Semiaxis = 6945.03,
            Eccentricity = 0.0,
            InclinationDeg = 97.65,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = 0.0,
            RightAscensionAscendingNodeDeg = 281.62765470484646,
            Period = 5760.0,
            Epoch = new DateTime(2007, 7, 1, 12, 0, 0),
            LookAngleDeg = 40.0,
            RadarAngleDeg = 16.0
        };

        var orbit = satellite.ToOrbit();

        var dir = (IsLeft) ? SpaceScience.Model.SwathDirection.Left : SpaceScience.Model.SwathDirection.Right;

        var swath = new Swath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, dir);
        var (_, center, border) = FootprintBuilder.GetRandomFootprint(orbit, swath, Node, U);

        var footprint = new Footprint()
        {
            Name = $"Footprint0345",
            Center = center,
            Border = new LineString(border),
            // Node = Node,
            // Direction = direction,
        };

        var feature = FeatureBuilder.Build(footprint);

        _layer1.Clear();
        _layer1.Add(feature);
        _layer1.DataHasChanged();
    }
}
