using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.InfoPanel;

public abstract class InfoPanelItem : ReactiveObject
{
    public InfoPanelItem()
    {
        //Close = ReactiveCommand.Create(() => this);

        Close = ReactiveCommand.CreateFromObservable(() => Observable.Return(this));
    }

    public ReactiveCommand<Unit, InfoPanelItem> Close { get; protected set; }
}
