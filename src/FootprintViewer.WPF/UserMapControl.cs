using input = FootprintViewer.Input;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.UI.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using FootprintViewer.Interactivity;

namespace FootprintViewer.WPF
{
    public class UserMapControl : MapControl, IMapView
    {
        private Mapsui.Geometries.Point? _mouseDownPoint;
        private Cursor? _grabHandCursor;
        private bool _isLeftMouseDown = false;
        private input.CursorType _currentCursorType = input.CursorType.Default;
        private ItemsControl? _tipControl;
        private Mapsui.UI.MapInfoEventArgs? _mapInfoEventArgs;
        private string? _lastNameFeatureInfo;
        private bool _infoLeftClick = false;

        public UserMapControl() : base()
        {
            MouseEnter += MyMapControl_MouseEnter;
            MouseLeave += MyMapControl_MouseLeave;
            MouseWheel += MyMapControl_MouseWheel;
            MouseDown += MyMapControl_MouseDown;
            MouseMove += MyMapControl_MouseMove;
            MouseUp += MyMapControl_MouseUp;

            Info += UserMapControl_Info;
        }

        private void UserMapControl_Info(object? sender, Mapsui.UI.MapInfoEventArgs e)
        {
            if (MapListener != null)
            {
                _mapInfoEventArgs = e;

                if (e.MapInfo != null && e.MapInfo.Feature != null)
                {
                    var feature = e.MapInfo.Feature;

                    if (feature != null && feature.Fields.Contains("Name") == true)
                    {
                        var name = (string)feature["Name"];
                        _infoLeftClick = true;
                        _lastNameFeatureInfo = name;
                    }
                }
            }
        }

        private static ItemsControl? CreateTip()
        {
            string xaml = @"
            <ItemsControl>
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <Canvas Background=""Transparent""/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemContainerStyle>
                    <Style>
                        <Setter Property=""Canvas.Left"" Value=""{Binding X}""/>
                        <Setter Property=""Canvas.Top"" Value=""{Binding Y}""/>
                    </Style>
                </ItemsControl.ItemContainerStyle>
            </ItemsControl>";

            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            return XamlReader.Parse(xaml, parserContext) as ItemsControl;
        }

        public DataTemplate TipDataTemplate
        {
            get { return (DataTemplate)GetValue(TipDataTemplateProperty); }
            set { SetValue(TipDataTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TipDataTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TipDataTemplateProperty =
            DependencyProperty.Register("TipDataTemplate", typeof(DataTemplate), typeof(UserMapControl), new PropertyMetadata(null, OnTipDataTemplateChanged));

        private static void OnTipDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)d;
            var template = (DataTemplate)e.NewValue;
            var itemsControl = CreateTip();

            if (itemsControl != null)
            {
                itemsControl.ItemTemplate = template;

                if (mapControl._tipControl != null)
                {
                    mapControl.Children.Remove(mapControl._tipControl);
                }

                mapControl._tipControl = itemsControl;
                mapControl.Children.Add(itemsControl);
            }
        }

        public IController Controller
        {
            get { return (IController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IController), typeof(UserMapControl), new PropertyMetadata(new input.DefaultController(), OnControllerChanged));

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)d;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new input.MouseEventArgs());
        }

        public ITip? TipSource
        {
            get { return (ITip)GetValue(TipSourceProperty); }
            set { SetValue(TipSourceProperty, value); }
        }

        public static readonly DependencyProperty TipSourceProperty =
            DependencyProperty.Register("TipSource", typeof(ITip), typeof(UserMapControl), new PropertyMetadata(null, OnTipSourceChanged));

        private static void OnTipSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)d;
            if (e.NewValue == null)
            {
                mapControl.HideTip();
            }
            else
            {
                mapControl.ShowTip();
            }
        }

        public Map MapSource
        {
            get { return (Map)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapSourceProperty =
            DependencyProperty.Register("MapSource", typeof(Map), typeof(UserMapControl), new PropertyMetadata(null, OnMapSourceChanged));

        private static void OnMapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is Map map)
                {
                    mapControl.Map = map;
                }
            }
        }

        public MapListener MapListener
        {
            get { return (MapListener)GetValue(MapListenerProperty); }
            set { SetValue(MapListenerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapListenerProperty =
            DependencyProperty.Register("MapListener", typeof(MapListener), typeof(UserMapControl), new PropertyMetadata(null, OnMapListenerChanged));

        private static void OnMapListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is MapListener mapListener)
                {

                }
            }
        }

        public IMapObserver MapObserver
        {
            get { return (IMapObserver)GetValue(MapObserverProperty); }
            set { SetValue(MapObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Observer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapObserverProperty =
            DependencyProperty.Register("MapObserver", typeof(MapObserver), typeof(UserMapControl));

        private void MyMapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Handled)
            {
                return;
            }

            e.MouseDevice.Capture(null);

            //e.Handled = 
            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

            if (_infoLeftClick == true && string.IsNullOrEmpty(_lastNameFeatureInfo) == false)
            {
                MapListener.LeftClick(_lastNameFeatureInfo);
                _infoLeftClick = false;
            }

            _isLeftMouseDown = false;

            // Open the context menu
            //var p = e.GetPosition(this).ToScreenPoint();
            //var d = p.DistanceTo(_mouseDownPoint);

            //if (ContextMenu != null)
            //{
            //    if (Math.Abs(d) < 1e-8 && releasedArgs.InitialPressMouseButton == MouseButton.Right)
            //    {
            //        ContextMenu.DataContext = DataContext;
            //        ContextMenu.IsVisible = true;
            //    }
            //    else
            //    {
            //        ContextMenu.IsVisible = false;
            //    }
            //}
        }

        private void MyMapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Handled)
            {
                return;
            }

            if (TipSource != null)
            {
                var screenPosition = e.GetPosition(this);
                TipSource.X = screenPosition.X + 20;
                TipSource.Y = screenPosition.Y;
            }

            //e.Handled = 
            var args = e.ToMouseEventArgs(this);
            Controller.HandleMouseMove(this, args);

            if (args.Handled == false)
            {
                if (_isLeftMouseDown == true && e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    _infoLeftClick = false;
                    SetCursor(input.CursorType.HandGrab);
                }
                else
                {
                    SetCursor(input.CursorType.Default);
                }
            }
        }

        private void MyMapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Handled)
            {
                return;
            }

            Focus();
            e.MouseDevice.Capture(this);

            // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
            _mouseDownPoint = e.GetPosition(this).ToScreenPoint();

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                _isLeftMouseDown = true;
            }

            //e.Handled = 
            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            //e.Handled = 
            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            //e.Handled = 
            Controller.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            //e.Handled = 
            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public void NavigateToAOI(BoundingBox boundingBox)
        {
            Navigator.NavigateTo(boundingBox.Grow(boundingBox.Width * 0.2));
        }

        public void SetCursor(input.CursorType cursorType)
        {
            if (_grabHandCursor == null)
            {
                _grabHandCursor = new Cursor(App.GetResourceStream(new Uri("resources/GrabHand.cur", UriKind.Relative)).Stream);
            }

            if (_currentCursorType == cursorType)
            {
                return;
            }

            switch (cursorType)
            {
                case input.CursorType.Default:
                    Cursor = Cursors.Arrow;
                    break;
                case input.CursorType.Hand:
                    Cursor = Cursors.Hand;
                    break;
                case input.CursorType.HandGrab:
                    Cursor = _grabHandCursor;
                    break;
                case input.CursorType.Cross:
                    Cursor = Cursors.Cross;
                    break;
                default:
                    throw new Exception();
            }

            _currentCursorType = cursorType;
        }

        protected void ShowTip()
        {
            if (_tipControl != null && TipSource != null)
            {
                _tipControl.ItemsSource = new ObservableCollection<ITip>() { TipSource };
            }
        }

        protected void HideTip()
        {
            if (_tipControl != null)
            {
                _tipControl.ItemsSource = new ObservableCollection<Tip>();
            }
        }

        public Mapsui.Geometries.Point ScreenToWorld(Mapsui.Geometries.Point screenPosition)
        {
            return Viewport.ScreenToWorld(screenPosition);
        }

        public Mapsui.Geometries.Point WorldToScreen(Mapsui.Geometries.Point worldPosition)
        {
            return Viewport.WorldToScreen(worldPosition);
        }
    }
}
