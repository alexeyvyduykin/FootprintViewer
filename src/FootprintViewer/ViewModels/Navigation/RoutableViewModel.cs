using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.ViewModels.Navigation;

public abstract class RoutableViewModel : ViewModelBase, INavigatable
{
    private CompositeDisposable? _currentDisposable;

    public NavigationTarget CurrentTarget { get; internal set; }

    public virtual NavigationTarget DefaultTarget => NavigationTarget.HomeScreen;

    protected RoutableViewModel()
    {
        BackCommand = ReactiveCommand.Create(() => Navigate().Back());
        CancelCommand = ReactiveCommand.Create(() => Navigate().Clear());
    }

    [Reactive]
    public bool EnableBack { get; set; }

    [Reactive]
    public bool EnableCancel { get; set; }

    [Reactive]
    public bool IsActive { get; set; }

    public ICommand? NextCommand { get; protected set; }

    public ICommand BackCommand { get; protected set; }

    public ICommand CancelCommand { get; protected set; }

    public INavigationStack<RoutableViewModel> Navigate()
    {
        var currentTarget = CurrentTarget == NavigationTarget.Default ? DefaultTarget : CurrentTarget;

        return Navigate(currentTarget);
    }

    public static INavigationStack<RoutableViewModel> Navigate(NavigationTarget currentTarget)
    {
        return currentTarget switch
        {
            NavigationTarget.HomeScreen => NavigationState.Instance.HomeScreenNavigation,
            NavigationTarget.DialogScreen => NavigationState.Instance.DialogScreenNavigation,
            NavigationTarget.FullScreen => NavigationState.Instance.FullScreenNavigation,
            _ => throw new NotSupportedException(),
        };
    }

    public void SetActive()
    {
        if (NavigationState.Instance.HomeScreenNavigation.CurrentPage is { } homeScreen)
        {
            homeScreen.IsActive = false;
        }

        if (NavigationState.Instance.DialogScreenNavigation.CurrentPage is { } dialogScreen)
        {
            dialogScreen.IsActive = false;
        }

        if (NavigationState.Instance.FullScreenNavigation.CurrentPage is { } fullScreen)
        {
            fullScreen.IsActive = false;
        }

        IsActive = true;
    }

    void INavigatable.OnNavigatedFrom(bool isInHistory)
    {
        DoNavigateFrom(isInHistory);
    }

    private void DoNavigateFrom(bool isInHistory)
    {
        OnNavigatedFrom(isInHistory);

        _currentDisposable?.Dispose();
        _currentDisposable = null;
    }

    protected virtual void OnNavigatedFrom(bool isInHistory) { }

    public void OnNavigatedTo(bool isInHistory)
    {
        DoNavigateTo(isInHistory);
    }

    private void DoNavigateTo(bool isInHistory)
    {
        if (_currentDisposable is { })
        {
            throw new Exception("Can't navigate to something that has already been navigated to.");
        }

        _currentDisposable = new CompositeDisposable();

        OnNavigatedTo(isInHistory, _currentDisposable);
    }

    protected virtual void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables) { }

    public async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(DialogViewModelBase<TResult> dialog)
    => await NavigateDialogAsync(dialog, CurrentTarget);

    public static async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(DialogViewModelBase<TResult> dialog, NavigationTarget target, NavigationMode navigationMode = NavigationMode.Normal)
    {
        var dialogTask = dialog.GetDialogResultAsync();

        Navigate(target).To(dialog, navigationMode);

        var result = await dialogTask;

        Navigate(target).Back();

        return result;
    }
}