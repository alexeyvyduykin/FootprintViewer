using Splat;
using System;

namespace DataSettingsSample
{
    public static class LocatorExtensions
    {
        public static T GetExistingService<T>(this IReadonlyDependencyResolver dependencyResolver)
        {
            return dependencyResolver.GetService<T>() ?? throw new Exception($"Type {typeof(T)} not registered.");
        }
    }
}
