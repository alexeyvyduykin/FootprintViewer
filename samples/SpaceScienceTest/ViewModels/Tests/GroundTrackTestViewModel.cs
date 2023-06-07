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
    private readonly QueueLayer _layer;

    public GroundTrackTestViewModel() : this(MainWindowViewModel.Instance) { }

    public GroundTrackTestViewModel(MainWindowViewModel viewModel)
    {
        _layer = (QueueLayer)viewModel.Map.Layers.FindLayer(MainWindowViewModel.TestKey).FirstOrDefault()!;

        Update = ReactiveCommand.Create(UpdateImpl);

        this.WhenAnyValue(s => s.Node)
            .Select(_ => Unit.Default)
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    [Reactive]
    public int Node { get; set; }

    private void UpdateImpl()
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

        var trackLine = track.GetTrack(Node, LonConverters.Default).ToCutList();
        var trackFeature = trackLine.Select(s => s.ToLineStringFeature("FootprintTrack"));

        _layer.Clear();
        _layer.AddRange(trackFeature);
        _layer.Add(FeatureBuilder.CreateArrow(trackLine.Last()));
        _layer.DataHasChanged();
    }
}
