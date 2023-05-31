using FootprintViewer.UI.ViewModels.SidePanel;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.InfoPanel;

public class InfoPanelItemViewModel : ViewModelBase, ISelectorItem
{
    public static InfoPanelItemViewModel Create(string key, string text) => new() { Key = key, Text = text };

    public InfoPanelItemViewModel()
    {
        //Close = ReactiveCommand.Create(() => this);

        Close = ReactiveCommand.CreateFromObservable(() => Observable.Return(this));
    }

    public ReactiveCommand<Unit, InfoPanelItemViewModel> Close { get; protected set; }

    public string? Key { get; set; }

    public string? Text { get; set; }
}
