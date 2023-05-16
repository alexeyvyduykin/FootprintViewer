using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.Events;
using System.Reactive.Disposables;

namespace FootprintViewer.Fluent.Behaviors;

public class CloseFlyoutAfterLostCapture : PointerCaptureLostEventBehavior
{
    protected override void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (sender is ILogical obj)
        {
            var flyout = obj.FindLogicalAncestorOfType<Popup>();

            if (flyout is { })
            {
                flyout.IsOpen = false;
            }
        }
    }
}

public class CloseFlyoutAfterLostCaptureOld : AttachedToVisualTreeBehavior<Control>
{
    private Popup? _flyout;

    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        var obj = AssociatedObject;

        if (obj is null)
        {
            return;
        }

        _flyout = obj.FindLogicalAncestorOfType<Popup>();

        obj.AddDisposableHandler(Control.PointerCaptureLostEvent, OnLost).DisposeWith(disposable);
    }

    private void OnLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_flyout is { })
        {
            _flyout.IsOpen = false;
        }
    }
}
