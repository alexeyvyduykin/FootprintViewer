using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
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
        private Border _border = new Border();
        private Border _arrow = new Border();

        public Flyout()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            TargetProperty.Changed.Subscribe(OnTargetChanged);
            IsOpenProperty.Changed.Subscribe(OnDataChanged);

            Initialized += Flyout_Loaded;
        }

        private void Flyout_Loaded(object? sender, EventArgs e)
        {
            var offset = 8;
            var a = Math.Sqrt(2) * offset;

            _border = new Border()
            {
                Name = "FlyoutBorder",
                BorderThickness = new Thickness(0),
                Background = Background,
            };

            //_border.SizeChanged += _border_SizeChanged;

            _arrow = new Border()
            {
                Width = a,
                Height = a,
                Name = "FlyoutArrow",
                BorderThickness = new Thickness(0),
                Background = Background,
            };

            _border.Child = new ContentControl()
            {
                Content = Content,
                ContentTemplate = FlyoutTemplate
            };

            InvalidateData();
        }

        //private void _border_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var border = (Border)sender;

        //    if (border != null)
        //    {
        //        if (Placement == FlyoutPlacementMode.Left)
        //        {
        //            var offset = 8;
        //            var a = Math.Sqrt(2) * offset;
        //            var b = a / 2.0;
        //            var w = Target.ActualWidth;
        //            var h = Target.ActualHeight;
        //            var W = _overlay.ActualWidth;
        //            var H = _overlay.ActualHeight;

        //            var vd = 0.0;
        //            if (VerticalAlignment == VerticalAlignment.Center)
        //            {
        //                var h1 = border.ActualHeight;

        //                vd = h1 / 2.0 - h / 2.0;

        //                vd = -vd;
        //            }


        //            var p = Target.TransformToAncestor(_overlay).Transform(new Point(0, 0));

        //            var marginFlyout = new Thickness(0, p.Y + vd, W - p.X + offset, 0);
        //            var marginArrow = new Thickness(0, p.Y, W - p.X + offset, 0);

        //            var horizontal = HorizontalAlignment.Right;
        //            var vertical = VerticalAlignment.Top;

        //            var transform = new TransformGroup()
        //            {
        //                Children = new TransformCollection()
        //                    {
        //                        new TranslateTransform(b, -b),
        //                        new RotateTransform(45),
        //                        new TranslateTransform(0, h / 2.0),
        //                    }
        //            };

        //            var transformPoint = new Point(1, 0);

        //            _border.HorizontalAlignment = horizontal;
        //            _border.VerticalAlignment = vertical;
        //            _border.Margin = new Thickness(Margin.Left + marginFlyout.Left, Margin.Top + marginFlyout.Top, Margin.Right + marginFlyout.Right, Margin.Bottom + marginFlyout.Bottom);

        //            _arrow.HorizontalAlignment = horizontal;
        //            _arrow.VerticalAlignment = vertical;
        //            _arrow.RenderTransformOrigin = transformPoint;
        //            _arrow.RenderTransform = transform;
        //            _arrow.Margin = marginArrow;
        //        }
        //    }
        //}

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
                    TransformGroup transform = null;
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
                    _border.HorizontalAlignment = horizontal;
                    _border.VerticalAlignment = vertical;
                    _border.Margin = new Thickness(Margin.Left + marginFlyout.Left, Margin.Top + marginFlyout.Top, Margin.Right + marginFlyout.Right, Margin.Bottom + marginFlyout.Bottom);
                    _arrow.HorizontalAlignment = horizontal;
                    _arrow.VerticalAlignment = vertical;
                    _arrow.RenderTransformOrigin = transformPoint;
                    _arrow.RenderTransform = transform;
                    _arrow.Margin = marginArrow;
                    overlay.Children.Add(_arrow);
                    overlay.Children.Add(_border);
                }
                else
                {
                    overlay.Children.Remove(_arrow);
                    overlay.Children.Remove(_border);
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

                    _border.Margin = new Thickness(Margin.Left + marginFlyout.Left, Margin.Top + marginFlyout.Top, Margin.Right + marginFlyout.Right, Margin.Bottom + marginFlyout.Bottom);
                    _arrow.Margin = marginArrow;
                }
            }
        }

        public DataTemplate? FlyoutTemplate
        {
            get { return GetValue(FlyoutTemplateProperty); }
            set { SetValue(FlyoutTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlyoutTemplate.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<DataTemplate?> FlyoutTemplateProperty =
            AvaloniaProperty.Register<Flyout, DataTemplate?>("FlyoutTemplate", null);

        public Control? Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly StyledProperty<Control?> TargetProperty =
            AvaloniaProperty.Register<Flyout, Control?>("Target", null);

        private static void OnTargetChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)e.Sender;

            if (e.NewValue is Control target)
            {
                if (flyout._overlay != null)
                {
                    flyout._overlay.PropertyChanged -= flyout._overlay_SizeChanged;
                }

                //Window? parentWindow = flyout.Target.FindControl<Window>("TopLevel");

                //Window? parentWindow = App.GetWindow();

                Window? parentWindow = WindowsManager.AllWindows.FirstOrDefault();

                if (parentWindow == null)
                {
                    throw new Exception("Not find parent Window.");
                }

                //Window parentWindow = Window.GetWindow(flyout.Target);

                flyout._overlay = (Grid)parentWindow.Content;

                //flyout._overlay = FlyoutExtensions.FindVisualChildren<Grid>(parentWindow).First();

                if (flyout._overlay == null)
                {
                    throw new Exception("Not find parent Grid.");
                }

                flyout._overlay.PropertyChanged += flyout._overlay_SizeChanged;
            }
        }

        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly StyledProperty<FlyoutPlacementMode> PlacementProperty =
            AvaloniaProperty.Register<Flyout, FlyoutPlacementMode>("Placement", FlyoutPlacementMode.Bottom);

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<Flyout, bool>("IsOpen", false);

        private static void OnDataChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)e.Sender;

            flyout.InvalidateData();
        }

        public bool IsAnchor
        {
            get { return (bool)GetValue(IsAnchorProperty); }
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
