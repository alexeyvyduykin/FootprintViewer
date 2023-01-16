using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.DataManager.Sources;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.Settings.SourceBuilders;

public sealed class JsonBuilderViewModel : DialogViewModelBase<ISource>
{
    private readonly string _key;
    private readonly SourceList<FileViewModel> _targetList = new();
    private readonly SourceList<FileViewModel> _availableList = new();
    private readonly ReadOnlyObservableCollection<FileViewModel> _targetFiles;
    private readonly ReadOnlyObservableCollection<FileViewModel> _availableFiles;

    public JsonBuilderViewModel(string key)
    {
        _key = key;

        _availableList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _availableFiles)
            .Subscribe();

        _targetList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _targetFiles)
            .Subscribe();

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
