﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.SidePanel;

public sealed class SidePanelViewModel : ViewModelBase
{
    public SidePanelViewModel()
    {
        this.WhenAnyValue(s => s.SelectedTab).Subscribe(tab =>
        {
            if (tab != null)
            {
                foreach (var item in Tabs)
                {
                    item.IsActive = false;

                    if (item == tab)
                    {
                        item.IsActive = true;
                    }
                }
            }
        });

        SelectedTab = Tabs.FirstOrDefault();

        IsExpandedChanged = ReactiveCommand.Create<bool>(IsExpandedImpl);

        this.WhenAnyValue(s => s.IsExpanded).InvokeCommand(IsExpandedChanged);

        IsExpanded = true;
    }

    private void IsExpandedImpl(bool value)
    {
        foreach (var item in Tabs)
        {
            item.IsExpanded = value;
        }
    }

    private ReactiveCommand<bool, Unit> IsExpandedChanged { get; }

    [Reactive]
    public bool IsExpanded { get; set; }

    public List<SidePanelTabViewModel> Tabs { get; set; } = new();

    public List<SidePanelActionTabViewModel> ActionTabs { get; set; } = new();

    [Reactive]
    public SidePanelTabViewModel? SelectedTab { get; set; }
}