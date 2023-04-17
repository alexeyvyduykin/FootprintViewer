using ReactiveUI;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels;

public class ApplicationViewModel : ViewModelBase
{
    private readonly IMainWindowService _mainWindowService;

    public ApplicationViewModel(IMainWindowService mainWindowService)
    {
        _mainWindowService = mainWindowService;

        QuitCommand = ReactiveCommand.Create(() => Shutdown());
    }

    public void Shutdown() => _mainWindowService.Shutdown();

    public ICommand QuitCommand { get; }
}
