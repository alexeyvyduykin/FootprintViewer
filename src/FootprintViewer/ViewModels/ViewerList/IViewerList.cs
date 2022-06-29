using FootprintViewer.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public interface IViewerList<T>
    {
        void FiringUpdate(string[]? names, double seconds = 1.0);

        void FiringUpdate(IFilter<T>? filter, double seconds = 1.0);

        ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        ReactiveCommand<T, T> Select { get; }

        ReactiveCommand<T, T> Unselect { get; }

        ReactiveCommand<T, T> MouseOverEnter { get; }

        ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

        ReactiveCommand<T?, Unit> Add { get; }

        ReactiveCommand<T?, Unit> Remove { get; }

        void ClickOnItem(T? item);

        void SelectItem(string name);

        T? GetItem(string name);

        IObservable<T?> SelectedItemObservable { get; }

        bool IsLoading { get; }

        T? SelectedItem { get; set; }

        List<T> Items { get; }
    }
}
