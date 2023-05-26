namespace FootprintViewer.UI.ViewModels.Navigation;

public interface INavigatable
{
    void OnNavigatedTo(bool isInHistory);

    void OnNavigatedFrom(bool isInHistory);
}
