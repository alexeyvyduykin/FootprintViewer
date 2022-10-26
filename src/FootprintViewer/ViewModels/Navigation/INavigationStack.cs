namespace FootprintViewer.ViewModels.Navigation;

public interface INavigationStack<T>
{
    T? CurrentPage { get; }

    void To(T viewmodel);

    void Back();

    void Clear();
}
