﻿using DataSettingsSample.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels;

public class FileViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<bool> _isVerified;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private bool _dirty = true;
    private readonly string _path;

    public FileViewModel(string path)
    {
        _path = path;

        Command = ReactiveCommand.CreateFromTask<DbKeys, bool>(GetModelAsync, outputScheduler: RxApp.MainThreadScheduler);

        _isVerified = Command
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsVerified);

        _isLoading = Command.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);
    }

    private ReactiveCommand<DbKeys, bool> Command { get; }

    private async Task<bool> GetModelAsync(DbKeys key)
    {
        if (_path == null)
        {
            return false;
        }

        await Task.Delay(TimeSpan.FromSeconds(1));

        string jsonString;

        try
        {
            jsonString = await File.ReadAllTextAsync(_path);
        }
        catch (Exception)
        {

            return false;
        }

        return DbHelper.JsonValidation(key, jsonString);
    }

    public void Verified(DbKeys key)
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
