using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public static class LocatorExtensions
    {
        public static T GetExistingService<T>(this IReadonlyDependencyResolver dependencyResolver)
        {
            return dependencyResolver.GetService<T>() ?? throw new Exception($"Type {typeof(T)} not registered.");
        }
    }
}
