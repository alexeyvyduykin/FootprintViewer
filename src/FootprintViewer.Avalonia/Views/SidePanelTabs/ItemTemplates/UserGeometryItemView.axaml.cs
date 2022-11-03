using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class UserGeometryItemView : UserControl
    {
        public UserGeometryItemView()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;
        }

        private void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            var args = new RoutedEventArgs(RemoveClickEvent);

            RaiseEvent(args);
        }

        public static readonly RoutedEvent<RoutedEventArgs> RemoveClickEvent =
            RoutedEvent.Register<UserGeometryItemView, RoutedEventArgs>(nameof(RemoveClick), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> RemoveClick
        {
            add => AddHandler(RemoveClickEvent, value);
            remove => RemoveHandler(RemoveClickEvent, value);
        }
    }
}
