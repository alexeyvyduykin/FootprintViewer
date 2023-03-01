using FootprintViewer.AppStates;
using FootprintViewer.Data;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.Settings;

public sealed class SettingsViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public SettingsViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        var languageManager = dependencyResolver.GetExistingService<ILanguageManager>();

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = dependencyResolver.GetExistingService<MainState>();

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            mainState.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });

        LanguageSettings = new LanguageSettingsViewModel(languageManager);

        LanguageSettings.Activate();
    }

    [Reactive]
    public LanguageSettingsViewModel LanguageSettings { get; set; }
}
