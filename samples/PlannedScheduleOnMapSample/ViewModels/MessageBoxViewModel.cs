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
           Node      = {feature["Node"]}
""";

        Text = text;
    }

    [Reactive]
    public string Text { get; set; } = string.Empty;
}
