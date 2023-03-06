namespace FootprintViewer.ViewModels.Navigation;

public interface INavigationStack<T> where T : INavigatable
{
    T? CurrentPage { get; }

    bool CanNavigateBack { get; }

    void To(T viewmodel, NavigationMode mode = NavigationMode.Normal);

    void Back();

    void BackTo(T viewmodel);

    void BackTo<TViewModel>() where TViewModel : T;

    void Clear();
}