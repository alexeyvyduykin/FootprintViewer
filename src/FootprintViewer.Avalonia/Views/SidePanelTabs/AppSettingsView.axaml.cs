using Avalonia.Controls;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class AppSettingsView : ReactiveUserControl<SettingsTabViewModel>
    {
        public AppSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                LanguageExpander.WhenAnyValue(s => s.IsExpanded).Where(s => s == true).Subscribe(_ => ActivateLanguageSettings());

                ViewModel?.LanguageSettings?.Save.Subscribe(_ => UpdateMainWindow()).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.LanguageSettings!.Languages![0].IsChecked, v => v.RadioButton1.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.LanguageSettings!.Languages![1].IsChecked, v => v.RadioButton2.IsChecked).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.LanguageSettings!.Languages![0], v => v.LanguageTextBlock1.Text, value => LanguageConvert(value)).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.LanguageSettings!.Languages![1], v => v.LanguageTextBlock2.Text, value => LanguageConvert(value)).DisposeWith(disposables);
            });
        }

        private static object LanguageConvert(LanguageViewModel language)
        {
            return language.Code switch
            {
                "en" => Properties.Resources.LanguageEnglish,
                "ru" => Properties.Resources.LanguageRussian,
                _ => throw new Exception(),
            };
        }

        private void ActivateLanguageSettings()
        {
            ViewModel?.LanguageSettings?.Activate();
        }

        private static void UpdateMainWindow()
        {
            if (Design.IsDesignMode == true)
            {
                return;
            }

            App.GetMainWindow().UpdateComponent();
        }
    }
}
