using FootprintViewer.Configurations;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Splat;

namespace FootprintViewer.Designer
{
    public class DesignTimeSettingsTab : SettingsTabViewModel
    {
        private static readonly IReadonlyDependencyResolver _resolver = new DesignTimeData();

        public DesignTimeSettingsTab() : base(_resolver)
        {
            var config = new LanguagesConfiguration() { AvailableLocales = new[] { "en", "ru" } };

            var languageManager = new LanguageManager(config);

            LanguageSettings = new LanguageSettingsViewModel(languageManager);

            LanguageSettings.Activate();
        }
    }
}
