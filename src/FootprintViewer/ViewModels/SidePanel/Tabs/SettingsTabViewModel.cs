using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.ViewModels
{
    public class SettingsTabViewModel : SidePanelTab
    {
        public SettingsTabViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            Title = "Пользовательские настройки";

            Providers = new List<ProviderViewModel>();
        }

        [Reactive]
        public List<ProviderViewModel> Providers { get; set; }

        [Reactive]
        public LanguageSettingsViewModel? LanguageSettings { get; set; }
    }
}
