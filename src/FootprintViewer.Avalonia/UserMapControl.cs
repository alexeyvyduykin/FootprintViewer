using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Platform;
using Avalonia.Styling;
using FootprintViewer.Graphics;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace FootprintViewer.Avalonia
{
    public class UserMapControl : MapControl, IMapView
    {
        private Mapsui.Geometries.Point? _mouseDownPoint;
        private Cursor? _grabHandCursor;
        private bool _isLeftMouseDown = false;
        private CursorType _currentCursorType = CursorType.Default;
        private ItemsControl? _tipControl;
        private Mapsui.UI.MapInfoEventArgs? _mapInfoEventArgs;
        private string? _lastNameFeatureInfo;
        private bool _infoLeftClick = false;

        public UserMapControl() : base()
        {    
            PointerEnter += MyMapControl_MouseEnter;
            PointerLeave += MyMapControl_MouseLeave;
            PointerWheelChanged += MyMapControl_MouseWheel;      
            PointerPressed += MyMapControl_MouseDown;     
            PointerMoved += MyMapControl_MouseMove;
            PointerReleased += MyMapControl_MouseUp;
      
            Info += UserMapControl_Info;

            TipDataTemplateProperty.Changed.Subscribe(OnTipDataTemplateChanged);
            ControllerProperty.Changed.Subscribe(OnControllerChanged);
            TipSourceProperty.Changed.Subscribe(OnTipSourceChanged);
            MapSourceProperty.Changed.Subscribe(OnMapSourceChanged);
            MapListenerProperty.Changed.Subscribe(OnMapListenerChanged);
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
          <ItemsControl xmlns='https://github.com/avaloniaui'
                       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>

          <ItemsControl.Styles>
            <Style Selector='ItemsControl > ContentPresenter'>
              <Setter Property='Canvas.Left' Value='{Binding X}'/>   
              <Setter Property='Canvas.Top' Value='{Binding Y}'/>      
            </Style>      
          </ItemsControl.Styles>
      
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <Canvas Background='Transparent'/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
        
         </ItemsControl>";

            return AvaloniaRuntimeXamlLoader.Parse<ItemsControl>(xaml);
        }

        public DataTemplate TipDataTemplate
        {
            get { return GetValue(TipDataTemplateProperty); }
            set { SetValue(TipDataTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<DataTemplate> TipDataTemplateProperty =
            AvaloniaProperty.Register<UserMapControl, DataTemplate>(nameof(TipDataTemplate));

        private static void OnTipDataTemplateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)e.Sender;
            var template = (DataTemplate?)e.NewValue;
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

        public Plotter Plotter
        {
            get { return GetValue(PlotterProperty); }
            set { SetValue(PlotterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Observer.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<Plotter> PlotterProperty =
            AvaloniaProperty.Register<UserMapControl, Plotter>(nameof(Plotter));

        public IController Controller
        {
            get { return GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IController> ControllerProperty =
            AvaloniaProperty.Register<UserMapControl, IController>(nameof(Controller), new EditController());

        private static void OnControllerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)e.Sender;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new MouseEventArgs());
        }

        public ITip? TipSource
        {
            get { return GetValue(TipSourceProperty); }
            set { SetValue(TipSourceProperty, value); }
        }

        public static readonly StyledProperty<ITip?> TipSourceProperty =
            AvaloniaProperty.Register<UserMapControl, ITip?>(nameof(TipSource), null);

        private static void OnTipSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)e.Sender;
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
        public static readonly StyledProperty<Map> MapSourceProperty =
            AvaloniaProperty.Register<UserMapControl, Map>(nameof(MapSource));

        private static void OnMapSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is UserMapControl mapControl)
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
        public static readonly StyledProperty<MapListener> MapListenerProperty =
            AvaloniaProperty.Register<UserMapControl, MapListener>(nameof(MapListener));

        private static void OnMapListenerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is MapListener mapListener)
                {

                }
            }
        }


        private void MyMapControl_MouseUp(object? sender, PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.Handled)
            {
                return;
            }

            e.Pointer.Capture(null);

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

        private void MyMapControl_MouseMove(object? sender, PointerEventArgs e)
        {       
            base.OnPointerMoved(e);

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
                if (_isLeftMouseDown == true && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
                {
                    _infoLeftClick = false;
                    SetCursor(CursorType.HandGrab, "UserMapControl.MyMapControl_MouseMove");
                }
                else
                {
                    SetCursor(CursorType.Default, "UserMapControl.MyMapControl_MouseMove");
                }
            }
        }

        private void MyMapControl_MouseDown(object? sender, PointerPressedEventArgs e)
        {           
            base.OnPointerPressed(e);

            if (e.Handled)
            {
                return;
            }

            Focus();
            e.Pointer.Capture(this);

            // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
            _mouseDownPoint = e.GetPosition(this).ToScreenPoint();

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
            {
                _isLeftMouseDown = true;
            }

            //e.Handled = 
            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object? sender, PointerWheelEventArgs e)
        {       
            base.OnPointerWheelChanged(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            //e.Handled = 
            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object? sender, PointerEventArgs e)
        {           
            base.OnPointerLeave(e);

            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            //e.Handled = 
            Controller.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object? sender, PointerEventArgs e)
        {     
            base.OnPointerEnter(e);

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

        public void SetCursor(CursorType cursorType, string info = "")
        {
            if (_grabHandCursor == null)
            {
                //_grabHandCursor = (Cursor?)App.Current.Resources["GrabHandCursor"];
                //_grabHandCursor = new Cursor(App.GetResourceStream(new Uri("resources/GrabHand.cur", UriKind.Relative)).Stream);
            }

            if (_currentCursorType == cursorType)
            {
                return;
            }

            switch (cursorType)
            {
                case CursorType.Default:
                    Cursor = new Cursor(StandardCursorType.Arrow);
                    break;
                case CursorType.Hand:
                    Cursor = new Cursor(StandardCursorType.Hand);                 
                    break;
                case CursorType.HandGrab:
                    Cursor = new Cursor(StandardCursorType.DragMove);// _grabHandCursor;
                    break;
                case CursorType.Cross:
                    Cursor = new Cursor(StandardCursorType.Cross);                
                    break;
                default:
                    throw new Exception();
            }

            _currentCursorType = cursorType;

            Debug.WriteLine($"Set Cursor = {Cursor}, Info = {info}");
        }

        protected void ShowTip()
        {
            if (_tipControl != null && TipSource != null)
            {
                _tipControl.Items = new ObservableCollection<ITip>() { TipSource };
            }
        }

        protected void HideTip()
        {
            if (_tipControl != null)
            {
                _tipControl.Items = new ObservableCollection<Tip>();
            }
        }
    }
}
