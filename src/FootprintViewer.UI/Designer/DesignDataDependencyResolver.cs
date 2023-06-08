using FootprintViewer.Builders;
using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Models;
using FootprintViewer.Services;
using FootprintViewer.UI.Services2;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.Designer;

public sealed class DesignDataDependencyResolver : IServiceProvider
{
    private ILocalStorageService? _localStorage;
    private IMapService? _mapService;

    public T GetService<T>()
    {
        return (T)GetService(typeof(T))!;
    }

    public object? GetService(Type? serviceType)
    {
        if (serviceType == typeof(IMapService))
        {
            return _mapService ??= new MapService();
        }
        else if (serviceType == typeof(ILocalStorageService))
        {
            return _localStorage ??= CreateLocalStorage();
        }
        throw new Exception();
    }

    private static ILocalStorageService CreateLocalStorage()
    {
        var source5 = new LocalSource<UserGeometry>(BuildUserGeometries);
        var source6 = new LocalSource<MapResource>(BuildMapResources);
        var source7 = new LocalSource<FootprintPreview>(BuildFootprintPreviews);
        var source8 = new LocalSource<FootprintPreviewGeometry>(BuildFootprintPreviewGeometries);
        var source9 = new LocalSource<PlannedScheduleResult>(BuildPlannedSchedule);

        var dir = Directory.GetCurrentDirectory();

        var filesource1 = new FileSource(new[] { Path.Combine(dir, "map_topo_4343.mbtiles") }, MapResource.Build);
        var filesource2 = new FileSource(new[] { Path.Combine(dir, "world.mbtiles") }, MapResource.Build);
        var filesource3 = new FileSource(new[] { Path.Combine(dir, "WorlMapWithCountryBorders.mbtiles") }, MapResource.Build);
        var filesource4 = new FileSource(new[] { Path.Combine(dir, "MapBackground_Mercator.mbtiles") }, MapResource.Build);

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { DbKeys.UserGeometries.ToString(), new[] { source5 } },
            { DbKeys.Maps.ToString(), new[] { filesource1, filesource2, filesource3, filesource4 } },
            { DbKeys.FootprintPreviews.ToString(), new[] { source7 } },
            { DbKeys.FootprintPreviewGeometries.ToString(), new[] { source8 } },
            { DbKeys.PlannedSchedules.ToString(), new[] { source9 } }
        };

        var localStorage = new LocalStorageService();

        localStorage.RegisterSources(sources);

        return localStorage;
    }

    private static List<T> Build<T>(int count, Func<T> func)
    {
        var tasks = new int[count]
            .Select(_ => Task<T>.Factory.StartNew(() => func()))
            .ToArray();

        Task.WaitAll(tasks);

        return tasks.Select(s => s.Result).ToList();
    }

    private static List<PlannedScheduleResult> BuildPlannedSchedule() => Build(1, PlannedScheduleBuilder.CreateRandom);

    private static List<UserGeometry> BuildUserGeometries() => Build(10, UserGeometryBuilder.CreateRandom);

    private static List<MapResource> BuildMapResources() =>
        new()
        {
                new MapResource("WorldMapDefault", ""),
                new MapResource("OAM-World-1-8-min-J70", ""),
                new MapResource("OAM-World-1-10-J70", "")
        };

    private static List<FootprintPreview> BuildFootprintPreviews() => Build(8, FootprintPreviewBuilder.CreateRandom);

    private static List<FootprintPreviewGeometry> BuildFootprintPreviewGeometries() =>
        new()
        {
                new FootprintPreviewGeometry() { Name = "WorldMapDefault" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-8-min-J70" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-10-J70" }
        };

    private class LocalSource<T> : ISource
    {
        private readonly Func<List<T>> _func;

        public LocalSource(Func<List<T>> func)
        {
            _func = func;
        }

        public IList<object> GetValues() => _func.Invoke().Cast<object>().ToList();

        public async Task<IList<object>> GetValuesAsync() =>
            await Observable.Start(() => _func.Invoke().Cast<object>().ToList(), RxApp.TaskpoolScheduler);
    }
}
