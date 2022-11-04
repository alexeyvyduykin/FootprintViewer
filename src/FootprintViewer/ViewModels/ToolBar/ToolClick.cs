using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FootprintViewer.Models;
using ReactiveUI;

namespace FootprintViewer.ViewModels;

public class ToolClick : ReactiveObject, IToolClick
{
    private Func<Task> _click = () => Task.Run(() => { });

    public ToolClick()
    {
        Click = ReactiveCommand.CreateFromTask(() => _click(), outputScheduler: RxApp.MainThreadScheduler);
    }

    public void SubscribeAsync(Func<Task> click)
    {
        _click = click;
    }

    public string GetKey() => (string?)Tag ?? string.Empty;

    public object? Tag { get; set; }

    public ICommand Click { get; }
}
