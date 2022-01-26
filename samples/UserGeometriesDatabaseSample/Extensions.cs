using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGeometriesDatabaseSample.Data;
using UserGeometriesDatabaseSample.ViewModels;
using UserGeometriesDatabaseSample.Views;

namespace UserGeometriesDatabaseSample
{
    public static class Extensions
    {
        public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
        {
            var service = resolver.GetService<TService>();
            if (service is null)
            {
                throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
            }

            return service;
        }
    }

}
