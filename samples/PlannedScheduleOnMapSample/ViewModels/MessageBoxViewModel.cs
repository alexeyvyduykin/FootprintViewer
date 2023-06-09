using Mapsui;
using ReactiveUI.Fody.Helpers;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MessageBoxViewModel : ViewModelBase
{
    public MessageBoxViewModel()
    {

    }

    public void ShowFootprintFeature(IFeature feature)
    {
        var text = $"""
ClickInfo: Footprint = {feature["Name"]}
           Satellite = {feature["Satellite"]}
           Target    = {feature["Target"]}
           Node      = {feature["Node"]}
           Direction = {feature["Direction"]}
""";

        Text = text;
    }

    [Reactive]
    public string Text { get; set; } = string.Empty;
}
