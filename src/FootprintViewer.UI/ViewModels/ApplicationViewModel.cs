using FootprintViewer.UI.Services2;
using ReactiveUI;
using System.Windows.Input;

namespace FootprintViewer.UI.ViewModels;

public class ApplicationViewModel : ViewModelBase
{
    public ApplicationViewModel()
    {
        QuitCommand = ReactiveCommand.Create(() => Shutdown());
    }

    public void Shutdown()
    {
        var appService = Services.Locator.GetRequiredService<ApplicationService>();

        appService.Exit();
    }

    public ICommand QuitCommand { get; }
}
