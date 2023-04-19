using FootprintViewer.Fluent.ViewModels.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.Dialogs;

public class DialogScreenViewModel : TargettedNavigationStack
{
    [Reactive]
    public bool IsDialogOpen { get; set; }

    [Reactive]
    public bool ShowAlert { get; set; }

    public DialogScreenViewModel(NavigationTarget navigationTarget = NavigationTarget.DialogScreen) : base(navigationTarget)
    {
        this.WhenAnyValue(x => x.IsDialogOpen)
            .Skip(1) // Skip the initial value change (which is false).
            .DistinctUntilChanged()
            .Subscribe(s =>
            {
                if (s == false)
                {
                    CloseScreen();
                }
            });
    }

    protected override void OnNavigated(RoutableViewModel? oldPage, bool oldInStack, RoutableViewModel? newPage, bool newInStack)
    {
        base.OnNavigated(oldPage, oldInStack, newPage, newInStack);

        IsDialogOpen = CurrentPage is not null;
    }

    private static void CloseDialogs(IEnumerable<RoutableViewModel> navigationStack)
    {
        // Close all dialogs so the awaited tasks can complete.
        // - DialogViewModelBase.ShowDialogAsync()
        // - DialogViewModelBase.GetDialogResultAsync()

        foreach (var routable in navigationStack)
        {
            if (routable is DialogViewModelBase dialog)
            {
                dialog.IsDialogOpen = false;
            }
        }
    }

    private void CloseScreen()
    {
        var navStack = Stack.ToList();
        Clear();

        CloseDialogs(navStack);
    }
}
