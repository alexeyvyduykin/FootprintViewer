using Avalonia.Controls;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class UserGeometryItemView : ReactiveUserControl<UserGeometryViewModel>
    {
        private UserGeometryTab? _userGeometryTab;

        public UserGeometryItemView()
        {
            InitializeComponent();

            _click = ReactiveCommand.Create(ClickImpl);

            this.WhenActivated(disposables =>
            {
                this.RemoveButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v._click).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Type, v => v.TypeTextBlock.Text, value => value.ToString()).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameTextBlock.Text).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _click;

        private void ClickImpl()
        {
            _userGeometryTab ??= Locator.Current.GetExistingService<UserGeometryTab>();

            _userGeometryTab.ViewerList.Remove.Execute(ViewModel).Subscribe();
        }
    }
}
