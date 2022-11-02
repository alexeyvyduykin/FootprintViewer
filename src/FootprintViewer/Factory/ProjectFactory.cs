using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Mapsui;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer;

public class ProjectFactory
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;

    public ProjectFactory(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    public LanguageManager CreateLanguageManager()
    {
        var languagesConfiguration = _dependencyResolver.GetExistingService<LanguagesConfiguration>();

        return new LanguageManager(languagesConfiguration);
    }

    public InfoPanel CreateInfoPanel()
    {
        return new InfoPanel();
    }

    public BottomPanel CreateBottomPanel()
    {
        return new BottomPanel(_dependencyResolver);
    }

    public IMapNavigator CreateMapNavigator()
    {
        return new MapNavigator();
    }

    public ScaleMapBar CreateScaleMapBar()
    {
        return new ScaleMapBar();
    }

    public MapBackgroundList CreateMapBackgroundList()
    {
        var loader = _dependencyResolver.GetExistingService<TaskLoader>();
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        var map = (Map)_dependencyResolver.GetExistingService<IMap>();

        var mapBackgroundList = new MapBackgroundList();

        mapBackgroundList.WorldMapChanged.Subscribe(s => map.SetWorldMapLayer(s));

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(LoadingAsync, outputScheduler: RxApp.MainThreadScheduler));

        loader.AddTaskAsync(() => LoadingAsync());

        return mapBackgroundList;

        async Task LoadingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            var maps = await dataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());

            mapBackgroundList.Update(maps);

            var item = maps.FirstOrDefault();

            if (item != null)
            {
                map.SetWorldMapLayer(item);
                int hjhj = 0;
            }
        }
    }

    public MapLayerList CreateMapLayerList()
    {
        return new MapLayerList(_dependencyResolver);
    }
}
