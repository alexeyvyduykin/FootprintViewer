using DynamicData;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Services2;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed class UserGeometryTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly SourceList<UserGeometry> _userGeometries = new();
    private readonly ReadOnlyObservableCollection<UserGeometryViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public UserGeometryTabViewModel()
    {
        Title = "User geometries";

        Key = nameof(UserGeometryTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var mapService = Services.Locator.GetRequiredService<IMapService>();
        var layerProvider = mapService.GetProvider<UserGeometryProvider>();

        var mainObservable = _userGeometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler);

        mainObservable
            .Transform(s => new UserGeometryViewModel(s))
            .Bind(out _items)
            .Subscribe();

        var layerObservable = mainObservable
            .ToCollection();

        layerProvider?.SetObservable(layerObservable);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _localStorage.DataChanged
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
        var res = await _localStorage.GetValuesAsync<UserGeometry>(DbKeys.UserGeometries.ToString());

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
            await _localStorage.TryRemoveAsync(DbKeys.UserGeometries.ToString(), value.Model);
        }
    }

    public ReadOnlyObservableCollection<UserGeometryViewModel> Items => _items;
}