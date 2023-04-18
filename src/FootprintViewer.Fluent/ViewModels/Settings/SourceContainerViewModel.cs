﻿using FootprintViewer.AppStates;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Sources;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using FootprintViewer.Fluent.ViewModels.Settings.SourceBuilders;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels.Settings;

public sealed class SourceContainerViewModel : ViewModelBase
{
    public SourceContainerViewModel(DialogViewModelBase<object> dialog, string key)
    {
        Sources = new List<ISourceViewModel>();

        Remove = ReactiveCommand.Create<ISourceViewModel>(RemoveImpl, outputScheduler: RxApp.MainThreadScheduler);

        DatabaseBuilderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = Services.MainState;
            var state = mainState.LastOpenDatabase;

            var databaseBuilderDialog = new DatabaseBuilderViewModel(key)
            {
                Host = state?.Host ?? "localhost",
                Database = state?.Database ?? "FootprintViewerDatabase",
                Username = state?.Username ?? "postgres",
                Password = state?.Password ?? "user"
            };

            var dialogResult = await dialog.NavigateDialogAsync(databaseBuilderDialog);

            if (dialogResult.Result is DatabaseSource source)
            {
                AddSource(new SourceViewModel(source));

                mainState.LastOpenDatabase = new LastDatabaseState()
                {
                    Host = databaseBuilderDialog.Host,
                    Database = databaseBuilderDialog.Database,
                    Username = databaseBuilderDialog.Username,
                    Password = databaseBuilderDialog.Password
                };
            }
        });

        JsonBuilderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = Services.MainState;

            var jsonBuilderDialog = new JsonBuilderViewModel(key)
            {
                Directory = mainState.LastOpenDirectory
            };

            var dialogResult = await dialog.NavigateDialogAsync(jsonBuilderDialog);

            if (dialogResult.Result is JsonSource source)
            {
                AddSource(new SourceViewModel(source));

                mainState.LastOpenDirectory = jsonBuilderDialog.Directory;
            }
        });

        if (DbHelper.IsKeyEquals(key, DbKeys.UserGeometries) == true)
        {
            MenuItems = new[]
            {
                new MenuItemViewModel
                {
                    Header = ".database",
                    Command = DatabaseBuilderCommand,
                }
            };
        }
        else
        {
            MenuItems = new[]
            {
                new MenuItemViewModel
                {
                    Header = ".database",
                    Command = DatabaseBuilderCommand,
                },
                new MenuItemViewModel
                {
                    Header = ".json",
                    Command = JsonBuilderCommand,
                }
            };
        }
    }

    private void RemoveImpl(ISourceViewModel source)
    {
        var list = Sources.ToList();

        var index = list.FindIndex(s => string.Equals(s.Name, source.Name));

        list.RemoveAt(index);

        SourceOperationsStack.Push((source.Source, false));

        Sources = new List<ISourceViewModel>(list);
    }

    private void AddSource(ISourceViewModel source)
    {
        var list = Sources.ToList();
        var number = Min(list);

        source.Name = $"Source{number}";

        list.Add(source);

        SourceOperationsStack.Push((source.Source, true));

        Sources = new List<ISourceViewModel>(list);
    }

    public Stack<(ISource, bool)> SourceOperationsStack { get; } = new();

    private static int Min(List<ISourceViewModel> list)
    {
        var list1 = list.Where(s => s.Name?.Contains("Source") ?? false).Select(s => s.Name!).ToList();
        var list2 = list1.Select(s => s.Replace("Source", "")).ToList();
        var numbers = list2.Where(s => int.TryParse(s, out var _)).Select(s => int.Parse(s)).Where(s => s > 0).ToList();

        int min = 1;

        numbers.Sort();

        foreach (var item in numbers)
        {
            if (item != min)
            {
                return min;
            }
            else
            {
                min++;
            }
        }

        return min;
    }

    public ReactiveCommand<ISourceViewModel, Unit> Remove { get; set; }

    [Reactive]
    public string? Header { get; set; }

    [Reactive]
    public IList<ISourceViewModel> Sources { get; set; }

    public ICommand DatabaseBuilderCommand { get; }

    public ICommand JsonBuilderCommand { get; }

    public IReadOnlyList<MenuItemViewModel> MenuItems { get; set; }
}
