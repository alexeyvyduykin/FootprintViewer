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

    public class LanguageSettingsViewModel : ReactiveObject
    {
        //private readonly ILocalizationService _localizationService;
        private readonly ILanguageManager _languageManager;
        //private ObservableCollection<LanguageViewModel> _languages;
        private bool _isActivated;

        public LanguageSettingsViewModel(/*ILocalizationService localizationService,*/ ILanguageManager languageManager)
        {
            //_languages = new ObservableCollection<LanguageViewModel>();
            //_localizationService = localizationService;
            _languageManager = languageManager;
            //CurrentLanguage = languageManager.CurrentLanguage;

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

            //var savedLanguage = _localizationService.CurrentLanguage;
            var currentLanguage = _languageManager.DefaultLanguage;// CurrentLanguage;

            //var languageCode = savedLanguage is null ? currentLanguage.Code : savedLanguage.Code;
            var languageCode = currentLanguage.Code;
            //CurrentLanguage = GetLanguageOrDefault(languageCode);

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

            //_languageManager.SetLanguage(CurrentLanguage);

            //_localizationService.CurrentLanguage = new()
            //{
            //    Code = CurrentLanguage.Code,
            //    Name = CurrentLanguage.Name
            //};
        }

        private IEnumerable<LanguageModel> GetSortedLanguages() =>
            _languageManager.AllLanguages.OrderBy(l => l.Name);

        //private LanguageModel GetLanguageOrDefault(string languageCode)
        //    => Languages.SingleOrDefault(l => l.Code == languageCode) ?? _languageManager.DefaultLanguage;

        [Reactive]
        public List<LanguageViewModel>? Languages { get; set; }
    }
}
