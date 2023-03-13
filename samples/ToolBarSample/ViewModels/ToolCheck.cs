using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ToolBarSample.ViewModels;

public class ToolCheck : ViewModelBase
{
    bool _stop = false;

    public ToolCheck(IObservable<Unit> update, Action? selector, Func<bool>? validator)
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

    [Reactive]
    public bool IsCheck { get; set; } = false;

    public object? Tag { get; set; }
}

