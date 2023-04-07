using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace FootprintViewer.Fluent.Views.SidePanel.Items
{
    public partial class FootprintView : UserControl
    {
        public FootprintView()
        {
            InitializeComponent();

            TargetToMapButton.Click += TargetToMapButton_Click;
        }

        private void TargetToMapButton_Click(object? sender, RoutedEventArgs e)
        {
            var args = new RoutedEventArgs(TargetToMapClickEvent);

            RaiseEvent(args);
        }

        public static readonly RoutedEvent<RoutedEventArgs> TargetToMapClickEvent =
            RoutedEvent.Register<FootprintView, RoutedEventArgs>(nameof(TargetToMapClick), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> TargetToMapClick
        {
            add => AddHandler(TargetToMapClickEvent, value);
            remove => RemoveHandler(TargetToMapClickEvent, value);
        }
    }
}
