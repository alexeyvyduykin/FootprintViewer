using FootprintViewer.Data.DbContexts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.Settings.SourceBuilders;

public class FileViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<bool> _isVerified;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private bool _dirty = true;
    private readonly string _path;

    public FileViewModel(string path)
    {
        _path = path;

        Command = ReactiveCommand.CreateFromTask<string, bool>(GetModelAsync, outputScheduler: RxApp.MainThreadScheduler);

        _isVerified = Command
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsVerified);

        _isLoading = Command.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);
    }

    private ReactiveCommand<string, bool> Command { get; }

    private async Task<bool> GetModelAsync(string key)
    {
        if (_path == null)
        {
            return false;
        }

        await Observable
            .Return(Unit.Default)
            .Delay(TimeSpan.FromSeconds(1));

        return await DbHelper.JsonValidationAsync(key, _path);
    }

    public void Verified(string key)
    {
        if (_dirty == true)
        {
            Command.Execute(key).Subscribe();

            _dirty = false;
        }
    }

    public bool IsVerified => _isVerified.Value;

    public bool IsLoading => _isLoading.Value;

    [Reactive]
    public string? Name { get; set; }

    public string Path => _path;

    [Reactive]
    public bool IsSelected { get; set; }
}
