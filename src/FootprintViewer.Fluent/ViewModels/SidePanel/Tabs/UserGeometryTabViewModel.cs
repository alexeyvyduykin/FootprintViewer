using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed partial class UserGeometryTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<UserGeometry> _userGeometries = new();
    private readonly ReadOnlyObservableCollection<UserGeometryViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public UserGeometryTabViewModel()
    {
        _dataManager = Services.Locator.GetRequiredService<IDataManager>();

        Title = "Пользовательская геометрия";
        Key = nameof(UserGeometryTabViewModel);

        _userGeometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new UserGeometryViewModel(s))
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.UserGeometries.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        Remove = ReactiveCommand.CreateFromTask<UserGeometryViewModel?>(RemoveAsync);

        _isLoading = Update.IsExecuting
              .ObserveOn(RxApp.MainThreadScheduler)
              .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<UserGeometryViewModel?, Unit> Remove { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<UserGeometry>(DbKeys.UserGeometries.ToString());

        _userGeometries.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
    }

    private async Task RemoveAsync(UserGeometryViewModel? value)
    {
        if (value != null)
        {
            await _dataManager.TryRemoveAsync(DbKeys.UserGeometries.ToString(), value.Model);

            _dataManager.ForceUpdateData(DbKeys.UserGeometries.ToString());
        }
    }

    public ReadOnlyObservableCollection<UserGeometryViewModel> Items => _items;
}

public partial class UserGeometryTabViewModel
{
    public UserGeometryTabViewModel(DesignDataDependencyResolver resolver)
    {
        _dataManager = resolver.GetService<IDataManager>();

        Title = "Пользовательская геометрия";
        Key = nameof(UserGeometryTabViewModel);
        _userGeometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new UserGeometryViewModel(s))
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.UserGeometries.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        Remove = ReactiveCommand.CreateFromTask<UserGeometryViewModel?>(RemoveAsync);

        _isLoading = Update.IsExecuting
              .ObserveOn(RxApp.MainThreadScheduler)
              .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);
    }
}