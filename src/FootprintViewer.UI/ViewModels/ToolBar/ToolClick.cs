﻿using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public class ToolClick : ViewModelBase, IToolClick
{
    private Func<Task> _click = async () => await Observable.Start(() => { }, RxApp.TaskpoolScheduler);

    public ToolClick()
    {
        Click = ReactiveCommand.CreateFromTask(() => _click(), outputScheduler: RxApp.MainThreadScheduler);
    }

    public void SubscribeAsync(Action click)
    {
        _click = async () => await Observable.Start(click, RxApp.TaskpoolScheduler); 
        Key = (string?)Tag ?? string.Empty;
    }

    public string? Key { get; set; }// GetKey() => (string?)Tag ?? string.Empty;

    public object? Tag { get; set; }

    public ICommand Click { get; }
}
