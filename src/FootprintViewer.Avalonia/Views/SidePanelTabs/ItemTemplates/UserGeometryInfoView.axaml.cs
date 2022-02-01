using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
    public partial class UserGeometryInfoView : ReactiveUserControl<UserGeometryInfo>
    {
        private TextBlock TypeTextBlock => this.FindControl<TextBlock>("TypeTextBlock");

        private TextBlock NameTextBlock => this.FindControl<TextBlock>("NameTextBlock");

        private Button RemoveButton => this.FindControl<Button>("RemoveButton");

        private UserGeometryViewer? _userGeometryViewer;

        public UserGeometryInfoView()
        {
            InitializeComponent();

            _click = ReactiveCommand.Create(ClickImpl);

            this.WhenActivated(disposables =>
            {
                this.RemoveButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v._click).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Type, v => v.TypeTextBlock.Text, value => ((Data.UserGeometryType)value).ToString()).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameTextBlock.Text).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _click;

        private void ClickImpl()
        {
            _userGeometryViewer ??= Locator.Current.GetExistingService<UserGeometryViewer>();

            _userGeometryViewer.Remove.Execute(ViewModel).Subscribe();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
