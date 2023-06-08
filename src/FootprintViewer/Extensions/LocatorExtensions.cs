namespace FootprintViewer.Extensions;

public static class LocatorExtensions
{
    public static T GetExistingService<T>(this IServiceProvider dependencyResolver)
    {
        return (T)(dependencyResolver.GetService(typeof(T)) ?? throw new Exception($"Type {typeof(T)} not registered."));
    }
}
