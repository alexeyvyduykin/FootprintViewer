using ReactiveUI.Fody.Helpers;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MessageBoxViewModel : ViewModelBase
{
    public MessageBoxViewModel()
    {

    }

    public void Show(string text)
    {
        Text = text;
    }

    [Reactive]
    public string Text { get; set; } = string.Empty;
}
