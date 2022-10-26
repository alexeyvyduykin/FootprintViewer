using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace FootprintViewer.ViewModels.Navigation;

public class NavigationStack<T> : ViewModelBase, INavigationStack<T> where T : class
{
    private readonly Stack<T> _backStack;

    protected NavigationStack()
    {
        _backStack = new Stack<T>();
    }

    [Reactive]
    public T? CurrentPage { get; set; }

    protected virtual void OnNavigated(T? oldPage, T? newPage) { }

    public virtual void Clear()
    {
        if (_backStack.Count == 0 && CurrentPage is null)
        {
            return;
        }

        var oldPage = CurrentPage;

        _backStack.Clear();

        CurrentPage = null;

        OnNavigated(oldPage, CurrentPage);
    }

    public void To(T viewmodel)
    {
        var oldPage = CurrentPage;

        if (oldPage is { })
        {
            _backStack.Push(oldPage);
        }

        CurrentPage = viewmodel;

        OnNavigated(oldPage, CurrentPage);
    }

    public void Back()
    {
        if (_backStack.Count > 0)
        {
            var oldPage = CurrentPage;

            CurrentPage = _backStack.Pop();

            OnNavigated(oldPage, CurrentPage);
        }
        else
        {
            Clear(); // in this case only CurrentPage might be set and Clear will provide correct behavior.
        }
    }
}
