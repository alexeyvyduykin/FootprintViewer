using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Factories;

public class TaskLoader : ReactiveObject
{
    private readonly List<Func<Task>> _actions = new();

    public void AddTaskAsync(Func<Task> actionAsync)
    {
        _actions.Add(actionAsync);
    }

    public void Run()
    {
        foreach (var item in _actions)
        {
            Observable.StartAsync(item, RxApp.MainThreadScheduler);
        }
    }
}
