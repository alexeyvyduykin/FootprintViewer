using FootprintViewer.Fluent.ViewModels.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.Dialogs;

public abstract partial class DialogViewModelBase : RoutableViewModel
{
    [Reactive]
    public bool IsDialogOpen { get; set; }
}

public abstract class DialogViewModelBase<TResult> : DialogViewModelBase
{
    private readonly IDisposable _disposable;
    private TaskCompletionSource<DialogResult<TResult>>? _currentTaskCompletionSource;

    protected DialogViewModelBase()
    {
        _currentTaskCompletionSource = new TaskCompletionSource<DialogResult<TResult>>();

        _disposable = this.WhenAnyValue(x => x.IsDialogOpen)
                          .Skip(1) // Skip the initial value change (which is false).
                          .DistinctUntilChanged()
                          .Subscribe(OnIsDialogOpenChanged);

        BackCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back));

        CancelCommand = ReactiveCommand.Create(() =>
        {
            Close(DialogResultKind.Cancel);
            Navigate().Clear();
        });
    }

    private void OnIsDialogOpenChanged(bool dialogState)
    {
        // Triggered when closed abruptly (via the dialog overlay or the back button).
        if (dialogState == false)
        {
            Close();
        }
    }

    protected override void OnNavigatedFrom(bool isInHistory)
    {
        if (isInHistory == false)
        {
            Close(DialogResultKind.Cancel);
        }

        base.OnNavigatedFrom(isInHistory);
    }

    /// <summary>
    /// Method to be called when the dialog intends to close
    /// and ready to pass a value back to the caller.
    /// </summary>
    /// <param name="result">The return value of the dialog</param>
    protected void Close(DialogResultKind kind = DialogResultKind.Normal, TResult? result = default)
    {
        if (_currentTaskCompletionSource!.Task.IsCompleted)
        {
            return;
        }

        _currentTaskCompletionSource.SetResult(new DialogResult<TResult>(result, kind));

        _disposable.Dispose();

        _currentTaskCompletionSource = new TaskCompletionSource<DialogResult<TResult>>();

        IsDialogOpen = false;

        OnDialogClosed();
    }

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    /// <returns>The value to be returned when the dialog is finished.</returns>
    public Task<DialogResult<TResult>> GetDialogResultAsync()
    {
        IsDialogOpen = true;

        return _currentTaskCompletionSource!.Task;
    }

    /// <summary>         
    /// Method that is triggered when the dialog is closed.         
    /// </summary>
    protected virtual void OnDialogClosed() { }
}