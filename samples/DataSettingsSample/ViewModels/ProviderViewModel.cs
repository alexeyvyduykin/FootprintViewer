﻿using DataSettingsSample.Data;
using DataSettingsSample.ViewModels.Interfaces;
using DataSettingsSample.ViewModels.SourceBuilders;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DataSettingsSample.ViewModels;

public class MenuItemViewModel
{
    public string? Header { get; set; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public IList<MenuItemViewModel>? Items { get; set; }
}

public class ProviderViewModel : ReactiveObject
{
    public ProviderViewModel(DialogViewModelBase<IList<DbKeys>> dialog, DbKeys key)
    {
        Sources = new List<ISourceViewModel>();

        Remove = ReactiveCommand.Create<ISourceViewModel>(RemoveImpl, outputScheduler: RxApp.MainThreadScheduler);

        DatabaseBuilderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var databaseBuilderDialog = new DatabaseBuilderViewModel(key);

            var dialogResult = await dialog.NavigateDialogAsync(databaseBuilderDialog);

            if (dialogResult.Result is DatabaseSource source)
            {
                AddSource(new SourceViewModel(source));
            }
        });

        JsonBuilderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var jsonBuilderDialog = new JsonBuilderViewModel(key);

            var dialogResult = await dialog.NavigateDialogAsync(jsonBuilderDialog);

            if (dialogResult.Result is JsonSource source)
            {
                AddSource(new SourceViewModel(source));
            }
        });

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
