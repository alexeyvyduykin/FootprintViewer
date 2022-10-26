using FootprintViewer.ViewModels.Navigation;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels.Dialogs;

public class DialogNavigationStack : NavigationStack<RoutableViewModel>
{
    [Reactive]
    public bool IsDialogOpen { get; set; }

    protected override void OnNavigated(RoutableViewModel? oldPage, RoutableViewModel? newPage)
    {
        base.OnNavigated(oldPage, newPage);

        if (oldPage is { } && oldPage != newPage)
        {
            oldPage.IsActive = false;
        }

        IsDialogOpen = CurrentPage is not null;
    }
}