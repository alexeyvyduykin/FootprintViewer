using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using FootprintViewer.ViewModels;
using InteractiveGeometry.UI;
using InteractiveGeometry.UI.Avalonia;
using Mapsui;
using Mapsui.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia
{
    public class UserMapControl : InteractiveMapControl
    {
        private bool _isGrabbing = false;
        private Cursor? _grabHandCursor;
        private CursorType _currentCursorType = CursorType.Default;
        private readonly ItemsControl? _tipControl;

        public UserMapControl() : base()
        {
            TipSourceProperty.Changed.Subscribe(OnTipSourceChanged);
            MapSourceProperty.Changed.Subscribe(OnMapSourceChanged);
            MapNavigatorProperty.Changed.Subscribe(OnMapNavigatorChanged);

            var itemsControl = CreateTip();

            if (itemsControl != null)
            {
                _tipControl = itemsControl;
                Children.Add(itemsControl);
            }

            EffectiveViewportChanged += UserMapControl_EffectiveViewportChanged;
        }

        private void UserMapControl_EffectiveViewportChanged(object? sender, global::Avalonia.Layout.EffectiveViewportChangedEventArgs e)
        {
            var (resolution, scaleText, scaleLength) = ScaleMapBar.ChangedViewport(Viewport);
            ScaleMapBar!.Resolution = resolution;
            ScaleMapBar!.Scale = scaleText;
            ScaleMapBar!.ScaleLength = scaleLength;
        }

        private static ItemsControl? CreateTip()
        {
            string xaml = @"
          <ItemsControl xmlns='https://github.com/avaloniaui'
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        xmlns:views='clr-namespace:FootprintViewer.Avalonia.Views'>

          <ItemsControl.Styles>
            <Style Selector='ItemsControl > ContentPresenter'>
              <Setter Property='Canvas.Left' Value='{Binding X}'/>   
              <Setter Property='Canvas.Top' Value='{Binding Y}'/>      
              <Setter Property='IsVisible' Value='{Binding IsVisible}'/>      
            </Style>      
          </ItemsControl.Styles>
      
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <Canvas Background='Transparent'/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
        
      <ItemsControl.ItemTemplate>
        <DataTemplate>
          <views:TipView DataContext='{Binding}'/>
        </DataTemplate>
      </ItemsControl.ItemTemplate>

</ItemsControl>";

            return AvaloniaRuntimeXamlLoader.Parse<ItemsControl>(xaml);
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
                mapNavigator.Navigator = mapControl.Navigator;
                mapNavigator.Viewport = mapControl.Viewport;
            }
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
                _tipControl.Items = new ObservableCollection<ITip>();
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

                var (resolution, scaleText, scaleLength) = ScaleMapBar.ChangedViewport(Viewport);
                ScaleMapBar!.Resolution = resolution;
                ScaleMapBar!.Scale = scaleText;
                ScaleMapBar!.ScaleLength = scaleLength;

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

                    ScaleMapBar!.Position = ScaleMapBar.ChangedPosition(position.X, position.Y, Viewport).ToMPoint();
                }
            }

            if (TipSource != null)
            {
                var screenPosition = e.GetPosition(this);

                TipSource.X = screenPosition.X + 20;
                TipSource.Y = screenPosition.Y;

                if (TipSource.IsVisible == false)
                {
                    TipSource.IsVisible = true;
                }
            }
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            var (resolution, scaleText, scaleLength) = ScaleMapBar.ChangedViewport(Viewport);
            ScaleMapBar!.Resolution = resolution;
            ScaleMapBar!.Scale = scaleText;
            ScaleMapBar!.ScaleLength = scaleLength;
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);

            if (TipSource != null)
            {
                TipSource.IsVisible = false;
            }
        }

        public void NavigateToAOI(MRect boundingBox)
        {
            Navigator.NavigateTo(boundingBox.Grow(boundingBox.Width * 0.2));
        }

        public override void SetCursor(CursorType cursorType)
        {
            if (_currentCursorType == cursorType)
            {
                return;
            }

            Cursor = cursorType switch
            {
                CursorType.Default => new Cursor(StandardCursorType.Arrow),
                CursorType.Hand => new Cursor(StandardCursorType.Hand),
                CursorType.HandGrab => (_grabHandCursor ??= Services.CursorService.GetGrabHandCursor()),
                CursorType.Cross => new Cursor(StandardCursorType.Cross),
                _ => throw new Exception(),
            };

            _currentCursorType = cursorType;
        }
    }
}
