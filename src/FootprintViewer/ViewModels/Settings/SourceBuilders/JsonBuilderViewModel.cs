﻿using DynamicData;
using FootprintViewer.Data.DataManager.Sources;
using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using Svg.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace FootprintViewer.ViewModels.Settings.SourceBuilders;

public class JsonBuilderViewModel : DialogViewModelBase<ISource>
{
    private readonly string _key;
    private readonly SourceList<FileViewModel> _targetList = new();
    private readonly SourceList<FileViewModel> _availableList = new();
    private readonly ReadOnlyObservableCollection<FileViewModel> _targetFiles;
    private readonly ReadOnlyObservableCollection<FileViewModel> _availableFiles;

    public JsonBuilderViewModel(string key)
    {
        _key = key;

        var list1 = new List<FileViewModel>();
        var list2 = new List<FileViewModel>();

        //var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        //var res = assets?.GetAssets(new Uri("avares://DataSettingsSample/Assets/"), null).ToList() ?? new List<Uri>();

        //foreach (var uri in res)
        //{
        //    if (Equals(Path.GetExtension(uri.LocalPath), ".json") == true)
        //    {
        //        if (new Random().Next(0, 2) == 1)
        //        {
        //            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        //            var filename = Path.GetFileName(uri.LocalPath);
        //            var fullPath = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename));
        //            list1.Add(new FileViewModel(fullPath)
        //            {
        //                Name = filename,
        //                IsSelected = new Random().Next(0, 2) == 1,
        //            });
        //        }
        //        else
        //        {
        //            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        //            var filename = Path.GetFileName(uri.LocalPath);
        //            var fullPath = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename));
        //            var file = new FileViewModel(fullPath)
        //            {
        //                Name = filename,
        //                IsSelected = new Random().Next(0, 2) == 1,
        //            };

        //            file.Verified(_key);

        //            list2.Add(file);
        //        }
        //    }
        //}

        _availableList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _availableFiles)
            .Subscribe();

        _targetList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _targetFiles)
            .Subscribe();

        AddToAvailableList(list1);
        AddToTargetList(list2);

        var nextCommandCanExecute = TargetFiles
            .ToObservableChangeSet()
            .AutoRefresh(s => s.IsVerified)
            .ToCollection()
            .Select(s => (s.Count != 0) && s.All(d => d.IsVerified));

        Update = ReactiveCommand.Create<string?>(UpdateImpl, outputScheduler: RxApp.MainThreadScheduler);
        ToTarget = ReactiveCommand.Create(ToTargetImpl, outputScheduler: RxApp.MainThreadScheduler);
        FromTarget = ReactiveCommand.Create(FromTargetImpl, outputScheduler: RxApp.MainThreadScheduler);

        this.WhenAnyValue(s => s.Directory).InvokeCommand(Update);

        // dialog
        EnableCancel = true;
        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back, CreateSource()), nextCommandCanExecute);
        CancelCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back));
    }

    private ISource CreateSource()
    {
        return new JsonSource(_key, TargetFiles.Where(s => !string.IsNullOrEmpty(s.Path)).Select(s => s.Path!).ToList());
    }

    protected void AddToAvailableList(IList<FileViewModel> list)
    {
        _availableList.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    protected void AddToTargetList(IList<FileViewModel> list)
    {
        _targetList.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    private void UpdateImpl(string? directory)
    {
        if (directory != null)
        {
            var paths = System.IO.Directory.GetFiles(directory, "*.json").Select(Path.GetFullPath);

            var list = new List<FileViewModel>();

            foreach (var path in paths)
            {
                var file = new FileViewModel(path)
                {
                    Name = Path.GetFileName(path),
                    IsSelected = false,
                };

                list.Add(file);
            }

            AddToAvailableList(list);

            AddToTargetList(new List<FileViewModel>());
        }
    }

    private void ToTargetImpl()
    {
        var list1 = AvailableFiles.ToList();
        var list2 = TargetFiles.ToList();

        var listFalse = list1.Where(s => s.IsSelected == false).ToList();
        var listTrue = list1.Where(s => s.IsSelected == true).ToList();

        listTrue.ForEach(s => s.IsSelected = false);
        listTrue.ForEach(s => s.Verified(_key));

        list2.AddRange(listTrue);

        AddToAvailableList(listFalse);

        AddToTargetList(list2);
    }

    private void FromTargetImpl()
    {
        var list1 = AvailableFiles.ToList();
        var list2 = TargetFiles.ToList();

        var listFalse = list2.Where(s => s.IsSelected == false).ToList();
        var listTrue = list2.Where(s => s.IsSelected == true).ToList();

        listTrue.ForEach(s => s.IsSelected = false);

        list1.AddRange(listTrue);

        AddToAvailableList(list1);

        AddToTargetList(listFalse);
    }

    private ReactiveCommand<string?, Unit> Update { get; }

    public ReactiveCommand<Unit, Unit> ToTarget { get; }

    public ReactiveCommand<Unit, Unit> FromTarget { get; }

    public ReadOnlyObservableCollection<FileViewModel> AvailableFiles => _availableFiles;

    public ReadOnlyObservableCollection<FileViewModel> TargetFiles => _targetFiles;

    [Reactive]
    public string? Directory { get; set; }
}