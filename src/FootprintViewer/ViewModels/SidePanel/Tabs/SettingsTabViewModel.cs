using FootprintViewer.Data;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.ViewModels
{
    public class SettingsTabViewModel : SidePanelTab
    {
        public SettingsTabViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            Title = "Пользовательские настройки";

            Providers = new List<ProviderViewModel>();
        }

        public ProviderViewModel? Find(ProviderType type)
        {
            return Providers.Where(s => s.Type.Equals(type)).FirstOrDefault();
        }

        [Reactive]
        public List<ProviderViewModel> Providers { get; set; }

        [Reactive]
        public LanguageSettingsViewModel? LanguageSettings { get; set; }
    }
}
