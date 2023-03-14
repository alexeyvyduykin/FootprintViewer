using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.ToolBar;

public class ToolCheck2 : ViewModelBase, IToolCheck2
{
    bool _stop = false;

    public ToolCheck2()
    {

    }

    public ToolCheck2(IObservable<Unit> update, Action? selector, Func<bool>? validator)
    {
        update
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

    public IObservable<bool> IsCheckObservable => this.WhenAnyValue(s => s.IsCheck);

    public string GetKey() => (string?)Tag ?? string.Empty;

    [Reactive]
    public bool IsCheck { get; set; } = false;

    public object? Tag { get; set; }
}
