using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.Settings;

public sealed class ConnectionViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public ConnectionViewModel()
    {
        _dataManager = Services.DataManager;
        var languageManager = Services.LanguageManager;

        int counter = 0;

        var userGeometriesSources = _dataManager.GetSources(DbKeys.UserGeometries.ToString());

        SourceContainers = new List<SourceContainerViewModel>()
        {
            new SourceContainerViewModel(this, DbKeys.UserGeometries.ToString())
            {
                Header = DbKeys.UserGeometries.ToString(),
                Sources = userGeometriesSources.Select(s => new SourceViewModel(s) { Name = $"Source{++counter}" } ).ToList<ISourceViewModel>(),
            },
        };

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = Services.MainState;

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            foreach (var container in SourceContainers)
            {
                _ = Enum.TryParse<DbKeys>(container.Header, out var key);

                foreach (var (source, op) in container.SourceOperationsStack.Reverse())
                {
                    // add
                    if (op == true)
                    {
                        _dataManager.RegisterSource(key.ToString(), source);
                    }
                    // remove
                    else if (op == false)
                    {
                        _dataManager.UnregisterSource(key.ToString(), source);
                    }
                }
            }

            mainState?.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });
    }

    [Reactive]
    public IList<SourceContainerViewModel> SourceContainers { get; set; }

    [Reactive]
    public SourceContainerViewModel? SelectedSourceContainer { get; set; }
}
