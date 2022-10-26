using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;

namespace FootprintViewer.ViewModels.Navigation;

public abstract class RoutableViewModel : ViewModelBase
{
    private static readonly DialogNavigationStack _dialogStack = new();

    protected RoutableViewModel()
    {
        BackCommand = ReactiveCommand.Create(() => DialogStack().Back());
        CancelCommand = ReactiveCommand.Create(() => DialogStack().Clear());
    }

    public ICommand? NextCommand { get; protected set; }

    public ICommand BackCommand { get; protected set; }

    public ICommand CancelCommand { get; protected set; }

    public static DialogNavigationStack DialogStack() => _dialogStack;

    public void SetActive()
    {
        if (_dialogStack.CurrentPage is { } dialog)
        {
            dialog.IsActive = false;
        }

        IsActive = true;
    }

    [Reactive]
    public bool EnableBack { get; set; }

    [Reactive]
    public bool EnableCancel { get; set; }

    [Reactive]
    public bool IsActive { get; set; }
}

