﻿using System;
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
}
