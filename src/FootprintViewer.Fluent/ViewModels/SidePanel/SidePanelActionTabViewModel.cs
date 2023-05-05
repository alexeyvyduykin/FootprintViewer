using ReactiveUI;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels.SidePanel;

public class SidePanelActionTabViewModel : ViewModelBase
{
    public SidePanelActionTabViewModel() { }

    public SidePanelActionTabViewModel(string key)
    {
        Key = key;
    }

    public SidePanelActionTabViewModel(string key, ICommand command)
    {
        Key = key;
        Command = command;
    }

    public string Key { get; init; } = "Default";

    public ICommand Command { get; init; } = ReactiveCommand.Create(() => { });
}