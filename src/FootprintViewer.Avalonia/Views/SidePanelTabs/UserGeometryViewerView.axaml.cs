using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class UserGeometryViewerView : ReactiveUserControl<UserGeometryViewer>
    {
        private Button CreateButton => this.FindControl<Button>("CreateButton");

        private Button UpdateButton => this.FindControl<Button>("UpdateButton");

        private Button RemoveButton => this.FindControl<Button>("RemoveButton");

        private ListBox MainListBox => this.FindControl<ListBox>("MainListBox");

        public UserGeometryViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // CreateButton
                this.CreateButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v.ViewModel.Create).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Can, v => v.CreateButton.IsEnabled).DisposeWith(disposables);

                // UpdateButton
                this.UpdateButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v.ViewModel.Update).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Can, v => v.UpdateButton.IsEnabled).DisposeWith(disposables);

                // RemoveButton
                this.RemoveButton.Events().Click.Select(args => ViewModel.SelectedUserGeometry).InvokeCommand(this, v => v.ViewModel.Remove).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.CanRemove, v => v.RemoveButton.IsEnabled).DisposeWith(disposables);

                // MainListBox
                this.OneWayBind(ViewModel, vm => vm.UserGeometries, v => v.MainListBox.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedUserGeometry, v => v.MainListBox.SelectedItem).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
