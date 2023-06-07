using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpaceScience.Extensions;
using SpaceScience.Model;
using SpaceScienceTest.Builders;
using SpaceScienceTest.Extensions;
using SpaceScienceTest.Layers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SpaceScienceTest.ViewModels.Tests;

public class GroundTrackTestViewModel : ViewModelBase
{
    private readonly QueueLayer _layer1;
    private readonly QueueLayer _layer2;

    public GroundTrackTestViewModel() : this(MainWindowViewModel.Instance) { }

    public GroundTrackTestViewModel(MainWindowViewModel viewModel)
    {
        _layer1 = (QueueLayer)viewModel.Map.Layers.FindLayer(MainWindowViewModel.GroundTrackTestKey1).FirstOrDefault()!;
        _layer2 = (QueueLayer)viewModel.Map.Layers.FindLayer(MainWindowViewModel.GroundTrackTestKey2).FirstOrDefault()!;

        this.WhenAnyValue(s => s.IsVisible1).Where(s => s == false).Subscribe(_ => { _layer1.Clear(); _layer1.DataHasChanged(); });
        this.WhenAnyValue(s => s.IsVisible2).Where(s => s == false).Subscribe(_ => { _layer2.Clear(); _layer2.DataHasChanged(); });

        Update1 = ReactiveCommand.Create(UpdateTest1);
        Update2 = ReactiveCommand.Create(UpdateTest2);

        this.WhenAnyValue(s => s.Node1, s => s.IsVisible1)
            .Where(s => s.Item2 == true)
            .Select(_ => Unit.Default)
            .InvokeCommand(Update1);

        this.WhenAnyValue(s => s.BeginTimeSec, s => s.EndTimeSec, s => s.IsVisible2)
            .Where(s => s.Item3 == true)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => Unit.Default)
            .InvokeCommand(Update2);

        this.WhenAnyValue(s => s.Node2, s => s.IsVisible2)
            .Where(s => s.Item2 == true)
            .Select(_ => Unit.Default)
            .InvokeCommand(Update2);
    }

    public ReactiveCommand<Unit, Unit> Update1 { get; }

    public ReactiveCommand<Unit, Unit> Update2 { get; }

    [Reactive]
    public int Node1 { get; set; }

    [Reactive]
    public bool IsVisible1 { get; set; }

    [Reactive]
    public int Node2 { get; set; }

    [Reactive]
    public bool IsVisible2 { get; set; }

    [Reactive]
    public double BeginTimeSec { get; set; } = 0.0;

    [Reactive]
    public double EndTimeSec { get; set; } = 5760.0;

    [Reactive]
    public double Period { get; set; } = 5760.0;

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
        var epoch = satellite.Epoch;
        var period = satellite.Period;
        var radarAngle = satellite.RadarAngleDeg;
        var lookAngle = satellite.LookAngleDeg;

        var features = orbit.BuildTracks().ToFeature("");

        var track = new GroundTrack(orbit);

        track.CalculateTrack(60);

        var trackLine = track.GetTrack(Node1, LonConverters.Default).ToCutList();
        var trackFeature = trackLine.Select(s => s.ToLineStringFeature("Test1"));

        _layer1.Clear();
        _layer1.AddRange(trackFeature);
        _layer1.Add(FeatureBuilder.CreateArrow(trackLine.Last()));
        _layer1.DataHasChanged();
    }

    private void UpdateTest2()
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
        var epoch = satellite.Epoch;
        var period = satellite.Period;
        var radarAngle = satellite.RadarAngleDeg;
        var lookAngle = satellite.LookAngleDeg;

        var features = orbit.BuildTracks().ToFeature("");

        var track = new GroundTrack(orbit);

        track.CalculateTrackOnTimeInterval(BeginTimeSec, EndTimeSec, 60);

        var duration = EndTimeSec - BeginTimeSec;

        var trackLine = track.GetTrack(Node2, duration, LonConverters.Default).ToCutList();
        var trackFeature = trackLine.Select(s => s.ToLineStringFeature("Test2"));

        _layer2.Clear();
        _layer2.AddRange(trackFeature);
        _layer2.Add(FeatureBuilder.CreateArrow(trackLine.Last()));
        _layer2.DataHasChanged();
    }
}
