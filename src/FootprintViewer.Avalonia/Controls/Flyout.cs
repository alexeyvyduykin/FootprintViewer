using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
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
        private Border _arrow = new Border();
        private ContentControl _contentControl;

        public Flyout()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            
            Initialized += Flyout_Loaded;
        }


        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            
            if (change.Property.Name == nameof(Flyout.IsOpen))
            {
                InvalidateData();            
            }
            else if (change.Property.Name == nameof(Flyout.Target))
            {
                if (_overlay != null)
                {
                    _overlay.PropertyChanged -= _overlay_SizeChanged;
                }

                var parentWindow = WindowsManager.AllWindows.FirstOrDefault();

                if (parentWindow == null)
                {
                    // HACK: exception in designer mode
                    //throw new Exception("Not find parent Window.");
                    return;
                }

                _overlay = (Grid)parentWindow.Content;

                //flyout._overlay = FlyoutExtensions.FindVisualChildren<Grid>(parentWindow).First();

                if (_overlay == null)
                {
                    throw new Exception("Not find parent Grid.");
                }

                _overlay.PropertyChanged += _overlay_SizeChanged;
            }
        }

        private void Flyout_Loaded(object? sender, EventArgs e)
        {
            var offset = 8;
            var a = Math.Sqrt(2) * offset;

            _arrow = new Border()
            {
                Width = a,
                Height = a,
                Name = "FlyoutArrow",
                BorderThickness = new Thickness(0),
                Background = Background,
            };

            _contentControl = new ContentControl()
            {
                Content = ContentSource,         
            };
        }

        private void InvalidateData()
        {
            var target = Target;
            var overlay = _overlay;

            if (target != null && overlay != null)
            {
                if (IsOpen == true)
                {
                    var offset = 8;
                    var a = Math.Sqrt(2) * offset;
                    var b = a / 2.0;
                    var w = target.Bounds.Width;// ActualWidth;
                    var h = target.Bounds.Height;//ActualHeight;
                    var W = overlay.Bounds.Width;//ActualWidth;
                    var H = overlay.Bounds.Height;//ActualHeight;

                    var p = target.TranslatePoint(new Point(0, 0), overlay).Value;

                    //var p = target.TransformToAncestor(overlay).Transform(new Point(0, 0));

                    Thickness marginFlyout = new Thickness(0.0);
                    Thickness marginArrow = new Thickness(0.0);
                    TransformGroup? transform = null;
                    RelativePoint transformPoint = new RelativePoint();
                    HorizontalAlignment horizontal = HorizontalAlignment.Left;
                    VerticalAlignment vertical = VerticalAlignment.Top;

                    switch (Placement)
                    {
                        case FlyoutPlacementMode.Left:
                        {
                            marginFlyout = new Thickness(0, 0, W - p.X + offset, 0);
                            marginArrow = new Thickness(0, p.Y, W - p.X + offset, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(0, p.Y, W - p.X + offset, 0);
                            }

                            horizontal = HorizontalAlignment.Right;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup();
                            transform.Children.AddRange(new Transform[]
                            {
                                new TranslateTransform(b, -b),
                                new RotateTransform(45),
                                new TranslateTransform(0, h / 2.0),
                            });

                            transformPoint = new RelativePoint(new Point(1, 0), RelativeUnit.Relative);
                            break;
                        }
                        case FlyoutPlacementMode.Top:
                        {
                            marginFlyout = new Thickness(0, 0, 0, H - p.Y + offset);
                            marginArrow = new Thickness(p.X, 0, 0, H - p.Y + offset);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, 0, 0, H - p.Y + offset);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Bottom;

                            transform = new TransformGroup();
                            transform.Children.AddRange(new Transform[]
                            {
                                new TranslateTransform(-b, b),
                                new RotateTransform(45),
                                new TranslateTransform(w / 2.0, 0)
                            });

                            transformPoint = new RelativePoint(new Point(0, 1), RelativeUnit.Relative);
                            break;
                        }
                        case FlyoutPlacementMode.Right:
                        {
                            marginFlyout = new Thickness(p.X + w + offset, 0, 0, 0);
                            marginArrow = new Thickness(p.X + w + offset, p.Y, 0, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X + w + offset, p.Y, 0, 0);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup();
                            transform.Children.AddRange(new Transform[]
                            {
                                new TranslateTransform(-b, -b),
                                new RotateTransform(45),
                                new TranslateTransform(0, h / 2.0),
                            });

                            transformPoint = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
                            break;
                        }
                        case FlyoutPlacementMode.Bottom:
                        {
                            marginFlyout = new Thickness(0, p.Y + h + offset, 0, 0);
                            marginArrow = new Thickness(p.X, p.Y + h + offset, 0, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, p.Y + h + offset, 0, 0);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup();
                            transform.Children.AddRange(new Transform[]
                            {
                                new TranslateTransform(-b, -b),
                                new RotateTransform(45),
                                new TranslateTransform(w / 2.0, 0),
                            });

                            transformPoint = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
                            break;
                        }
                        default:
                            break;
                    }

                    _contentControl.HorizontalAlignment = horizontal;
                    _contentControl.VerticalAlignment = vertical;
                    _contentControl.Margin = new Thickness(Margin.Left + marginFlyout.Left, Margin.Top + marginFlyout.Top, Margin.Right + marginFlyout.Right, Margin.Bottom + marginFlyout.Bottom);

                    _arrow.HorizontalAlignment = horizontal;
                    _arrow.VerticalAlignment = vertical;
                    _arrow.RenderTransformOrigin = transformPoint;
                    _arrow.RenderTransform = transform;
                    _arrow.Margin = marginArrow;

                    overlay.Children.Add(_arrow);
                    overlay.Children.Add(_contentControl);
                }
                else
                {
                    overlay.Children.Remove(_arrow);
                    overlay.Children.Remove(_contentControl);          
                }
            }
        }

        private void _overlay_SizeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (IsOpen == true && Target != null && _overlay != null)
            {
                if (e.Property.Name == nameof(Grid.Bounds))
                {
                    var target = Target;
                    var overlay = _overlay;

                    var offset = 8;
                    var w = target.Bounds.Width;// ActualWidth;
                    var h = target.Bounds.Height;//ActualHeight;
                    var W = overlay.Bounds.Width;//ActualWidth;
                    var H = overlay.Bounds.Height;//ActualHeight;

                    var p = target.TranslatePoint(new Point(0, 0), overlay).Value;
                    // var p = target.TransformToAncestor(overlay).Transform(new Point(0, 0));

                    Thickness marginFlyout = new Thickness();
                    Thickness marginArrow = new Thickness();

                    switch (Placement)
                    {
                        case FlyoutPlacementMode.Left:
                        {
                            marginFlyout = new Thickness(0, 0, W - p.X + offset, 0);
                            marginArrow = new Thickness(0, p.Y, W - p.X + offset, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(0, p.Y, W - p.X + offset, 0);
                            }

                            break;
                        }
                        case FlyoutPlacementMode.Top:
                        {
                            marginFlyout = new Thickness(0, 0, 0, H - p.Y + offset);
                            marginArrow = new Thickness(p.X, 0, 0, H - p.Y + offset);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, 0, 0, H - p.Y + offset);
                            }

                            break;
                        }
                        case FlyoutPlacementMode.Right:
                        {
                            marginFlyout = new Thickness(p.X + w + offset, 0, 0, 0);
                            marginArrow = new Thickness(p.X + w + offset, p.Y, 0, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X + w + offset, p.Y, 0, 0);
                            }

                            break;
                        }
                        case FlyoutPlacementMode.Bottom:
                        {
                            marginFlyout = new Thickness(0, p.Y + h + offset, 0, 0);
                            marginArrow = new Thickness(p.X, p.Y + h + offset, 0, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, p.Y + h + offset, 0, 0);
                            }

                            break;
                        }
                        default:
                            break;
                    }

                    _contentControl.Margin = new Thickness(Margin.Left + marginFlyout.Left, Margin.Top + marginFlyout.Top, Margin.Right + marginFlyout.Right, Margin.Bottom + marginFlyout.Bottom);

                    _arrow.Margin = marginArrow;
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

        public object? ContentSource
        {
            get { return GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        public static readonly StyledProperty<object?> ContentSourceProperty =
            AvaloniaProperty.Register<Flyout, object?>("ContentSource", null);

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

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<Flyout, bool>("IsOpen", false);

        public bool IsAnchor
        {
            get { return GetValue(IsAnchorProperty); }
            set { SetValue(IsAnchorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<bool> IsAnchorProperty =
            AvaloniaProperty.Register<Flyout, bool>("IsAnchor", false);
    }

    public static class FlyoutExtensions
    {
        public static T? GetParentOfType<T>(this AvaloniaObject? depObj) where T : AvaloniaObject
        {
            if (depObj == null)
            {
                return null;
            }

            return depObj.GetParentOfType<T>();

            //T res = null;

            //var parent = LogicalTreeHelper.GetParent(depObj);

            //while (parent != null)
            //{
            //    if (parent is T p)
            //    {
            //        res = p;
            //    }

            //    parent = LogicalTreeHelper.GetParent(parent);
            //}

            //return res;
        }

        public static IEnumerable<T> FindVisualChildren<T>(AvaloniaObject? depObj) where T : AvaloniaObject
        {
            if (depObj == null)
            {
                yield break;
            }

            var count = ((IVisual)depObj).VisualChildren.Count;

            for (int i = 0; i < count; i++)
            {
                var child = (AvaloniaObject)((IVisual)depObj).VisualChildren[i];//VisualTreeHelper.GetChild(depObj, i);

                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}
