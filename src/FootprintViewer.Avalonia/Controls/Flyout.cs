using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Controls
{
    public enum FlyoutPlacementMode
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public class Flyout : ContentControl
    {
        private Grid? _overlay = null;
        private FlyoutBase? _flyoutBase;

        public Flyout()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            IsVisible = false;
        }

        private Grid? Overlay => _overlay;

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Flyout.IsOpen))
            {
                if (_overlay != null)
                {
                    _overlay.PropertyChanged -= Overlay_SizeChanged;
                }

                var parentWindow = WindowsManager.AllWindows.FirstOrDefault();

                if (parentWindow == null)
                {
                    // HACK: exception in designer mode
                    //throw new Exception("Not find parent Window.");
                    return;
                }

                _overlay = (Grid)parentWindow.Content;

                if (_overlay == null)
                {
                    throw new Exception("Not find parent Grid.");
                }

                _overlay.PropertyChanged += Overlay_SizeChanged;

                InvalidateData();
            }
        }

        private void InvalidateData()
        {
            if (Target != null && Overlay != null)
            {
                if (IsOpen == true)
                {
                    var offset = 0.0;

                    var w = Target.Bounds.Width;
                    var h = Target.Bounds.Height;
                    var W = Overlay.Bounds.Width;
                    var H = Overlay.Bounds.Height;

                    var tp = Target.TranslatePoint(new Point(0, 0), Overlay);

                    if (tp == null)
                    {
                        return;
                    }

                    var p = tp.Value;

                    Thickness margin = new Thickness();

                    switch (Placement)
                    {
                        case FlyoutPlacementMode.Left:
                        {
                            offset = h / 2.0;
                            margin = new Thickness(0, p.Y, W - p.X, 0);
                            break;
                        }
                        case FlyoutPlacementMode.Top:
                        {
                            offset = w / 2.0;
                            margin = new Thickness(p.X, 0, 0, H - p.Y);
                            break;
                        }
                        case FlyoutPlacementMode.Right:
                        {
                            offset = h / 2.0;
                            margin = new Thickness(p.X + w, p.Y, 0, 0);
                            break;
                        }
                        case FlyoutPlacementMode.Bottom:
                        {
                            offset = w / 2.0;
                            margin = new Thickness(p.X, p.Y + h, 0, 0);
                            break;
                        }
                        default:
                            break;
                    }

                    _flyoutBase = new FlyoutBase()
                    {
                        Content = new ViewModelViewHost() { ViewModel = Content, },                        
                        Margin = margin,
                        Background = Background,
                        Placement = Placement,
                        ArrowOffset = offset
                    };

                    Overlay.Children.Add(_flyoutBase);
                }
                else
                {
                    Overlay.Children.Remove(_flyoutBase);
                }
            }
        }

        private void Overlay_SizeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (IsOpen == true && Target != null && Overlay != null && _flyoutBase != null)
            {
                if (e.Property.Name == nameof(Grid.Bounds))
                {
                    var w = Target.Bounds.Width;
                    var h = Target.Bounds.Height;
                    var W = Overlay.Bounds.Width;
                    var H = Overlay.Bounds.Height;

                    var tp = Target.TranslatePoint(new Point(0, 0), Overlay);

                    if (tp == null)
                    {
                        return;
                    }

                    var p = tp.Value;

                    switch (Placement)
                    {
                        case FlyoutPlacementMode.Left:
                        {
                            _flyoutBase.Margin = new Thickness(0, p.Y, W - p.X, 0);

                            break;
                        }
                        case FlyoutPlacementMode.Top:
                        {
                            _flyoutBase.Margin = new Thickness(p.X, 0, 0, H - p.Y);
                            break;
                        }
                        case FlyoutPlacementMode.Right:
                        {
                            _flyoutBase.Margin = new Thickness(p.X + w, p.Y, 0, 0);
                            break;
                        }
                        case FlyoutPlacementMode.Bottom:
                        {
                            _flyoutBase.Margin = new Thickness(p.X, p.Y + h, 0, 0);
                            break;
                        }
                        default:
                            break;
                    }
                }
            }
        }

        public Control? Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly StyledProperty<Control?> TargetProperty =
            AvaloniaProperty.Register<Flyout, Control?>("Target", null);

        public FlyoutPlacementMode Placement
        {
            get { return GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly StyledProperty<FlyoutPlacementMode> PlacementProperty =
            AvaloniaProperty.Register<Flyout, FlyoutPlacementMode>("Placement", FlyoutPlacementMode.Bottom);

        public bool IsOpen
        {
            get { return GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<Flyout, bool>("IsOpen", false);
    }
}
