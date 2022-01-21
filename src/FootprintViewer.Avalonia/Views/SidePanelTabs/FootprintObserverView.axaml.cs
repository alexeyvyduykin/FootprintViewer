using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Topten.RichTextKit;
using controls = FootprintViewer.Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintObserverView : ReactiveUserControl<FootprintObserver>
    {
        private controls.Flyout Flyout => this.FindControl<controls.Flyout>("Flyout");

        private ToggleButton SearchToggleButton => this.FindControl<ToggleButton>("SearchToggleButton");

        private ContentControl MainContentControl => this.FindControl<ContentControl>("MainContentControl");

        public FootprintObserverView()
        {
            InitializeComponent();

            _click = ReactiveCommand.Create(ClickImpl);

            this.WhenActivated(disposables =>
            {
                // Flyout
        //   this.OneWayBind(Flyout, fl => fl, vm => vm.Flyout.Target).DisposeWith(disposables);
            //    this.OneWayBind(ViewModel, vm => vm.Filter.IsOpen, v => v.Flyout.IsOpen).DisposeWith(disposables);
            //    this.OneWayBind(ViewModel, vm => vm.Filter.IsOpen, v => v.Flyout.IsVisible).DisposeWith(disposables);
            //    this.OneWayBind(ViewModel, vm => vm.Filter, v => v.Flyout.Content).DisposeWith(disposables);
          //      this.WhenAnyValue(x => x.SearchToggleButton).BindTo(this, v => v.Flyout.Target).DisposeWith(disposables);
             //   SearchToggleButton.Bind
             //   textBlock.Bind(TextBlock.TextProperty, new Binding("Name"))

                // SearchToggleButton
               // this.BindCommand(ViewModel, vm => vm.FilterClick, v => v.SearchToggleButton).DisposeWith(disposables);
              //  this.SearchToggleButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v._click).DisposeWith(disposables);


                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.MainContent, v => v.MainContentControl.Content).DisposeWith(disposables);
            });

        }

        private readonly ReactiveCommand<Unit, Unit> _click;

        private void ClickImpl()
        {         
            ViewModel?.FilterClick.Execute().Subscribe();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
