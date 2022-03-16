using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using Material.Styles;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class FootprintInfoView : ReactiveUserControl<FootprintInfo>
    {
        private Card MainCard => this.FindControl<Card>("MainCard");

        private StackPanel MainStackPanel => this.FindControl<StackPanel>("MainStackPanel");

        private TextBlock HeaderTextBlock => this.FindControl<TextBlock>("HeaderTextBlock");

        private TextBlock SatelliteTextBlock => this.FindControl<TextBlock>("SatelliteTextBlock");

        private TextBlock CenterTextBlock => this.FindControl<TextBlock>("CenterTextBlock");

        private TextBlock BeginTextBlock => this.FindControl<TextBlock>("BeginTextBlock");

        private TextBlock DurationTextBlock => this.FindControl<TextBlock>("DurationTextBlock");

        private TextBlock NodeTextBlock => this.FindControl<TextBlock>("NodeTextBlock");

        private TextBlock DirectionTextBlock => this.FindControl<TextBlock>("DirectionTextBlock");
        
        private static FootprintObserver? _footprintObserver;

        public FootprintInfoView()
        {
            InitializeComponent();
            
            _command = ReactiveCommand.Create(CommandImpl);

            this.WhenActivated(disposables => 
            {
                this.MainCard.Events().PointerPressed.Select(args => Unit.Default).InvokeCommand(this, v => v._command).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.HeaderTextBlock.Text).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.IsShowInfo, v => v.MainStackPanel.IsVisible).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.SatelliteName, v => v.SatelliteTextBlock.Text).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Center, v => v.CenterTextBlock.Text, value => $"{value?.X:0.00}° {value?.Y:0.00}°").DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Begin, v => v.BeginTextBlock.Text, value => value.ToString("dd.MM.yyyy HH:mm:ss")).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Duration, v => v.DurationTextBlock.Text, value => $"{value} sec").DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Node, v => v.NodeTextBlock.Text).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Direction, v => v.DirectionTextBlock.Text, value => ((Data.SatelliteStripDirection)value).ToString()).DisposeWith(disposables);

                
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _command;

        private void CommandImpl()
        {
            _footprintObserver ??= Locator.Current.GetExistingService<FootprintObserver>();

            _footprintObserver?.ClickOnItem.Execute(ViewModel).Subscribe();       
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
