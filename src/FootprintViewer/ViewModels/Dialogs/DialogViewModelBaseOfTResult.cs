﻿using FootprintViewer.ViewModels.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.Dialogs;

public abstract class DialogViewModelBase<TResult> : RoutableViewModel
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
            DialogStack().Clear();
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
    }

    public Task<DialogResult<TResult>> GetDialogResultAsync()
    {
        IsDialogOpen = true;

        return _currentTaskCompletionSource!.Task;
    }

    [Reactive]
    public bool IsDialogOpen { get; set; }
}
