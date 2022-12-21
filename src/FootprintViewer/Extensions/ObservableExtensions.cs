using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer;

public static class ObservableExtensions
{
    public static IObservable<Unit> ToSignal<T>(this IObservable<T> observable)
    {
        return observable.Select(_ => Unit.Default);
    }

    public static IObservable<bool> WhereTrue(this IObservable<bool> observable)
    {
        return observable.Where(s => s == true);
    }

    public static IObservable<bool> WhereFalse(this IObservable<bool> observable)
    {
        return observable.Where(s => s == false);
    }

    public static bool? AllPropertyCheck<T>(this IEnumerable<T> collection, Func<T, bool> propertyAccesor)
    {
        var allTrue = collection.All(s => propertyAccesor.Invoke(s) == true);
        var allFalse = collection.All(s => propertyAccesor.Invoke(s) == false);
        var anyFalse = collection.Any(s => propertyAccesor.Invoke(s) == false);

        if (allTrue == true)
        {
            return true;
        }
        else if (allFalse == true)
        {
            return false;
        }
        else if (anyFalse == true)
        {
            return null;
        }

        return null;
    }

    public static bool? AllPropertyCheck(this IEnumerable<bool> collection)
    {
        var allTrue = collection.All(s => s == true);
        var allFalse = collection.All(s => s == false);
        var anyFalse = collection.Any(s => s == false);

        if (allTrue == true)
        {
            return true;
        }
        else if (allFalse == true)
        {
            return false;
        }
        else if (anyFalse == true)
        {
            return null;
        }

        return null;
    }

    public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items)
        {
            action(item);
        }

        return items;
    }
}
