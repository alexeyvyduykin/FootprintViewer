using Avalonia;
using Avalonia.Input;
using Mapsui.UI.Avalonia;
using SpaceScienceSample.Models;
using SpaceScienceSample.ViewModels;
using System;

namespace SpaceScienceSample;

public class UserMapControl : MapControl
{
    public UserMapControl() : base()
    {
        MapNavigatorProperty.Changed.Subscribe(OnMapNavigatorChanged);

        EffectiveViewportChanged += UserMapControl_EffectiveViewportChanged;
    }

    private void UserMapControl_EffectiveViewportChanged(object? sender, global::Avalonia.Layout.EffectiveViewportChangedEventArgs e)
    {
        ScaleMapBar?.ChangedViewport(Viewport);
    }

    public ScaleMapBar? ScaleMapBar
    {
        get { return GetValue(ScaleMapBarProperty); }
        set { SetValue(ScaleMapBarProperty, value); }
    }

    public static readonly StyledProperty<ScaleMapBar?> ScaleMapBarProperty =
        AvaloniaProperty.Register<UserMapControl, ScaleMapBar?>(nameof(ScaleMapBar), null);

    public IMapNavigator? MapNavigator
    {
        get { return GetValue(MapNavigatorProperty); }
        set { SetValue(MapNavigatorProperty, value); }
    }

    public static readonly StyledProperty<IMapNavigator?> MapNavigatorProperty =
        AvaloniaProperty.Register<UserMapControl, IMapNavigator?>(nameof(MapNavigator), null);

    private static void OnMapNavigatorChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var mapControl = (UserMapControl)e.Sender;

        if (e.NewValue != null && e.NewValue is IMapNavigator mapNavigator)
        {
            if (mapControl.Navigator != null)
            {
                mapNavigator.Navigator = mapControl.Navigator;
                mapNavigator.Viewport = mapControl.Viewport;

                mapControl.MouseWheelAnimation.Duration = 850;
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var isMiddleMouseDown = e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed;

        if (isMiddleMouseDown == true)
        {
            var position = e.GetPosition(this);

            var worldPosition = Viewport.ScreenToWorld(position.X, position.Y);

            MapNavigator?.Click(worldPosition);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (e.Handled == false)
        {
            var isLeftMouseDown = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            ScaleMapBar?.ChangedViewport(Viewport);

            if (isLeftMouseDown == false)
            {
                var position = e.GetPosition(this);

                var worldPosition = Viewport.ScreenToWorld(position.X, position.Y);

                ScaleMapBar?.ChangedPosition(worldPosition);
            }
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        ScaleMapBar?.ChangedViewport(Viewport);
    }
}
