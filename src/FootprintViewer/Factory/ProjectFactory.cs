using FootprintViewer.Configurations;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Mapsui;
using Splat;

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

    public IMapNavigator CreateMapNavigator(Map map)
    {
        return new MapNavigator(map);
    }

    public ScaleMapBar CreateScaleMapBar()
    {
        return new ScaleMapBar();
    }
}
