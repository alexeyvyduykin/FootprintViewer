using FootprintViewer.Configurations;
using FootprintViewer.Localization;
using Splat;

namespace FootprintViewer.Factories;

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
}
