﻿using FootprintViewer.Fluent.ViewModels.SidePanel.Items;

namespace FootprintViewer.Fluent.ViewModels.InfoPanel;

public sealed class GroundTargetInfoPanelItemViewModel : InfoPanelItemViewModel
{
    public static GroundTargetInfoPanelItemViewModel Create(GroundTargetViewModel gt) => new(gt);

    public GroundTargetInfoPanelItemViewModel()
    {
        Key = "GroundTarget";
    }

    private GroundTargetInfoPanelItemViewModel(GroundTargetViewModel groundTargetViewModel) : this()
    {
        Text = groundTargetViewModel.Name;

        TypeInfo = groundTargetViewModel.Type.ToString();
    }

    public string? TypeInfo { get; set; }
}
