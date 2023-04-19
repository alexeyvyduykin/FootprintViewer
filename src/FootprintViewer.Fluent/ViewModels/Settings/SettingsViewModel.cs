using FootprintViewer.Data;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.Settings;

public sealed class SettingsViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public SettingsViewModel()
    {
        _dataManager = Services.DataManager;
        var languageManager = Services.LanguageManager;

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = Services.MainState;

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            mainState?.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });

        LanguageSettings = new LanguageSettingsViewModel(languageManager);

        LanguageSettings.Activate();
    }

    public override string Title { get => "Settings"; protected set { } }

    [Reactive]
    public LanguageSettingsViewModel LanguageSettings { get; set; }
}
