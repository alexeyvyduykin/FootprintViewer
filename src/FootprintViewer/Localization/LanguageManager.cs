using FootprintViewer.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace FootprintViewer.Localization;

public class LanguageManager : ILanguageManager
{
    private readonly Lazy<Dictionary<string, LanguageModel>> _availableLanguages;
    private readonly string[] _availableLocales;

    public LanguageManager(string[] availableLocales)
    {
        _availableLocales = availableLocales;

        _availableLanguages = new Lazy<Dictionary<string, LanguageModel>>(GetAvailableLanguages);

        DefaultLanguage = CreateLanguageModel(CultureInfo.GetCultureInfo("en"));

        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(DefaultLanguage.Code);
    }

    public LanguageModel DefaultLanguage { get; }

    public LanguageModel CurrentLanguage => CreateLanguageModel(Thread.CurrentThread.CurrentUICulture);

    public IEnumerable<LanguageModel> AllLanguages => _availableLanguages.Value.Values;

    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            throw new ArgumentException($"{nameof(languageCode)} can't be empty.");
        }

        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
    }

    public void SetLanguage(LanguageModel languageModel) => SetLanguage(languageModel.Code);

    private Dictionary<string, LanguageModel> GetAvailableLanguages() =>
        _availableLocales
            .Select(locale => CreateLanguageModel(new CultureInfo(locale)))
            .ToDictionary(lm => lm.Code, lm => lm);

    private LanguageModel CreateLanguageModel(CultureInfo cultureInfo) =>
        cultureInfo is null
            ? DefaultLanguage
            : new LanguageModel(cultureInfo.EnglishName, cultureInfo.NativeName.ToTitleCase(),
                cultureInfo.TwoLetterISOLanguageName);
}
