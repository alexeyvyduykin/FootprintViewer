using Avalonia;
using Avalonia.Input;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels;
using Mapsui;
using Mapsui.Interactivity.UI;
using Mapsui.UI.Avalonia;

namespace FootprintViewer.UI;

public class UserMapControl : MapControl
{
    private bool _isGrabbing = false;
    private Cursor? _grabHandCursor;
    private CursorType _currentCursorType = CursorType.Default;

    public UserMapControl() : base()
    {
        EffectiveViewportChanged += UserMapControl_EffectiveViewportChanged;

        MouseWheelAnimation.Duration = 850;

        // TODO: hack
        var mapService = Services.Locator.GetRequiredService<IMapService>();
        this.Map = (Map)mapService.Map;
        mapService.SetNavigator(this.Navigator!);
        mapService.SetViewport(this.Viewport!);
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

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (_isGrabbing == true)
        {
            _isGrabbing = false;

            if (e.Handled == false)
            {
                SetCursor(CursorType.Default);
            }
        }

        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (e.Handled == false)
        {
            var isLeftMouseDown = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            ScaleMapBar?.ChangedViewport(Viewport);

            if (isLeftMouseDown == true)
            {
                if (_isGrabbing == false)
                {
                    _isGrabbing = true;

                    SetCursor(CursorType.HandGrab);
                }
            }
            else
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

    public void SetCursor(CursorType cursorType)
    {
        if (_currentCursorType == cursorType)
        {
            return;
        }

        Cursor = cursorType switch
        {
            CursorType.Default => new Cursor(StandardCursorType.Arrow),
            CursorType.Hand => new Cursor(StandardCursorType.Hand),
            CursorType.HandGrab => (_grabHandCursor ??= Services2.CursorService.GetGrabHandCursor()),
            CursorType.Cross => new Cursor(StandardCursorType.Cross),
            _ => throw new Exception(),
        };

        _currentCursorType = cursorType;
    }
}
