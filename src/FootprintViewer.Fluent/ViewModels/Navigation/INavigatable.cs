namespace FootprintViewer.Fluent.ViewModels.Navigation;

public interface INavigatable
{
    void OnNavigatedTo(bool isInHistory);

    void OnNavigatedFrom(bool isInHistory);
}
