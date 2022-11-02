using ReactiveUI.Fody.Helpers;
using Splat;

namespace FootprintViewer.ViewModels
{
    public class SettingsTabViewModel : SidePanelTab
    {
        public SettingsTabViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            Title = "Пользовательские настройки";
        }

        [Reactive]
        public LanguageSettingsViewModel? LanguageSettings { get; set; }
    }
}
