using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.Tips;

public class CustomTipViewModel : ViewModelBase
{
    protected CustomTipViewModel()
    {

    }

    public TipTarget? Target { get; init; }

    public TipMode? Mode { get; init; }

    public IObservable<CustomTipViewModel> ValueObservable => this.WhenAnyValue(s => s.Value).Select(s => this);

    [Reactive]
    public object? Value { get; set; }

    public static CustomTipViewModel Init(TipTarget target, object? value = null)
    {
        return new CustomTipViewModel()
        {
            Target = target,
            Mode = TipMode.Init,
            Value = value
        };
    }

    public static CustomTipViewModel HoverCreating(TipTarget target, object? value = null)
    {
        return new CustomTipViewModel()
        {
            Target = target,
            Mode = TipMode.HoverCreating,
            Value = value
        };
    }

    public static CustomTipViewModel BeginCreating(TipTarget target, object? value = null)
    {
        return new CustomTipViewModel()
        {
            Target = target,
            Mode = TipMode.BeginCreating,
            Value = value
        };
    }

    public static CustomTipViewModel Creating(TipTarget target, object? value = null)
    {
        return new CustomTipViewModel()
        {
            Target = target,
            Mode = TipMode.Creating,
            Value = value
        };
    }
}
