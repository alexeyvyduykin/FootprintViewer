using FootprintViewer.Layers;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;

namespace FootprintViewer;

public class ViewModelFactory
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;

    public ViewModelFactory(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    public GroundStationTabViewModel CreateGroundStationTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new GroundStationTabViewModel(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Update);

        return tab;
    }

    public FootprintPreviewTabViewModel CreateFootprintPreviewTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new FootprintPreviewTabViewModel(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Update);

        return tab;
    }

    public FootprintTabViewModel CreateFootprintTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new FootprintTabViewModel(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Update);

        return tab;
    }

    public GroundTargetTabViewModel CreateGroundTargetTab()
    {
        var map = _dependencyResolver.GetExistingService<IMap>();
        var layer = map.GetLayer<Layer>(LayerType.GroundTarget);
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        var targetManager = layer?.BuildManager(() => ((TargetLayerSource)layer.DataSource!).GetFeatures());
        var tab = new GroundTargetTabViewModel(_dependencyResolver);

        tab.SelectedItemObservable.Subscribe(groundTarget =>
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    targetManager?.SelectFeature(name);
                }
            }
        });

        tab.Enter.Subscribe(groundTarget =>
        {
            var name = groundTarget.Name;

            if (name != null)
            {
                targetManager?.ShowHighlight(name);
            }
        });

        tab.Leave.Subscribe(_ =>
        {
            targetManager?.HideHighlight();
        });

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public SatelliteTabViewModel CreateSatelliteTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new SatelliteTabViewModel(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public UserGeometryTabViewModel CreateUserGeometryTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        var tab = new UserGeometryTabViewModel(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }
}
