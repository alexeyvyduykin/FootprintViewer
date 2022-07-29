using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class UserGeometryItemView : ReactiveUserControl<UserGeometryViewModel>
    {
        public UserGeometryItemView()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Type, v => v.TypeTextBlock.Text, value => value.ToString()).DisposeWith(disposables);
            });
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
