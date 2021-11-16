using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FootprintViewer.WPF.Controls
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

            Loaded += Flyout_Loaded;
        }

        private void Flyout_Loaded(object sender, RoutedEventArgs e)
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
                    var w = target.ActualWidth;
                    var h = target.ActualHeight;
                    var W = overlay.ActualWidth;
                    var H = overlay.ActualHeight;

                    var p = target.TransformToAncestor(overlay).Transform(new Point(0, 0));

                    Thickness marginFlyout = new Thickness(0.0);
                    Thickness marginArrow;
                    Transform transform = null;
                    Point transformPoint = new Point();
                    HorizontalAlignment horizontal = HorizontalAlignment.Left;
                    VerticalAlignment vertical = VerticalAlignment.Top;

                    switch (Placement)
                    {
                        case FlyoutPlacementMode.Left:
                        {
                            marginFlyout = new Thickness(0, 0, W - p.X + offset, 0);
                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(0, p.Y, W - p.X + offset, 0);
                            }

                            horizontal = HorizontalAlignment.Right;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup()
                            {
                                Children = new TransformCollection()
                                    {
                                        new TranslateTransform(b, -b),
                                        new RotateTransform(45),
                                        new TranslateTransform(0, h / 2.0),
                                    }
                            };

                            transformPoint = new Point(1, 0);

                            break;
                        }
                        case FlyoutPlacementMode.Top:
                        {
                            marginFlyout = new Thickness(0, 0, 0, H - p.Y + offset);

                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, 0, 0, H - p.Y + offset);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Bottom;

                            transform = new TransformGroup()
                            {
                                Children = new TransformCollection()
                                    {
                                        new TranslateTransform(-b, b),
                                        new RotateTransform(45),
                                        new TranslateTransform(w / 2.0, 0),
                                    }
                            };
                            transformPoint = new Point(0, 1);
                            break;
                        }
                        case FlyoutPlacementMode.Right:
                        {
                            marginFlyout = new Thickness(p.X + w + offset, 0, 0, 0);

                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X + w + offset, p.Y, 0, 0);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup()
                            {
                                Children = new TransformCollection()
                                    {
                                        new TranslateTransform(-b, -b),
                                        new RotateTransform(45),
                                        new TranslateTransform(0, h / 2.0),
                                    }
                            };
                            transformPoint = new Point(0, 0);
                            break;
                        }
                        case FlyoutPlacementMode.Bottom:
                        {
                            marginFlyout = new Thickness(0, p.Y + h + offset, 0, 0);

                            if (IsAnchor == true)
                            {
                                marginFlyout = new Thickness(p.X, p.Y + h + offset, 0, 0);
                            }

                            horizontal = HorizontalAlignment.Left;
                            vertical = VerticalAlignment.Top;

                            transform = new TransformGroup()
                            {
                                Children = new TransformCollection()
                                    {
                                        new TranslateTransform(-b, -b),
                                        new RotateTransform(45),
                                        new TranslateTransform(w / 2.0, 0),
                                    }
                            };
                            transformPoint = new Point(0, 0);
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
                    _arrow.Margin = marginFlyout;

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

        private void _overlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsOpen == true)
            {
                var target = Target;
                var overlay = _overlay;

                var offset = 8;
                var w = target.ActualWidth;
                var h = target.ActualHeight;
                var W = overlay.ActualWidth;
                var H = overlay.ActualHeight;

                var p = target.TransformToAncestor(overlay).Transform(new Point(0, 0));

                Thickness margin;

                switch (Placement)
                {
                    case FlyoutPlacementMode.Left:
                    {
                        margin = new Thickness(0, p.Y, W - p.X + offset, 0);
                        break;
                    }
                    case FlyoutPlacementMode.Top:
                    {
                        margin = new Thickness(p.X, 0, 0, H - p.Y + offset);
                        break;
                    }
                    case FlyoutPlacementMode.Right:
                    {
                        margin = new Thickness(p.X + w + offset, p.Y, 0, 0);
                        break;
                    }
                    case FlyoutPlacementMode.Bottom:
                    {
                        margin = new Thickness(p.X, p.Y + h + offset, 0, 0);
                        break;
                    }
                    default:
                        break;
                }

                _border.Margin = margin;

                _arrow.Margin = margin;
            }
        }

        public DataTemplate FlyoutTemplate
        {
            get { return (DataTemplate)GetValue(FlyoutTemplateProperty); }
            set { SetValue(FlyoutTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlyoutTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyoutTemplateProperty =
            DependencyProperty.Register("FlyoutTemplate", typeof(DataTemplate), typeof(Flyout), new PropertyMetadata(null));

        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(Flyout), new PropertyMetadata(null, OnTargetChanged));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)d;

            if (e.NewValue is FrameworkElement target)
            {
                if (flyout._overlay != null)
                {
                    flyout._overlay.SizeChanged -= flyout._overlay_SizeChanged;
                }

                flyout._overlay = flyout.Target.GetParentOfType<Grid>();

                if (flyout._overlay == null)
                {
                    throw new Exception("Not find parent Grid.");
                }

                flyout._overlay.SizeChanged += flyout._overlay_SizeChanged;
            }
        }

        public FlyoutPlacementMode Placement
        {
            get { return (FlyoutPlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(FlyoutPlacementMode), typeof(Flyout), new PropertyMetadata(FlyoutPlacementMode.Bottom));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new PropertyMetadata(false, OnDataChanged));

        public bool IsAnchor
        {
            get { return (bool)GetValue(IsAnchorProperty); }
            set { SetValue(IsAnchorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnchorProperty =
            DependencyProperty.Register("IsAnchor", typeof(bool), typeof(Flyout), new PropertyMetadata(false));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)d;

            flyout.InvalidateData();
        }
    }

    public static class FlyoutExtensions
    {
        public static T? GetParentOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                return null;
            }

            T res = null;

            var parent = LogicalTreeHelper.GetParent(depObj);

            while (parent != null)
            {
                if (parent is T p)
                {
                    res = p;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            return res;
        }
    }
}
