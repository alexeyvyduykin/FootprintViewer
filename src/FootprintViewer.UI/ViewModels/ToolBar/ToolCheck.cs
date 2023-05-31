using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public class ToolCheck : ViewModelBase, IToolCheck
{
    bool _stop = false;

    public ToolCheck() { }

    public ToolCheck(IObservable<Unit> callback, Action? selector, Func<bool>? validator)
    {
        callback
            .Subscribe(s =>
            {
                _stop = true;
                IsCheck = validator?.Invoke() ?? false;
                _stop = false;
            });

        this.WhenAnyValue(s => s.IsCheck)
            .Skip(1)
            .Where(_ => _stop == false)
            .Subscribe(_ => selector?.Invoke());
    }

    public IObservable<IToolCheck> Activate
        => this.WhenAnyValue(s => s.IsCheck)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Where(s => s == true)
        .Select(_ => this);

    public IObservable<Unit> Deactivate
        => this.WhenAnyValue(s => s.IsCheck)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Where(s => s == false)
        .Select(_ => Unit.Default);

    public string? Key { get; set; }// GetKey() => Tag is string tag ? tag : string.Empty;

    [Reactive]
    public bool IsCheck { get; set; } = false;

    public object? Tag { get; set; }
}
