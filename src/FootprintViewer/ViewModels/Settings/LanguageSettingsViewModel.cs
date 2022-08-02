using FootprintViewer.AppStates;
using FootprintViewer.Localization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class LanguageViewModel : ReactiveObject
    {
        private readonly LanguageModel _language;
        private readonly LanguageSettingsViewModel _parent;


        public LanguageViewModel(LanguageModel language, LanguageSettingsViewModel parent)
        {
            _language = language;
            _parent = parent;

            this.WhenAnyValue(s => s.IsChecked)
                .Where(s => s == true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => _parent.SaveChanged(Code));
        }

        public string Name => _language.Name;

        public string NativeName => _language.NativeName;

        public string Code => _language.Code;

        [Reactive]
        public bool IsChecked { get; set; }
    }

    public class LanguageSettingsViewModel : ReactiveObject, ISuspendableState<LocalizationState>
    {
        private readonly ILanguageManager _languageManager;
        private bool _isActivated;
        private LocalizationState? _state;

        public LanguageSettingsViewModel(ILanguageManager languageManager)
        {
            _languageManager = languageManager;

            Save = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Save { get; }

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            var savedLanguage = _state?.Language;

            var currentLanguage = _languageManager.DefaultLanguage;

            var languageCode = savedLanguage is null ? currentLanguage.Code : savedLanguage.Code;

            Languages = new List<LanguageViewModel>(
                GetSortedLanguages().Select(s =>
                new LanguageViewModel(s, this)
                {
                    IsChecked = s.Code.Equals(languageCode),
                }));
        }

        public void SaveChanged(string code)
        {
            if (_languageManager.CurrentLanguage.Code != code)
            {
                _languageManager.SetLanguage(code);

                Save.Execute().Subscribe();
            }

            if (_state != null)
            {
                _state.Language = _languageManager.CurrentLanguage;
            }
        }

        private IEnumerable<LanguageModel> GetSortedLanguages() =>
            _languageManager.AllLanguages.OrderBy(l => l.Name);

        public void LoadState(LocalizationState state)
        {
            _state = state;

            var code = state.Language?.Code;

            if (code != null && _languageManager.CurrentLanguage.Code != code)
            {
                _languageManager.SetLanguage(code);

                Save.Execute().Subscribe();
            }
        }

        //private LanguageModel GetLanguageOrDefault(string languageCode)
        //    => Languages.SingleOrDefault(l => l.Code == languageCode) ?? _languageManager.DefaultLanguage;

        [Reactive]
        public List<LanguageViewModel>? Languages { get; set; }
    }
}
