﻿using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class UserGeometryTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<UserGeometry> _userGeometries = new();
    private readonly ReadOnlyObservableCollection<UserGeometryViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public UserGeometryTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        Title = "Пользовательская геометрия";

        _userGeometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new UserGeometryViewModel(s))
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        Remove = ReactiveCommand.CreateFromTask<UserGeometryViewModel?>(RemoveAsync);

        _isLoading = Update.IsExecuting
              .ObserveOn(RxApp.MainThreadScheduler)
              .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .ToSignal()
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<UserGeometryViewModel?, Unit> Remove { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<UserGeometry>(DbKeys.UserGeometries.ToString());

        _userGeometries.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
    }

    private async Task RemoveAsync(UserGeometryViewModel? value)
    {
        if (value != null)
        {
            await _dataManager.TryRemoveAsync(DbKeys.UserGeometries.ToString(), value.Model);
        }
    }

    public async Task<List<UserGeometryViewModel>> GetUserGeometryViewModelsAsync(string name)
    {
        var res = await _dataManager.GetDataAsync<UserGeometry>(DbKeys.UserGeometries.ToString());

        return res
            .Where(s => Equals(s.Name, name))
            .Select(s => new UserGeometryViewModel(s))
            .ToList();
    }

    public ReadOnlyObservableCollection<UserGeometryViewModel> Items => _items;
}
